using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using System.Collections;
using System.Drawing.Printing;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;

namespace GIS应用系统
{
    public partial class frmPrintView : Form
    {
        //定义页面设置、打印预览和打印对话框
        private PrintPreviewDialog printPreviewDialog1;
        private PrintDialog printDialog1;
        private PageSetupDialog pageSetupDialog1;
        private System.Drawing.Printing.PrintDocument document = new System.Drawing.Printing.PrintDocument();
        private ITrackCancel m_TrackCancel = new CancelTrackerClass();
        private short currentPrintPage;// 定义页数变量
        public frmPrintView(AxPageLayoutControl pageltcontrol)
        {
            InitializeComponent();
            //同步布局视图函数
            SynchroniseLayoutView(pageltcontrol);
        }

        private void frmPrintView_Load(object sender, EventArgs e)
        {
            InitializePrintPreviewDialog(); //初始化打印预览对话框
            printDialog1 = new PrintDialog(); //实例化打印对话框
            InitializePageSetupDialog(); //初始化打印设置对话     
        }
        #region 页面设置
        private void InitializePageSetupDialog()
        {
            pageSetupDialog1 = new PageSetupDialog();
            //初始化页面设置对话框的页面设置属性为缺省设置
            pageSetupDialog1.PageSettings = new System.Drawing.Printing.PageSettings();
            //初始化页面设置对话框的打印机属性为缺省设置
            pageSetupDialog1.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
        }
        private void pageSetup_Click(object sender, EventArgs e)
        {
            try
            {
                //实例化打印设置窗口
                DialogResult result = pageSetupDialog1.ShowDialog();
                //设置打印文档对象的打印机
                document.PrinterSettings = pageSetupDialog1.PrinterSettings;
                //设置打印文档对象的页面设置为用户在打印设置对话框中的设置
                document.DefaultPageSettings = pageSetupDialog1.PageSettings;
                //页面设置
                int i;
                IEnumerator paperSizes = pageSetupDialog1.PrinterSettings.PaperSizes.GetEnumerator();
                paperSizes.Reset();
                for (i = 0; i < pageSetupDialog1.PrinterSettings.PaperSizes.Count; ++i)
                {
                    paperSizes.MoveNext();
                    if (((PaperSize)paperSizes.Current).Kind == document.DefaultPageSettings.PaperSize.Kind)
                    {
                        document.DefaultPageSettings.PaperSize = ((PaperSize)paperSizes.Current);
                    }
                }
                //初始化纸张和打印机
                IPaper paper = new PaperClass();
                IPrinter printer = new EmfPrinterClass();
                //关联打印机对象和纸张对象 
                paper.Attach(pageSetupDialog1.PrinterSettings.GetHdevmode(pageSetupDialog1.PageSettings).ToInt32(),
                pageSetupDialog1.PrinterSettings.GetHdevnames().ToInt32());
                printer.Paper = paper;
                PrintPageLayoutControl.Printer = printer;
            }
            catch { }
        }
        #endregion

