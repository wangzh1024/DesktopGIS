using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using System.IO;

namespace GIS应用系统
{
    public partial class ColorRampForm : Form
    {
        public IStyleGalleryItem m_styleGalleryItem = null;
        string stylesPath = string.Empty;

        public ColorRampForm()
        {
            InitializeComponent();
        }

        private void ColorRampForm_Load(object sender, EventArgs e)
        {
            //Get the ArcGIS install location
            string sInstall = ESRI.ArcGIS.RuntimeManager.ActiveRuntime.Path;
            //Load the ESRI.ServerStyle file into the SymbologyControl
            axSymbologyControl1.LoadStyleFile(sInstall + "\\Styles\\ESRI.ServerStyle");
            //Set the style class
            axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassColorRamps;
            //Select the color ramp item
            axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass).SelectItem(0);
            stylesPath = sInstall + "\\Styles";
            cbxStyles.Items.Clear();
            cbxStylesAddItems(stylesPath);
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Hide();
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
            if (cbxStyles.SelectedItem != null)
            {
                axSymbologyControl1.Clear();
                stylesPath = cbxStyles.SelectedItem.ToString();
                string ext = System.IO.Path.GetExtension(stylesPath).ToLower();
                //Load the ESRI.ServerStyle file into the SymbologyControl
                if (ext == ".serverstyle")
                    axSymbologyControl1.LoadStyleFile(stylesPath);
                if (ext == ".style")
                    axSymbologyControl1.LoadDesktopStyleFile(stylesPath);
                //Set the style class
                axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassColorRamps;
                //Select the color ramp item
                if (axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass).get_ItemCount(string.Empty) != 0)
                    axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass).SelectItem(0);
                axSymbologyControl1.Update();
            }
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
