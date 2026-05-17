using System.ComponentModel.DataAnnotations;

namespace MjeshtriAPI.Models.Exam1
{
    public class Contract
    {
        [Key]
        public int ContractId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
    }
}
