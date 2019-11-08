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
    public class TestDepartments
    {
        [Fact]
        public async Task Test_Get_All_Departments()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/department");


                string responseBody = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(departments.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Get_Single_Department()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var getAccounting = await client.GetAsync("/api/department/1" +
                    "");
                getAccounting.EnsureSuccessStatusCode();

                string getAccountingBody = await getAccounting.Content.ReadAsStringAsync();
                Department newAccounting = JsonConvert.DeserializeObject<Department>(getAccountingBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, getAccounting.StatusCode);

            }
        }

        [Fact]
        public async Task Test_Post_Department()
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

                Department pr = new Department
                {
                    Name = "Public Relations",
                    Budget = 6000
                 
                };

                // Serialize the C# object into a JSON string
                var prAsJSON = JsonConvert.SerializeObject(pr);


                /*
                    ACT
                */

                // Use the client to send the request and store the response
                var response = await client.PostAsync(
                    "/api/department",
                    new StringContent(prAsJSON, Encoding.UTF8, "application/json")
                );

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Animal
                var newPr = JsonConvert.DeserializeObject<Department>(responseBody);


                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Public Relations", newPr.Name);
                Assert.Equal(6000, newPr.Budget);
            }
        }

        [Fact]
        public async Task Test_Update_Department()
        {
            // New last name to change to and test
            int newBudget = 11000;

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Department modifiedAccounting = new Department
                {
                    Name = "Accounting",
                    Budget = newBudget
                
                };
                var modifiedAccountingAsJSON = JsonConvert.SerializeObject(modifiedAccounting);

                var response = await client.PutAsync(
                    "/api/department/1",
                    new StringContent(modifiedAccountingAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getAccounting = await client.GetAsync("/api/department/1");
                getAccounting.EnsureSuccessStatusCode();

                string getAccountingBody = await getAccounting.Content.ReadAsStringAsync();
                Department newAccounting = JsonConvert.DeserializeObject<Department>(getAccountingBody);

                Assert.Equal(HttpStatusCode.OK, getAccounting.StatusCode);
                Assert.Equal(newBudget, newAccounting.Budget);
                
            }
        }
    }
}