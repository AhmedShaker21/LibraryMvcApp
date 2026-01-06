using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryMvcApp.Models
{
    public class FormEntry
    {
        public int Id { get; set; }

        public int DepartmentId { get; set; }   // FK
        public Department Department { get; set; }

        public int DepartmentNo { get; set; }   // للعرض فقط

        public string ProcedureName { get; set; }
        public string ProcedureCode { get; set; }
        public string FormName { get; set; }

        public int FormNumber { get; set; }
        public string FullNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int Review { get; set; } = 1;
    }


}
