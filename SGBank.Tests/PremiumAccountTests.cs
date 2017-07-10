using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SGBank.BLL;
using SGBank.Models.Responses;
using SGBank.Models;
using SGBank.Models.Interfaces;
using SGBank.BLL.DepositRules;
using SGBank.BLL.WithdrawRules;

namespace SGBank.Tests
{
    [TestFixture]
    public class PremiumAccountTests
    {
        [TestCase("33333", true)]
        public void CanLoadAccountTestData(string testData, bool expected)
        {
            AccountManager manager = AccountManagerFactory.Create();
            AccountLookupResponse response = manager.LookupAccount(testData);

            Assert.AreEqual(testData, response.Account.AccountNumber);
        }

        [TestCase("44444", "Premium Account", 500, AccountType.Premium, 100, true)]
        [TestCase("44444", "Premium Account", 500, AccountType.Premium, -100, false)]
        [TestCase("44444", "Premium Account", 100, AccountType.Free, 250, false)]
        [TestCase("44444", "Premium Account", -200, AccountType.Premium, 250, true)]
        public void PremiumAccountDepositRuleTest(string accountNumber, string name, decimal balance, AccountType accountType, decimal amount, bool expectedResult)
        {
            IDeposit deposit = new NoLimitDepositRule();
            Account account = new Account() { AccountNumber = accountNumber, Balance = balance, Name = name, Type = accountType };
            AccountDepositResponse response = deposit.Deposit(account, amount);

            Assert.AreEqual(response.Success, expectedResult);
        }

        [TestCase("44444", "Premium Account", 1500, AccountType.Premium, 1000, 1500, false)]
        [TestCase("44444", "Premium Account", 100, AccountType.Premium, -600, -500, true)]
        [TestCase("44444", "Premium Account", 150, AccountType.Basic, -50, 100, false)]
        [TestCase("44444", "Premium Account", 100, AccountType.Premium, -150, -50, true)]
        public void PremiumAccountWithdrawalRuleTest(string accountNumber, string name, decimal balance, AccountType accountType, decimal amount, decimal newBalance, bool expectedResult)
        {
            IWithdraw withdrawal = new PremiumAccountWithdrawRule();
            Account account = new Account() { AccountNumber = accountNumber, Balance = balance, Name = name, Type = accountType };
            AccountWithdrawResponse response = withdrawal.Withdraw(account, amount);

            if (response.Success)
            {
                Assert.AreEqual(newBalance, response.Account.Balance);
            }
            Assert.AreEqual(response.Success, expectedResult);
        }
    }
}