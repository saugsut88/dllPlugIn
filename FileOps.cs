using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace VPS.SWplugin.ICT
{
    class FileOps
    {
        public FileOps()
        {

        }
        public void WriteToFile(StringBuilder mySB)
        {
            string text = mySB.ToString();
            
            File.WriteAllText("ICT Scan " + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt", text);
            
            
        }
    }
}
