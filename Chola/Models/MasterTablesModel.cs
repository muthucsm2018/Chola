using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Chola.Models
{    

    public class DocumentsModel
    {
        public string DocumentType { get; set; }
        
        public string DocumentName { get; set; }

        public string Nationality { get; set; }

        public string VisaType { get; set; }

        public string Duration { get; set; }
        
        public string Charge { get; set; }

        public string VisaCost { get; set; }

        public string ProcessingDays { get; set; }

        public string SortOrder { get; set; } 
        
    }

    public class ServiceTypesModel
    {
        public string OthersTypeCode { get; set; }

        public string OthersTypeName { get; set; }

    }

    public class VendorsModel
    {
        public string VendorCode { get; set; }

        public string VendorName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string TaxID { get; set; }

        public string Status { get; set; }

    }

    public class FlightsModel
    {
        public string FlightCode { get; set; }

        public string FlightName { get; set; }       

        public string Status { get; set; }

    }

    public class CustomersModel
    {
        public string RefNo { get; set; }

        public string CustomerName { get; set; }

        public decimal? OpeningBalance { get; set; }       

    }

}