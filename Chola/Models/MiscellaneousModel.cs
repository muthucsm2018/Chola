using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Chola.Models
{    

    public class EntryListModel
    {
        public string OthersCode { get; set; }
        
        public string OthersTypeCode { get; set; }
        
        public string OthersTypeName { get; set; }

        public string Charge { get; set; }

        public string Remarks { get; set; }

        public string TrxnDate { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public string UpdatedDate { get; set; }

    }

    public class CreateEntryModel
    {
        public int? OthersCode { get; set; }

        [Required, Display(Name = "Service")]
        public string OthersTypeCode { get; set; }

        [Required]
        public string Charge { get; set; }

        public string Remarks { get; set; }

    }

    public class InvoiceModel
    {
        public string ServiceType { get; set; }

        public string Charge { get; set; }

    }
   

    
}