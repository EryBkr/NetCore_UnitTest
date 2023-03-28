namespace XUnitTest.App
{
    //Moq Framework'ün kullanılabilmesi için soyutlama yapmak gerekmektedir.Biz de Calculator class'ının kullanılışını bu şekilde clean hale getirdik
    public class Calculator
    {
        private ICalculatorService _calculatorService;

        public Calculator(ICalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        public int add(int a, int b)
        {
            return _calculatorService.add(a, b);
        }

        public int multip(int a, int b)
        {
            return _calculatorService.multip(a, b);
        }
    }
}
