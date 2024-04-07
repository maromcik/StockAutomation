// See https://aka.ms/new-console-template for more information

using StockAutomationCore.Parser;

Console.WriteLine("Hello, World!");

HoldingSnapshotLineParser.ParseLines("../dataset/snapshot_04_05_2024.csv");
