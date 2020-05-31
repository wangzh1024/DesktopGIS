using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.SystemUI;

namespace GIS应用系统
{
    public partial class frmMain : Form
    {
       
        #region 定义全局变量 
        //axMapControl1事件
        private IPoint pPointPt = null;//鼠标点击点  
        //TOC右键菜单       
        IFeatureLayer pTocFeatureLayer = null;            //点击的要素图层
        private frmAttribute frmattribute = null;        //图层属性窗体
        private ILayer pMoveLayer;                        //需要调整显示顺序的图层
        private int toIndex;                              //存放拖动图层移动到的索引号               
        //打印       
        private frmPrintView frmPrintPreview = null;
        //编辑       
        private IMap map = null;
        private IActiveView activeView = null;
        private IFeatureLayer currentLayer = null;
        private IEngineEditor engineEditor = null;
        private IEngineEditTask engineEditTask = null;
        private IEngineEditLayers engineEditLayers = null; 
        //定制工具条
        ICustomizeDialog m_CustomizeDialog = new CustomizeDialogClass();
        ICustomizeDialogEvents_OnStartDialogEventHandler startDialogE;
        ICustomizeDialogEvents_OnCloseDialogEventHandler closeDialogE;
        //地图标注
        public static IMapControl3 m_mapControl = null;     
        #endregion

        #region 初始化
        public frmMain()
        {
            InitializeComponent();
        }
        #endregion

        #region 主窗体Load事件
        private void frmMain_Load(object sender, EventArgs e)
        {
            //地图标注
            m_mapControl = (IMapControl3)axMapControl1.Object;
            //设置伙伴控件关系            
            axTOCControl1.SetBuddyControl(axMapControl1);
            axToolbarControl1.SetBuddyControl(axMapControl1);
            //调用操作函数，设置“编辑”菜单的状态
            ChangeButtonState(true);
            //“编辑”菜单变量实例化
            engineEditor = new EngineEditorClass();
            MapManager.EngineEditor = engineEditor;
            engineEditTask = engineEditor as IEngineEditTask;
            engineEditLayers = engineEditor as IEngineEditLayers;
            //定制工具条
            CreateCustomizeDialog();
        }
        #endregion

        #region axMapControl1事件
        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            //屏幕坐标点转化为地图坐标点
            pPointPt = (axMapControl1.Map as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);

            if (e.button == 1)
            {
                IActiveView pActiveView = axMapControl1.ActiveView;
                IEnvelope pEnvelope = new EnvelopeClass();

                switch (pMouseOperate)
                {
                    #region 拉框放大

                    case "ZoomIn":
                        pEnvelope = axMapControl1.TrackRectangle();
                        //如果拉框范围为空则返回
                        if (pEnvelope == null || pEnvelope.IsEmpty || pEnvelope.Height == 0 || pEnvelope.Width == 0)
                        {
                            return;
                        }
                        //如果有拉框范围，则放大到拉框范围
                        pActiveView.Extent = pEnvelope;
                        pActiveView.Refresh();
                        break;

                    #endregion

                    #region 拉框缩小

                    case "ZoomOut":
                        pEnvelope = axMapControl1.TrackRectangle();

                        //如果拉框范围为空则退出
                        if (pEnvelope == null || pEnvelope.IsEmpty || pEnvelope.Height == 0 || pEnvelope.Width == 0)
                        {
                            return;
                        }
                        //如果有拉框范围，则以拉框范围为中心，缩小倍数为：当前视图范围/拉框范围
                        else
                        {
                            double dWidth = pActiveView.Extent.Width * pActiveView.Extent.Width / pEnvelope.Width;
                            double dHeight = pActiveView.Extent.Height * pActiveView.Extent.Height / pEnvelope.Height;
                            double dXmin = pActiveView.Extent.XMin -
                                           ((pEnvelope.XMin - pActiveView.Extent.XMin) * pActiveView.Extent.Width /
                                            pEnvelope.Width);
                            double dYmin = pActiveView.Extent.YMin -
                                           ((pEnvelope.YMin - pActiveView.Extent.YMin) * pActiveView.Extent.Height /
                                            pEnvelope.Height);
                            double dXmax = dXmin + dWidth;
                            double dYmax = dYmin + dHeight;
                            pEnvelope.PutCoords(dXmin, dYmin, dXmax, dYmax);
                        }
                        pActiveView.Extent = pEnvelope;
                        pActiveView.Refresh();
                        break;

                    #endregion

                    #region 漫游

                    case "Pan":
                        axMapControl1.Pan();
                        break;

                    #endregion

                    #region 选择要素

                    case "SelFeature":
                        IEnvelope pEnv = axMapControl1.TrackRectangle();
                        IGeometry pGeo = pEnv as IGeometry;
                        //矩形框若为空，即为点选时，对点范围进行扩展
                        if (pEnv.IsEmpty == true)
                        {
                            tagRECT r;
                            r.left = e.x - 5;
                            r.top = e.y - 5;
                            r.right = e.x + 5;
                            r.bottom = e.y + 5;
                            pActiveView.ScreenDisplay.DisplayTransformation.TransformRect(pEnv, ref r, 4);
                            pEnv.SpatialReference = pActiveView.FocusMap.SpatialReference;
                        }
                        pGeo = pEnv as IGeometry;
                        axMapControl1.Map.SelectByShape(pGeo, null, false);
                        axMapControl1.Refresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                        break;

                    #endregion
                                                   
                    #region 要素选择
                    case "SelectFeature":
                        IPoint point = new PointClass();
                        IGeometry pGeometry = point as IGeometry;
                        axMapControl1.Map.SelectByShape(pGeometry, null, false);
                        axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                        break;
                    #endregion

                    default:
                        break;
                }
            }
            else if (e.button == 2)
            {
                pMouseOperate = "";
                axMapControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;
            }
        }
        #endregion

