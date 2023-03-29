using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTest.WEB.Controllers;
using UnitTest.WEB.Models;
using UnitTest.WEB.Repository;
using Xunit;

namespace UnitTest.Test
{
    public class ProductControllerTest
    {
        //Product tipinde IRepository'nin taklidini oluşturuyorum sonuçta ana amacım controller'ı test etmek araya db connectionlar problemleri ve gecikmeleri girmesin
        private readonly Mock<IRepository<Product>> _productMock;

        //Asıl test edeceğim yapıyı da ekliyorum
        private readonly ProductsController _controller;

        //Get işlemlerini simüle edebilmek için product listesi oluşturuyorum
        private List<Product> _seedProducts;

        public ProductControllerTest()
        {
            //Taklit Initilaizer
            _productMock = new Mock<IRepository<Product>>();
            //Product Controller DI'da IRepository<Product> istiyor,istediğini veriyorum ve gerçek instance oluşturuyorum
            _controller = new ProductsController(_productMock.Object);

            //Product List Initilaizer
            _seedProducts = new List<Product>()
            {
                new Product{Id=1,Name="Kalem",Price=60,Stock=15},
                new Product{Id=2,Name="Silgi",Price=5,Stock=100},
                new Product{Id=3,Name="Defter",Price=25,Stock=80},
            };
        }

        #region Index
        [Fact]
        //Index metodunun doğru çalıştığı ve view döndüğü senaryoyu test ediyorum
        public async Task Index_ActionExecute_ReturnView()
        {
            var result = await _controller.Index();
            //View result dönüp dönmediğini test ediyorum
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        //Index metodunun doğru çalıştığı ve product döndüğü senaryoyu test ediyorum
        public async Task Index_ActionExecute_ReturnProductList()
        {
            //IRepository.GetAllAsync çalıştığı zaman database'e bağlanmayacak ve _seedProducts'ı dönecek
            _productMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_seedProducts);

            var result = await _controller.Index();

            //View result dönüp dönmediğini test ediyorum ve Casting işlemi de yapmış oluyorum
            var viewResult = Assert.IsType<ViewResult>(result);

            //ViewResult'taki modelimizin tipini de kontrol ediyoruz
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            //Product listesinde 3 adet eleman bekliyorum, bunu da test ediyorum,tüm assertler şartı sağlarsa sonuç benim için doğrudur
            Assert.Equal<int>(3, productList.Count());
        }
        #endregion

