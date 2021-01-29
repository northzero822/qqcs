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

        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            this.dt = new DataTable();

            var id = this.QueryCombobox.SelectedIndex;

            var query = (from p in this.Xml.Elements("Query") where p.Attribute("id").Value == id.ToString() select p).First();
            string sql = query.Element("SQL").Value;
            using (SqlCommand command = new SqlCommand(sql, this.Connection))
            {
                var addapter = new SqlDataAdapter(command);
                addapter.Fill(this.dt);
                CollectionView cv = new BindingListCollectionView(this.dt.AsDataView());
                this.ResultFlexGrid.ItemsSource = cv;
            }
        }

        private void ResultToCsv_Click(object sender, RoutedEventArgs e)
        {
            CsvOperator co = new CsvOperator();
            var dialog = new SaveFileDialog();
            dialog.Title = "CSVファイルを保存";
            dialog.Filter = "CSVファイル|*.csv";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                co.OutputCsv(this.dt, dialog.FileName, true, ",");
            }

            System.Windows.MessageBox.Show("CSVファイルを保存しました。", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
