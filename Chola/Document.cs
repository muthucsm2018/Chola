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
    
    public partial class Document
    {
        public int DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string Nationality { get; set; }
        public string VisaType { get; set; }
        public string Duration { get; set; }
        public Nullable<decimal> VisaCost { get; set; }
        public Nullable<decimal> Charge { get; set; }
        public Nullable<int> ProcessingDays { get; set; }
        public Nullable<int> SortOrder { get; set; }
    }
}
