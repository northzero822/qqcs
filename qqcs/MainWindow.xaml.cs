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

namespace qqcs
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DatabaseConnection();


        }

        private void DatabaseConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"172.16.3.141";   // 接続先の SQL Server インスタンス
            builder.UserID = "sa";              // 接続ユーザー名
            builder.Password = "Sapassword1"; // 接続パスワード
            builder.InitialCatalog = "GBS_V1_DATA";  // 接続するデータベース(ここは変えないでください)

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                MessageBox.Show("接続しました");
                InsertTest(connection);
                SelectTest(connection);

            }

        }
        private void InsertTest(SqlConnection connection)
        {
            string sql = "";
            sql += "SELECT * FROM Mユーザー情報";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {

            }
        }

        private void SelectTest(SqlConnection connection)
        {
            string sql = "";
            sql += "SELECT * FROM Mユーザー情報";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {

            }
        }
    }
}
