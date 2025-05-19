using Stripe;
using Stripe.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class StripeHelper
{
    public StripeHelper(string stripeApiKey)
    {
        StripeConfiguration.ApiKey = stripeApiKey;
    }

    // 1. Create a connection token
    public async Task<string> CreateConnectionTokenAsync()
    {
        var service = new ConnectionTokenService();
        var options = new ConnectionTokenCreateOptions();
        var token = await service.CreateAsync(options);
        return token.Secret;
    }

    // 2. List available readers in a location
    public async Task<List<Reader>> ListReadersAsync(string locationId)
    {
        var service = new ReaderService();
        var options = new ReaderListOptions
        {
            Location = locationId,
            Limit = 10
        };

        var readers = await service.ListAsync(options);
        return readers.Data;
    }

    // 3. Create a PaymentIntent
    public async Task<PaymentIntent> CreatePaymentIntentAsync(long amountInCents, string currency = "aud")
    {
        var service = new PaymentIntentService();
        var options = new PaymentIntentCreateOptions
        {
            Amount = amountInCents,
            Currency = currency,
            PaymentMethodTypes = new List<string> { "card_present" },
            CaptureMethod = "automatic"//"manual"
        };

        var intent = await service.CreateAsync(options);
        return intent;
    }

    // 4. Attach a PaymentIntent to a reader for collection
    public async Task<Reader> ProcessPaymentIntentAsync(string readerId, string paymentIntentId)
    {
        var service = new ReaderService();

        var processOptions = new ReaderProcessPaymentIntentOptions
        {
            PaymentIntent = paymentIntentId
        };

        return await service.ProcessPaymentIntentAsync(readerId, processOptions);
    }

    // 5. Capture a PaymentIntent (after customer confirms)
    public async Task<PaymentIntent> CapturePaymentIntentAsync(string paymentIntentId)
    {
        var service = new PaymentIntentService();
        return await service.CaptureAsync(paymentIntentId);
    }

    public async Task<PaymentIntent> UpdatePaymentIntentAsync(string paymentIntentId)
    {
        var service = new PaymentIntentService();

        var options = new PaymentIntentUpdateOptions
        {
            Metadata = new Dictionary<string, string> { { "status", "requires_capture" } },
            
        };

        return await service.UpdateAsync(paymentIntentId, options);
    }

    // 6. Cancel a PaymentIntent
    public async Task<PaymentIntent> CancelPaymentIntentAsync(string paymentIntentId)
    {
        var service = new PaymentIntentService();
        return await service.CancelAsync(paymentIntentId);
    }

    // 7. List Locations (optional, needed for reader discovery)
    public async Task<List<Location>> ListLocationsAsync()
    {
        var service = new LocationService();
        var locations = await service.ListAsync(new LocationListOptions { Limit = 10 });
        return locations.Data;
    }

    public async Task<PaymentIntent> GetPaymentIntentAsync(string paymentIntentId)
    {
        var service = new PaymentIntentService();
        return await service.GetAsync(paymentIntentId);
    }
}
