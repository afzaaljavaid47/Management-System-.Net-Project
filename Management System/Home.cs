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
    public partial class Home : Form
    {
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

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pruductId = itemId.Text;
            string productName=itemName.Text;
            double price=double.Parse(itemPrice.Text);
            double qty = double.Parse(itemQty.Text);
            double total = qty * price;

            this.dataGridView.Rows.Add(pruductId, productName, price, qty, total);
            int sum = 0;
            for(int i = 0;i < this.dataGridView.Rows.Count; i++)
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
