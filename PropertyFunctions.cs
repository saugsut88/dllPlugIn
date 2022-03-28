using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;
using View = SolidWorks.Interop.sldworks.View;
using VPS.SWplugin.ICT.Parts;

namespace VPS.SWplugin.ICT

{
    [ComVisible(true)]
    [ProgId(TaskpaneIntegration.SWTASKPANE_PROGID)]
    class PropertyFunctions
    {
        public Utilities.partInformation partInfo = new Utilities.partInformation();
        private Utilities.DBConnector dbConnect = new Utilities.DBConnector();
        public Utilities.Interferences interferences = new Utilities.Interferences();
        

        public const string SWTASKPANE_PROGID = "VPS.SWPlugin.ICT.Taskpane";
        ISldWorks swApp;
        ModelDoc2 swDoc;

        // used in openDocIteratorMaterialSearch()
        private DataTable dtPartMaterials = new DataTable();
        int foundMaterialID = 0;
        private bool IsIndivCompMatSearch;
        // variable to store saved material matches when individual components are selected
        public DataTable SavedPartMaterialTable = new DataTable();
        // variable to store partname and SW material name
        public DataTable PartSWMaterialTable = new DataTable();


        public string partName;
        public string materialName;
        // The SldWorks swApp variable is pre-assigned for you.
      //  string tempMaterialName;
        ModelDoc2 swModel;

        #region Private Members

        private const string CustomPropertyDescription = "Description";

        #endregion

        // class Constructor
        public PropertyFunctions()
        {
            SavedPartMaterialTable = BuildPartMaterialTable();
            dtPartMaterials = BuildPartMaterialTable();
            PartSWMaterialTable = BuildPartSWMaterialTable();
        }


        //get PartName from a selected model

        #region Read Part name
        public string readPartName()
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
            Debug.Print("Number of open documents in this SolidWorks session: " + count);
            models = (object[])swApp.GetDocuments();

            for (index = 0; index < count; index++)
            {
                swModel = models[index] as ModelDoc2;
                pathName = swModel.GetPathName();
                Debug.Print("Path and name of open document: " + pathName);
            }

            //seperate a path name
            string tempFileName = Path.GetFileName(pathName);

            swDoc = swApp.IActivateDoc3(tempFileName, false, ref errors);
            SelectionMgr swSelectionMgr = default(SelectionMgr);
            swSelectionMgr = (SelectionMgr)swDoc.SelectionManager;

            Entity swEntity = default(Entity);


            swEntity = (Entity)swSelectionMgr.GetSelectedObject6(1, -1);

            Component2 swComponent = default(Component2);

            try
            {
                swComponent = (Component2)swEntity.GetComponent();
            }
            catch (Exception e)
            {
                string message = "No object selected, Please select an object to add to the corrosion model.";
                string title = "Selection Error";
                MessageBox.Show(message, title);
            }

            if (swComponent == null)
            {
                partName = "";
                return "";
            }
            else
            {  
                string partName = swComponent.Name2;    
                //remove -1 from part name
                string appendedpartName = partName.Split('-')[0].Trim();

                if (partInfo.StoredComponents.Count == 0)
                {
                    partInfo.StoredComponents.Add(appendedpartName);
                    return appendedpartName;
                }
                else
                {
                    //check if part name is already in list
                    for (int z = 0; z < partInfo.StoredComponents.Count; z++)
                    {
                        if (partInfo.StoredComponents.Contains(appendedpartName))
                        {
                            MessageBox.Show(appendedpartName + " already selected, please check selection or component name.");
                        }
                        else
                        {
                            partInfo.StoredComponents.Add(appendedpartName);
                            return appendedpartName;
                        }
                        return "";
                    }
                }
                return "";

            }
            swDoc.ForceRebuild3(true);
        }//end read part name
        #endregion

        #region Select all
        //select all componenets of an assembly drawing

