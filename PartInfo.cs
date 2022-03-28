using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using VPS.SWplugin.ICT;

namespace VPS.SWplugin.ICT
{
    class PartInfo
    {
        private string strPartName = "Default";
        private string strPath = "Default";
        private double partVolume;

        public FaceInfo[] myFaces;

        public PartInfo()
        {
            strPartName = "Default";
            strPath = "Default";
        }


        public string Name
        {
            get { return strPartName; }
            set {strPartName = value; }
        }

        public string Path
        {
            get { return strPath; }
            set{strPath = value;}
        }

        public double PartVolume
        {
            get { return partVolume; }
            set { partVolume = value; }

        }

        public int NumFaces()
        {
            if(myFaces!=null)
            {
                return myFaces.Length;
            }
            else
            {
                return 0;
            }
        }

        


        public void InitializeFaces(int NumFaces)
        {
            myFaces = new FaceInfo[NumFaces];
        }

        public bool AddFace(int index, Face2 newFace)
        {   
            bool boolFaceAdded = false;

            if(myFaces[index]==null)
            {
                myFaces[index] = new FaceInfo (newFace);
                //myFaces[index].InitializeFace(newFace);
                boolFaceAdded = true;
            }
            return boolFaceAdded;
            
        }

        public void AddFaceCoedges(int FaceIndex, int numCoedges)
        {
            myFaces[FaceIndex].NumCoEdges = numCoedges;

        }

        public bool PipeCheck()
        {
            bool boolHasPipes = false;
            bool boolPartHasCylinder = false;
            int intHoleCount = 0;
            //bool boolPartHas2Holes = false;


            //Gather face info
            for(int i=0; i<myFaces.Length; i++)
            {
                if (myFaces[i].IsCylinderWithHoles)
                {
                    boolPartHasCylinder = true;
                    intHoleCount = intHoleCount + myFaces[i].NumHoles;
                }
                else if (myFaces[i].HasHoles)
                {
                    intHoleCount = intHoleCount + myFaces[i].NumHoles;
                }
                else if (myFaces[i].IsCylinder)
                {
                    boolPartHasCylinder = true;
                }
            }

            //Is it a possibly a pipe????
            if (intHoleCount/2==1 && boolPartHasCylinder && myFaces.Length % 4 == 0)
            {
                boolHasPipes = true;
            }
            else
            {
                boolHasPipes = false;
            }


            return boolHasPipes;
        }
    }
}
