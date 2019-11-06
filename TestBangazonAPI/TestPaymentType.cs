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
                    Name = "Amex",
                    AcctNumber = newAcctNumber,
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
    }
}