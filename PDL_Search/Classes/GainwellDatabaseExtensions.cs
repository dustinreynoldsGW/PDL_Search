using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PDL_Search.Classes
{
    public static class GainwellDatabaseExtensions
    {
        // Configurable Constants
        private const String CONNECTIONSTRING_NAME = "GainwellConnection";

        public static ArrayList GetSQLData(this String strSQL, Hashtable htParameters = null, String strConnectionString = CONNECTIONSTRING_NAME)
        {
            using (MsSQLFunctions MsSQL_Functions = new MsSQLFunctions())
            {
                return MsSQL_Functions.GetSQLData(strSQL, htParameters);
            }
        }

        public static String GetSQLJSON(this String strSQL, Hashtable htParameters = null, String strConnectionString = CONNECTIONSTRING_NAME)
        {
            using (MsSQLFunctions MsSQL_Functions = new MsSQLFunctions())
            {
                return MsSQL_Functions.GetSQLJSON(strSQL, htParameters);
            }
        }

        public static ArrayList GetSQLValues(this String strSQL, Hashtable htParameters = null, String strConnectionString = CONNECTIONSTRING_NAME)
        {
            using (MsSQLFunctions MsSQL_Functions = new MsSQLFunctions())
            {
                return MsSQL_Functions.GetSQLValues(strSQL, htParameters);
            }
        }

        public static Object GetSQLObject(this String strSQL, Hashtable htParameters = null, String strConnectionString = CONNECTIONSTRING_NAME)
        {
            using (MsSQLFunctions MsSQL_Functions = new MsSQLFunctions())
            {
                return MsSQL_Functions.GetSQLObject(strSQL, htParameters);
            }
        }

        public static Object GetSQLDataTable(this String strSQL, Hashtable htParameters = null, String strConnectionString = CONNECTIONSTRING_NAME)
        {
            using (MsSQLFunctions MsSQL_Functions = new MsSQLFunctions())
            {
                return MsSQL_Functions.GetSQLDataTable(strSQL, htParameters);
            }
        }

        /// <summary>
        /// Executes a SQL command that doesn't need table data returned.  This CAN be a stored procedure.
        /// </summary>
        /// <param name="strSQL">The SQL to execute</param>
        /// <param name="htParameters">Any parameters for the SQL</param>
        /// <param name="blnShouldReturnIndex"></param>
        /// <param name="strConnectionString">Defaults to GainwellConnection.  This can be any connection string in the system</param>
        /// <returns>boolean true/false if successful (may have been recoded.... need to check code in execsql & retest this</returns>
        /// <returns>if ShouldReturnIndex is true (ie. for an SQL INSERT), Uses @@SqlIdentity to return the ID of the last object inserted,</returns>
        public static Object ExecSQL(this String strSQL, Hashtable htParameters = null, Boolean blnShouldReturnIndex = false, String strConnectionString = CONNECTIONSTRING_NAME)
        {
            using (MsSQLFunctions MsSQL_Functions = new MsSQLFunctions())
            {
                return MsSQL_Functions.ExecSQL(strSQL, htParameters, blnShouldReturnIndex, strConnectionString);
            }
        }

        public static Object ExecSQL(String strProcName,
                                     Hashtable htParams = null,
                                     String strConnectionStringName = CONNECTIONSTRING_NAME)
        {
            using (MsSQLFunctions MsSQL_Functions = new MsSQLFunctions())
            {
                return MsSQL_Functions.ExecStoredProcedure(strProcName, htParams, strConnectionStringName);
            }
        }
    }
}