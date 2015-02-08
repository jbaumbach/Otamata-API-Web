using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using OttaMatta.Common;

namespace OttaMatta.Data
{
    /// <summary>
    /// General class with a bunch of helper functions.
    /// </summary>
    public class DataFunctions
    {
        /// <summary>
        /// The write mode of the command.
        /// </summary>
        public enum ObjectWriteMode
        {
            Insert,
            Update,
            Delete
        }

        /// <summary>
        /// The result of an upsert call.
        /// </summary>
        public enum UpsertStatus
        {
            Unknown,
            New,
            Exists
        }

        /// <summary>
        /// Get the upsertion status from a result set.
        /// </summary>
        /// <param name="procResult"></param>
        /// <returns></returns>
        public static UpsertStatus ParseUpsertStatus(DataSet procResult)
        {
            UpsertStatus result = UpsertStatus.Unknown;

            if (Functions.HasRows(procResult))
            {
                string res = Functions.GetStringFromDataRow(procResult.Tables[0].Rows[0], "upsert_status");

                Enum.TryParse<UpsertStatus>(res, true, out result);
            }

            return result;
        }

        /// <summary>
        /// Get a DataSet object with the results of the database call.
        /// </summary>
        /// <param name="sql">SQL command to execute</param>
        /// <param name="sqlParams">Pameters to pass to sql command (if any)</param>
        /// <returns>A non-null dataset (may be empty)</returns>
        public static DataSet GetQueryResults(string sql, params SqlParameter[] sqlParams)
        {
            DataSet result = new DataSet();
            GetQueryResults(result, sql, sqlParams);
            return result;
        }

        /// <summary>
        /// Get a DataSet object with the results of a stored procedure call.
        /// </summary>
        /// <param name="sql">SQL command to execute</param>
        /// <param name="sqlParams">Pameters to pass to sql command (if any)</param>
        /// <returns>A non-null dataset (may be empty)</returns>
        public static DataSet GetProcResults(string sql, params SqlParameter[] sqlParams)
        {
            DataSet result = new DataSet();
            GetProcResults(result, sql, sqlParams);
            return result;
        }

        /// <summary>
        /// Fill this DataSet object with the results of the database call.  Only use this for inline SQL.
        /// </summary>
        /// <param name="ds">The dataset to fill</param>
        /// <param name="sql">SQL command to execute</param>
        /// <param name="sqlParams">Pameters to pass to sql command (if any)</param>
        private static void GetQueryResults(DataSet ds, string sql, params SqlParameter[] sqlParams)
        {
            //
            // Connect to the database
            //
            using (SqlConnection connection = new SqlConnection(ConnectionString()))
            {

                try
                {
                    connection.Open();

                    //
                    // Specify the command to use to query the database
                    //
                    SqlCommand command = new SqlCommand(sql, connection);

                    //
                    // Add parameters if we have any
                    //
                    if (sqlParams != null && sqlParams.Length > 0)
                    {
                        foreach (SqlParameter parameter in sqlParams)
                        {
                            command.Parameters.Add(parameter);
                        }

                        //
                        // Let's assume that a call with parameters is a stored proc
                        //
                        command.CommandType = CommandType.StoredProcedure;
                    }

                    //
                    // Connect to db and run the query
                    //
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                    dataAdapter.Fill(ds);
                }
                //catch(Exception e)
                //{
                //}
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Fill this DataSet object with the results of a stored procedure call.
        /// </summary>
        /// <param name="ds">The dataset to fill</param>
        /// <param name="sql">Stored procedure name</param>
        /// <param name="sqlParams">Pameters to pass to sql command (if any)</param>
        private static void GetProcResults(DataSet ds, string sql, params SqlParameter[] sqlParams)
        {
            //
            // Connect to the database
            //
            using (SqlConnection connection = new SqlConnection(ConnectionString()))
            {

                try
                {
                    connection.Open();

                    //
                    // Specify the command to use to query the database
                    //
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    //
                    // Add parameters if we have any
                    //
                    if (sqlParams != null && sqlParams.Length > 0)
                    {
                        foreach (SqlParameter parameter in sqlParams)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    //
                    // Connect to db and run the query
                    //
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                    dataAdapter.Fill(ds);
                }
                //catch(Exception e)
                //{
                //}
                finally
                {
                    connection.Close();
                }
            }
        }


        /// <summary>
        /// Execute a command that doesn't return stuff, with parameters if you got 'em.  Returns # of rows affected.  If this is
        /// a stored procedure, use 'ExecuteNonQueryProc()'.
        /// </summary>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="sqlParams">Optional - parameters.</param>
        /// <returns>Number of rows affected.</returns>
        /// <remarks>
        /// Note: if sql is a stored procedure, make sure the proc doesn't have "SET NOCOUNT ON".  That lines causes SQL to
        /// suppress the # of rows returned from the statement.  Then, the result of this function will always be -1.
        /// </remarks>
        public static int ExecuteNonQuery(string sql, params SqlParameter[] sqlParams)
        {
            int result = -1;

            //
            // Connect to the database
            //
            using (SqlConnection connection = new SqlConnection(ConnectionString()))
            {
                try
                {
                    connection.Open();

                    //
                    // Specify the command to use to query the database
                    //
                    SqlCommand command = new SqlCommand(sql, connection);
                    if (sqlParams != null)
                    {
                        command.CommandType = CommandType.StoredProcedure;
                    }

                    //
                    // Add parameters if we have any
                    //
                    foreach (SqlParameter parameter in sqlParams)
                    {
                        command.Parameters.Add(parameter);
                    }

                    result = command.ExecuteNonQuery();
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Execute a stored procedure that doesn't return stuff, with parameters if you got 'em.  Returns # of rows affected.
        /// </summary>
        /// <param name="sql">The sql to execute.</param>
        /// <param name="sqlParams">Optional - parameters.</param>
        /// <returns>Number of rows affected.</returns>
        public static int ExecuteNonQueryProc(string sql, params SqlParameter[] sqlParams)
        {
            int result = -1;

            //
            // Connect to the database
            //
            using (SqlConnection connection = new SqlConnection(ConnectionString()))
            {
                try
                {
                    connection.Open();

                    //
                    // Specify the command to use to query the database
                    //
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    //
                    // Add parameters if we have any
                    //
                    foreach (SqlParameter parameter in sqlParams)
                    {
                        command.Parameters.Add(parameter);
                    }

                    result = command.ExecuteNonQuery();
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }


        /// <summary>
        /// Gets the connection string for this computer to the database
        /// </summary>
        /// <returns>Connection string</returns>
        /// <remarks>
        /// See article on MSDN for connection stuff: "Accessing SQL Server with Explicit Credentials" (google it)
        /// ASPNET user is an explicit user created in SQL Manager, then associated with this DB and with role of "public".
        /// You have to manually grant permissions to public on all SQL server tables - I wrote a script for that, see the script.
        /// </remarks>
        public static string ConnectionString()
        {
            return Config.ConnectionString;
        }


    }
}
