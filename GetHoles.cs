using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
//using SolidWorks.Interop.SWRoutingLib;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace VPS.SWplugin.ICT
{ 
    public partial class MyHoles
    {
        public MyHoles()
        { }

         ~MyHoles() { }

      

        public double[] GetFaceNormalAtMidCoEdge(CoEdge swCoEdge)
        {
            Face2 swFace = default(Face2);
            Surface swSurface = default(Surface);
            Loop2 swLoop = default(Loop2);
            double[] varParams = null;
            double[] varPoint = null;
            double dblMidParam = 0;
            double[] dblNormal = new double[3];
            bool bFaceSenseReversed = false;

            varParams = (double[])swCoEdge.GetCurveParams();
            if (varParams[6] > varParams[7])
            {
                dblMidParam = (varParams[6] - varParams[7]) / 2 + varParams[7];
            }
            else
            {
                dblMidParam = (varParams[7] - varParams[6]) / 2 + varParams[6];
            }
            varPoint = (double[])swCoEdge.Evaluate(dblMidParam);

            // Get the face of the given coedge
            // Check for the sense of the face
            swLoop = (Loop2)swCoEdge.GetLoop();
            swFace = (Face2)swLoop.GetFace();
            swSurface = (Surface)swFace.GetSurface();
            bFaceSenseReversed = swFace.FaceInSurfaceSense();
            varParams = (double[])swSurface.EvaluateAtPoint(varPoint[0], varPoint[1], varPoint[2]);
            if (bFaceSenseReversed)
            {
                // Negate the surface normal as it is opposite from the face normal
                dblNormal[0] = -varParams[0];
                dblNormal[1] = -varParams[1];
                dblNormal[2] = -varParams[2];
            }
            else
            {
                dblNormal[0] = varParams[0];
                dblNormal[1] = varParams[1];
                dblNormal[2] = varParams[2];
            }
            return dblNormal;

        }//end public object GetFaceNormalAtMidCoEdge(swCoEdge As SldWorks.CoEdge)


        public double[] GetTangentAtMidCoEdge(CoEdge swCoEdge)
        {
            double[] varParams = null;
            double dblMidParam = 0;
            double[] dblTangent = new double[3];
            varParams = (double[])swCoEdge.GetCurveParams();
            if (varParams[6] > varParams[7])
            {
                dblMidParam = (varParams[6] - varParams[7]) / 2.0 + varParams[7];
            }
            else
            {
                dblMidParam = (varParams[7] - varParams[6]) / 2.0 + varParams[6];
            }
            varParams = (double[])swCoEdge.Evaluate(dblMidParam);
            dblTangent[0] = varParams[3];
            dblTangent[1] = varParams[4];
            dblTangent[2] = varParams[5];
            return dblTangent;

        }
        

        public double[] GetCrossProduct(double[] varVec1, double[] varVec2)
        {
            double[] dblCross = new double[3];

            dblCross[0] = varVec1[1] * varVec2[2] - varVec1[2] * varVec2[1];
            dblCross[1] = varVec1[2] * varVec2[0] - varVec1[0] * varVec2[2];
            dblCross[2] = varVec1[0] * varVec2[1] - varVec1[1] * varVec2[0];
            return dblCross;

        }
        

        public bool VectorsAreEqual(double[] varVec1, double[] varVec2)
        {
            double dblMag;
            double dblDot;
            double[] dblUnit1 = new double[3];
            double[] dblUnit2 = new double[3];

            dblMag = Math.Pow((varVec1[0] * varVec1[0] + varVec1[1] * varVec1[1] + varVec1[2] * varVec1[2]), 0.5);

            dblUnit1[0] = varVec1[0] / dblMag;
            dblUnit1[1] = varVec1[1] / dblMag;
            dblUnit1[2] = varVec1[2] / dblMag;
            dblMag = Math.Pow((varVec2[0] * varVec2[0] + varVec2[1] * varVec2[1] + varVec2[2] * varVec2[2]), 0.5);
            dblUnit2[0] = varVec2[0] / dblMag;
            dblUnit2[1] = varVec2[1] / dblMag;
            dblUnit2[2] = varVec2[2] / dblMag;
            dblDot = dblUnit1[0] * dblUnit2[0] + dblUnit1[1] * dblUnit2[1] + dblUnit1[2] * dblUnit2[2];
            dblDot = Math.Abs(dblDot - 1.0);
            
            //' Compare within a tolerance
            if (dblDot < 0.0000000001)
            {
                return true;
            }
            else
            {
                return false;
            }

        }//End public bool VectorsAreEqual(varVec1 As Variant, varVec2 As Variant)


        public int SelectHoleEdges(Face2 swFace, SelectData swSelData)
        {
            Loop2 swThisLoop = default(Loop2);
            CoEdge swThisCoEdge = default(CoEdge);
            CoEdge swPartnerCoEdge = default(CoEdge);
            Entity swEntity = default(Entity);
            double[] varThisNormal = null;
            double[] varPartnerNormal = null;
            double[] varCrossProduct = null;
            double[] varTangent = null;
            object[] vEdgeArr = null;
            Edge swEdge = default(Edge);
            Curve swCurve = default(Curve);
            bool bRet = true;
            int count = 0;
            bool bCount = true;
            int myFaceHoleCount = 0;

            swThisLoop = (Loop2)swFace.GetFirstLoop();
            while ((swThisLoop != null))
            {
                // Hole is inner loop
                // Circular or elliptical hole has only one edge
                bRet = swThisLoop.IsOuter();
                count = swThisLoop.GetEdgeCount();
                if (count != 1)
                {
                    bCount = false;
                }
                else
                {
                    bCount = true;
                }
                if ((bRet == false) && (bCount == true))
                {
                    swThisCoEdge = (CoEdge)swThisLoop.GetFirstCoEdge();
                    swPartnerCoEdge = (CoEdge)swThisCoEdge.GetPartner();
                    varThisNormal = (double[])GetFaceNormalAtMidCoEdge(swThisCoEdge);
                    varPartnerNormal = (double[])GetFaceNormalAtMidCoEdge(swPartnerCoEdge);
                    if (!VectorsAreEqual(varThisNormal, varPartnerNormal))
                    {
                        // There is a sufficient change between the two faces to determine
                        // what kind of transition is being made
                        varCrossProduct = (double[])GetCrossProduct(varThisNormal, varPartnerNormal);
                        varTangent = (double[])GetTangentAtMidCoEdge(swThisCoEdge);
                        if (VectorsAreEqual(varCrossProduct, varTangent))
                        {
                            // Hole
                            vEdgeArr = (object[])swThisLoop.GetEdges();
                            Debug.Assert(0 < vEdgeArr.Length);
                            swEdge = (Edge)vEdgeArr[0];
                            swCurve = (Curve)swEdge.GetCurve();
                            // Ignore elliptical holes
                            if (swCurve.IsCircle())
                            {
                                myFaceHoleCount++;
                                swEdge.Display(10, 1, 0, 0, true);
                                swEntity = (Entity)swEdge;
                                bRet = swEntity.Select4(true, swSelData);
                                Debug.Assert(bRet);
                            }
                        }
                    }
                    
                 
                }


                // Move on to the next
                swThisLoop = (Loop2)swThisLoop.GetNext();
            }
            return myFaceHoleCount;
        }//End public void SelectHoleEdges(swFace As SldWorks.Face2, swSelData As SldWorks.SelectData)


        public StringBuilder GetAllHoles(SldWorks swApp, ModelDoc2 swModel, Face2 HolyFace, ref FaceInfo myFace)//, string strFileName)
        {
            //SldWorks _swApp = swApp;
            //ModelDoc2 swModel = default(ModelDoc2);
            StringBuilder usrMessage = new StringBuilder();
            ModelDocExtension swModelDocExt;//= (default)ModelDocExtension;
            SelectionMgr swSelMgr;// =(default)SelectionMgr;
            SelectData swSelData;// =(default)SelectData;
            //Face2 swFace;// =(default)Face2;
            double[] varPoint = new double[3];
            int intHoleEdgesFound = 0;
            bool boolIsHoleCyl = false;

            //string fileName;
            int errors=0;
            int warnings=0;
            bool bRet;
            //swApp = CreateObject("SldWorks.Application");
            //fileName = "C:\Users\Public\Documents\SOLIDWORKS\SOLIDWORKS 2017\tutorial\advdrawings\gear- caddy.sldprt";
            //swModel = (ModelDoc2) swApp.ActiveDoc;  //swApp.OpenDoc6(strFileName, (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
            //swModel.ShowNamedView2("*Back", 2);
            swModelDocExt = swModel.Extension;
            varPoint = GetFaceSelPoint(HolyFace);
            //bRet = swModelDocExt.SelectByID2("", "FACE", 0.0290197084065686, 0.0111645373580202, 0, false, 0, null, 0);
            bRet = swModelDocExt.SelectByID2("", "FACE", (double)varPoint[0], (double)varPoint[1], (double)varPoint[2], false, 0, null, 0);
            //bRet = swModelDocExt.SelectByID2("", "FACE", 2.90197084065686E-02, 1.11645373580202E-02, 0, false, 0, null, 0);
            swSelMgr = (SelectionMgr)swModel.SelectionManager;
            //swFace = (Face2)swSelMgr.GetSelectedObject6(1, -1);
            swSelData = swSelMgr.CreateSelectData();
            swModel.ClearSelection2(true);
            //Look for holes in the current face
            intHoleEdgesFound=SelectHoleEdges(HolyFace, swSelData);
            //Check face to see if it is a hole's cylindrical face
            boolIsHoleCyl = FaceIsHoleCylinder(HolyFace, swModel, swSelData, swApp, ref myFace);
            if(intHoleEdgesFound>0)
            {
                myFace.NumHoles = intHoleEdgesFound;
                usrMessage.AppendLine("\t\t\t\t\t\t\tHoles found in this face:  " + intHoleEdgesFound);
            }
            else if(intHoleEdgesFound==0 && boolIsHoleCyl)
            {
                myFace.IsCylinder = true;
                usrMessage.AppendLine("\t\t\t\t\t\t\tThis face is a hole cylinder.");

            }else if(intHoleEdgesFound>0 && boolIsHoleCyl)
            {
                myFace.IsCylinderWithHoles = true;
                usrMessage.AppendLine("\t\t\t\t\t\t\tThis face is a hole cylinder and contains " + intHoleEdgesFound + " holes.");
            }
            else //no holes and face is not a cylinder
            {
                usrMessage.AppendLine("\t\t\t\t\t\t\tNo holes found in this face.");
            }

            return usrMessage;
        }//end public GetAllHoles()

        private bool FaceIsHoleCylinder(Face2 HolyFace, ModelDoc2 swModel, SelectData SelData ,SldWorks swApp, ref FaceInfo myFaceInfo)
        {
            object[] vFaceprop = new object[12];
            AssemblyDoc swAss;
            Entity swEntity;
            bool boolCylFound = false;

            if (IsHole(HolyFace, swApp, ref myFaceInfo))
            {
                swAss = (AssemblyDoc)swModel;
                
                swEntity = (Entity)HolyFace;
                swEntity.Select4(false, SelData);
                boolCylFound = true;
                //vFaceprop = HolyFace.MaterialPropertyValues;
                
                //swModel.SelectedFaceProperties(255, (double)vFaceprop[3], (double)vFaceprop[4], (double)vFaceprop[5], (double)vFaceprop[6], (double)vFaceprop[7], (double)vFaceprop[8], false, "" );
                //vFaceprop = HolyFace.MaterialPropertyValues;
                //vFaceprop[0] = 255;
                //vFaceprop[1] = 0;
                //vFaceprop[2] = 0;
                
                //vFaceprop.Highlight(true);
            }
            return boolCylFound;
        }

        private double[] GetFaceSelPoint(Face2 HolyFace)
        {
            object[] swEdges= new object[36];
            Edge swEdge = default(Edge);
            double[] varPoint = new double[3];
            double[] varParams= new double[8];
            //Loop2 swLoop;
            //Surface swSurface;
            double dblMidParam;
            double[] dblNormal = new double[2];
            swEdges = (object[])HolyFace.GetEdges();
            
            swEdge = (Edge)swEdges[0];
            varParams = (double[])swEdge.GetCurveParams();

            //Set the evaluation point based on the coedge curve parameters
            if ((double)varParams[6] > (double)varParams[7])
            {
                dblMidParam = ((double)varParams[6] - (double)varParams[7]) / 2 + (double)varParams[7];
            }
            else
            {
                dblMidParam = ((double)varParams[7] - (double)varParams[6]) / 2 + (double)varParams[6];
            }
            //Get the location of the middle of the coedge
            varPoint = (double [])swEdge.Evaluate(dblMidParam);



            return varPoint;
        }

      
        private bool IsHole(IFace2 face, SldWorks swApp, ref FaceInfo myFaceInfo)
        {
            double[] vParams = new double[7];

            ISurface surf = face.IGetSurface();
            
            if (surf.IsCylinder())
            {
                vParams = surf.CylinderParams;
                //MessageBox.Show("Radius: " +  (double)vParams[6] * 1000 /25.4+ " in.");
                myFaceInfo.CylinderDiameterInches = (double)vParams[6]*1000/25.4;

                face.Highlight(true);
                double[] uvBounds = face.GetUVBounds() as double[];

                double[] evalData = surf.Evaluate((uvBounds[1] - uvBounds[0]) / 2, (uvBounds[3] - uvBounds[2]) / 2, 1, 1) as double[];

                double[] pt = new double[] { evalData[0], evalData[1], evalData[2] };

                int sense = face.FaceInSurfaceSense() ? 1 : -1;

                double[] norm = new double[] { evalData[evalData.Length - 3] * sense, evalData[evalData.Length - 2] * sense, evalData[evalData.Length - 1] * sense };

                double[] cylParams = surf.CylinderParams as double[];

                double[] orig = new double[] { cylParams[0], cylParams[1], cylParams[2] };

                double[] dir = new double[] { pt[0] - orig[0], pt[1] - orig[1], pt[2] - orig[2] };

                IMathUtility mathUtils = swApp.IGetMathUtility();

                IMathVector dirVec = mathUtils.CreateVector(dir) as IMathVector;
                IMathVector normVec = mathUtils.CreateVector(norm) as IMathVector;

                return GetAngle(dirVec, normVec) < Math.PI / 2;
            }
            else
            {
                //not a clinder
                //throw new NotSupportedException("Only cylindrical face is supported");
                return false;
            }
        }

        private double GetAngle(IMathVector vec1, IMathVector vec2)
        {
            return Math.Acos(vec1.Dot(vec2) / (vec1.GetLength() * vec2.GetLength()));
        }
    }//End Class
}//end Namespace
