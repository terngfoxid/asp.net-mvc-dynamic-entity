using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PMStest.Models
{
    public class ProjectMetadata
    {
        [Required(ErrorMessage = "กรุณาระบุวันที่เริ่มต้น")]
        [DisplayName("วันที่เริ่มต้น")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> StartDate { get; set; }

        [Required(ErrorMessage = "กรุณาระบุวันที่สิ้นสุด")]
        [DisplayName("วันที่สิ้นสุด")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> EndDate { get; set; }

        [DisplayName("วันที่ปรับปรุง")]
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [DisplayName("ชื่อ")]
        public string Name { get; set; }
    }

    [MetadataType(typeof(ProjectMetadata))]
    public partial class Project
    {
        public bool IsAddActivity { get; set; }
    }
}