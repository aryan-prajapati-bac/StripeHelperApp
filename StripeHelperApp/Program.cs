using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Forms;
using System.IO.Pipes;
using System.IO;
using Newtonsoft.Json;

namespace StripeHelperApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        public static async Task MainAsync(string[] args)
        {
            //Testing Code with hard coded values
            //try
            //{
            //    var result = await TransactionManager.ProcessTransaction("200");
            //}
            //catch (Exception ex) 
            //{ 
            
            //}

            //Code with all changes
            while (true)
            {
                try
                {
                    using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("StripePipe", PipeDirection.InOut, 10))
                    {
                        Console.WriteLine("Waiting for connection...");
                        await pipeServer.WaitForConnectionAsync();

                        if (!pipeServer.IsConnected) continue;

                        using (StreamReader reader = new StreamReader(pipeServer))
                        {
                            using (StreamWriter writer = new StreamWriter(pipeServer) { AutoFlush = true })
                            {
                                while (pipeServer.IsConnected)
                                {
                                    string jsonRequest = await reader.ReadLineAsync();
                                    TransactionRequest request = JsonConvert.DeserializeObject<TransactionRequest>(jsonRequest);
                                    //Fetching the amount
                                    string amount = request.Amount;

                                    try
                                    {
                                        //Validating Amount Value
                                        if (string.IsNullOrWhiteSpace(amount))
                                        {
                                            writer.WriteLine(JsonConvert.SerializeObject(new { Success = false, Message = "Invalid input received" }));
                                            continue;
                                        }

                                        //Processing transaction
                                        var result = await TransactionManager.ProcessTransaction(amount);

                                        //Writing result to client 
                                        writer.WriteLine(JsonConvert.SerializeObject(result));

                                        //Logging the transaction details
                                        LogManager.LogTransaction(result);
                                    }

                                    catch (Exception ex)
                                    {
                                        writer.WriteLine(JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }));
                                    }

                                }
                            }
                        }


                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unexpected Error: " + ex.Message);
                }
            }
        }
    }
}
