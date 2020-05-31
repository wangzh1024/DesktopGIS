using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;

namespace GIS应用系统
{
    public class CreateFeatureTool:ICommand,ITool
    {
        #region 定义全局变量
        private IMap map = null;
        private bool bEnable = true;
        public static IActiveView activeView = null;
        private IHookHelper hookHelper = null;
        private IEngineEditor engineEditor = null;
        private IEngineEditLayers engineEditLayers = null;
        private IPointCollection pointCollection;
        private INewLineFeedback newLineFeedback;
        private INewPolygonFeedback newPolygonFeedback;
        private INewMultiPointFeedback newMultiPointFeedback;        
        #endregion

        #region ICommand成员
        public int Bitmap { get { return -1; } }
        public string Caption { get { return "添加要素"; } }
        public string Category { get { return "编辑按钮"; } }
        public bool Checked { get { return false; } }
        public bool Enabled { get { return bEnable; } }
        public int HelpContextID { get { return -1; } }
        public string HelpFile { get { return ""; } }
        public string Message { get { return "添加要素过程所做的操作"; } }
        public string Name { get { return "CreateFeatureTool"; } }
        public string Tooltip { get { return "添加要素过程所做的操作"; } }
        public void OnClick()
        {
            map = hookHelper.FocusMap;
            activeView = map as IActiveView;
            engineEditor = MapManager.EngineEditor;
            engineEditLayers = MapManager.EngineEditor as IEngineEditLayers;
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
            if (hookHelper == null) bEnable = false;
            else bEnable = true;
        }
        #endregion

        #region ITool成员
        public int Cursor { get { return -1; } }
        public bool Deactivate(){return true;}
        public bool OnContextMenu(int x, int y) { return false; }
        public void OnDblClick()
        {
            IGeometry resultGeometry = null;
            if (engineEditLayers == null) return;
            //获取编辑目标图层
            IFeatureLayer featureLayer = engineEditLayers.TargetLayer;
            if (featureLayer == null) return;
            IFeatureClass featureClass = featureLayer.FeatureClass;
            if (featureClass == null) return;
            switch(featureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryMultipoint:
                    newMultiPointFeedback.Stop();
                    resultGeometry = pointCollection as IGeometry;
                    newMultiPointFeedback = null;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    IPolyline polyline = null;
                    polyline = newLineFeedback.Stop();
                    resultGeometry = polyline as IGeometry;
                    newLineFeedback = null;
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    IPolygon polygon = null;
                    polygon = newPolygonFeedback.Stop();
                    resultGeometry = polygon as IGeometry;
                    newPolygonFeedback = null;
                    break;
            }
            IZAware zaware = resultGeometry as IZAware;
            zaware.ZAware = true;
            CreateFeature(resultGeometry);//创建新要素
        }
        public void OnKeyDown(int keyCode, int shift) { }
        public void OnKeyUp(int keyCode, int shift) { }
        public void OnMouseDown(int button, int shift, int x, int y)
        {
            try
            {
                IPoint point = activeView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
                if (engineEditor == null) return;
                if (engineEditor.EditState != esriEngineEditState.esriEngineStateEditing) return;
                if (engineEditLayers == null) return;
                IFeatureLayer featureLayer = engineEditLayers.TargetLayer;
                if (featureLayer == null) return;
                IFeatureClass featureClass = featureLayer.FeatureClass;
                if (featureClass == null) return;
                //解决编辑要素的Z值问题
                IZAware zaware = point as IZAware;
                zaware.ZAware = true;
                point.Z = 0;
                object missing = Type.Missing;
                map.ClearSelection();
                switch (featureClass.ShapeType)
                {
                    //当为点层时，直接创建要素
                    case esriGeometryType.esriGeometryPoint:
                        CreateFeature(point as IGeometry);
                        break;
                    //点集的处理方式
                    case esriGeometryType.esriGeometryMultipoint:
                        if (pointCollection == null)
                        {
                            pointCollection = new MultipointClass();
                        }
                        else
                        {
                            pointCollection.AddPoint(point, ref missing,ref missing);
                        }
                        if (newMultiPointFeedback == null)
                        {
                            newMultiPointFeedback = new NewMultiPointFeedbackClass();
                            newMultiPointFeedback.Display = activeView.ScreenDisplay;
                            newMultiPointFeedback.Start(pointCollection,point);
                        }
                        break;
                        //多段线处理方式
                    case esriGeometryType.esriGeometryPolyline:
                        if (newLineFeedback == null)
                        {
                            newLineFeedback = new NewLineFeedbackClass();
                            newLineFeedback.Display = activeView.ScreenDisplay;
                            newLineFeedback.Start(point);
                        }
                        else
                        {
                            newLineFeedback.AddPoint(point);
                        }
                        break;
                        //多边形处理方式
                    case esriGeometryType.esriGeometryPolygon:
                        if (newPolygonFeedback == null)
                        {
                            newPolygonFeedback = new NewPolygonFeedbackClass();
                            newPolygonFeedback.Display = activeView.ScreenDisplay;
                            newPolygonFeedback.Start(point);
                        }
                        else
                        {
                            newPolygonFeedback.AddPoint(point);
                        }
                        break;
                }
            }
            catch(Exception exception)
            {}
        }
        public void OnMouseMove(int button, int shift, int x, int y)
        {
            IPoint point = activeView.ScreenDisplay.DisplayTransformation.ToMapPoint(x,y);
            if (engineEditLayers == null) return;
            //获取编辑目标图层
            IFeatureLayer featureLayer = engineEditLayers.TargetLayer;
            if (featureLayer == null) return;
            IFeatureClass featureClass = featureLayer.FeatureClass;
            if (featureClass == null) return;
            switch(featureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPolyline:
                    if (newLineFeedback != null)
                        newLineFeedback.MoveTo(point);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    if (newLineFeedback != null)
                        newPolygonFeedback.MoveTo(point);
                    break;
            }
        }
        public void OnMouseUp(int button, int shift, int x, int y)
        { }
        public void Refresh(int hdc)
        { }
        #endregion

        #region 操作函数
        /// <summary>
        /// 创建要素
        /// </summary>
        /// <param name="geometry"></param>
        private void CreateFeature(IGeometry geometry)
        {
            try
            {
                if (engineEditLayers == null) return;
                IFeatureLayer featureLayer = engineEditLayers.TargetLayer;
                if (featureLayer == null) return;
                IFeatureClass featureClass = featureLayer.FeatureClass;
                if (featureClass == null) return;
                if (engineEditor == null) return;
                if (geometry == null) return;
                ITopologicalOperator topologicalOperator = geometry as ITopologicalOperator;
                topologicalOperator.Simplify();
                IGeoDataset geoDataset = featureClass as IGeoDataset;
                if (geoDataset.SpatialReference != null)
                {
                    geometry.Project(geoDataset.SpatialReference);
                }
                engineEditor.StartOperation();
                IFeature feature = null;
                feature = featureClass.CreateFeature();
                feature.Shape = SupportZMFeatureClass.ModifyGeometryZMValue(featureClass,geometry);
                feature.Store();
                engineEditor.StopOperation("添加要素");
                map.SelectFeature(featureLayer,feature);
                activeView.Refresh();
            }
            catch(Exception exception)
            {}
        }       
        #endregion

    }
}
