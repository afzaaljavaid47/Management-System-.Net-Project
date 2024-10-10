using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Reporting.WinForms;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Management_System
{
    public partial class Invoices : Form
    {
        public Invoices()
        {
            InitializeComponent();
            progressBar1.Visible = false; // Hide the progress bar initially
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Invoices_Load(object sender, EventArgs e)
        {
            // Load data into the sales table
            this.salesTableAdapter.Fill(this.habibMedicalStoreDataSet.sales);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Print"].Index && e.RowIndex >= 0)
            {
                // Start the progress bar when the print process begins
                progressBar1.Visible = true;
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 30;

                Task.Run(() =>
                {
                    try
                    {
                        string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "SELECT * FROM Sales where invoiceId='" + dataGridView1.Rows[e.RowIndex].Cells[1].Value + "'";
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                string reportPath = @"CrystalReport1.rpt";
                                string fullpath = Path.Combine(Application.StartupPath, reportPath);

                                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                                SalesDataSet salesDataSet = new SalesDataSet();
                                dataAdapter.Fill(salesDataSet, "sales");

                                ReportDocument reportDocument = new ReportDocument();
                                reportDocument.Load(fullpath);
                                reportDocument.SetDataSource(salesDataSet);

                                string printerName = "CITIZEN CBM1000 Type II";
                                PrintDocument printDoc = new PrintDocument
                                {
                                    PrinterSettings = { PrinterName = printerName }
                                };

                                PaperSize customPaperSize = new PaperSize("Custom", 300, 600); // 3x6 inches
                                printDoc.DefaultPageSettings.PaperSize = customPaperSize;
                                printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0); // No margins for thermal receipt

                                reportDocument.PrintOptions.PrinterName = printerName;
                                reportDocument.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                                reportDocument.PrintToPrinter(1, false, 0, 0);
                            }
                        }

                        // After printing is complete
                        this.Invoke((MethodInvoker)delegate
                        {
                            progressBar1.Visible = false; // Hide the progress bar
                            MessageBox.Show("Invoice printed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            progressBar1.Visible = false; // Hide the progress bar
                            MessageBox.Show("Error printing the invoice: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
                    }
                });
            }
        }
    }
}
