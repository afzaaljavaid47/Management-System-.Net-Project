using CrystalDecisions.CrystalReports.Engine;
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

namespace Management_System
{
    public partial class Print : Form
    {
        private static List<Stream> m_streams;
        private static int m_currentPageIndex = 0;
        public Print()
        {
            InitializeComponent();
        }

        public string Reportname { get; set; }
        public DataTable Reportdata { get; set; }
        private void printPreviewControl1_Click(object sender, EventArgs e)
        {
            

        }

        private void Print_Load(object sender, EventArgs e)
        {
           
        }
        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            ReportDocument rdd = new ReportDocument();
            rdd.Load(Reportname);
            rdd.SetDataSource(Reportdata);
            crystalReportViewer1.ReportSource = rdd;
        }
    }
}
