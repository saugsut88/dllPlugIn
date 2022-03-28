using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorksTools;
using SolidWorks.Interop.swconst;


namespace VPS.SWplugin.ICT.Utilities
{
    class SWAppManager
    {
        private ISldWorks _swApp;
        private IModelDoc2 _swDoc;
        private IAssemblyDoc _swPartDoc;
        private ISelectionMgr _swSelMgr;
        //private IPartDoc swPart = default(IPartDoc);
        private string _FilePath;
        private string _FileName;
        private bool _DocOpened;
     
        

        public ISldWorks swInstance => _swApp;
        public IModelDoc2 swActiveDoc => _swDoc;
        //public IAssemblyDoc swPartDoc => _swPartDoc;
        public ISelectionMgr swSelectionMgr => _swSelMgr;
        public bool IsDocOpened => _DocOpened;
        public string CurrentFilePath => _FilePath;
        private string CurrentFileName => _FileName;

        public SWAppManager()
        {
            _swApp = (ISldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
            
        }

        public bool OpenDoc()
        {
            //SldWorks swApp;
            //ModelDoc2 swModelDoc = default(ModelDoc2);
            //AssemblyDoc swAssemblyDoc = default(AssemblyDoc);
            //swApp = (SldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
            //swModelDoc = (ModelDoc2)swApp.ActiveDoc;
            //swAssemblyDoc = (AssemblyDoc)swModelDoc;


            _swDoc = _swApp.IActiveDoc2;
            if (_swDoc != null)
            {
                //_swPartDoc = (IAssemblyDoc)_swApp.IActiveDoc2;
                _swSelMgr = _swDoc.SelectionManager;
                _FilePath = _swDoc.GetPathName();
                _FileName = _swDoc.GetTitle();
                _DocOpened = true;
            }
            else
            {
                _DocOpened = false;
            }//end if (_swDoc != null)
            return _DocOpened;
        }//end public bool OpenDoc()
    }
}
