using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;
using System.Xml.XPath;

namespace qqcs
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection Connection;
        private DataTable dt;
        private XElement Xml;

        public MainWindow()
        {
            InitializeComponent();

            LoadSettingFile();

            SetQueryName();

            DatabaseConnection();


        }

        private void LoadSettingFile()
        {

            string currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            this.Xml = XElement.Load(currentDirectory + "qqcs.xml");


        }

        private void SetQueryName()
        {
            var querys = (from p in this.Xml.Elements("Query") select p);
            foreach(var q in querys)
            {
                QueryCombobox.Items.Add(q.Element("Name").Value);
            }

        }
        private void DatabaseConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            var cs = this.Xml.Element("ConnectString").Value;
            this.Connection = new SqlConnection(cs);
           
            this.Connection.Open();

            this.StatusLabel.Content = this.Connection.State;

        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            var id = this.QueryCombobox.SelectedIndex;

            var query = (from q in this.Xml.Elements("Query") where q.Attribute("id").Value == id.ToString() select q).First();
            string sql = query.Element("SQL").Value;
            // パラメータ セット
            foreach (var p in query.XPathSelectElements("Param"))
            {
                if(sql.Contains("#param" + p.Attribute("id").Value))
                {
                    sql = sql.Replace(("#param" + p.Attribute("id").Value), this.ConditionFlexGrid[int.Parse(p.Attribute("id").Value), 1].ToString());
                }
            }

            using (SqlCommand command = new SqlCommand(sql, this.Connection))
            {
                var addapter = new SqlDataAdapter(command);
                this.dt = new DataTable();
                addapter.Fill(this.dt);
                CollectionView cv = new BindingListCollectionView(this.dt.AsDataView());
                this.ResultFlexGrid.ItemsSource = cv;
            }

            // 抽出条件保存
            query = (from p in this.Xml.Elements("Query") where p.Attribute("id").Value == id.ToString() select p).First();
            foreach (var p in query.XPathSelectElements("Param"))
            {
                p.Element("Value").Value = this.ConditionFlexGrid[int.Parse(p.Attribute("id").Value), 1].ToString();
            }
            string currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            this.Xml.Save(currentDirectory + "qqcs.xml");

        }

        private void ResultToCsv_Click(object sender, RoutedEventArgs e)
        {
            CsvOperator co = new CsvOperator();
            var dialog = new SaveFileDialog();
            dialog.Title = "CSVファイル保存";
            dialog.Filter = "CSVファイル|*.csv";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                co.OutputCsv(this.dt, dialog.FileName, true, ",");
                System.Windows.MessageBox.Show("CSVファイルを保存しました。", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void QueryCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var id = this.QueryCombobox.SelectedIndex;
            var query = (from p in this.Xml.Elements("Query") where p.Attribute("id").Value == id.ToString() select p).First();
            var dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");
            foreach (var p in query.XPathSelectElements("Param"))
            {
                dt.Rows.Add(p.Element("Name").Value, p.Element("Value").Value);
            }
            CollectionView cv = new BindingListCollectionView(dt.AsDataView());
            this.ConditionFlexGrid.ItemsSource = cv;
            if (ConditionFlexGrid.Columns.Count > 0)
            {
                ConditionFlexGrid.Columns[0].IsReadOnly = true;
            }
            ConditionFlexGrid.Columns[0].MaxWidth = 140;
            ConditionFlexGrid.Columns[1].MaxWidth = 120;

        }
    }
}
