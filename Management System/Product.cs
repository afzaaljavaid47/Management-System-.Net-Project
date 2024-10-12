using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Management_System
{
    public partial class Product : Form
    {
        public Product()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string productCode=textBox1.Text.ToString();
                string productName=textBox2.Text.ToString();
                string productRate=textBox3.Text.ToString();
                string productStock=textBox4.Text.ToString();

                connection.Open();
                    string query = "INSERT INTO product (productCode, productName, productRate, productStock) VALUES (@productCode, @productName, @productRate,@productStock)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@productCode", productCode);
                        command.Parameters.AddWithValue("@productName", productName);
                        command.Parameters.AddWithValue("@productRate", productRate);
                        command.Parameters.AddWithValue("@productStock", productStock);
                        command.ExecuteNonQuery();
                    }
                MessageBox.Show("Product created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox1.Text = String.Empty;
                textBox2.Text = String.Empty;
                textBox3.Text = String.Empty;
                textBox4.Text = String.Empty;
                textBox1.Focus();
                connection.Close();
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
