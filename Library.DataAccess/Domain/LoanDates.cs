using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Domain
{
    public class LoanDates
    {
        [Key]
        public long LOAN_DATE_ID { get; set; }

        [Required(ErrorMessage = "El ID Prestamo es Obligatorio")]
        [ForeignKey("Loans")]
        public long ID_LOAN{ get; set; }
        public Loans Loans { get; set; }
        // FK REAL A LoansTeacher
        [Column("LOANSTEACHER_ID")]
        [ForeignKey(nameof(LoansTeacher))]
        public long? LOANSTEACHER_ID { get; set; }
        public LoansTeacher? LoansTeacher { get; set; }

        [Required(ErrorMessage = "La fecha de solicitud es Obligatoria")]
        public DateTime START_DATE { get; set; }

        [Required(ErrorMessage = "La fecha de devolución es Obligatoria")]
        public DateTime END_DATE { get; set; }

        [Required(ErrorMessage = "El Estatus del prestamo es Obligatorio")]
        public byte STATUS { get; set; }


    }
    public class LoanDates2
    { 
        public long LOAN_DATE_ID { get; set; } 
        public long ID_LOAN { get; set; }
        //public Loans Loans { get; set; } 
        public DateTime START_DATE { get; set; } 
        public DateTime END_DATE { get; set; } 
        public byte STATUS { get; set; }

        

    }
}
