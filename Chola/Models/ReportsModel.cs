
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Chola.Models
{    

    public class BillingListModel
    {        
        public string PatientID { get; set; }
        public string PatientName { get; set; }
        public string DepName { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceNo { get; set; }
    }
}