using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Chola.Models
{    

    public class ExpenseListModel
    {
        public string ExpenseCode { get; set; }
        
        public string ExpenseName { get; set; }

        public string Charge { get; set; }

        public string Remarks { get; set; }

        public string TrxnDate { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public string UpdatedDate { get; set; }

    }

    public class CreateExpenseModel
    {
        public int? ExpenseCode { get; set; }

        [Required, Display(Name = "Expense Name")]
        public string ExpenseName { get; set; }

        [Required]
        public string Charge { get; set; }

        public string Remarks { get; set; }

    }

   
}