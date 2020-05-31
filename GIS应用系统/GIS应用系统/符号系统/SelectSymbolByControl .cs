using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Display;
using System.IO;
using ESRI.ArcGIS.Controls;

namespace GIS应用系统
{
    public partial class SelectSymbolByControl : Form
    {
        public IStyleGalleryItem m_styleGalleryItem = null;
        string stylesPath = string.Empty;

        string styleClass = "Fill Symbols";

        public SelectSymbolByControl(string strStyleClass)
        {
            InitializeComponent();
            styleClass = strStyleClass;
        }


        private void SelectSymbolByControl_Load(object sender, EventArgs e)
        {
            //Get the ArcGIS install location
            string sInstall = ESRI.ArcGIS.RuntimeManager.ActiveRuntime.Path;
            string defaultStyle = System.IO.Path.Combine(sInstall, "Styles\\ESRI.ServerStyle");
            if (System.IO.File.Exists(defaultStyle))
            {
                //Load the ESRI.ServerStyle file into the SymbologyControl
                axSymbologyControl1.LoadStyleFile(defaultStyle);
                //Set the style class
                SetStyleClass();
                //Select the color ramp item
                axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass).SelectItem(0);
                cbxStyles.Text = defaultStyle;
            }
            stylesPath = sInstall + "\\Styles";
            cbxStyles.Items.Clear();
            cbxStylesAddItems(stylesPath);
        }

        private void SetStyleClass()
        {
            switch (styleClass)
            {
                case "Reference Systems":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassReferenceSystems;
                    break;
                case "Maplex Labels":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassMaplexLabels;
                    break;
                case "Shadows":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassShadows;
                    break;
                case "Area Patches":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassAreaPatches;
                    break;
                case "Line Patches":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassLinePatches;
                    break;
                case "Labels":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassLabels;
                    break;
                case "North Arrows":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassNorthArrows;
                    break;
                case "Scale Bars":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassScaleBars;
                    break;
                case "Legend Items":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassLegendItems;
                    break;
                case "Scale Texts":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassScaleTexts;
                    break;
                case "Color Ramps":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassColorRamps;
                    break;
                case "Borders":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassBorders;
                    break;
                case "Backgrounds":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassBackgrounds;
                    break;
                case "Colors":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassColors;
                    break;
                case "Vectorization Settings":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassVectorizationSettings;
                    break;
                case "Fill Symbols":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassFillSymbols;
                    break;
                case "Line Symbols":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassLineSymbols;
                    break;
                case "Marker Symbols":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassMarkerSymbols;
                    break;
                case "Text Symbols":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassTextSymbols;
                    break;
                case "Hatches":
                    axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassHatches;
                    break;

                default:
                    break;
            }

            if (axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass).get_ItemCount(string.Empty) != 0)
                axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass).SelectItem(0);
            axSymbologyControl1.Update();
            axSymbologyControl1.Refresh();
        }

        private void cbxStylesAddItems(string path)
        {
            string[] serverstyleFiles = System.IO.Directory.GetFiles(stylesPath, "*.serverstyle", SearchOption.AllDirectories);
            string[] styleFiles = System.IO.Directory.GetFiles(stylesPath, "*.style", SearchOption.AllDirectories);

            cbxStylesAddItems(serverstyleFiles);
            cbxStylesAddItems(styleFiles);
        }

        private void cbxStylesAddItems(string[] files)
        {
            if (files.GetLength(0) == 0) return;
            foreach (string file in files)
            {
                cbxStyles.Items.Add(file);
            }
        }

        private void btnOtherStyles_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                stylesPath = folderBrowserDialog1.SelectedPath;
                cbxStylesAddItems(stylesPath);
            }
        }

        private void cbxStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxStyles.SelectedItem == null) return;
            axSymbologyControl1.Clear();
            stylesPath = cbxStyles.SelectedItem.ToString();
            string ext = System.IO.Path.GetExtension(stylesPath).ToLower();
            if (ext == ".serverstyle")
                axSymbologyControl1.LoadStyleFile(stylesPath);
            if (ext == ".style")
                axSymbologyControl1.LoadDesktopStyleFile(stylesPath);
            SetStyleClass();
            if (axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass).get_ItemCount(string.Empty) != 0)
                axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass).SelectItem(0);
        }

        private void axSymbologyControl1_OnItemSelected(object sender, ESRI.ArcGIS.Controls.ISymbologyControlEvents_OnItemSelectedEvent e)
        {
            m_styleGalleryItem = (IStyleGalleryItem)e.styleGalleryItem;
        }

        public bool IsInteger(string s)
        {
            try
            {
                Int32.Parse(s);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
