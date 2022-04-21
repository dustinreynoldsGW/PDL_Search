using System;

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