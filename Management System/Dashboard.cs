using CrystalDecisions.CrystalReports.Engine;
using Org.BouncyCastle.Asn1.X509;
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace Management_System
{
    public partial class Dashboard : Form
    {
        private string paymentMethod;
        public Dashboard()
        {
            InitializeComponent();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            stock.Text = "Current Stock";
            txtNetInvoiceTotal.SelectAll();
            txtNetInvoiceTotal.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'habibMedicalStoreDataSet.sales' table. You can move, or remove it, as needed.

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            txtCashReceived.Text = txtNetInvoiceTotal.Text;
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            if (ValidateFields())
            {
                string salesPerson=txtSalesPerson.Text;
                string customername=txtCustomerName.Text;
                DateTime dateTime=Convert.ToDateTime(txtDateTime.Text);
                string comments = txtComments.Text;
                string additionalComments = txtAdditionalComments.Text;
                bool isPrintReceipt = printReceiptCheckBox.Checked;
                int noOfCopies = Convert.ToInt32(txtNoOfCopied.Text);
                int grossTotal = Convert.ToInt32(txtGrossTotal.Text);
                float customerDiscount = float.Parse(txtCustomerDiscount.Text);
                int netGrossTotal = Convert.ToInt32(txtNetGrossTotal.Text);
                float receiptAdjustment = float.Parse(txtReceiptAdjustment.Text);
                float netInvoiceTotal = float.Parse(txtNetInvoiceTotal.Text);
                int cashreceived = Convert.ToInt32(txtCashReceived.Text);
                int returnChange = Convert.ToInt32(txtReturnChange.Text);
                int invoiceId = 0;

                string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    try
                    {
                        string insertInvoiceQuery = "INSERT INTO Invoice (salesPerson, customerName, dateTime,paymentMode,invoiceComments,invoiceAdditionalComments, isPrintReceipt, noOfCopies, invoiceGrandTotal, invoiceCustomerDiscount, netGrandTotal, invoiceAdjustment, invoiceNetTotal, invoiceCashReceive, invoiceReturnChange ) " +
                                                    "OUTPUT INSERTED.invoiceId " +
                                                    "VALUES (@salesPerson, @customername, @dateTime, @paymentMethod, @comments, @additionalComments,@isPrintReceipt,@noOfCopies,@grossTotal,@customerDiscount,@netGrossTotal,@receiptAdjustment,@netInvoiceTotal,@cashreceived,@returnChange)";

                        SqlCommand invoiceCommand = new SqlCommand(insertInvoiceQuery, connection, transaction);
                        invoiceCommand.Parameters.AddWithValue("@salesPerson", salesPerson);
                        invoiceCommand.Parameters.AddWithValue("@customername", customername);
                        invoiceCommand.Parameters.AddWithValue("@dateTime", dateTime);
                        invoiceCommand.Parameters.AddWithValue("@paymentMethod", paymentMethod);
                        invoiceCommand.Parameters.AddWithValue("@comments", comments);
                        invoiceCommand.Parameters.AddWithValue("@additionalComments", additionalComments);
                        invoiceCommand.Parameters.AddWithValue("@isPrintReceipt", isPrintReceipt);
                        invoiceCommand.Parameters.AddWithValue("@noOfCopies", noOfCopies);
                        invoiceCommand.Parameters.AddWithValue("@grossTotal", grossTotal);  
                        invoiceCommand.Parameters.AddWithValue("@customerDiscount", customerDiscount);
                        invoiceCommand.Parameters.AddWithValue("@netGrossTotal", netGrossTotal);
                        invoiceCommand.Parameters.AddWithValue("@receiptAdjustment", receiptAdjustment);
                        invoiceCommand.Parameters.AddWithValue("@netInvoiceTotal", netInvoiceTotal);
                        invoiceCommand.Parameters.AddWithValue("@cashreceived", cashreceived);
                        invoiceCommand.Parameters.AddWithValue("@returnChange", returnChange);

                        invoiceId = (int)invoiceCommand.ExecuteScalar();

                        string insertDetailsQuery = "INSERT INTO InvoiceItems (invoiceId, productName, quantity, productRate,productDiscount, productTotal) " +
                                                    "VALUES (@invoiceId, @productName, @quantity, @productRate, @productDiscount, @productTotal)";

                        SqlCommand detailsCommand = new SqlCommand(insertDetailsQuery, connection, transaction);

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                detailsCommand.Parameters.Clear();

                                detailsCommand.Parameters.AddWithValue("@invoiceId", invoiceId);
                                detailsCommand.Parameters.AddWithValue("@productName", row.Cells["Product_Name"].Value);
                                detailsCommand.Parameters.AddWithValue("@quantity", row.Cells["Qty"].Value);
                                detailsCommand.Parameters.AddWithValue("@productRate", row.Cells["Rate"].Value);
                                detailsCommand.Parameters.AddWithValue("@productDiscount", row.Cells["Discount"].Value);
                                detailsCommand.Parameters.AddWithValue("@productTotal", Convert.ToInt32(row.Cells["Total"].Value)- Convert.ToInt32(row.Cells["Discount"].Value));
                                detailsCommand.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                        MessageBox.Show("Invoice saved successfully!");
                        
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error: " + ex.Message);
                    }

                    string query = "SELECT  * FROM Invoice I INNER JOIN InvoiceItems II ON I.invoiceId = II.invoiceId WHERE I.invoiceId = '" + invoiceId + "'";
                    string query1 = "SELECT * FROM Invoice where invoiceId='" + invoiceId + "'";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        string fullpath = Path.Combine("F:\\.NET\\Management-System-.Net-Project\\Management System\\CrystalReport3.rpt");

                        ReportDocument reportDocument = new ReportDocument();
                        reportDocument.Load(fullpath);
                        DataSet dataSet = GetInvoiceData(connectionString, invoiceId);
                        reportDocument.SetDataSource(dataSet);

                        string printerName = "CITIZEN CBM1000 Type II";
                        PrintDocument printDoc = new PrintDocument
                        {
                            PrinterSettings = { PrinterName = printerName }
                        };

                        PaperSize customPaperSize = new PaperSize("Custom", 300, 600);
                        printDoc.DefaultPageSettings.PaperSize = customPaperSize;
                        printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

                        reportDocument.PrintOptions.PrinterName = printerName;
                        reportDocument.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                        reportDocument.PrintToPrinter(1, false, 0, 0);
                    }
                    connection.Close();
                    ResetForm();
                }
            }
        }

        public DataSet GetInvoiceData(string connectionString, int invoiceId)
        {
            string invoiceQuery = "SELECT * FROM Invoice WHERE invoiceId = @InvoiceID";
            string invoiceItemsQuery = "SELECT * FROM InvoiceItems WHERE invoiceId = @InvoiceID";
            DataSet dataSet = new DataSet();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand invoiceCommand = new SqlCommand(invoiceQuery, connection))
                using (SqlCommand invoiceItemsCommand = new SqlCommand(invoiceItemsQuery, connection))
                {
                    invoiceCommand.Parameters.AddWithValue("@InvoiceID", invoiceId);
                    invoiceItemsCommand.Parameters.AddWithValue("@InvoiceID", invoiceId);

                    SqlDataAdapter invoiceAdapter = new SqlDataAdapter(invoiceCommand);
                    SqlDataAdapter invoiceItemsAdapter = new SqlDataAdapter(invoiceItemsCommand);

                    invoiceAdapter.Fill(dataSet, "Invoice");

                    invoiceItemsAdapter.Fill(dataSet, "InvoiceItems");
                }
            }
            return dataSet;
        }


        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtSalesPerson.Text))
            {
                MessageBox.Show("Salesperson is required.");
                txtSalesPerson.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Customer name is required.");
                txtCustomerName.Focus();
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Quantity must be a positive integer.");
                txtQuantity.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtProductSKU.Text))
            {
                MessageBox.Show("Customer name is required.");
                txtProductSKU.Focus();
                return false;
            }

            if (!DateTime.TryParse(txtDateTime.Text, out DateTime dateTime))
            {
                MessageBox.Show("Invalid date format.");
                txtDateTime.Focus();
                return false;
            }

            if (!bool.TryParse(printReceiptCheckBox.Checked.ToString(), out bool isPrintReceipt))
            {
                MessageBox.Show("Invalid value for 'Print Receipt'.");
                return false;
            }
            int noOfCopies = 0; // Default initialization

            if (string.IsNullOrWhiteSpace(txtNoOfCopied.Text) ||
                (!int.TryParse(txtNoOfCopied.Text, out noOfCopies) || noOfCopies <= 0))
            {
                MessageBox.Show("Number of copies must be a positive integer.");
                txtNoOfCopied.Focus();
                return false;
            }

            if (!decimal.TryParse(txtGrossTotal.Text, out decimal invoiceGrandTotal) || invoiceGrandTotal <= 0)
            {
                MessageBox.Show("Grand total must be a positive number.");
                txtGrossTotal.Focus();
                return false;
            }

            if (!decimal.TryParse(txtCustomerDiscount.Text, out decimal invoiceCustomerDiscount) || invoiceCustomerDiscount < 0)
            {
                MessageBox.Show("Customer discount must be a non-negative number.");
                txtCustomerDiscount.Focus();
                return false;
            }

            if (!decimal.TryParse(txtNetGrossTotal.Text, out decimal netGrandTotal) || netGrandTotal <= 0)
            {
                MessageBox.Show("Net grand total must be a positive number.");
                txtNetGrossTotal.Focus();
                return false;
            }

            if (!decimal.TryParse(txtReceiptAdjustment.Text, out decimal invoiceAdjustment) || invoiceAdjustment < 0)
            {
                MessageBox.Show("Adjustment must be a non-negative number.");
                txtReceiptAdjustment.Focus();
                return false;
            }

            if (!decimal.TryParse(txtNetInvoiceTotal.Text, out decimal invoiceNetTotal) || invoiceNetTotal <= 0)
            {
                MessageBox.Show("Invoice net total must be a positive number.");
                txtNetInvoiceTotal.Focus();
                return false;
            }

            if (!decimal.TryParse(txtCashReceived.Text, out decimal invoiceCashReceive) || invoiceCashReceive <= 0)
            {
                MessageBox.Show("Cash received must be a positive number.");
                txtCashReceived.Focus();
                return false;
            }

            if (!decimal.TryParse(txtReturnChange.Text, out decimal invoiceReturnChange) || invoiceReturnChange < 0)
            {
                MessageBox.Show("Return change must be a non-negative number.");
                txtReturnChange.Focus();
                return false;
            }

            if (radioCashButton.Checked)
            {
                paymentMethod = "Cash";
            }
            else if (radioCreditButton.Checked)
            {
                paymentMethod = "Credit";
            }
            else if (radioCashCreditButton.Checked)
            {
                paymentMethod = "Cash Credit";
            }
            else if (radioCreditDebitButton.Checked)
            {
                paymentMethod = "Credit Debit";
            }

            if (radioCashButton.Checked)
            {
                if (!decimal.TryParse(txtCashReceived.Text, out decimal cashReceived) || cashReceived <= 0)
                {
                    MessageBox.Show("Cash received must be a positive number when paying by cash.");
                    txtCashReceived.Focus();
                    return false;
                }
            }

            return true;
        }

        private void Dashboard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                if (ValidateFields())
                {
                    string salesPerson = txtSalesPerson.Text;
                    string customername = txtCustomerName.Text;
                    DateTime dateTime = Convert.ToDateTime(txtDateTime.Text);
                    string comments = txtComments.Text;
                    string additionalComments = txtAdditionalComments.Text;
                    bool isPrintReceipt = printReceiptCheckBox.Checked;
                    int noOfCopies = Convert.ToInt32(txtNoOfCopied.Text);
                    int grossTotal = Convert.ToInt32(txtGrossTotal.Text);
                    float customerDiscount = float.Parse(txtCustomerDiscount.Text);
                    int netGrossTotal = Convert.ToInt32(txtNetGrossTotal.Text);
                    float receiptAdjustment = float.Parse(txtReceiptAdjustment.Text);
                    float netInvoiceTotal = float.Parse(txtNetInvoiceTotal.Text);
                    int cashreceived = Convert.ToInt32(txtCashReceived.Text);
                    int returnChange = Convert.ToInt32(txtReturnChange.Text);
                    int invoiceId = 0;

                    string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();
                        try
                        {
                            string insertInvoiceQuery = "INSERT INTO Invoice (salesPerson, customerName, dateTime,paymentMode,invoiceComments,invoiceAdditionalComments, isPrintReceipt, noOfCopies, invoiceGrandTotal, invoiceCustomerDiscount, netGrandTotal, invoiceAdjustment, invoiceNetTotal, invoiceCashReceive, invoiceReturnChange ) " +
                                                        "OUTPUT INSERTED.invoiceId " +
                                                        "VALUES (@salesPerson, @customername, @dateTime, @paymentMethod, @comments, @additionalComments,@isPrintReceipt,@noOfCopies,@grossTotal,@customerDiscount,@netGrossTotal,@receiptAdjustment,@netInvoiceTotal,@cashreceived,@returnChange)";

                            SqlCommand invoiceCommand = new SqlCommand(insertInvoiceQuery, connection, transaction);
                            invoiceCommand.Parameters.AddWithValue("@salesPerson", salesPerson);
                            invoiceCommand.Parameters.AddWithValue("@customername", customername);
                            invoiceCommand.Parameters.AddWithValue("@dateTime", dateTime);
                            invoiceCommand.Parameters.AddWithValue("@paymentMethod", paymentMethod);
                            invoiceCommand.Parameters.AddWithValue("@comments", comments);
                            invoiceCommand.Parameters.AddWithValue("@additionalComments", additionalComments);
                            invoiceCommand.Parameters.AddWithValue("@isPrintReceipt", isPrintReceipt);
                            invoiceCommand.Parameters.AddWithValue("@noOfCopies", noOfCopies);
                            invoiceCommand.Parameters.AddWithValue("@grossTotal", grossTotal);
                            invoiceCommand.Parameters.AddWithValue("@customerDiscount", customerDiscount);
                            invoiceCommand.Parameters.AddWithValue("@netGrossTotal", netGrossTotal);
                            invoiceCommand.Parameters.AddWithValue("@receiptAdjustment", receiptAdjustment);
                            invoiceCommand.Parameters.AddWithValue("@netInvoiceTotal", netInvoiceTotal);
                            invoiceCommand.Parameters.AddWithValue("@cashreceived", cashreceived);
                            invoiceCommand.Parameters.AddWithValue("@returnChange", returnChange);

                            invoiceId = (int)invoiceCommand.ExecuteScalar();

                            string insertDetailsQuery = "INSERT INTO InvoiceItems (invoiceId, productName, quantity, productRate,productDiscount, productTotal) " +
                                                        "VALUES (@invoiceId, @productName, @quantity, @productRate, @productDiscount, @productTotal)";

                            SqlCommand detailsCommand = new SqlCommand(insertDetailsQuery, connection, transaction);

                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (!row.IsNewRow)
                                {
                                    detailsCommand.Parameters.Clear();

                                    detailsCommand.Parameters.AddWithValue("@invoiceId", invoiceId);
                                    detailsCommand.Parameters.AddWithValue("@productName", row.Cells["Product_Name"].Value);
                                    detailsCommand.Parameters.AddWithValue("@quantity", row.Cells["Qty"].Value);
                                    detailsCommand.Parameters.AddWithValue("@productRate", row.Cells["Rate"].Value);
                                    detailsCommand.Parameters.AddWithValue("@productDiscount", row.Cells["Discount"].Value);
                                    detailsCommand.Parameters.AddWithValue("@productTotal", row.Cells["Total"].Value);
                                    detailsCommand.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                            MessageBox.Show("Invoice saved successfully!");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Error: " + ex.Message);
                        }

                        string query = "SELECT  * FROM Invoice I INNER JOIN InvoiceItems II ON I.invoiceId = II.invoiceId WHERE I.invoiceId = '" + invoiceId + "'";
                        string query1 = "SELECT * FROM Invoice where invoiceId='" + invoiceId + "'";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            string fullpath = Path.Combine("F:\\.NET\\Management-System-.Net-Project\\Management System\\CrystalReport3.rpt");

                            ReportDocument reportDocument = new ReportDocument();
                            reportDocument.Load(fullpath);
                            DataSet dataSet = GetInvoiceData(connectionString, invoiceId);
                            reportDocument.SetDataSource(dataSet);

                            string printerName = "CITIZEN CBM1000 Type II";
                            PrintDocument printDoc = new PrintDocument
                            {
                                PrinterSettings = { PrinterName = printerName }
                            };

                            PaperSize customPaperSize = new PaperSize("Custom", 300, 600);
                            printDoc.DefaultPageSettings.PaperSize = customPaperSize;
                            printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

                            reportDocument.PrintOptions.PrinterName = printerName;
                            reportDocument.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                            reportDocument.PrintToPrinter(1, false, 0, 0);
                        }
                        connection.Close();
                        ResetForm();
                    }
                }
                e.SuppressKeyPress = true;
            }
        }

        private void Dashboard_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void ResetForm()
        {
            dataGridView1.Rows.Clear();
            txtProductSKU.Text = String.Empty;
            txtComments.Text = String.Empty;
            txtAdditionalComments.Text = String.Empty;
            txtGrossTotal.Text = "0";
            txtCustomerDiscount.Text = "0.0";
            txtNetGrossTotal.Text = "0.0";
            txtReceiptAdjustment.Text = "0.0";
            txtNetInvoiceTotal.Text = "0.0";
            txtCashReceived.Text = "0.0";
            txtReturnChange.Text = "0";
            txtCustomerName.Text = String.Empty;
            txtCustomerName.Focus();
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtProductSKU_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtProductSKU.Text))
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT productCode, productName, productRate, productStock FROM Product WHERE productCode = @productCode";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@productCode", txtProductSKU.Text);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows && !string.IsNullOrWhiteSpace(txtProductSKU.Text))
                            {
                                while (reader.Read())
                                {
                                    string productCode = reader["productCode"].ToString();
                                    string productName = reader["productName"].ToString();
                                    decimal productRate = Convert.ToDecimal(reader["productRate"]);
                                    int productStock = Convert.ToInt32(reader["productStock"]);
                                    dataGridView1.Rows.Add(dataGridView1.RowCount + 1, productCode, productName, productRate, txtQuantity.Text, 0, productRate * Convert.ToInt32(txtQuantity.Text));
                                    int sum = 0;
                                    for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                                    {
                                        sum += Convert.ToInt32(dataGridView1.Rows[i].Cells[6].Value);
                                    }

                                    txtNetInvoiceTotal.Text = sum.ToString();
                                    txtGrossTotal.Text = sum.ToString();
                                    txtNetGrossTotal.Text = sum.ToString();
                                }
                            }
                            else
                            {
                                MessageBox.Show("No product found with the specified Product Code.");
                            }
                        }
                    }
                }
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Product product=new Product();
            product.ShowDialog();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Delete"].Index && e.RowIndex >= 0)
            {
                dataGridView1.Rows.Remove(dataGridView1.Rows[e.RowIndex]);
            }

            if (e.ColumnIndex == 4)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                decimal productRate = Convert.ToDecimal(row.Cells["Rate"].Value);
                int updatedQuantity = Convert.ToInt32(row.Cells["Qty"].Value);
                decimal newTotal = productRate * updatedQuantity;
                row.Cells["Total"].Value = newTotal;
            }
            int sum = 0;
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                sum += Convert.ToInt32(dataGridView1.Rows[i].Cells[6].Value);
            }
            txtNetInvoiceTotal.Text = sum.ToString();
            txtGrossTotal.Text = sum.ToString();
        }
        private void dataGridViewProducts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.ColumnIndex == 4)
                {
                    // Get the current row from the DataGridView
                    DataGridViewRow currentRow = dataGridView1.CurrentRow;

                    if (currentRow != null)
                    {
                        DataGridViewRow row = dataGridView1.Rows[Convert.ToInt32(currentRow)];
                        decimal productRate = Convert.ToDecimal(row.Cells["Rate"].Value);
                        int updatedQuantity = Convert.ToInt32(row.Cells["Qty"].Value);
                        decimal newTotal = productRate * updatedQuantity;
                        row.Cells["Total"].Value = newTotal;
                        int sum = 0;
                        for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                        {
                            sum += Convert.ToInt32(dataGridView1.Rows[i].Cells[6].Value);
                        }
                        txtNetInvoiceTotal.Text = sum.ToString();
                        txtGrossTotal.Text = sum.ToString();
                        e.Handled = true;
                    }
                }
            }
        }
        private void txtCustomerDiscount_TextChanged(object sender, EventArgs e)
        {
            int grossTotal = 0;
            int customerDiscount = 0;

            if (!int.TryParse(txtGrossTotal.Text, out grossTotal))
            {
                grossTotal = 0;
            }

            if (!int.TryParse(txtCustomerDiscount.Text, out customerDiscount))
            {
                customerDiscount = 0;
            }

            int netGrossTotal = grossTotal - customerDiscount;

            txtNetGrossTotal.Text = netGrossTotal.ToString();
            txtNetInvoiceTotal.Text = txtNetGrossTotal.Text;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            SalesInvoices invoices= new SalesInvoices();
            invoices.ShowDialog();
        }

        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtReceiptAdjustment_TextChanged(object sender, EventArgs e)
        {
            int grossTotal = 0;
            int receiptAdjustment = 0;

            if (!int.TryParse(txtNetGrossTotal.Text, out grossTotal))
            {
                grossTotal = 0;
            }

            if (!int.TryParse(txtReceiptAdjustment.Text, out receiptAdjustment))
            {
                receiptAdjustment = 0;
            }

            int netGrossTotal = grossTotal - receiptAdjustment;

            txtNetGrossTotal.Text = netGrossTotal.ToString();
            txtNetInvoiceTotal.Text = netGrossTotal.ToString();
        }
    }
}
