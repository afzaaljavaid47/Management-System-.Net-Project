using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Management_System
{
    public partial class SalesInvoices : Form
    {
        public SalesInvoices()
        {
            InitializeComponent();
        }

        private void SalesInvoices_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'invoiceDataSet.Invoice' table. You can move, or remove it, as needed.
            this.invoiceTableAdapter.Fill(this.invoiceDataSet.Invoice);
            // TODO: This line of code loads data into the 'invoiceDataSet.InvoiceItems' table. You can move, or remove it, as needed.
            this.invoiceItemsTableAdapter.Fill(this.invoiceDataSet.InvoiceItems);
            // TODO: This line of code loads data into the 'habibMedicalStoreDataSet.sales' table. You can move, or remove it, as needed.
            this.salesTableAdapter.Fill(this.habibMedicalStoreDataSet.sales);

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
