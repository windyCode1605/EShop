namespace CR.Core.Dto.EmployeeDto
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? CreatDate { get; set; }
    }
}