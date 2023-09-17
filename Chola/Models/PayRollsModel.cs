using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Chola.Models
{       

    public class VouchersListModel
    {
        public int PaySlipCode { get; set; }

        public string MonthYear { get; set; }

        public string PayTo { get; set; }

        public string FINNo { get; set; }

        public decimal? BasicPay { get; set; }

        public decimal? OverTime { get; set; }

        public decimal? Commission { get; set; }

        public decimal? Allowance { get; set; }

        public decimal? GrossPay { get; set; }

        public decimal? NetPay { get; set; }

        public decimal? CPF { get; set; }

        public decimal? MPF { get; set; }

        public decimal? Advance { get; set; }

        public decimal? IncomeTax { get; set; }

        public decimal? TotalDeductions { get; set; }

        public decimal? Reembursement { get; set; }

        public decimal? TotalAdditions { get; set; }

        public string ApprovedBy { get; set; }

        public DateTime? PayDay { get; set; }


    }

    public class CreateVoucherModel
    {
       
        public int? PaySlipCode { get; set; }

        public string MonthYear { get; set; }

        [Required]
        public DateTime? PayDay { get; set; }

        [Required]
        public string PayTo { get; set; }

        public string FINNo { get; set; }

        public Decimal? BasicPay { get; set; }

        public Decimal? OverTime { get; set; }

        public Decimal? Commission { get; set; }

        public Decimal? Allowance { get; set; }

        public Decimal? GrossPay { get; set; }

        public Decimal? NetPay { get; set; }

        public Decimal? CPF { get; set; }

        public Decimal? MBF { get; set; }

        public Decimal? Advance { get; set; }

        public Decimal? IncomeTax { get; set; }

        public Decimal? TotalDeductions { get; set; }

        public Decimal? Reembursement { get; set; }

        public Decimal? TotalAdditions { get; set; }

        public string ApprovedBy { get; set; }



    }

    public class EditVoucherModel
    {
        public int? PaySlipCode { get; set; }

        public string MonthYear { get; set; }

        [Required]
        public DateTime? PayDay { get; set; }

        [Required]
        public string PayTo { get; set; }

        public string FINNo { get; set; }

        public Decimal? BasicPay { get; set; }

        public Decimal? OverTime { get; set; }

        public Decimal? Commission { get; set; }

        public Decimal? Allowance { get; set; }

        public Decimal? GrossPay { get; set; }

        public Decimal? NetPay { get; set; }

        public Decimal? CPF { get; set; }

        public Decimal? MBF { get; set; }

        public Decimal? Advance { get; set; }

        public Decimal? IncomeTax { get; set; }

        public Decimal? TotalDeductions { get; set; }

        public Decimal? Reembursement { get; set; }

        public Decimal? TotalAdditions { get; set; }

        public string ApprovedBy { get; set; }        


    }
    
}