        #region Details
        [Fact]
        //Detail metodunun id null olduğunda ki davranışını test edeceğim
        public async Task Details_IdIsNull_ReturnRedirectToActionIndex()
        {
            //Id null olduğunda
            var result = await _controller.Details(null);

            //Bana RedirectToAction dönmeli
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            //Redirect edilecek action ismi Index mi?
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        //Detail metodunun id olmadığında ki davranışını test edeceğim
        public async Task Details_IdInvalid_ReturnNotFound()
        {
            Product product = null;

            //IRepository.GetById'nin verilen id'ye ait değeri yokmuş gibi davranması sağlayacağız
            _productMock.Setup(repo => repo.GetByIdAsync(0)).ReturnsAsync(product);

            //Id Invalid olduğunda (yukarıda id 0 olduğunda null dönecektir ondan dolayı burada da 0 olarak verdik id değerini)
            var result = await _controller.Details(0);

            //Bana NotFoundResult dönmeli
            var redirect = Assert.IsType<NotFoundResult>(result);

            //Status Code Not Found için 404'tür bize o değer mi gelecek
            Assert.Equal(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        //Detail metodunun id olmadığında ki davranışını test edeceğim
        public async Task Details_ValidId_ReturProduct(int productId)
        {
            var product = _seedProducts.FirstOrDefault(i => i.Id == productId);

            //IRepository.GetById'nin verilen id'ye ait product'ı aldığımız durumu simüle ediyorum
            _productMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            //Valid id gönderdiğimizde...
            var result = await _controller.Details(productId);

            //Bana ViewResult dönmeli
            var viewResult = Assert.IsType<ViewResult>(result);

            //View ile birlikte Product tipinde dönüş aldık mı
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            //Gelen product property leri beklediklerim le eşit mi?
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }
        #endregion

        #region Create
        [Fact]
        //Create HTTP GET Test
        public void Create_ActionExecute_ReturnView()
        {
            //Create (HTTP_GET) sayfasını çağırıyorum
            var result = _controller.Create();

            //ViewResult'ı elde edebiliyor muyum
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        //Validasyon hatasının gerçekleşeceği senaryo
        public async Task CreatePost_InValidModelState_ReturnView()
        {
            //Model State'e hatayı ekledim,her türlü model state invalid durumda olacaktır
            _controller.ModelState.AddModelError("Name", "Name alanı gereklidir");

            var result = await _controller.Create(_seedProducts.First());

            //View Result döneceğini bekliyorum
            var viewResult = Assert.IsType<ViewResult>(result);

            //Validasyon kuralına uyulmadığı takdirde kişiye eklemeyi çalıştığı modeli dönüyoruz, bu modelin tipi beklediğimizle uyumlu mu?
            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        //Ekleme işlemi başarılıysa Index e yönlendirecek mi?
        public async Task CreatePost_ValidModelState_ReturnRedirectToIndexViews()
        {
            var result = await _controller.Create(_seedProducts.First());

            //Redirect işlemi yapılacağından dolayı RedirectToAction dönüşü almamız gerekiyor
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            //Yönleneceği sayfa index mi?
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        //Validasyon hatası yoksa Create metodu düzgün çalışıyor mu
        public async Task CreatePost_ValidModelState_CreateMethodExecute()
        {
            Product newProduct = null;

            //Metodun dönüş değerini değil artık davranışını kontrol edeceğimiz için, gönderilen product'ın kendi product nesnemize atanmasını bekliyor olacağız
            //Burada ki işlem amaçsız gelebilir fakat Validasyon problemi yok ise Create Action'ı içerisinde ki _repo.CreatAsync'nin çalışıp çalışmadığına test etmek için yaptığımız bir çalışmadır bu
            _productMock.Setup(repo => repo.CreateAsync(It.IsAny<Product>())).Callback<Product>(x => newProduct = x);

            //Validasyon hatası olmadan ekleme işlemini yapıyoruz
            var result = await _controller.Create(_seedProducts.First());

            //Create metodumuz (Action olan create değil repo da ki) bir kez çalıştı mı?
            _productMock.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Once);

            //Create düzgün çalıştıysa newProduct=_seedProducts.First()
            Assert.Equal(_seedProducts.First().Id, newProduct.Id);
        }

        [Fact]
        //Validasyon hatası varsa Create metodunun çalışmamasını bekliyorum
        public async Task CreatePost_InValidModelState_NeverCreateMethodExecute()
        {
            //ModelState'e bir hata ekledim (InValid senaryo)
            _controller.ModelState.AddModelError("Name", "");

            //Create işlemini simüle diyorum
            var result = await _controller.Create(_seedProducts.First());

            //Validasyon geçerli olmadığı için repo.Create asla çalışmamalı
            _productMock.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), times: Times.Never);
        }
        #endregion

        #region Edit
        [Fact]
        //Id null ise Redirect To Action çalışıyor mu?
        public async Task Edit_IdIsNull_RedirectToAction()
        {
            //Id nin null olacağı senaryo
            var result = await _controller.Edit(null);

            //RedirectToAction mı çalıştı
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            //Index e mi yönlendirildi
            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(5)]
        //Id invalid ise NotFound() çalışıyor mu?
        public async Task Edit_InValidId_ReturnNotFound(int productId)
        {
            Product product = null;

            _productMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            //Id si 5 olan bir kayıt yok
            var result = await _controller.Edit(productId);

            //NotFound mu çalıştı
            var redirect = Assert.IsType<NotFoundResult>(result);

            //Status Code 404 mü
            Assert.Equal(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        //Her şey başarılı ise
        public async Task Edit_ActionExecute_ReturnProduct(int productId)
        {
            Product product = _seedProducts.First(i => i.Id == productId);

            _productMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            //Doğru bir şekilde güncelleme yapılacak
            var result = await _controller.Edit(productId);

            //RedirectToAction mı çalıştı
            var viewResult = Assert.IsType<ViewResult>(result);

            //Dönen data Product tipinde mi
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            //Gönderdiğimiz ve gelen product aynı mı
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Theory]
        [InlineData(1)]
        //Gönderilen Id ile Product içerisinde ki Id'nin uyuşmadığı senaryoda ki davranışın doğru olup olmadığını test ediyorum.
        //Normalde bu şekilde bir action oluşturmayız ama scaffolding sağolsın ;))
        public async Task EditPost_IdIsNotEqualProductId_ReturnNotFound(int productId)
        {
            //Id leri eşleşmeyecek şekilde gönderiyorum
            var result = await _controller.Edit(productId, _seedProducts.First(i => i.Id == 2));

            //NotFound mu döndü
            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        //Validasyon hatası olduğu zaman gerekli sonucu alıyor muyuz
        public async Task EditPost_InvalidModelState_ReturnView(int productId)
        {
            //Validation error add
            _controller.ModelState.AddModelError("Name", "");

            var result = await _controller.Edit(productId, _seedProducts.First(i => i.Id == productId));

            //View Result dönmesini bekliyorum
            var viewResult = Assert.IsType<ViewResult>(result);

            //Eklemeye çalıştığım data bana geldi mi?
            Assert.IsType<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        //Güncelleme eyleminde validasyon hatası yoksa RedirectToAction çalışıyor mu?
        public async Task EditPost_ValidModelState_ReturnRedirectToAction(int productId)
        {
            var result = await _controller.Edit(productId, _seedProducts.First(i => i.Id == productId));

            //RedirectToAction geldi mi?
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            //Index'e mi yönleniyor
            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(1)]
        //Güncelleme eyleminde validasyon hatası yoksa ekleme işlemi yapılıyor mu?
        public async Task EditPost_ValidModelState_UpdateMethodExecute(int productId)
        {
            var product = _seedProducts.First(i => i.Id == productId);

            //Tabiki de db de bir işlem yapılmayacak fakat metot çalışıyormuş gibi olacak
            _productMock.Setup(repo => repo.Update(product));

            //Edit isteği gönderiliyor
            var result = await _controller.Edit(productId, product);

            //Validasyon hatası yoksa Update işlemi bir kez çalışmalı,bunu kontrol ediyoruz
            _productMock.Verify(repo => repo.Update(It.IsAny<Product>()), times: Times.Once);
        }
        #endregion

        #region Delete/Delete Confirmed
        [Fact]
        //Id null ise NotFound çalışmalı
        public async Task Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);

            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        //id mevcut değilse notfound bekliyorum
        public async Task Delete_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            Product product = null;

            //Olmayan bir id gönderdik,null olan product dönecek
            _productMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            //Action'u çalıştırıyoruz
            var result = await _controller.Delete(productId);

            //NotFoundResult gelmeli
            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        //Mevcut bir ürünü DeleteConfirmed'e gönderebiliyor muyuz
        public async Task Delete_ActionExecute_ReturnView(int productId)
        {
            var product = _seedProducts.First(i => i.Id == productId);

            //Bu arkadaşı çalışacak şekilde simüle ediyorum
            _productMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            //Action'u çalıştırıyoruz
            var result = await _controller.Delete(productId);

            //ViewResult gelmeli
            var viewResult = Assert.IsType<ViewResult>(result);

            //Product dönmeli
            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        //Delete Confirmed düzgün çalışıyorsa Redirect işlemi gerçekleşecek mi
        public async Task DeleteConfirmed_ActionExecutes_ReturnRedirectToAction(int productId)
        {
            var result = await _controller.DeleteConfirmed(productId);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Theory]
        [InlineData(1)]
        //Delete Confirmed düzgün çalışıyorsa Repository.Delete kaç kez icra edilmiş olur?
        public async Task DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int productId)
        {
            var product = _seedProducts.First(i => i.Id == productId);

            _productMock.Setup(repo => repo.Delete(product));

            var result = await _controller.DeleteConfirmed(productId);

            _productMock.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
        }
        #endregion
    }
}
