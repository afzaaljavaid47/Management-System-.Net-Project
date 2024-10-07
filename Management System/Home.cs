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
            //this.Hide();
            //Print p = new Print();
            //p.Show();

            //string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Sales where invoiceId='" + 100 + "'";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    SalesDataSet salesDataSet = new SalesDataSet();
                    dataAdapter.Fill(salesDataSet, "SalesDataTable");
                    ReportDataSource rds = new ReportDataSource("SalesDataSet", salesDataSet.Tables[0]);

                    LocalReport report = new LocalReport();
                    string path = Path.GetDirectoryName(Application.ExecutablePath);
                    string fullPath = Path.GetDirectoryName(Application.ExecutablePath).Remove(path.Length - 10) + @"\Report1.rdlc";
                    report.ReportPath = fullPath;
                    report.DataSources.Add(new ReportDataSource("SalesDataSet", salesDataSet.Tables[0]));
                    PrintToPrinter(report);

                    Document doc = new Document(new iTextSharp.text.Rectangle(3 * 72, 6 * 72)); // 1 inch = 72 points
                    string paths = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Invoice_{invoiceId}.pdf");

                    PdfWriter.GetInstance(doc, new FileStream(paths, FileMode.Create));
                    doc.Open();

                    // Add Title
                    iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                    Paragraph title = new Paragraph("Invoice", titleFont) { Alignment = Element.ALIGN_CENTER };
                    doc.Add(title);

                    doc.Add(new Paragraph("\n")); // Add some space

                    // Add Invoice Details
                    iTextSharp.text.Font detailFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);


                    Paragraph invoiceDetails = new Paragraph
                    {
                        Alignment = Element.ALIGN_LEFT
                    };
                    invoiceDetails.Add(new Chunk($"Invoice ID: {invoiceId}  ", detailFont)); // Add Invoice ID
                    invoiceDetails.Add(new Chunk($"Customer Name: Afzaal", detailFont)); // Add Customer Name

                    doc.Add(invoiceDetails);
                    doc.Add(new Paragraph($"Date: {DateTime.Now.ToString("yyyy-MM-dd")}", detailFont));

                    doc.Add(new Paragraph("\n")); // Add some space




                    doc.Add(new Paragraph($"Date: {DateTime.Now.ToString("yyyy-MM-dd")}", detailFont));

                    doc.Add(new Paragraph("\n")); // Add some space

                    // Add Items Table
                    PdfPTable table = new PdfPTable(3); // 3 columns
                    table.WidthPercentage = 100;
                    table.AddCell("Item Description");
                    table.AddCell("Quantity");
                    table.AddCell("Price");

                    // Example items - replace with actual items
                    for (int i = 1; i <= 5; i++)
                    {
                        table.AddCell($"Item {i}");
                        table.AddCell("1"); // Quantity
                        table.AddCell($"${10 * i}"); // Price
                    }

                    doc.Add(table);

                    doc.Add(new Paragraph("\n")); // Add some space

                    // Add Footer
                    iTextSharp.text.Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    Paragraph footer = new Paragraph("Thank you for your business!", footerFont)
                    {
                        Alignment = Element.ALIGN_CENTER
                    };
                    doc.Add(footer);

                    // Close the document
                    doc.Close();
                    MessageBox.Show("Invoice generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);



                }
            }
        }
        public static void PrintToPrinter(LocalReport report)
        {
            Export(report);

        }
        public void GenerateFastReport(string invoiceId, string customerName, DateTime invoiceDate)
        {
            // Create a new report instance
            Report report = new Report();

            // Create a DataTable to hold the report data
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemDescription");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("Price");

            // Add example data - replace this with actual data
            for (int i = 1; i <= 5; i++)
            {
                dt.Rows.Add($"Item {i}", 1, 10 * i); // Example data
            }

            // Register the DataTable as a data source
            report.RegisterData(dt, "InvoiceItems");

            // Load a report template (optional)
            // report.Load("PathToYourReportTemplate.frx");

            // Set the report title and invoice details
            report.SetParameterValue("InvoiceId", invoiceId);
            report.SetParameterValue("CustomerName", customerName);
            report.SetParameterValue("InvoiceDate", invoiceDate.ToString("yyyy-MM-dd"));

            // Create a report page
            var page = report.Pages[0];

            // Create report layout
            report.Prepare();

            // Show the report in a viewer
            var reportViewer = new ReportViewer();
            reportViewer.Report = report;
            reportViewer.ShowDialog();
        }
        public static void Export(LocalReport report, bool print = true)
        {
            string deviceInfo =
             @"<DeviceInfo>
        <OutputFormat>EMF</OutputFormat>
        <PageWidth>3in</PageWidth>
        <PageHeight>6in</PageHeight>
        <MarginTop>0in</MarginTop>
        <MarginLeft>0.1in</MarginLeft>
        <MarginRight>0.1in</MarginRight>
        <MarginBottom>0in</MarginBottom>
    </DeviceInfo>";

            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);

            foreach (Stream stream in m_streams)
                stream.Position = 0;

            if (print)
            {
                PrintPrinter();
            }
        }
        public static Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        public static void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            System.Drawing.Rectangle adjustedRect = new System.Drawing.Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }
        public static void PrintPrinter()
        {
            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                m_currentPageIndex = 0;
                printDoc.Print();
            }
        }
        public static void DisposePrint()
        {
            if (m_streams != null)
            {
                foreach (Stream stream in m_streams)
                    stream.Close();
                m_streams = null;
            }
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
