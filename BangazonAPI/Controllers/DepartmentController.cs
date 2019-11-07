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
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DepartmentController(IConfiguration config)
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

        // GET api/Department
        [HttpGet]
        public async Task<IActionResult> Get(string _include, string _filter, int _gt)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if(_include?.ToLower() == "employees")
                    {
                        cmd.CommandText = @"SELECT d.Id, d.Name, d.Budget, e.Id AS EmployeeId, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor, e.StartDate, IsNull(e.EndDate, '') AS EndDate
                                            FROM Department d
                                            LEFT JOIN Employee e on e.DepartmentId = d.Id";

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        Dictionary<int, Department> departments = new Dictionary<int, Department>();
                        while (reader.Read())
                        {
                            int departmentId = reader.GetInt32(reader.GetOrdinal("Id"));
                            if (!departments.ContainsKey(departmentId))
                            {


                                Department department = new Department
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Budget = reader.GetInt32(reader.GetOrdinal("Budget"))

                                };

                                departments.Add(departmentId, department);
                            }
                            Department fromDictionary = departments[departmentId];
                            if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                            {
                                Employee anEmployee = new Employee()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                };
                                fromDictionary.employees.Add(anEmployee);
                            }
                        }

                        reader.Close();

                        return Ok(departments.Values);
                    }
                    else if(_filter?.ToLower() == "budget") {
                        cmd.CommandText = @"SELECT Id, Name, Budget FROM Department
                                            WHERE BUDGET > @amount";

                        cmd.Parameters.Add(new SqlParameter("@amount", _gt));
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        Dictionary<int, Department> departments = new Dictionary<int, Department>();
                        while (reader.Read())
                        {
                            int departmentId = reader.GetInt32(reader.GetOrdinal("Id"));
                            if (!departments.ContainsKey(departmentId))
                            {


                                Department department = new Department
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Budget = reader.GetInt32(reader.GetOrdinal("Budget"))

                                };

                                departments.Add(departmentId, department);
                            }
                        }

                        reader.Close();

                        return Ok(departments.Values);

                    }

                    else
                    {
                    cmd.CommandText = @"SELECT Id, Name, Budget FROM Department";

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Dictionary<int, Department> departments = new Dictionary<int, Department>();
                    while (reader.Read())
                    {
                        int departmentId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!departments.ContainsKey(departmentId))
                        {


                            Department department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))

                            };

                            departments.Add(departmentId, department);
                        }
                    }

                    reader.Close();

                    return Ok(departments.Values);

                    }

                }
            }
        }

        // GET api/department/5
        [HttpGet("{id}", Name = "GetDepartment")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, Budget FROM Department
                                        WHERE 
Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Department department = null;
                    if (reader.Read())
                    {
                        department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))

                        };
                    }
                    reader.Close();

                    return Ok(department);
                }
            }
        }

        // POST api/department
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Department department)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Department (Name, Budget)
                        OUTPUT INSERTED.Id
                        VALUES (@name, @budget)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                    cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));

                    department.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetDepartment", new { id = department.Id }, department);
                }
            }
        }

        // PUT api/department/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Department department)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Department
                            SET Name = @name, Budget = @budget
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                        cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));
                      

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
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/department/5
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
                        cmd.CommandText = @"DELETE FROM Department WHERE Id = @id";
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
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool DepartmentExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM Department WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}