using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTest.WEB.Controllers;
using UnitTest.WEB.Helpers;
using UnitTest.WEB.Models;
using UnitTest.WEB.Repository;
using Xunit;

namespace UnitTest.Test
{
    public class ProductApiControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly Helper _helper;
        private readonly ProductsApiController _controller;
        private List<Product> _products;

        public ProductApiControllerTest()
        {
            _helper = new Helper();
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsApiController(_mockRepo.Object);
            _products = new List<Product>()
            {
                new Product{Id=1,Name="Kalem",Price=60,Stock=15},
                new Product{Id=2,Name="Silgi",Price=5,Stock=100},
                new Product{Id=3,Name="Defter",Price=25,Stock=80}
            };
        }

        #region Add Bussiness
        [Theory]
        [InlineData(5,6,11)]
        [InlineData(7, 8,15)]
        [InlineData(5000, 6000,11000)]
        public void Add_ExecuteAction_ReturnOkResultWithTotalCount(int num1,int num2,int total)
        {
            var result = _helper.Add(num1,num2);
            Assert.Equal(total,result);
        }
        #endregion

        #region Get/GetAll
        [Fact]
        public async Task GetProducts_ActionExecutes_ReturnOkResultWithProduct()
        {
            _mockRepo.Setup(i => i.GetAllAsync()).ReturnsAsync(_products);

            var result = await _controller.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            Assert.Equal(3, returnProduct.Count());
        }

        [Theory]
        [InlineData(0)]
        public async Task GetProduct_InvalidId_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(i => i.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.GetProduct(productId);

            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetProduct_ValidId_ReturnOkResultWithProduct(int productId)
        {
            _mockRepo.Setup(i => i.GetByIdAsync(productId)).ReturnsAsync(_products.First(i => i.Id == productId));

            var result = await _controller.GetProduct(productId);

            var OkObjectResult = Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsType<Product>(OkObjectResult.Value);

            Assert.Equal(productId, returnProduct.Id);
        }
        #endregion

        #region Put
        [Theory]
        [InlineData(1)]
        public async Task PutProduct_IsIsNotEqualProduct_ReturnBadRequestResult(int productId)
        {
            var product = _products.First(i => i.Id == productId);

            var result = await _controller.PutProduct(2, product);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async Task PutProduct_ExecuteAction_ReturnNoContentResult(int productId)
        {
            var product = _products.First(i => i.Id == productId);
            _mockRepo.Setup(repo => repo.Update(product));
            var result = await _controller.PutProduct(productId, product);

            Assert.IsType<NoContentResult>(result);
            _mockRepo.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
        }
        #endregion

        #region Post
        [Fact]
        public async Task PostProduct_ActionExecute_ReturnCreatedAction()
        {
            var product = _products.First();
            _mockRepo.Setup(repo => repo.CreateAsync(product)).Returns(Task.CompletedTask);

            var result = await _controller.PostProduct(product);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal("GetProduct", createdAtActionResult.ActionName);

            _mockRepo.Verify(repo => repo.CreateAsync(product), Times.Once);
        }
        #endregion

        #region Delete
        [Theory]
        [InlineData(0)]
        public async Task DeleteProduct_IdInvalid_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            //IActionResult değil ActionResult<Product> döndüğü için aşağıda notFoundResult.Result şeklinde kullandık
            var notFoundResult =await _controller.DeleteProduct(productId);

            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }

        [Theory]
        [InlineData(1)]
        public async Task DeleteProduct_ExecuteAction_NoContent(int productId)
        {
            Product product = _products.First(i=>i.Id==productId);
            _mockRepo.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockRepo.Setup(repo => repo.Delete(product));

            //IActionResult değil ActionResult<Product> döndüğü için aşağıda noContentResult.Result şeklinde kullandık
            var noContentResult = await _controller.DeleteProduct(productId);

            Assert.IsType<NoContentResult>(noContentResult.Result);
            _mockRepo.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
        }
        #endregion
    }
}
