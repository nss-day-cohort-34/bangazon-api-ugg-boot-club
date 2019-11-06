using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TestBangazonAPI
{
    public class TestPaymentType
    {
        
        [Fact]
        public async Task Test_Get_All_PaymentTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/paymenttype");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentTypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymentTypes.Count > 0);
            }

        }
        [Fact]
        public async Task Test_Get_Single_PaymentType()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var getJoe = await client.GetAsync("/api/paymenttype/1" +
                    "");
                getJoe.EnsureSuccessStatusCode();

                string getJoeBody = await getJoe.Content.ReadAsStringAsync();
                PaymentType newJoe = JsonConvert.DeserializeObject<PaymentType>(getJoeBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, getJoe.StatusCode);

            }
        }
        [Fact]
        public async Task Test_Post_PaymentType()
        {
            /*
                Generate a new instance of an HttpClient that you can
                use to generate HTTP requests to your API controllers.
                The `using` keyword will automatically dispose of this
                instance of HttpClient once your code is done executing.
            */
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                // Construct a new student object to be sent to the API
                PaymentType Mastercard = new PaymentType
                {
                    AcctNumber = "000333",
                    Name = "Mastercard",
                    CustomerId = 1
                };

                // Serialize the C# object into a JSON string
                var mastercardAsJSON = JsonConvert.SerializeObject(Mastercard);


                /*
                    ACT
                */

                // Use the client to send the request and store the response
                var response = await client.PostAsync(
                    "/api/paymenttype",
                    new StringContent(mastercardAsJSON, Encoding.UTF8, "application/json")
                );

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Animal
                var newMastercard = JsonConvert.DeserializeObject<PaymentType>(responseBody);


                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("000333", newMastercard.AcctNumber);
                Assert.Equal("Mastercard", newMastercard.Name);
                Assert.Equal(1, newMastercard.CustomerId);
            }
        }
        [Fact]
        public async Task Test_Modify_PaymentType()
        {
            // New last name to change to and test
            string newAcctNumber = "000111";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                PaymentType modifiedJoe = new PaymentType
                {
                    AcctNumber = newAcctNumber,
                    Name = "Amex",
                    CustomerId = 1,
                };
                var modifiedJoeAsJSON = JsonConvert.SerializeObject(modifiedJoe);

                var response = await client.PutAsync(
                    "/api/paymenttype/1",
                    new StringContent(modifiedJoeAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getJoe = await client.GetAsync("/api/paymenttype/1");
                getJoe.EnsureSuccessStatusCode();

                string getJoeBody = await getJoe.Content.ReadAsStringAsync();
                PaymentType newJoe = JsonConvert.DeserializeObject<PaymentType>(getJoeBody);

                Assert.Equal(HttpStatusCode.OK, getJoe.StatusCode);
                Assert.Equal(newAcctNumber, newJoe.AcctNumber);
            }
        }
        [Fact]
        public async Task Test_Delete_PaymentType()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                PaymentType Venmo = new PaymentType
                {
                    Id = -1,
                    Name = "Venmo"
                };

                // Serialize the C# object into a JSON string
                var venmoAsJSON = JsonConvert.SerializeObject(Venmo);


                /*
                    ACT
                */
                //Insert object
                var response = await client.PostAsync(
                    "/api/PaymentType",
                    new StringContent(venmoAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                var newType = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                //Get object
                var getVenmo = await client.GetAsync("/api/paymenttype/-1" +
                    "");
                getVenmo.EnsureSuccessStatusCode();

                string getVenmoBody = await getVenmo.Content.ReadAsStringAsync();
                PaymentType newVenmo = JsonConvert.DeserializeObject<PaymentType>(getVenmoBody);

                //Delete Object
                var deleteVenmo = await client.DeleteAsync("/api/paymenttype/-1" +
                    "");
                getVenmo.EnsureSuccessStatusCode();
                //Try to Get Object Again
                var attemptGetVenmo = await client.GetAsync("/api/paymenttype/-1" +
                    "");
                attemptGetVenmo.EnsureSuccessStatusCode();

                string attemptGetVenmoBody = await getVenmo.Content.ReadAsStringAsync();
                PaymentType newAttemptVenmo = JsonConvert.DeserializeObject<PaymentType>(attemptGetVenmoBody);


                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, attemptGetVenmo.StatusCode);

            }
        }
    }
}