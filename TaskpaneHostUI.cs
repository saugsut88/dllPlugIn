using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Diagnostics;

using System.IO;

namespace VPS.SWplugin.ICT
{
    [ProgId(TaskpaneIntegration.SWTASKPANE_PROGID)]
    public partial class TaskpaneHostUI : UserControl
    {
        private Utilities.DBConnector dbConnect = new Utilities.DBConnector();
        private Utilities.EnvironmentalData environmentData = new Utilities.EnvironmentalData();
        private Utilities.BoundingBox boundingBox = new Utilities.BoundingBox();
        private Utilities.ModelDataAggregator modelDataAggregator = new Utilities.ModelDataAggregator();


        //link to propertyFuncs SA 12/6/2021 1517
        private PropertyFunctions propFuncs = new PropertyFunctions();
        // private Utilities.partInformation PartInfo = new Utilities.partInformation();

        // Constants assigned to each tab page's TAG property
        private const string MATERIALTAG = "materials";
        private const string ENVTAG = "environment";
        private const string BOUNDBOXTAG = "boundingbox";

        private string strPartNameCol = "partname";
        private string strMaterialCol = "material";
        private DataTable dtMaterials = new DataTable();

        // Material-Model Options and Environment tab variables
        private bool pBypassIdxChg;
        private int pElectrolyteID = 0;
        private string pElectrolyte;
        private bool pBoolImmersion;
        private string pModelName;

        public double PartVolume;

        /* Keegan's Variables */
        ISldWorks swApp;
        ModelDoc2 swDoc;


        //  private DataTable matchedTable;
        private List<int> newMaterialIDs = new List<int>();

        public TaskpaneHostUI()
        {
            InitializeComponent();

            label2.Parent = gradientPanel1;
            label5.Parent = gradientPanel2;
            label6.Parent = gradientPanel3;
            lblBoundBox.Parent = gradientPanel3;

            // Make the Materials tab the only visible tab during class construction
            HideTabPages(true, true);

            //Load combo box with available materials in the DB
            LoadMaterialTable();

        }  //end constructor   public TaskpaneHostUI()



        #region BoundingBox
     
        private void TestBoundingBox2(double dblExtend)
        {
            string msg = boundingBox.GetBoundingBoxUsingComponents(dblExtend);
            //lbCoordinates.Text = msg;
            modelDataAggregator.BoundingBoxVolume = boundingBox.GetBoundingBoxVolumne();
        }


