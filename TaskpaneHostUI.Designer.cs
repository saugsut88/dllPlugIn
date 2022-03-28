
using System.Windows.Forms;

namespace VPS.SWplugin.ICT
{
    partial class TaskpaneHostUI
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabMaterials = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.gradientPanel1 = new GradientPanelDemo.GradientPanel();
            this.roundedButton4 = new RoundedButton();
            this.roundedButton2 = new RoundedButton();
            this.roundedButton3 = new RoundedButton();
            this.roundedButton1 = new RoundedButton();
            this.dgvParts = new System.Windows.Forms.DataGridView();
            this.tabEnv = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.gradientPanel2 = new GradientPanelDemo.GradientPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlEnvGroup = new System.Windows.Forms.Panel();
            this.btnClearEnv = new System.Windows.Forms.Button();
            this.lblEnvGroup = new System.Windows.Forms.Label();
            this.cmbEnvGroup = new System.Windows.Forms.ComboBox();
            this.pnlEnvConds = new System.Windows.Forms.Panel();
            this.lvEnvConds = new System.Windows.Forms.ListView();
            this.ID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ConditionName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Units = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblEnvConds = new System.Windows.Forms.Label();
            this.pnlElectrolyte = new System.Windows.Forms.Panel();
            this.tbElectrolyte = new System.Windows.Forms.TextBox();
            this.lvImmConds = new System.Windows.Forms.ListView();
            this.elect_cond_id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ele_cond_type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ele_cond_value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ele_cond_units = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblAvailImmCond = new System.Windows.Forms.Label();
            this.lblImmersionConds = new System.Windows.Forms.Label();
            this.pnlSave = new System.Windows.Forms.Panel();
            this.roundedButton6 = new RoundedButton();
            this.tabBoundBox = new System.Windows.Forms.TabPage();
            this.roundedButton10 = new RoundedButton();
            this.roundedButton9 = new RoundedButton();
            this.roundedButton8 = new RoundedButton();
            this.roundedButton7 = new RoundedButton();
            this.label6 = new System.Windows.Forms.Label();
            this.gradientPanel3 = new GradientPanelDemo.GradientPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tbBoundBox = new System.Windows.Forms.TextBox();
            this.lblBoundBox = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblPartName = new System.Windows.Forms.Label();
            this.tbPartName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblMaterial = new System.Windows.Forms.Label();
            this.btnApplyMaterial = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabMaterials.SuspendLayout();
            this.gradientPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParts)).BeginInit();
            this.tabEnv.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.pnlEnvGroup.SuspendLayout();
            this.pnlEnvConds.SuspendLayout();
            this.pnlElectrolyte.SuspendLayout();
            this.pnlSave.SuspendLayout();
            this.tabBoundBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabMaterials);
            this.tabControl1.Controls.Add(this.tabEnv);
            this.tabControl1.Controls.Add(this.tabBoundBox);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(14, 34);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(535, 554);
            this.tabControl1.TabIndex = 3;
            // 
            // tabMaterials
            // 
            this.tabMaterials.BackColor = System.Drawing.Color.Transparent;
            this.tabMaterials.Controls.Add(this.label2);
            this.tabMaterials.Controls.Add(this.roundedButton1);
            this.tabMaterials.Controls.Add(this.dgvParts);
            this.tabMaterials.Controls.Add(this.gradientPanel1);
            this.tabMaterials.Location = new System.Drawing.Point(4, 22);
            this.tabMaterials.Name = "tabMaterials";
            this.tabMaterials.Padding = new System.Windows.Forms.Padding(3);
            this.tabMaterials.Size = new System.Drawing.Size(527, 528);
            this.tabMaterials.TabIndex = 2;
            this.tabMaterials.Tag = "materials";
            this.tabMaterials.Text = "Part/Material Selection";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(89, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(368, 68);
            this.label2.TabIndex = 15;
            this.label2.Text = "Material Selection";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gradientPanel1
            // 
            this.gradientPanel1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gradientPanel1.ColorBottom = System.Drawing.Color.LightSlateGray;
            this.gradientPanel1.ColorTop = System.Drawing.Color.LightSteelBlue;
            this.gradientPanel1.Controls.Add(this.roundedButton4);
            this.gradientPanel1.Controls.Add(this.roundedButton2);
            this.gradientPanel1.Controls.Add(this.roundedButton3);
            this.gradientPanel1.Location = new System.Drawing.Point(0, 0);
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Size = new System.Drawing.Size(527, 532);
            this.gradientPanel1.TabIndex = 18;
            // 
            // roundedButton4
            // 
            this.roundedButton4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundedButton4.Location = new System.Drawing.Point(308, 472);
            this.roundedButton4.Name = "roundedButton4";
            this.roundedButton4.Size = new System.Drawing.Size(202, 37);
            this.roundedButton4.TabIndex = 23;
            this.roundedButton4.Text = "Environment Selections   >>>";
            this.roundedButton4.UseVisualStyleBackColor = true;
            this.roundedButton4.Click += new System.EventHandler(this.roundedButton4_Click);
            // 
            // roundedButton2
            // 
            this.roundedButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundedButton2.Location = new System.Drawing.Point(308, 71);
            this.roundedButton2.Name = "roundedButton2";
            this.roundedButton2.Size = new System.Drawing.Size(202, 37);
            this.roundedButton2.TabIndex = 21;
            this.roundedButton2.Text = "Add All Components";
            this.roundedButton2.UseVisualStyleBackColor = true;
            this.roundedButton2.Click += new System.EventHandler(this.roundedButton2_Click);
            // 
            // roundedButton3
            // 
            this.roundedButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundedButton3.Location = new System.Drawing.Point(308, 297);
            this.roundedButton3.Name = "roundedButton3";
            this.roundedButton3.Size = new System.Drawing.Size(202, 37);
            this.roundedButton3.TabIndex = 22;
            this.roundedButton3.Text = "Clear Selection";
            this.roundedButton3.UseVisualStyleBackColor = true;
            this.roundedButton3.Click += new System.EventHandler(this.roundedButton3_Click);
            // 
            // roundedButton1
            // 
            this.roundedButton1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.roundedButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundedButton1.Location = new System.Drawing.Point(29, 71);
            this.roundedButton1.Name = "roundedButton1";
            this.roundedButton1.Size = new System.Drawing.Size(202, 37);
            this.roundedButton1.TabIndex = 20;
            this.roundedButton1.Text = "Add Individual Component";
            this.roundedButton1.UseVisualStyleBackColor = false;
            this.roundedButton1.Click += new System.EventHandler(this.roundedButton1_Click);
            // 
            // dgvParts
            // 
            this.dgvParts.AllowUserToAddRows = false;
            this.dgvParts.AllowUserToDeleteRows = false;
            this.dgvParts.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvParts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParts.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvParts.Location = new System.Drawing.Point(29, 120);
            this.dgvParts.MultiSelect = false;
            this.dgvParts.Name = "dgvParts";
            this.dgvParts.RowHeadersVisible = false;
            this.dgvParts.RowHeadersWidth = 72;
            this.dgvParts.Size = new System.Drawing.Size(481, 156);
            this.dgvParts.TabIndex = 18;
            this.dgvParts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvParts_CellClick);
            this.dgvParts.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvParts_CellEnter);
            // 
            // tabEnv
            // 
            this.tabEnv.BackColor = System.Drawing.Color.LightSlateGray;
            this.tabEnv.Controls.Add(this.label5);
            this.tabEnv.Controls.Add(this.flowLayoutPanel1);
            this.tabEnv.Controls.Add(this.gradientPanel2);
            this.tabEnv.Location = new System.Drawing.Point(4, 22);
            this.tabEnv.Name = "tabEnv";
            this.tabEnv.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnv.Size = new System.Drawing.Size(527, 528);
            this.tabEnv.TabIndex = 0;
            this.tabEnv.Tag = "environment";
            this.tabEnv.Text = "Environment Conditions";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(80, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(368, 68);
            this.label5.TabIndex = 16;
            this.label5.Text = "Environment Selection";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gradientPanel2
            // 
            this.gradientPanel2.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gradientPanel2.ColorBottom = System.Drawing.Color.LightSlateGray;
            this.gradientPanel2.ColorTop = System.Drawing.Color.LightSteelBlue;
            this.gradientPanel2.Location = new System.Drawing.Point(-4, -2);
            this.gradientPanel2.Name = "gradientPanel2";
            this.gradientPanel2.Size = new System.Drawing.Size(535, 532);
            this.gradientPanel2.TabIndex = 28;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.flowLayoutPanel1.Controls.Add(this.pnlEnvGroup);
            this.flowLayoutPanel1.Controls.Add(this.pnlEnvConds);
            this.flowLayoutPanel1.Controls.Add(this.pnlElectrolyte);
            this.flowLayoutPanel1.Controls.Add(this.pnlSave);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 79);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(521, 434);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // pnlEnvGroup
            // 
            this.pnlEnvGroup.Controls.Add(this.btnClearEnv);
            this.pnlEnvGroup.Controls.Add(this.lblEnvGroup);
            this.pnlEnvGroup.Controls.Add(this.cmbEnvGroup);
            this.pnlEnvGroup.Location = new System.Drawing.Point(3, 3);
            this.pnlEnvGroup.Name = "pnlEnvGroup";
            this.pnlEnvGroup.Size = new System.Drawing.Size(508, 45);
            this.pnlEnvGroup.TabIndex = 2;
            // 
            // btnClearEnv
            // 
            this.btnClearEnv.Location = new System.Drawing.Point(612, 10);
            this.btnClearEnv.Name = "btnClearEnv";
            this.btnClearEnv.Size = new System.Drawing.Size(75, 23);
            this.btnClearEnv.TabIndex = 2;
            this.btnClearEnv.Text = "Clear";
            this.btnClearEnv.UseVisualStyleBackColor = true;
            this.btnClearEnv.Click += new System.EventHandler(this.btnClearEnv_Click);
            // 
            // lblEnvGroup
            // 
            this.lblEnvGroup.AutoSize = true;
            this.lblEnvGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnvGroup.Location = new System.Drawing.Point(18, 11);
            this.lblEnvGroup.Name = "lblEnvGroup";
            this.lblEnvGroup.Size = new System.Drawing.Size(159, 13);
            this.lblEnvGroup.TabIndex = 1;
            this.lblEnvGroup.Text = "Select Environment Group:";
            // 
            // cmbEnvGroup
            // 
            this.cmbEnvGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEnvGroup.FormattingEnabled = true;
            this.cmbEnvGroup.Location = new System.Drawing.Point(180, 10);
            this.cmbEnvGroup.Name = "cmbEnvGroup";
            this.cmbEnvGroup.Size = new System.Drawing.Size(286, 21);
            this.cmbEnvGroup.TabIndex = 0;
            this.cmbEnvGroup.SelectedIndexChanged += new System.EventHandler(this.cmbEnvGroup_SelectedIndexChanged);
            // 
            // pnlEnvConds
            // 
            this.pnlEnvConds.Controls.Add(this.lvEnvConds);
            this.pnlEnvConds.Controls.Add(this.lblEnvConds);
            this.pnlEnvConds.Location = new System.Drawing.Point(3, 54);
            this.pnlEnvConds.Name = "pnlEnvConds";
            this.pnlEnvConds.Size = new System.Drawing.Size(508, 151);
            this.pnlEnvConds.TabIndex = 3;
            this.pnlEnvConds.Visible = false;
            // 
            // lvEnvConds
            // 
            this.lvEnvConds.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lvEnvConds.CheckBoxes = true;
            this.lvEnvConds.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ID,
            this.ConditionName,
            this.Value,
            this.Units});
            this.lvEnvConds.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvEnvConds.FullRowSelect = true;
            this.lvEnvConds.GridLines = true;
            this.lvEnvConds.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvEnvConds.HideSelection = false;
            this.lvEnvConds.Location = new System.Drawing.Point(18, 34);
            this.lvEnvConds.Name = "lvEnvConds";
            this.lvEnvConds.Size = new System.Drawing.Size(487, 97);
            this.lvEnvConds.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvEnvConds.TabIndex = 7;
            this.lvEnvConds.UseCompatibleStateImageBehavior = false;
            this.lvEnvConds.View = System.Windows.Forms.View.Details;
            // 
            // ID
            // 
            this.ID.Text = "ID";
            this.ID.Width = 0;
            // 
            // ConditionName
            // 
            this.ConditionName.Text = "Condition Name";
            this.ConditionName.Width = 300;
            // 
            // Value
            // 
            this.Value.Text = "Value";
            this.Value.Width = 90;
            // 
            // Units
            // 
            this.Units.Text = "Units";
            this.Units.Width = 160;
            // 
            // lblEnvConds
            // 
            this.lblEnvConds.AutoSize = true;
            this.lblEnvConds.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnvConds.Location = new System.Drawing.Point(18, 18);
            this.lblEnvConds.Name = "lblEnvConds";
            this.lblEnvConds.Size = new System.Drawing.Size(198, 13);
            this.lblEnvConds.TabIndex = 2;
            this.lblEnvConds.Text = "Selected Environment Conditions:";
            // 
            // pnlElectrolyte
            // 
            this.pnlElectrolyte.Controls.Add(this.tbElectrolyte);
            this.pnlElectrolyte.Controls.Add(this.lvImmConds);
            this.pnlElectrolyte.Controls.Add(this.lblAvailImmCond);
            this.pnlElectrolyte.Controls.Add(this.lblImmersionConds);
            this.pnlElectrolyte.Location = new System.Drawing.Point(3, 211);
            this.pnlElectrolyte.Name = "pnlElectrolyte";
            this.pnlElectrolyte.Size = new System.Drawing.Size(508, 171);
            this.pnlElectrolyte.TabIndex = 4;
            this.pnlElectrolyte.Visible = false;
            // 
            // tbElectrolyte
            // 
            this.tbElectrolyte.Location = new System.Drawing.Point(152, 15);
            this.tbElectrolyte.Name = "tbElectrolyte";
            this.tbElectrolyte.ReadOnly = true;
            this.tbElectrolyte.Size = new System.Drawing.Size(210, 20);
            this.tbElectrolyte.TabIndex = 12;
            // 
            // lvImmConds
            // 
            this.lvImmConds.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lvImmConds.CheckBoxes = true;
            this.lvImmConds.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.elect_cond_id,
            this.ele_cond_type,
            this.ele_cond_value,
            this.ele_cond_units});
            this.lvImmConds.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvImmConds.FullRowSelect = true;
            this.lvImmConds.GridLines = true;
            this.lvImmConds.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvImmConds.HideSelection = false;
            this.lvImmConds.Location = new System.Drawing.Point(18, 64);
            this.lvImmConds.Name = "lvImmConds";
            this.lvImmConds.Size = new System.Drawing.Size(487, 97);
            this.lvImmConds.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvImmConds.TabIndex = 11;
            this.lvImmConds.UseCompatibleStateImageBehavior = false;
            this.lvImmConds.View = System.Windows.Forms.View.Details;
            // 
            // elect_cond_id
            // 
            this.elect_cond_id.Text = "ID";
            this.elect_cond_id.Width = 0;
            // 
            // ele_cond_type
            // 
            this.ele_cond_type.Text = "Condition Type";
            this.ele_cond_type.Width = 300;
            // 
            // ele_cond_value
            // 
            this.ele_cond_value.Text = "Value";
            this.ele_cond_value.Width = 90;
            // 
            // ele_cond_units
            // 
            this.ele_cond_units.Text = "Units";
            this.ele_cond_units.Width = 160;
            // 
            // lblAvailImmCond
            // 
            this.lblAvailImmCond.AutoSize = true;
            this.lblAvailImmCond.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAvailImmCond.Location = new System.Drawing.Point(18, 48);
            this.lblAvailImmCond.Name = "lblAvailImmCond";
            this.lblAvailImmCond.Size = new System.Drawing.Size(130, 13);
            this.lblAvailImmCond.TabIndex = 4;
            this.lblAvailImmCond.Text = "Immersion Conditions:";
            // 
            // lblImmersionConds
            // 
            this.lblImmersionConds.AutoSize = true;
            this.lblImmersionConds.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImmersionConds.Location = new System.Drawing.Point(18, 18);
            this.lblImmersionConds.Name = "lblImmersionConds";
            this.lblImmersionConds.Size = new System.Drawing.Size(125, 13);
            this.lblImmersionConds.TabIndex = 3;
            this.lblImmersionConds.Text = "Selected Electrolyte:";
            // 
            // pnlSave
            // 
            this.pnlSave.Controls.Add(this.roundedButton6);
            this.pnlSave.Location = new System.Drawing.Point(3, 388);
            this.pnlSave.Name = "pnlSave";
            this.pnlSave.Size = new System.Drawing.Size(508, 36);
            this.pnlSave.TabIndex = 5;
            this.pnlSave.Visible = false;
            // 
            // roundedButton6
            // 
            this.roundedButton6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundedButton6.Location = new System.Drawing.Point(255, 3);
            this.roundedButton6.Name = "roundedButton6";
            this.roundedButton6.Size = new System.Drawing.Size(250, 30);
            this.roundedButton6.TabIndex = 5;
            this.roundedButton6.Text = "Save and Draw Bounding Box >>>";
            this.roundedButton6.UseVisualStyleBackColor = true;
            this.roundedButton6.Click += new System.EventHandler(this.roundedButton6_Click);
            // 
            // tabBoundBox
            // 
            this.tabBoundBox.BackColor = System.Drawing.Color.LightSlateGray;
            this.tabBoundBox.Controls.Add(this.roundedButton10);
            this.tabBoundBox.Controls.Add(this.roundedButton9);
            this.tabBoundBox.Controls.Add(this.roundedButton8);
            this.tabBoundBox.Controls.Add(this.roundedButton7);
            this.tabBoundBox.Controls.Add(this.label6);
            this.tabBoundBox.Controls.Add(this.label1);
            this.tabBoundBox.Controls.Add(this.tbBoundBox);
            this.tabBoundBox.Controls.Add(this.lblBoundBox);
            this.tabBoundBox.Controls.Add(this.gradientPanel3);
            this.tabBoundBox.Location = new System.Drawing.Point(4, 22);
            this.tabBoundBox.Name = "tabBoundBox";
            this.tabBoundBox.Padding = new System.Windows.Forms.Padding(3);
            this.tabBoundBox.Size = new System.Drawing.Size(527, 528);
            this.tabBoundBox.TabIndex = 1;
            this.tabBoundBox.Tag = "boundingbox";
            this.tabBoundBox.Text = "Bounding Box";
            // 
            // roundedButton10
            // 
            this.roundedButton10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundedButton10.Location = new System.Drawing.Point(73, 366);
            this.roundedButton10.Name = "roundedButton10";
            this.roundedButton10.Size = new System.Drawing.Size(373, 37);
            this.roundedButton10.TabIndex = 21;
            this.roundedButton10.Text = "Process and Generate Model Suggestions >>>";
            this.roundedButton10.UseVisualStyleBackColor = true;
            this.roundedButton10.Click += new System.EventHandler(this.roundedButton10_Click);
            // 
            // roundedButton9
            // 
            this.roundedButton9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundedButton9.Location = new System.Drawing.Point(136, 213);
            this.roundedButton9.Name = "roundedButton9";
            this.roundedButton9.Size = new System.Drawing.Size(246, 39);
            this.roundedButton9.TabIndex = 20;
            this.roundedButton9.Text = "Roll Back Bounding Box";
            this.roundedButton9.UseVisualStyleBackColor = true;
            this.roundedButton9.Click += new System.EventHandler(this.roundedButton9_Click);
            // 
            // roundedButton8
            // 
            this.roundedButton8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundedButton8.Location = new System.Drawing.Point(136, 168);
            this.roundedButton8.Name = "roundedButton8";
            this.roundedButton8.Size = new System.Drawing.Size(246, 39);
            this.roundedButton8.TabIndex = 19;
            this.roundedButton8.Text = "Draw Box Using Buffer";
            this.roundedButton8.UseVisualStyleBackColor = true;
            this.roundedButton8.Click += new System.EventHandler(this.roundedButton8_Click);
            // 
            // roundedButton7
            // 
            this.roundedButton7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundedButton7.Location = new System.Drawing.Point(136, 123);
            this.roundedButton7.Name = "roundedButton7";
            this.roundedButton7.Size = new System.Drawing.Size(246, 39);
            this.roundedButton7.TabIndex = 18;
            this.roundedButton7.Text = "Draw Box - Minimum Size  Possible";
            this.roundedButton7.UseVisualStyleBackColor = true;
            this.roundedButton7.Click += new System.EventHandler(this.roundedButton7_Click);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(78, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(368, 68);
            this.label6.TabIndex = 16;
            this.label6.Text = "Environment Area Selection";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gradientPanel3
            // 
            this.gradientPanel3.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gradientPanel3.ColorBottom = System.Drawing.Color.LightSlateGray;
            this.gradientPanel3.ColorTop = System.Drawing.Color.LightSteelBlue;
            this.gradientPanel3.Location = new System.Drawing.Point(-4, -2);
            this.gradientPanel3.Name = "gradientPanel3";
            this.gradientPanel3.Size = new System.Drawing.Size(535, 532);
            this.gradientPanel3.TabIndex = 29;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(135, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 16);
            this.label1.TabIndex = 5;
            // 
            // tbBoundBox
            // 
            this.tbBoundBox.Location = new System.Drawing.Point(305, 91);
            this.tbBoundBox.Name = "tbBoundBox";
            this.tbBoundBox.Size = new System.Drawing.Size(100, 20);
            this.tbBoundBox.TabIndex = 1;
            this.tbBoundBox.Enter += new System.EventHandler(this.tbBoundBox_Enter);
            // 
            // lblBoundBox
            // 
            this.lblBoundBox.AutoSize = true;
            this.lblBoundBox.BackColor = System.Drawing.Color.Transparent;
            this.lblBoundBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBoundBox.Location = new System.Drawing.Point(99, 94);
            this.lblBoundBox.Name = "lblBoundBox";
            this.lblBoundBox.Size = new System.Drawing.Size(201, 13);
            this.lblBoundBox.TabIndex = 0;
            this.lblBoundBox.Text = "Specify the Environmental Buffer: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 552);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(173, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Selected Part-Material Assignments";
            this.label4.Visible = false;
            // 
            // lblPartName
            // 
            this.lblPartName.AutoSize = true;
            this.lblPartName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPartName.Location = new System.Drawing.Point(16, 484);
            this.lblPartName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPartName.Name = "lblPartName";
            this.lblPartName.Size = new System.Drawing.Size(137, 13);
            this.lblPartName.TabIndex = 14;
            this.lblPartName.Text = "Selected Part Name is:";
            this.lblPartName.Visible = false;
            // 
            // tbPartName
            // 
            this.tbPartName.Enabled = false;
            this.tbPartName.Location = new System.Drawing.Point(26, 517);
            this.tbPartName.Name = "tbPartName";
            this.tbPartName.Size = new System.Drawing.Size(246, 20);
            this.tbPartName.TabIndex = 13;
            this.tbPartName.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(242, 552);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 11;
            // 
            // lblMaterial
            // 
            this.lblMaterial.AutoSize = true;
            this.lblMaterial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaterial.Location = new System.Drawing.Point(165, 482);
            this.lblMaterial.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMaterial.Name = "lblMaterial";
            this.lblMaterial.Size = new System.Drawing.Size(235, 13);
            this.lblMaterial.TabIndex = 10;
            this.lblMaterial.Text = "Please select a material for component: ";
            this.lblMaterial.Visible = false;
            // 
            // btnApplyMaterial
            // 
            this.btnApplyMaterial.Location = new System.Drawing.Point(424, 500);
            this.btnApplyMaterial.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.btnApplyMaterial.Name = "btnApplyMaterial";
            this.btnApplyMaterial.Size = new System.Drawing.Size(80, 40);
            this.btnApplyMaterial.TabIndex = 7;
            this.btnApplyMaterial.Text = "Apply";
            this.btnApplyMaterial.UseVisualStyleBackColor = true;
            this.btnApplyMaterial.Visible = false;
            // 
            // TaskpaneHostUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.tbPartName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnApplyMaterial);
            this.Controls.Add(this.lblPartName);
            this.Controls.Add(this.lblMaterial);
            this.Controls.Add(this.label3);
            this.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.Name = "TaskpaneHostUI";
            this.Size = new System.Drawing.Size(552, 591);
            this.tabControl1.ResumeLayout(false);
            this.tabMaterials.ResumeLayout(false);
            this.gradientPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvParts)).EndInit();
            this.tabEnv.ResumeLayout(false);
            this.tabEnv.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.pnlEnvGroup.ResumeLayout(false);
            this.pnlEnvGroup.PerformLayout();
            this.pnlEnvConds.ResumeLayout(false);
            this.pnlEnvConds.PerformLayout();
            this.pnlElectrolyte.ResumeLayout(false);
            this.pnlElectrolyte.PerformLayout();
            this.pnlSave.ResumeLayout(false);
            this.tabBoundBox.ResumeLayout(false);
            this.tabBoundBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabEnv;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel pnlEnvGroup;
        private System.Windows.Forms.Label lblEnvGroup;
        private System.Windows.Forms.ComboBox cmbEnvGroup;
        private System.Windows.Forms.Panel pnlEnvConds;
        private System.Windows.Forms.TabPage tabBoundBox;
        private System.Windows.Forms.Label lblEnvConds;
        private System.Windows.Forms.Panel pnlElectrolyte;
        private System.Windows.Forms.Label lblAvailImmCond;
        private System.Windows.Forms.Label lblImmersionConds;
        private System.Windows.Forms.Button btnClearEnv;
        private System.Windows.Forms.ListView lvEnvConds;
        private System.Windows.Forms.ColumnHeader ID;
        private System.Windows.Forms.ColumnHeader ConditionName;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.ColumnHeader Units;
        private System.Windows.Forms.ListView lvImmConds;
        private System.Windows.Forms.ColumnHeader elect_cond_id;
        private System.Windows.Forms.ColumnHeader ele_cond_type;
        private System.Windows.Forms.ColumnHeader ele_cond_value;
        private System.Windows.Forms.ColumnHeader ele_cond_units;
        private System.Windows.Forms.TextBox tbElectrolyte;
        private System.Windows.Forms.TextBox tbBoundBox;
        private System.Windows.Forms.Label lblBoundBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblMaterial;
        private System.Windows.Forms.Button btnApplyMaterial;
        private System.Windows.Forms.Label lblPartName;
        private System.Windows.Forms.TextBox tbPartName;
        private System.Windows.Forms.Label label4;

        private System.Windows.Forms.Panel pnlSave;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private RoundedButton roundedButton6;
        private RoundedButton roundedButton9;
        private RoundedButton roundedButton8;
        private RoundedButton roundedButton7;
        private RoundedButton roundedButton10;
        private TabPage tabMaterials;
        private RoundedButton roundedButton4;
        private RoundedButton roundedButton3;
        private RoundedButton roundedButton2;
        private RoundedButton roundedButton1;
        private DataGridView dgvParts;
        private Label label2;
        private GradientPanelDemo.GradientPanel gradientPanel1;
        private GradientPanelDemo.GradientPanel gradientPanel2;
        private GradientPanelDemo.GradientPanel gradientPanel3;

        public DataGridViewCellEventHandler CurrentCellDirtyStateChanged { get; private set; }
    }
}
