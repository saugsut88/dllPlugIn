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
    public class FaceInfo
    {
        int intID;
        string PartName;
        bool boolIsCylinder;
        bool boolHasHoles;
        public double dblSurfArea;
        private Face2 thisFace;
        private int numCoedges;
        bool boolIsPipe;
        bool boolIsCylinderWithHoles;
        int intNumHoles;
        double dblCylinderDiameter;

        public FaceInfo()
        {

        }

        public FaceInfo(Face2 newFace)
        {
            InitializeFace(newFace);            
        }

        public void InitializeFace(Face2 newFace)
        {
            //thisFace = new Face2();
            thisFace = newFace;
            dblSurfArea = thisFace.GetArea();
            boolIsPipe = false;
            boolHasHoles = false;
            boolIsCylinder = false;
            boolIsCylinderWithHoles = false;
            intNumHoles = 0;
            dblCylinderDiameter = 0;
        }

        public double CylinderDiametermm
        {
            get
            {
                return dblCylinderDiameter * 1000;
            }
            set
            {
                dblCylinderDiameter = value * 1000;
            }
        }

        public double CylinderDiameterInches
        {
            get
            {
                return dblCylinderDiameter * 1000 / 25.4;
            }
            set
            {
                dblCylinderDiameter = value/1000*25.4;
            }
        }
        public int NumCoEdges
        {
            get { return numCoedges; }
            set
            {
                numCoedges = value;
            }
        }

        public double FaceArea
        {
            get { return dblSurfArea; }
            set
            {

            }
        }
        
        public bool HasHoles
        {
            get { return boolHasHoles; }
            set
            {
                boolHasHoles = value;
            }
        }

        public bool IsCylinder
        {
            get {
                return boolIsCylinder;
            }
            set
            {
                boolIsCylinder = value;
            }
        }

        public bool IsPipe
        {
            get
            {
                return boolIsPipe;
            }

            set
            {
                boolIsPipe = value;
            }
        }

        public bool IsCylinderWithHoles
        {
            get {
                return boolIsCylinderWithHoles;
            }
            set
            {
                boolHasHoles = true;
                boolIsCylinder = true;
                boolIsCylinderWithHoles = value;
            }
        }

        public int NumHoles
        {
            get
            {
                return intNumHoles;
            }

            set
            {
                intNumHoles = value;
                boolHasHoles = true;
            }
        }
    }
}
