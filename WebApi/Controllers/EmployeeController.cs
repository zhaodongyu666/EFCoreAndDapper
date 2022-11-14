using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DTOs;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        public EmployeeController(IApplicationDbContext dbContext, IApplicationReadDbConnection readDbConnection, IApplicationWriteDbConnection writeDbConnection)
        {
            _dbContext = dbContext;
            _readDbConnection = readDbConnection;
            _writeDbConnection = writeDbConnection;
        }

        public IApplicationDbContext _dbContext { get; }
        public IApplicationReadDbConnection _readDbConnection { get; }
        public IApplicationWriteDbConnection _writeDbConnection { get; }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var query = $"Select * from Employees";
            var employees = await _readDbConnection.QueryAsync<Employee>(query);
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllEmployeesById(int id)
        {
            var employees = await _dbContext.Employees.Include(a => a.Department).Where(a => a.Id == id).ToListAsync();
            return Ok(employees);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewEmployeeWithDepartment(Employee employeeDto)
        {
            _dbContext.Connection.Open();
            using (var transaction = _dbContext.Connection.BeginTransaction())
            {
                try
                {
                    _dbContext.Database.UseTransaction(transaction as DbTransaction);
                    //Check if Department Exists (By Name)
                    bool DepartmentExists = await _dbContext.Departments.AnyAsync(a => a.Name == employeeDto.Department.Name);
                    if (DepartmentExists)
                    {
                        throw new Exception("Department Already Exists");
                    }
                    //Add Department
                    //var addDepartmentQuery = $"INSERT INTO Departments(Name,Description) VALUES('{employeeDto.Department.Name}','{employeeDto.Department.Description}');SELECT CAST(SCOPE_IDENTITY() as int)";
                    //var departmentId = await _writeDbConnection.QuerySingleAsync<int>(addDepartmentQuery, transaction: transaction);


                    //Check if Department Id is not Zero.
                    //if (departmentId == 0)
                    //{
                    //    throw new Exception("Department Id");
                    //}
                    //Add Employee
                    //var a = _dbContext.Employees.Include(x => x.Department).ThenInclude(a => a.Description).ToList();
                    var employee = new Employee
                    {
                        DepartmentId = employeeDto.Department.Id,
                        Name = employeeDto.Name,
                        Email = employeeDto.Email,
                        Department = new Department{ Id = employeeDto.Department.Id, Name = employeeDto.Department.Name, Description = employeeDto.Department.Description }
                    };
                    //employee.Department = employeeDto.Department;
                    await _dbContext.Employees.AddAsync(employee);
                    await _dbContext.SaveChangesAsync(default);


                    //Commmit
                    transaction.Commit();
                    //Return EmployeeId
                    return Ok(1);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _dbContext.Connection.Close();
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            _dbContext.Connection.Open();
            using (var transaction = _dbContext.Connection.BeginTransaction())
            {
                try
                {
                    //using var context = new ApplicationDbContext();
                    _dbContext.Database.UseTransaction(transaction as DbTransaction);

                    //var deleteDepartmentQuery = $"de INTO Departments(Name,Description) VALUES('{employeeDto.Department.Name}','{employeeDto.Department.Description}');SELECT CAST(SCOPE_IDENTITY() as int)";
                    //var departmentId = await _writeDbConnection.QuerySingleAsync<int>(addDepartmentQuery, transaction: transaction);


                    //var employee = await _dbContext.Employees.Include(e => e.Department).FirstOrDefaultAsync();
                    //var employee = _dbContext.Employees.FirstOrDefault();
                    //var department = employee.Department;
                    //var department =  _dbContext.Employees.Include(e => e.Department);
                    //if (employee == null)
                    //{
                    //    return NotFound();
                    //}
                    //_dbContext.Departments.Remove(department);
                    var employee = _dbContext.Employees.OrderBy(e => e.Id).Include(e => e.Department).First();

                    //_dbContext.Employees(blog);

                    //context.SaveChanges();
                    _dbContext.Employees.Remove(employee);
                    await _dbContext.SaveChangesAsync(default);


                    //Commmit
                    transaction.Commit();
                    //Return EmployeeId
                    return Ok(1);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _dbContext.Connection.Close();
                }
            }
        }
    }
}