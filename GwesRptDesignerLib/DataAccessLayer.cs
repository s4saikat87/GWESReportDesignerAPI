using System;
using System.Data;

using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;


namespace GwesRptDesignerLib
{
    public class DataAccessLayer
    {
        private const string ValidateUser = "Rt_ValidateUser";
        private const string RefreshTokenSet = "RT_RefreshTokenSet";
        private const string RefreshTokenGet = "RT_RefreshTokenGet";
        public DataSet DsValidateUser(string connectionString, string userName, string? password=null)
        {
            var resultDataSet = new DataSet();
            var resultDataTable = new DataTable();
            try
            {
                
                DbCommand dbCommand = null;
                Database dataBase = new SqlDatabase(connectionString);


                dbCommand = dataBase.GetStoredProcCommand(ValidateUser);
                dbCommand.CommandTimeout = 0;
                dbCommand.CommandType = CommandType.StoredProcedure;
                dataBase.AddInParameter(dbCommand, name: "EmailAdrs", dbType: DbType.String, value: userName);
                if (password!=null && password!="".Trim() && password.Length > 0)
                {
                    dataBase.AddInParameter(dbCommand, name: "Password", dbType: DbType.String, value: password);
                }
                
                resultDataSet = dataBase.ExecuteDataSet(dbCommand);

                resultDataTable = resultDataSet.Tables[0];
                resultDataTable.TableName = "Results";
            }
            catch (Exception dalException)
            {
                resultDataTable = new DataTable();

                resultDataTable.Columns.Add("ErrorMessage", typeof(string));
                resultDataTable.Columns.Add("StackTrace", typeof(string));
                var dataRow=resultDataTable.NewRow();
                dataRow[0]=dalException.Message;
                dataRow[1] = dalException.StackTrace?.ToString();
                resultDataTable.Rows.Add(dataRow);
                resultDataTable.TableName = "Error";
                resultDataSet = new DataSet();
                resultDataSet.Tables.Add(resultDataTable);
            }
            return resultDataSet;
        }

        public DataSet DsRefreshTokenSet(string connectionString, int userId, string refreshToken, DateTime refreshTokenExpiryTime)
        {
            var resultDataSet = new DataSet();
            var resultDataTable = new DataTable();
            try
            {

                DbCommand dbCommand = null;
                Database dataBase = new SqlDatabase(connectionString);


                dbCommand = dataBase.GetStoredProcCommand(RefreshTokenSet);
                dbCommand.CommandTimeout = 0;
                dbCommand.CommandType = CommandType.StoredProcedure;
                dataBase.AddInParameter(dbCommand, name: "UserId", dbType: DbType.Int32, value: userId);
                dataBase.AddInParameter(dbCommand, name: "RefreshToken", dbType: DbType.String, value: refreshToken);
                dataBase.AddInParameter(dbCommand, name: "RefreshTokenExpiryTime", dbType: DbType.DateTime, value: refreshTokenExpiryTime);
                resultDataSet = dataBase.ExecuteDataSet(dbCommand);

                resultDataTable = resultDataSet.Tables[0];
                resultDataTable.TableName = "Results";
            }
            catch (Exception dalException)
            {
                resultDataTable = new DataTable();

                resultDataTable.Columns.Add("ErrorMessage", typeof(string));
                resultDataTable.Columns.Add("StackTrace", typeof(string));
                var dataRow = resultDataTable.NewRow();
                dataRow[0] = dalException.Message;
                dataRow[1] = dalException.StackTrace?.ToString();
                resultDataTable.Rows.Add(dataRow);
                resultDataTable.TableName = "Error";
                resultDataSet = new DataSet();
                resultDataSet.Tables.Add(resultDataTable);
            }
            return resultDataSet;
        }

        public DataSet DsRefreshTokenGet(string connectionString, string userName)
        {
            var resultDataSet = new DataSet();
            var resultDataTable = new DataTable();
            try
            {

                DbCommand dbCommand = null;
                Database dataBase = new SqlDatabase(connectionString);


                dbCommand = dataBase.GetStoredProcCommand(RefreshTokenGet);
                dbCommand.CommandTimeout = 0;
                dbCommand.CommandType = CommandType.StoredProcedure;
                dataBase.AddInParameter(dbCommand, name: "UserName", dbType: DbType.String, value: userName);                
                resultDataSet = dataBase.ExecuteDataSet(dbCommand);

                resultDataTable = resultDataSet.Tables[0];
                resultDataTable.TableName = "Results";
            }
            catch (Exception dalException)
            {
                resultDataTable = new DataTable();

                resultDataTable.Columns.Add("ErrorMessage", typeof(string));
                resultDataTable.Columns.Add("StackTrace", typeof(string));
                var dataRow = resultDataTable.NewRow();
                dataRow[0] = dalException.Message;
                dataRow[1] = dalException.StackTrace?.ToString();
                resultDataTable.Rows.Add(dataRow);
                resultDataTable.TableName = "Error";
                resultDataSet = new DataSet();
                resultDataSet.Tables.Add(resultDataTable);
            }
            return resultDataSet;
        }

        public DataSet DsRefreshTokenDel(string connectionString, int userId)
        {
            var resultDataSet = new DataSet();
            var resultDataTable = new DataTable();
            try
            {

                DbCommand dbCommand = null;
                Database dataBase = new SqlDatabase(connectionString);


                dbCommand = dataBase.GetStoredProcCommand(RefreshTokenSet);
                dbCommand.CommandTimeout = 0;
                dbCommand.CommandType = CommandType.StoredProcedure;
                dataBase.AddInParameter(dbCommand, name: "UserId", dbType: DbType.Int32, value: userId);
                dataBase.AddInParameter(dbCommand, name: "IsDeleted", dbType: DbType.Int32, value: 1);
                
                resultDataSet = dataBase.ExecuteDataSet(dbCommand);

                resultDataTable = resultDataSet.Tables[0];
                resultDataTable.TableName = "Results";
            }
            catch (Exception dalException)
            {
                resultDataTable = new DataTable();

                resultDataTable.Columns.Add("ErrorMessage", typeof(string));
                resultDataTable.Columns.Add("StackTrace", typeof(string));
                var dataRow = resultDataTable.NewRow();
                dataRow[0] = dalException.Message;
                dataRow[1] = dalException.StackTrace?.ToString();
                resultDataTable.Rows.Add(dataRow);
                resultDataTable.TableName = "Error";
                resultDataSet = new DataSet();
                resultDataSet.Tables.Add(resultDataTable);
            }
            return resultDataSet;
        }
    }
}