        #region 数据视图与布局视图的同步

        private void CopyMapFromMapControlToPageLayoutControl()
        {
            //将MapControl中的地图复制到PageLayoutControl中去
            IObjectCopy pObjectCopy = new ObjectCopyClass();
            object copyFromMap = axMapControl1.Map;
            object copiedMap = pObjectCopy.Copy(copyFromMap);//复制地图到copiedMap中
            object copyToMap = axPageLayoutControl1.ActiveView.FocusMap;
            pObjectCopy.Overwrite(copiedMap, ref copyToMap);//复制地图
            axPageLayoutControl1.ActiveView.Refresh();
        }
        //当MapControl中的地图数据发生重绘时，PageLayoutControl视图也发生相应变化
        private void axMapControl1_OnAfterScreenDraw(object sender, IMapControlEvents2_OnAfterScreenDrawEvent e)
        {
            //获得axPageLayoutControl1当前视图
            IActiveView pPageLayoutView = (IActiveView)axPageLayoutControl1.ActiveView.FocusMap;
            //获得显示转换
            IDisplayTransformation pDisplayTransformation = pPageLayoutView.ScreenDisplay.DisplayTransformation;
            pDisplayTransformation.VisibleBounds = axMapControl1.Extent;//设置范围
            axPageLayoutControl1.ActiveView.Refresh();
            //根据MapControl的视图范围，确定PageLayoutControl的视图范围
            CopyMapFromMapControlToPageLayoutControl();
        }
        #endregion

