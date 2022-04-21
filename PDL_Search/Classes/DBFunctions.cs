using Microsoft.AspNetCore.Mvc.Rendering;
using PDL_Search.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static PDL_Search.Classes.GainwellDatabaseExtensions;

namespace PDL_Search.Classes
{
    public static class DBFunctions
    {
        public static String GetJSON(DrugModel modelDrug)
        {
            // Get the data from the database as an ArrayList of Hashtables.
            String strJSONResult = "";
            String strSQL = @"SELECT RTRIM(rxnumber) As NDC,
	                          RTRIM(drugname) + '<br /><small>' + RTRIM(drugnamegeneric) + '</small>' As DrugName,
	                          RTRIM(drugnamebrand) As DrugClass,
	                          otcind As RxOTC,
	                          CASE WHEN drugclass = 'F' 
                                   THEN 'Preferred PDL Agent'
                                   ELSE 'Non-Preferred PDL Agent'
	                          END as PDLStatus			   
                              FROM [plandata_rx_production].[dbo].[claimpharm]";

            //******************
            //** NDC
            //******************
            if (!String.IsNullOrEmpty(modelDrug.NDC))
            {
                if (modelDrug.NDC == "test")
                    modelDrug.NDC = "055111034005";
                Hashtable htParameters = new Hashtable();
                htParameters.Add("NDC", modelDrug.NDC);

                strJSONResult = $"{strSQL} WHERE rxnumber=@NDC AND drugname!=''".GetSQLJSON(htParameters);
                return strJSONResult;
            }

            //******************
            //** GenericName
            //******************
            if (!String.IsNullOrEmpty(modelDrug.GenericName))
            {
                if (modelDrug.NDC == "test")
                    modelDrug.NDC = "055111034005";
                Hashtable htParameters = new Hashtable();
                htParameters.Add("GenericName", modelDrug.GenericName);

                strJSONResult = $"{strSQL} WHERE drugnamegeneric like '%' + @GenericName + '%' AND drugname!=''".GetSQLJSON(htParameters);
                return strJSONResult;
            }

            //******************
            //** BrandName
            //******************
            if (!String.IsNullOrEmpty(modelDrug.BrandName))
            {
                if (modelDrug.NDC == "test")
                    modelDrug.NDC = "055111034005";
                Hashtable htParameters = new Hashtable();
                htParameters.Add("BrandName", modelDrug.BrandName);

                strJSONResult = $"{strSQL} WHERE drugname like '%' + @BrandName + '%' AND drugname!=''".GetSQLJSON(htParameters);
                return strJSONResult;
            }

            //******************
            //** DrugClass
            //******************
            if (!String.IsNullOrEmpty(modelDrug.DrugClass))
            {
                if (modelDrug.NDC == "test")
                    modelDrug.NDC = "055111034005";
                Hashtable htParameters = new Hashtable();
                htParameters.Add("DrugClass", modelDrug.DrugClass);

                strJSONResult = $"{strSQL} WHERE drugnamebrand like '%' + @DrugClass + '%' AND drugname!=''".GetSQLJSON(htParameters);
                return strJSONResult;
            }

            //******************
            //** No Parameters    (returning results for now... can remove after testing)
            //******************
            strJSONResult = $"{strSQL} WHERE drugname != ''".GetSQLJSON();
            return strJSONResult;
        }
  
        public static List<SelectListItem> GetClassList()
        {
            // Query the database for the item list
            String strSQL = @"SELECT DISTINCT(RTRIM(drugnamebrand)) As DrugClass
                              FROM [plandata_rx_production].[dbo].[claimpharm] 
                              WHERE drugnamebrand != '';";

            ArrayList alResults = strSQL.GetSQLValues();

            // Create the list instance to return
            List<SelectListItem> lstFields = new List<SelectListItem>();

            // Add an empty item to the list.
            lstFields.Add(new SelectListItem("-Any Class-", ""));

            // Add the List Items from the Database
            foreach (String strItem in alResults)
            {
                SelectListItem sliNew = new SelectListItem();
                sliNew.Value = strItem;
                sliNew.Text = strItem;
                lstFields.Add(sliNew);
            }

            // Return the results
            return lstFields;
        }
    }
}





