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
using ESRI.ArcGIS.DisplayUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace GIS应用系统
{
    public partial class UniqueValuesSymbol : Form
    {
        IHookHelper m_hookHelper = null;
        IActiveView m_activeView = null;
        IMap m_map = null;

        IFeatureLayer layer2Symbolize = null;
        string strRendererField = string.Empty;

        IColorRamp colorRamp = null;
        ISymbol gloabalSymbol = null;  

        public UniqueValuesSymbol(IHookHelper hookHelper)
        {
            InitializeComponent();
            m_hookHelper = hookHelper;
            m_activeView = m_hookHelper.ActiveView;
            m_map = m_hookHelper.FocusMap;          
        }

        private void UniqueValuesSymbol_Load(object sender, EventArgs e)
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
                cbxFields.Text = strRendererField;
            }
        }

        private void CbxFieldAdditems(IFeatureLayer featureLayer)
        {
            IFields fields = featureLayer.FeatureClass.Fields;
            IField field = null;
            cbxFields.Items.Clear();
            for (int i = 0; i < fields.FieldCount; i++)
            {
                field = fields.get_Field(i);
                if (field.Type != esriFieldType.esriFieldTypeGeometry)
                    cbxFields.Items.Add(field.Name);
            }
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

        private int GetUniqueValuesCount(IFeatureClass featureClass, string strField)
        {
            ICursor cursor = (ICursor)featureClass.Search(null, false);
            IDataStatistics dataStatistics = new DataStatisticsClass();
            dataStatistics.Field = strField;
            dataStatistics.Cursor = cursor;
            System.Collections.IEnumerator enumerator = dataStatistics.UniqueValues;
            return dataStatistics.UniqueValueCount;
        }

        private System.Collections.IEnumerator GetUniqueValues(IFeatureClass featureClass, string strField)
        {
            ICursor cursor = (ICursor)featureClass.Search(null, false);
            IDataStatistics dataStatistics = new DataStatisticsClass();
            dataStatistics.Field = strField;
            dataStatistics.Cursor = cursor;

            System.Collections.IEnumerator enumerator = dataStatistics.UniqueValues;
            return enumerator;
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
            IUniqueValueRenderer pUniqueValueR = CreateRenderer(featureClass);
            if (pUniqueValueR == null) return;
            pGeoFeatureL.Renderer = (IFeatureRenderer)pUniqueValueR;
            m_activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, m_activeView.Extent);
        }

        private IEnumColors GetEnumColorsByRandomColorRamp(int colorSize)
        {
            IRandomColorRamp pColorRamp = new RandomColorRampClass();
            pColorRamp.StartHue = 0;  //0
            pColorRamp.EndHue = 360;     //360       
            pColorRamp.MinSaturation = 15;  //15
            pColorRamp.MaxSaturation = 30;   //30
            pColorRamp.MinValue = 99;    //99
            pColorRamp.MaxValue = 100;     //100          
            pColorRamp.Size = colorSize;
            bool ok = true;
            pColorRamp.CreateRamp(out ok);
            IEnumColors pEnumRamp = pColorRamp.Colors;
            pEnumRamp.Reset();
            return pEnumRamp;
        }

        private IEnumColors GetEnumColorsBySelectColorRamp(int colorSize)
        {
            if (colorRamp == null)
            {
                MessageBox.Show("请选择色带...", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            colorRamp.Size = colorSize;
            bool createRamp;
            colorRamp.CreateRamp(out createRamp);
            IEnumColors enumColors = colorRamp.Colors;
            enumColors.Reset();
            return enumColors;
        }

        private IUniqueValueRenderer CreateRenderer(IFeatureClass featureClass)
        {
            int uniqueValuesCount = GetUniqueValuesCount(featureClass, strRendererField);
            System.Collections.IEnumerator enumerator = GetUniqueValues(featureClass, strRendererField);

            if (uniqueValuesCount == 0) return null;

            IEnumColors pEnumRamp = GetEnumColorsByRandomColorRamp(uniqueValuesCount);
            //IEnumColors pEnumRamp = GetEnumColorsBySelectColorRamp(uniqueValuesCount);
            pEnumRamp.Reset();

            IUniqueValueRenderer pUniqueValueR = new UniqueValueRendererClass();
            //只用一个字段进行单值着色
            pUniqueValueR.FieldCount = 1;
            //用于区分着色的字段
            pUniqueValueR.set_Field(0, strRendererField);

            IColor pColor = null;
            ISymbol symbol = null;
            enumerator.Reset();
            while (enumerator.MoveNext())
            {
                object codeValue = enumerator.Current;
                pColor = pEnumRamp.Next();

                switch (featureClass.ShapeType)
                {
                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                        ISimpleMarkerSymbol markerSymbol = new SimpleMarkerSymbolClass() as ISimpleMarkerSymbol;
                        markerSymbol.Color = pColor;
                        symbol = markerSymbol as ISymbol;
                        break;
                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                        ISimpleLineSymbol lineSymbol = new SimpleLineSymbolClass() as ISimpleLineSymbol;
                        lineSymbol.Color = pColor;
                        symbol = lineSymbol as ISymbol;
                        break;
                    case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                        ISimpleFillSymbol fillSymbol = new SimpleFillSymbolClass() as ISimpleFillSymbol;
                        fillSymbol.Color = pColor;
                        symbol = fillSymbol as ISymbol;
                        break;
                    default:
                        break;
                }

                //将每次得到的要素字段值和修饰它的符号放入着色对象中
                pUniqueValueR.AddValue(codeValue.ToString(), strRendererField, symbol);

            }
            return pUniqueValueR;
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
                strRendererField = cbxFields.SelectedItem.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            gloabalSymbol = GetSymbolBySymbolSelector(layer2Symbolize.FeatureClass.ShapeType);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ColorRampForm colorRampForm = new ColorRampForm();
            colorRampForm.ShowDialog();
            colorRamp = colorRampForm.m_styleGalleryItem.Item as IColorRamp;
            colorRampForm.Dispose();
        }

        private ISymbol GetSymbolBySymbolSelector(esriGeometryType geometryType)
        {
            ISymbolSelector pSymbolSelector = new SymbolSelectorClass();
            ISymbol symbol = null;
            switch (geometryType)
            {
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                    symbol = new SimpleMarkerSymbolClass();
                    break;
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                    symbol = new SimpleLineSymbolClass();
                    break;
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                    symbol = new SimpleFillSymbolClass();
                    break;
                default:
                    break;
            }
            pSymbolSelector.AddSymbol(symbol);
            bool response = pSymbolSelector.SelectSymbol(0);
            if (response)
            {
                symbol = pSymbolSelector.GetSymbolAt(0);
                return symbol;
            }
            return null;
        }
    }
}
