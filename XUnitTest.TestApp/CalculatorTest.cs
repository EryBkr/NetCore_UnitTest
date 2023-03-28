using Xunit;
using XUnitTest.App;

namespace XUnitTest.TestApp
{
    //Class isimlendirmesi test işlemlerinde önemlidir
    public class CalculatorTest
    {
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void AddTest()
        {
            //Arrange --> Initilaizer işlemleri
            int a = 5;
            int b = 20;
            var calculator = new Calculator();

            //Act --> Test edilecek metotlar çalıştırılır
            var total = calculator.add(a, b);

            //Assert --> Doğrulama evresi
            //Beklenen ve aslında elde ettiğimiz değerleri veriyoruz
            Assert.Equal(25, total);
        }
    }
}
