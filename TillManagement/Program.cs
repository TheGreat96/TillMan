using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Read input from file
        string[] lines = File.ReadAllLines("input.txt");

        // Initialize till
        Dictionary<int, int> till = new Dictionary<int, int>
        {
            { 50, 5 },
            { 20, 5 },
            { 10, 6 },
            { 5, 12 },
            { 2, 10 },
            { 1, 10 }
        };

        // Opening output file for writing
        StreamWriter writer = new StreamWriter("output.txt");

        // Initialize starting till balance
        int tillBalance = CalculateTillBalance(till);

        // Write starting balance to output
        WriteToOutput(writer, tillBalance, "Till Start");

        // Process each transaction
        foreach (string line in lines)
        {
            string[] transactionData = line.Split(';');
            string[] items = transactionData[0].Split(',');

            // Calculate total cost of transaction
            int transactionTotal = 0;
            foreach (string item in items)
            {
                string[] itemData = item.Split(' ');
                int amount = int.Parse(itemData[1].Substring(1)); // Remove 'R' from amount
                transactionTotal += amount;
            }

            // Parse paid amount
            int paidAmount = int.Parse(transactionData[1].Split('-')[0].Substring(1)); // Remove 'R' from amount

            // Calculate change
            int change = paidAmount - transactionTotal;

            // Update till balance
            tillBalance += paidAmount;

            // Write transaction details to output
            WriteToOutput(writer, tillBalance, transactionTotal.ToString(), paidAmount.ToString(), change.ToString(), CalculateChangeBreakdown(change, till));

            // Update till with coins & notes given as change
            UpdateTill(change, till);
        }

        // Close output file
        writer.Close();
    }

    // Calculate current till balance
    static int CalculateTillBalance(Dictionary<int, int> till)
    {
        int balance = 0;
        foreach (KeyValuePair<int, int> kvp in till)
        {
            balance += kvp.Key * kvp.Value;
        }
        return balance;
    }

    // Write transaction details to output file
    static void WriteToOutput(StreamWriter writer, params string[] values)
    {
        writer.WriteLine(string.Join(", ", values));
    }

    // Calculate breakdown of change into currency denominations
    static string CalculateChangeBreakdown(int change, Dictionary<int, int> till)
    {
        List<string> breakdown = new List<string>();
        foreach (KeyValuePair<int, int> kvp in till)
        {
            int count = Math.Min(change / kvp.Key, kvp.Value);
            if (count > 0)
            {
                breakdown.Add($"{count}xR{kvp.Key}");
                change -= kvp.Key * count;
            }
        }
        return string.Join("-", breakdown);
    }

    // Update till with coins/notes given as change
    static void UpdateTill(int change, Dictionary<int, int> till)
    {
        foreach (KeyValuePair<int, int> kvp in till)
        {
            int count = Math.Min(change / kvp.Key, kvp.Value);
            if (count > 0)
            {
                till[kvp.Key] -= count;
                change -= kvp.Key * count;
            }
        }
    }
}
