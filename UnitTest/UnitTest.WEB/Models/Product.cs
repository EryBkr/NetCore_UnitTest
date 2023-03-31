using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace UnitTest.WEB.Models
{
    public partial class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }



        //Db Unit Test işlemlerinde genellikle ilişkili datalarla çalışırız bundan kaynaklı yeni bir tablo ekliyoruz
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
