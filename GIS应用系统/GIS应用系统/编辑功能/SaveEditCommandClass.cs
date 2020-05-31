using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SystemUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GIS应用系统
{
    class SaveEditCommandClass:ICommand
    {

        #region 定义变量
        private IMap map = null;
        private bool bEnable = true;
        private IActiveView activeView = null;
        private IHookHelper hookHelper = null;
        private IEngineEditor engineEditor = null;
        #endregion

        #region ICommand成员
        public int Bitmap { get { return -1; } }
        public string Caption { get { return "保存编辑"; } }
        public string Category { get { return "编辑按钮"; } }
        public bool Checked { get { return false; } }
        public bool Enabled { get { return bEnable; } }
        public int HelpContextID { get { return -1; } }
        public string HelpFile { get { return ""; } }
        public string Message { get { return "保存编辑过程所做的操作"; } }
        public string Name { get { return "SaveEditCommand"; } }
        public string Tooltip { get { return "保存编辑过程所做的操作"; } }
        public void OnClick()
        { 
            map = hookHelper.FocusMap;
            activeView = map as IActiveView;
            engineEditor = MapManager.EngineEditor;
            if(engineEditor == null) return;
            if(engineEditor.EditState != esriEngineEditState.esriEngineStateEditing) return;
            IWorkspace pWs = engineEditor.EditWorkspace;
            Boolean bHasEdit = engineEditor.HasEdits();
            if(bHasEdit)
            {
                if(MessageBox.Show("是否保存所做的编辑？","提示",MessageBoxButtons.YesNo,MessageBoxIcon.Information)==DialogResult.Yes)
                {
                    engineEditor.StopEditing(true);
                    engineEditor.StartEditing(pWs,map);
                    activeView.Refresh();
                }
            }
        }
        public void OnCreate(object Hook)
        {
            if (Hook == null) return;
            try
            {
                hookHelper = new HookHelperClass();
                hookHelper.Hook = Hook;
                if (hookHelper.ActiveView == null)
                {
                    hookHelper = null;
                }
            }
            catch { hookHelper = null; }
            if (hookHelper == null) 
                bEnable = false;
            else 
                bEnable = true;
        }
        #endregion

    }
}
