using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using static PDL_Search.Classes.GainwellDatabaseExtensions;

namespace PDL_Search.Models
{
    public class DrugModel
    {
        // Properties
        public String BrandName { get; set; } = "";
        public String GenericName { get; set; } = "";
        public String SearchDate { get; set; } = "";
        public String NDC { get; set; } = "";
        public String DrugClass { get; set; }
        
        public String DrugList { get; set; } = "";

        // Constructor
        public DrugModel()
        {
            BrandName = "";
            GenericName = "";
            SearchDate = "";
            NDC = "";
            DrugList = "";
        }
    }
}