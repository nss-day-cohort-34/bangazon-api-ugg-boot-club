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
    public class TestEmployees
    {
        [Fact]
        public async Task Test_Get_All_Employees()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/employee");


                string responseBody = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(employees.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Get_Single_Employee()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var getMatt = await client.GetAsync("/api/employee/1" +
                    "");
                getMatt.EnsureSuccessStatusCode();

                string getMattBody = await getMatt.Content.ReadAsStringAsync();
                Employee newMatt = JsonConvert.DeserializeObject<Employee>(getMattBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, getMatt.StatusCode);

            }
        }

        [Fact]
        public async Task Test_Post_Employee()
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

                var start = new DateTime(2019, 01, 01);
                var end = new DateTime(2019, 01, 01);
                Employee joey = new Employee
                {
                    FirstName = "Joey",
                    LastName = "Smith",
                    DepartmentId = 2,
                    IsSuperVisor = true,
                    StartDate = start,
                    EndDate = end
                };

                // Serialize the C# object into a JSON string
                var joeyAsJSON = JsonConvert.SerializeObject(joey);


                /*
                    ACT
                */

                // Use the client to send the request and store the response
                var response = await client.PostAsync(
                    "/api/employee",
                    new StringContent(joeyAsJSON, Encoding.UTF8, "application/json")
                );

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Animal
                var newJoey = JsonConvert.DeserializeObject<Employee>(responseBody);


                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Joey", newJoey.FirstName);
                Assert.Equal("Smith", newJoey.LastName);
                Assert.Equal(2, newJoey.DepartmentId);
                Assert.True(newJoey.IsSuperVisor);
                Assert.Equal(start, newJoey.StartDate);
                Assert.Equal(end, newJoey.EndDate);
            }
        }

        [Fact]
        public async Task Test_Update_Employee()
        {
            // New last name to change to and test
            DateTime start = new DateTime(2019, 01, 01);
            DateTime end = new DateTime(2019, 01, 01);

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Employee modifiedMatt = new Employee
                {
                    FirstName = "Matthew",
                    LastName = "Ross",
                    DepartmentId = 1,
                    IsSuperVisor = false,
                    StartDate = start,
                    EndDate = end
                };
                var modifiedMattAsJSON = JsonConvert.SerializeObject(modifiedMatt);

                var response = await client.PutAsync(
                    "/api/employee/1",
                    new StringContent(modifiedMattAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getMatt = await client.GetAsync("/api/employee/1");
                getMatt.EnsureSuccessStatusCode();

                string getMattBody = await getMatt.Content.ReadAsStringAsync();
                Employee newMatt = JsonConvert.DeserializeObject<Employee>(getMattBody);

                Assert.Equal(HttpStatusCode.OK, getMatt.StatusCode);
                Assert.Equal(start, newMatt.StartDate);
                Assert.Equal(end, newMatt.EndDate);
            }
        }
    }
}