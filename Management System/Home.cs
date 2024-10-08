using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Microsoft.Reporting.WinForms;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Font = iTextSharp.text.Font;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Microsoft.ReportingServices.Diagnostics.Internal;
using PaperSource = CrystalDecisions.Shared.PaperSource;

namespace Management_System
{
    public partial class Home : Form
    {
        private static List<Stream> m_streams;
        private static int m_currentPageIndex = 0;
        public Home()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int invoiceId = 100;
            string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                for (int i = 0; i < this.dataGridView.Rows.Count; i++)
                {
                    int pruductId = Convert.ToInt32(dataGridView.Rows[i].Cells[0].Value);
                    string productName = dataGridView.Rows[i].Cells[1].Value.ToString();
                    double price = Convert.ToInt32(dataGridView.Rows[i].Cells[2].Value);
                    double qty = Convert.ToInt32(dataGridView.Rows[i].Cells[3].Value);
                    double total = Convert.ToInt32(dataGridView.Rows[i].Cells[4].Value);

                    string query = "INSERT INTO sales (invoiceId, itemId, itemName, price, quantity,total) VALUES (@invoiceId, @pruductId,@productName, @price, @qty, @total)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@invoiceId", invoiceId);
                        command.Parameters.AddWithValue("@pruductId", pruductId);
                        command.Parameters.AddWithValue("@productName", productName);
                        command.Parameters.AddWithValue("@price", price);
                        command.Parameters.AddWithValue("@qty", qty);
                        command.Parameters.AddWithValue("@total", total);
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Invoice created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                connection.Close();
            }
          
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Sales where invoiceId='" + 100 + "'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    DataTable data = new DataTable();
                    SqlDataReader reader = command.ExecuteReader();
                    data.Load(reader);
                    Print print = new Print();
                    string reportPath = @"CrystalReport1.rpt";
                    string fullpath=Path.Combine(Application.StartupPath, reportPath);
                    print.Reportname= fullpath;
                    print.Reportdata=data;
                    this.Hide();
                    print.Show();
                    //SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    //SalesDataSet salesDataSet = new SalesDataSet();
                    //dataAdapter.Fill(salesDataSet, "sales");
                    //ReportDataSource rds = new ReportDataSource("SalesDataSet", salesDataSet.Tables[0]);

                    //ReportDocument reportDocument = new ReportDocument();
                    //reportDocument.Load("F:\\.NET\\Management-System-.Net-Project\\Management System\\CrystalReport1.rpt");
                    //reportDocument.SetDataSource(salesDataSet);

                    //reportDocument.PrintOptions.PrinterName = "CITIZEN CBM1000 Type II";
                    //PrinterSettings printerSettings = new PrinterSettings();
                    //printerSettings.PrinterName = "CITIZEN CBM1000 Type II"; 

                    //reportDocument.PrintToPrinter(1, false, 0, 0);
                }
            }
        }
        public static Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            int invoiceId = 100;
            string pruductId = itemId.Text;
            string productName = itemName.Text;
            double price = double.Parse(itemPrice.Text);
            double qty = double.Parse(itemQty.Text);
            double total = qty * price;

            this.dataGridView.Rows.Add(pruductId, productName, price, qty, total);
            int sum = 0;
            for (int i = 0; i < this.dataGridView.Rows.Count; i++)
            {
                sum += Convert.ToInt32(dataGridView.Rows[i].Cells[4].Value);
            }
            itemTotal.Text = sum.ToString();
        }

        private void itemQty_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Payment_TextChanged(object sender, EventArgs e)
        {
            Balance.Text = (Convert.ToInt32(itemTotal.Text) - Convert.ToInt32(Payment.Text)).ToString();
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == dataGridView.Columns["Delete"].Index && e.RowIndex>=0)
            {
                dataGridView.Rows.Remove(dataGridView.Rows[e.RowIndex]);
            }
            int sum = 0;
            for (int i = 0; i < this.dataGridView.Rows.Count; i++)
            {
                sum += Convert.ToInt32(dataGridView.Rows[i].Cells[4].Value);
            }

            itemTotal.Text = sum.ToString();
        }
    }
}