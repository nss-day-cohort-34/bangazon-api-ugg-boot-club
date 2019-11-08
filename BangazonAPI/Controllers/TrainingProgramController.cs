using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BangazonAPI.Models;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingProgramController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TrainingProgramController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(string completed, string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (completed == "false")
                    {
                        cmd.CommandText = @"SELECT t.Id, t.Name AS ProgramName, t.StartDate AS ProgramStartDate, 
                                        t.EndDate AS ProgramEndDate, t.MaxAttendees, e.Id AS EmployeeId,
                                        e.FirstName, e.LastName
		                                FROM TrainingProgram t
		                                LEFT JOIN EmployeeTraining et ON t.Id = et.TrainingProgramId
		                                LEFT JOIN Employee e ON e.Id = et.EmployeeId
                                        WHERE (t.EndDate >= SYSDATETIME())";
                    }

                    else
                    {
                        cmd.CommandText = @"SELECT t.Id, t.Name AS ProgramName, t.StartDate AS ProgramStartDate,
                                            t.EndDate AS ProgramEndDate, 
                                            t.MaxAttendees, e.FirstName,
                                            e.LastName, e.Id AS EmployeeId
                                            FROM TrainingProgram t
                                            LEFT JOIN EmployeeTraining et ON t.Id = et.TrainingProgramId
                                            LEFT JOIN Employee e ON e.Id = et.EmployeeId";
                    }

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    while (reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("ProgramName")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("ProgramStartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("ProgramEndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),

                            };

                            if (!trainingPrograms.Exists(t => t.Id == trainingProgram.Id))
                            {
                                trainingPrograms.Add(trainingProgram);
                                trainingProgram.EmployeeList.Add(employee);
                            }
                            else
                            {
                                TrainingProgram existingTrainingProgram = trainingPrograms.Find(t => t.Id == trainingProgram.Id);
                                existingTrainingProgram.EmployeeList.Add(employee);
                            }
                        }
                        else
                        {
                            if (!trainingPrograms.Exists(t => t.Id == trainingProgram.Id))
                            {
                                trainingPrograms.Add(trainingProgram);
                            }
                        }

                        trainingPrograms.Add(trainingProgram);
                    }
                    reader.Close();

                    return Ok(trainingPrograms);
                }
            }
        }


        [HttpGet("{id}", Name = "GetTrainingProgram")]
        public async Task<IActionResult> Get([FromRoute] int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT t.Id, t.Name AS ProgramName, t.StartDate AS ProgramStartDate,
                                            t.EndDate AS ProgramEndDate, 
                                            t.MaxAttendees, e.FirstName,
                                            e.LastName, e.Id AS EmployeeId
                                            FROM TrainingProgram t
                                            LEFT JOIN EmployeeTraining et ON t.Id = et.TrainingProgramId
                                            LEFT JOIN Employee e ON e.Id = et.EmployeeId
                                            WHERE t.id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", Id));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    TrainingProgram trainingProgram = null;

                    while (reader.Read())
                    {
                        if (trainingProgram == null)
                        {
                            trainingProgram = new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("ProgramName")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("ProgramStartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("ProgramEndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                            };
                        }


                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };

                            trainingProgram.EmployeeList.Add(employee);
                        }
                    }
                    reader.Close();

                    return Ok(trainingProgram);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TrainingProgram trainingProgram)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees)
                                        OUTPUT INSERTED.Id
                                        VALUES (@Name, @StartDate, @EndDate, @MaxAttendees)";
                    cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                    cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));

                    trainingProgram.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetTrainingProgram", new { id = trainingProgram.Id }, trainingProgram);
                }
            }
        }

        // PUT api/TrainingProgram/3
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE TrainingProgram
                                            SET Name = @Name,
                                                StartDate = @StartDate,
                                                EndDate = @EndDate,
                                                MaxAttendees = @MaxAttendees
                                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));
                        cmd.Parameters.Add(new SqlParameter("@Id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!TrainingProgramExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromRoute] int id)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = "DELETE FROM TrainingProgram WHERE Id = @id";

        //                cmd.Parameters.Add(new SqlParameter("@id", id));

        //                int rowsAffected = cmd.ExecuteNonQuery();
        //                if (rowsAffected > 0)
        //                {
        //                    return new StatusCodeResult(StatusCodes.Status204NoContent);
        //                }
        //                throw new Exception("No rows affected");
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        if (!TrainingProgramExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        private bool TrainingProgramExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, StartDate, EndDate, MaxAttendees
                        FROM TrainingProgram
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}

