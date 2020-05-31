using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GIS应用系统
{
    public partial class GraduatedColorSymbols : Form
    {
        IHookHelper m_hookHelper = null;
        IActiveView m_activeView = null;
        IMap m_map = null;

        IFeatureLayer layer2Symbolize = null;
        string strRendererField = string.Empty;
        string strNormalizeField = string.Empty;

        int gClassCount = 5;
        double[] gClassbreaks = null;
        string strClassifyMethod = "自然裂点分类";
        IClassBreaksRenderer m_classBreaksRenderer = null;

        IStyleGallery pStyleGlry = new ServerStyleGalleryClass();
        IColorRamp colorRamp = null;


        public GraduatedColorSymbols(IHookHelper hookHelper)
        {
            InitializeComponent();
            m_hookHelper = hookHelper;
            m_activeView = m_hookHelper.ActiveView;
            m_map = m_hookHelper.FocusMap;
        }

        private void GraduatedColorSymbols_Load(object sender, EventArgs e)
        {
            CbxLayersAddItems();
        }

        private void cbxLayers2Symbolize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLayers2Symbolize.SelectedItem != null)
            {
                string strLayer2Symbolize = cbxLayers2Symbolize.SelectedItem.ToString();
                layer2Symbolize = GetFeatureLayer(strLayer2Symbolize);
                CbxFieldAdditems(layer2Symbolize);
                strRendererField = cbxFields.Items[0].ToString();
            }
        }

        private void CbxFieldAdditems(IFeatureLayer featureLayer)
        {
            IFields fields = featureLayer.FeatureClass.Fields;
            cbxFields.Items.Clear();
            cbxNormalization.Items.Clear();
            cbxNormalization.Items.Add("None");

            for (int i = 0; i < fields.FieldCount; i++)
            {
                if ((fields.get_Field(i).Type == esriFieldType.esriFieldTypeDouble) ||
                    (fields.get_Field(i).Type == esriFieldType.esriFieldTypeInteger) ||
                    (fields.get_Field(i).Type == esriFieldType.esriFieldTypeSingle) ||
                    (fields.get_Field(i).Type == esriFieldType.esriFieldTypeSmallInteger))
                {
                    cbxFields.Items.Add(fields.get_Field(i).Name);
                    cbxNormalization.Items.Add(fields.get_Field(i).Name);
                }
            }
            cbxFields.SelectedIndex = 0;
            cbxNormalization.SelectedIndex = 0;
        }

        private void CbxLayersAddItems()
        {
            if (GetLayers() == null) return;
            IEnumLayer layers = GetLayers();
            layers.Reset();
            ILayer layer = layers.Next();
            while (layer != null)
            {
                if (layer is IFeatureLayer)
                {
                    cbxLayers2Symbolize.Items.Add(layer.Name);
                }
                layer = layers.Next();
            }
        }

        private void btnSymbolize_Click(object sender, EventArgs e)
        {
            if (layer2Symbolize == null) return;
            Renderer();
        }

        private void Renderer()
        {
            IGeoFeatureLayer pGeoFeatureL = (IGeoFeatureLayer)layer2Symbolize;
            IFeatureClass featureClass = pGeoFeatureL.FeatureClass;

            //找出rendererField在字段中的编号
            int lfieldNumber = featureClass.FindField(strRendererField);
            if (lfieldNumber == -1)
            {
                MessageBox.Show("Can't find field called " + strRendererField);
                return;
            }
            m_classBreaksRenderer = CreateClassBreaksRenderer(featureClass);
            if (m_classBreaksRenderer == null) return;
            pGeoFeatureL.Renderer = (IFeatureRenderer)m_classBreaksRenderer;
            m_activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, m_activeView.Extent);
        }

        private IClassBreaksRenderer CreateClassBreaksRenderer(IFeatureClass featureClass)
        {
            if (colorRamp == null)
            {
                MessageBox.Show("请先选择色带！！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            classify();
            int ClassesCount = gClassbreaks.GetUpperBound(0);
            if (ClassesCount == 0) return null;
            nudClassCount.Value = ClassesCount;

            IClassBreaksRenderer pClassBreaksRenderer = new ClassBreaksRendererClass();
            pClassBreaksRenderer.Field = strRendererField;
            if (strNormalizeField.ToLower() != "none")
                pClassBreaksRenderer.NormField = strNormalizeField;
            //设置着色对象的分级数目
            pClassBreaksRenderer.BreakCount = ClassesCount;
            pClassBreaksRenderer.SortClassesAscending = true;

            //通过色带设置各级分类符号的颜色
            colorRamp.Size = ClassesCount;
            bool createRamp;
            colorRamp.CreateRamp(out createRamp);
            IEnumColors enumColors = colorRamp.Colors;
            enumColors.Reset();
            IColor pColor = null;
            ISymbol symbol = null;
            //需要注意的是分级着色对象中的symbol和break的下标都是从0开始
            for (int i = 0; i < ClassesCount; i++)
            {
                pColor = enumColors.Next();
                switch (featureClass.ShapeType)
                {
                    case esriGeometryType.esriGeometryPoint:
                        ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbolClass();
                        simpleMarkerSymbol.Color = pColor;
                        symbol = simpleMarkerSymbol as ISymbol;
                        break;
                    case esriGeometryType.esriGeometryPolyline:
                        ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
                        simpleLineSymbol.Color = pColor;
                        symbol = simpleLineSymbol as ISymbol;
                        break;
                    case esriGeometryType.esriGeometryPolygon:
                        ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                        simpleFillSymbol.Color = pColor;
                        symbol = simpleFillSymbol as ISymbol;
                        break;
                    default:
                        break;
                }
                pClassBreaksRenderer.set_Symbol(i, symbol);
                pClassBreaksRenderer.set_Break(i, gClassbreaks[i + 1]);
            }
            return pClassBreaksRenderer;
        }

        private void btnSelectColorRamp_Click(object sender, EventArgs e)
        {
            //GetSymbolByControl colorRampForm = new GetSymbolByControl(esriSymbologyStyleClass.esriStyleClassColorRamps);
            //colorRampForm.ShowDialog();   //08.04.09
            //if (colorRampForm.m_styleGalleryItem != null)
            //    colorRamp = colorRampForm.m_styleGalleryItem.Item as IColorRamp;

            //colorRampForm.Dispose();


            ColorRampForm colorRampform = new ColorRampForm();
            colorRampform.ShowDialog();
            colorRamp = colorRampform.m_styleGalleryItem.Item as IColorRamp;
            colorRampform.Dispose();
 
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region "GetLayers"
        private IEnumLayer GetLayers()
        {
            UID uid = new UIDClass();
            uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";// IFeatureLayer
            //uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";  // IGeoFeatureLayer
            //uid.Value = "{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}";  // IDataLayer
            if (m_map.LayerCount != 0)
            {
                IEnumLayer layers = m_map.get_Layers(uid, true);
                return layers;
            }
            return null;
        }
        #endregion

        #region "GetFeatureLayer"
        private IFeatureLayer GetFeatureLayer(string layerName)
        {
            //get the layers from the maps
            if (GetLayers() == null) return null;
            IEnumLayer layers = GetLayers();
            layers.Reset();

            ILayer layer = null;
            while ((layer = layers.Next()) != null)
            {
                if (layer.Name == layerName)
                    return layer as IFeatureLayer;
            }
            return null;
        }

        #endregion

        private void cbxFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxFields.SelectedItem != null)
            {
                strRendererField = cbxFields.SelectedItem.ToString();
            }
        }

        private void cbxNormalization_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxNormalization.SelectedItem != null)
            {
                strNormalizeField = cbxNormalization.SelectedItem.ToString();
            }
        }

        private void nudClassCount_ValueChanged(object sender, EventArgs e)
        {
            gClassCount = Convert.ToInt32(nudClassCount.Value);
        }

        private void classifyCBX_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (classifyCBX.SelectedItem != null)
            {
                strClassifyMethod = classifyCBX.SelectedItem.ToString();
            }  
        }

        private void classify()
        {
            if (layer2Symbolize == null) return;
            IFeatureClass featureClass = layer2Symbolize.FeatureClass;
            ITable pTable = (ITable)featureClass;
            ITableHistogram pTableHistogram = new BasicTableHistogramClass();
            IBasicHistogram pHistogram = (IBasicHistogram)pTableHistogram;
            pTableHistogram.Field = strRendererField;
            if (strNormalizeField.ToLower() != "none")
                pTableHistogram.NormField = strNormalizeField;
            pTableHistogram.Table = pTable;
            object dataFrequency;
            object dataValues;
            pHistogram.GetHistogram(out dataValues, out dataFrequency);
            //下面是分级方法，用于根据获得的值计算得出符合要求的数据
            //根据条件计算出Classes和ClassesCount，numDesiredClasses为预定的分级数目
            IClassifyGEN pClassify = new NaturalBreaksClass();
            switch (strClassifyMethod)
            {
                case "等间隔分类":
                    pClassify = new EqualIntervalClass();
                    break;
                //case "预定义间隔分类":
                //    pClassify = new DefinedIntervalClass();
                //    break;
                case "分位数分类":
                    pClassify = new QuantileClass();
                    break;
                case "自然裂点分类":
                    pClassify = new NaturalBreaksClass();
                    break;
                //case "标准差分类":
                //    pClassify = new StandardDeviationClass();
                //    break;
                case "几何间隔分类":
                    pClassify = new GeometricalIntervalClass();
                    break;
                default:
                    break;
            }
            int numDesiredClasses = gClassCount;
            pClassify.Classify(dataValues, dataFrequency, ref numDesiredClasses);
            gClassbreaks = (double[])pClassify.ClassBreaks;

        }

    }
}
