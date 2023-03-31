﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UnitTest.WEB.Controllers;
using UnitTest.WEB.Models;
using Xunit;

namespace UnitTest.Test
{
    //SQlite ile test ederken kullanılacaktır
    public class ProductControllerTestWithSQLite : ProductControllerTestBase
    {
        public ProductControllerTestWithSQLite()
        {
            //Sqlite connection'ı oluşturduk
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            //Bu provider SQLite'a bağlansın
            SetContextOptions(new DbContextOptionsBuilder<UnitTestDbContext>().UseSqlite(connection).Options);

            //Database'e test dataları ekliyoruz ve aslında refresh atıyoruz (tekrar oluşması gerekebilir,contex te yeni bir tablo eklenmiştir vs...)
            Seed();
        }


        [Fact]
        public async Task Create_ModelValidProduct_ReturnsRedirectToActionWithSaveProduct()
        {
            var newProduct = new Product { Name = "Defter", Price = 40, Stock = 50};

            //Sqlite ile çalışacak Context'i oluşturuyorum
            using (var context = new UnitTestDbContext(_contextOptions))
            {
                var category = await context.Categories.FirstAsync();
                newProduct.CategoryId = category.Id;

                var controller = new ProductsController(context);

                var result = await controller.Create(newProduct);

                var redirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirect.ActionName);

            }

            //Yeni bir context ile test'in devamını gerçekleştireceğim çünkü aynı context üzerinden kontrol ettiğim zaman kaydın InMemory'e aktığından (sonuçta sqlite provider'ı ile çalışsakta InMemory de çalışacaktır) emin olamam çünkü EFCore zaten context nesnesi üzerinde data ları track ediyor ve yanıltıcı bir durum ortaya çıkabilir
            using (var context = new UnitTestDbContext(_contextOptions))
            {
                //kayıt gerçekten InMemory db ye eklenmiş mi?
                var product =await context.Products.FirstOrDefaultAsync(x => x.Name == "Defter");
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
            }

            using (var context = new UnitTestDbContext(_contextOptions))
            {
                var products = await context.Products.Where(i => i.CategoryId == cateogryId).ToListAsync();

                Assert.Empty(products);
            }
        }
    }
}