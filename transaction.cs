using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/*----------------------------------------------
Author: SDK Support Group
Company: Paya
Contact: sdksupport@paya.com
!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
!!! Samples intended for educational use only!!!
!!!        Not intended for production       !!!
!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
-----------------------------------------------*/

class MainClass
{
    public static void Main(string[] args)
    {
        Trans().Wait();
    }

    static async Task Trans()
    {


        HttpClient client = new HttpClient();

        // TH - Test Data. This is the test account infomation we provide
        // in the API Sandbox. Please contact us at sdksupport@paya.com if
        // you need a unique test account and receive the Merchant ID and
        // Merchant Key. In order to get your own Client ID and Client Secret
        // you must register at https://developer.sagepayments.com and setup
        // an App under My Apps. Please let us know if you have any questions.
        var merchantId = "173859436515";
        var merchantKey = "P1J2V8P2Q3D8";

        // TH - The Client ID and Client Key should be hard coded and not displayed
        // in the production product. These are your API credentials used for
        // security and tracking purposes.
        var clientId = "W8yvKQ5XbvAn7dUDJeAnaWCEwA4yXEgd";//githu original
        var clientSecret = "iLzODV5AUsCGWGkr";//githu original

        // Build URL
        var query = "?type=";
        var queryType = "Sale";
        var url = "https://api-cert.sagepayments.com/bankcard/v1/charges" + query + queryType;
        
        // URL used to vault a card and create the token. Do not use with sale/authorization requests
        //var url = "https://api-cert.sagepayments.com/token/v1/tokens";

        // Build Timestamp and Nonce, I'm using the timestamp as the nonce here, but it's
        // recommended to use a separate unique value for the nonce.
        TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
        var timestamp = t.TotalSeconds.ToString();
        var nonce = timestamp.Split(',')[0];

        // Additional variables
        var verb = "POST";
        var contentType = "application/json";
        // TH - 20170304 - Added the equivalent of vbCrLf in vb.net
        var nl = Environment.NewLine;

        // Console output for debugging.
        Console.WriteLine("EXECUTING THE FOLLOWING:");
        Console.WriteLine(nl);
        Console.WriteLine("URL: " + url);
        Console.WriteLine(nl);
        Console.WriteLine("Verb: " + verb);
        Console.WriteLine(nl);
        Console.WriteLine("Timestamp: " + timestamp);
        Console.WriteLine(nl);

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // It is possible submit PCI-sensitive card data to our
        // RESTful Direct API. This will place your solution in-scope for PCI
        // You will be required to provide your PCI certification from an
        // Approved Scanning Vendor listed at the link below.
        // https://www.pcisecuritystandards.org/assessors_and_solutions/approved_scanning_vendors
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        // Use JSON request from file. There are 3 examples below.
        // 1. This will submit a sale using card data, return the result and a vault token.
        StreamReader sr = new StreamReader("sale.json");
        // 2. This will submit a vault token sale. No card data is submitted with this request.
        //StreamReader sr = new StreamReader("token_sale.json");
        // 3. This will vault a card and return a vault token. Only a luhn check is performed.
        // You will need to use the token URL above with this request.
        //StreamReader sr = new StreamReader("token_create.json");
        string request = sr.ReadToEnd();
        sr.Close();

        // TH - Build the Authorization
        string authToken = verb + url + request + merchantId + nonce + timestamp;
        byte[] hash_authToken = new HMACSHA512(Encoding.ASCII.GetBytes(clientSecret)).ComputeHash(Encoding.ASCII.GetBytes(authToken));
        string hash64_authToken = Convert.ToBase64String(hash_authToken);

        Console.WriteLine("Authorization: " + hash64_authToken);
        Console.WriteLine(nl);

        // Headers
        client.DefaultRequestHeaders.Add("clientId", clientId);
        client.DefaultRequestHeaders.Add("merchantId", merchantId);
        client.DefaultRequestHeaders.Add("merchantKey", merchantKey);
        client.DefaultRequestHeaders.Add("nonce", nonce);
        client.DefaultRequestHeaders.Add("timestamp", timestamp);
        client.DefaultRequestHeaders.TryAddWithoutValidation("authorization", hash64_authToken);
        
        
        // Construct an HttpContent from a StringContent
        HttpContent payload = new StringContent(request.ToString());
        // and add the header to this object instance
        // optional: add a formatter option to it as well
        payload.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        // Send the request.
        var response = await client.PostAsync(url, payload);
        Console.WriteLine("<<<<<<<<Connection Opened>>>>>>>>");
        Console.WriteLine(nl);

        // Gather the response and display
        // >>Note: I have not included any response logic or error handling.
        // >>This will need to be included for production implementations.
        var responseString = await response.Content.ReadAsStringAsync();
        var respStatDesc = response.StatusCode.ToString();
        var respStatCode = (int)response.StatusCode;
        Console.WriteLine("Response Status Desc: " + respStatDesc);
        Console.WriteLine(nl);
        Console.WriteLine("Response Status Code: " + respStatCode);
        Console.WriteLine(nl);
        Console.WriteLine("Response: " + responseString);
        Console.WriteLine(nl);

        response.Dispose();
        Console.WriteLine("<<<<<<<<Connection Closed>>>>>>>>");
        Console.WriteLine(nl);
        Console.WriteLine("Transaction Ended");
        Console.WriteLine(nl);
        Console.WriteLine("Press Enter to exit:");
        Console.ReadLine();
    }

}
