using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Chola.Models
{    

    public class TransactionListModel
    {
        public string TransactionID { get; set; }
        
        public string DocumentType { get; set; }

        public string DocumentName { get; set; }

        public string PassportNumber { get; set; }
        
        public string InvoiceNumber { get; set; }

        public string CustomerName { get; set; }

        public string ContactNo { get; set; }

        public string StatusCode { get; set; }

        public string StatusName { get; set; }

        public string PaidAmount { get; set; }

        public string VisaCost { get; set; }

        public string TransactionDate { get; set; }

        public string DeliveryDate { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public string UpdatedDate { get; set; }

    }

    public class CreateTransactionModel
    {
        [Required]
        public int TransactionID { get; set; }

        [Required, Display(Name = "Visa Type")]
        public int DocumentType { get; set; }
        
        public string DocumentName { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }

        [Required, Display(Name = "Passport Number")]
        public string PassportNumber { get; set; }

        [Required, Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        public string ContactNo { get; set; }

        public string Nationality { get; set; }

        public string VisaType { get; set; }

        public string Duration { get; set; } 

        public int? StatusCode { get; set; }

        public string StatusName { get; set; }

        [Required, Display(Name = "Amount"), DataType(DataType.Currency)]
        public decimal PaidAmount { get; set; }
        [Required]
        public DateTime? TransactionDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string TransactionDateDisplay { get; set; }

        public string DeliveryDateDisplay { get; set; }

        public string Remarks { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public string UpdatedDate { get; set; }

    }   

    
}