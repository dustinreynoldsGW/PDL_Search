using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace PDL_Search.Classes
{
    public class MsSQLFunctions : IDisposable
    {
        // Configurable Constants
        private const String CONNECTIONSTRING_NAME = "GainwellConnection";

        /// <summary>
        /// Constructor
        /// </summary>
        public MsSQLFunctions()
        {
            //Environment.MachineName;
        }

        ///// <summary>
        ///// Return a connection string
        ///// </summary>
        ///// <param name="ConnectionStringName"></param>
        ///// <returns>The full connection string of the specified connection string name</returns>
        public String GetDBConnectionString(String ConnectionStringName)
        {
            return "data source=10.143.61.49;initial catalog=QCSIDB_Rx_Ref;user id=mms_rx_rw;password=DevRxRw8";

            //if (ConnectionStringName == null)
            //    ConnectionStringName = "";
            //if (ConnectionStringName.Equals(String.Empty))
            //    return System.Configuration.ConfigurationManager.ConnectionStrings[CONNECTIONSTRING_NAME].ConnectionString;
            //else
            //    return System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
        }


        /// <summary>
        /// Gets a DBConnection Object
        /// </summary>
        /// <param name="strConnectionStringName"></param>
        /// <returns>DBConnection Object</returns>
        /// <remarks>DBConnection object is not yet open on return.</remarks>
        public DbConnection GetDBConnection(String strConnectionStringName)
        {
            String connString = GetDBConnectionString(strConnectionStringName);
            String provider = "System.Data.SqlClient";

            // Get the provider
            DbProviderFactory fact = DbProviderFactories.GetFactory(provider);

            // Create the connection      
            //SqlConnection conn = new SqlConnection(connString);   
            DbConnection conn = fact.CreateConnection();
            conn = new SqlConnection(connString);
            return conn;
        }

        // Return an SqlConnection to the database
        //   parameters:
        //      Connection String Name
        /// <summary>
        /// Gets a sqlconnection object based on the connection string parameter
        /// </summary>
        /// <param name="strConnectionStringName"></param>
        /// <returns>an open SQLConnection Object</returns>
        public SqlConnection GetSQLConnection(String strConnectionStringName)
        {
            // Open the connection
            String strConnectionString = GetDBConnectionString(strConnectionStringName);
            SqlConnection conn = new SqlConnection(strConnectionString);
            conn.Open();
            return conn;
        }


        /// <summary>
        /// Gets a set of values from a single SQL statement.
        /// </summary>
        /// <param name="strConnectionStringName"></param>
        /// <param name="strSQL"></param>
        /// <returns>An arraylist in containing the respective objects</returns>
        public ArrayList GetSQLValues(String strSQL, Hashtable htParams = null)
        {
            return GetSQLValues(CONNECTIONSTRING_NAME, strSQL, htParams);
        }

        public ArrayList GetSQLValues(String strConnectionStringName, String strSQL, Hashtable htParameters = null)
        {
            ArrayList alResults = new ArrayList();
            SqlConnection conn = null;
            try
            {
                // Open DB Connection values
                conn = GetSQLConnection(strConnectionStringName);

                // Build an SQL command for the insert
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strSQL;

                // get the sqldbtype for each object in the hashtable
                if (htParameters != null)
                {
                    foreach (DictionaryEntry de in htParameters)
                    {
                        cmd.Parameters.Add(de.Key.ToString(), GetSQLType(de.Value)).Value = de.Value;
                    }
                }

                // Execute the SQL command
                SqlDataReader myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    alResults.Add(myReader[0]);
                }
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                try
                {
                    conn.Close();
                    conn.Dispose();
                }
                catch (Exception) { }
            }
            return alResults;
        }

        /// <summary>
        /// Return a single object using an SQL query
        /// </summary>
        /// <param name="strConnectionStringName"></param>
        /// <param name="strSQL"></param>
        /// <param name="htParameters"></param>
        /// <returns>a single object based on the SQL query</returns>
        /// <example>
        ///  The following code would return the first name of the specified employee in a String object.
        /// <code>
        /// String empFirstName = (String)GetSQLObject("WebDB1Pool", SELECT emp_fn FROM webtest.dbo.web_demog WHERE uid = 'jsmith');
        /// </code>
        /// </example>
        /// <remarks>You will need to cast the object as the type you desire after return.</remarks>       
        public Object GetSQLObject(String strSQL, Hashtable htParams = null)
        {
            return GetSQLObject(CONNECTIONSTRING_NAME, strSQL, htParams);
        }
        public Object GetSQLObject(String strConnectionStringName, String strSQL, Hashtable htParameters = null)
        {
            Object objResult = null;
            SqlConnection conn = null;
            try
            {
                // Open DB Connection
                conn = GetSQLConnection(strConnectionStringName);

                // Build an SQL command for the insert
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strSQL;

                if (htParameters != null)
                {
                    foreach (DictionaryEntry de in htParameters)
                    {
                        cmd.Parameters.AddWithValue(de.Key.ToString(), de.Value);
                    }
                }

                // Execute the SQL command
                objResult = cmd.ExecuteScalar();

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                try
                {
                    conn.Close();
                    conn.Dispose();
                }
                catch (Exception) { }
            }
            return objResult;
        }

        /// <summary>
        /// Creates and executes a parameterized insert SqlCommand for the values in the specified hashtable. 
        /// </summary>
        /// <param name="HasIdentitySeed">Boolean. Whether the insert is into a table with an identity seed or not.</param>
        /// <param name="ConnectionStringName">String.  Name of the connectionstring to use.</param>
        /// <param name="TableName">String.  Name of the table to insert to.  Should be of the form databasename.dbo.tablename</param>
        /// <param name="Values">Hashtable. full of the Key-Value pairs to insert. </param>
        /// <returns>Int64 value, either number of rows inserted (if not identity seed) or the identity seed.</returns>
        /// <example>
        /// The following code will insert a row and return an the identity seed value in an Int64 object.
        /// <code>
        /// // create the hashtable with first name, last name, and userid values
        /// Hashtable rowToInsert = new Hashtable();
        /// rowToInsert.Add("FirstName", "John");
        /// rowToInsert.Add("LastName", "Smith");
        /// rowToInsert.Add("UserID", "jsmith");
        /// 
        /// // if the table has an identity seed, the identity seed value for the inserted row will be returned.
        /// // otherwise, the value 1 will be returned to indicate the number of rows inserted.
        /// Int64 rowIdentitySeed = InsertValues(true, "WebDB1Pool", "PatientDB.dbo.PatientTable", rowToInsert);
        /// </code>
        /// </example>
        /// 
        public Int64 InsertValues(Boolean HasIdentitySeed, String ConnectionStringName, String TableName, Hashtable Values)
        {
            Int64 returnVal = 0;

            SqlConnection conn = GetSQLConnection(ConnectionStringName);

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = conn;

            // stringbuilders for the various sections of the sql statement
            StringBuilder SQLStatementStringBuilder = new StringBuilder();
            StringBuilder FieldsSection = new StringBuilder();
            StringBuilder ValuesSection = new StringBuilder();

            // initial parts of the section strings
            FieldsSection.Append("(");
            ValuesSection.Append(" VALUES(");

            String FieldName;

            foreach (DictionaryEntry de in Values)
            {
                // add to the fields string section
                FieldName = de.Key.ToString();
                FieldsSection.Append("[" + FieldName + "],");

                // add to the values string section
                ValuesSection.Append("@" + FieldName + ",");
            }

            // remove the trailing commas and add a closing paran to each string section
            int flen = FieldsSection.Length;
            flen--;
            FieldsSection.Remove(flen, 1);
            FieldsSection.Append(") ");
            flen = ValuesSection.Length;
            flen--;
            ValuesSection.Remove(flen, 1);
            ValuesSection.Append(")");

            // add the insert statement and append the sections to complete the sql string
            SQLStatementStringBuilder.Append("INSERT INTO " + TableName);
            SQLStatementStringBuilder.Append(FieldsSection.ToString());
            SQLStatementStringBuilder.Append(ValuesSection.ToString());

            // if the table has an identity seed key, get, get the identity for returning
            if (HasIdentitySeed)
            {
                SQLStatementStringBuilder.Append("; Select @@Identity");
            }
            String strSQL = SQLStatementStringBuilder.ToString();

            cmd.CommandText = strSQL;

            // run the parameter calls
            foreach (DictionaryEntry de in Values)
            {

                // deprecated 
                //cmd.Parameters.Add("@" + de.Key.ToString(), dbType).Value = de.Value;
                if ((de.Value.GetType() == typeof(DateTime) || de.Value.GetType() == typeof(DateTime?)) && de.Value != null && (DateTime)de.Value < new DateTime(1753, 1, 2))
                {
                    cmd.Parameters.AddWithValue("@" + de.Key.ToString(), SqlDbType.DateTime).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.AddWithValue("@" + de.Key.ToString(), de.Value);
                }
            }
            Object x = cmd.ExecuteScalar();
            returnVal = Convert.ToInt64(x);
            conn.Close();
            return returnVal;
        }

        /// <summary>
        /// Tests the CLR type of the object and attmpts to translate it to a SqlDbType
        /// </summary>
        /// <param name="inputObject">Object to get the SqlDbType of</param>
        /// <returns>the object's SqlDbType</returns>
        /// <remarks>Returns SqlDbType.Udt (User defined type) if no data type is recognized</remarks>
        private SqlDbType GetSQLType(Object inputObject)
        {

            System.Type objType = inputObject.GetType();
            SqlDbType returnType = new SqlDbType();

            switch (objType.ToString())
            {
                case "System.String":
                case "System.Char[]":
                    returnType = SqlDbType.VarChar;
                    break;
                case "System.Int16":

                    returnType = SqlDbType.SmallInt;
                    break;
                case "System.UInt16":
                case "System.Int32":
                    returnType = SqlDbType.Int;
                    break;

                case "System.UInt32":
                case "System.Int64":
                    returnType = SqlDbType.BigInt;

                    break;
                case "System.DateTime":
                    returnType = SqlDbType.DateTime;
                    break;
                case "System.Boolean":
                    returnType = SqlDbType.Bit;
                    break;
                case "System.Double":
                    returnType = SqlDbType.Float;
                    break;
                case "System.Byte":
                    returnType = SqlDbType.TinyInt;
                    break;
                case "System.Byte[]":
                    returnType = SqlDbType.Image;
                    break;
                case "System.Decimal":
                    returnType = SqlDbType.Decimal;
                    break;

                default:
                    // default to user defined type for sql2005
                    returnType = SqlDbType.BigInt;
                    break;
            }
            return returnType;
        }

        public Object ExecSQL(String strSQL = "SELECT @@version",
                              Hashtable htParams = null,
                              Boolean blnReturnIndex = false,
                              String strConnectionStringName = CONNECTIONSTRING_NAME)
        {
            SqlConnection conn = null;
            Boolean blnResult = false;
            Object objReturnedIndex = null;

            if (strSQL.Contains("!autoAdd"))
            {
                int intCount = 0;
                StringBuilder sbColumns = new StringBuilder();
                StringBuilder sbValues = new StringBuilder();

                if ((htParams != null))
                {
                    sbColumns.Append("(");
                    sbValues.Append("VALUES (");
                    foreach (DictionaryEntry de in htParams)
                    {
                        intCount++; // Increment Counter to decide on placement of commas

                        String tmpKey = de.Key.ToString().Substring(0);
                        sbColumns.Append(tmpKey);
                        sbValues.Append("@" + tmpKey);
                        if (intCount < htParams.Count)
                        {
                            sbColumns.Append(", ");
                            sbValues.Append(", ");
                        }
                    }
                    sbColumns.Append(")");
                    sbValues.Append(")");
                }
                strSQL = strSQL.Replace("!autoAdd", sbColumns.ToString() + " " + sbValues.ToString());
            }

            if (strSQL.Contains("!autoSet"))
            {
                int intNewParamCount = htParams.Count;

                int intCount = 0;
                StringBuilder sbValues = new StringBuilder();

                if ((htParams != null))
                {
                    sbValues.Append("SET");
                    foreach (DictionaryEntry de in htParams)
                    {
                        intCount++; // Increment Counter to decide on placement of commas

                        String tmpKey = de.Key.ToString().Substring(1);
                        if (!tmpKey.Equals("autoIndex"))
                        {
                            sbValues.Append(" " + tmpKey + "=@" + tmpKey);
                            if (intCount < intNewParamCount)
                            {
                                sbValues.Append(", ");
                            }
                        }
                    }
                }
                String strValue = sbValues.ToString();
                if (strValue.EndsWith(", "))
                    strValue.Equals(strValue.Substring(0, strValue.Length - 2));

                strSQL = strSQL.Replace("!autoSet", sbValues.ToString());
            }

            if (strSQL.Contains("!autoWhere"))
            {
                int intCount = 0;
                StringBuilder sbValues = new StringBuilder();

                if ((htParams != null))
                {
                    sbValues.Append(" WHERE");
                    foreach (DictionaryEntry de in htParams)
                    {
                        intCount++; // Increment Counter to decide on placement of commas

                        String tmpKey = de.Key.ToString().Substring(1);
                        sbValues.Append(" (" + tmpKey + "=@" + tmpKey + ")");
                        if (intCount < htParams.Count)
                        {
                            sbValues.Append(" AND ");
                        }
                    }
                }
                strSQL = strSQL.Replace("!autoWhere", sbValues.ToString());
            }

            try
            {
                // Open DB Connection
                conn = GetSQLConnection(strConnectionStringName);

                // Build an SQL command for the insert
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strSQL;

                // get the sqldbtype for each object in the hashtable
                if ((htParams != null))
                {
                    foreach (DictionaryEntry de in htParams)
                    {
                        cmd.Parameters.AddWithValue(de.Key.ToString(), de.Value);
                    }
                }

                // Execute the SQL command
                if (blnReturnIndex)
                {
                    cmd.CommandText = strSQL + "; Select @@Identity";
                    objReturnedIndex = cmd.ExecuteScalar();
                }
                else
                {
                    cmd.ExecuteNonQuery();
                    blnResult = true;
                }
                cmd.Dispose();

            }
            catch (Exception ex)
            {
                //HttpContext.Current.Session["SQLMessage"] = ex.Message;
                //HttpContext.Current.Session["SQLException"] = ex.InnerException;
                //ex.EmailError(strSQL);
            }
            finally
            {
                try
                {
                    if (conn != null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
                catch (Exception) { }
            }
            if (blnReturnIndex)
                return objReturnedIndex;
            else
            {
                if (blnResult)
                    return 1;
                else
                    return 0;
            }
        }


        public DataTable GetSQLDataTable2(String strSQL,
                                          Hashtable htParams = null,
                                          String strConnectionStringName = CONNECTIONSTRING_NAME)
        {
            SqlConnection conn = null;
            DataTable dtResults = new DataTable();

            try
            {
                // Open DB Connection
                conn = GetSQLConnection(strConnectionStringName);

                // Build an SQL command for the insert
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strSQL;

                if (htParams != null)
                {
                    foreach (DictionaryEntry de in htParams)
                    {
                        cmd.Parameters.Add(de.Key.ToString(), GetSQLType(de.Value)).Value = de.Value;
                    }
                }

                // Execute the SQL command
                SqlDataReader myReader = cmd.ExecuteReader();

                // Get Schema
                System.Data.DataTable schema = myReader.GetSchemaTable();
                while (myReader.Read())
                {
                    for (int i = 0; i < myReader.FieldCount; i++)
                    {
                        String columnName = schema.Rows[i]["columnName"].ToString();
                        dtResults.Columns.Add(columnName);
                    }

                    DataRow dtRow = dtResults.NewRow();
                    for (int i = 0; i < myReader.FieldCount; i++)
                    {
                        String columnName = schema.Rows[i]["columnName"].ToString();

                        // if the return value is the number of rows effected or an identity seed, give the field the keyname "result"
                        if (columnName.Equals("")) columnName = "result";

                        dtRow[columnName] = myReader[i];
                    }
                    dtResults.Rows.Add();
                }
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    conn.Close();
                    conn.Dispose();
                }
                catch (Exception) { }
            }
            return dtResults;
        }

        public ArrayList GetSQLData(String strSQL,
                                    Hashtable htParams = null,
                                    String strConnectionStringName = CONNECTIONSTRING_NAME)
        {
            SqlConnection conn = null;
            ArrayList alResults = new ArrayList();

            try
            {
                // Open DB Connection
                conn = GetSQLConnection(strConnectionStringName);

                // Build an SQL command for the insert
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strSQL;

                if (htParams != null)
                {
                    foreach (DictionaryEntry de in htParams)
                    {
                        cmd.Parameters.Add(de.Key.ToString(), GetSQLType(de.Value)).Value = de.Value;
                    }
                }

                // Execute the SQL command
                SqlDataReader myReader = cmd.ExecuteReader();

                // Get Schema
                System.Data.DataTable schema = myReader.GetSchemaTable();
                while (myReader.Read())
                {
                    Hashtable htResults = new Hashtable();
                    for (int i = 0; i < myReader.FieldCount; i++)
                    {
                        String columnName = schema.Rows[i]["columnName"].ToString();

                        // if the return value is the number of rows effected or an identity seed, give the field the keyname "result"
                        if (columnName.Equals("")) columnName = "result";

                        htResults.Add(columnName, myReader[i]);
                    }
                    alResults.Add(htResults);
                }
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    conn.Close();
                    conn.Dispose();
                }
                catch (Exception) { }
            }
            return alResults;
        }


        public String GetSQLJSON(String strSQL,
                      Hashtable htParams = null,
                      String strConnectionStringName = CONNECTIONSTRING_NAME)
        {
            SqlConnection conn = null;
            ArrayList alResults = new ArrayList();

            try
            {
                // Open DB Connection
                conn = GetSQLConnection(strConnectionStringName);

                // Build an SQL command for the insert
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strSQL;

                if (htParams != null)
                {
                    foreach (DictionaryEntry de in htParams)
                    {
                        cmd.Parameters.Add(de.Key.ToString(), GetSQLType(de.Value)).Value = de.Value;
                    }
                }

                // Create a StringBuilder to create the JSON results.
                StringBuilder sbData = new StringBuilder();

                // blnNew is used to indicate we are not on the first row so that a comma can be added between results
                Boolean blnNew = true;

                // Execute the SQL command
                SqlDataReader myReader = cmd.ExecuteReader();

                // Get Schema
                System.Data.DataTable schema = myReader.GetSchemaTable();

                // Start the JSON Array
                sbData.Append("[");

                while (myReader.Read())
                {
                    // Only add a comma before the row if it is NOT the first one
                    if (!blnNew)
                        sbData.Append(",");

                    // Create the record set for JSON
                    sbData.Append("{");

                    for (int i = 0; i < myReader.FieldCount; i++)
                    {
                        String columnName = schema.Rows[i]["columnName"].ToString();

                        // if the return value is the number of rows effected or an identity seed, give the field the keyname "result"
                        if (columnName.Equals("")) columnName = "result";

                        sbData.Append($@"""{columnName}"":""{myReader[i]}""");

                        if (i < (myReader.FieldCount - 1))
                            sbData.Append(",");
                    }
                    sbData.Append("}");

                    // Set blnNew to false so that commas can be added before additional rows.
                    blnNew = false;
                }

                // Close the JSON Array
                sbData.Append("]");

                // Return the Results
                return sbData.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    conn.Close();
                    conn.Dispose();
                }
                catch (Exception) { }
            }            
        }


        public ArrayList ExecStoredProcedure(String strProcName,
                                                 Hashtable htParams = null,
                                                 String strConnectionStringName = CONNECTIONSTRING_NAME)
        {
            SqlConnection conn = null;
            ArrayList alResults = new ArrayList();

            try
            {
                // Open DB Connection


                conn = GetSQLConnection(strConnectionStringName);

                // Build an SQL command for the insert
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = strProcName;


                if (htParams != null)
                {
                    foreach (DictionaryEntry de in htParams)
                    {
                        SqlDbType @type = GetSQLType(de.Value);
                        cmd.Parameters.Add(de.Key.ToString(), @type).Value = de.Value;
                    }
                }

                // Execute the SQL command
                SqlDataReader myReader = cmd.ExecuteReader();
                //object myReader = cmd.ExecuteScalar();

                // Get Schema
                System.Data.DataTable schema = myReader.GetSchemaTable();
                while (myReader.Read())
                {
                    Hashtable htResults = new Hashtable();
                    for (int i = 0; i < myReader.FieldCount; i++)
                    {
                        String columnName = schema.Rows[i]["columnName"].ToString();

                        // if the return value is the number of rows effected or an identity seed, give the field the keyname "result"
                        if (columnName.Equals("")) columnName = "result";

                        htResults.Add(columnName, myReader[i]);
                    }
                    alResults.Add(htResults);
                }
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    conn.Close();
                    conn.Dispose();
                }
                catch (Exception) { }
            }
            return alResults;
        }

        public System.Data.DataTable GetSQLDataTable(string strSQL, Hashtable htParams = null, String strConnectionStringName = CONNECTIONSTRING_NAME)
        {
            ArrayList results = GetSQLData(strSQL, htParams, strConnectionStringName);
            if (results == null) return null;

            System.Data.DataTable dt = new System.Data.DataTable();
            if (results.Count == 0)
                return null;
            Hashtable htFirstItem = (Hashtable)results[0];

            foreach (String myKey in htFirstItem.Keys)
            {
                dt.Columns.Add(new DataColumn(myKey, typeof(string)));
            }

            foreach (Hashtable htResult in results)
            {
                // Create the four records
                DataRow dr = dt.NewRow();
                foreach (String myKey in htFirstItem.Keys)
                    dr[myKey] = htResult[myKey];
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public void Dispose()
        {
            //this.Dispose();
            //throw new NotImplementedException();
        }
    }
}