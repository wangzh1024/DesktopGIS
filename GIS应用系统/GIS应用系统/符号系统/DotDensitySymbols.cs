using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DisplayUI;
using stdole;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Framework;
using System.Runtime.InteropServices;

namespace GIS应用系统
{
    public partial class DotDensitySymbols : Form
    {
        IHookHelper m_hookHelper = null;
        IActiveView m_activeView = null;
        IMap m_map = null;

        IFeatureLayer layer2Symbolize = null;

        IMarkerSymbol markerSymbol = null;
        IColor gColor = null;
        double dotSize = 2;
        int dotValue = 2000;

        System.Collections.Hashtable fieldSymbolHashTable = new System.Collections.Hashtable();

        public DotDensitySymbols(IHookHelper hookHelper)
        {
            InitializeComponent();
            m_hookHelper = hookHelper;
            m_activeView = m_hookHelper.ActiveView;
            m_map = m_hookHelper.FocusMap;
        }

        private void DotDensitySymbols_Load(object sender, EventArgs e)
        {
            CbxLayersAddItems();
            lvRendererFields.View = View.List;
        }

        private void cbxLayers2Symbolize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLayers2Symbolize.SelectedItem != null)
            {
                string strLayer2Symbolize = cbxLayers2Symbolize.SelectedItem.ToString();
                layer2Symbolize = GetFeatureLayer(strLayer2Symbolize);
                lstSourceFieldsAdditems(layer2Symbolize);
                lvRendererFields.Items.Clear();
                fieldSymbolHashTable.Clear();
            }
        }

        private void lstSourceFieldsAdditems(IFeatureLayer featureLayer)
        {
            IFields fields = featureLayer.FeatureClass.Fields;
            lstSourceFields.Items.Clear();

            for (int i = 0; i < fields.FieldCount; i++)
            {
                if ((fields.get_Field(i).Type == esriFieldType.esriFieldTypeDouble) ||
                    (fields.get_Field(i).Type == esriFieldType.esriFieldTypeInteger) ||
                    (fields.get_Field(i).Type == esriFieldType.esriFieldTypeSingle) ||
                    (fields.get_Field(i).Type == esriFieldType.esriFieldTypeSmallInteger))
                {
                    lstSourceFields.Items.Add(fields.get_Field(i).Name);
                }
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
                    IFeatureLayer featureLayer = layer as IFeatureLayer;
                    if (featureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
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

            IDotDensityRenderer renderer = CreateRenderer();
            if (renderer == null) return;
            pGeoFeatureL.Renderer = (IFeatureRenderer)renderer;
            m_activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, m_activeView.Extent);
        }

        private IDotDensityRenderer CreateRenderer()
        {
            IDotDensityRenderer pDotDensityRenderer = new DotDensityRendererClass();
            IRendererFields pRendererFields = (IRendererFields)pDotDensityRenderer;
            IDotDensityFillSymbol pDotDensityFillS = new DotDensityFillSymbolClass();
            //pDotDensityFillS.DotSize = 2 * dotSize;
            //pDotDensityFillS.Color = getRGB(0, 0, 0);
            if (gColor != null)
                pDotDensityFillS.BackgroundColor = gColor;
            ISymbolArray pSymbolArray = (ISymbolArray)pDotDensityFillS;
            string strField = string.Empty;

            foreach (System.Collections.DictionaryEntry de in fieldSymbolHashTable)
            {
                strField = de.Key.ToString(); ;
                ISymbol symbol = de.Value as ISymbol;
                pRendererFields.AddField(strField, strField);
                IMarkerSymbol pMarkerSymbol = symbol as IMarkerSymbol;
                pMarkerSymbol.Size = dotSize;
                pSymbolArray.AddSymbol(symbol);
            }
            pDotDensityRenderer.DotDensitySymbol = pDotDensityFillS;
            pDotDensityRenderer.DotValue = dotValue;
            pDotDensityRenderer.CreateLegend();

            return pDotDensityRenderer;
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

        private void btnSelectBackColor_Click(object sender, EventArgs e)
        {
            gColor = GetColorByColorPalette(btnSelectBackColor.Right, btnSelectBackColor.Bottom);
        }

        public IRgbColor getRGB(int yourRed, int yourGreen, int yourBlue)
        {
            IRgbColor pRGB;
            pRGB = new RgbColorClass();
            pRGB.Red = yourRed;
            pRGB.Green = yourGreen;
            pRGB.Blue = yourBlue;
            pRGB.UseWindowsDithering = true;
            return pRGB;

        }

        private IColor GetColorByColorBrowser()
        {
            IColor pNewColor;

            IColor pInitColor = new RgbColorClass();
            pInitColor.RGB = 255;
            IColorBrowser pColorBrowser = new ColorBrowserClass();
            pColorBrowser.Color = pInitColor;
            bool bColorSet = pColorBrowser.DoModal(0);
            if (bColorSet)
            {
                pNewColor = pColorBrowser.Color;
                return pNewColor;
            }
            else return pInitColor;
        }

        private IColor GetColorByColorSelector()
        {
            //Set the initial color to be diaplyed when the dialog opens
            IColor pColor;
            pColor = new RgbColorClass();
            pColor.RGB = 255;

            IColorSelector pSelector;
            pSelector = new ColorSelectorClass();
            pSelector.Color = pColor;

            // Display the dialog
            if (pSelector.DoModal(0))
            {
                IColor pOutColor;
                pOutColor = pSelector.Color;
                return pOutColor;
            }
            else return pColor;
        }

        private IColor GetColorByColorPalette(int left, int top)
        {
            IColor pColor;

            pColor = new RgbColorClass();
            pColor.RGB = 255;

            try
            {
                IColorPalette pPalette;
                pPalette = new ColorPaletteClass();

                tagRECT pRect = new tagRECT();
                pRect.left = left;
                pRect.top = top;

                pPalette.TrackPopupMenu(ref pRect, pColor, false, 0);
                pColor = pPalette.Color;
                return pColor;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return pColor;
            }
        }

        private void btnSelectSymbol_Click(object sender, EventArgs e)
        {
            ISymbol symbol = GetSymbolBySymbolSelector(esriGeometryType.esriGeometryPoint);
            if (symbol != null)
                markerSymbol = symbol as IMarkerSymbol;
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

        private void nudDotSize_ValueChanged(object sender, EventArgs e)
        {
            dotSize = Convert.ToDouble(nudDotSize.Value);
        }

        private void txtDotValue_TextChanged(object sender, EventArgs e)
        {
            if (IsInteger(txtDotValue.Text))
                dotValue = Convert.ToInt32(txtDotValue.Text);
        }


        private bool IsInteger(string s)
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

        private void lstSourceFields_DoubleClick(object sender, EventArgs e)
        {
            System.Object selectedItem = lstSourceFields.SelectedItem;
            if (selectedItem != null)
            {
                lstSourceFields.Items.Remove(selectedItem);
                lvRendererFieldsAddItemWithSymbol(selectedItem);
            }
        }

        private void lvRendererFieldsAddItemWithSymbol(System.Object selectedItem)
        {
            IStyleGalleryItem styleItem = GetSymbolBySymbologyControl("Marker Symbols");
            if (styleItem == null) return;

            ISymbol symbol = styleItem.Item as ISymbol;
            if (symbol == null) return;

            //symbolPBox.Visible = true;
            IStyleGalleryClass mStyleClass = new MarkerSymbolStyleGalleryClassClass();
            Bitmap image = StyleGalleryItemToBmp(24, 24, mStyleClass, styleItem);
            //Bitmap image = DrawToPictureBox(symbol, symbolPBox);
            int currentIdx = Largeimage.Images.Count;
            currentIdx = Smallimage.Images.Count;
            Largeimage.Images.Add(image);
            Smallimage.Images.Add(image);
            ListViewItem newItem = new ListViewItem();
            newItem.ImageIndex = currentIdx;
            newItem.Text = selectedItem.ToString();
            lvRendererFields.Items.Add(newItem);
            lvRendererFields.Refresh();
            if (fieldSymbolHashTable.ContainsKey(selectedItem.ToString()))
            {
                fieldSymbolHashTable.Remove(selectedItem.ToString());
            }
            fieldSymbolHashTable.Add(selectedItem.ToString(), symbol);
            //symbolPBox.Visible = false;
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            System.Object selectedItem = lstSourceFields.SelectedItem;
            if (selectedItem != null)
            {
                lstSourceFields.Items.Remove(selectedItem);
                lvRendererFieldsAddItemWithSymbol(selectedItem);
            }
        }

        private void btnOut_Click(object sender, EventArgs e)
        {
            ListViewItem selectedItem = lvRendererFields.FocusedItem;
            if (selectedItem != null)
            {
                lvRendererFields.Items.Remove(selectedItem);
                lstSourceFields.Items.Add(selectedItem.Text);
                fieldSymbolHashTable.Remove(selectedItem.Text);
            }
        }

        private void btnAllOut_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvRendererFields.Items)
            {
                lstSourceFields.Items.Add(item.Text);
            }
            lvRendererFields.Items.Clear();
            fieldSymbolHashTable.Clear();
        }

        private IStyleGalleryItem GetSymbolBySymbologyControl(string styleClass)
        {
            SelectSymbolByControl symbolForm = new SelectSymbolByControl(styleClass);
            symbolForm.ShowDialog();
            IStyleGalleryItem styleItem = symbolForm.m_styleGalleryItem;
            symbolForm.Dispose();

            return styleItem;
        }

        private Bitmap DrawToPictureBox(ISymbol pSym, PictureBox pBox)
        {
            IPoint pPoint = null;
            IGeometry pGeometry = null;
            int hDC;
            System.Drawing.Graphics pGraphics = null;
            pGraphics = System.Drawing.Graphics.FromHwnd(pBox.Handle);
            //clear drawing canvas
            pGraphics.FillRectangle(System.Drawing.Brushes.White, pBox.ClientRectangle);
            if (pSym is IMarkerSymbol)
            {
                pPoint = new PointClass();      //the geometry of a MarkerSymbol
                pPoint.PutCoords(pBox.Width / 2, pBox.Height / 2);       //center in middle of pBox
                pGeometry = pPoint;
            }
            if (pSym is ILineSymbol)
            {
                ISegmentCollection polyline = new ESRI.ArcGIS.Geometry.PolylineClass();
                ISegment line = new ESRI.ArcGIS.Geometry.LineClass();
                IPoint fromPoint = new PointClass();
                fromPoint.PutCoords(pBox.Left, pBox.Bottom);
                IPoint toPoint = new PointClass();
                toPoint.PutCoords(pBox.Right, pBox.Top);
                line.FromPoint = fromPoint;
                line.ToPoint = toPoint;
                object missing = Type.Missing;
                polyline.AddSegment(line, ref missing, ref missing);
                pGeometry = polyline as IGeometry;
            }
            if (pSym is IFillSymbol)
            {
                IEnvelope pEnvelope = new EnvelopeClass();
                pEnvelope.PutCoords(pBox.Left, pBox.Top, pBox.Right, pBox.Bottom);
                pGeometry = pEnvelope;
            }

            hDC = GetDC(pBox.Handle.ToInt32());
            pSym.SetupDC(hDC, null);
            pSym.ROP2 = esriRasterOpCode.esriROPCopyPen;
            pSym.Draw(pGeometry);
            pSym.ResetDC();

            Bitmap image = new Bitmap(pBox.Width, pBox.Height, pGraphics);
            Graphics g2 = Graphics.FromImage(image);
            //获得屏幕的句柄 
            IntPtr dc3 = pGraphics.GetHdc();
            //获得位图的句柄 
            IntPtr dc2 = g2.GetHdc();
            BitBlt(dc2, 0, 0, pBox.Width, pBox.Height, dc3, 0, 0, SRCCOPY);
            pGraphics.ReleaseHdc(dc3);//释放屏幕句柄             
            g2.ReleaseHdc(dc2);//释放位图句柄             
            //image.Save("c:\\MyJpeg.Icon", ImageFormat.Bmp);
            return image;
        }

        private void DrawToTarget(ISymbol pSym, PictureBox pBox)
        {
            IPoint pPoint = null;
            IGeometry pGeometry = null;

            IDisplayTransformation pDisplayTrans = null;

            if (pSym is IMarkerSymbol)
            {
                pPoint = new PointClass();      //the geometry of a MarkerSymbol
                pPoint.PutCoords(pBox.Width / 2, pBox.Height / 2);       //center in middle of pBox
                pGeometry = pPoint;
            }
            if (pSym is ILineSymbol)
            {
                ISegmentCollection polyline = new ESRI.ArcGIS.Geometry.PolylineClass();
                ISegment line = new ESRI.ArcGIS.Geometry.LineClass();
                IPoint fromPoint = new PointClass();
                fromPoint.PutCoords(pBox.Left, pBox.Bottom);
                IPoint toPoint = new PointClass();
                toPoint.PutCoords(pBox.Right, pBox.Top);
                line.FromPoint = fromPoint;
                line.ToPoint = toPoint;
                object missing = Type.Missing;
                polyline.AddSegment(line, ref missing, ref missing);
                pGeometry = polyline as IGeometry;
            }
            if (pSym is IFillSymbol)
            {
                IEnvelope pEnvelope = new EnvelopeClass();
                pEnvelope.PutCoords(pBox.Left, pBox.Top, pBox.Right, pBox.Bottom);
                pGeometry = pEnvelope;
            }
            pDisplayTrans = getTransformation(pBox) as IDisplayTransformation;
            pSym.SetupDC(pBox.Handle.ToInt32(), null);
            pSym.ROP2 = esriRasterOpCode.esriROPCopyPen;
            pSym.Draw(pGeometry);
            pSym.ResetDC();
        }

        private ITransformation getTransformation(PictureBox picTarget)
        {
            RECT boundsRect;
            GetWindowRect(picTarget.Handle.ToInt32(), out boundsRect);
            int lWidth = boundsRect.Right - boundsRect.Left;
            int lHeight = boundsRect.Bottom - boundsRect.Top;

            IDisplayTransformation pDispTrans = new DisplayTransformationClass();
            IEnvelope pBounds = new EnvelopeClass();
            pBounds.PutCoords(picTarget.Left, picTarget.Top, picTarget.Right, picTarget.Bottom);
            //pBounds.PutCoords(0, 0, lHeight, lWidth);
            pDispTrans.VisibleBounds = pBounds;
            pDispTrans.Bounds = pBounds;

            tagRECT deviceRect;
            deviceRect.left = 0;
            deviceRect.top = 0;
            deviceRect.right = lWidth;
            deviceRect.bottom = lHeight;
            pDispTrans.set_DeviceFrame(ref deviceRect);


            IntPtr lHDC = picTarget.Handle;
            int lDpi = GetDeviceCaps(lHDC, LOGPIXELSY);
            if (lDpi == 0) lDpi = 300;

            pDispTrans.Resolution = lDpi;
            return pDispTrans;
        }

        private IPictureDisp CreatePictureFromSymbol(int hDCOld, int hBmpNew, ISymbol pSymbol, int lWidth, int lHeight, int lGap)
        {
            int hDCNew = CreateCompatibleDC(hDCOld);
            hBmpNew = CreateCompatibleBitmap(hDCOld, 2);
            int hBmpOld = SelectObject(hDCNew, hBmpNew);
            // Draw the symbol to the new device context.
            bool lResult;
            lResult = DrawToDC(hDCNew, lWidth, lHeight, pSymbol, lGap);
            hBmpNew = SelectObject(hDCNew, hBmpOld);
            DeleteDC(hDCNew);
            // Return the Bitmap as an OLE Picture.
            return CreatePictureFromBitmap(hBmpNew);
        }

        private IPictureDisp CreatePictureFromBitmap(int hBmpNew)
        {
            PicDesc Pic = new PicDesc();
            IPicture pPic = null;
            GUID IID_IDispatch = new GUID();

            IID_IDispatch.Data1 = 0x20400;
            IID_IDispatch.Data4[0] = 0xC0;
            IID_IDispatch.Data4[7] = 0x46;

            Pic.SIZE = 20;
            Pic.Type = 1;
            Pic.hBmp = hBmpNew;
            Pic.hPal = 0;

            // Create Picture object.
            int result = OleCreatePictureIndirect(Pic, IID_IDispatch, 1, pPic);

            return pPic as IPictureDisp;
        }

        private bool DrawToDC(int hdc, int lWidth, int lHeight, ISymbol pSymbol, int lGap)
        {
            // Create the Transformation and Geometry required by ISymbol::Draw.
            IEnvelope pEnvelope;
            ITransformation pTransformation;
            IGeometry pGeom;
            pEnvelope = new EnvelopeClass();
            pEnvelope.PutCoords(lGap, lGap, lWidth - lGap, lHeight - lGap);
            pTransformation = CreateTransFromDC(hdc, lWidth, lHeight);
            pGeom = CreateSymShape(pSymbol, pEnvelope);
            // Perform the Draw operation.
            if (pTransformation == null || pGeom == null) return false;
            pSymbol.SetupDC(hdc, pTransformation);
            pSymbol.Draw(pGeom);
            pSymbol.ResetDC();
            return true;
        }

        private bool DrawToWnd(int hWnd, ISymbol pSymbol, int lGap)
        {
            int hDC;
            if (hWnd != 0)
            {
                // Calculate size of window.
                RECT udtRect = new RECT();
                int lResult;
                lResult = GetClientRect(hWnd, out udtRect);
                if (lResult != 0)
                {
                    int lWidth, lHeight;
                    lWidth = udtRect.Right - udtRect.Left;
                    lHeight = udtRect.Bottom - udtRect.Top;
                    hDC = GetDC(hWnd);    // Must release the DC afterwards.
                    if (hDC != 0)
                    {
                        return DrawToDC(hDC, lWidth, lHeight, pSymbol, lGap);
                    }
                    ReleaseDC(hWnd, hDC);  //' Release cached DC obtained with GetDC.
                }
            }
            return false;
        }

        private ITransformation CreateTransFromDC(int hDC, int lWidth, int lHeight)
        {
            // Calculate the parameters for the new transformation, based on the dimensions passed to this function.
            IEnvelope pBoundsEnvelope = new EnvelopeClass();
            pBoundsEnvelope.PutCoords(0, 0, lWidth, lHeight);
            tagRECT deviceRect;

            deviceRect.left = 0;
            deviceRect.top = 0;
            deviceRect.right = lWidth;
            deviceRect.bottom = lHeight;

            //int dpi;
            //dpi = GetDeviceCaps(hDC, LOGPIXELSY);
            //if (dpi == 0) return null;
            // Create a new display transformation and set its properties.
            IDisplayTransformation pDisplayTransformation = new DisplayTransformationClass();


            pDisplayTransformation.VisibleBounds = pBoundsEnvelope;
            pDisplayTransformation.Bounds = pBoundsEnvelope;
            pDisplayTransformation.set_DeviceFrame(ref deviceRect);
            pDisplayTransformation.Resolution = 300;
            return pDisplayTransformation;
        }

        private IGeometry CreateSymShape(ISymbol pSymbol, IEnvelope pEnvelope)
        {
            IGeometry geom = null;
            // This function returns an appropriate Geometry type depending on the Symbol type passed in.
            if (pSymbol is IMarkerSymbol)
            {
                // For a MarkerSymbol return a Point.
                IArea pArea = pEnvelope as IArea;
                geom = pArea.Centroid;
            }
            else if (pSymbol is ILineSymbol || pSymbol is ITextSymbol)
            {
                //For a LineSymbol or TextSymbol return a Polyline.
                IPolyline pPolyline = new PolylineClass();
                pPolyline.FromPoint = pEnvelope.LowerLeft;
                pPolyline.ToPoint = pEnvelope.UpperRight;
                geom = pPolyline;
            }
            else
            {
                // For any FillSymbol return an Envelope.
                geom = pEnvelope;
            }
            return geom;
        }

        private void ssss()
        {

        }

        private int GetBitmap(ISymbol symbol, IGeometry geometry, int lWidth, int lHeight)
        {
            if (symbol == null) return 0;
            if (geometry == null) return 0;


            RECT rect;
            rect.Left = 0;
            rect.Top = 0;
            rect.Right = lWidth;
            rect.Bottom = lHeight;

            int hDC = GetDC(0);
            int hMemDC = CreateCompatibleDC(hDC);
            int hBitmap = CreateCompatibleBitmap(hDC, lWidth, lHeight);
            int hOldBitmap = SelectObject(hMemDC, hBitmap);

            int hBrush = CreateSolidBrush(RGB2Long(255, 255, 255));
            int hOldBrush = SelectObject(hMemDC, hBrush);
            FillRect(hMemDC, rect, hBrush);//paint a white background.

            symbol.SetupDC(hMemDC, null);
            symbol.Draw(geometry);
            symbol.ResetDC();

            SelectObject(hDC, hOldBitmap);
            SelectObject(hDC, hOldBrush);
            DeleteObject(hBrush);

            ReleaseDC(0, hMemDC);
            ReleaseDC(0, hDC);
            DeleteDC(hMemDC);
            DeleteDC(hDC);
            return hBitmap;
        }

        private long RGB2Long(int red, int green, int blue)
        {
            return red + (0x100 * green) + (0x10000 * blue);
        }


        private int CopyScreenToBitmap(RECT lpRect)
        {
            //// 屏幕和内存设备描述表 
            //IntPtr hScrDC, hMemDC;
            //// 位图句柄 
            //int hBitmap, hOldBitmap;
            //// 选定区域坐标 
            //int nX, nY, nX2, nY2;
            //// 位图宽度和高度
            //int nWidth, nHeight;
            //// 屏幕分辨率
            //int xScrn, yScrn;

            //// 确保选定区域不为空矩形 
            //if (IsRectEmpty(lpRect) != 0) return 0;
            ////为屏幕创建设备描述表 
            //hScrDC = CreateDC("DISPLAY", "aa", "bb", 0);
            ////为屏幕设备描述表创建兼容的内存设备描述表 
            //hMemDC = CreateCompatibleDC(hScrDC);
            //// 获得选定区域坐标 
            //nX = lpRect.Left;
            //nY = lpRect.Top;
            //nX2 = lpRect.Right;
            //nY2 = lpRect.Bottom;
            //// 获得屏幕分辨率 
            //xScrn = GetDeviceCaps(hScrDC, HORZRES);
            //yScrn = GetDeviceCaps(hScrDC, VERTRES);
            ////确保选定区域是可见的 
            //if (nX < 0) nX = 0;
            //if (nY < 0) nY = 0;
            //if (nX2 > xScrn) nX2 = xScrn;
            //if (nY2 > yScrn) nY2 = yScrn;
            //nWidth = nX2 - nX;
            //nHeight = nY2 - nY;
            //// 创建一个与屏幕设备描述表兼容的位图 
            //hBitmap = CreateCompatibleBitmap(hScrDC, nWidth, nHeight);
            //// 把新位图选到内存设备描述表中 
            //hOldBitmap = SelectObject(hMemDC, hBitmap);
            //// 把屏幕设备描述表拷贝到内存设备描述表中 
            //BitBlt(hMemDC, 0, 0, nWidth, nHeight, hScrDC, nX, nY, 13369376);
            ////得到屏幕位图的句柄 
            //hBitmap = SelectObject(hMemDC, hOldBitmap);

            ////清除 
            //DeleteDC(hScrDC);
            //DeleteDC(hMemDC);
            //// 返回位图句柄 
            //return hBitmap;

            ////Image.FromHbitmap(new IntPtr(hBMP));
            return 0;
        }

        public const int HORZSIZE = 4;
        public const int VERTSIZE = 6;
        public const int HORZRES = 8;
        public const int VERTRES = 10;
        public const int ASPECTX = 40;
        public const int ASPECTY = 42;
        public const int LOGPIXELSX = 88;
        public const int LOGPIXELSY = 90;
        public const int SRCCOPY = 13369376;

        public struct GUID
        {
            public int Data1;
            public int Data2;
            public int Data3;
            public byte[] Data4;
        }

        public struct PicDesc
        {
            public int SIZE;
            public int Type;
            public int hBmp;
            public int hPal;
            public int intReserved;
        }

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern IntPtr DeleteDC(IntPtr hDc);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        public static extern IntPtr DeleteObject(IntPtr hDc);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        public static extern bool BitBlt(IntPtr hdcDest, int xDest,
                   int yDest, int wDest, int hDest, IntPtr hdcSource,
                   int xSrc, int ySrc, int RasterOp);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,
                   int nWidth, int nHeight);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);


        [DllImport("gdi32")]
        public static extern int CreateCompatibleDC(int hDC);
        [DllImport("gdi32")]
        public static extern int CreateCompatibleBitmap(int hDC, int hObject);
        [DllImport("gdi32")]
        public static extern int CreateCompatibleBitmap(int hDC, int width, int hight);
        [DllImport("gdi32")]
        public static extern int DeleteDC(int hDC);
        [DllImport("gdi32")]
        public static extern int SelectObject(int hDC, int hObject);
        [DllImport("gdi32")]
        public static extern int CreatePen(int nPenStyle, int nWidth, int crColor);
        [DllImport("gdi32")]
        public static extern int CreateSolidBrush(long crColor);
        [DllImport("gdi32")]
        public static extern int DeleteObject(int hObject);

        [DllImport("GDI32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDC(
        string lpszDriver, // 驱动名称 
        string lpszDevice, // 设备名称 
        string lpszOutput, // 无用，可以设定位"NULL" 
        int lpInitData // 任意的打印机数据 
        );


        [DllImportAttribute("olepro32.dll")]
        public static extern int OleCreatePictureIndirect(PicDesc pDesc, GUID RefIID, int fPictureOwnsHandle, IPicture pPic);

        [DllImport("User32.dll")]
        public static extern int GetDC(int hWnd);
        [DllImport("User32.dll")]
        public static extern int ReleaseDC(int hWnd, int hDC);
        [DllImport("User32.dll")]
        public static extern int FillRect(int hdc, RECT lpRect, int hBrush);
        [DllImport("User32.dll")]
        public static extern int IsRectEmpty(RECT lpRect);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int pvParam, uint fWinIni);
        [DllImport("user32.dll")]
        public static extern int GetClientRect(int hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern int GetWindowRect(int hwnd, out RECT lpRect);

        private void lvRendererFields_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem selectedItem = lvRendererFields.FocusedItem;
            if (selectedItem == null) return;
            int imgIdx = selectedItem.ImageIndex;

            //ISymbol symbol = GetSymbolBySymbolSelector(esriGeometryType.esriGeometryPolygon);
            IStyleGalleryItem styleItem = GetSymbolBySymbologyControl("Marker Symbols");
            if (styleItem == null) return;

            ISymbol symbol = styleItem.Item as ISymbol;
            if (symbol == null) return;

            //symbolPBox.Visible = true;
            IStyleGalleryClass mStyleClass = new MarkerSymbolStyleGalleryClassClass();
            Bitmap image = StyleGalleryItemToBmp(24, 24, mStyleClass, styleItem);
            //Bitmap image = DrawToPictureBox(symbol, symbolPBox);
            Largeimage.Images[imgIdx] = image;
            Smallimage.Images[imgIdx] = image;
            lvRendererFields.Refresh();
            if (fieldSymbolHashTable.ContainsKey(selectedItem.Text))
            {
                fieldSymbolHashTable.Remove(selectedItem.Text);
            }
            fieldSymbolHashTable.Add(selectedItem.Text, symbol);
            //symbolPBox.Visible = false;
        }

        public Bitmap StyleGalleryItemToBmp(int iWidth, int iHeight, IStyleGalleryClass mStyleGlyCs, IStyleGalleryItem mStyleGlyItem)
        {
            //建立符合规格的内存图片
            Bitmap bmp = new Bitmap(iWidth, iHeight);
            Graphics gImage = Graphics.FromImage(bmp);
            //建立对应的符号显示范围
            tagRECT rect = new tagRECT();
            rect.right = bmp.Width;
            rect.bottom = bmp.Height;
            //生成预览
            System.IntPtr hdc = new IntPtr();
            hdc = gImage.GetHdc();
            //在图片上绘制符号
            mStyleGlyCs.Preview(mStyleGlyItem.Item, hdc.ToInt32(), ref rect);
            gImage.ReleaseHdc(hdc);
            gImage.Dispose();

            return bmp;
        }
     
    }
}
