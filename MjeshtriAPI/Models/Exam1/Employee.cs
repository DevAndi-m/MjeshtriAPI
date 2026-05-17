using System.ComponentModel.DataAnnotations;

namespace MjeshtriAPI.Models.Exam1
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public List<Contract> Contracts { get; set; } = new();
    }
}
