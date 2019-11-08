using Newtonsoft.Json;
using BangazonAPI.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using TestBangazonAPI;

namespace TestBangazonAPI
{
    public class ProductTests
    {
        [Fact]
        public async Task Test_Get_All_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/products");


                string responseBody = await response.Content.ReadAsStringAsync();
                var productList = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productList.Count > 0);
               
            
            }
        }

        [Fact]
        public async Task Test_Create_Product()
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

                // Construct a new exercise object to be sent to the API
                Product Popcorn = new Product
                {
                    ProductTypeId = 1,
                    CustomerId = 1,
                    Price = 5.50M,
                    Title = "Popcorn",
                    Description = "Buttery",
                    Quantity = 80
                };

                // Serialize the object into a JSON string
                var productAsJSON = JsonConvert.SerializeObject(Popcorn);


                /*
                    ACT
                */

                // Use the client to send the request and store the response
                var response = await client.PostAsync(
                    "/api/products",
                    new StringContent(productAsJSON, Encoding.UTF8, "application/json")
                );

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of product
                var newProduct = JsonConvert.DeserializeObject<Product>(responseBody);


                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(1, newProduct.ProductTypeId);
                Assert.Equal(1, newProduct.CustomerId);
                Assert.Equal(5.50M, newProduct.Price);
                Assert.Equal("Popcorn", newProduct.Title);
                Assert.Equal("Buttery", newProduct.Description);
                Assert.Equal(80, newProduct.Quantity);
            }
        }

        [Fact]
        public async Task Test_Modify_Product()
        {
            // New quantity to change to and test
            int newQuantity = 100;

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Product modifiedProduct = new Product
                {
                    ProductTypeId = 1,
                    CustomerId = 1,
                    Price = 5.50M,
                    Title = "Popcorn",
                    Description = "Buttery",
                    Quantity = newQuantity,
                    
                };
                var modifiedProductAsJSON = JsonConvert.SerializeObject(modifiedProduct);

                var response = await client.PutAsync(
                    "/api/products/1",
                    new StringContent(modifiedProductAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getProduct = await client.GetAsync("/api/products/1");
                getProduct.EnsureSuccessStatusCode();

                string getProductBody = await getProduct.Content.ReadAsStringAsync();
                Product newProduct = JsonConvert.DeserializeObject<Product>(getProductBody);

                Assert.Equal(HttpStatusCode.OK, getProduct.StatusCode);
                Assert.Equal(newQuantity, newProduct.Quantity);
            }
        }

        [Fact]
        public async Task Test_Delete_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */

<<<<<<< HEAD
                var response = await client.DeleteAsync("/api/products/1");
=======
                var response = await client.DeleteAsync("/api/products/3");
>>>>>>> master

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }
    }
}
