using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Net.Http;

namespace TestBangazonAPI
{
    public class TestOrders
    {
        [Fact]
        public async Task Test_Get_All_Orders()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/orders");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
            }
        }
            [Fact]
        public async Task Test_Get_Single_Order_By_Id()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/orders/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        // POST new order
        [Fact]
        public async Task Test_Create_Order()
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
                Order createdOrder = new Order
                {
                    CustomerId = 1,
                    PaymentTypeId = 1,
                    Status = "Complete"
                };

                // Serialize the C# object into a JSON string
                var createdOrderAsJSON = JsonConvert.SerializeObject(createdOrder);


                /*
                    ACT
                */

                // Use the client to send the request and store the response
                var response = await client.PostAsync(
                    "/api/orders",
                    new StringContent(createdOrderAsJSON, Encoding.UTF8, "application/json")
                );

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Animal
                var newOrderFromResponse = JsonConvert.DeserializeObject<Order>(responseBody);


                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(1, newOrderFromResponse.CustomerId);
                Assert.Equal(1, newOrderFromResponse.PaymentTypeId);
                Assert.Equal("Complete", newOrderFromResponse.Status);
            }
        }

        [Fact]
        public async Task Test_Modify_Order()
        {
            // New last name to change to and test
            int newPaymentTypeId = 1;

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Order modifiedOrder = new Order
                {
                    CustomerId = 1,
                    PaymentTypeId = newPaymentTypeId,
                    Status = "In Progress"
                };
                var modifiedOrderAsJSON = JsonConvert.SerializeObject(modifiedOrder);

                var response = await client.PutAsync(
                    "/api/orders/1",
                    new StringContent(modifiedOrderAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getModifiedOrder = await client.GetAsync("/api/orders/1");
                getModifiedOrder.EnsureSuccessStatusCode();

                string getOrderBody = await getModifiedOrder.Content.ReadAsStringAsync();
                Order newOrder = JsonConvert.DeserializeObject<Order>(getOrderBody);

                Assert.Equal(HttpStatusCode.OK, getModifiedOrder.StatusCode);
                Assert.Equal(newPaymentTypeId, newOrder.PaymentTypeId);
            }
        }
        [Fact]
        public async Task Test_Delete_Order()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Order orderToDelete = new Order
                {
                    CustomerId = 1,
                    PaymentTypeId = 1,
                    Status = "alive... for now"
                };

                // Serialize the C# object into a JSON string
                var orderAsJSON = JsonConvert.SerializeObject(orderToDelete);

                /*
                    ACT
                */
                //Insert object
                var response = await client.PostAsync(
                    "/api/orders",
                    new StringContent(orderAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                var newOrder = JsonConvert.DeserializeObject<Order>(responseBody);

                //Get object
                var getOrderToDelete = await client.GetAsync($"/api/orders/{newOrder.Id}");
                getOrderToDelete.EnsureSuccessStatusCode();

                string getOrderToDeleteBody = await getOrderToDelete.Content.ReadAsStringAsync();
                Order newOrderFromResponse = JsonConvert.DeserializeObject<Order>(getOrderToDeleteBody);

                //Delete Object
                var deleteOrder = await client.DeleteAsync($"/api/orders/{newOrder.Id}");
                deleteOrder.EnsureSuccessStatusCode();
                //Try to Get Object Again
                var attemptGetOrder = await client.GetAsync($"/api/orders/{newOrder.Id}");
                attemptGetOrder.EnsureSuccessStatusCode();

                string attemptGetOrderBody = await getOrderToDelete.Content.ReadAsStringAsync();
                Order newAttemptToGetOrder = JsonConvert.DeserializeObject<Order>(attemptGetOrderBody);


                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, attemptGetOrder.StatusCode);

            }
        }
    }
}

