using Microsoft.Data.SqlClient;
using Task2.Models;

namespace Task2.SqlCommandsHelpers
{
    public static class SqlHelper
    {
        public static void InsertBalanceAccount(int balanceAccountId, int classId, string fileName, string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand insertBalanceAccountsCommand = new SqlCommand(
                            $"BEGIN IF NOT EXISTS (SELECT * FROM Balance_Accounts WHERE AccountId = {balanceAccountId}) " +
                            $"BEGIN INSERT INTO Balance_Accounts(AccountId, Class) " +
                            $"VALUES({balanceAccountId}, {classId}) END END",
                            connection);
                insertBalanceAccountsCommand.ExecuteNonQuery();
                SqlCommand insertBalanceAccountsFilesCommand = new SqlCommand(
                            $"INSERT INTO BalanceAccounts_Files (AccountId, FileName) VALUES ({balanceAccountId}, '{fileName}')",
                            connection);
                insertBalanceAccountsFilesCommand.ExecuteNonQuery();

                connection.Close();
            }
        }

        public static void InsertOpeningBalance(int balanceAccountId, decimal openingBalanceAssets, 
            decimal openingBalanceLiabilities, string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand insertOpeningBalance = new SqlCommand("insert_opening_balance", connection);
                insertOpeningBalance.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter accountIdParam = new SqlParameter
                {
                    ParameterName = "@accountId",
                    Value = balanceAccountId
                };
                insertOpeningBalance.Parameters.Add(accountIdParam);
                SqlParameter assetsParam = new SqlParameter
                {
                    ParameterName = "@assets",
                    Value = openingBalanceAssets
                };
                insertOpeningBalance.Parameters.Add(assetsParam);
                SqlParameter liabilitiesParam = new SqlParameter
                {
                    ParameterName = "@liabilities",
                    Value = openingBalanceLiabilities
                };
                insertOpeningBalance.Parameters.Add(liabilitiesParam);

                insertOpeningBalance.ExecuteNonQuery();

                connection.Close();
            }
        }

        public static void InsertMoneyTurnover(int balanceAccountId, decimal moneyTurnoverDebit,
            decimal moneyTurnoverCredit, string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand insertMoneyTurnover = new SqlCommand("insert_money_turnover", connection);
                insertMoneyTurnover.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter accountIdParam = new SqlParameter
                {
                    ParameterName = "@accountId",
                    Value = balanceAccountId
                };
                insertMoneyTurnover.Parameters.Add(accountIdParam);
                SqlParameter debitParam = new SqlParameter
                {
                    ParameterName = "@debit",
                    Value = moneyTurnoverDebit
                };
                insertMoneyTurnover.Parameters.Add(debitParam);
                SqlParameter creditParam = new SqlParameter
                {
                    ParameterName = "@credit",
                    Value = moneyTurnoverCredit
                };
                insertMoneyTurnover.Parameters.Add(creditParam);

                insertMoneyTurnover.ExecuteNonQuery();

                connection.Close();
            }
        }

        public static void InsertClosingBalance(int balanceAccountId, decimal closingBalanceAssets,
            decimal closingBalanceLiabilities, string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand insertClosingBalance = new SqlCommand("insert_closing_balance", connection);
                insertClosingBalance.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter accountIdParam = new SqlParameter
                {
                    ParameterName = "@accountId",
                    Value = balanceAccountId
                };
                insertClosingBalance.Parameters.Add(accountIdParam);
                SqlParameter assetsParam = new SqlParameter
                {
                    ParameterName = "@assets",
                    Value = closingBalanceAssets
                };
                insertClosingBalance.Parameters.Add(assetsParam);
                SqlParameter liabilitiesParam = new SqlParameter
                {
                    ParameterName = "@liabilities",
                    Value = closingBalanceLiabilities
                };
                insertClosingBalance.Parameters.Add(liabilitiesParam);

                insertClosingBalance.ExecuteNonQuery();

                connection.Close();
            }
        }

        public static List<Data> GetData(string fileName, string connectionString)
        {
            var dataList = new List<Data>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand getData = new SqlCommand("get_data", connection);
                getData.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter fileNameParam = new SqlParameter
                {
                    ParameterName = "@fileName",
                    Value = fileName
                };
                getData.Parameters.Add(fileNameParam);

                var reader = getData.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var data = new Data()
                        {
                            AccountId = reader.GetInt32(0),
                            OpeningBalanceAssets = reader.GetDecimal(1),
                            OpeningBalanceLiabilities = reader.GetDecimal(2),
                            MoneyTurnoverDebit = reader.GetDecimal(3),
                            MoneyTurnoverCredit = reader.GetDecimal(4),
                            ClosingBalanceAssets = reader.GetDecimal(5),
                            ClosingBalanceLiabilities = reader.GetDecimal(6),
                            ClassId = reader.GetInt32(7),
                            ClassName = reader.GetString(8),
                        };
                        data.AccountGroup = int.Parse(data.AccountId.ToString().Substring(0, 2));
                        dataList.Add(data);
                    }
                }
                reader.Close();

                connection.Close();
            }

            return dataList;
        }
    }
}
