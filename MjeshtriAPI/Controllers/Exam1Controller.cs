using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MjeshtriAPI.Data;
using MjeshtriAPI.Models.DTOs;
using MjeshtriAPI.Models.DTOs.exam1;
using MjeshtriAPI.Models.Exam1;

namespace MjeshtriAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Exam1Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Exam1Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-employee")]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeDTO employee)
        {
            Employee e = new Employee
            {
                Name = employee.Name,
                Surname = employee.Surname
            };

            _context.Employees.Add(e);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Employee Created Successfully with ID: {e.Id}"
            });
        }

        [HttpPost("create-contract")]
        public async Task<IActionResult> CreateContract(CreateContractDTO contract)
        {
            Contract c = new Contract
            {
                Title = contract.Title,
                Description = contract.Description,
                EmployeeId = contract.EmployeeId
            };

            _context.Contracts.Add(c);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Contract Created Successfully with ID: {c.ContractId}"
            });
        }

        [HttpGet("get-employees")]
        public async Task<IActionResult> GetEmployeesWithContracts()
        {
            var employees = await _context.Employees
                .Include(e => e.Contracts)
                .Select(s => new GetEmployeesDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Surname = s.Surname
                })
                .ToListAsync();

            return Ok(employees);
        }

        [HttpGet("get-contracts")]
        public async Task<IActionResult> GetContracts()
        {
            var contracts = await _context.Contracts
                .Include(c => c.Employee)
                .Select(s => new GetContractDTO
                {
                    ContractId = s.ContractId,
                    Title = s.Title,
                    Description = s.Description,
                    EmployeeName = s.Employee.Name,
                    EmployeeSurname = s.Employee.Surname,
                    EmployeeId = s.EmployeeId
                })
                .ToListAsync();

            return Ok(contracts);
        }

        [HttpPut("update-employee")]
        public async Task<IActionResult> UpdateEmployee(UpdateEmployeeDTO emp)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(s => s.Id == emp.Id);

            if (employee == null)
            {
                return NotFound(new
                {
                    message = $"Employee with ID: {emp.Id} not found."
                });
            }

            employee.Name = emp.Name;
            employee.Surname = emp.Surname;
            employee.Id = emp.Id;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Employee with ID: {emp.Id} updated successfully."
            });
        }

        [HttpDelete("delete-contract/{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FirstOrDefaultAsync(s => s.ContractId == id);
            if (contract == null)
            {
                return NotFound(new
                {
                    message = $"Contract with ID: {id} not found."
                });
            }
            _context.Contracts.Remove(contract);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Contract with ID: {id} deleted successfully."
            });
        }
    }
}
