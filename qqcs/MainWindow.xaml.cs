using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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

            //InsertTest();

            //SelectTest(connection);
            //UpdateTest(connection);
            //SelectTest(connection);
            //DeleteTest(connection);


        }
        private void InsertTest()
        {
            string sql = "INSERT INTO M地区 VALUES(99,'99',99,99)";
            using (SqlCommand command = new SqlCommand(sql, this.Connection))
            {
                int rowsAffected = command.ExecuteNonQuery();
                MessageBox.Show(rowsAffected.ToString() + " 行 挿入されました。");
            }
        }

        private void SelectTest(SqlConnection connection)
        {
            int i = 0;
            string sql = "SELECT * FROM D売上入金 WHERE 削除区分=0 AND 伝票日付>=20200101 AND データ区分=1";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        i = (int)reader["地区コード"];
                        MessageBox.Show(i.ToString() + reader["地区名"]);
                    }
                }
            }
        }

        private void UpdateTest(SqlConnection connection)
        {
            string sql = "UPDATE M地区 SET 地区名='100' WHERE 地区コード=99";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                int rowsAffected = command.ExecuteNonQuery();
                MessageBox.Show(rowsAffected.ToString() + "更新しました");
            }
        }
        private void DeleteTest(SqlConnection connection)
        {
            string sql = "DELETE M地区 WHERE 地区コード=99";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                int rowsAffected = command.ExecuteNonQuery();
                MessageBox.Show(rowsAffected.ToString() + " 行 削除されました。");
            }

        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            this.dt = new DataTable();

            var query = (from p in this.Xml.Elements("Query") where p.Attribute("id").Value == "0" select p).First();
            MessageBox.Show(query.Element("Name").Value);

            string sql = "SELECT * FROM D売上入金 WHERE 削除区分=0 AND 伝票日付>=20200101 AND データ区分=1";
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
            co.OutputCsv(this.dt, @"c:\okita\test.csv", true, ",");
        }
    }
}
