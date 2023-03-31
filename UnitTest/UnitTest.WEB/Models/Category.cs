using System.Collections.Generic;

namespace UnitTest.WEB.Models
{
    //Db Unit Test işlemlerinde genellikle ilişkili datalarla çalışırız bundan kaynaklı yeni bir tablo ekliyoruz
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
