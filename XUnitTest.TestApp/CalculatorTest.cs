using Xunit;
using XUnitTest.App;

namespace XUnitTest.TestApp
{
    //Class isimlendirmesi test işlemlerinde önemlidir
    public class CalculatorTest
    {
        public Calculator _calculator { get; set; }


        //Test class'ları en nihayetinde normal class'lardır, yapıcı metot kullanabiliriz
        public CalculatorTest()
        {
            //Arrange Object
            _calculator = new Calculator();
        }

       
        //Test metotları isimlendirilirken MethodNaame_StateUnderTest_ExpectedBehavior şeklinde bir şema izlemek uygundur
                
        [Fact] //Test metodu olduğunu belirtiyoruz (Fact testleri parametre almazlar)
        public void add_simpleValues_returnTotalValue()
        {
            //Arrange --> Initilaizer işlemleri
            int a = 5;
            int b = 20;

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
            //Act --> Test edilecek metotlar çalıştırılır
            //Gerçek datayı elde ediyorum
            var actualData = _calculator.add(a, b);

            //Assert --> Doğrulama evresi
            //Beklenen ve aslında elde ettiğimiz değerleri veriyoruz
            Assert.Equal(expectedTotal, actualData);
        }
    }
}
