using Microsoft.EntityFrameworkCore;
using UnitTest.WEB.Models;

namespace UnitTest.Test
{
    //Testlerimizi gerçek bir veritabanına bağlanarak değil de ef core inmemory ya sqlite inmemory gibi alternatif kaynaklara bağlanarak gerçekleştirmek hız ve güvenilirlik açısından daha sağlıklı olacaktır. Bu işlemleri gerçekleştirirken bu class'ı base class olarak kullanacağım
    public class ProductControllerTestBase
    {
        //Db Connection'u manipüle edeceğim için handle ettim
        protected DbContextOptions<UnitTestDbContext> _contextOptions { get; private set; }

        public void SetContextOptions(DbContextOptions<UnitTestDbContext> contextOptions)
        {
            _contextOptions = contextOptions;
        }

        public void Seed()
        {
            //Context options set edildiği anda HasData ile oluşturduğumuz datalar yeni veritabanında da oluşacaktır
            using (UnitTestDbContext context = new UnitTestDbContext(_contextOptions))
            {
                //Database silinsin
                context.Database.EnsureDeleted();

                //Database tekrar oluşsun
                context.Database.EnsureCreated();

                //Deneme amaçlı ekleme işlemi yapıyorum
                context.Products.Add(new Product { CategoryId = 1, Name = "Silgi", Price = 5, Stock = 230 });
                context.SaveChanges();
            }
        }
    }
}
