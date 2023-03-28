using System.Collections.Generic;
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

        #region Contains/Does Not Contains
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestContain()
        {
            //Beklenen ve aslında elde ettiğimiz değerleri veriyoruz
            //Karşılaştırma neticesinde karşılaştırılan değer örnek değeri kapsıyorsa testimiz geçerlidir (case-sensetive).
            Assert.Contains("Er", "Eray");
        }

        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestContainForList()
        {
            //Arrange
            var names = new List<string>() { "eray", "berkay" };

            //Bu sefer ienumerable düzeyinde bir karşılaştırma yaptık
            Assert.Contains(names, x => x == "eray");
        }

        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestDoesNotContain()
        {
            //Beklenen ve aslında elde ettiğimiz değerleri veriyoruz
            //Karşılaştırma neticesinde karşılaştırılan değer örnek değeri kapsamıyorsa (eşleşme yok ise) testimiz geçerlidir (case-sensetive).
            Assert.DoesNotContain("ber", "Eray");
        }
        #endregion

        #region True/False
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestTrue()
        {
            //Mantık Basit (Sonucun doğruluğunu test ediyoruz)
            Assert.True(5 > 1);
        }
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestFalse()
        {
            //Mantık Basit (Sonucun bilakis yanlış olması gerektiği durumu test ediyoruz)
            Assert.False(5 < 1);
        }

        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestTrueForType()
        {
            //Mantık Basit
            Assert.True("".GetType() == typeof(string));
        }
        #endregion

        #region Match/Does Not Match
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestMatch()
        {
            //Regex kontrolünü sağlar (doğru senaryo için)
            var regex = "^eray";
            //regex kuralına göre verdiğimiz değer gerekli eşleşmeyi sağlıyor mu?
            Assert.Matches(regex, "eray bakır");
        }

        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestDoesNotMatch()
        {
            //Regex kontrolünü sağlar (yanlış beklediğimiz senaryo için)
            var regex = "^eray";
            //regex kuralına göre verdiğimiz değer gerekli eşleşmeyi sağlıyor mu?
            Assert.DoesNotMatch(regex, "bakır eray");
        }
        #endregion

        #region Start With/End With
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestStartWith()
        {
            //Beklenen değer ile asıl değerin başlangıçlarını kıyaslıyor
            Assert.StartsWith("B", "Berkay");
        }
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestEndWith()
        {
            //Beklenen değer ile asıl değerin sonlarını kıyaslıyor
            Assert.EndsWith("y", "Berkay");
        }
        #endregion

        #region Empty / NotEmpty
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestEmptyWith()
        {
            //Dizinin boş olup olmadığını kontrol ediyoruz,aslında boş olmasını bekliyoruz
            Assert.Empty(new List<string>());
        }

        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestNotEmptyWith()
        {
            //Dizinin boş olup olmadığını kontrol ediyoruz,aslında boş olmamasını bekliyoruz
            Assert.NotEmpty(new List<string>() { "eray" });
        }
        #endregion

        #region InRange/NotInRange
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestInRange()
        {
            //Beklediğimiz sayının 2 ile 30 arasında olup olmadığını teyit ettiğimiz test metottur
            Assert.InRange(20, 2, 30);
        }
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestNotInRange()
        {
            //Beklediğimiz (elde edeceğimiz) sayının 2 ile 30 arasında olmamasını beklediğimiz metottur
            Assert.NotInRange(35, 2, 30);
        }
        #endregion

        #region Single
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestSingle()
        {
            //Array'imizde tek elemen olup olmadığını kontrol ettiğimiz metottur
            //Assert.Single<> şeklinde de generic bir şekilde de çalışabiliriz
            Assert.Single(new List<string>() { "eray" });
        }
        #endregion

        #region IsType/Is Not Type
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestIsType()
        {
            //Generic tip ile elde ettiğimiz datanın tipini kıyaslıyoruz, doğru olmasını bekliyoruz
            Assert.IsType<string>("Eray");
        }

        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestIsNotType()
        {
            //Generic tip ile elde ettiğimiz datanın tipini kıyaslıyoruz, sonucun yanlış olmasını bekliyoruz
            Assert.IsNotType<string>(23);
        }
        #endregion

        #region IsAssignableFrom
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestIsAssignableFrom()
        {
            //Generic tip ile elde ettiğimiz datanın tipini kıyaslıyoruz,elde ettiğimiz datanın tipi generic ifadeye atanabiliyorsa (ondan refere edilmişse) testimiz başarılı olacaktır
            Assert.IsAssignableFrom<IEnumerable<string>>(new List<string>() { "eray" });
        }
        #endregion

        #region Null/Not Null
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestNull()
        {
            string val = null;
            //Değerin null olmasını beklediimiz zamanlarda kullanırız
            Assert.Null(val);
        }

        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestNotNull()
        {
            string val = "";
            //Değerin null olmamasını beklediimiz zamanlarda kullanırız
            Assert.NotNull(val);
        }
        #endregion

        #region Equal/Not Equal
        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestEqual()
        {
            //Mantık Basit eşitliği kıyaslıyor
            Assert.Equal(2, 2);
        }

        [Fact] //Test metodu olduğunu belirtiyoruz
        public void TestNotEqual()
        {
            //Mantık Basit eşitliği kıyaslıyor
            Assert.NotEqual(2, 3);
        }
        #endregion
    }
}
