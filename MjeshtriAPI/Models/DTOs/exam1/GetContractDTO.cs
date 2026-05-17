namespace MjeshtriAPI.Models.DTOs.exam1
{
    public class GetContractDTO
    {
        public int ContractId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string EmployeeSurname { get; set; } = "";

        public int EmployeeId { get; set; }
    }
}
