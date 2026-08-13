using CR.DtoBase;
using Microsoft.VisualBasic;

namespace CR.Core.Dto.EmployeeDto
{
    public class EmployeeQueryDto : PagingRequestBaseDto
    {
        public int? IsActive { get; set; }
        public DateTime? FormDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}