using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StripeHelperApp
{
    class TransactionManager
    {
        public static async Task<PaymentIntent> ProcessTransaction(string amount) 
        {
            try
            {
                //Process the transaction
                var stripeSecretKey = ConfigurationSettings.AppSettings["stripeKey"].ToString();
                var stripeHelper = new StripeHelper(stripeSecretKey);

                Console.WriteLine("Creating Connection Token...");
                var token = await stripeHelper.CreateConnectionTokenAsync();

                var paymentIntent = await stripeHelper.CreatePaymentIntentAsync((long)Convert.ToDouble(amount)); // $50

                Console.WriteLine("PaymentIntent created: " + paymentIntent.Id);

                Console.WriteLine("Creating PaymentIntent...");
                var readerId = ConfigurationSettings.AppSettings["stripeReaderId"].ToString();

                Console.WriteLine("Sending PaymentIntent to reader " + readerId);
                var reader = await stripeHelper.ProcessPaymentIntentAsync(readerId, paymentIntent.Id);

                //var service = new Stripe.TestHelpers.Terminal.ReaderService();
                //Stripe.Terminal.Reader reader = service.PresentPaymentMethod(readerId);

                Console.WriteLine("Waiting for simulated customer to complete payment...");
                Console.WriteLine("Reader status: " + reader.Status);

                //while (reader.Status == "online") {
                //    var intent = stripeHelper.GetPaymentIntentAsync(paymentIntent.Id);
                //    Console.WriteLine("Intent Status: " + intent.Status);
                //}

                for (int i = 0; i < 10; i++) // Retry for ~10 seconds
                {
                    var intent = await stripeHelper.UpdatePaymentIntentAsync(paymentIntent.Id);
                    intent = await stripeHelper.GetPaymentIntentAsync(paymentIntent.Id);
                    if (intent.Status == "requires_capture")
                        break;
                    Console.WriteLine("Intent Status: " + intent.Status);

                    Thread.Sleep(1000);
                }
                //paymentIntent.Status = "requires_capture";
                Console.WriteLine("Reader status: " + reader.Status);
                Console.WriteLine("Press any key after the reader shows success (simulated)...");
                Console.ReadKey();

                var capturedIntent = await stripeHelper.CapturePaymentIntentAsync(paymentIntent.Id);
                Console.WriteLine("Payment captured! Status: " + capturedIntent.Status);

                return capturedIntent;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
