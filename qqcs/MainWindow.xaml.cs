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
                UpdateTest(connection);
                SelectTest(connection);
                DeleteTest(connection);

            }

        }
        private void InsertTest(SqlConnection connection)
        {
            string sql = "INSERT INTO M地区 VALUES(99,'99',99,99)";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                int rowsAffected = command.ExecuteNonQuery();
                MessageBox.Show(rowsAffected.ToString() + " 行 挿入されました。");
            }
        }

        private void SelectTest(SqlConnection connection)
        {
            int i = 0;
            string sql = "SELECT * FROM M地区 WHERE 地区コード=99";
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
    }
}
