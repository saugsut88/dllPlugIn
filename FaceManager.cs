using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace VPS.SWplugin.ICT
{
    class FaceManager
    {
        private StringBuilder usrMessage = new StringBuilder();
        private ISldWorks swApp  = default (ISldWorks);
        private AssemblyDoc swAssDoc = default(AssemblyDoc);
        private ModelDoc2 swDoc = default(ModelDoc2);

        

        public FaceManager(ISldWorks myswApp, AssemblyDoc myAssDoc)
        {
            swAssDoc = myAssDoc;
            swDoc = (ModelDoc2)myAssDoc;
            swApp = myswApp;
        }// end
        

        public StringBuilder GetAssInfo(PartInfo[] myParts)
        {
            GetComponents(myParts);
            return usrMessage;
        }

        private void GetComponents(PartInfo[] myParts)
        {
            Component2 swComponenet = default(Component2);
            Body2 swBody = default(Body2);
            object[] vComponents = null;
            object[] vBodies = new object[1];
            object vBodyInfo;
            int BodyType = 0;
            int[] BodiesInfo = null;
            object[] vAssBodies = new object[1];
            object[] swBodyVar = new object[1];
            object[] swSelFaceVar = new object[4];
           

            #region components
            //Get Components in the assembly
            vComponents = (object[])swAssDoc.GetComponents(true);

            //Loop through all the components
            for (int i = 0; i <= vComponents.Length - 1; i++)
            { 
                swComponenet = (Component2)vComponents[i];
                myParts[i] = new PartInfo { Name = swComponenet.Name2 };

                usrMessage.AppendLine("\tComponenet " + i.ToString() + ":  " + swComponenet.Name2);

                //Get the bodies in the componenet
                vBodies = (object[])swComponenet.GetBodies3((int)swBodyType_e.swSolidBody, out vBodyInfo);
                BodiesInfo = (int[])vBodyInfo;
                usrMessage.AppendLine("\t\tBody Count: " + (vBodies.Length + 1));

                //Loop through each body in this component
                for (int j = 0; j <= vBodies.Length - 1; j++)
                {
                    swBody = (Body2)vBodies[j];

                    usrMessage.AppendLine("\t\t\tBody " + j.ToString() + ": " + swBody.Name);

                    //Type of body
                    BodyType = (int)BodiesInfo[j];
                    switch (BodyType)
                    {
                        case 0:
                            usrMessage.AppendLine("\t\t\t\tType: User = " + BodyType.ToString());
                            break;
                        case 1:
                            usrMessage.AppendLine("\t\t\t\tType: Normal = " + BodyType.ToString());
                            break;
                    }//end switch(BodyType)

                    //usrMessage.AppendLine("            Face ID:");
                    //Get Faces 
                    GetFaces(swBody,  myParts[i]);
                }//end for (int j=0;j<=vBodies.Length-1;j++)

            }//end for (i = 0; i <= vComponents.Length - 1; i++)
            #endregion components
        }

        //This function is not currently being used**
        private String GetBodies()
        {
            StringBuilder strUserMessage = new StringBuilder();
            // 1 radian = 180º/p = 57.295779513º or approximately 57.3º
            const double RadPerDeg = 1.0 / 57.3;
            const double MaxUAngle = 1.0 * RadPerDeg;
            const double MaxVAngle = 1.0 * RadPerDeg;
            //Utilities.SWSelectionManager _mySelMgr = new Utilities.SWSelectionManager();
            Utilities.SWAppManager _swApp = new Utilities.SWAppManager();
            ModelDoc2 swModel = default(ModelDoc2);
            AssemblyDoc swAssDoc;
            ModelDocExtension swModelDocExt = default(ModelDocExtension);
            //PartDoc swPart = default(PartDoc);
            Body2 swBody = default(Body2);
            Body2 swProcBody = default(Body2);
            PartDoc swPartDoc = default(PartDoc);
            PartDoc swNewPartDoc = default(PartDoc);
            Feature swFeat = default(Feature);
            object[] vBodies = new object[1];
            object[] swBodyVar = new object[1];
            object[] swSelFaceVar = new object[4];
            int swSelFaceCount = 0;

            _swApp.OpenDoc();
            if (_swApp.IsDocOpened)
            {
                //Open Part
                swModel = (ModelDoc2)_swApp.swActiveDoc;
                swPartDoc = (PartDoc)swModel;

                //Get and process body in part
                vBodies = (object[])swPartDoc.GetBodies2((int)swBodyType_e.swSolidBody, false);
                swBody = (Body2)vBodies[0];
                swProcBody = (Body2)swBody.GetProcessedBody2(MaxUAngle, MaxVAngle);

                //Create new part containing processed body
                swNewPartDoc = (PartDoc)_swApp.swInstance.NewDocument("C:\\ProgramData\\SOLIDWORKS\\SOLIDWORKS 2014\\templates\\TempPart.prtdot", 0, 0, 0);
                //swApp.NewDocument("C:\\ProgramData\\SOLIDWORKS\\SOLIDWORKS 2014\\templates\\part.prtdot", 0, 0, 0);
                swFeat = (Feature)swNewPartDoc.CreateFeatureFromBody3(swProcBody, false, (int)swCreateFeatureBodyOpts_e.swCreateFeatureBodyCheck);
                strUserMessage.Append("Original part: " + swModel.GetPathName());
                strUserMessage.Append(System.Environment.NewLine);
                strUserMessage.Append("  Title: " + swModel.GetTitle());
                strUserMessage.Append(System.Environment.NewLine);
                strUserMessage.Append("    Body faces: " + swBody.GetFaceCount());
                strUserMessage.Append(System.Environment.NewLine);
                strUserMessage.Append("    Processed body faces: " + swProcBody.GetFaceCount());
                strUserMessage.Append(System.Environment.NewLine);

                //Select multiple faces in new part
                swModel = (ModelDoc2)swNewPartDoc;
                strUserMessage.Append(System.Environment.NewLine);
                strUserMessage.Append("New part title: " + swModel.GetTitle());
                strUserMessage.Append(System.Environment.NewLine);
                strUserMessage.Append("    Body faces: " + swBody.GetFaceCount());
                strUserMessage.Append(System.Environment.NewLine);
                strUserMessage.Append("    Processed body faces: " + swProcBody.GetFaceCount());
                strUserMessage.Append(System.Environment.NewLine);
                swModelDocExt = (ModelDocExtension)swModel.Extension;

                ////Select multiple faces in new part
                //swModel = (ModelDoc2)swNewPart;
                //Debug.Print("New part title: " + swModel.GetTitle());
                //Debug.Print("    Body faces: " + swBody.GetFaceCount());
                //Debug.Print("    Processed body faces: " + swProcBody.GetFaceCount());
                //swModelDocExt = (ModelDocExtension)swModel.Extension;
                //status = swModelDocExt.SelectByID2("", "FACE", -0.0258707587273648, -0.00453920675113295, -0.00750000000022055, false, 0, null, 0);
                //status = swModelDocExt.SelectByID2("", "FACE", -0.016247803762667, 0, -0.0112417538793466, true, 0, null, 0);
                //status = swModelDocExt.SelectByID2("", "FACE", -0.0149546544521968, -0.026689891165347, 0, true, 0, null, 0);
                //status = swModelDocExt.SelectByID2("", "FACE", -0.0208314165242882, -0.0200000000001523, -0.00322480979224338, true, 0, null, 0);

                //Get selected faces in body
                swBodyVar = (object[])swNewPartDoc.GetBodies2((int)swBodyType_e.swAllBodies, true);
                if ((swBodyVar == null))
                {
                    MessageBox.Show("    Did not get any bodies.");
                }
                else
                {
                    swBody = (Body2)swBodyVar[0];
                    MessageBox.Show("    Name of processed body: " + swBody.Name);
                }
                swProcBody = (Body2)swBody.GetProcessedBodyWithSelFace();
                swSelFaceVar = (object[])swProcBody.GetSelectedFaces();
                if ((swSelFaceVar != null))
                {
                    swSelFaceCount = swProcBody.GetSelectedFaceCount();
                    MessageBox.Show("      Number of faces selected in processed body: " + swSelFaceCount);
                }
                else
                {
                    MessageBox.Show("      No faces selected in processed body.");
                }//end if ((swSelFaceVar != null))
            }



            return strUserMessage.ToString();
        }//end private void GetBodies()

        void GetFaces(Body2 currentBody,  PartInfo myPart)
        {
            object[] swFaces = new object[4];
            Face2 swFace = default(Face2);
            MyHoles varHoles = new MyHoles();
            StringBuilder sbMyFaceHoleInfo = new StringBuilder();

            swFaces = (object[])currentBody.GetFaces();
            myPart.InitializeFaces(swFaces.Length);

            usrMessage.AppendLine("\t\t\t\t\tFaces:");

            //MessageBox.Show("Face Count: " + swFaces.Length.ToString());
            for (int k = 0; k < swFaces.Length; k++)
            {
                try
                {
                    swFace = (Face2)swFaces[k];
                    usrMessage.AppendLine("\t\t\t\t\t\tFace: " + k + ": Area =  " + swFace.GetArea().ToString());
                    myPart.AddFace(k, swFace);

                    myPart.AddFaceCoedges(k, GetMyCoEdges(swFace));
                    //IsHole((IFace2)swFace);
                    sbMyFaceHoleInfo =  varHoles.GetAllHoles((SldWorks)swApp, swDoc, swFace, ref myPart.myFaces[k]);
                    usrMessage.AppendLine(sbMyFaceHoleInfo.ToString());

                }
                catch (Exception e)
                {
                    MessageBox.Show("Face Error: Face - " + k.ToString() + " : Excepton - " + e.Message);
                }

            }//end for (int k =0;k<=swFaces.Length;k++)
        }//End void GetFaces(Body2 currentBody)

        private int GetMyCoEdges(Face2 currentFace)
        {
            CoEdge swEdge = default(CoEdge);
            Loop2 swLoop = default(Loop2);
            object[] swEdges = new object[4];
            int numCoEdges = 0;

            try
            {

                swLoop = (Loop2)currentFace.GetFirstLoop();
                swEdges = (object[])swLoop.GetCoEdges();
                numCoEdges = swEdges.Length;

                usrMessage.AppendLine("\t\t\t\t\t\t\tCoEdge Count: " + swEdges.Length.ToString());
                
                for (int l = 0; l < swEdges.Length; l++)
                {
                    swEdge = (CoEdge)swEdges[l];

                    //usrMessage.AppendLine("                           " + swEdge.GetSense().ToString());
                }//end foreach (CoEdge edge in swEdges)
            }
            catch
            {
                MessageBox.Show("Coedge loop Error");
            }

            return numCoEdges;
        }//end private void GetCoEdges()

    }
}
