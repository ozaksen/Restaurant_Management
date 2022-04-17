using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace QrMenuAgain.Models
{
    public class FoodItem
    {
        public FoodItem( int menuId, string category, string name, double price, string? description, bool isActive)
        {
            //Id = id;
            MenuId = menuId;
            Category = category;
            Name = name;
            Price = price;
            Description = description;
            IsActive = isActive;

        }

        public long Id { get; set; }
        public int MenuId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; } 
        public double Price { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int OrderScore { get; set; }


    }
}