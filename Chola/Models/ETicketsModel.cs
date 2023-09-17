using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Chola.Models
{    

    public class ETicketListModel
    {
        public string ETicketCode { get; set; }
        
        public string CustomerName { get; set; }
        
        public string Prefix { get; set; }

        public string AirlinePNR { get; set; }

        public string RefNo { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public string UpdatedDate { get; set; }

    }

    public class ETicketPassengerListModel
    {
        public string ETicketCode { get; set; }

        public string PassengerCode { get; set; }

        public string PassengerName { get; set; }

        public string PaxType { get; set; }

        public string TicketNo { get; set; }      

    }

    public class ETicketItineryListModel
    {
        public string ETicketCode { get; set; }

        public string ItineryCode { get; set; }

        public string Flight { get; set; }

        public string FlightNo { get; set; }

        public string Depart { get; set; }
        public string DepartTer { get; set; }

        public string DepartDate { get; set; }

        public string DepartTime { get; set; }

        public string Arrive { get; set; }
        public string ArriveTer { get; set; }

        public string ArriveDate { get; set; }

        public string ArriveTime { get; set; }

        public string Class { get; set; }

        public string Baggage { get; set; }

    }
  

    public class ETicketModel
    {
        public int? ETicketCode { get; set; }

        [Required, Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        public string Prefix { get; set; }
        [Required]
        public string AirlinePNR { get; set; }

        public string RefNo { get; set; }
       
    }
    

    public class ETicketPassengerModel
    {
        public int? ETicketCode { get; set; }

        public int? PassengerCode { get; set; }

        [Required]
        public string PassengerName { get; set; }

        [Required]
        public string PaxType { get; set; }
       
        public string TicketNo { get; set; }       
    }

    public class ETicketItineyModel
    {
        public int? ETicketCode { get; set; }
        public int? ItineryCode { get; set; }

        [Required]
        public string Flight { get; set; }
        [Required]
        public string FlightNo { get; set; }
        [Required]
        public string Depart { get; set; }
        public string DepartTer { get; set; }
        [Required]
        public DateTime? DepartDate { get; set; }
        [Required]
        public string DepartTime { get; set; }
        [Required]
        public string Arrive { get; set; }
        public string ArriveTer { get; set; }
        [Required]
        public DateTime? ArriveDate { get; set; }
        [Required]
        public string ArriveTime { get; set; }
        [Required]
        public string Class { get; set; }
        [Required]
        public string Baggage { get; set; }
    }
}