        #region 实现鹰眼功能
        //鹰眼控件和数据视图控件的数据共享
        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            SynchronizeEagleEye();
        }
        private void SynchronizeEagleEye()
        {
            IMap pMap = axMapControl1.Map;//要复制的地图
            //将数据视图中的地图添加到鹰眼中
            for (int i = 0; i <= pMap.LayerCount - 1; i++)
            {
                eagleEyeMapControl.Map.AddLayer(pMap.get_Layer(i));
            }
            //将鹰眼地图设置为全图显示（有什么作用？）
            eagleEyeMapControl.Extent = eagleEyeMapControl.FullExtent;
            eagleEyeMapControl.Refresh();
            //调用复制方法，将MapControl中的地图复制到PageLayoutControl中去（与鹰眼功能无关）
            CopyMapFromMapControlToPageLayoutControl();
        }
        //主视图范围发生变化
        private void axMapControl1_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            //得到新范围
            IEnvelope pEnv = (IEnvelope)e.newEnvelope;//实例化
            IGraphicsContainer pGra = eagleEyeMapControl.Map as IGraphicsContainer;//实例化
            IActiveView pAv = pGra as IActiveView;//QI 活动视图
            //重新绘制前，清除之前绘制的图形元素
            pGra.DeleteAllElements();
            IRectangleElement pRectangleEle = new RectangleElementClass();
            IElement pEle = pRectangleEle as IElement;
            pEle.Geometry = pEnv;//设置元素的几何形体对象属性
            //设置鹰眼中矩形框的颜色
            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 255;//不透明
            //产生一个线符号对象
            ILineSymbol pOutline = new SimpleLineSymbolClass();
            pOutline.Width = 2;
            pOutline.Color = pColor;
            //设置填充对象的颜色属性
            pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 0;
            //设置填充符号的属性
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Color = pColor;
            pFillSymbol.Outline = pOutline;
            IFillShapeElement pFillShapEle = pEle as IFillShapeElement;
            pFillShapEle.Symbol = pFillSymbol;
            pGra.AddElement((IElement)pFillShapEle, 0);//将元素放入Map对象中
            //活动视图局部刷新以重绘地图
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);//刷新图形元素
        }
        private void axMapControl2_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (this.eagleEyeMapControl.Map.LayerCount != 0)
            {   //左键移动
                if (e.button == 1)
                {
                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(e.mapX, e.mapY);
                    IEnvelope pEnvelope = this.axMapControl1.Extent;
                    pEnvelope.CenterAt(pPoint);
                    this.axMapControl1.Extent = pEnvelope;
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                }
                else if (e.button == 2)
                {
                    IEnvelope pPenvelope = this.eagleEyeMapControl.TrackRectangle();
                    this.axMapControl1.Extent = pPenvelope;
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                }
            }
        }
        private void axMapControl2_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (e.button != 1) return;
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(e.mapX, e.mapY);
            this.axMapControl1.CenterAt(pPoint);
            this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        #endregion

        #region 实现TOCControl右键菜单功能
        private ESRI.ArcGIS.Geometry.Point pMoveLayerPoint = new ESRI.ArcGIS.Geometry.Point();  //鼠标在TOC中左键按下时点的位置
        //TOC右键菜单的添加
        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            try
            {
                if (e.button == 2)
                {
                    esriTOCControlItem pItem = esriTOCControlItem.esriTOCControlItemNone;
                    IBasicMap pMap = null;
                    ILayer pLayer = null;
                    object unk = null;
                    object data = null;
                    axTOCControl1.HitTest(e.x, e.y, ref pItem, ref pMap, ref pLayer, ref unk, ref data);
                    pTocFeatureLayer = pLayer as IFeatureLayer;
                    if (pItem == esriTOCControlItem.esriTOCControlItemLayer && pTocFeatureLayer != null)
                    {
                        contextMenuLayerSel.Enabled = !pTocFeatureLayer.Selectable;
                        contextMenuLayerUnSel.Enabled = pTocFeatureLayer.Selectable;
                        contextMenuStrip.Show(Control.MousePosition);
                    }
                }
                if (e.button == 1)
                {
                    esriTOCControlItem pItem = esriTOCControlItem.esriTOCControlItemNone;
                    IBasicMap pMap = null; object unk = null;
                    object data = null; ILayer pLayer = null;
                    axTOCControl1.HitTest(e.x, e.y, ref pItem, ref pMap, ref pLayer, ref unk, ref data);
                    if (pLayer == null) return;

                    pMoveLayerPoint.PutCoords(e.x, e.y);
                    if (pItem == esriTOCControlItem.esriTOCControlItemLayer)
                    {
                        if (pLayer is IAnnotationSublayer)
                        {
                            return;
                        }
                        else
                        {
                            pMoveLayer = pLayer;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void axTOCControl1_OnMouseUp(object sender, ITOCControlEvents_OnMouseUpEvent e)
        {
            try
            {
                if (e.button == 1 && pMoveLayer != null && pMoveLayerPoint.Y != e.y)
                {
                    esriTOCControlItem pItem = esriTOCControlItem.esriTOCControlItemNone;
                    IBasicMap pBasicMap = null; object unk = null;
                    object data = null; ILayer pLayer = null;
                    axTOCControl1.HitTest(e.x, e.y, ref pItem, ref pBasicMap, ref pLayer, ref unk, ref data);
                    IMap pMap = axMapControl1.ActiveView.FocusMap;
                    if (pItem == esriTOCControlItem.esriTOCControlItemLayer || pLayer != null)
                    {
                        if (pMoveLayer != pLayer)
                        {
                            ILayer pTempLayer;
                            //获得鼠标弹起时所在图层的索引号
                            for (int i = 0; i < pMap.LayerCount; i++)
                            {
                                pTempLayer = pMap.get_Layer(i);
                                if (pTempLayer == pLayer)
                                {
                                    toIndex = i;
                                }
                            }
                        }
                    }
                    //移动到最前面
                    else if (pItem == esriTOCControlItem.esriTOCControlItemMap)
                    {
                        toIndex = 0;
                    }
                    //移动到最后面
                    else if (pItem == esriTOCControlItem.esriTOCControlItemNone)
                    {
                        toIndex = pMap.LayerCount - 1;
                    }
                    pMap.MoveLayer(pMoveLayer, toIndex);
                    axMapControl1.ActiveView.Refresh();
                    axTOCControl1.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // 属性表窗口
        private void contextMenuAtributeTable_Click(object sender, EventArgs e)
        {
            if (frmattribute == null || frmattribute.IsDisposed)
            {
                frmattribute = new frmAttribute();
            }
            frmattribute.CurFeatureLayer = pTocFeatureLayer;
            frmattribute.InitUI();
            frmattribute.ShowDialog();
        }

        //缩放到图层
        private void contextMenuZoomToLayer_Click(object sender, EventArgs e)
        {
            if (pTocFeatureLayer == null) return;
            (axMapControl1.Map as IActiveView).Extent = pTocFeatureLayer.AreaOfInterest;
            (axMapControl1.Map as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        // 移除图层
        private void contextMenuRemoveLayer_Click(object sender, EventArgs e)
        {
            try
            {
                if (pTocFeatureLayer == null) return;
                DialogResult result = MessageBox.Show("是否删除[" + pTocFeatureLayer.Name + "]图层", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    axMapControl1.Map.DeleteLayer(pTocFeatureLayer);
                }
                axMapControl1.ActiveView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //图层可选
        private void contextMenuLayerSel_Click(object sender, EventArgs e)
        {
            pTocFeatureLayer.Selectable = true;
            contextMenuLayerSel.Enabled = !contextMenuLayerSel.Enabled;
        }

        //图层不可选
        private void contextMenuLayerUnSel_Click(object sender, EventArgs e)
        {
            pTocFeatureLayer.Selectable = false;
            contextMenuLayerUnSel.Enabled = !contextMenuLayerUnSel.Enabled;
        }

        #region 导出数据
        private void contextMenuExportData_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (DialogResult.OK == dlg.ShowDialog())
            {
                string file = dlg.FileName.Substring(0, dlg.FileName.LastIndexOf('\\'));
                if (!System.IO.Directory.Exists(file))
                {
                    System.IO.Directory.CreateDirectory(file);
                }
                try
                {
                    ILayer pLayer = axMapControl1.Map.get_Layer(0);
                    if (pLayer != null)
                    {
                        IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                        if (pFeatureLayer.Visible)
                        {
                            ExportFeature(pFeatureLayer.FeatureClass, dlg.FileName);
                        }
                    }
                    MessageBox.Show("导出成功");
                }
                catch
                {
                    MessageBox.Show("导出失败！");
                }
            }

        }  
   
        public void ExportFeature(IFeatureClass pInFeatureClass, string pPath)
        {
            // create a new Access workspace factory       
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
            string parentPath = pPath.Substring(0, pPath.LastIndexOf('\\'));
            string fileName = pPath.Substring(pPath.LastIndexOf('\\') + 1, pPath.Length - pPath.LastIndexOf('\\') - 1);
            IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create(parentPath, fileName, null, 0);
            // Cast for IName       
            IName name = (IName)pWorkspaceName;
            //Open a reference to the access workspace through the name object       
            IWorkspace pOutWorkspace = (IWorkspace)name.Open();
            IDataset pInDataset = pInFeatureClass as IDataset;
            IFeatureClassName pInFCName = pInDataset.FullName as IFeatureClassName;
            IWorkspace pInWorkspace = pInDataset.Workspace;
            IDataset pOutDataset = pOutWorkspace as IDataset;
            IWorkspaceName pOutWorkspaceName = pOutDataset.FullName as IWorkspaceName;
            IFeatureClassName pOutFCName = new FeatureClassNameClass();
            IDatasetName pDatasetName = pOutFCName as IDatasetName;
            pDatasetName.WorkspaceName = pOutWorkspaceName;
            pDatasetName.Name = pInFeatureClass.AliasName;
            IFieldChecker pFieldChecker = new FieldCheckerClass();
            pFieldChecker.InputWorkspace = pInWorkspace;
            pFieldChecker.ValidateWorkspace = pOutWorkspace;
            IFields pFields = pInFeatureClass.Fields;
            IFields pOutFields;
            IEnumFieldError pEnumFieldError;
            pFieldChecker.Validate(pFields, out pEnumFieldError, out pOutFields);
            IFeatureDataConverter pFeatureDataConverter = new FeatureDataConverterClass();
            pFeatureDataConverter.ConvertFeatureClass(pInFCName, null, null, pOutFCName, null, pOutFields, "", 100, 0);
        }
        #endregion

        #endregion

        #region 定制工具条
        private void chkCustomize_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCustomize.Checked == false)
            {
                m_CustomizeDialog.CloseDialog();
            }
            else
            {
                m_CustomizeDialog.StartDialog(axToolbarControl1.hWnd);
            }
        }
        private void CreateCustomizeDialog()
        {
            // Set the customize dialog box events.
            ICustomizeDialogEvents_Event pCustomizeDialogEvent = m_CustomizeDialog as ICustomizeDialogEvents_Event;
            startDialogE = new ICustomizeDialogEvents_OnStartDialogEventHandler(OnStartDialogHandler);
            pCustomizeDialogEvent.OnStartDialog += startDialogE;
            closeDialogE = new ICustomizeDialogEvents_OnCloseDialogEventHandler(OnCloseDialogHandler);
            pCustomizeDialogEvent.OnCloseDialog += closeDialogE;
            // Set the title.
            m_CustomizeDialog.DialogTitle = "定制工具条";
            // Set the ToolbarControl that new items will be added to.
            m_CustomizeDialog.SetDoubleClickDestination(axToolbarControl1);
        }
        private void OnStartDialogHandler()
        {
            axToolbarControl1.Customize = true;

        }
        private void OnCloseDialogHandler()
        {
            axToolbarControl1.Customize = false;
            chkCustomize.Checked = false;
        }
        #endregion

        #region 文件菜单

        #region 新建
        private void menuNewDoc_Click(object sender, EventArgs e)
        {
            file file1 = new file();
            file1.NewMapDoc(axMapControl1);           
        }
        #endregion

        #region 打开
        private void menuOpenDoc_Click(object sender, EventArgs e)
        {
            file file1 = new file();
            file1.OpenMapDocument(axMapControl1);
        }
        #endregion

        #region 保存
        private void menuSaveDoc_Click(object sender, EventArgs e)
        {
            file file1 = new file();
            file1.SaveDocument(axMapControl1);
        }
        #endregion

        #region 另存为
        private void menuSaveAsDoc_Click(object sender, EventArgs e)
        {
            file file1 = new file();
            file1.SaveAsDocument(axMapControl1);
        }
        #endregion

        #region 打印
        private void menuPrintMap_Click(object sender, EventArgs e)
        {
            frmPrintPreview = new frmPrintView(axPageLayoutControl1);
            frmPrintPreview.ShowDialog();
        }
        #endregion

        #region 导出地图
        private void menuExportMap_Click(object sender, EventArgs e)
        {
            file file1 = new file();
            file1.ExportMapToImage(axPageLayoutControl1);
        }
        #endregion

        #region 退出
        private void menuExitProgram_Click(object sender, EventArgs e)
        {           
            file file1 = new file();
            file1.ExitProgram(axMapControl1,axTOCControl1);
        }
        #endregion

        #endregion

        #region 编辑菜单

        #region 开始编辑
        private void menuStartEdit_Click(object sender, EventArgs e)
        {
            map = axMapControl1.Map;
            activeView = map as IActiveView;
            if (axMapControl1.Map.LayerCount == 0)
            {
                MessageBox.Show("请加载编辑图层", "提示", MessageBoxButtons.OK);
                return;
            }
            map.ClearSelection();
            activeView.Refresh();
            //调用操作函数，将“开始编辑”设置为非激活状态，其他子菜单项设置为激活状态
            ChangeButtonState(false);
            //初始化“图层选择”
            menuSelectLayer.Items.Clear();
            for (int i = 0; i < axMapControl1.LayerCount; i++)
            {
                menuSelectLayer.Items.Add(axMapControl1.Map.get_Layer(i).Name);
            }
            //默认选择顶层图层
            if (menuSelectLayer.Items.Count != 0)
            {
                menuSelectLayer.SelectedIndex = 0;
            }
            //获取当前编辑图层工作空间
            IDataset dataSet = currentLayer.FeatureClass as IDataset;
            IWorkspace workspace = dataSet.Workspace;
            //设置编辑模式，如果是ArcSDE采用版本模式
            if (workspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
            {
                engineEditor.EditSessionMode = esriEngineEditSessionMode.esriEngineEditSessionModeVersioned;
            }
            else
            {
                engineEditor.EditSessionMode = esriEngineEditSessionMode.esriEngineEditSessionModeNonVersioned;
            }
            //设置编辑任务
            engineEditTask = engineEditor.GetTaskByUniqueName("ControlToolsEditing_CreateNewFeatureTask");
            engineEditor.CurrentTask = engineEditTask;
            engineEditor.EnableUndoRedo(true);//是否可以进行撤销、恢复操作
            engineEditor.StartEditing(workspace, map);//开始编辑操作            
        }
        #endregion

        #region 保存编辑
        private void menuSaveEdit_Click(object sender, EventArgs e)
        {
            ICommand saveEditCom = new SaveEditCommandClass();
            saveEditCom.OnCreate(axMapControl1.Object);
            saveEditCom.OnClick();
        }
        #endregion

        #region 结束编辑
        private void menuEndEdit_Click(object sender, EventArgs e)
        {
            try
            {
                ICommand stopEditCommand = new StopEditCommandClass();
                stopEditCommand.OnCreate(axMapControl1.Object);
                stopEditCommand.OnClick();
                axMapControl1.CurrentTool = null;
                axMapControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;
            }

            catch (Exception exception)
            {
            }
            //调用操作函数，将“开始编辑”设置为激活状态，其他子菜单项设置为非激活状态
            ChangeButtonState(true);
        }
        #endregion

        #region 图层选择
        private void menuSelectLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (menuSelectLayer.SelectedItem == null) return;
            string LayerName = menuSelectLayer.SelectedItem.ToString();
            for (int i = 0; i < axMapControl1.Map.LayerCount; i++)
            {
                ILayer layer = axMapControl1.get_Layer(i);
                if (layer.Name == LayerName)
                {
                    currentLayer = layer as IFeatureLayer;
                    break;
                }
            }
            //设置目标图层
            engineEditLayers.SetTargetLayer(currentLayer, 0);
            //获取当前编辑图层工作空间
            IDataset pDataset = currentLayer.FeatureClass as IDataset;
            IWorkspace pWorkspace = pDataset.Workspace;
            MessageBox.Show("OK!");
        }
        #endregion

        #region 添加要素
        private void menuAddLayerFeature_Click(object sender, EventArgs e)
        {
            try
            {
                ICommand createFeatureTool = new CreateFeatureTool();
                createFeatureTool.OnCreate(axMapControl1.Object);
                createFeatureTool.OnClick();
                axMapControl1.CurrentTool = createFeatureTool as ITool;
                axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            }
            catch (Exception ex) { }

        }
        #endregion

        #region 操作函数
        //用于改变“编辑”菜单子菜单项的状态
        private void ChangeButtonState(bool isActivate)
        {
            menuStartEdit.Enabled = isActivate;
            menuSaveEdit.Enabled = !isActivate;
            menuEndEdit.Enabled = !isActivate;
            menuSelectLayer.Enabled = !isActivate;
            menuAddLayerFeature.Enabled = !isActivate;
        }
        #endregion

        #endregion
    
        #region 地图浏览
        //拉框放大
        string pMouseOperate = null;
        private void menuZoomIn_Click(object sender, EventArgs e)
        {
            axMapControl1.CurrentTool = null;
            pMouseOperate = "ZoomIn";
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerZoomIn;
        }

        //拉框缩小
        private void menuZoomOut_Click(object sender, EventArgs e)
        {
            axMapControl1.CurrentTool = null;
            pMouseOperate = "ZoomOut";
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerZoomOut;
        }

        //漫游
        private void menuPan_Click(object sender, EventArgs e)
        {
            axMapControl1.CurrentTool = null;
            pMouseOperate = "Pan";
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerPan;
        }

        //全图显示
        private void menuFullExtent_Click(object sender, EventArgs e)
        {
            axMapControl1.Extent = axMapControl1.FullExtent;
        }
          
        #endregion

        #region 数据加载

        #region 添加矢量数据
        private void menuAddShapefile_Click(object sender, EventArgs e)
        {
            //利用工作空间加载shp格式文件（可同时添加多个shp文件）
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "请选择矢量数据文件";
            openFileDialog.Filter = "矢量数据文件(*.shp)|*.shp";
            openFileDialog.Multiselect = true;//是否允许选择多个文件
            openFileDialog.ShowDialog();
            string[] strFileNameS = openFileDialog.FileNames;
            for (int i = 0; i < strFileNameS.Length; i++)
            {
                ILayer layer = AddShp(strFileNameS[i]);
                axMapControl1.Map.AddLayer(layer);
                //同步鹰眼
                SynchronizeEagleEye();
            }
        }
        //添加shp的自定义方法
        private ILayer AddShp(string strFileName)
        {
            string workSpacePath = System.IO.Path.GetDirectoryName(strFileName);//返回指定路径字符串的目录信息
            string shapeFileName = System.IO.Path.GetFileName(strFileName);//返回指定路径字符串的的文件名和扩展名
            IWorkspaceFactory workSpaceFactory = new ShapefileWorkspaceFactoryClass();
            IWorkspace workSpace = workSpaceFactory.OpenFromFile(workSpacePath, 0);//打开工作空间
            IFeatureWorkspace featureWorkSpace = workSpace as IFeatureWorkspace;//接口转换
            //得到要素类
            IFeatureClass pFeatureClass = featureWorkSpace.OpenFeatureClass(shapeFileName);
            IDataset pDataset = pFeatureClass as IDataset;
            IFeatureLayer pFeatureLayer = new FeatureLayerClass();
            pFeatureLayer.FeatureClass = pFeatureClass;
            pFeatureLayer.Name = pDataset.Name;
            ILayer player = pFeatureLayer as ILayer;
            return player;
        }

        #endregion

        #region 添加栅格数据
        private void menuAddRaster_Click(object sender, EventArgs e)
        {
            OpenFileDialog pOpenFileDialog = new OpenFileDialog();
            pOpenFileDialog.CheckFileExists = true;
            pOpenFileDialog.Title = "打开Raster文件";
            pOpenFileDialog.Filter = "栅格文件 (*.*)|*.bmp;*.tif;*.jpg;*.img|(*.bmp)|*.bmp|(*.tif)|*.tif|(*.jpg)|*.jpg|(*.img)|*.img";
            pOpenFileDialog.ShowDialog();
            string pRasterFileName = pOpenFileDialog.FileName;
            if (pRasterFileName == "")
            {
                return;
            }
            string pPath = System.IO.Path.GetDirectoryName(pRasterFileName);
            string pFileName = System.IO.Path.GetFileName(pRasterFileName);
            IWorkspaceFactory pWorkspaceFactory = new RasterWorkspaceFactory();
            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(pPath, 0);
            IRasterWorkspace pRasterWorkspace = pWorkspace as IRasterWorkspace;
            IRasterDataset pRasterDataset = pRasterWorkspace.OpenRasterDataset(pFileName);
            //影像金字塔判断与创建
            IRasterPyramid3 pRasPyrmid;
            pRasPyrmid = pRasterDataset as IRasterPyramid3;
            if (pRasPyrmid != null)
            {
                if (!(pRasPyrmid.Present))
                {
                    pRasPyrmid.Create(); //创建金字塔
                }
            }
            IRaster pRaster;
            pRaster = pRasterDataset.CreateDefaultRaster();
            IRasterLayer pRasterLayer;
            pRasterLayer = new RasterLayerClass();
            pRasterLayer.CreateFromRaster(pRaster);
            ILayer pLayer = pRasterLayer as ILayer;
            axMapControl1.AddLayer(pLayer, 0);
        }
        #endregion

        #region 添加XY数据
        private void menuAddXYData_Click(object sender, EventArgs e)
        {
            frmAddXYData pFrmXYToPoint = new frmAddXYData();
            pFrmXYToPoint.GetAxMapControl = this.axMapControl1;
            pFrmXYToPoint.GetAxTOCControl = this.axTOCControl1;
            pFrmXYToPoint.ShowDialog();
        }
        #endregion

        #endregion

        #region 数据查询

        #region 属性查询
        private void menuQueryByAttribute_Click(object sender, EventArgs e)
        {
            //新创建属性查询窗体
            frmQueryByAttribute frmQueryByAttribute = new frmQueryByAttribute();
            //将当前主窗体中MapControl控件中的Map对象赋值给frmQueryByAttribute窗体的CurrentMap属性
            frmQueryByAttribute.CurrentMap = axMapControl1.Map;
            //显示属性查询窗体
            frmQueryByAttribute.Show();
        }
        #endregion

        #region 空间查询
        private void menuQueryBySpatial_Click(object sender, EventArgs e)
        {
            //新创建空间查询窗体
            frmQueryBySpatial frmQueryBySpatial = new frmQueryBySpatial();
            //将当前主窗体中MapControl控件中的Map对象赋值给frmQueryBySpatial窗体的CurrentMap属性
            frmQueryBySpatial.CurrentMap = axMapControl1.Map;
            //显示空间查询窗体
            frmQueryBySpatial.Show();
        }
        #endregion

        #region 统计
        private void menuStatistics_Click(object sender, EventArgs e)
        {
            //新创建统计窗体
            frmStatistics formStatistics = new frmStatistics();
            //将当前主窗体中MapControl控件中的Map对象赋值给frmStatistics窗体的CurrentMap属性
            formStatistics.CurrentMap = axMapControl1.Map;
            //显示统计窗体
            formStatistics.Show();
        }
        #endregion

        #endregion

        #region 地图制图

        #region 符号系统

        #region 单一符号化
        private void menuSingleSymbol_Click(object sender, EventArgs e)
        {
            ICommand command = new SingleSymbolCmd();
            command.OnCreate(axMapControl1.Object);
            command.OnClick();
        }
        #endregion

        #region 唯一值符号化
        private void menuUniqueValuesSymbol_Click(object sender, EventArgs e)
        {
            ICommand command = new UniqueValuesSymbolCmd();
            command.OnCreate(axMapControl1.Object);
            command.OnClick();
        }

        #endregion

        #region 分级色彩符号化
        private void menuGraduatedColorSymbol_Click(object sender, EventArgs e)
        {
            ICommand command = new GraduatedColorSymbolsCmd();
            command.OnCreate(axMapControl1.Object);
            command.OnClick();
        }
        #endregion

        #region 依比例符号化
        private void menuProportionalSymbols_Click(object sender, EventArgs e)
        {
            ICommand command = new ProportionalSymbolsCmd();
            command.OnCreate(axMapControl1.Object);
            command.OnClick();
        }
        #endregion

        #region 点密度符号化
        private void menuDotDensitySymbols_Click(object sender, EventArgs e)
        {
            ICommand command = new DotDensitySymbolsCmd();
            command.OnCreate(axMapControl1.Object);
            command.OnClick();
        }
        #endregion

        #endregion

        #region 地图标注
        private void menuMapMark_Click(object sender, EventArgs e)
        {
            frmMapMark frmmapMark = new frmMapMark(m_mapControl);
            frmmapMark.Show();
        }
        #endregion     

        #endregion

        #region GP工具

        #region 系统工具
        /// <summary>
        /// 缓冲区分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuGPBuffer_Click(object sender, EventArgs e)
        {
            frmBuffer frmB = new frmBuffer();
            frmB.Show();
            #region 简单例子
            //Geoprocessor gp = new Geoprocessor();
            //object sev = null;
            //try
            //{                   
            //    gp.OverwriteOutput = true;//输出文件可覆盖
            //    ESRI.ArcGIS.AnalysisTools.Buffer pBuffer = new ESRI.ArcGIS.AnalysisTools.Buffer();//创建工具流程对象
            //    //获取缓冲区分析图层
            //    ILayer pLayer = this.axMapControl1.get_Layer(0);
            //    IFeatureLayer featLayer = pLayer as IFeatureLayer;
            //    pBuffer.in_features = featLayer;//输入参数
            //    string filepath = @"c:\";
            //    //设置生成结果存储路径
            //    pBuffer.out_feature_class = filepath + "\\" + pLayer.Name + ".shp";
            //    //设置缓冲区距离
            //    pBuffer.buffer_distance_or_field = "5000 Meters";
            //    pBuffer.dissolve_option = "ALL";               
            //    gp.Execute(pBuffer, null);//执行缓冲区分析
            //    MessageBox.Show(gp.GetMessages(ref sev));//输出地理处理工具执行结果
            //    //将生成结果添加到地图中
            //    this.axMapControl1.AddShapeFile(filepath, pLayer.Name);
            //    this.axMapControl1.MoveLayerTo(1, 0);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(gp.GetMessages(ref sev));//输出地理处理工具执行结果
            //   // MessageBox.Show("请输入有效数据");                
            //}
            #endregion
        }
        /// <summary>
        /// 交集制表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuIntersectTable_Click(object sender, EventArgs e)
        {
        //    Geoprocessor gp = new Geoprocessor();
        //    object sev = null;
        //    gp.OverwriteOutput = true;//输出文件可覆盖
        //    ESRI.ArcGIS.AnalysisTools.TabulateIntersection tabulateIntersection = new ESRI.ArcGIS.AnalysisTools.TabulateIntersection();
        //    tabulateIntersection.in_zone_features = @"‪C:\Users\HP\Desktop\test\A_ZD.shp";
        //    tabulateIntersection.zone_fields = "BM";
        //    tabulateIntersection.in_class_features = @"‪C:\Users\HP\Desktop\test\P_QY.shp";
        //    tabulateIntersection.out_table = @"C:\Users\HP\Desktop\test\test.dbf";
        //    tabulateIntersection.class_fields = "BM";
        //    tabulateIntersection.sum_fields = "cz";
              
        //    try
        //    {
        //        gp.Execute(tabulateIntersection, null);//执行自定义工具
        //        MessageBox.Show(gp.GetMessages(ref sev));//输出地理处理工具执行结果
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(gp.GetMessages(ref sev));
        //        //MessageBox.Show("请输入有效数据");
        //    }
        }
        #endregion
        
        #region 自定义工具
        private void menuCustomTool_Click(object sender, EventArgs e)
        {   
            ////定义GeoProcessor对象       
            //Geoprocessor gp = new Geoprocessor();
            //object sev = null;
            ////设置参数
            //gp.OverwriteOutput = true;
            ////设置工具箱所在的路径
            //gp.AddToolbox(@"F:\lib_test\AirportsAndGolf.tbx");            
            ////生成参数数组，并设置输入参数
            //IVariantArray parameters = new VarArrayClass();
            //parameters.Add(filepath);
            //parameters.Add("100 Feet");
            //parameters.Add(filepath);          
            //try
            //{                
            //    gp.Execute("Model2", parameters, null);//执行自定义工具
            //    MessageBox.Show(gp.GetMessages(ref sev));//输出地理处理工具执行结果
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("请输入有效数据");          
            //}

        }       
        #endregion

        #endregion

        #region 帮助

        private void menuAboutSoftware_Click(object sender, EventArgs e)
        {
            frmAboutSoftware aboutSoftware = new frmAboutSoftware();
            aboutSoftware.Show();
        }

        #endregion

    }
}
