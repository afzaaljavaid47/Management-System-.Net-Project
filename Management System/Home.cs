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
using System;
using System.Drawing.Printing;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Random random = new Random();
            int randomNumber = random.Next(1525, 100000);
            int invoiceId = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                for (int i = 0; i < this.dataGridView.Rows.Count; i++)
                {
                    invoiceId=randomNumber;
                    int pruductId = Convert.ToInt32(dataGridView.Rows[i].Cells[0].Value);
                    string productName = dataGridView.Rows[i].Cells[1].Value.ToString();
                    double price = Convert.ToInt32(dataGridView.Rows[i].Cells[2].Value);
                    double qty = Convert.ToInt32(dataGridView.Rows[i].Cells[3].Value);
                    double total = Convert.ToInt32(dataGridView.Rows[i].Cells[4].Value);
                    string customerName= textCustomerName.Text.ToString();

                    string query = "INSERT INTO sales (invoiceId, customerName, itemId, itemName, price, quantity,total) VALUES (@invoiceId, @customerName, @pruductId,@productName, @price, @qty, @total)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@invoiceId", invoiceId);
                        command.Parameters.AddWithValue("@customerName", customerName);
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
                string query = "SELECT * FROM Sales where invoiceId='" + invoiceId + "'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //DataTable data = new DataTable();
                    //SqlDataReader reader = command.ExecuteReader();
                    //data.Load(reader);
                    //Print print = new Print();
                    //string reportPath = @"CrystalReport1.rpt";
                    //string fullpath=Path.Combine(Application.StartupPath, reportPath);
                    //print.Reportname= fullpath;
                    //print.Reportdata=data;
                    //this.Hide();
                    //print.Show();

                    string reportPath = @"CrystalReport1.rpt";
                    string fullpath = Path.Combine(Application.StartupPath, reportPath);

                    //DataTable data = new DataTable();
                    //SqlDataReader reader = command.ExecuteReader();
                    //data.Load(reader);
                    //Print print = new Print();
                    //print.Reportname = fullpath;
                    //print.Reportdata = data;
                    //print.Show();


                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    SalesDataSet salesDataSet = new SalesDataSet();
                    dataAdapter.Fill(salesDataSet, "sales");
                    ReportDocument reportDocument = new ReportDocument();
                    reportDocument.Load(fullpath);
                    reportDocument.SetDataSource(salesDataSet);
                    string printerName = "CITIZEN CBM1000 Type II";
                    PrintDocument printDoc = new PrintDocument();
                    printDoc.PrinterSettings.PrinterName = printerName;
                    System.Drawing.Printing.PaperSize customPaperSize = new System.Drawing.Printing.PaperSize("Custom", 300, 600);
                    printDoc.DefaultPageSettings.PaperSize = customPaperSize;
                    printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                    reportDocument.PrintOptions.PrinterName = printerName;
                    reportDocument.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                    reportDocument.PrintToPrinter(1, false, 0, 0);
                    MessageBox.Show("Invoice printed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            string pruductId = itemId.Text;
            string productName = itemName.Text;
            double price = double.Parse(itemPrice.Text);
            double qty = double.Parse(itemQty.Text);
            double total = qty * price;

            this.dataGridView.Rows.Add(pruductId, productName, price, qty, total);
            int sum = 0;
            for (int i = 0; i < this.dataGridView.Rows.Count; i++)
            {
                button2.Enabled = true;
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

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Home_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Invoices invoices = new Invoices();
            this.Hide();
            invoices.Show();
        }
    }
}