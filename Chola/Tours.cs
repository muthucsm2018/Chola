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
    
    public partial class Tours
    {
        public int TourCode { get; set; }
        public string CustomerName { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> TrxnDate { get; set; }
        public Nullable<decimal> BuyingPrice { get; set; }
        public Nullable<decimal> SellingPrice { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}