using Moq;
using System;
using Xunit;
using XUnitTest.App;

namespace XUnitTest.TestApp
{
    //Moq kullanılmasındaki temel amaç gerçek external servislere (+ db) istek yapılmasını önlemektir.
    //class lar birçok DI içerebilir,her birinin instance'larını almaktansa moq ile taklit ediyoruz
    //Metotlarda bir den fazla Assert kullanılabilir,hepsinin geçerli olması gerekir
    public class CalculatorTest
    {
        public Calculator _calculator { get; set; }
        public Mock<ICalculatorService> myMock { get; set; }

        //Test class'ları en nihayetinde normal class'lardır, yapıcı metot kullanabiliriz
        public CalculatorTest()
        {
            //ICalculator Servisinin taklidini oluşturdum
            myMock = new Mock<ICalculatorService>();

            //Arrange Object
            _calculator = new Calculator(myMock.Object);
        }


        //Test metotları isimlendirilirken MethodNaame_StateUnderTest_ExpectedBehavior şeklinde bir şema izlemek uygundur

        [Fact] //Test metodu olduğunu belirtiyoruz (Fact testleri parametre almazlar)
        public void add_simpleValues_returnTotalValue()
        {
            //Arrange --> Initilaizer işlemleri
            int a = 5;
            int b = 20;

            //add metodunun çalıştığı zaman ne döneceğine karar verdik ki , çalışmasına gerek kalmasın (instance'a gerek yok)
            myMock.Setup(x => x.add(a, b)).Returns(a + b);


            //Act --> Test edilecek metotlar çalıştırılır
            var total = _calculator.add(a, b);

            //Assert --> Doğrulama evresi
            //Beklenen ve aslında elde ettiğimiz değerleri veriyoruz
            Assert.Equal(25, total);
        }

        [Theory] //Theory tipindeki testler parametre alırlar
        [InlineData(20, 5, 25)] //Parametrelin value'larını bu şekilde initilaize ediyorum
        [InlineData(20, 5, 30)] //Theory testlerinde bir çok farklı değer ile kıyaslama yapabiliriz,birden fazla metot oluşturmamıza gerek olmamaktadır
        public void add_externalValues_returnTotalValue(int a, int b, int expectedTotal)
        {
            //myMock taklidinin hangi metotta nasıl davranması gerektiğini belirledim (add metodu çalışıyormuş gibi bizlere değer dönecek)
            myMock.Setup(x => x.add(a, b)).Returns(expectedTotal);
            myMock.Setup(x => x.multip(a, b)).Returns(expectedTotal);

            //Act --> Test edilecek metotlar çalıştırılır
            //Gerçek datayı elde ediyorum
            var actualData = _calculator.add(a, b);

            //Assert --> Doğrulama evresi
            //Beklenen ve aslında elde ettiğimiz değerleri veriyoruz
            Assert.Equal(expectedTotal, actualData);

            //Add metodunun 1 kez çalışıp çalışmadığını kontrol ediyoruz
            myMock.Verify(x => x.add(a, b), Times.Once);

            //Hem Assert hem de Verify sonuçları true dönmeli ki testimiz başarılı olsun
        }

        [Theory]
        [InlineData(20, 5, 100)]
        [InlineData(20, 6, 120)]
        public void multip_externalValues_returnMultipValue(int a, int b, int expectedMultip)
        {
            //myMock taklidinin hangi metotta nasıl davranması gerektiğini belirledim (multip metodu çalışıyormuş gibi bizlere değer dönecek)
            myMock.Setup(x => x.multip(a, b)).Returns(expectedMultip);

            var actualData = _calculator.multip(a, b);

            Assert.Equal(expectedMultip, actualData);
        }


        [Theory]
        [InlineData(20, 5)]
        public void multip_externalValues_returnMultipValueForCallBack(int a, int b)
        {
            //Elde ettiğimiz değerin değişkenini oluşturdum
            int actualMultip = default(int);

            //myMock servisimiz de ki çarpma işlemini taklit ediyor ama bu sefer bizler parametreleri hazır vermiyoruz ve int tipinde herhangi bir değer alabileceğini belirtiyoruz ve metodun hangi değeri döneceğini sabit olarak karar vermeyip programatik olarak oluşturmasını istiyoruz.
            myMock.Setup(x => x.multip(It.IsAny<int>(), It.IsAny<int>())).Callback<int, int>((x, y) => actualMultip = x * y);

            _calculator.multip(a, b);

            var actualData = _calculator.multip(a, b);
            Assert.Equal(100, actualMultip);

            //Dışarıdan parametre alabilecek şekilde oluşturduğumuz için aynı test metodu içerisinde bir den fazla test senaryosu oluşturabildik. Inline Data olarakta verebilirdik ama programatik olarak test etmemiz gerekiyorsa bu şekilde daha sağlıklı olacaktır
            _calculator.multip(100, 150);
            Assert.Equal(15000, actualMultip);
        }


        //Hatayı handle etmek ile ilgili test
        [Theory]
        [InlineData(0, 5)]
        public void multip_zeroValues_returnException(int a, int b)
        {
            //myMock taklidinin hangi metotta nasıl davranması gerektiğini belirledim (multip metodunun hata fırlatılması simüle ediliyor)
            myMock.Setup(x => x.multip(a, b)).Throws(new Exception("a=0 olamaz"));

            //Exception'u handle edebiliyor muyuz
            var exception = Assert.Throws<Exception>(() => _calculator.multip(a, b));
            //Gelen hata mesajı beklediğim bir mesaj mı?
            Assert.Equal("a=0 olamaz", exception.Message);
        }
    }
}
