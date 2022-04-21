using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDL_Search.Models
{
    public class InformationModel
    {
        // Properties
        public String StringInformation { get; set; }
        
        // Constructor
        public InformationModel()
        {
            StringInformation = "";
        }

        public InformationModel(String InitialInformation)
        {
            StringInformation = InitialInformation;
        }
    }
}