namespace XUnitTest.App
{
    public class CalculatorService : ICalculatorService
    {
        public int add(int a, int b)
        {
            if (a == 0 || b == 0)
                return 0;
            return a + b;
        }

        public int multip(int a,int b)
        {
            //Hatayı test aşamasında test etmek için ekledik
            if (a == 0)
                throw new System.Exception("a=0 olamaz");
            return a * b;
        }
    }
}
