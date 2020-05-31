using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GIS应用系统
{
    public partial class frmQueryBySpatial : Form
    {
        private IMap currentMap;    //当前MapControl控件中的Map对象        

        /// <summary>
        /// 获得当前MapControl控件中的Map对象。
        /// </summary>
        public IMap CurrentMap
        {
            set
            {
                currentMap = value;
            }
        }

        public frmQueryBySpatial()
        {
            InitializeComponent();
        }

        //窗体加载时触发事件，执行本函数
        private void frmQueryBySpatial_Load(object sender, EventArgs e)
        {
            try
            {
                //清空目标图层列表
                checkedListBoxTargetLayers.Items.Clear();

                string layerName;   //设置临时变量存储图层名称

                //对Map中的每个图层进行判断并添加图层名称
                for (int i = 0; i < currentMap.LayerCount; i++)
                {
                    //如果该图层为图层组类型，则分别对所包含的每个图层进行操作
                    if (currentMap.get_Layer(i) is GroupLayer)
                    {
                        //使用ICompositeLayer接口进行遍历操作
                        ICompositeLayer compositeLayer = currentMap.get_Layer(i) as ICompositeLayer;
                        for (int j = 0; j < compositeLayer.Count; j++)
                        {
                            //将图层的名称添加到checkedListBoxTargetLayers控件和comboBoxMethods控件中
                            layerName = compositeLayer.get_Layer(j).Name;
                            checkedListBoxTargetLayers.Items.Add(layerName);
                            comboBoxSourceLayer.Items.Add(layerName);
                        }
                    }
                    //如果图层不是图层组类型，则直接添加名称
                    else
                    {
                        layerName = currentMap.get_Layer(i).Name;
                        checkedListBoxTargetLayers.Items.Add(layerName);
                        comboBoxSourceLayer.Items.Add(layerName);
                    }
                }

                //将comboBoxSourceLayer控件的默认选项设置为第一个图层的名称
                comboBoxSourceLayer.SelectedIndex = 0;
                //将comboBoxMethods控件的默认选项设置为第一种空间选择方法
                comboBoxMethods.SelectedIndex = 0;
            }
            catch { }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            try
            {
                SelectFeaturesBySpatial();
            }
            catch 
            { }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                SelectFeaturesBySpatial();
                this.Close();
            }
            catch
            { }
        }

        /// <summary>
        /// 在地图中根据图层名称获得矢量图层。
        /// </summary>
        /// <param name="map">当前地图</param>
        /// <param name="layerName">图层名称</param>
        /// <returns>IFeatureLayer接口的矢量图层对象</returns>
        private IFeatureLayer GetFeatureLayerByName(IMap map, string layerName)
        {
            //对地图中的图层进行遍历
            for (int i = 0; i < map.LayerCount; i++)
            {
                //如果该图层为图层组类型，则分别对所包含的每个图层进行操作
                if (map.get_Layer(i) is GroupLayer)
                {
                    //使用ICompositeLayer接口进行遍历操作
                    ICompositeLayer compositeLayer = map.get_Layer(i) as ICompositeLayer;
                    for (int j = 0; j < compositeLayer.Count; j++)
                    {
                        //如果图层名称为所要查询的图层名称，则返回IFeatureLayer接口的矢量图层对象
                        if (compositeLayer.get_Layer(j).Name == layerName)
                            return (IFeatureLayer)compositeLayer.get_Layer(j);
                    }
                }
                //如果图层不是图层组类型，则直接进行判断
                else
                {
                    if (map.get_Layer(i).Name == layerName)
                        return (IFeatureLayer)map.get_Layer(i);
                }
            }
            return null;
        }

        /// <summary>
        /// 将矢量图层中所有要素的几何体进行Union操作得到一个合并后的新几何体。
        /// </summary>
        /// <param name="featureLayer">进行操作的矢量图层</param>
        /// <returns>合并后的新几何体</returns>
        private IGeometry GetFeatureLayerGeometryUnion(IFeatureLayer featureLayer)
        {
            //定义IGeometry接口的对象，存储每一步拓扑操作后得到的几何体
            IGeometry geometry = null;
            //使用ITopologicalOperator接口进行几何体的拓扑操作
            ITopologicalOperator topologicalOperator;
            //使用null作为查询过滤器得到图层中所有要素的游标
            IFeatureCursor featureCursor = featureLayer.Search(null, false);
            //获取IFeature接口的游标中的第一个元素
            IFeature feature = featureCursor.NextFeature();
            //当游标不为空时
            while (feature != null)
            {
                //如果几何体不为空
                if (geometry != null)
                {
                    //进行接口转换，使用当前几何体的ITopologicalOperator接口进行拓扑操作
                    topologicalOperator = geometry as ITopologicalOperator;
                    //执行拓扑合并操作，将当前要素的几何体与已有几何体进行Union，返回新的合并后的几何体
                    geometry = topologicalOperator.Union(feature.Shape);
                }
                else
                    geometry = feature.Shape;
                //移动游标到下一个要素
                feature = featureCursor.NextFeature();
            }
            //返回最新合并后的几何体
            return geometry;
        }

        /// <summary>
        /// 根据已配置的查询条件来执行空间查询操作。
        /// </summary>
        private void SelectFeaturesBySpatial()
        {
            //定义和创建用于空间查询的ISpatialFilter接口的对象
            ISpatialFilter spatialFilter = new SpatialFilterClass();
            //默认设定用于查询的空间几何体为当前地图源图层中所有要素几何体的集合
            spatialFilter.Geometry = GetFeatureLayerGeometryUnion
                (GetFeatureLayerByName(currentMap, comboBoxSourceLayer.SelectedItem.ToString()));
            //根据对空间选择方法的选择采用相应的空间选择方法
            switch (comboBoxMethods.SelectedIndex)
            {
                case 0:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
                case 1:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
                    break;
                case 2:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    break;
                case 3:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
                    break;
                case 4:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;
                    break;
                case 5:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                    break;
                default:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
            }

            //对所选择的目标图层进行遍历，并对每一个图层进行空间查询操作，查询结果将放在选择集中
            IFeatureLayer featureLayer;
            //对所有被选择的目标图层进行遍历
            for (int i = 0; i < checkedListBoxTargetLayers.CheckedItems.Count; i++)
            {
                //根据选择的目标图层名称获得对应的矢量图层
                featureLayer = GetFeatureLayerByName(currentMap, (string)checkedListBoxTargetLayers.CheckedItems[i]);
                //进行接口转换，使用IFeatureSelection接口选择要素
                IFeatureSelection featureSelection = featureLayer as IFeatureSelection;
                //使用IFeatureSelection接口的SelectFeatures方法根据空间查询过滤器选择要素，将其放在新的选择集中
                featureSelection.SelectFeatures((IQueryFilter)spatialFilter, esriSelectionResultEnum.esriSelectionResultAdd, false);
            }

            //进行接口转换，使用IActiveView接口进行视图操作
            IActiveView activeView = currentMap as IActiveView;
            //部分刷新操作，只刷新地理选择集的内容
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, activeView.Extent);
        }
    }
}
