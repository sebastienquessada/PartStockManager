using System;
using System.Collections.Generic;
using System.Text;

namespace PartStockManager.CoreLogic.Models
{
    public class Part
    {
        public Part(string name, string reference, int stockQuantity, int lowStockThreshold)
        {
            UpdateInfo(name, reference, lowStockThreshold);
            AdjustStock(stockQuantity);
        }

        public override bool Equals(object? obj)
        {
            return obj is Part part &&
                   Name == part.Name &&
                   Reference == part.Reference &&
                   StockQuantity == part.StockQuantity &&
                   LowStockThreshold == part.LowStockThreshold;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Reference, StockQuantity, LowStockThreshold);
        }

        public string Name { get; private set; }
        public string Reference { get; private set; }
        public int StockQuantity { get; private set; }
        public int LowStockThreshold { get; private set; }

        public void UpdateInfo(string name, string reference, int lowStockThreshold)
        {
            if (string.IsNullOrEmpty(name.Trim()))
                throw new ArgumentException(nameof(name), "Name can't be empty !");

            if (name.Length > 200)
                throw new ArgumentOutOfRangeException(nameof(name), "Name cannot exceed 200 characters.");

            if (string.IsNullOrEmpty(reference.Trim()))
                throw new ArgumentException(nameof(reference), "Reference can't be empty !");

            if (reference.Length > 100)
                throw new ArgumentOutOfRangeException(nameof(reference), "Reference cannot exceed 100 characters.");

            if (lowStockThreshold < 0)
                throw new ArgumentOutOfRangeException(nameof(lowStockThreshold), "Low stock threshold can't be lower than zero !");

            Name = name.Trim();
            Reference = reference.Trim();
            LowStockThreshold = lowStockThreshold;
        }

        public void AdjustStock(int newQuantity)
        {
            if (newQuantity < 0)
                throw new ArgumentOutOfRangeException(nameof(newQuantity), "Quantity can't be lower than zero !");

            StockQuantity = newQuantity;
        }

        public void StockExit(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity can't be lower than zero !");

            StockQuantity -= quantity;
        }

        public void StockEntry(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity can't be lower than zero !");

            StockQuantity += quantity;
        }
    }
}
