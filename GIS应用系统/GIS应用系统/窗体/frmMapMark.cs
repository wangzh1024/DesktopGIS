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
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GIS应用系统
{
    public partial class frmMapMark : Form
    {
        IMapControl3 mapcontrol;
        IMap pMap;
        IRgbColor pRGB = new RgbColorClass();

        public frmMapMark(IMapControl3 m_mapControl)
        {
            InitializeComponent();
            mapcontrol = m_mapControl;            
        }

        private void frmMapMark_Load(object sender, EventArgs e)
        {
            addlayer();
            if (cbxLayer.Items.Count != 0)
            {
                cbxLayer.SelectedIndex = 0;
                addfield();
            }
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            pMap = mapcontrol.Map;
        }

        private IFeatureLayer getlayerbyname(string layername)
        {
            ILayer layer = null;
            IFeatureLayer flayer = null;
            for (int i = 0; i <= mapcontrol.LayerCount - 1; i++)
            {
                layer = mapcontrol.get_Layer(i);
                if (layername == mapcontrol.get_Layer(i).Name.ToString() && layer is IFeatureLayer)
                { flayer = layer as IFeatureLayer; break; }
            }
            return flayer;

        }
        private void addlayer()
        {
            try
            {
                cbxLayer.Items.Clear();
                if (mapcontrol.LayerCount > 0)
                {
                    cbxLayer.Enabled = true;
                    for (int i = 0; i <= mapcontrol.LayerCount - 1; i++)
                    { cbxLayer.Items.Add(mapcontrol.get_Layer(i).Name); }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void addfield()
        {
            cbxField.Items.Clear();
            string layername = cbxLayer.SelectedItem.ToString();
            IFeatureLayer pflayer = getlayerbyname(layername);
            for (int i = 0; i <= pflayer.FeatureClass.Fields.FieldCount - 1; i++)
            {
               cbxField.Items.Add(pflayer.FeatureClass.Fields.get_Field(i).Name.ToString());
            }
        }

        private void cbxLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxField.Items.Clear();
            addfield();
        }

        private void cbxField_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fieldname = cbxField.SelectedItem.ToString();
        }
        
        private void btnA_Click(object sender, EventArgs e)
        {
            pMap = mapcontrol.Map;
            if (cbxLayer.SelectedItem == null)
            {
                MessageBox.Show("请选择要标注的图层！");
                return;
            }
            if (cbxField.SelectedItem == null)
            {
                MessageBox.Show("请选择要标注的字段！");
                return;
            }

            if (radioButton1.Checked)
            {
                LayerLabel();
            }
            if (radioButton2.Checked)
            {
                Annotation();
            }
            this.Dispose();
        }

        private void Annotation()
        {
            string fieldname = cbxField.SelectedItem.ToString();
            IFeatureLayer featureLayer = getlayerbyname(cbxLayer.SelectedItem.ToString());
            IGeoFeatureLayer pGeoFeatLyr = featureLayer as IGeoFeatureLayer;
            stdole.IFontDisp pFont = new stdole.StdFontClass() as stdole.IFontDisp;
            pFont.Name = comboBox2.SelectedItem.ToString();
            ITextSymbol pTextSymbol = new TextSymbolClass();
            pTextSymbol.Size = Convert.ToInt16(comboBox3.SelectedItem.ToString());
            pTextSymbol.Font = pFont;
            if (pRGB.NullColor)
            {
                pRGB.Red = 255;
                pRGB.Blue = 0;
                pRGB.Green = 0;
            }
            pTextSymbol.Color = pRGB;
            IAnnotateLayerPropertiesCollection pAnnoProps = pGeoFeatLyr.AnnotationProperties;
            pAnnoProps.Clear();                
            ILabelEngineLayerProperties pLabelEngine = new LabelEngineLayerPropertiesClass();
            pLabelEngine.Symbol = pTextSymbol;
            pLabelEngine.Expression = "[" + fieldname + "]";             
            IAnnotateLayerProperties  pAnnoLayerProps = pLabelEngine as IAnnotateLayerProperties;
            pAnnoLayerProps.DisplayAnnotation = true;
            pAnnoLayerProps.LabelWhichFeatures = esriLabelWhichFeatures.esriAllFeatures; 
            pAnnoProps.Add(pAnnoLayerProps);
            pGeoFeatLyr.DisplayField = fieldname;
            pGeoFeatLyr.DisplayAnnotation = true;
            IActiveView pActiveView = pMap as IActiveView;
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void LayerLabel()
        {
            IFeatureLayer featureLayer = getlayerbyname(cbxLayer.SelectedItem.ToString());
            IGeoFeatureLayer geoFeatureLayer = featureLayer as IGeoFeatureLayer;
            IFeatureClass featureclass = featureLayer.FeatureClass;
            IFields pFields = featureclass.Fields;
            int i = pFields.FindField(cbxField.SelectedItem.ToString());
            IField field = pFields.get_Field(i);
            geoFeatureLayer.DisplayField = field.Name;
            geoFeatureLayer.DisplayAnnotation = true;

            IActiveView pActiveView = pMap as IActiveView;
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void textelementlable()
        {
            IFeatureLayer pfl = getlayerbyname(cbxLayer.SelectedItem.ToString());
            IFeatureClass featureclass = pfl.FeatureClass;
            IFeatureCursor pFeatureCursor;
            pFeatureCursor = featureclass.Search(null, true);
            IFeature pFeat = pFeatureCursor.NextFeature();
            while (pFeat != null)
            {
                IFields pFields = pFeat.Fields;
                int i = pFields.FindField(cbxField.SelectedItem.ToString()); ;
                IEnvelope pEnv = pFeat.Extent;
                IPoint pPoint = new PointClass();
                pPoint.PutCoords(pEnv.XMin + pEnv.Width / 2, pEnv.YMin + pEnv.Height / 2);
                stdole.IFontDisp pFont = new stdole.StdFontClass() as stdole.IFontDisp;
                pFont.Name = comboBox2.SelectedItem.ToString();
                ITextSymbol pTextSymbol = new TextSymbolClass();

                pTextSymbol.Size = Convert.ToInt16(comboBox3.SelectedItem.ToString());
                pTextSymbol.Font = pFont;
                if (pRGB.NullColor)
                {
                    pRGB.Red = 110;
                    pRGB.Blue = 200;
                    pRGB.Green = 60;
                }
                pTextSymbol.Color = pRGB;

                ITextElement pTextEle = new TextElementClass();
                pTextEle.Text = pFeat.get_Value(i).ToString();
                pTextEle.ScaleText = true;
                pTextEle.Symbol = pTextSymbol;
                IElement pEle = pTextEle as IElement;
                pEle.Geometry = pPoint;
                IActiveView pActiveView = pMap as IActiveView;
                IGraphicsContainer pGraphicsContainer = pMap as IGraphicsContainer;
                pGraphicsContainer.AddElement(pEle, 0);
                pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                pPoint = null;
                pEle = null;
                pFeat = pFeatureCursor.NextFeature();
            }
        }

        private void btnSelectColor_Click(object sender, EventArgs e)
        {
            
            pRGB = new RgbColorClass();
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pRGB.Blue = colorDialog1.Color.B;
                pRGB.Green = colorDialog1.Color.G;
                pRGB.Red = colorDialog1.Color.R;
            }
        }   
    }
}
