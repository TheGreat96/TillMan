using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TillManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read input from file
            string[] lines = File.ReadAllLines("input.txt");
            List<Transaction> transactions = new List<Transaction>();

            // Parse input
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                string[] items = parts[0].Split(';');
                string[] amounts = parts[1].Split('-').Skip(1).ToArray();

                List<Item> itemList = new List<Item>();
                foreach (string item in items)
                {
                    string[] itemParts = item.Split(' ');
                    string description = string.Join(" ", itemParts.Take(itemParts.Length - 1));
                    int amount = int.Parse(itemParts.Last());
                    itemList.Add(new Item(description, amount));
                }

                int[] paidAmounts = Array.ConvertAll(amounts, int.Parse);

                transactions.Add(new Transaction(itemList.ToArray(), paidAmounts));
            }

            // Process transactions
            List<string> outputLines = new List<string>();
            Till till = new Till(5, 5, 6, 12, 10, 10);

            foreach (Transaction transaction in transactions)
            {
                // Record initial state of the till
                string tillStateBeforeTransaction = till.ToString();

                // Calculate total cost of the transaction
                int totalCost = transaction.TotalCost();

                // Calculate amount paid
                int amountPaid = transaction.AmountPaid();

                // Calculate change
                int change = amountPaid - totalCost;

                // Calculate change breakdown
                Dictionary<int, int> changeBreakdown = till.CalculateChange(change);

                // Update till
                till.Update(transaction);

                // Construct output line
                string outputLine = $"{tillStateBeforeTransaction} | Total: {totalCost} | Paid: {amountPaid} | Change: {change} | Breakdown: {string.Join(", ", changeBreakdown.Select(kv => $"{kv.Value} x R{kv.Key}"))}";
                outputLines.Add(outputLine);
            }

            // Output results to file
            outputLines.Add(till.ToString());
            File.WriteAllLines("output.txt", outputLines);
            Console.WriteLine("Output written to output.txt");
        }
    }

    class Item
    {
        public string Description { get; }
        public int Amount { get; }

        public Item(string description, int amount)
        {
            Description = description;
            Amount = amount;
        }
    }

    class Transaction
    {
        private Item[] items;
        private int[] paidAmounts;

        public Transaction(Item[] items, int[] paidAmounts)
        {
            this.items = items;
            this.paidAmounts = paidAmounts;
        }

        public int TotalCost()
        {
            return items.Sum(item => item.Amount);
        }

        public int AmountPaid()
        {
            return paidAmounts.Sum();
        }

        public Item[] GetItems()
        {
            return items;
        }
    }

    class Till
    {
        private Dictionary<int, int> denominations = new Dictionary<int, int>()
        {
            { 50, 5 },
            { 20, 5 },
            { 10, 6 },
            { 5, 12 },
            { 2, 10 },
            { 1, 10 }
        };

        public Till(int fifties, int twenties, int tens, int fives, int twos, int ones)
        {
            denominations[50] = fifties;
            denominations[20] = twenties;
            denominations[10] = tens;
            denominations[5] = fives;
            denominations[2] = twos;
            denominations[1] = ones;
        }

        public void Update(Transaction transaction)
        {
            foreach (var item in transaction.GetItems())
            {
                if (item.Amount > 50)
                {
                    denominations[50]--;
                }
                else if (item.Amount > 20)
                {
                    denominations[20]--;
                }
                else if (item.Amount > 10)
                {
                    denominations[10]--;
                }
                else if (item.Amount > 5)
                {
                    denominations[5]--;
                }
                else if (item.Amount > 2)
                {
                    denominations[2]--;
                }
                else
                {
                    denominations[1]--;
                }
            }
        }

        public Dictionary<int, int> CalculateChange(int change)
        {
            var changeBreakdown = new Dictionary<int, int>();

            foreach (var kvp in denominations.OrderByDescending(x => x.Key))
            {
                int denomination = kvp.Key;
                int count = change / denomination;

                if (count > kvp.Value)
                {
                    count = kvp.Value;
                }

                if (count > 0)
                {
                    changeBreakdown.Add(denomination, count);
                    change -= count * denomination;
                }
            }

            return changeBreakdown;
        }

        public override string ToString()
        {
            return $"Till status: {string.Join(", ", denominations.Select(kv => $"{kv.Value} x R{kv.Key}"))}";
        }
    }
}