//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class VolEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Place { get; set; }
        public Nullable<System.DateTime> StartDT { get; set; }
        public Nullable<System.DateTime> EndDT { get; set; }
        public Nullable<System.DateTimeOffset> Length { get; set; }
        public Nullable<int> ImageId { get; set; }
        public string Content { get; set; }
        public Nullable<int> NumUsers { get; set; }
        public string Org { get; set; }
        public string Type { get; set; }
        public string Comments { get; set; }
        public string Importants { get; set; }
        public string Longtitude { get; set; }
        public string Latitude { get; set; }
    }
}
