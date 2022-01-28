using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToolsManagementSystem.Models
{
    public class AFBorrowerIssue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index("IX_ItemIDAndSerialNoandAFEmployeeID", 3, IsUnique = true)]
        public int id { get; set; }
        [Index("IX_ItemIDAndSerialNoandAFEmployeeID", 1, IsUnique = true)]
        public int AFBorrowerID { get; set; }
        [Index("IX_ItemIDAndSerialNoandAFEmployeeID", 2, IsUnique = true)]
        public int ItemID { get; set; }
        [Display(Name = "Serial No")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        //[Index("IX_ItemIDAndSerialNoandAFEmployeeID", 3, IsUnique = true)]
        public string SerialNo { get; set; }
        public string PO { get; set; }
        public int Quantity { get; set; }
        public int QuantityAdj { get; set; }
        public int QuantityTransferred { get; set; }
        public string UoM { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Amount { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateIssued { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ? DateTransferred { get; set; }

        //public int ContractorID { get; set; }
        public string WorkOrder { get; set; }
        public string Remarks { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        public DateTime Created_At { get; set; }

        public virtual ICollection<AFBorrowerReturn> AFBorrowerReturns { get; set; }
        public virtual AFBorrower AFBorrowers { get; set; }
        public virtual Item Items { get; set; }
        //public virtual Contractor Contractors { get; set; }

        public AFBorrowerIssue()
        {
            Status = "Active";
            Created_At = DateTime.Now.Date;
        }
    }
}