using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Text;

namespace TestBangazonAPI
{
    public class TestTrainingProgram
    {
        [Fact]
        public async Task Test_Get_All_TrainingPrograms()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/trainingprogram");

                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingPrograms = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(trainingPrograms.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Get_One_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/trainingProgram/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(trainingProgram.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Create_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                TrainingProgram newTrainingProgram = new TrainingProgram
                {
                    Name = "Anti-Harrassment Training",
                    StartDate = DateTime.Today.AddDays(-1),
                    EndDate = DateTime.Now,
                    MaxAttendees = 50
                };

                var newTrainingProgramAsJSON = JsonConvert.SerializeObject(newTrainingProgram);

                /*
                    ACT
                */
                var response = await client.PostAsync("/api/trainingProgram",
                new StringContent(newTrainingProgramAsJSON, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Anti-Harrassment Training", trainingProgram.Name);
                Assert.Equal(50, trainingProgram.MaxAttendees);
            }
        }

        [Fact]
        public async Task Test_Modify_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                int newMaxAttendees = 25;

                TrainingProgram updatedTrainingProgram = new TrainingProgram
                {
                    Name = "Anti-Harrassment Training",
                    StartDate = DateTime.Today.AddDays(-1),
                    EndDate = DateTime.Now,
                    MaxAttendees = newMaxAttendees
                };

                var newTrainingProgramAsJSON = JsonConvert.SerializeObject(updatedTrainingProgram);

                /*
                    ACT
                */
                var response = await client.PutAsync("/api/trainingProgram/1",
                    new StringContent(newTrainingProgramAsJSON, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    Get Section
                */

                var getTrainingProgram = await client.GetAsync("/api/trainingProgram/1");
                getTrainingProgram.EnsureSuccessStatusCode();

                string getTrainingProgramBody = await getTrainingProgram.Content.ReadAsStringAsync();

                TrainingProgram trainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(getTrainingProgramBody);

                Assert.Equal(HttpStatusCode.OK, getTrainingProgram.StatusCode);
                Assert.Equal(newMaxAttendees, updatedTrainingProgram.MaxAttendees);
                Assert.Equal("Anti-Harrassment Training", updatedTrainingProgram.Name);

            }
        }
    }
}
