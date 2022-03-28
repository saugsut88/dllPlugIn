
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorksTools;
using SolidWorks.Interop.swconst;
using System.Runtime.InteropServices;


namespace VPS.SWplugin.ICT.Utilities
{
    class SWSelectionManager
    {
        #region Private Stuffs
        private SWAppManager _swApp = new SWAppManager();
        private Entity _swEntity;
        private int _SelectedComponentIndex;
        private Component2 _CurrentComponent;
        private int _SelectedComponentCount;
        private swSelectType_e _CurrentComponentType;
        private bool _EOF;
        private bool _BOF;
        private bool _DocOpened;

        private int NextIndex()
        {
            if (_SelectedComponentIndex == _SelectedComponentCount)
            {
                _EOF = true;
                _BOF = false;
                return _SelectedComponentCount;
            }
            else
            {
                _EOF = false;
                _BOF = false;
                return _SelectedComponentIndex + 1;
            }//endif if(_SelectedComponentIndex==_SelectedComponentCount)
        }//end   private void IncrementIndex()

        private int PrevIndex()
        {
            if (_SelectedComponentIndex == 1)
            {
                _EOF = false;
                _BOF = true;
                return 1;
            }
            else
            {
                _EOF = false;
                _BOF = false;
                return _SelectedComponentIndex - 1;
            }//endif if(_SelectedComponentIndex==_SelectedComponentCount)
        }//end   private void PrevIndex()
        #endregion Private Stuffs


        #region Public Stuff
        public SWSelectionManager()
        {

            if (_swApp.OpenDoc())
            {
                _SelectedComponentIndex = 0;
                _SelectedComponentCount = _swApp.swSelectionMgr.GetSelectedObjectCount();
                if (_SelectedComponentCount > 0)
                {
                    _EOF = false;
                    _BOF = true;
                    _DocOpened = true;
                    MoveFirstComponent();
                }
                else
                {
                    _BOF = true;
                    _EOF = true;
                   
                }//end if(_SelectedComponentCount>0)
            }
            else
            {
                _DocOpened = false;
            }//end if (_swApp.OpenDoc())

        }//end public SWSelectionManager()

        ~SWSelectionManager()
        {
            Marshal.ReleaseComObject(_swApp);
            _swApp = null;
            _swEntity = null;
            _CurrentComponent = null;
        }

        public bool IsDocOpen() => _swApp.IsDocOpened;
        public bool EOF() => _EOF;
        public bool BOF() => _BOF;
        public swSelectType_e CurrentComponentType() => _CurrentComponentType;

        public Component2 GetCurrentComponent()
        {
            _swEntity = _swApp.swSelectionMgr.GetSelectedObject6(_SelectedComponentIndex, -1);
            return (Component2)_swEntity.IGetComponent2();
        }//end public Component2 GetCurrentComponent()

        public Component2 GetFirstSelectedComponent()
        {
            _SelectedComponentIndex = 1;
            _swEntity = _swApp.swSelectionMgr.GetSelectedObject6(_SelectedComponentIndex, -1);
            _CurrentComponentType = (swSelectType_e)_swApp.swSelectionMgr.GetSelectedObjectType3(_SelectedComponentIndex, -1);
            _CurrentComponent = (Component2)_swEntity.IGetComponent2();
            return _CurrentComponent;
        }//end  public Component2 GetFirstSelectedComponent()

        public Component2 GetLastSelectedComponent()
        {
            _SelectedComponentIndex = _SelectedComponentCount;
            _swEntity = _swApp.swSelectionMgr.GetSelectedObject6(_SelectedComponentIndex,-1);
            _CurrentComponentType = (swSelectType_e)_swApp.swSelectionMgr.GetSelectedObjectType3(_SelectedComponentIndex, -1);
            _CurrentComponent = (Component2)_swEntity.IGetComponent2();
            return _CurrentComponent;
        }//end public Component2 GetLastSelectedComponent()

        public int GetSelectedObjectCount()
        {
            return _swApp.swSelectionMgr.GetSelectedObjectCount();
        }//end public int GetSelectedObjectCount()

        public Component2 MoveFirstComponent()
        {
            _SelectedComponentIndex = 1;
            _swEntity = _swApp.swSelectionMgr.GetSelectedObject6(_SelectedComponentIndex, -1);
            _CurrentComponentType = (swSelectType_e)_swApp.swSelectionMgr.GetSelectedObjectType3(_SelectedComponentIndex, -1);
            _CurrentComponent = (Component2)_swEntity.IGetComponent2();
            return _CurrentComponent;
        }//end public Component2 MoveFirstComponent()

        public Component2 MoveLastComponent()
        {
            _SelectedComponentIndex = _SelectedComponentCount;
            _swEntity = _swApp.swSelectionMgr.GetSelectedObject6(_SelectedComponentIndex, -1);
            _CurrentComponentType = (swSelectType_e)_swApp.swSelectionMgr.GetSelectedObjectType3(_SelectedComponentIndex, -1);
            _CurrentComponent = (Component2) _swEntity.IGetComponent2();
            return _CurrentComponent;
        }//end public Component2 MoveLastComponent()

        public Component2 MoveNextComponent()
        {
            _SelectedComponentIndex = NextIndex();
            _swEntity = _swApp.swSelectionMgr.GetSelectedObject6(_SelectedComponentIndex, -1);
            _CurrentComponentType = (swSelectType_e)_swApp.swSelectionMgr.GetSelectedObjectType3(_SelectedComponentIndex, -1);
            _CurrentComponent = (Component2)_swEntity.IGetComponent2();
            return _CurrentComponent;
        }//end public Component2 MoveNextComponent()

        public Component2 MovePrevComponent()
        {
            _SelectedComponentIndex = PrevIndex();
            _swEntity = _swApp.swSelectionMgr.GetSelectedObject6(_SelectedComponentIndex, -1);
            _CurrentComponentType = (swSelectType_e)_swApp.swSelectionMgr.GetSelectedObjectType3(_SelectedComponentIndex, -1);
            _CurrentComponent = (Component2)_swEntity.IGetComponent2();
            return _CurrentComponent;
        }//end public Component2 MovePrevComponent()

        
        #endregion Public Stuffs


       

       

        
    }//end class SWSelectionManager
}// end namespace VPS.SWplugin.ICT.Utilities
