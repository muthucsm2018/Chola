using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chola.Models
{
    public class AccountPaymentModel
    {
        public int? TableKey { get; set; }

        public int? VendorCode { get; set; }

        public string VendorName { get; set; }

        public DateTime? PaymentDate { get; set; }

        public double? PaymentAmount { get; set; }

        public double? BalanceAmount { get; set; }

        public string PaymentBy { get; set; }

    }

    public class AccountPaymentListModel
    {
        public string TableKey { get; set; }

        public string VendorCode { get; set; }

        public string VendorName { get; set; }

        public string PaymentDate { get; set; }

        public string PaymentBy { get; set; }

        public string PaymentAmount { get; set; }

        public string BalanceAmount { get; set; }

    }

    public class CustomerPaymentListModel
    {
        public string PaymentID { get; set; }

        public string RefNo { get; set; }

        public string CustomerName { get; set; }

        public string PaymentDate { get; set; }

        public string PaymentAmount { get; set; }

        public string BalanceAmount { get; set; }

        public string PaymentBy { get; set; }

    }

    public class CustomerPaymentModel
    {
        public int? PaymentID { get; set; }

        public string RefNo { get; set; }

        //public string CustomerName { get; set; }

        public DateTime? PaymentDate { get; set; }

        public decimal? PaymentAmount { get; set; }

        public double? BalanceAmount { get; set; }

        public string PaymentBy { get; set; }

    }
}