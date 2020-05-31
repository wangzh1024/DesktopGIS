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
    public class StopEditCommandClass:ICommand
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
        public string Caption { get { return "停止编辑"; } }
        public string Category { get { return "编辑按钮"; } }
        public bool Checked { get { return false; } }
        public bool Enabled { get { return bEnable; } }
        public int HelpContextID { get { return -1; } }
        public string HelpFile { get { return ""; } }
        public string Message { get { return "停止编辑"; } }
        public string Name { get { return "StopEditCommand"; } }
        public string Tooltip { get { return "停止编辑"; } }
        public void OnClick()
        {
            map = hookHelper.FocusMap;
            activeView = map as IActiveView;
            engineEditor = MapManager.EngineEditor;
            Boolean bSave = true;
            if (engineEditor == null) return;
            if (engineEditor.EditState != esriEngineEditState.esriEngineStateEditing) return;
            IWorkspaceEdit2 pWsEdit2 = engineEditor.EditWorkspace as IWorkspaceEdit2;            
            if (pWsEdit2.IsBeingEdited())
            {
                Boolean bHasEdit = engineEditor.HasEdits();
                if (bHasEdit)
                {
                    if (MessageBox.Show("是否保存所做的编辑？", "提示", MessageBoxButtons.YesNo,MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        bSave = true;
                    }
                    else
                    {
                        bSave = false;
                    }
                }
                engineEditor.StopEditing(bSave);
            }
            map.ClearSelection();
            activeView.Refresh();
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
            catch 
            { 
                hookHelper = null; 
            }
            if (hookHelper == null) 
                bEnable = false;
            else 
                bEnable = true;
        }
        #endregion
    }
}
