using System;

namespace task05
{
    // Тестовый атрибут
    [AttributeUsage(AttributeTargets.Class)]
    public class SampleAttribute : Attribute
    {
    }

    // Тестовый класс с атрибутом
    [Sample]
    public class SampleClass
    {
        private int _privateField;
        public string PublicField;
        protected double ProtectedField;
        private static int _staticField;

        public int PublicProperty { get; set; }
        private string PrivateProperty { get; set; }

        public void MethodWithoutParams()
        {
        }

        public int MethodWithParams(int param1, string param2)
        {
            return 0;
        }

        public static void StaticMethod(double value)
        {
        }

        private void PrivateMethod()
        {
        }
    }

    public class ClassWithoutAttribute
    {
        public int Value { get; set; }
    }
}