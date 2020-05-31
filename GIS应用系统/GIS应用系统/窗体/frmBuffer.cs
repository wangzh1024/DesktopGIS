using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;

namespace GIS应用系统
{
    public partial class frmBuffer : Form
    {
        public frmBuffer()
        {
            InitializeComponent();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmBuffer_Load(object sender, EventArgs e)
        {
            AddAllLayerstoComboBox(ComboBoxLayer);
            //默认选择第一个图层
            if (ComboBoxLayer.Items.Count>0)
            {
                ComboBoxLayer.SelectedIndex = 0;
            }
            //设置输出路径的默认值
            string tempDir = System.IO.Path.GetTempPath();
            this.txtOutputPath.Text = System.IO.Path.Combine(tempDir, (ComboBoxLayer.SelectedText+ "_buffer.shp"));

        }

        private void AddAllLayerstoComboBox(ComboBox combox)
        {
            try
            {
                combox.Items.Clear();
                int pLayerCount = frmMain.m_mapControl.LayerCount;
                if (pLayerCount > 0)
                {
                    for (int i = 0; i <= pLayerCount - 1; i++)
                    {
                    
                        if (frmMain.m_mapControl.get_Layer(i) is IFeatureLayer)  //只添加矢量图层，栅格图层没有属性表
                            combox.Items.Add(frmMain.m_mapControl.get_Layer(i).Name);
                   
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void btnOutputLayer_Click(object sender, EventArgs e)
        {
            //设置输出图层的路径
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.CheckPathExists = true;
            saveDlg.Filter = "Shapefile (*.shp)|*.shp";
            saveDlg.OverwritePrompt = true;
            saveDlg.Title = "输出图层";
            saveDlg.RestoreDirectory = true;
            //saveDlg.FileName = ComboBoxLayer.SelectedText+ "_buffer.shp";

            DialogResult dr = saveDlg.ShowDialog();
            if (dr == DialogResult.OK && saveDlg.FileName !="")
                this.txtOutputPath.Text = saveDlg.FileName;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            //检验参数是否正确设置
            double bufferDistance;
            double.TryParse(txtBufferDistance.Text, out bufferDistance);
            if (0.0 == bufferDistance)
            {
                MessageBox.Show("距离设置错误!");
                return;
            }

            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(txtOutputPath.Text)) || ".shp" != System.IO.Path.GetExtension(txtOutputPath.Text))
            {
                MessageBox.Show("输出格式错误!");
                return;
            }

            if (ComboBoxLayer.Items.Count<=0)
            {
                return;
            }

            IFeatureLayer pFeatureLayer =(IFeatureLayer) GetLayerByName(ComboBoxLayer.SelectedItem.ToString());

            Geoprocessor gp = new Geoprocessor();
            gp.OverwriteOutput = true;
            gp.AddOutputsToMap = true;            

            string unit = "Kilometers";
            ESRI.ArcGIS.AnalysisTools.Buffer buffer = new ESRI.ArcGIS.AnalysisTools.Buffer(pFeatureLayer, txtOutputPath.Text, Convert.ToString(bufferDistance) + " " + unit);
            IGeoProcessorResult results = (IGeoProcessorResult)gp.Execute(buffer, null);

            //添加缓冲区图层到当前地图中
            string fileDirectory = txtOutputPath.Text.ToString().Substring(0, txtOutputPath.Text.LastIndexOf("\\"));
            int j;
            j = txtOutputPath.Text.LastIndexOf("\\");
            string tmpstr = txtOutputPath.Text.ToString().Substring(j + 1);
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
            IWorkspace pWS = pWorkspaceFactory.OpenFromFile(fileDirectory, 0);
            IFeatureWorkspace pFS = pWS as IFeatureWorkspace;
            //IFeatureClass pfc = pFS.OpenFeatureClass(this.ComboBoxLayer.SelectedText+ "_buffer.shp");
            IFeatureClass pfc = pFS.OpenFeatureClass(tmpstr);
            IFeatureLayer pfl = new FeatureLayerClass();
            pfl.FeatureClass = pfc;
            pfl.Name = pfc.AliasName;

            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 255;
            //产生一个线符号对象
            ILineSymbol pOutline = new SimpleLineSymbolClass();
            pOutline.Width = 2;
            pOutline.Color = pColor;
            //设置颜色属性
            pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 100;
            //设置填充符号的属性
            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbol();
            pFillSymbol.Color = pColor;
            pFillSymbol.Outline = pOutline;
            pFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;

            ISimpleRenderer pRen;
            IGeoFeatureLayer pGeoFeatLyr = pfl as IGeoFeatureLayer;
            pRen = pGeoFeatLyr.Renderer as ISimpleRenderer;
            pRen.Symbol = pFillSymbol as ISymbol;
            pGeoFeatLyr.Renderer = pRen as IFeatureRenderer;

            ILayerEffects pLayerEffects = pfl as ILayerEffects;
            pLayerEffects.Transparency = 150;

            frmMain.m_mapControl.AddLayer((ILayer)pfl, 0);
            MessageBox.Show(ComboBoxLayer.SelectedText + "缓存生成成功！");
        }

        //private IFeatureLayer GetFeatureLayer(string layerName)
        //{
        //        ILayer pFeatureLayer = null;
        //        int pLayerCount = frmMain.m_mapControl.LayerCount;
        //        for (int i = 0; i <= pLayerCount - 1; i++)
        //        {
        //            if (frmMain.m_mapControl.get_Layer(i).Name == layerName)
        //            {
        //                pFeatureLayer = frmMain.m_mapControl.get_Layer(i);
        //                return pFeatureLayer as IFeatureLayer;
        //                break;
        //            }
        //        }
        //}

        private ILayer GetLayerByName(string strLayerName)
        {
            ILayer pLayer = null;
            for (int i = 0; i <= frmMain.m_mapControl.LayerCount - 1; i++)
            {
                if (strLayerName == frmMain.m_mapControl.get_Layer(i).Name)
                { pLayer = frmMain.m_mapControl.get_Layer(i); break; }
            }
            return pLayer;
        }
    }
}
