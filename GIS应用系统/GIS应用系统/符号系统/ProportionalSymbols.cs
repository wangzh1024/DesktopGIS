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
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DisplayUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GIS应用系统
{
    public partial class ProportionalSymbols : Form
    {
        IHookHelper m_hookHelper = null;
        IActiveView m_activeView = null;
        IMap m_map = null;

        IFeatureLayer layer2Symbolize = null;
        string strRendererField = string.Empty;
        string strNormalizeField = string.Empty;

    //    IStyleGallery pStyleGlry = new StyleGallery();
        IMarkerSymbol markerSymbol = null;
        IFillSymbol fillSymbol = null;

     //   IColor gColor = null;

        double minSize = 12;
        int legendCount = 5;

        public ProportionalSymbols(IHookHelper hookHelper)
        {
            InitializeComponent();
            m_hookHelper = hookHelper;
            m_activeView = m_hookHelper.ActiveView;
            m_map = m_hookHelper.FocusMap;
        }

        private void ProportionalSymbols_Load(object sender, EventArgs e)
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

        private IProportionalSymbolRenderer CreateRenderer()
        {
            IGeoFeatureLayer pGeoFeatureLayer = (IGeoFeatureLayer)layer2Symbolize;
            ITable pTable = (ITable)pGeoFeatureLayer;
            ICursor pCursor = pTable.Search(null, false);
            //Use the statistics objects to calculate the max value and the min value
            IDataStatistics pDataStatistics = new DataStatisticsClass();
            pDataStatistics.Cursor = pCursor;
            //Set statistical field
            pDataStatistics.Field = strRendererField;
            //Get the result of statistics
            IStatisticsResults pStatisticsResult = pDataStatistics.Statistics;
            if (pStatisticsResult == null) return null;
            if (markerSymbol == null)
            {
                MessageBox.Show("请先选择点符号...", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            if (fillSymbol == null)
            {
                MessageBox.Show("请先选择背景...", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            markerSymbol.Size = minSize;
            // Create a new proportional symbol renderer to draw pop1990
            IProportionalSymbolRenderer pProportionalSymbolR = new ProportionalSymbolRendererClass();   
            pProportionalSymbolR.Field = strRendererField;
            if (strNormalizeField.ToLower() != "none")
                pProportionalSymbolR.NormField = strNormalizeField;
            pProportionalSymbolR.MinDataValue = pStatisticsResult.Minimum;
            pProportionalSymbolR.MaxDataValue = pStatisticsResult.Maximum;
            pProportionalSymbolR.BackgroundSymbol = fillSymbol;
            pProportionalSymbolR.MinSymbol = (ISymbol)markerSymbol;
            pProportionalSymbolR.LegendSymbolCount = legendCount;
            pProportionalSymbolR.CreateLegendSymbols();

            return pProportionalSymbolR;
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
            IProportionalSymbolRenderer renderer = CreateRenderer();
            if (renderer == null) return;
            pGeoFeatureL.Renderer = (IFeatureRenderer)renderer;
            m_activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, m_activeView.Extent);
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

        private void btnSelectBackColor_Click(object sender, EventArgs e)
        {
            GetSymbolByControl fillSymbolForm = new GetSymbolByControl(esriSymbologyStyleClass.esriStyleClassFillSymbols);
            fillSymbolForm.ShowDialog();
            if (fillSymbolForm.m_styleGalleryItem == null) return;
            fillSymbol = fillSymbolForm.m_styleGalleryItem.Item as IFillSymbol;
            fillSymbolForm.Dispose();
        }

        private void btnSelectSymbol_Click(object sender, EventArgs e)
        {
            GetSymbolByControl markerSymbolForm = new GetSymbolByControl(esriSymbologyStyleClass.esriStyleClassMarkerSymbols);
            markerSymbolForm.ShowDialog();
            if (markerSymbolForm.m_styleGalleryItem == null) return;
            markerSymbol = markerSymbolForm.m_styleGalleryItem.Item as IMarkerSymbol;
            markerSymbolForm.Dispose();
        }

        private ISymbol GetSymbolBySymbolSelector(esriGeometryType geometryType)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                return symbol;

            }
        }
        

        private void nudLegendCount_ValueChanged(object sender, EventArgs e)
        {
            legendCount = Convert.ToInt32(nudLegendCount.Value);
        }

        private void nudMinsize_ValueChanged(object sender, EventArgs e)
        {
            minSize = Convert.ToDouble(nudMinsize.Value);
        }           

    }
}
