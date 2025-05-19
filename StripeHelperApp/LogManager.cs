using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripeHelperApp
{
    class LogManager
    {
        
        public static void LogTransaction(PaymentIntent resultIntent)
        {
            try
            {
                // Define log directory (one level outside the project folder)
                //string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Logs", "Transaction Logs");
                //string fullPath = Path.GetFullPath(logDirectory); // Resolves relative path to absolute
                string fullPath = ConfigurationSettings.AppSettings["LogsPath"].ToString();

                // Ensure directory exists
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                // Generate log file name based on the current date
                string logFileName = "TransactionLog_" + System.DateTime.UtcNow.ToString("yyyyMMdd") + ".txt";
                string logPath = Path.Combine(fullPath, logFileName);

                // Check if the file exists
                bool fileExists = System.IO.File.Exists(logPath);

                using (StreamWriter writer = new StreamWriter(logPath, true))
                {
                    // Write header only if file does not exist or is empty
                    if (!fileExists || new FileInfo(logPath).Length == 0)
                    {
                        writer.WriteLine("Time                 || Transaction ID   || Order ID  || Amount  || Transaction Type  || Card Type  || Success || Code  || Message               ");
                        writer.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------");
                    }

                    // Build log entry string using StringBuilder
                    var logEntry = new StringBuilder();
                    //logEntry.AppendFormat("{0,-20} || {1,-16} || {2,-8} || {3,-7} || {4,-l20} || {5,-20} || {6,-7} || {7,-5} || {8,-20}",
                    //    resultIntent.dat, resultIntent.Id ?? "N/A", resultIntent.SourceId ?? "N/A",
                    //    resultIntent.Amount, resultIntent.PaymentMethodTypes ?? "Unknown", resultIntent. ?? "N/A",
                    //    resultIntent. ? "Yes" : "No", resultIntent.Code ?? "N/A", resultIntent.Message ?? "N/A");

                    // Write log entry
                    writer.WriteLine(logEntry.ToString());
                }

                //Console.WriteLine("Transaction logged successfully.");
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error logging transaction: " + ex.Message);
            }
        }
    }
}
