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
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DisplayUI;

namespace GIS应用系统
{
    public partial class frmSingleSymbol : Form
    {
        IHookHelper m_hookHelper = null;
        IActiveView m_activeView = null;
        IMap m_map = null;

        IFeatureLayer layer2Symbolize = null;

        public frmSingleSymbol(IHookHelper hookHelper)
        {
            InitializeComponent();
            m_hookHelper = hookHelper;
            m_activeView = m_hookHelper.ActiveView;
            m_map = m_hookHelper.FocusMap;       
        }

        private void frmSingleSymbol_Load(object sender, EventArgs e)
        {
            CbxLayersAddItems();
        }

        private void cbxLayers2Symbolize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLayers2Symbolize.SelectedItem != null)
            {
                string strLayer2Symbolize = cbxLayers2Symbolize.SelectedItem.ToString();
                layer2Symbolize = GetFeatureLayer(strLayer2Symbolize);
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

        private void btnSymbolize_Click(object sender, EventArgs e)
        {
            if (layer2Symbolize == null) return;
            ISymbol symbol = GetSymbolByControl(layer2Symbolize.FeatureClass.ShapeType);
            if (symbol == null) return;
            ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();
            pSimpleRenderer.Symbol = symbol;
            pSimpleRenderer.Label = "单一符号化";
            IGeoFeatureLayer pGeoFeatureL = layer2Symbolize as IGeoFeatureLayer;
            pGeoFeatureL.Renderer = pSimpleRenderer as IFeatureRenderer;
            m_activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, m_activeView.Extent);
        }

        private ISymbol GetSymbolByControl(esriGeometryType geometryType)
        {
            GetSymbolByControl symbolForm = null;
            ISymbol symbol = null;
            switch (geometryType)
            {
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                    symbolForm = new GetSymbolByControl(esriSymbologyStyleClass.esriStyleClassMarkerSymbols);
                    break;
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                    symbolForm = new GetSymbolByControl(esriSymbologyStyleClass.esriStyleClassLineSymbols);
                    break;
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                    symbolForm = new GetSymbolByControl(esriSymbologyStyleClass.esriStyleClassFillSymbols);
                    break;
                default:
                    break;
            }
            symbolForm.ShowDialog();
            if (symbolForm.m_styleGalleryItem != null)
                symbol = symbolForm.m_styleGalleryItem.Item as ISymbol;
            symbolForm.Dispose();
            return symbol;
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
