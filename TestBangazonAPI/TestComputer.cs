using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System;

namespace TestBangazonAPI
{
    public class TestComputer
    {

        [Fact]
        public async Task Test_Get_All_Computers()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/computer");


                string responseBody = await response.Content.ReadAsStringAsync();
                var computers = JsonConvert.DeserializeObject<List<Computer>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(computers.Count > 0);
            }

        }
        [Fact]
        public async Task Test_Get_Single_Computer()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var getOneComputer = await client.GetAsync("/api/computers/1" +
                    "");
                getOneComputer.EnsureSuccessStatusCode();

                string getOneBody = await getOneComputer.Content.ReadAsStringAsync();
                Computer newComp = JsonConvert.DeserializeObject<Computer>(getOneBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, getOneComputer.StatusCode);

            }
        }
        [Fact]
        public async Task Test_Modify_Computer()
        {
            // New last name to change to and test
            int newEmployeeId = 4;
            var date = new DateTime(2019, 01, 01);

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Computer modifiedEmployeeId = new Computer
                {
                    PurchaseDate = date,
                    DecomissionDate = date,
                    Make =  "Something",
                    Manufacturer = "Something",
                    CurrentEmployeeId = newEmployeeId
                };
                var modifiedEmployeeAsJSON = JsonConvert.SerializeObject(modifiedEmployeeId);

                var response = await client.PutAsync(
                    "/api/computer/1",
                    new StringContent(modifiedEmployeeAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getUpdate = await client.GetAsync("/api/computer/1");
                getUpdate.EnsureSuccessStatusCode();

                string getUpdatedBody = await getUpdate.Content.ReadAsStringAsync();
                Computer newComp = JsonConvert.DeserializeObject<Computer>(getUpdatedBody);

                Assert.Equal(HttpStatusCode.OK, getUpdate.StatusCode);
                Assert.Equal(newEmployeeId, newComp.CurrentEmployeeId);
            }
        }
        [Fact]
        public async Task Test_Delete_Computer()
        {
            var date = new DateTime(2019, 01, 01);

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Computer Something = new Computer
                {
                    Id = -1,
                    PurchaseDate = date,
                    DecomissionDate = date,
                    Make = "Something",
                    Manufacturer = "Something"
                };

                // Serialize the C# object into a JSON string
                var somethingAsJSON = JsonConvert.SerializeObject(Something);


                /*
                    ACT
                */
                //Insert object
                var response = await client.PostAsync(
                    "/api/PaymentType",
                    new StringContent(somethingAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                var newType = JsonConvert.DeserializeObject<Computer>(responseBody);

                //Get object
                var getSomething = await client.GetAsync("/api/computer/-1" +
                    "");
                getSomething.EnsureSuccessStatusCode();

                string getSomethingBody = await getSomething.Content.ReadAsStringAsync();
                Computer newSomething = JsonConvert.DeserializeObject<Computer>(getSomethingBody);

                //Delete Object
                var deleteSomething = await client.DeleteAsync("/api/computer/-1" +
                    "");
                getSomething.EnsureSuccessStatusCode();
                //Try to Get Object Again
                var attemptGetSomething = await client.GetAsync("/api/computer/-1" +
                    "");
                attemptGetSomething.EnsureSuccessStatusCode();

                string attemptGetSomethingBody = await getSomething.Content.ReadAsStringAsync();
                Computer newAttemptSomething = JsonConvert.DeserializeObject<Computer>(attemptGetSomethingBody);


                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, attemptGetSomething.StatusCode);

            }
        }
    }
}
