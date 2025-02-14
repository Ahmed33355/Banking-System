using System;
using System.Collections.Generic;

namespace BankingSystem
{
    // Encapsulation: Account details are hidden and accessed via public methods
    public abstract class Account
    {
        public int AccountNumber { get; protected set; }
        public string AccountHolder { get; protected set; }
        protected decimal Balance;

        public Account(int accountNumber, string accountHolder, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            AccountHolder = accountHolder;
            Balance = initialBalance;
        }

        public abstract void Deposit(decimal amount);
        public abstract bool Withdraw(decimal amount);
        public decimal GetBalance() => Balance;
    }

    // Inheritance: SavingsAccount inherits from Account and applies interest on deposits.
    public class SavingsAccount : Account
    {
        private const decimal InterestRate = 0.03m;

        public SavingsAccount(int accountNumber, string accountHolder, decimal initialBalance)
            : base(accountNumber, accountHolder, initialBalance) { }

        public override void Deposit(decimal amount)
        {
            decimal interest = amount * InterestRate;
            Balance += amount + interest;
            Console.WriteLine($"Deposited {amount} with interest {interest}. New Balance: {Balance}");
        }

        public override bool Withdraw(decimal amount)
        {
            if (Balance - amount < 0)
            {
                Console.WriteLine("Insufficient funds!");
                return false;
            }
            Balance -= amount;
            Console.WriteLine($"Withdrawn {amount}. New Balance: {Balance}");
            return true;
        }
    }

    // Inheritance: CheckingAccount inherits from Account and supports overdraft.
    public class CheckingAccount : Account
    {
        private const decimal OverdraftLimit = 500;

        public CheckingAccount(int accountNumber, string accountHolder, decimal initialBalance)
            : base(accountNumber, accountHolder, initialBalance) { }

        public override void Deposit(decimal amount)
        {
            Balance += amount;
            Console.WriteLine($"Deposited {amount}. New Balance: {Balance}");
        }

        public override bool Withdraw(decimal amount)
        {
            if (Balance - amount < -OverdraftLimit)
            {
                Console.WriteLine("Overdraft limit reached!");
                return false;
            }
            Balance -= amount;
            Console.WriteLine($"Withdrawn {amount}. New Balance: {Balance}");
            return true;
        }
    }

    // Polymorphism: Transaction class represents both deposits and withdrawals.
    public class Transaction
    {
        public int TransactionId { get; }
        public int AccountNumber { get; }
        public decimal Amount { get; }
        public string Type { get; }
        public DateTime Date { get; }

        public Transaction(int transactionId, int accountNumber, decimal amount, string type)
        {
            TransactionId = transactionId;
            AccountNumber = accountNumber;
            Amount = amount;
            Type = type;
            Date = DateTime.Now;
        }

        public override string ToString() => $"Transaction {TransactionId}: {Type} of {Amount} on {Date}";
    }

    public class Customer
    {
        public string Name { get; }
        public int CustomerId { get; }
        public List<Account> Accounts { get; }

        public Customer(int customerId, string name)
        {
            CustomerId = customerId;
            Name = name;
            Accounts = new List<Account>();
        }

        public void AddAccount(Account account) => Accounts.Add(account);
    }

    public class Bank
    {
        private List<Account> accounts = new List<Account>();
        private List<Transaction> transactions = new List<Transaction>();
        private int transactionCounter = 0;

        public void AddAccount(Account account)
        {
            accounts.Add(account);
            Console.WriteLine($"Account {account.AccountNumber} added for {account.AccountHolder}.");
        }

        public Account GetAccount(int accountNumber) => accounts.Find(a => a.AccountNumber == accountNumber);

        public void MakeTransaction(int accountNumber, decimal amount, string type)
        {
            Account account = GetAccount(accountNumber);
            if (account == null)
            {
                Console.WriteLine("Account not found!");
                return;
            }
            bool success = false;
            if (type == "Deposit")
            {
                account.Deposit(amount);
                success = true;
            }
            else if (type == "Withdrawal")
            {
                success = account.Withdraw(amount);
            }
            if (success)
            {
                transactions.Add(new Transaction(++transactionCounter, accountNumber, amount, type));
                Console.WriteLine("Transaction successful.");
            }
        }

        public void PrintTransactions()
        {
            if (transactions.Count == 0)
            {
                Console.WriteLine("No transactions to show.");
                return;
            }
            foreach (var transaction in transactions)
                Console.WriteLine(transaction);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Bank bank = new Bank();
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n----- Banking System Menu -----");
                Console.WriteLine("1. Create Account");
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Withdraw");
                Console.WriteLine("4. View Account Balance");
                Console.WriteLine("5. View All Transactions");
                Console.WriteLine("6. Exit");
                Console.Write("Enter your choice: ");
                string input = Console.ReadLine();
                int choice;
                if (!int.TryParse(input, out choice))
                {
                    Console.WriteLine("Invalid choice. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        CreateAccount(bank);
                        break;
                    case 2:
                        PerformTransaction(bank, "Deposit");
                        break;
                    case 3:
                        PerformTransaction(bank, "Withdrawal");
                        break;
                    case 4:
                        ViewBalance(bank);
                        break;
                    case 5:
                        bank.PrintTransactions();
                        break;
                    case 6:
                        exit = true;
                        Console.WriteLine("Exiting the banking system. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        static void CreateAccount(Bank bank)
        {
            Console.WriteLine("\nSelect Account Type:");
            Console.WriteLine("1. Savings Account");
            Console.WriteLine("2. Checking Account");
            Console.Write("Enter your choice: ");
            string inputType = Console.ReadLine();
            int accType;
            if (!int.TryParse(inputType, out accType) || (accType != 1 && accType != 2))
            {
                Console.WriteLine("Invalid account type.");
                return;
            }

            Console.Write("Enter Account Number: ");
            string accNumInput = Console.ReadLine();
            int accountNumber;
            if (!int.TryParse(accNumInput, out accountNumber))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            Console.Write("Enter Account Holder Name: ");
            string accountHolder = Console.ReadLine();

            Console.Write("Enter initial deposit amount: ");
            string amountInput = Console.ReadLine();
            decimal initialDeposit;
            if (!decimal.TryParse(amountInput, out initialDeposit))
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            Account account;
            if (accType == 1)
                account = new SavingsAccount(accountNumber, accountHolder, initialDeposit);
            else
                account = new CheckingAccount(accountNumber, accountHolder, initialDeposit);

            bank.AddAccount(account);
        }

        static void PerformTransaction(Bank bank, string type)
        {
            Console.Write($"\nEnter Account Number for {type}: ");
            string accNumInput = Console.ReadLine();
            int accountNumber;
            if (!int.TryParse(accNumInput, out accountNumber))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            Console.Write($"Enter amount to {type}: ");
            string amountInput = Console.ReadLine();
            decimal amount;
            if (!decimal.TryParse(amountInput, out amount))
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            bank.MakeTransaction(accountNumber, amount, type);
        }

        static void ViewBalance(Bank bank)
        {
            Console.Write("\nEnter Account Number to view balance: ");
            string accNumInput = Console.ReadLine();
            int accountNumber;
            if (!int.TryParse(accNumInput, out accountNumber))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            Account account = bank.GetAccount(accountNumber);
            if (account == null)
            {
                Console.WriteLine("Account not found.");
                return;
            }
            Console.WriteLine($"Account {account.AccountNumber} belonging to {account.AccountHolder} has a balance of {account.GetBalance()}");
        }
    }
}