        public void selectAllParts()
        {

            ModelDocExtension swModelDocExt;
            SelectionMgr swSelMgr;
            
            int warnings = 0;

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
            Debug.Print("Number of open documents in this SolidWorks session: " + count);
            models = (object[])swApp.GetDocuments();

            for (index = 0; index < count; index++)
            {
                swModel = models[index] as ModelDoc2;
                pathName = swModel.GetPathName();
                Debug.Print("Path and name of open document: " + pathName);
            }

            //seperate a path name
            string tempFileName = Path.GetFileName(pathName);

            swModel = (ModelDoc2)swApp.OpenDoc6(pathName, (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
            swModelDocExt = (ModelDocExtension)swModel.Extension;
            swSelMgr = (SelectionMgr)swModel.SelectionManager;
            //Select all components in assembly

            //call SelectAllinDocument below
            SelectAllinDocument(swModel, swModelDocExt, swSelMgr, pathName);

        }//end select all 



        public void SelectAllinDocument(ModelDoc2 swModel, ModelDocExtension swModelDocExt, SelectionMgr swSelMgr, string pathName)
        {
            int selCount;
            string pathNameVar = pathName;
            int errors = 0;
            int warnings = 0;

            // MP 12/9/21 - commented these lines out - this one was not used Utilities.partInformation StoredPartInfo = new Utilities.partInformation();
            // This one directly referenced the list with use of static - Utilities.partInformation.StoredComponents.Clear();
            partInfo.StoredComponents.Clear();

            // Select all edges in a part, all components in an assembly,
            // or all entities in a drawing
            swModelDocExt.SelectAll();


            // Get and print the number of selections
            selCount = 0;
            selCount = swSelMgr.GetSelectedObjectCount2(-1);

            swApp.OpenDoc6(pathNameVar, (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
            Entity swEntity = default(Entity);
            Component2 swComponent = default(Component2);

            //create array as large as the count of selected Components
            object[] SWcomponentsArray = new object[selCount];

            switch (swModel.GetType())
            {
                case (int)swDocumentTypes_e.swDocPART:
                    Debug.Print("Number of edges selected in part          = " + selCount);
                    break;
                case (int)swDocumentTypes_e.swDocASSEMBLY:
                    Debug.Print("Number of components selected in assembly = " + selCount);
                    string message = ("Number of components selected in assembly = " + selCount);
                    string title = "Component count";
                    MessageBox.Show(message, title);

                    swApp = (ISldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
                    int errorsx = 0;

                    ModelDoc2 swModelx;
                    object[] models;
                    int count;
                    int index;
                    string pathNamex = "";

                    //get all of the active docs
                    count = swApp.GetDocumentCount();
                    Debug.Print("Number of open documents in this SolidWorks session: " + count);
                    models = (object[])swApp.GetDocuments();
                    int indexNum = 1;



                    for (index = 0; index < count - 1; index++)
                    {
                        swModelx = models[index] as ModelDoc2;
                        pathNamex = swModelx.GetPathName();
                        Debug.Print("Path and name of open document: " + pathNamex);

                        string tempFileName = Path.GetFileNameWithoutExtension(pathNamex);

                        //MessageBox.Show("component name " + tempFileName);

                        //store part in list, to convert to array use ".ToArray();"
                        // MP 12/9 replaced with following line of code - Utilities.partInformation.StoredComponents.Add(tempFileName);
                        partInfo.StoredComponents.Add(tempFileName);


                        indexNum++;

                    }

                    //add de-Select----Works!!!!
                    swModel.ClearSelection2(true);

                    break;
                case (int)swDocumentTypes_e.swDocDRAWING:
                    Debug.Print("Number of entities selected in drawing    = " + selCount);
                    break;
                default:
                    Debug.Print("Unknown type of document.");
                    break;
            }

        }//end SelectAllinDocument func

        #endregion select all


        #region Highlight part by name 
        //Selected part name from table is fed to function, function highlights selected part on User Gui
        public void HighlightPartbyName(string partNametoSelect)
        {
            string partToHighlight = partNametoSelect;

            //set instance of solidworks
            ModelDocExtension swModelDocExt;
            SelectionMgr swSelMgr;

            int warnings = 0;

            swApp = (ISldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
            int errors = 0;

            ModelDoc2 swModel;
            object[] models;
            int count;
            int index;
            string pathName = "";

            //get all of the active docs
            count = swApp.GetDocumentCount();
            Debug.Print("Number of open documents in this SolidWorks session: " + count);
            models = (object[])swApp.GetDocuments();

            for (index = 0; index < count; index++)
            {
                swModel = models[index] as ModelDoc2;
                pathName = swModel.GetPathName();
                Debug.Print("Path and name of open document: " + pathName);
            }

            //seperate a path name
            string tempFileName = Path.GetFileName(pathName);
            string tempFileNameNoEXT = Path.GetFileNameWithoutExtension(pathName);

            swModel = (ModelDoc2)swApp.OpenDoc6(pathName, (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
            swModelDocExt = (ModelDocExtension)swModel.Extension;
            swSelMgr = (SelectionMgr)swModel.SelectionManager;

            //clear current selection of part if any
            swModel.ClearSelection2(true);


            //highlight part by name
            ModelDocExtension swExt;
            SelectionMgr swSelMgr2;
            bool boolstatus;
            swExt = swModel.Extension;
            swSelMgr2 = (SelectionMgr)swModel.SelectionManager;



            boolstatus = swExt.SelectByID2(partNametoSelect + "-1@" + tempFileNameNoEXT, "COMPONENT", 0, 0, 0, false, 0, null, 0);




        }//end HighlightPartbyName
        #endregion

        #region apply part values NOT FUNCTIONAL NOT USED
        public void applyPartValues()
        {
            //addref
            TaskpaneHostUI taskPane = new TaskpaneHostUI();
            //partInformation part = new partInformation();

            //apply material selection, store in variable
            //taskPane.checkedListBox1.Items.Add(partName);

            //Add variables to object
            partInfo.partNameObj = partName;
            partInfo.partMaterialObj = materialName;

            //need to re work to create an object.
            //hide to ignore error partinfo.partsList.Add(partInformation.partNameObj);
            //hide to ignore error  partinfo.partsList.Add(partInformation.partMaterialObj);

            //store objected in linked list
            //partInformation.partsList.Add(part);

            //reset material and part name to null
            partName = "";
            materialName = "";

        }
        #endregion



        #region Detect mates in an assembly
        //reads all available mates on objects displayed returns a linked list with associated data per mate
        //type, name, entitiy, XYZ of mate, vectos IJK of mate
        public string mateRead(List<MaterialSelection> partSelectionInformation)
        {
            var sbMateData = new System.Text.StringBuilder();

            bool flagged90Angle = false;

            swApp = (ISldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
            int errors = 0;

            ModelDoc2 swModel;
            object[] models;
            int count;
            int index;
            string pathName = "";


            Configuration swConfig;
            ModelDocExtension swModelDocExt;
            Entity swEntity;
            SelectionMgr swSelMgr;
            RenderMaterial swRenderMaterial;

            int warnings = 0;

            //get all of the active docs
            count = swApp.GetDocumentCount();
            Debug.Print("Number of open documents in this SolidWorks session: " + count);
            models = (object[])swApp.GetDocuments();

            for (index = 0; index < count; index++)
            {
                swModel = models[index] as ModelDoc2;
                pathName = swModel.GetPathName();
                Debug.Print("Path and name of open document: " + pathName);
            }

            Feature swFeat = default(Feature);
            Feature swMateFeat = null;
            Feature swSubFeat = default(Feature);
            Mate2 swMate = default(Mate2);
            Component2 swComp = default(Component2);
            MateEntity2[] swMateEnt = new MateEntity2[3];
            string fileName = null;

            int i = 0;
            double[] entityParameters = new double[8];

            string extension = Path.GetExtension(pathName);
            string openError = "No coincidents found for Part Doc";
            if (extension.ToUpper() != ".SLDASM")
            {
                MessageBox.Show("No coincidents found for Part Doc, must use an assembly document");
                return openError;
            }

            swModel = (ModelDoc2)swApp.OpenDoc6(pathName, (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);

            //Get the first feature in the assembly
            swFeat = (Feature)swModel.FirstFeature();
            
            //Iterate over features in FeatureManager design tree
            while ((swFeat != null))
            {
                if ("MateGroup" == swFeat.GetTypeName())
                {
                    swMateFeat = (Feature)swFeat;
                    break;
                }
                swFeat = (Feature)swFeat.GetNextFeature();
            }
            Debug.Print("  " + swMateFeat.Name);
            Debug.Print("");

            string previousCoincedent = "";
            string comp1Material = "";
            string comp2Material = "";

            //Get first mate, which is a subfeature
            swSubFeat = (Feature)swMateFeat.GetFirstSubFeature();
            while ((swSubFeat != null))
            {
                swMate = (Mate2)swSubFeat.GetSpecificFeature2();
                if ((swMate != null))
                {

                    for (i = 0; i <= 1; i++)
                    {

                        swMateEnt[i] = swMate.MateEntity(i);
                        swComp = (Component2)swMateEnt[i].ReferenceComponent;
                        entityParameters = (double[])swMateEnt[i].EntityParams;
                       
                        string mateType = "";
                        int typeOfMate = swMate.Type;
                        switch (typeOfMate)
                        {
                            case 0:
                                mateType = "Coincident";
                                break;
                            case 1:
                                mateType = "Concentric";
                                break;
                            case 2:
                                mateType = "Perpendicular";
                                break;
                            case 3:
                                mateType = "Parallel";
                                break;
                            case 4:
                                mateType = "Tangent";
                                break;
                            case 5:
                                mateType = "Distance";
                                break;
                            case 6:
                                mateType = "Angle";
                                break;
                            case 7:
                                mateType = "Unknown";
                                break;
                            case 8:
                                mateType = "Symmetric";
                                break;
                            case 9:
                                mateType = "CAM follower";
                                break;
                            case 10:
                                mateType = "Gear";
                                break;
                            case 11:
                                mateType = "Width";
                                break;
                            case 12:
                                mateType = "Lock to sketch";
                                break;
                            case 13:
                                mateType = "Rack pinion";
                                break;
                            case 14:
                                mateType = "Max mates";
                                break;
                            case 15:
                                mateType = "Path";
                                break;
                            case 16:
                                mateType = "Lock";
                                break;
                            case 17:
                                mateType = "Screw";
                                break;
                            case 18:
                                mateType = "Linear coupler";
                                break;
                            case 19:
                                mateType = "Universal joint";
                                break;
                            case 20:
                                mateType = "Coordinate";
                                break;
                            case 21:
                                mateType = "Slot";
                                break;
                            case 22:
                                mateType = "Hinge";
                                // 
                                break;
                                // Add new mate types introduced after SOLIDWORKS 2010 FCS here 
                        }

                        if (mateType == "Perpendicular")
                        {
                            flagged90Angle = true;
                        }

                        string mateEntType = "";
                        int entType = swMateEnt[i].ReferenceType;
                        switch (entType)
                        {
                            case 0:
                                mateEntType = "Unsupported";
                                break;
                            case 1:
                                mateEntType = "Point";
                                break;
                            case 2:
                                mateEntType = "Line";
                                break;
                            case 3:
                                mateEntType = "Plane";
                                break;
                            case 4:
                                mateEntType = "Cylinder";
                                break;
                            case 5:
                                mateEntType = "Cone";
                                break;
                            case 6:
                                mateEntType = "Sphere";
                                break;
                            case 7:
                                mateEntType = "Circle";
                                break;
                        }

                        string alignmentType = "";
                        int alignType = swMate.Alignment;
                        switch (alignType)
                        {
                            case 0:
                                alignmentType = "Aligned";
                                break;
                            case 1:
                                alignmentType = "Anti Aligned";
                                break;
                            case 2:
                                alignmentType = "Closest";
                                break;
                        }

                        double x = (double)entityParameters[0];
                        double y = (double)entityParameters[1];
                        double z = (double)entityParameters[2];

                        string sotredNameCompare = swSubFeat.Name;

                        string PartNameforCompare = swComp.Name2;

                        var digits = new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

                        PartNameforCompare = PartNameforCompare.TrimEnd(digits);

                        //find component material data from list OBJ
                        if ((previousCoincedent != swSubFeat.Name))
                        {
                            var item = partSelectionInformation.FirstOrDefault(o => o.PartName == PartNameforCompare);
                            if (item != null)
                            {
                                comp1Material = item.MaterialName.ToString();
                            }
                        }

                        if (previousCoincedent == swSubFeat.Name)
                        {
                            string PartNameforCompare2 = swComp.Name2;
                            var digits2 = new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

                            PartNameforCompare2 = PartNameforCompare2.TrimEnd(digits);

                            var item2 = partSelectionInformation.FirstOrDefault(o => o.PartName == PartNameforCompare2);
                            if (item2 != null)
                            {
                                comp2Material = item2.MaterialName.ToString();
                            }

                            sbMateData.AppendLine("Ref Component 2:    " + swComp.Name2 + "\n" +
                                                  "Material:           " + comp2Material + "\n")  ;

                            //MessageBox.Show(sbMateData.ToString());

                            if ((comp1Material != comp2Material) && (comp1Material != "") && (comp2Material != ""))
                            {

                                if ((comp1Material != "non-conductive material") && (comp2Material != "non-conductive material"))
                                {
                                    if ((comp1Material != "non-conductive") && (comp2Material != "non-conductive"))
                                    {
                                        sbMateData.AppendLine("Galvanic couple recognized" + "\n");
                                    }
                                }

                            }

                            //MessageBox.Show(sbMateData.ToString());

                        }
                        else
                        {
                            sbMateData.AppendLine("Feature Name:      " + swSubFeat.Name + "\n" +
                                                    "Type:              " + mateType + "\n" +
                                                    "Alignment:          " + alignmentType + "\n" +
                                                    "Can be Flipped?:    " + swMate.CanBeFlipped + "\n" +
                                                    "Mate Entity Type:   " + mateEntType + "\n" +
                                                    "Ref Component 1:    " + swComp.Name2 + "\n" + 
                                                    "Material:           " + comp1Material);
                                                    
                        }

                        previousCoincedent = swSubFeat.Name;

                    }
                    //MessageBox.Show(sbMateData.ToString());

                    Debug.Print(" ");
                }
                // Get the next mate in MateGroup
                swSubFeat = (Feature)swSubFeat.GetNextSubFeature();

            }
            string readout = sbMateData.ToString();
            string coincedentNone = "No coincidents found";

            if (readout == "")
            {
                //MessageBox.Show("No coincidents found");
                return coincedentNone;
            }
            else
            {

                //MessageBox.Show(readout);

                return readout;
            }
        }//end read mate function
        #endregion 

        #region DetectComponenet interference
        //detect collsion between components in an assembly
        public List<Utilities.Interferences> DetectInterfearence()
        { //set instance of solidworks
            var listOfObjects = interferences.listOfInterferenceData;
            System.Text.StringBuilder DataStringbuilder = new System.Text.StringBuilder("");

            ModelDocExtension swModelDocExt;
            SelectionMgr swSelMgr;

            int warnings = 0;

            swApp = (ISldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
            int errors = 0;

            ModelDoc2 swModel;
            object[] models;
            int count;
            int index;
            string pathName = "";

            //get all of the active docs
            count = swApp.GetDocumentCount();
            Debug.Print("Number of open documents in this SolidWorks session: " + count);
            models = (object[])swApp.GetDocuments();

            for (index = 0; index < count; index++)
            {
                swModel = models[index] as ModelDoc2;
                pathName = swModel.GetPathName();
                Debug.Print("Path and name of open document: " + pathName);
            }

            //seperate a path name
            string tempFileName = Path.GetFileName(pathName);
            string tempFileNameNoEXT = Path.GetFileNameWithoutExtension(pathName);

            int numbOfInterferences;

            string extension = Path.GetExtension(pathName);
            string openError = "No interferences found for Part Doc";
            if (extension.ToUpper() != ".SLDASM")
            {
                numbOfInterferences = 0;
                MessageBox.Show("No coincidents found for Part Doc, must use an assembly document");
                
                listOfObjects.Add(new Utilities.Interferences(numbOfInterferences, openError));
                return listOfObjects;//list ;
            }

            swModel = (ModelDoc2)swApp.OpenDoc6(pathName, (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);

            swModelDocExt = (ModelDocExtension)swModel.Extension;
            swSelMgr = (SelectionMgr)swModel.SelectionManager;

            //start interfearence variable assignemnt
            Object[] vInts = null;
            Object[] vComps = null;
            IInterference interference = default(IInterference);
            Component2 comp = default(Component2);

            InterferenceDetectionMgr pIntMgr = default(InterferenceDetectionMgr);

            ModelDoc2 swModelDoc = default(ModelDoc2);
            swModelDoc = (ModelDoc2)swApp.ActiveDoc;
            AssemblyDoc swAssemblyDoc = default(AssemblyDoc);
            swAssemblyDoc = (AssemblyDoc)swModelDoc;

            //AssemblyDoc swAssemblyDoc = default(AssemblyDoc);                           //swAssemblyDoc executed to null       
            pIntMgr = swAssemblyDoc.InterferenceDetectionManager;       //returned null collapse here

            //interfearecne detection
            // Specify the interference detection settings and options
            pIntMgr.TreatCoincidenceAsInterference = false;
            pIntMgr.TreatSubAssembliesAsComponents = true;
            pIntMgr.IncludeMultibodyPartInterferences = true;
            pIntMgr.MakeInterferingPartsTransparent = false;
            pIntMgr.CreateFastenersFolder = true;
            pIntMgr.IgnoreHiddenBodies = true;
            pIntMgr.ShowIgnoredInterferences = false;

            //Display 
            pIntMgr.NonInterferingComponentDisplay = (int)swNonInterferingComponentDisplay_e.swNonInterferingComponentDisplay_Wireframe;

            // Run interference detection
            vInts = (object[])pIntMgr.GetInterferences();

            //double interferenceVolume = (interference.Volume * 1000000000);

            //tester
            //MessageBox.Show("# of interferences: " + pIntMgr.GetInterferenceCount());
            numbOfInterferences = pIntMgr.GetInterferenceCount();
            string data = "";

            if (numbOfInterferences == 0)
            {
                data = "No Interferences Found";
                listOfObjects.Add(new Utilities.Interferences(numbOfInterferences, data));
                pIntMgr.Done();

                return listOfObjects;
            }

            long i = 0;
            long j = 0;
            double vol = 0;

            for (i = 0; i <= vInts.GetUpperBound(0); i++)
            {

                interference = (Interference)vInts[i];
                vol = interference.Volume;
                double volMM3 = (vol * 1000000000);

                //ignore interference under 0.003MM cubed
                if (volMM3 >= 0.003)
                {
                    //MessageBox.Show("# of interferences: " + pIntMgr.GetInterferenceCount() + " \n" +
                         //"Interference number: " + (i + 1) + "\n" +
                        // "Number of components in this interference: " + interference.GetComponentCount());

                    DataStringbuilder.Append(("Interference number: " + (i + 1) + " \n" +
                         "Number of components in this interference: " + interference.GetComponentCount() + "\n"));
                    DataStringbuilder.Append("Components: " + " \n");

                    vComps = (object[])interference.Components;
                    for (j = 0; j <= vComps.GetUpperBound(0); j++)
                    {
                        comp = (Component2)vComps[j];
                        string compName = comp.Name2;

                        var digits = new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

                        compName = compName.TrimEnd(digits);

                        //MessageBox.Show("     " + compName);
                        DataStringbuilder.Append("     " + compName + " \n");

                    }

                    //MessageBox.Show("Interference volume is " + (vol * 1000000000) + " mm^3");
                    DataStringbuilder.Append("Interference volume is " + (vol * 1000000000) + " mm^3" + " \n\n");

                }
                
            }

            data = DataStringbuilder.ToString();

            MessageBox.Show(data);
            
            listOfObjects.Add(new Utilities.Interferences(numbOfInterferences, data));

            //must call to un select all parts of the interface
            pIntMgr.Done();

            return listOfObjects;

        }//end detectInterfearence
        #endregion

        #region tree itterator
        //itteratre through the feature tree
        //uses 3 sub functions TraverseFeatureFeatures, TraverseComponentFeatures, TraverseModelFeatures

        public bool holeFlagged;
        public string compNameWithHole;
        public int HoleCount;
        public void treeItterator()
        {
            HoleCount = 0;
            //look for holes in a panel, object, component from an assembly
            //read all components

            //allow access of DLL to the sw assembly
            swApp = (ISldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
            int errors = 0;
            int warnings = 0;

            ModelDoc2 swModel;
            object[] models;
            int count;
            int index;
            string pathName = "";

            //get all of the active docs
            count = swApp.GetDocumentCount();
            Debug.Print("Number of open documents in this SolidWorks session: " + count);
            models = (object[])swApp.GetDocuments();

            for (index = 0; index < count; index++)
            {
                swModel = models[index] as ModelDoc2;
                pathName = swModel.GetPathName();
                Debug.Print("Path and name of open document: " + pathName);
            }

            //seperate a path name
            string tempFileName = Path.GetFileName(pathName);
            string tempFileNameNoEXT = Path.GetFileNameWithoutExtension(pathName);

            swModel = (ModelDoc2)swApp.OpenDoc6(pathName, (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);

            //parse feature tree start
            //ModelDoc2 swModel;
            ConfigurationManager swConfMgr;
            Configuration swConf;
            Component2 swRootComp;

            //swModel = (ModelDoc2)swApp.ActiveDoc;
            swConfMgr = (ConfigurationManager)swModel.ConfigurationManager;
            swConf = (Configuration)swConfMgr.ActiveConfiguration;

            swRootComp = (Component2)swConf.GetRootComponent();

            TraverseModelFeatures(swModel, 1);

            if (swModel.GetType() == (int)swDocumentTypes_e.swDocASSEMBLY)
            {
                TraverseComponent(swRootComp, 1);
            }

        }// end look for holes


        public void TraverseFeatureFeatures(Feature swFeat, long nLevel)
        {
            Feature swSubFeat;
            Feature swSubSubFeat;
            Feature swSubSubSubFeat;
            string sPadStr = " ";
            long i = 0;

            for (i = 0; i <= nLevel; i++)
            {
                sPadStr = sPadStr + " ";
            }
            while ((swFeat != null))
            {

                Debug.Print(sPadStr + swFeat.Name + " [" + swFeat.GetTypeName2() + "]");
                swSubFeat = (Feature)swFeat.GetFirstSubFeature();
                //holeFlagged = false;
                string featName = swFeat.Name.ToString();

                //hole check
                isThisaHole(featName);

                while ((swSubFeat != null))
                {
                    Debug.Print(sPadStr + "  " + swSubFeat.Name + " [" + swSubFeat.GetTypeName() + "]");
                    swSubSubFeat = (Feature)swSubFeat.GetFirstSubFeature();

                    while ((swSubSubFeat != null))
                    {
                        Debug.Print(sPadStr + "    " + swSubSubFeat.Name + " [" + swSubSubFeat.GetTypeName() + "]");
                        swSubSubSubFeat = (Feature)swSubSubFeat.GetFirstSubFeature();

                        while ((swSubSubSubFeat != null))
                        {
                            Debug.Print(sPadStr + "      " + swSubSubSubFeat.Name + " [" + swSubSubSubFeat.GetTypeName() + "]");
                            swSubSubSubFeat = (Feature)swSubSubSubFeat.GetNextSubFeature();

                        }
                        swSubSubFeat = (Feature)swSubSubFeat.GetNextSubFeature();
                    }
                    swSubFeat = (Feature)swSubFeat.GetNextSubFeature();
                }
                swFeat = (Feature)swFeat.GetNextFeature();
            }

        }

        public void TraverseComponentFeatures(Component2 swComp, long nLevel)
        {
            Feature swFeat;
            swFeat = (Feature)swComp.FirstFeature();
            TraverseFeatureFeatures(swFeat, nLevel);
        }
        public void TraverseComponent(Component2 swComp, long nLevel)
        {
            object[] vChildComp;
            Component2 swChildComp;
            string sPadStr = " ";
            long i = 0;

            for (i = 0; i <= nLevel - 1; i++)
            {
                sPadStr = sPadStr + " ";
            }

            vChildComp = (object[])swComp.GetChildren();
            for (i = 0; i < vChildComp.Length; i++)
            {
                swChildComp = (Component2)vChildComp[i];
                Debug.Print(sPadStr + "+" + swChildComp.Name2 + " <" + swChildComp.ReferencedConfiguration + ">");

                TraverseComponentFeatures(swChildComp, nLevel);
                TraverseComponent(swChildComp, nLevel + 1);
            }
        }
        public void TraverseModelFeatures(ModelDoc2 swModel, long nLevel)
        {
            Feature swFeat;
            swFeat = (Feature)swModel.FirstFeature();
            TraverseFeatureFeatures(swFeat, nLevel);
        }

        #endregion


        #region holecheck
        public void isThisaHole(string hole)
        {
            string ishole = hole;

            var digits = new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            ishole = ishole.TrimEnd(digits);

            if (ishole == "Hole")
            {
                holeFlagged = true;
                HoleCount = HoleCount + 1;
                
                //MessageBox.Show("hole found");
            }

        }
        #endregion

        #region read pre assigned material NOT USED
        //read pre assigned material does not work at the moment
        public void readPreAssignedMaterial(string preMaterial)
        {
            string materialAssigned = preMaterial;

            var digits = new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            materialAssigned = materialAssigned.TrimEnd(digits);

            switch (materialAssigned)
            {
                case "AL 5083":
                    MessageBox.Show("Material Found in database" + materialAssigned);
                    break;
                case "AL 5456":
                    MessageBox.Show("Material Found in database" + materialAssigned);
                    break;
                case "DH36 Steel":
                    MessageBox.Show("Material Found in database" + materialAssigned);
                    break;
                case "HY-80 Steel":
                    MessageBox.Show("Material Found in database" + materialAssigned);
                    break;
                case "HY 100 Steel":
                    MessageBox.Show("Material Found in database" + materialAssigned);
                    break;
                case "CRES 316L":
                    MessageBox.Show("Material Found in database" + materialAssigned);
                    break;
                case "Zinc":
                    MessageBox.Show("Material Found in database" + materialAssigned);
                    break;
                case "Zinc AC41A Alloy, As Cast":
                    MessageBox.Show("Material Found in database" + materialAssigned);
                    break;
            }
            //MessageBox.Show("Material Not Found Please Manualy Select Material");


        }// end readPreassignedMaterial Func
        #endregion


        #region isThatAPartName NOT USED
        //Read the swFeat name and compare it to the stroed parts list to see if it is a part
        public string isThatAPartName(string isItAPart)
        {
            string partNameCompare = isItAPart;

            var digits = new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            partNameCompare = partNameCompare.TrimEnd(digits);

            for (int i = 0; i < partInfo.StoredComponents.Count; i++)
            {
                if (partNameCompare == partInfo.StoredComponents[i])
                {
                    return partNameCompare;

                }
            }

            return null;

        }
        #endregion


        #region Read Materials V2


        public DataTable openDocIteratorMaterialSearch(string partName)
        {
            // Clear out public table to return back to TaskpaneHostUI
            if (dtPartMaterials != null && dtPartMaterials.Rows.Count > 0)
            {
                dtPartMaterials.Rows.Clear();
            }

            // Set bool whether this is an individual comp search or All
            if (partName == string.Empty)
            {
                IsIndivCompMatSearch = false;
            }
            else
            {
                IsIndivCompMatSearch = true;
            }

            swApp = (ISldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));

            ModelDoc2 swModel;
            object[] models;
            int count;
            int index;
            string pathName = "";

            //get all of the active docs
            count = swApp.GetDocumentCount();
            Debug.Print("Number of open documents in this SolidWorks session: " + count);
            models = (object[])swApp.GetDocuments();

            // string assemblyDocExt = ".sldasm";
            string partDocExt = ".sldprt";

            StringBuilder strBldr = new StringBuilder();
            for (index = 0; index < count; index++)
            {
                swModel = models[index] as ModelDoc2;
                pathName = swModel.GetPathName();
                Debug.Print("Path and name of open document: " + pathName);
                //seperate a path name
                string partExtension = Path.GetExtension(pathName);
                string tempFileName = Path.GetFileName(pathName);
                string tempFileNameNoEXT = Path.GetFileNameWithoutExtension(pathName);

                //ignore assembly doc extension and any other extension. Process only Part docs.
                if (partExtension.Equals(partDocExt, StringComparison.OrdinalIgnoreCase))
                {
                    // check if this is an individual component search, if so just get the material match for that part
                    if (IsIndivCompMatSearch)
                    {
                        if (partName.Equals(tempFileNameNoEXT, StringComparison.OrdinalIgnoreCase))
                        {
                            //open part doc and read pre assigned material
                            strBldr.AppendLine(readMaterialV2(pathName, tempFileNameNoEXT).ToString());
                            // break out of loop since we found the correct part doc
                            break;
                        }
                    }
                    else
                    {
                        // OTHERWISE - open part doc associated with assembly doc and read the part and pre assigned material
                        strBldr.AppendLine(readMaterialV2(pathName, tempFileNameNoEXT).ToString());
                    }
                }
            }
            MessageBox.Show(strBldr.ToString(), "Material Comparison");
            // dtPartMaterials is populated in the readMaterialV2 method call. Pass this back to TaskPaneHostUI for processing
            return dtPartMaterials;
        } //end openDocItteratorMaterialSearch

        private DataTable BuildPartMaterialTable()
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("MaterialID", typeof(int));
            dt.Columns.Add("PartName", typeof(String));

            dt.PrimaryKey = new DataColumn[] { dt.Columns["PartName"] };
            return dt;
        } // end   private DataTable BuildPartMaterialTable()


        private DataTable BuildPartSWMaterialTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SWMaterial", typeof(String));
            dt.Columns.Add("PartName", typeof(String));

            dt.PrimaryKey = new DataColumn[] { dt.Columns["PartName"] };
            return dt;
        } // end   private DataTable  BuildPartSWMaterialTable()


        //reads material from a part doc
        public StringBuilder readMaterialV2(string pathToPart, string partNameFromFile)
        {
            string partPath = pathToPart;
            string partNameInDGV = partNameFromFile;
            // ModelDoc2 swModel = default(ModelDoc2);

            PartDoc swPart = default(PartDoc);

            Body2 swBody = default(Body2);

            int errors = 0;
            int warnings = 0;
            object[] vMatDBarr = null;
            object[] Bodies = null;
            string sMatName = "";
            string sMatDB = "";
            int i = 0;
            int j = 0;
            bool boolstat = false;
            StringBuilder sbMessages = new StringBuilder();
            try
            {

          
            // Open the document
            swModel = (ModelDoc2)swApp.OpenDoc6(pathToPart, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);

            swPart = (PartDoc)swModel;

            vMatDBarr = (object[])swApp.GetMaterialDatabases();

            Debug.Print("");

            // Get material from Part object
            sMatName = swPart.GetMaterialPropertyName2("Default", out sMatDB);

            // Add row to PartSWMaterial Table ("SWMaterial", "PartName")
            if (sMatName == null) { sMatName = string.Empty;}
            PartSWMaterialTable.Rows.Add(sMatName, partNameInDGV);

          
            if (string.IsNullOrEmpty(sMatName))
            {
                sbMessages.AppendLine(partNameInDGV + "'s material is: No material assignment found.");
            }
            else
            {
                string returnString = RunMmaterialCompare(sMatName, partNameInDGV);
                if (returnString == string.Empty)
                {
                    if (foundMaterialID == -1)
                    {
                        // not found
                        sbMessages.AppendLine(String.Format("{0} material assignement is {1}, but no match was found in the database. Please manually assign the material for this part.", partNameInDGV, sMatName));
                    }
                    else
                    {
                        // we have a good material ID - it has been saved to dtPartMaterials
                        sbMessages.AppendLine(String.Format("{0} material assignement is {1}, Match found in the database.", partNameInDGV, sMatName));
                    }
                }
                else
                {
                    MessageBox.Show(String.Format("An error occured while running the material comparison. {0}", returnString));
                }
            }

            sbMessages.AppendLine(" ");
            return sbMessages;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                
                return sbMessages;
            }

            /*
            // I dont think this is not necessary, the material is assigned to the part not the body
            Bodies = (object[])swPart.GetBodies2((int)swBodyType_e.swAllBodies, false);
            for (j = 0; j < Bodies.Length; j++)
            {
                swBody = (Body2)Bodies[j];
                swBody.Select2(false, null);
                sMatName = swBody.GetMaterialPropertyName("", out sMatDB);

                int swMaterialErrorCode;
                if (string.IsNullOrEmpty(sMatName))
                {
                    sbMessages.AppendLine(partNameInDGV + "'s material name: No material assignment found.");
                    //MessageBox.Show(partNameInDGV  + "'s material name: No material applied");

                    // TEMP WORK AROUND
                    // Call to SetMaterialProperty returns a int swBodyMaterialApplicationError_e Enumeration. -1 is an error, 1 is ok
                    // swMaterialErrorCode = swBody.SetMaterialProperty("Default", "solidworks materials.sldmat", "Plain Carbon Steel");
                    // sMatName = swBody.GetMaterialPropertyName("", out sMatDB);
                }
                else
                {
                    string returnString = RunMmaterialCompare(sMatName);
                    if (returnString == string.Empty)
                    {
                        if (foundMaterialID == -1)
                        {
                            // not found
                            //MessageBox.Show(partNameInDGV + "'s material name: " + sMatName);
                            sbMessages.AppendLine(String.Format("{0} material assignement is {1}, but no match was found in the database. Please manually assign the material for this part.", partNameInDGV, sMatName));
                        }
                        else
                        {
                            // we have a good material ID - it has been saved to dtPartMaterials
                            sbMessages.AppendLine(String.Format("{0} material assignement is {1}, Match found in the database.", partNameInDGV, sMatName));
                        }
                    }
                    else
                    {
                        MessageBox.Show(String.Format("An error occured while running the material comparison. {0}", returnString));
                    }
                }
            } // end   for (j = 0; j < Bodies.Length; j++) */


        }//end read material v2


        private string RunMmaterialCompare(string matName, string partName)
        {
            // pass in matName and see if there is a match in DB

            string errorMsg;
            string spName = "spCheckMaterialMatch";
            foundMaterialID = -1;

            DataSet ds = new DataSet();
            SqlParameter[] sqlParams = new SqlParameter[1];

            try
            {
                sqlParams[0] = new SqlParameter("@partMaterial", SqlDbType.NVarChar, 100)
                {
                    Value = matName
                };
                ds = dbConnect.GetDataSet(out errorMsg, spName, sqlParams);

                if (errorMsg == string.Empty)
                {
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            // The table has 2 columns: material_id and material_name
                            // get first row
                            DataRow dr = ds.Tables[0].Rows[0];
                            foundMaterialID = Int32.Parse(dr[0].ToString());
                            // string materialName = dr[1].ToString();
                            dtPartMaterials.Rows.Add(foundMaterialID, partName);

                            // check if we need to add this to the Saved table
                            if (IsIndivCompMatSearch)
                            {
                                //check if the part is already in the table
                                DataRow row = SavedPartMaterialTable.Rows.Find(partName);
                                if (row != null)
                                {
                                    row["MaterialID"] = foundMaterialID;
                                }
                                else
                                {
                                    SavedPartMaterialTable.Rows.Add(foundMaterialID, partName);
                                }
                            }
                        }
                        else
                        {
                            // no match, foundMaterialID is -1
                            // check if we need to remove this from the public Saved table
                            if (IsIndivCompMatSearch)
                            {
                                //check if the part is already in the table
                                DataRow row = SavedPartMaterialTable.Rows.Find(partName);
                                if (row != null)
                                {
                                    row.Delete();
                                    SavedPartMaterialTable.AcceptChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        errorMsg = "No dataset returned.";
                    }
                }
                return errorMsg;
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
            finally
            {
                if (ds != null)
                    ds.Dispose();
            }
        } // end private string RunMmaterialCompare(string matName)
        #endregion




        #region read void space in a box

        //read voidspace in a box?

        /*thoughts
         * select object
         * get face count
         * select lowest face count by numb
         * get legth and width of face
         * look at face for co Edges
         * grab outstanding data IE heigh?  can be same number if exact cube,  can be different if a rectangular prism
         * multiply LWH to get volume of outer shell
         * grab component data Volume of actual shell 
         * subtract the actual shell volume from the calulated volume
         * 
         * left with void space
         * 
         * 
        */


        public PartInfo[] readpartVolume(PartInfo[] partInfoArray)
        {
            
            ModelDocExtension Extn;
            MassProperty2 MyMassProp;
            MassPropertyOverrideOptions OvProp;
            ModelDoc2 swModelDoc;
            double[] value = new double[3];
            
        //set instance of solidworks
            
            int warnings = 0;

            swApp = (ISldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
            int errors = 0;

            ModelDoc2 swModel;
            object[] models;
            int count;
            int index;
            string pathName = "";

            //get all of the active docs
            count = swApp.GetDocumentCount();
            Debug.Print("Number of open documents in this SolidWorks session: " + count);
            models = (object[])swApp.GetDocuments();

            double recordedVolume = 0.00;
            string tempFileName;
            string tempFileNameNoEXT;
            string FileExtension;

            for (index = 0; index < count; index++)
            {
                swModel = models[index] as ModelDoc2;
                pathName = swModel.GetPathName();
               
                //seperate a path name
                tempFileName = Path.GetFileName(pathName);
                tempFileNameNoEXT = Path.GetFileNameWithoutExtension(pathName);
                FileExtension = Path.GetExtension(pathName);

                if (FileExtension.ToUpper() == ".SLDPRT")
                {

                    swModelDoc = swApp.OpenDoc6(pathName, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);

                    Extn = swModelDoc.Extension;

                    // Create mass properties and override options
                    MyMassProp = (MassProperty2)Extn.CreateMassProperty2();

                    OvProp = (MassPropertyOverrideOptions)MyMassProp.GetOverrideOptions();
                    OvProp.OverrideMass = true;
                    OvProp.SetOverrideMassValue(100);
                    double[] comArr = new double[9];
                    comArr[0] = 0.1677;
                    comArr[1] = 0;
                    comArr[2] = 0;
                    comArr[3] = 0;
                    comArr[4] = 0.21358;
                    comArr[5] = 0;
                    comArr[6] = 0;
                    comArr[7] = 0;
                    comArr[8] = 0.34772;
                    OvProp.OverrideMomentsOfInertia = true;
                    OvProp.SetOverrideMomentsOfInertiaValue(0, comArr, "");

                    
                    // Use document property units (MKS)
                    MyMassProp.UseSystemUnits = true;
                                                                                
                    recordedVolume = MyMassProp.Volume;
                    recordedVolume = (recordedVolume * 61023.744095);

                    //MessageBox.Show(tempFileNameNoEXT + " Volume is: " + recordedVolume.ToString("F") + " Cubic Inches");

                    var digits = new[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                   
                    try
                    {
                        var partInfoArrFound = Array.Find(partInfoArray, item => item.Name.TrimEnd(digits) == tempFileNameNoEXT);
                        if (partInfoArrFound != null)
                        {
                            partInfoArrFound.PartVolume = recordedVolume;
                        }
                    }catch(Exception e)
                    {
                        Debug.Print("part name not found in array");
                    }

                }
            }
            return partInfoArray;

        }//end readBoxVoidSpace 


        #region read edgedata preselected  Unused
        /*
         * User must preselect an edge Unused
         * */
        public void readEdgeData()
        {
            //set instance of solidworks
            ModelDocExtension swModelDocExt;
            SelectionMgr swSelMgr;

            int warnings = 0;

            swApp = (ISldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
            int errors = 0;

            ModelDoc2 swModel;
            object[] models;
            int count;
            int index;
            string pathName = "";

            //get all of the active docs
            count = swApp.GetDocumentCount();
            Debug.Print("Number of open documents in this SolidWorks session: " + count);
            models = (object[])swApp.GetDocuments();

            for (index = 0; index < count; index++)
            {
                swModel = models[index] as ModelDoc2;
                pathName = swModel.GetPathName();
                Debug.Print("Path and name of open document: " + pathName);
            }

            //seperate a path name
            string tempFileName = Path.GetFileName(pathName);
            string tempFileNameNoEXT = Path.GetFileNameWithoutExtension(pathName);

            ModelDoc2 Part = default(ModelDoc2);

            Part = (ModelDoc2)swApp.ActiveDoc;

            SelectionMgr swSelectMgr = default(SelectionMgr);
            swSelectMgr = (SelectionMgr)Part.SelectionManager;

            Edge swEdge = default(Edge);
            swEdge = (Edge)swSelectMgr.GetSelectedObject5(1);


            Curve swCurve = default(Curve);
            CurveParamData swCurveParaData = default(CurveParamData);
            swCurve = (Curve)swEdge.GetCurve();
            swCurveParaData = (CurveParamData)swEdge.GetCurveParams3();

            MessageBox.Show("The curve tag is: " + swCurveParaData.CurveTag);
            MessageBox.Show("The curve type as defined in swCurveType_e is: " + swCurveParaData.CurveType);

            double[] vEndPoint = null;
            vEndPoint = (double[])swCurveParaData.EndPoint;
            double[] EndPoint = new double[3];

            int i = 0;

            for (i = 0; i <= vEndPoint.GetUpperBound(0); i++)
            {
                EndPoint[i] = vEndPoint[i];
            }
            MessageBox.Show("The end point x,y,z coordinates are: " + EndPoint[0] + "," + EndPoint[1] + "," + EndPoint[2]);

            double[] vStartPoint = null;
            double[] StartPoint = new double[3];
            vStartPoint = (double[])swCurveParaData.StartPoint;
            for (i = 0; i <= vStartPoint.GetUpperBound(0); i++)
            {
                StartPoint[i] = vStartPoint[i];
            }

            MessageBox.Show("The start point x,y,z coordinates are: " + StartPoint[0] + "," + StartPoint[1] + "," + StartPoint[2]);

            /*
             * difference between start and end data for XYZ is the length of a particular edge
             * if one value is negative take absolute and add to positive
             * 
             * */
            


        }
        #endregion


        #region GetDimensions NOT FINISHED NOT USED
        public void getDimension()
        {
            ModelDocExtension swModelDocExt;
            SelectionMgr swSelMgr;

            int warnings = 0;

            swApp = (ISldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
            int errors = 0;

            ModelDoc2 swModel;
            object[] models;
            int count;
            int index;
            string pathName = "";

            //get all of the active docs
            count = swApp.GetDocumentCount();
            Debug.Print("Number of open documents in this SolidWorks session: " + count);
            models = (object[])swApp.GetDocuments();

            for (index = 0; index < count; index++)
            {
                swModel = models[index] as ModelDoc2;
                pathName = swModel.GetPathName();
                Debug.Print("Path and name of open document: " + pathName);
            }

            //seperate a path name
            string tempFileName = Path.GetFileName(pathName);
            string tempFileNameNoEXT = Path.GetFileNameWithoutExtension(pathName);



        
            SelectData swSelData = default(SelectData);
            Face2 swFace = default(Face2);
            string fileName = null;
            bool bRet = false;




           
            swModel = (ModelDoc2)swApp.OpenDoc6(pathName, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
            swModel.ShowNamedView2("*Front", 2);
            swModelDocExt = (ModelDocExtension)swModel.Extension;
            bRet = swModelDocExt.SelectByID2("Face1", "FACE", 0, 0, 0, false, 0, null, 0);
            swSelMgr = (SelectionMgr)swModel.SelectionManager;
            swFace = (Face2)swSelMgr.GetSelectedObject6(1, -1);
            swSelData = (SelectData)swSelMgr.CreateSelectData();

        }
        #endregion

        #region getEdgeCount NOT USED
        /*
         * Requires preseected face  
         */
        public void getEdgeCount()
        {
            //set instance of solidworks
            ModelDocExtension swModelDocExt;
            SelectionMgr swSelMgr;

            int warnings = 0;

            swApp = (ISldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
            int errors = 0;

            ModelDoc2 swModel;
            object[] models;
            int count;
            int index;
            string pathName = "";

            //get all of the active docs
            count = swApp.GetDocumentCount();
            Debug.Print("Number of open documents in this SolidWorks session: " + count);
            models = (object[])swApp.GetDocuments();

            for (index = 0; index < count; index++)
            {
                swModel = models[index] as ModelDoc2;
                pathName = swModel.GetPathName();
                Debug.Print("Path and name of open document: " + pathName);
            }

            //seperate a path name
            string tempFileName = Path.GetFileName(pathName);
            string tempFileNameNoEXT = Path.GetFileNameWithoutExtension(pathName);

            swModel = swApp.OpenDoc6(pathName, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);



            //ModelDoc2 swModel = default(ModelDoc2);
            //SelectionMgr swSelMgr = default(SelectionMgr);
            Face2 swFace = default(Face2);
            Loop2 swLoop = default(Loop2);

            //swModel = (ModelDoc2)swApp.ActiveDoc;
            swSelMgr = (SelectionMgr)swModel.SelectionManager;
            swFace = (Face2)swSelMgr.GetSelectedObject6(1, -1);
            swLoop = (Loop2)swFace.GetFirstLoop();

            Debug.Print("LoopCount = " + swFace.GetLoopCount());


            while ((swLoop != null))
            {
                Debug.Print("  IsOuter      = " + swLoop.IsOuter());
                Debug.Print("  IsSingular   = " + swLoop.IsSingular());
                Debug.Print("  EdgeCount    = " + swLoop.GetEdgeCount()); //returns number of edges
                Debug.Print("  VertexCount  = " + swLoop.GetVertexCount());
                Debug.Print("");

                swLoop = (Loop2)swLoop.GetNext();
            }
        }
        #endregion

        public double checkEdges(double volumeFound)
        {
            Double partVolume = volumeFound;
        
            HashSet<Double> innerCube = new HashSet<Double>();
            HashSet<Double> outterCube = new HashSet<Double>();

            double innerL, innerW, innerH;
            double outterL, outterW, outterH;

            double innerVolume, outterVolume;


            ModelDocExtension swModelDocExt;
            SelectionMgr swSelMgr;

            int warnings = 0;

            swApp = (ISldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
            int errors = 0;

            ModelDoc2 swModel;
            object[] models;
            int count;
            int index;
            string pathName = "";

            //get all of the active docs
            count = swApp.GetDocumentCount();
            Debug.Print("Number of open documents in this SolidWorks session: " + count);
            models = (object[])swApp.GetDocuments();

            for (index = 0; index < count; index++)
            {
                swModel = models[index] as ModelDoc2;
                pathName = swModel.GetPathName();
                Debug.Print("Path and name of open document: " + pathName);
            }

            //seperate a path name
            string tempFileName = Path.GetFileName(pathName);
            string tempFileNameNoEXT = Path.GetFileNameWithoutExtension(pathName);

            swModel = swApp.OpenDoc6(pathName, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);

            
            PartDoc swPart = default(PartDoc);
            object[] vBodyArr = null;
            object vBody = null;
            Body2 swBody = default(Body2);
            int nRetval1 = 0;
            int nRetval2 = 0;
            object[] vEdgeArr = null;
            object vEdge = null;
            Edge swEdge = default(Edge);
            FaultEntity swFaultEnt = default(FaultEntity);
            Curve swCurve = default(Curve);
            CurveParamData swCurveParaData = default(CurveParamData);

            swPart = (PartDoc)swModel;
            Debug.Print("File = " + swModel.GetPathName());
            vBodyArr = (object[])swPart.GetBodies2((int)swBodyType_e.swAllBodies, true);
            Debug.Assert((vBodyArr != null));
            foreach (object vBody_loopVariable in vBodyArr)
            {
                vBody = vBody_loopVariable;
                swBody = (Body2)vBody;
                Debug.Print("  Body[" + swBody.GetType() + "] --> " + swBody.GetSelectionId());
                nRetval1 = swBody.Check(); // Obsolete method
                nRetval2 = swBody.Check2(); // Obsolete method
                Debug.Print("    IBody2::Check (1 if valid; 0 if not) = " + nRetval1);
                Debug.Print("    IBody2::Check2(Number of faults) = " + nRetval2);
                vEdgeArr = (object[])swBody.GetEdges();

                if (vEdgeArr.Length > 12)
                {
                    MessageBox.Show("void detected inside Cube");

                    //MessageBox.Show("total number of edges found: " + vEdgeArr.Length);
                    int edgeNum = 0;
                    if ((vEdgeArr != null))
                    {

                        foreach (Object vEdge_loopVariable in vEdgeArr)
                        {
                            edgeNum++;
                            vEdge = vEdge_loopVariable;
                            swEdge = (Edge)vEdge;
                            swCurveParaData = (CurveParamData)swEdge.GetCurveParams3();
                            swCurve = (Curve)swEdge.GetCurve();
                            double lengthOfCurveMeters = swCurve.GetLength3(swCurveParaData.UMinValue, swCurveParaData.UMaxValue);
                            double lengthOfCurveInch = Math.Round((lengthOfCurveMeters * 39.37), 2);
                            /*
                             *  Edges 1-12 are interior edges of a hollow cube/ rectangular prism
                             *  Edges 13-24 are the exterior edges of a hollow cube/ rectangular prism
                             */
                            //MessageBox.Show("Edge " + edgeNum + " length of curve: " + lengthOfCurveInch + " in.");

                            if (edgeNum < 13)
                            {
                                innerCube.Add(lengthOfCurveInch);
                            }
                            else if (edgeNum >= 13)
                            {
                                outterCube.Add(lengthOfCurveInch);
                            }
                            else
                            {
                                //do nothing
                            }



                        }//end for each

                        //sort both lists lowest to largest by value

                        innerVolume = (innerCube.ElementAt(0) * innerCube.ElementAt(1) * innerCube.ElementAt(2));
                        outterVolume = (outterCube.ElementAt(0) * outterCube.ElementAt(1) * outterCube.ElementAt(2));

                        if (partVolume != outterVolume)
                        {

                            MessageBox.Show("void volume: " + innerVolume + " cubic inches");
                        }

                        //grab 3 larges values of inner list no duplicates

                        //grab 3 larges values of outer list no duplicates

                        return innerVolume;
                    }

                    return 0;

                }

                return 0;

            }

            return 0;

        } //end Check Edges

        #endregion


    }//end class

}//end namespace