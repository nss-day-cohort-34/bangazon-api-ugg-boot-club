using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeeController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET api/productTypes
        [HttpGet]
        public async Task<IActionResult> Get(string _include, string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor, e.StartDate, e.EndDate,
			                                    d.Name,
			                                    c. Make, C.Manufacturer
                                                FROM EMPLOYEE e
                                                INNER JOIN Department d on d.Id = e.DepartmentId
                                                LEFT JOIN Computer c on c.CurrentEmployeeId = e.Id";

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Dictionary<int, Employee> employees = new Dictionary<int, Employee>();
                    while (reader.Read())
                    {
                        int employeeId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!employees.ContainsKey(employeeId))
                        {
                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("FirstName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("Start Date")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("End Date")),
                                Department = new Department()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name"))
                                },
                                Computer = new Computer()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                                }

                            };

                            employees.Add(employeeId, employee);
                        }
                        Employee fromDictionary = employees[employeeId];
                    }

                    reader.Close();

                    return Ok(employees.Values);
                }
            }
        }

        // GET api/productTypes/5
        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor, e.StartDate, e.EndDate,
			                                    d.Name,
			                                    c. Make, C.Manufacturer
                                                FROM EMPLOYEE e
                                                INNER JOIN Department d on d.Id = e.DepartmentId
                                                LEFT JOIN Computer c on c.CurrentEmployeeId = e.Id
                                        WHERE e.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Employee employee = null;
                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("FirstName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("Start Date")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("End Date")),
                            Department = new Department()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            },
                            Computer = new Computer()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                            }

                        };
                    }
                    reader.Close();

                    return Ok(employee);
                }
            }
        }

        // POST api/productTypes
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee employee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor, StartDate, EndDate)
                        OUTPUT INSERTED.Id
                        VALUES (@firstName, @lastName, @departmentId, @isSuperVisor, @startDate, @endDate)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                    cmd.Parameters.Add(new SqlParameter("@isSuperVisor", employee.IsSuperVisor));
                    cmd.Parameters.Add(new SqlParameter("@startDate", employee.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@endDate", employee.EndDate));

                    employee.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetEmployee", new { id = employee.Id }, employee);
                }
            }
        }

        // PUT api/productTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] ProductType productType)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE ProductType
                            SET Name = @name
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@name", productType.Name));

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
                if (!ProductTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/productTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM ProductType WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
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
                if (!ProductTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ProductTypeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM ProductType WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}