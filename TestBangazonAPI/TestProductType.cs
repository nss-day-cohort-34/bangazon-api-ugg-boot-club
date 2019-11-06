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
    public class TestProductTypes
    {
        [Fact]
        public async Task Test_Get_All_ProductTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/ProductType");


                string responseBody = await response.Content.ReadAsStringAsync();
                var productTypes = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypes.Count > 0);
            }
        }
            [Fact]
        public async Task Test_Get_Single_ProductType()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var getFood = await client.GetAsync("/api/productType/1" +
                    "");
                getFood.EnsureSuccessStatusCode();

                string getFoodBody = await getFood.Content.ReadAsStringAsync();
                ProductType newFood = JsonConvert.DeserializeObject<ProductType>(getFoodBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, getFood.StatusCode);
               
            }
        }

        [Fact]
        public async Task Test_Post_ProductType()
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
                 ProductType toys = new ProductType
                {
                    Name = "Toys"
                };

                // Serialize the C# object into a JSON string
                var toysAsJSON = JsonConvert.SerializeObject(toys);


                /*
                    ACT
                */

                // Use the client to send the request and store the response
                var response = await client.PostAsync(
                    "/api/ProductType",
                    new StringContent(toysAsJSON, Encoding.UTF8, "application/json")
                );

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Animal
                var newToys = JsonConvert.DeserializeObject<ProductType>(responseBody);


                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Toys", newToys.Name);
            }
        }

        [Fact]
        public async Task Test_Update_ProductType()
        {
            // New last name to change to and test
            string newName = "Toys and Games";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                ProductType modifiedToys = new ProductType
                {
                    Name = newName
                };
                var modifiedToysAsJSON = JsonConvert.SerializeObject(modifiedToys);

                var response = await client.PutAsync(
                    "/api/ProductType/1",
                    new StringContent(modifiedToysAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getToys = await client.GetAsync("/api/ProductType/1");
                getToys.EnsureSuccessStatusCode();

                string getToysBody = await getToys.Content.ReadAsStringAsync();
                ProductType newToys = JsonConvert.DeserializeObject<ProductType>(getToysBody);

                Assert.Equal(HttpStatusCode.OK, getToys.StatusCode);
                Assert.Equal(newName, newToys.Name);
            }
        }
    }
}
