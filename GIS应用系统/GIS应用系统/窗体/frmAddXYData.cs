using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesFile;

namespace GIS应用系统
{
    public partial class frmAddXYData : Form
    {
        private OleDbConnection m_conDBConnection;
        public frmAddXYData()
        {
            InitializeComponent();
        }
        private AxMapControl m_axMapControl;
        /// <summary>
        /// 传递主窗体AxMapcontrol控件
        /// </summary>
        public AxMapControl GetAxMapControl
        {
            set { m_axMapControl = value; }
            get { return m_axMapControl; }
        }

        private AxTOCControl m_axTOCControl;
        /// <summary>
        /// 传递主窗体AxTOCControl控件
        /// </summary>
        public AxTOCControl GetAxTOCControl
        {
            set { m_axTOCControl = value; }
            get { return m_axTOCControl; }
        }
        /// <summary>
        /// 打开Excel操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenExcel_Click(object sender, EventArgs e)
        {
            var OFD = new OpenFileDialog();
            OFD.Filter = "Excel数据|*.xls;*.xlsx";
            if (OFD.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            //获取选择Excel路径
            string strDBName = OFD.FileName;
            textBoxExcelPath.Text = strDBName;
            textBoxExcelPath.Tag = OFD.FilterIndex;
            //构造连接Excel字符串
            StringBuilder strConnect = new StringBuilder();
            string extension = System.IO.Path.GetExtension(strDBName);
            switch (extension)
            {
                case ".xls":
                    //当为Excel03格式时
                    strConnect.Append(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};", strDBName));
                    strConnect.Append("Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'");
                    break;
                case ".xlsx":
                    //当为Excel07格式时
                    strConnect.Append(string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};", strDBName));
                    strConnect.Append("Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'");
                    break;
                default:
                    break;
            }
            if (strConnect.ToString() == string.Empty)
            {
                MessageBox.Show("打开Excel格式不支持！");
                return;
            }
            comboBoxExcelSheets.Items.Clear();
            m_conDBConnection = new OleDbConnection();
            m_conDBConnection.ConnectionString = strConnect.ToString();
            m_conDBConnection.Open();
            //获取Excel中sheet列表
            DataTable dtTable = m_conDBConnection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, 
            new System.Object[] { null, null, null, "TABLE" });
            if (dtTable == null)
            {
                MessageBox.Show("未能找到有效的Sheet表");
                return;
            }
            //将获取到的Sheet列表添加到【选择坐标数据表】下拉列表中           
            foreach (System.Data.DataRow row in dtTable.Rows)
            {
                string strTableName = row["TABLE_NAME"].ToString();
                comboBoxExcelSheets.Items.Add(strTableName);
            }
            comboBoxExcelSheets.SelectedIndex = 0;
        }
        /// <summary>
        /// 选择sheet表发生变化时，读取sheet表中列名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxExcelSheets_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.comboBoxXField.Items.Clear();
            this.comboBoxYield.Items.Clear();
            // 选择sheet表发生变化时，读取sheet表中列名加载到“成图X字段”和“成图Y字段”
            //下拉选择列表
            string sheetTableName = comboBoxExcelSheets.SelectedItem.ToString();
            DataTable dt = new DataTable();
            //读取前10条数据，返回数据结果DataTable
            dt = QueryBySql(String.Format("select top 10 * from [{0}]", sheetTableName));
            if (dt != null)
            {
                //通过遍历加载X Y字段下拉选择框
                foreach (DataColumn column in dt.Columns)
                {
                    this.comboBoxXField.Items.Add(column.ColumnName);
                    this.comboBoxYield.Items.Add(column.ColumnName);
                }
                this.comboBoxXField.SelectedIndex = 0;
                this.comboBoxYield.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// sql查询返回table形式
        /// </summary>
        /// <param name="p_strSql"></param>
        /// <returns></returns>
        private DataTable QueryBySql(string p_strSql)
        {
            //构造数据库操作变量，利用sql查询返回DataTable形式
            OleDbCommand m_cmdCommand = new OleDbCommand();
            m_cmdCommand.Connection = m_conDBConnection;
            m_cmdCommand.CommandText = p_strSql;
            m_cmdCommand.CommandType = CommandType.Text;
            //进行数据查询
            using (OleDbDataAdapter m_dtrAdapter = new OleDbDataAdapter(m_cmdCommand))
            {
                DataSet objDs = new DataSet();
                m_dtrAdapter.Fill(objDs);
                return objDs.Tables[0];
            }
        }

        /// <summary>
        /// 将X Y坐标成图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            //将X Y坐标成图，首先判定各设置条件是否完整
            if (this.textBoxExcelPath.Text.Trim() == string.Empty)
            {
                MessageBox.Show("坐标数据不能为空！");
                return;
            }
            if (this.comboBoxExcelSheets.SelectedIndex == -1)
            {
                MessageBox.Show("未选择X Y坐标数据Sheet表！");
                return;
            }
            if (this.comboBoxXField.SelectedIndex == -1)
            {
                MessageBox.Show("未选择成图X字段 ！");
                return;
            }
            if (this.comboBoxYield.SelectedIndex == -1)
            {
                MessageBox.Show("未选择成图Y字段 ！");
                return;
            }
            if (this.comboBoxXField.SelectedItem.ToString() == this.comboBoxYield.SelectedItem.ToString())
            {
                MessageBox.Show("成图X字段与成图Y字段不能相同 ！");
                return;
            }
            //下面进行数据成图
            string xFieldName = this.comboBoxXField.SelectedItem.ToString();
            string yFieldName = this.comboBoxYield.SelectedItem.ToString();
            string sheetTableName = this.comboBoxExcelSheets.SelectedItem.ToString();
            //获得所有数据
            DataTable dt = QueryBySql(String.Format("select * from [{0}]", sheetTableName));
            //将DataTable转化为ITable
            ITable pTable = DataTableToITable(dt, xFieldName, yFieldName, @"c:\");
            if (pTable == null)
            {
                MessageBox.Show("数据转换失败！没有将DataTable转化为ITable。");
                return;
            }
            IFeatureClass pFeatureClass = OpenXYData(pTable, "X", "Y", "", new UnknownCoordinateSystemClass());
            IFeatureLayer pFeatureLayer = new FeatureLayerClass();
            pFeatureLayer.FeatureClass = pFeatureClass;
            pFeatureLayer.Name = "转换结果";
            m_axMapControl.AddLayer(pFeatureLayer as ILayer, 0);
            m_axMapControl.Refresh();
            m_axTOCControl.Refresh();
            this.Close();
        }

        /// <summary>
        /// 将带有x、y字段的ITable对象转化为IFeatureClass
        /// </summary>
        /// <param name="pTable">带有x、y字段的ITable数据源</param>
        /// <param name="xName">X字段名称</param>
        /// <param name="yName">y字段名称</param>
        /// <param name="zName">z字段名称（可为空）</param>
        /// <param name="spatialReference">空间坐标参考</param>
        /// <returns></returns>
        private IFeatureClass OpenXYData(ITable pTable, string xName, string yName, string zName, ISpatialReference spatialReference)
        {
            // 将带有x、y字段的ITable对象转化为IFeatureClass,确定X、Y、Z字段以及空间参考坐标
            // xy字段属性描述
            IXYEvent2FieldsProperties pXYEventFieldsPro = new XYEvent2FieldsPropertiesClass();
            pXYEventFieldsPro.XFieldName = xName;
            pXYEventFieldsPro.YFieldName = yName;
            pXYEventFieldsPro.ZFieldName = zName;

            //XY事件 数据表名称描述
            IXYEventSourceName pXYEventSourceName = new XYEventSourceNameClass();
            pXYEventSourceName.EventProperties = pXYEventFieldsPro;

            IDataset ds = pTable as IDataset;
            IName tname = ds.FullName;
            pXYEventSourceName.SpatialReference = spatialReference;
            pXYEventSourceName.EventTableName = tname;

            IName xyname = pXYEventSourceName as IName;
            IXYEventSource xysrc = (IXYEventSource)xyname.Open();
            return xysrc as IFeatureClass;
        }

        /// <summary>
        /// 把DataTable转为ITable ,tempPath 不含文件名的w文件夹路径
        /// </summary>
        /// <param name="mTable"></param>
        /// <returns></returns>
        private ITable DataTableToITable(DataTable mTable, string xFieldName, string yFieldName, string tempPath)
        {
            try
            {
                #region 新建表字段
                IField pField = null;
                IFields fields = new FieldsClass();
                IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
                fieldsEdit.FieldCount_2 = 2;
                //添加X字段
                pField = new FieldClass();
                IFieldEdit fieldEdit = (IFieldEdit)pField;
                fieldEdit.Name_2 = "X";
                fieldEdit.AliasName_2 = "X";
                //设置字段类型
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                fieldEdit.Editable_2 = true;
                fieldsEdit.set_Field(0, pField);
                //添加Y字段
                IField pField1 = new FieldClass();
                IFieldEdit fieldEdit1 = (IFieldEdit)pField1;
                fieldEdit1.Name_2 = "Y";
                fieldEdit1.AliasName_2 = "Y";
                fieldEdit1.Type_2 = esriFieldType.esriFieldTypeDouble;
                fieldEdit1.Editable_2 = true;
                fieldsEdit.set_Field(1, pField1);
                #endregion
                ShapefileWorkspaceFactoryClass class2 = new ShapefileWorkspaceFactoryClass();
                ESRI.ArcGIS.Geodatabase.IWorkspace pWorkspace = class2.OpenFromFile(tempPath, 0);
                IFeatureWorkspace pFWS = pWorkspace as IFeatureWorkspace;
                //删除已有的
                if (System.IO.File.Exists(tempPath + "点数据.dbf"))
                {
                    System.IO.File.Delete(tempPath + "点数据.dbf");
                }
                //创建空表
                ITable pTable = pFWS.CreateTable("点数据", fieldsEdit, null, null, "");
                //遍历DataTable中数据，然后转换为ITable中的数据
                int xRowIndex = pTable.Fields.FindField("X");
                int yRowIndex = pTable.Fields.FindField("Y");
                for (int k = 0; k < mTable.Rows.Count; k++)
                {
                    //ITable 的记录
                    IRow row = pTable.CreateRow();
                    DataRow pRrow = mTable.Rows[k];
                    row.set_Value(xRowIndex, pRrow[xFieldName]);
                    row.set_Value(yRowIndex, pRrow[yFieldName]);
                    row.Store();
                }
                return pTable;
            }
            catch
            {
                return null;
            }
        }

        private void buttonCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
