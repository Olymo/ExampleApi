using ExampleApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi
{
    public class InMemmoryDatabase
    {
        public static List<Product> Products = new List<Product>();
        public static List<Category> Categories = new List<Category>();
    }
}
