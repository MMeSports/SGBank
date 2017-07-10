using NUnit.Framework;
using SGBank.BLL;
using SGBank.BLL.DepositRules;
using SGBank.BLL.WithdrawRules;
using SGBank.Models;
using SGBank.Models.Interfaces;
using SGBank.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGBank.Tests
{
    [TestFixture]
    public class FreeAccountTests
    {
        [Test]
        public void CanLoadFreeAccountTestData()
        {
            AccountManager manager = AccountManagerFactory.Create();

            AccountLookupResponse response = manager.LookupAccount("11111");

            Assert.IsNotNull(response.Account);
            Assert.IsTrue(response.Success);
            Assert.AreEqual("11111", response.Account.AccountNumber);
        }

        [TestCase("12345", "Free Account", 100, AccountType.Free, 250, false)]
        [TestCase("12345", "Free Account", 100, AccountType.Free, -100, false)]
        [TestCase("12345", "Free Account", 100, AccountType.Basic, 50, false)]
        [TestCase("12345", "Free Account", 100, AccountType.Free, 50, true)]

        public void FreeAccountDepositRuleTest(string accountNumber, string name, decimal balance,
                    AccountType accountType, decimal amount, bool expectedResult)
        {
            IDeposit testDepositRule = new FreeAccountDepositRule();
            Account testAccount = new Account();

            testAccount.AccountNumber = accountNumber;
            testAccount.Name = name;
            testAccount.Balance = balance;
            testAccount.Type = accountType;

            AccountDepositResponse response = testDepositRule.Deposit(testAccount, amount);
            Assert.AreEqual(expectedResult, response.Success);
        }
        [Test]
        [TestCase("12345", "Free Account", 100, AccountType.Free, 250, false)]
        [TestCase("12345", "Free Account", 100, AccountType.Free, -100, false)]
        [TestCase("12345", "Free Account", 100, AccountType.Basic, 50, false)]
        [TestCase("12345", "Free Account", 100, AccountType.Free, 50, true)]
        public void FreeAccountWithdrawRuleTest(string accountNumber, string name, decimal balance,
                    AccountType accountType, decimal amount, bool expectedResult)
        {
            IWithdraw testWithdrawRule = new FreeAccountWithdrawRule();
            Account testAccount = new Account();

            testAccount.AccountNumber = accountNumber;
            testAccount.Name = name;
            testAccount.Balance = balance;
            testAccount.Type = accountType;

            AccountWithdrawResponse response = testWithdrawRule.Withdraw(testAccount, amount);
            Assert.AreEqual(expectedResult, response.Success);
        }
    }
}