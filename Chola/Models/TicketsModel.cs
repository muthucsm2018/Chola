using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Chola.Models
{    

    public class TicketListModel
    {
        public string TicketCode { get; set; }
        
        public string CompanyName { get; set; }
        
        public string Charge { get; set; }

        public string TrxnDate { get; set; }

        public string DueDate { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public string UpdatedDate { get; set; }

    }

    public class TourListModel
    {
        public string TourCode { get; set; }

        public string CustomerName { get; set; }

        public string BuyingPrice { get; set; }

        public string SellingPrice { get; set; }

        public string TrxnDate { get; set; }

        public string Remarks { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public string UpdatedDate { get; set; }

    }

    public class TicketListReportModel
    {
        public string TicketCode { get; set; }

        public string CompanyName { get; set; }

        public string CustomerName { get; set; }

        public string Description { get; set; }

        public string Remarks { get; set; }

        public string BuyingPrice { get; set; }

        public string SellingPrice { get; set; }

        public string TrxnDate { get; set; }

        public string DueDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }

    }

    public class CustomerListReportModel
    {
        public string RefNo { get; set; }

        public string CustomerName { get; set; }

        public string OpeningBalance { get; set; }

        public string Debit { get; set; }

        public string Credit { get; set; }

    }

    public class CreateTicketModel
    {
        //public int? TicketCode { get; set; }
        public Int64? TicketCode { get; set; }

        [Required, Display(Name = "Customer Name")]
        public int? VendorCode { get; set; }

        public DateTime? DueDate { get; set; }

        public string Charge { get; set; }

        public string RefNo { get; set; }
        [Required]
        public string ContactNo { get; set; }


    }

    public class EditTicketModel
    {
        public Int64? TicketCode { get; set; }

        public string RefNo { get; set; }
        [Required]
        public string ContactNo { get; set; }

        [Required, Display(Name = "Customer Name")]
        public int? VendorCode { get; set; }

        public DateTime? DueDate { get; set; }

        public string Addr1 { get; set; }

        public string Addr2 { get; set; }

        public string Addr3 { get; set; }

        public string TaxID { get; set; }

        public string Charge { get; set; }

    }

    public class TicketCustomersListModel
    {
        public Int64? TicketCode { get; set; }

        public int CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public string Description { get; set; }

        public string Remarks { get; set; }

        public decimal? BuyingPrice { get; set; }

        public decimal? SellingPrice { get; set; }


    }  

    public class CreateTourModel
    {
        public int? TourCode { get; set; }

        [Required, Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Required, Display(Name = "Remarks")]
        public string Remarks { get; set; }
        [Required]
        public DateTime? TrxnDate { get; set; }
        [Required]
        public decimal BuyingPrice { get; set; }

        [Required]
        public decimal SellingPrice { get; set; }

    }

    public class EditTourModel
    {
        public int? TourCode { get; set; }

        [Required, Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Required, Display(Name = "Remarks")]
        public string Remarks { get; set; }
        [Required]
        public DateTime? TrxnDate { get; set; }
        [Required]
        public decimal BuyingPrice { get; set; }

        [Required]
        public decimal SellingPrice { get; set; }


    }
}