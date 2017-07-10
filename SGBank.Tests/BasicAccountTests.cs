using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGBank.BLL;
using SGBank.BLL.WithdrawRules;
using SGBank.BLL.DepositRules;
using SGBank.Models.Responses;
using SGBank.Models;
using SGBank.Models.Interfaces;

namespace SGBank.Tests
{
    [TestFixture]
    class BasicAccountTests
    {
        [TestCase("33333", true)]
        public void CanLoadBasicAccountTestData(string name, bool expected)
        {
            AccountManager manager = AccountManagerFactory.Create();
            AccountLookupResponse response = manager.LookupAccount(name);

            Assert.AreEqual(name, response.Account.AccountNumber);
        }

        [TestCase("33333", "Basic Account", 100, AccountType.Free, 250, false)]
        [TestCase("33333", "Basic Account", 100, AccountType.Basic, -100, false)]
        [TestCase("33333", "Basic Account", 100, AccountType.Basic, 250, true)]
        public void BasicAccountDepositRuleTest(string accountNumber, string name, decimal balance, AccountType accountType, decimal amount, bool expectedResult)
        {
            IDeposit deposit = new NoLimitDepositRule();
            Account account = new Account() { AccountNumber = accountNumber, Balance = balance, Name = name, Type = accountType };
            AccountDepositResponse response = deposit.Deposit(account, amount);

            Assert.AreEqual(expectedResult, response.Success);
        }

        [TestCase("33333", "Basic Account", 1500, AccountType.Basic, -1000, 1500, false)]
        [TestCase("33333", "Basic Account", 100, AccountType.Free, -100, 100, false)]
        [TestCase("33333", "Basic Account", 100, AccountType.Basic, 100, 100, false)]
        [TestCase("33333", "Basic Account", 150, AccountType.Basic, -50, 100, true)]
        [TestCase("33333", "Basic Account", 100, AccountType.Basic, -150, -60, true)]
        public void BasicAccountWithdrawalRuleTest(string accountNumber, string name, decimal balance, AccountType accountType, decimal amount, decimal newBalance, bool expectedResult)
        {
            IWithdraw withdrawal = new BasicAccountWithdrawRule();
            Account account = new Account() { AccountNumber = accountNumber, Balance = balance, Name = name, Type = accountType };
            AccountWithdrawResponse response = withdrawal.Withdraw(account, amount);

            if (response.Success)
            {
                Assert.AreEqual(newBalance, response.Account.Balance);
            }
            Assert.AreEqual(expectedResult, response.Success);
        }
    }
}