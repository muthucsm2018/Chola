//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Chola
{
    using System;
    using System.Collections.Generic;
    
    public partial class Acc_SalesPayments
    {
        public int TableKey { get; set; }
        public int VendorCode { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<double> PaymentAmount { get; set; }
        public string PaymentBy { get; set; }
        public Nullable<double> BalanceAmount { get; set; }
    }
}
