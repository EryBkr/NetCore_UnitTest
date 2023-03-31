using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UnitTest.WEB.Controllers;
using UnitTest.WEB.Models;
using Xunit;

namespace UnitTest.Test
{
    //LocalDB gerçek Veritabanını tam manasıyla simüle etmemiz için en iyi seçenecek olabilir
    public class ProductControllerTestWithLocalDB : ProductControllerTestBase
    {
        public ProductControllerTestWithLocalDB()
        {
            var sqlCon = @"Server=(localdb)\MSSQLLocalDB;Database=UnitTestLocalDBTest;Trusted_Connection=true;MultipleActiveResultSets=true;";

            //Bu provider LocalDB'te bağlansın
            SetContextOptions(new DbContextOptionsBuilder<UnitTestDbContext>().UseSqlServer(sqlCon).Options);

            //Database'e test dataları ekliyoruz ve aslında refresh atıyoruz (tekrar oluşması gerekebilir,contex te yeni bir tablo eklenmiştir vs...)
            Seed();
        }


        [Fact]
        public async Task Create_ModelValidProduct_ReturnsRedirectToActionWithSaveProduct()
        {
            var newProduct = new Product { Name = "Defter", Price = 40, Stock = 50};

            using (var context = new UnitTestDbContext(_contextOptions))
            {
                var category = await context.Categories.FirstAsync();

                Assert.NotNull(category);

                newProduct.CategoryId = category.Id;

                var controller = new ProductsController(context);

                var result = await controller.Create(newProduct);

                var redirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirect.ActionName);

                //kayıt gerçekten db ye eklenmiş mi?
                var product = await context.Products.FirstOrDefaultAsync(x => x.Name == "Defter");
                Assert.Equal(newProduct.Name, product.Name);

            }
        }


        [Theory]
        [InlineData(1)]
        public async Task DeleteCategory_ExistCategoryId_DeleteAllProducts(int cateogryId)
        {
            using (var context = new UnitTestDbContext(_contextOptions))
            {
                var category = await context.Categories.FindAsync(cateogryId);
                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                var products = await context.Products.Where(i => i.CategoryId == cateogryId).ToListAsync();

                //Cascade.Delete'ten kaynaklı Ürünlerin hepsi Kategorisi silindiği için silinmiş olmalı
                Assert.Empty(products);
            }
        }
    }
}
