using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;
using NpgsqlTypes;
using System.Linq;

namespace dinar_form
{
    public partial class Form1 : Form
    {
        DataSet ds;
        NpgsqlDataAdapter adapter;
        NpgsqlCommandBuilder commandBuilder;
        string connectionString = @"Server=127.0.0.1;Port=5432;Database=dinar;User Id=postgres;Password=admin;";
        string sql = "SELECT * FROM customer";

        public Form1()
        {
            InitializeComponent();

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                adapter = new NpgsqlDataAdapter(sql, connection);

                ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Columns["id"].ReadOnly = true;
               
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataRow row = ds.Tables[0].NewRow(); // добавляем новую строку в DataTable
            ds.Tables[0].Rows.Add(row);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                adapter = new NpgsqlDataAdapter(sql, connection);
                commandBuilder = new NpgsqlCommandBuilder(adapter);
             
                adapter.InsertCommand=new NpgsqlCommand(@"INSERT INTO customer (club_id, passport, adress, telephone, name)
                                                          VALUES(@club_id, @passport, @adress, @telephone, @name)");
                adapter.InsertCommand.Parameters.Add(new NpgsqlParameter("@club_id", NpgsqlDbType.Bigint));
                adapter.InsertCommand.Parameters.Add(new NpgsqlParameter("@passport", NpgsqlDbType.Bigint));
                adapter.InsertCommand.Parameters.Add(new NpgsqlParameter("@adress", NpgsqlDbType.Text));
                adapter.InsertCommand.Parameters.Add(new NpgsqlParameter("@telephone", NpgsqlDbType.Text));
                adapter.InsertCommand.Parameters.Add(new NpgsqlParameter("@name", NpgsqlDbType.Text));
                adapter.InsertCommand.Parameters[0].SourceColumn = "club_id";
                adapter.InsertCommand.Parameters[1].SourceColumn = "passport";
                adapter.InsertCommand.Parameters[2].SourceColumn = "adress";
                adapter.InsertCommand.Parameters[3].SourceColumn = "telephone";
                adapter.InsertCommand.Parameters[4].SourceColumn = "name";
                adapter.Update(ds);
                
                ds.Clear();
                adapter.Fill(ds);
                MessageBox.Show("Данные синхронизированы!");
            }

        }

        public void button3_Click(object sender, EventArgs e)
        {
            // удаляем выделенные строки из dataGridView1
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
            }
        }
    }
}
