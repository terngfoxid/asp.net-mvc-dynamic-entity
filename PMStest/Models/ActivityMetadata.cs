using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PMStest.Models
{
    public class ActivityMetadata
    {
        
    }

    [MetadataType(typeof(ActivityMetadata))]
    public partial class Activity
    {
        public bool IsAddActivity { get; set; }
        public bool IsDeleteThisActivity { get; set;}
    }
}