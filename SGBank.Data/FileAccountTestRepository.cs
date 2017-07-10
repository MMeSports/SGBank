using SGBank.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGBank.Models;
using System.IO;

namespace SGBank.Data
{
    public class FileAccountRepository : IAccountRepository
    {
        private string _path = @"C:\Users\praisekek\OneDrive\1GuildBackup\Guild-github\firstRepo\matthew-wilkinson-individual-work\SGBank\Accounts.txt";

        public FileAccountRepository(string path)
        {
            path = _path;
        }

        public List<Account> AccountList()
        {
            List<Account> account = new List<Account>();

            using (StreamReader reader = new StreamReader(_path))
            {
                reader.ReadLine();
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    Account newAccount = new Account();

                    string[] columns = line.Split(',');

                    newAccount.AccountNumber = columns[0];
                    newAccount.Name = columns[1];
                    newAccount.Balance = decimal.Parse(columns[2]);
                    newAccount.Type = EnumTypeConverter(columns[3]);

                    account.Add(newAccount);
                }
            }

            return account;
        }

        public AccountType EnumTypeConverter(string accountType)
        {
            switch (accountType)
            {
                case "P":
                    return AccountType.Premium;
                case "B":
                    return AccountType.Basic;
                case "F":
                    return AccountType.Free;
                default:
                    throw new Exception("Not valid account type. Please try again!");
            }
        }



        public Account LoadAccount(string AccountNumber)
        {
            var account = AccountList();

            var findAcct = account.Find(x => x.AccountNumber == AccountNumber);

            return findAcct;
        }

        public void SaveAccount(Account acct)
        {
            var account = AccountList();

            var indexAcct = account.FindIndex(s => s.AccountNumber == acct.AccountNumber);

            account[indexAcct] = acct;

            CreateAccountFile(account);
        }

        private string CreateCsvforAccount(Account account)
        {
            string accountType;
            switch (account.Type)
            {
                case AccountType.Basic:
                    accountType = "B";
                    break;
                case AccountType.Free:
                    accountType = "F";
                    break;
                case AccountType.Premium:
                    accountType = "P";
                    break;
                default:
                    throw new Exception("Not valid account type. Please try again!");
            }
            return string.Format("{0},{1},{2},{3}", account.AccountNumber, account.Name, account.Balance.ToString(), accountType);
        }

        private void CreateAccountFile(List<Account> account)
        {
            if (File.Exists(_path))
                File.Delete(_path);

            using (StreamWriter reader = new StreamWriter(_path))
            {
                reader.WriteLine("AccountNumber, Name, Balance, AccountType");
                foreach (var acct in account)
                {
                    reader.WriteLine(CreateCsvforAccount(acct));
                }
            }
        }
    }
}
