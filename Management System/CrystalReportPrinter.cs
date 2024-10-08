using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Windows.Forms;

public class CrystalReportPrinter
{
    public void PrintReport(string reportPath, string printerName)
    {
        try
        {
            // Load the report
            ReportDocument reportDocument = new ReportDocument();
            reportDocument.Load(reportPath);

            // Optional: Set database login info if required
            // reportDocument.SetDatabaseLogon("username", "password", "server", "database");

            // Set the printer name (can be a network or local printer)
            reportDocument.PrintOptions.PrinterName = printerName;

            // Print the report without displaying it
            reportDocument.PrintToPrinter(1, false, 0, 0);

            // Dispose of the report document to free resources
            reportDocument.Dispose();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error while printing: " + ex.Message);
        }
    }
}