        private void btnRollBack_Click(object sender, EventArgs e)
        {
            try
            {
                string strMessage = boundingBox.RevertBoundingBox();
                MessageBox.Show(strMessage);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } // end  private void btnRollBack_Click(object sender, EventArgs e)


        private void tbBoundBox_Enter(object sender, EventArgs e)
        {
            TextBox TB = (TextBox)sender;
            int VisibleTime = 1000;  //in milliseconds

            ToolTip tt = new ToolTip();
            tt.Show("Tip: try a small decimal value like .005.", TB, 0, 0, VisibleTime);
        }

        #endregion



        #region EnvironmentTab

        /// <summary>
        /// Load Environment Tab - environment group combo, filtered by material Ids selected in Part-Materal tab
        /// </summary>

        private bool LoadEnvironmentTab()
        {
            string strError = "";
            // create a comma-delimited string of material IDs
            string strOfMaterilIds;
            DataSet ds = new DataSet();
            try
            {
                strOfMaterilIds = string.Join(",", modelDataAggregator.MaterialIDList);
                // Load environment group combo - filter by material ID list options
                ds = environmentData.LoadEnvGoup(ref strError, strOfMaterilIds, modelDataAggregator.GetGalvanic);
                if (strError != string.Empty)
                {
                    MessageBox.Show(strError, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    //check for records
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        pBypassIdxChg = true;
                        cmbEnvGroup.DataSource = ds.Tables[0];
                        cmbEnvGroup.DisplayMember = ds.Tables[0].Columns[1].ToString(); //  env_group_name
                        cmbEnvGroup.ValueMember = ds.Tables[0].Columns[0].ToString(); //  env_group_id
                        cmbEnvGroup.SelectedIndex = -1;
                        pBypassIdxChg = false;
                        tabControl1.SelectedIndex = 0;
                        // hide other panels until needed
                        this.pnlEnvConds.Visible = false;
                        this.pnlElectrolyte.Visible = false;
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("No environment groups have been found, based on your material selections.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
            } // end try
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;

            }
            finally
            {
                if (ds != null)
                    ds.Dispose();
            }
        } // end  private void LoadEnvironmentTab()

        /// <summary>
        /// environment group combo selection trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbEnvGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If just loading the combo skip this event, otherwise exceptions will occur since no selection has been made
            if (!pBypassIdxChg)
            {
                // on change, get the immersion bit for the id. if true show electrolyte and env condition panels, otherwise just the env conditions level 
                if (cmbEnvGroup.SelectedIndex != -1)
                {
                    ClearEnvcondPanels();
                    int envGroupID = Convert.ToInt32(cmbEnvGroup.SelectedValue);
                    pBoolImmersion = false;
                    DataRow dr = environmentData.EnvGroupTable.Rows.Find(envGroupID);
                    if (dr != null)
                    {
                        pBoolImmersion = (bool)dr["immersion"];
                        if (pBoolImmersion)
                        {
                            pElectrolyteID = (int)dr["electrolyte_id"];
                            pElectrolyte = (string)dr["electrolyte"];
                        }
                        else
                        {
                            pElectrolyteID = -1;
                            pElectrolyte = "";
                        }
                        // store the model name
                        pModelName = dr["model_name"].ToString();
                    }
                    // Go load env conditions
                    string strMessage = LoadEnvConditions(envGroupID);
                    if (strMessage == "")
                    {
                        pnlEnvConds.Visible = true;
                        if (pBoolImmersion)
                        {
                            this.tbElectrolyte.Text = pElectrolyte;
                            // go load eletrolyte panel
                            strMessage = LoadElectrolytes();
                            if (strMessage != "")
                            {
                                MessageBox.Show(strMessage, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        pnlSave.Visible = true;
                    }
                    else
                    {
                        // display error
                        MessageBox.Show(strMessage, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } // end   if (cmbEnvGroup.SelectedIndex != -1)
            } // end if (!bypassIdxChg)
        } // env  private void cmbEnvGroup_SelectedIndexChanged(object sender, EventArgs e)

        private string LoadEnvConditions(int intEnvGrpID)
        {
            string strError = "";

            try
            {
                // Load environment conditions based on Environment Group selection
                DataSet ds = environmentData.LoadEnvConditions(ref strError, intEnvGrpID);
                if (strError != string.Empty)
                {
                    MessageBox.Show(strError, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    //check for records
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        this.lvEnvConds.Items.Clear();

                        string[] lvSubItems = new string[4];
                        ListViewItem lvi;
                        foreach (DataRow row in dt.Rows)
                        {
                            // env_cond_id is the first column
                            lvSubItems[0] = row["env_cond_id"].ToString();
                            lvSubItems[1] = row["env_cond_name"].ToString();
                            // format units = if can be converted to a whole number, then remove extra zeros to the right of decimal
                            string tmp = RemoveZeros(row["env_cond_value"].ToString());
                            lvSubItems[2] = tmp;
                            lvSubItems[3] = row["env_cond_units"].ToString();
                            lvi = new ListViewItem(lvSubItems);
                            lvi.Font = new Font(lvi.Font, FontStyle.Regular);
                            lvEnvConds.Items.Add(lvi);
                        }
                    }
                    else
                    {
                        strError = "No environment groups have been found.";
                    }
                }
                return strError;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        } // end   private string LoadEnvConditions(int intEnvGrpID)

        /// <summary>
        /// Method cleans up the decimal format string
        /// </summary>
        /// <param name="tmp"></param>
        /// <returns></returns>
        private string RemoveZeros(string tmp)
        {
            decimal decValue;
            // check if string can be converted to a number
            if (decimal.TryParse(tmp, out decValue))
            {
                return decValue.ToString("G29");
            }
            else
            {
                return tmp;
            }
        } // end   private string RemoveZeros(string tmp)

        /// <summary>
        /// Load listview of Electrolyte options based on selected Environment Group Condition selection
        /// </summary>
        /// <param name="intEnvGrpID">PK for Environment Group</param>
        /// <returns></returns>
        private string LoadElectrolytes()
        {
            string strError = "";
            try
            {
                // Load electrolyte options for this immersion Environment Group selection
                DataSet ds = environmentData.LoadImmersionElectrolytes(ref strError, pElectrolyteID);
                if (strError != string.Empty)
                {
                    MessageBox.Show(strError, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    //check for records
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        this.lvImmConds.Items.Clear();

                        string[] lvSubItems = new string[4];
                        ListViewItem lvi;
                        foreach (DataRow row in dt.Rows)
                        {
                            lvSubItems[0] = row["electrolyte_cond_id"].ToString();
                            lvSubItems[1] = row["ele_cond_type"].ToString();
                            // format units = if can be converted to a whole number, then remove extra zeros to the right of decimal
                            string tmp = RemoveZeros(row["ele_cond_value"].ToString());
                            lvSubItems[2] = tmp;
                            lvSubItems[3] = row["ele_cond_units"].ToString();
                            lvi = new ListViewItem(lvSubItems);
                            lvi.Font = new Font(lvi.Font, FontStyle.Regular);
                            this.lvImmConds.Items.Add(lvi);
                        }
                        this.pnlElectrolyte.Visible = true;
                    }
                    else
                    {
                        strError = "No environment groups have been found.";
                    }
                }
                return strError;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        } // end   private string LoadElectrolytes()

        private void btnClearEnv_Click(object sender, EventArgs e)
        {
            ClearEnvcondPanels();
            cmbEnvGroup.SelectedIndex = -1;
        }

        private void ClearEnvcondPanels()
        {
            this.tbElectrolyte.Text = "";
            lvImmConds.Items.Clear();
            pnlElectrolyte.Visible = false;
            lvEnvConds.Items.Clear();
            pnlEnvConds.Visible = false;
            pnlSave.Visible = false;
        }

        private void btnSaveEnv_Click(object sender, EventArgs e)
        {
            if (cmbEnvGroup.SelectedItem == null)
            {
                MessageBox.Show("You must select an Enviroment Group option.");
                return;
            }

            // Update Model Data Aggregator class
            modelDataAggregator.EnvData.EnvGroup_Id = Convert.ToInt32(cmbEnvGroup.SelectedValue);
            modelDataAggregator.EnvData.EnvGroupName = cmbEnvGroup.Text;

            foreach (ListViewItem lvi in lvEnvConds.Items)
            {
                // assign EnvironmentCondition properties
                Utilities.EnvironmentalData.EnvCondition envData_EnvCondition = new Utilities.EnvironmentalData.EnvCondition();
                envData_EnvCondition.Env_cond_ID = Convert.ToInt32(lvi.SubItems[0].Text);
                envData_EnvCondition.Env_cond_name = lvi.SubItems[1].Text;
                envData_EnvCondition.Env_cond_value = lvi.SubItems[2].Text;
                envData_EnvCondition.Env_cond_units = lvi.SubItems[3].Text;
                // now add the object to the Environment Conditions list
                modelDataAggregator.EnvData.EnvConditionsList.Add(envData_EnvCondition);
            }
            // if immersion conditions exist, save those too
            if (!String.IsNullOrEmpty(tbElectrolyte.Text))
            {
                modelDataAggregator.EnvData.Electrolyte = tbElectrolyte.Text;
                foreach (ListViewItem lvi in lvImmConds.Items)
                {
                    // assign EnvironemtnCondtion properties
                    Utilities.EnvironmentalData.ImmersionCondition envData_immCondition = new Utilities.EnvironmentalData.ImmersionCondition();
                    envData_immCondition.Immersion_cond_ID = Convert.ToInt32(lvi.SubItems[0].Text);
                    envData_immCondition.Immersion_cond_type = lvi.SubItems[1].Text;
                    envData_immCondition.Immersion_cond_value = lvi.SubItems[2].Text;
                    envData_immCondition.Immersion_cond_units = lvi.SubItems[3].Text;
                    // now add it to the Environment Conditions list
                    modelDataAggregator.EnvData.ImmConditionsList.Add(envData_immCondition);
                }
            }
            else
            { // clear out variables
                modelDataAggregator.EnvData.Electrolyte = string.Empty;
                modelDataAggregator.EnvData.ImmConditionsList.Clear();
            }

            // Move to the Bounding Box
            HideTabPages(false, false);
            tabControl1.SelectedIndex = 2; // bounding box
        } // end  private void btnSaveEnv_Click(object sender, EventArgs e)

        #endregion


        #region Part/Material Selection


        private void dgvParts_CellEnter(object sender, DataGridViewCellEventArgs e)
        {

            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1); //Make sure the clicked row/column is valid.
            var dgvParts = sender as DataGridView;

            // Check to make sure the cell clicked is the cell containing the combobox 
            if (dgvParts.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
            {
                dgvParts.BeginEdit(true);
                ((ComboBox)dgvParts.EditingControl).DroppedDown = true;
            }

            dgvParts.CommitEdit(DataGridViewDataErrorContexts.Commit);

        }

        private void btnCancelMaterial_Click(object sender, EventArgs e)
        {
            propFuncs.partInfo.StoredComponents.Clear();
            propFuncs.SavedPartMaterialTable.Rows.Clear();
            propFuncs.PartSWMaterialTable.Rows.Clear();
            BuildDataGridView();
        }

        private void LoadMaterialTable()
        {
            string strTitle = "Load Material Combo";
            string errorMsg;
            string spName = "spGetMaterialList";
            DataSet ds = new DataSet();

            try
            {
                ds = dbConnect.GetDataSet(out errorMsg, spName);

                if (errorMsg != string.Empty)
                {
                    MessageBox.Show(errorMsg, "Connection Error");
                }
                else
                {
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            // this combo has been removed but steel need to populate dtMaterials
                           dtMaterials = ds.Tables[0].Copy();
                        }
                        else
                        {
                            MessageBox.Show("No materials were found.", strTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No dataset returned.", strTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (ds != null)
                    ds.Dispose();
            }
        } // end private void LoadMaterialCombo()

        private void HideTabPages(bool hideEnv, bool hideBox)
        {
            if (hideEnv)
            {
                tabControl1.TabPages.Remove(tabEnv);
            }
            else
            {
                if (tabControl1.TabCount == 1)
                {
                    tabControl1.TabPages.Insert(1, tabEnv);
                }
                // otherwise the env tab was added
            }
            if (hideBox)
            {
                tabControl1.TabPages.Remove(tabBoundBox);
            }
            else
            {
                tabControl1.TabPages.Insert(2, tabBoundBox);
            }
        } // end  private void HideTabPages(bool hideEnv, bool hideBox)




        private void btnAddIndivComp_Click(object sender, EventArgs e)
        {
            string compName = propFuncs.readPartName();
            if (!String.IsNullOrEmpty(compName))
            {
                // tbPartName Visible = false
                tbPartName.Text = compName;
                BuildDataGridView();
                bool getInvCompMaterial = true;
                AssignMaterials(getInvCompMaterial);
                // when BuildDataGridView is called, it clears any previously selected material ids
                // Call PreviouslyAssignedMaterialsCheck method to reapply those selections
                PreviouslyAssignedMaterialsCheck();
            }
        } // end   private void btnAddIndivComp_Click(object sender, EventArgs e)

        private void BuildDataGridView()
        {
            try
            {
                // clear out the rows if re-running
                if (dgvParts.Rows.Count > 0)
                {
                    dgvParts.DataSource = null;
                }
                // clear out the columns, so duplicates aren't created
                if (dgvParts.ColumnCount > 0)
                {
                    dgvParts.Columns.Remove(strPartNameCol);
                    dgvParts.Columns.Remove(strMaterialCol);
                }
                dgvParts.AutoGenerateColumns = false;
                dgvParts.EditMode = DataGridViewEditMode.EditOnEnter;  // set this other wise user has to click twice to get material list

                DataTable dt = new DataTable();
                dt.Columns.Add("PartName", typeof(String));
                dt.Columns.Add("Material", typeof(int));

                // Replace the sample partList with yours, you can sawp a string list with an arraylist, what ever object you are using
                // Note the -1 is just a placeholder for the material combo box value
                //List <string> partList = new List<string> { "Part Name 1", "Part Name 2", "Part Name 3", "Part Name 4" };

                foreach (string item in propFuncs.partInfo.StoredComponents)
                {
                    dt.Rows.Add(new object[] { item, -1 });
                }

                DataGridViewTextBoxColumn partName = new DataGridViewTextBoxColumn
                {
                    HeaderText = "Part Name",
                    DataPropertyName = "partName",
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.Programmatic,
                    Name = strPartNameCol
                };

                DataGridViewComboBoxColumn cmbColMaterials = new DataGridViewComboBoxColumn
                {
                    DataSource = dtMaterials, // this was set during class construction - temporary measure
                    DisplayMember = "material_name",
                    ValueMember = "material_id",
                    HeaderText = "Material",
                    DataPropertyName = "material_id",
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    Name = strMaterialCol
                };

                dgvParts.DataSource = dt;
                dgvParts.Columns.AddRange(partName, cmbColMaterials);
                dgvParts.Columns[0].Width = (int)(dgvParts.Width * 0.4);
                dgvParts.Columns[1].Width = (int)(dgvParts.Width * 0.6);
                dgvParts.ClearSelection();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        } // end   private void BuildDataGridView()


        private void btnAssignMaterials_Click(object sender, EventArgs e)
        {
            // get the first row
            DataGridViewRow dgvRow = dgvParts.Rows[0];
            // ge the second cell (0-based index)
            DataGridViewCell dataGridViewCell = dgvRow.Cells[1];
            // grab the value of the combo box
            int materialID = (int)dataGridViewCell.Value;

            // loop through the datagridview combo column and preselect the material
            foreach (DataGridViewRow row in dgvParts.Rows)
            {
                row.Cells[1].Value = materialID;
            }
        }


        //Select All Components Click Event
        private void btnAddAllComps_Click(object sender, EventArgs e)
        {
            propFuncs.selectAllParts();
            BuildDataGridView();
            AssignMaterials(false);
        } // end  private void button1_Click(object sender, EventArgs e)

        /// <summary>
        /// Method builds a datatable of materials that match the Part doc's material assignment and the database.
        /// If matches exist,it loops through dgvParts to find matching part Name and preselects Material
        /// </summary>
        /// <param name="getSingleComponent">If getting a single component, pass in the partname to OpenDocIteratorMaterialSearch</param>
        private void AssignMaterials(bool getSingleComponent)
        {
            // pass in part name if just getting material for individual componet
            string partName = string.Empty;
            if (getSingleComponent)
            {
                partName = tbPartName.Text;
            }
            DataTable dtPartMaterialMatches = propFuncs.openDocIteratorMaterialSearch(partName);
            if (dtPartMaterialMatches.Rows.Count > 0)
            {
                SearchPartDataGrid(dtPartMaterialMatches);
            }  // end    if (dtPartMaterialMatches.Rows.Count > 0)
        } // end     private void AssignMaterials()

        /// <summary>
        /// Method searches dgvParts for a specific part name and then pre-selects the material combo
        /// </summary>
        /// <param name="dt" >DataTable see BuildPartMaterialTable in PropertyFunctions </param>
        private void SearchPartDataGrid(DataTable dt)
        {
            int tmpMaterialID; // column 0
            string tmpPartName; // column 1
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    tmpMaterialID = Int32.Parse(dt.Rows[i][0].ToString());
                    tmpPartName = dt.Rows[i][1].ToString();

                    // go find the partname in the datagridview
                    foreach (DataGridViewRow row in dgvParts.Rows)
                    {
                        // part name is the first column
                        if (row.Cells[0].Value.ToString().Equals(tmpPartName))
                        {
                            // found the right row, now change the dropdown selection
                            DataGridViewComboBoxCell _cellCbo = (DataGridViewComboBoxCell)row.Cells[1];
                            _cellCbo.Value = tmpMaterialID;
                            break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        /// <summary>
        /// This method reapplies any previously selected materials in dgvParts after the datagridview part list was refreshed
        /// </summary>
        private void PreviouslyAssignedMaterialsCheck()
        {
            if (propFuncs.SavedPartMaterialTable != null && propFuncs.SavedPartMaterialTable.Rows.Count > 0)
            {
                SearchPartDataGrid(propFuncs.SavedPartMaterialTable);
            }
        }


        private void dgvParts_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (dgvParts.CurrentCell.ColumnIndex.Equals(0) && e.RowIndex != -1)
            {
                DataGridViewRow row = dgvParts.Rows[e.RowIndex];
                string selectedComponent = row.Cells[0].Value.ToString();

                propFuncs.HighlightPartbyName(selectedComponent);
            }
        }

        private void btnSaveMaterial_Click(object sender, EventArgs e)
        {
            if (PartMaterialMissingCheck())
            {
                MessageBox.Show("Part - Material assignments are missing.", "Part-Material Selection");
            }
            else
            {
                // Save list of MaterialIDs and  MaterialSelection object to modelDataAggregator
                string errors = SavePartMaterialSelections();
                if (errors != string.Empty)
                {
                    MessageBox.Show(errors, "Material ID Save Error");
                }
                else
                {
                    // Figure out the environment filtering
                    if (modelDataAggregator.MaterialIDList.Count > 1)
                    {
                        string results = DetermineEnvCondFilterParameters();
                        if (results != string.Empty)
                        {
                            //error occured, stop
                            MessageBox.Show(results, "Material/Model Filter Error");
                            return;
                        }
                        else
                        {
                            if (newMaterialIDs.Count > 0)
                            {
                                modelDataAggregator.MaterialIDList.Clear();
                                modelDataAggregator.MaterialIDList = newMaterialIDs;

                                // populate controls in Env Tab
                                if (LoadEnvironmentTab())
                                {
                                    // Show the Env tab
                                    HideTabPages(false, true);
                                    tabControl1.SelectedIndex = 1;
                                    // clear combo box selection
                                    cmbEnvGroup.SelectedIndex = -1;
                                }
                            }

                        }
                    } // end if if (modelDataAggregator.MaterialIDList.Count > 1)
                    else
                    {
                        // There is only one material selection, so set bool to ignore galvanic models
                        modelDataAggregator.GetGalvanic = false;
                        if (LoadEnvironmentTab())
                        {
                            // Show the Env tab
                            HideTabPages(false, true);
                            tabControl1.SelectedIndex = 1;
                            // clear combo box selection
                            cmbEnvGroup.SelectedIndex = -1;
                        }
                    }
                } // end else if (errors != string.Empty)
            } // end else  if (PartMaterialMissingCheck())
        } // end    private void btnSaveMaterial_Click(object sender, EventArgs e)

        /// <summary>
        /// Check if all of the parts have a selected material
        /// </summary>
        /// <returns>bool</returns>
        private bool PartMaterialMissingCheck()
        {
            bool isMissing = false;

            if (dgvParts.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvParts.Rows)
                {
                    // second column is the material combo
                    DataGridViewComboBoxCell _cellCbo = (DataGridViewComboBoxCell)row.Cells[1];
                    if (_cellCbo.Value == null)
                    {
                        isMissing = true;
                        break;
                    }
                }
            }
            else
            {
                isMissing = true;
            }

            return isMissing;
        } // end   private bool PartMaterialCheck()


        /// <summary>
        /// Build a unique list of MaterialSelection objects for the model
        /// </summary>
        /// <param name="useNewList"></param>
        /// <returns></returns>
        private string SavePartMaterialSelections()
        {
            try
            {
                // clear previous selections
                modelDataAggregator.MaterialSelectionList.Clear();
                modelDataAggregator.MaterialIDList.Clear();

                List<Parts.MaterialSelection> materialSelections = new List<Parts.MaterialSelection>();
                List<int> materialIds = new List<int>();

                int matID;
                string partName;
                string dbMatName;
                string swMatName;
                int isInList;

                foreach (DataGridViewRow row in dgvParts.Rows)
                {
                    // Build MaterialIDList for use in environment condition selection (get unique list)
                    matID = (int)row.Cells[strMaterialCol].Value;
                    isInList = materialIds.IndexOf(matID);
                    if (isInList == -1)
                    {
                        //not found, add it to list
                        materialIds.Add(matID);
                    }

                    // Now finish building the MaterialSelections list for the Report
                    partName = row.Cells[strPartNameCol].Value.ToString();
                    dbMatName = Convert.ToString((row.Cells[strMaterialCol] as DataGridViewComboBoxCell).FormattedValue.ToString());
                    // get the SW mat name from table
                    DataRow dr = propFuncs.PartSWMaterialTable.Rows.Find(partName);
                    if (dr != null)
                    {
                        // (columnorder:  sMatName, partName);
                        swMatName = dr[0].ToString();
                    }
                    else
                    {
                        return String.Format(" SW Material assignment value missing for part {0}.", partName);
                    }
                    materialSelections.Add(new Parts.MaterialSelection { PartName = partName, SWMaterial = swMatName, Material_ID = matID, MaterialName = dbMatName });

                }
                materialIds.Sort();
                modelDataAggregator.MaterialIDList = materialIds;
                // materialSelections is a list of MaterialSelection objects, so sort using Linq - sample for multi-column (listOfCars.OrderBy(x => x.CreationDate).ThenBy(x => x.Make).ThenBy(x => x.Whatever);
                materialSelections.Sort((x, y) => x.PartName.CompareTo(y.PartName));
                modelDataAggregator.MaterialSelectionList = materialSelections;

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        } // end  private string SavePartMaterialSelections()


        private string DetermineEnvCondFilterParameters()
        {
            string strError = "";
            // create a comma-delimited string of material IDs
            string strOfMaterilIds;
            DataSet ds = new DataSet();
            try
            {
                strOfMaterilIds = string.Join(",", modelDataAggregator.MaterialIDList);
                // Load environment group combo - filter by material ID list options
                ds = environmentData.LoadMaterialModels(ref strError, strOfMaterilIds);
                if (strError != string.Empty)
                {
                    // MessageBox.Show(strError, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return strError;
                }
                else
                {
                    //check for records
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        // matchedTable = BuildModelTable(false);
                        //   unMatchedTable = BuildModelTables(false);

                        //  intNumOfMaterials = modelDataAggregator.MateraiIDList.Count;
                        //int modelId;

                        // get the first materialID (Always will be at least two material ID in the list if this function is ran)
                        //  int matID = modelDataAggregator.MateraiIDList[0];
                        // first get the list of model Ids from first materialID to use for comparison

                        // define temp variables to use in messages
                        bool material_A_HasModel = false;
                        bool material_B_HasModel = false;
                        bool material_A_HasGalvanic = false;
                        bool material_B_HasGalvanic = false;
                        string savedMaterial_A = "";
                        string savedMaterial_B = "";
                        int savedMaterialAId = 0;
                        int savedMaterialBId = 0;


                        // FIRST TABLE is just a count of the number of models for each Material.
                        // material_id, material_name, ModelCount, IsGalvanic
                        // This is prototype hack, once we have more than two models, will have to rethink the filtering logic

                        bool isGalvanic = false;
                        bool isSimple = false;
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = ds.Tables[0].Rows[i];

                            // For PROTOTYPE, we just have 2 models, NRL has not provided any yet, so if count is 2, the material has a simple and galvanic model
                            int modelCount = (int)dr["ModelCount"];
                            bool isGalvanicCheck = (bool)dr["IsGalvanic"];
                            if (modelCount == 2)
                            {
                                isGalvanic = true;
                                isSimple = true;
                            }
                            else if (modelCount == 1)
                            {
                                isGalvanic = isGalvanicCheck;
                                isSimple = !isGalvanic;
                            }
                            else
                            {
                                // no models found
                                isGalvanic = false;
                                isSimple = false;
                            }
                            if (i == 0)
                            {
                                material_A_HasModel = isSimple;
                                material_A_HasGalvanic = isGalvanic;
                                savedMaterial_A = dr["material_name"].ToString();
                                savedMaterialAId = (int)dr["material_id"];

                            }
                            else if (i == 1) // second row
                            {
                                material_B_HasModel = isSimple;
                                material_B_HasGalvanic = isGalvanic;
                                savedMaterial_B = dr["material_name"].ToString();
                                savedMaterialBId = (int)dr["material_id"];
                            }
                            // PROTOTYPE - other rows will be igoored for now
                        } // end    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)

                        // Determine the message and what model to use 
                        if ((material_A_HasModel == true && material_B_HasModel == false) || (material_A_HasModel == false && material_B_HasModel == true)
                            && (material_A_HasGalvanic == false || material_B_HasGalvanic == false))
                        {
                            if (material_A_HasModel)
                            {
                                string msg = String.Format("{0} is linked to a model, however {1} is not linked to a model. Both materials are not linked to a galvanic model." + System.Environment.NewLine + System.Environment.NewLine + "Click OK to proceed with the known model or Cancel to change material selections", savedMaterial_A, savedMaterial_B);
                                DialogResult response = MessageBox.Show(msg, "Model Choices", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                                if (response == DialogResult.Cancel)
                                {
                                    // DO NOTHING
                                    return string.Empty;
                                }
                                else
                                {
                                    newMaterialIDs.Add(savedMaterialAId);
                                    modelDataAggregator.GetGalvanic = false;
                                    return string.Empty;
                                }
                            }
                            else
                            {
                                // B has the simple model
                                string msg = String.Format("{0} is linked to a model, however {1} has no model. Both materials are not linked to a galvanic model. " + System.Environment.NewLine + System.Environment.NewLine + "Click OK to proceed with the known model or Cancel to change material selections.", savedMaterial_B, savedMaterial_A);
                                DialogResult response = MessageBox.Show(msg, "Model Choices", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                                if (response == DialogResult.Cancel)
                                {
                                    // DO NOTHING
                                    return string.Empty;
                                }
                                else
                                {
                                    newMaterialIDs.Add(savedMaterialBId);
                                    modelDataAggregator.GetGalvanic = false;
                                    return string.Empty;
                                }
                            }
                        }
                        else if (material_A_HasModel == false && material_B_HasModel == false && material_A_HasGalvanic == true && material_B_HasGalvanic == true)
                        {
                            //proceed to environment selection
                            newMaterialIDs.Add(savedMaterialAId);
                            newMaterialIDs.Add(savedMaterialBId);
                            modelDataAggregator.GetGalvanic = true;
                            return string.Empty;
                        }
                        else if (material_A_HasModel == true && material_B_HasModel == true && (material_A_HasGalvanic == false || material_B_HasGalvanic == false))
                        {
                            // B has the simple model
                            string msg = String.Format("There are no galvanic models for {0} and {1}.  " + System.Environment.NewLine + System.Environment.NewLine + "However, each material is linked to a model. Change material selection. " + System.Environment.NewLine + System.Environment.NewLine + "In Phase II, an option to process these materials independently will be available.", savedMaterial_A, savedMaterial_B);


                            DialogResult response = MessageBox.Show(msg, "Model Choices", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (response == DialogResult.Cancel)
                            {
                                // DO NOTHING until Phase II
                                return string.Empty;
                            }
                            else
                            {
                                // DO NOTHING until Phase II
                                return string.Empty;
                            }
                        }
                        else if (material_A_HasModel == true && material_B_HasModel == true && material_A_HasGalvanic == true && material_B_HasGalvanic == true)
                        {
                            string msg = String.Format("{0} and {1} have individual models and both have galvanic models." + System.Environment.NewLine + System.Environment.NewLine + "For Phase I, click OK to proceed with the galvanic option. Otherwise, click Cancel. " + System.Environment.NewLine + "In Phase II, option to perform individual analysis will be available.", savedMaterial_A, savedMaterial_B);
                            DialogResult response = MessageBox.Show(msg, "Model Choices", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (response == DialogResult.Cancel)
                            {
                                // DO NOTHING
                                return string.Empty;
                            }
                            else
                            {
                                newMaterialIDs.Add(savedMaterialAId);
                                newMaterialIDs.Add(savedMaterialBId);
                                modelDataAggregator.GetGalvanic = true;
                                return string.Empty;
                            }
                        }
                        else if (material_A_HasModel == false && material_B_HasModel == false && material_A_HasGalvanic == false && material_B_HasGalvanic == false)
                        {
                            string msg = String.Format("{0} and {1} do not have any models loaded. Either load models or change material selection.", savedMaterial_A, savedMaterial_B);
                            MessageBox.Show(msg, "Model Choices", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return string.Empty;
                        }
                        else
                        {
                            return "No Models found. Load models or select different materials.";
                        }
                    } // end if (ds.Tables[0].Rows.Count > 0)
                    else
                    {
                        return "No records were returned.";
                    }
                }
            } // end try
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ex.Message;

            }
            finally
            {
                if (ds != null)
                    ds.Dispose();
            }
        } // end  private string DetermineEnvCondFilterParameters()

        /*
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvParts.Rows[e.RowIndex];
                string selectedComponent = row.Cells[0].Value.ToString();

                propFuncs.HighlightPartbyName(selectedComponent);

            }
        }
        */

        /*
        private void dgvParts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvParts.Rows[e.RowIndex];
                string selectedComponent = row.Cells[0].Value.ToString();

                propFuncs.HighlightPartbyName(selectedComponent);

            }


        }
        */



        #endregion




        #region ProcessPartFacesAndHoles


        private void ProcessPartFaceHoles()
        {
            /* Keegan's */
            PartInfo[] myParts;
            FaceInfo[] someFaces = new FaceInfo[2];
            ModelDoc2 swModel = default(ModelDoc2);
            AssemblyDoc swAssDoc = default(AssemblyDoc);
            FaceManager fmMyFaces;
            StringBuilder sbFaceInfo = new StringBuilder();
            FileOps myFileOps = new FileOps();
            PartInfo myPart = new PartInfo();
            //Get application and loaded model info
            GetAppInfo();
            StringBuilder usrMessage = new StringBuilder();

            someFaces[0] = new FaceInfo();

            //Open part
            //swModel = (ModelDoc2)swApp.OpenDoc6(fileName, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
            //swModel = (ModelDoc2)swApp.swActiveDoc;
            swModel = (ModelDoc2)swDoc;

            //Cast model to an assembly - We should put some error checking here to verify it is not a 
            swAssDoc = (AssemblyDoc)swModel;
            myParts = new PartInfo[swAssDoc.GetComponentCount(true)];

            // assign values to ModelDataAggregator properties
            modelDataAggregator.partInfoArray = myParts;
            modelDataAggregator.AssemblyPathLocation = swModel.GetPathName();
            modelDataAggregator.AssemblyTitle = swModel.GetTitle();

            fmMyFaces = new FaceManager(swApp, swAssDoc);

            usrMessage.AppendLine("Original Assembly Location: " + swModel.GetPathName());
            usrMessage.AppendLine("Assembly Name: " + swModel.GetTitle());
            sbFaceInfo = fmMyFaces.GetAssInfo(myParts);
            usrMessage.AppendLine(sbFaceInfo.ToString());
            //myFileOps.WriteToFile(usrMessage);

            //Get answers on the pipe search from what was gathered during the GetAssInfo call
        //    StringBuilder sbPipeInfo = new StringBuilder();
        //    for (int i = 0; i < myParts.Length; i++)
        //    {
        //        sbPipeInfo.AppendLine("Component: " + myParts[i].Name + " - Is a pipe?  " + myParts[i].PipeCheck().ToString());
        //    }
        //    MessageBox.Show(sbPipeInfo.ToString());
            //Outputs all collected info to a messagebox - too much info and runs off the screen
            //MessageBox.Show(usrMessage.ToString());
        }

        private void GetAppInfo()
        {
            swApp = (ISldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
            int errors = 0;

            // NOTE: changed the method call from  swDoc = swApp.ActivateDoc(string.Empty); Saw in documentation that it was obsolete
            // With IActivateDoc3 you have to pass in the document name and if you want errors to be returned. I hard-coded the name for testing
            // After that, the rest of the code seems to work and populates the objects as expected.
            ModelDoc2 swModel;
            object[] models;
            int count;
            int index;
            string pathName = "";

            //get all of the active docs
            count = swApp.GetDocumentCount();
            //Debug.Print("Number of open documents in this SolidWorks session: " + count);
            models = (object[])swApp.GetDocuments();

            for (index = 0; index < count; index++)
            {
                swModel = models[index] as ModelDoc2;
                pathName = swModel.GetPathName();
                //Debug.Print("Path and name of open document: " + pathName);
            }

            //seperate a path name
            string tempFileName = Path.GetFileName(pathName);

            swDoc = swApp.IActivateDoc3(tempFileName, false, ref errors);
        }

        #endregion

        //Add Individual Component Rounded
        private void roundedButton1_Click(object sender, EventArgs e)
        {
            string compName = propFuncs.readPartName();
            if (!String.IsNullOrEmpty(compName))
            {
                // tbPartName Visible = false
                tbPartName.Text = compName;
                BuildDataGridView();
                bool getInvCompMaterial = true;
                AssignMaterials(getInvCompMaterial);
                // when BuildDataGridView is called, it clears any previously selected material ids
                // Call PreviouslyAssignedMaterialsCheck method to reapply those selections
                PreviouslyAssignedMaterialsCheck();
            }
        }


        //add all components
        private void roundedButton2_Click(object sender, EventArgs e)
        {
            propFuncs.selectAllParts();
            BuildDataGridView();
            AssignMaterials(false);
        }

        private void roundedButton3_Click(object sender, EventArgs e)
        {
            propFuncs.partInfo.StoredComponents.Clear();
            propFuncs.SavedPartMaterialTable.Rows.Clear();
            propFuncs.PartSWMaterialTable.Rows.Clear();
            BuildDataGridView();
        }

        //Environmental Section button pg1
        private void roundedButton4_Click(object sender, EventArgs e)
        {
            if (PartMaterialMissingCheck())
            {
                MessageBox.Show("Part - Material assignments are missing.", "Part-Material Selection");
            }
            else
            {
                // Save list of MaterialIDs and  MaterialSelection object to modelDataAggregator
                string errors = SavePartMaterialSelections();
                if (errors != string.Empty)
                {
                    MessageBox.Show(errors, "Material ID Save Error");
                }
                else
                {
                    // Figure out the environment filtering
                    if (modelDataAggregator.MaterialIDList.Count > 1)
                    {
                        string results = DetermineEnvCondFilterParameters();
                        if (results != string.Empty)
                        {
                            //error occured, stop
                            MessageBox.Show(results, "Material/Model Filter Error");
                            return;
                        }
                        else
                        {
                            if (newMaterialIDs.Count > 0)
                            {
                                modelDataAggregator.MaterialIDList.Clear();
                                modelDataAggregator.MaterialIDList = newMaterialIDs;

                                // populate controls in Env Tab
                                if (LoadEnvironmentTab())
                                {
                                    // Show the Env tab
                                    HideTabPages(false, true);
                                    tabControl1.SelectedIndex = 1;
                                    // clear combo box selection
                                    cmbEnvGroup.SelectedIndex = -1;
                                }
                            }

                        }
                    } // end if if (modelDataAggregator.MaterialIDList.Count > 1)
                    else
                    {
                        // There is only one material selection, so set bool to ignore galvanic models
                        modelDataAggregator.GetGalvanic = false;
                        if (LoadEnvironmentTab())
                        {
                            // Show the Env tab
                            HideTabPages(false, true);
                            tabControl1.SelectedIndex = 1;
                            // clear combo box selection
                            cmbEnvGroup.SelectedIndex = -1;
                        }
                    }
                } // end else if (errors != string.Empty)
            } // end else  if (PartMaterialMissingCheck())
        }

      

        private void roundedButton6_Click(object sender, EventArgs e)
        {
            if (cmbEnvGroup.SelectedItem == null)
            {
                MessageBox.Show("You must select an Enviroment Group option.");
                return;
            }

            // Update Model Data Aggregator class
            modelDataAggregator.EnvData.EnvGroup_Id = Convert.ToInt32(cmbEnvGroup.SelectedValue);
            modelDataAggregator.EnvData.EnvGroupName = cmbEnvGroup.Text;
            modelDataAggregator.ModelName = pModelName;

            foreach (ListViewItem lvi in lvEnvConds.Items)
            {
                // assign EnvironmentCondition properties
                Utilities.EnvironmentalData.EnvCondition envData_EnvCondition = new Utilities.EnvironmentalData.EnvCondition();
                envData_EnvCondition.Env_cond_ID = Convert.ToInt32(lvi.SubItems[0].Text);
                envData_EnvCondition.Env_cond_name = lvi.SubItems[1].Text;
                envData_EnvCondition.Env_cond_value = lvi.SubItems[2].Text;
                envData_EnvCondition.Env_cond_units = lvi.SubItems[3].Text;
                // now add the object to the Environment Conditions list
                modelDataAggregator.EnvData.EnvConditionsList.Add(envData_EnvCondition);
            }
            // if immersion conditions exist, save those too
            if (!String.IsNullOrEmpty(tbElectrolyte.Text))
            {
                modelDataAggregator.EnvData.Electrolyte = tbElectrolyte.Text;
                foreach (ListViewItem lvi in lvImmConds.Items)
                {
                    // assign EnvironmentCondition properties
                    Utilities.EnvironmentalData.ImmersionCondition envData_immCondition = new Utilities.EnvironmentalData.ImmersionCondition();
                    envData_immCondition.Immersion_cond_ID = Convert.ToInt32(lvi.SubItems[0].Text);
                    envData_immCondition.Immersion_cond_type = lvi.SubItems[1].Text;
                    envData_immCondition.Immersion_cond_value = lvi.SubItems[2].Text;
                    envData_immCondition.Immersion_cond_units = lvi.SubItems[3].Text;
                    // now add it to the Environment Conditions list
                    modelDataAggregator.EnvData.ImmConditionsList.Add(envData_immCondition);
                }
            }
            else
            { // clear out variables
                modelDataAggregator.EnvData.Electrolyte = string.Empty;
                modelDataAggregator.EnvData.ImmConditionsList.Clear();
            }

            // Move to the Bounding Box
            HideTabPages(false, false);
            tabControl1.SelectedIndex = 2; // bounding box
        }

        //draw box minimum size possible
        private void roundedButton7_Click(object sender, EventArgs e)
        {
            string tmp = boundingBox.GetBoundingBox();
            if (tmp != string.Empty)
            {
                MessageBox.Show(tmp, "Bounding Box Message");
            }
            else
            {
                modelDataAggregator.BoundingBoxVolume = boundingBox.GetBoundingBoxVolumne();
            }
        }

        //draw box using components
        private void roundedButton8_Click(object sender, EventArgs e)
        {
            // check if the value is a double
            double dblCheck;
            if (Double.TryParse(tbBoundBox.Text, out dblCheck))
            {
                // good to go, go draw the box
                TestBoundingBox2(dblCheck);
            }
            else
            {
                MessageBox.Show("Bounding box buffer must be of type double.", "Bounding Box Error");
            }
        }

        //roll back bounding box
        private void roundedButton9_Click(object sender, EventArgs e)
        {
            try
            {
                string strMessage = boundingBox.RevertBoundingBox();
                MessageBox.Show(strMessage);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //Process and Generate Model Suggestions >>>
        private void roundedButton10_Click(object sender, EventArgs e)
        {
            
            //SUPERSEDED hole count to Model data agregator
           // propFuncs.treeItterator();
          //  modelDataAggregator.holeCount = propFuncs.HoleCount;
            //

            modelDataAggregator.InterferencesList = propFuncs.DetectInterfearence();            
            modelDataAggregator.matesData = propFuncs.mateRead(modelDataAggregator.MaterialSelectionList);

            // Call to ProcessPartFaceHoles assigns values to 
            //  modelDataAggregator.partInfoArray;
            //  modelDataAggregator.AssemblyPathLocation and
            // modelDataAggregator.AssemblyTitle
            ProcessPartFaceHoles();

            // Get Part Volume after the partInfoArray has been populated
            modelDataAggregator.partInfoArray = propFuncs.readpartVolume(modelDataAggregator.partInfoArray);

            MessageBox.Show("Analysis is complete. Click OK to generate the report.");

            //Generate Report
            modelDataAggregator.ReportGenerator.GenerateReport(modelDataAggregator.AssemblyPathLocation, modelDataAggregator.AssemblyTitle,
              modelDataAggregator.InterferencesList, modelDataAggregator.matesData, modelDataAggregator.BoundingBoxVolume,
              modelDataAggregator.partInfoArray, modelDataAggregator.MaterialSelectionList, modelDataAggregator.EnvData, modelDataAggregator.ModelName);
        }
        
    } // end  public partial class TaskpaneHostUI : UserControl
}
