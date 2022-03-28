using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPS.SWplugin.ICT
{
    class partInformation
    {
        //part information to grab from solidworks


        public partInformation()
        {

        }

        //public static partInformation(string partName, string material, int materialID)
        //{
            //this.partNameObj = partName;
            //this.partMaterialObj = material;
            //this.materialID = materialID;
            //add as needed
        //}

        public static string partNameObj
        { get; set; }

        public static string partMaterialObj 
        { get; set; }

        public static int materialID
        { get; set; }

        //something not right
        public static ArrayList partsList = new ArrayList();
        //public List<partInformation> partList = new List<partInformation>();

        //public list of strings for selected components, used with selectall function
        public static List<string> StoredComponents = new List<string>();

    }





}