        #region 打印预览
        private void InitializePrintPreviewDialog()
        {
            printPreviewDialog1 = new PrintPreviewDialog();
            //设置打印预览的尺寸，位置，名称，以及最小尺寸
            printPreviewDialog1.ClientSize = new System.Drawing.Size(800, 600);
            printPreviewDialog1.Location = new System.Drawing.Point(29, 29);
            printPreviewDialog1.Name = "打印预览对话框";
            printPreviewDialog1.MinimumSize = new System.Drawing.Size(375, 250);
            printPreviewDialog1.UseAntiAlias = true;
            this.document.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(document_PrintPage);
        }
        private void printPreview_Click(object sender, EventArgs e)
        {
            try
            {
                //初始化当前打印页码
                currentPrintPage = 0;
                document.DocumentName = PrintPageLayoutControl.DocumentFilename;
                printPreviewDialog1.Document = document;
                printPreviewDialog1.ShowDialog();
            }
            catch { }
        }
        private void document_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                //当 PrintPreviewDialog的Show方法触发时，引用这段代码 
                PrintPageLayoutControl.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingTile;
                //获取打印分辨率
                short dpi = (short)e.Graphics.DpiX;
                IEnvelope devBounds = new EnvelopeClass();
                //获取打印页面
                IPage page = PrintPageLayoutControl.Page;
                //获取打印的页数
                short printPageCount;
                printPageCount = PrintPageLayoutControl.get_PrinterPageCount(0);
                currentPrintPage++;
                //选择打印机
                IPrinter printer = PrintPageLayoutControl.Printer;
                //获得打印机页面大小
                page.GetDeviceBounds(printer, currentPrintPage, 0, dpi, devBounds);
                //获得页面大小的坐标范围，即四个角的坐标
                tagRECT deviceRect;
                double xmin, ymin, xmax, ymax;
                devBounds.QueryCoords(out xmin, out ymin, out xmax, out ymax);
                deviceRect.bottom = (int)ymax;
                deviceRect.left = (int)xmin;
                deviceRect.top = (int)ymin;
                deviceRect.right = (int)xmax;
                //确定当前打印页面的大小
                IEnvelope visBounds = new EnvelopeClass();
                page.GetPageBounds(printer, currentPrintPage, 0, visBounds);
                IntPtr hdc = e.Graphics.GetHdc();
                PrintPageLayoutControl.ActiveView.Output(hdc.ToInt32(), dpi, ref deviceRect, visBounds, m_TrackCancel);
                e.Graphics.ReleaseHdc(hdc);
                if (currentPrintPage < printPageCount)
                    e.HasMorePages = true;
                else
                    e.HasMorePages = false;
            }
            catch { }
        }
        #endregion

        #region 打印
        private void print_Click(object sender, EventArgs e)
        {
            try
            {
                //显示帮助按钮            
                printDialog1.ShowHelp = true;
                printDialog1.Document = document;
                //显示打印窗口
                DialogResult result = printDialog1.ShowDialog();
                // 如果显示成功，则打印.
                if (result == DialogResult.OK) document.Print();
                Close();
            }
            catch { }
        }
        #endregion

        #region  同步布局视图
        private void SynchroniseLayoutView(AxPageLayoutControl mainlayoutControl)
        {
            IObjectCopy objectcopy = new ObjectCopyClass();
            object tocopymap = mainlayoutControl.ActiveView.GraphicsContainer;   //获取mapcontrol中的map，这个是原始的
            object copiedmap = objectcopy.Copy(tocopymap);       //复制一份map，是一个副本
            object tooverwritemap = PrintPageLayoutControl.ActiveView.GraphicsContainer; // 控件和工具作用的当前地图
            objectcopy.Overwrite(copiedmap, ref tooverwritemap);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(objectcopy);
            IGraphicsContainer mainGraphCon = tooverwritemap as IGraphicsContainer;
            mainGraphCon.Reset();
            IElement pElement = mainGraphCon.Next();
            IArray pArray = new ArrayClass();
            while (pElement != null)
            {
                pArray.Add(pElement);
                pElement = mainGraphCon.Next();
            }
            int pElementCount = pArray.Count;
            IPageLayout PrintPageLayout = PrintPageLayoutControl.PageLayout;
            IGraphicsContainer PrintGraphCon = PrintPageLayout as IGraphicsContainer;
            PrintGraphCon.Reset();
            IElement pPrintElement = PrintGraphCon.Next();
            while (pPrintElement != null)
            {
                PrintGraphCon.DeleteElement(pPrintElement);
                pPrintElement = PrintGraphCon.Next();
            }
            for (int i = 0; i < pArray.Count; i++)
            {
                PrintGraphCon.AddElement(pArray.get_Element(pElementCount - 1 - i) as IElement, 0);
            }
            PrintPageLayoutControl.Refresh();

        }
        #endregion

    }
}
