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
            string inputFilePath = "input.txt";
            string outputFilePath = "output.txt";

            // Check if input file exists
            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("Input file not found.");
                return;
            }

            // Read input from file
            string[] lines = File.ReadAllLines(inputFilePath);
            List<Transaction> transactions = new List<Transaction>();

            // Parse input
    foreach (string line in lines)
{
    Console.WriteLine($"Processing line: {line}");

    string[] lineParts = line.Split(';'); // Split by semicolon to separate items
    
    // Ensure that the line has at least two parts (items and amounts)
    if (lineParts.Length < 2)
    {
        Console.WriteLine("Invalid input format. Skipping line.");
        continue;
    }
    Console.WriteLine($"Line parts length: {lineParts.Length}");
     Console.WriteLine($"Second part of the line: {lineParts[1]}");
    string[] items = lineParts[0].Split(';');
    string[] amounts = lineParts[1].Split('-').Skip(1).ToArray(); // Split by dash to handle multiple amounts

Console.WriteLine($"Items count: {items.Length}, Amounts count: {amounts.Length}");
    // Rest of the processing logic...


    List<Item> itemList = new List<Item>();
    foreach (string item in items)
    {
        Console.WriteLine($"Processing item: {item}");

        // Extract description and amount
        string[] itemParts = item.Split(' ');
        int amountIndex = Array.FindLastIndex(itemParts, part => part.StartsWith("R"));
        string description = string.Join(" ", itemParts.Take(amountIndex));
        int amount = int.Parse(itemParts[amountIndex].Substring(1)); // Remove the 'R' symbol
        itemList.Add(new Item(description, amount));
    }

    Console.WriteLine($"Amounts paid: {string.Join(", ", amounts)}");

    // Parse amounts paid
    if (amounts.Length >0){
    List<int> paidAmounts = new List<int>();
    foreach (string amountString in amounts)
    {
        int startIndex = amountString.IndexOf('R'); // Find the index of 'R' symbol
        if (startIndex != -1)
        {
            string amountSubstring = amountString.Substring(startIndex + 1); // Extract the substring after 'R'
            if (int.TryParse(amountSubstring, out int amount))
            {
                paidAmounts.Add(amount);
            }
        }
    }
    }
    // Remaining logic...
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
            File.WriteAllLines(outputFilePath, outputLines);
            Console.WriteLine($"Output written to {outputFilePath}");
        }
    }

    // Item, Transaction, and Till classes remain the same as provided in the previous responses
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

