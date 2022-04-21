using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PDL_Search.Classes
{
    public static class GainwelleExtensions
    {
        public static String CleanUsername(this System.Security.Principal.IIdentity myIdentity)
        {
            return myIdentity.Name.Substring(myIdentity.Name.LastIndexOf("/") + 1);
        }  
    }
}