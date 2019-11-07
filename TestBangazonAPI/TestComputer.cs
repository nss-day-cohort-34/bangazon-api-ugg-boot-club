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
        public async Task Test_Get_All_Computer()
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
        public async Task Test_Get_Computer()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var getComp = await client.GetAsync("/api/computers/1" +
                    "");
                getComp.EnsureSuccessStatusCode();

                string getCompBody = await getComp.Content.ReadAsStringAsync();
                Computer newComp = JsonConvert.DeserializeObject<Computer>(getCompBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, getComp.StatusCode);

            }
        }
        [Fact]
        public async Task Test_Post_Computer()
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
                var fuckingDate = new DateTime(2019, 01, 01);

                // Construct a new student object to be sent to the API
                Computer Asus = new Computer
                {
                    PurchaseDate = fuckingDate,
                    DecomissionDate = fuckingDate,
                    Make = "Asus",
                    Manufacturer  = "XP 500",
                    CurrentEmployeeId = 2
                };

                // Serialize the C# object into a JSON string
                var asusAsJSON = JsonConvert.SerializeObject(Asus);


                /*
                    ACT
                */

                // Use the client to send the request and store the response
                var response = await client.PostAsync(
                    "/api/computer",
                    new StringContent(asusAsJSON, Encoding.UTF8, "application/json")
                );

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Animal
                var newAsus = JsonConvert.DeserializeObject<Computer>(responseBody);


                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(fuckingDate, newAsus.PurchaseDate);
                Assert.Equal(fuckingDate, newAsus.DecomissionDate);
                Assert.Equal("Asus", newAsus.Make);
                Assert.Equal("Manufacturer", newAsus.Manufacturer);
                Assert.Equal(2, newAsus.CurrentEmployeeId);
            }
        }
        [Fact]
        public async Task Test_Modify_Computer()
        {
            // New last name to change to and test
            int newCurrentEmployeeId = 4;

            var fuckingDate = new DateTime(2019, 04, 04);

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Computer modifiedEmployee = new Computer
                {
                    PurchaseDate = fuckingDate,
                    DecomissionDate =  fuckingDate,
                    Make = "Mac",
                    Manufacturer = "Apple",
                    CurrentEmployeeId = newCurrentEmployeeId,
                };
                var modifiedEmployeeAsJSON = JsonConvert.SerializeObject(modifiedEmployee);

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
                var getComp = await client.GetAsync("/api/computer/1");
                getComp.EnsureSuccessStatusCode();

                string getCompBody = await getComp.Content.ReadAsStringAsync();
                Computer newComp = JsonConvert.DeserializeObject<Computer>(getCompBody);

                Assert.Equal(HttpStatusCode.OK, getComp.StatusCode);
                Assert.Equal(newCurrentEmployeeId, newComp.CurrentEmployeeId);
            }
        }
        [Fact]
        public async Task Test_Delete_Computer()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Computer Seven = new Computer
                {
                    Id = -1,
                    Make = "777"
                };

                // Serialize the C# object into a JSON string
                var sevenAsJSON = JsonConvert.SerializeObject(Seven);


                /*
                    ACT
                */
                //Insert object
                var response = await client.PostAsync(
                    "/api/computer",
                    new StringContent(sevenAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                var newComp = JsonConvert.DeserializeObject<Computer>(responseBody);

                //Get object
                var getSeven = await client.GetAsync("/api/computer/-1" +
                    "");
                getSeven.EnsureSuccessStatusCode();

                string getSevenBody = await getSeven.Content.ReadAsStringAsync();
                Computer newSeven = JsonConvert.DeserializeObject<Computer>(getSevenBody);

                //Delete Object
                var deleteSeven = await client.DeleteAsync("/api/computer/-1" +
                    "");
                getSeven.EnsureSuccessStatusCode();
                //Try to Get Object Again
                var attemptGetSeven = await client.GetAsync("/api/computer/-1" +
                    "");
                attemptGetSeven.EnsureSuccessStatusCode();

                string attemptGetSevenBody = await getSeven.Content.ReadAsStringAsync();
                Computer newAttemptSeven = JsonConvert.DeserializeObject<Computer>(attemptGetSevenBody);


                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, attemptGetSeven.StatusCode);

            }
        }
    }
}
