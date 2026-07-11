using Xunit;
using Moq;
using task05;
using System.Linq;

namespace task05test{
    public class TestClass
    {
        public int PublicField;
        private string _privateField = "";
        public int Property { get; set; }

        public void Method() { }
        
        public int MethodWithParams(int param1, string param2) { return 0; }
        
        public static void StaticMethod() { }
        
        private void PrivateMethod() { }
    }

    [Serializable]
    public class AttributedClass { }

    public class ClassWithoutAttribute { }

    public class ClassAnalyzerTests
    {

        [Fact]
        public void GetPublicMethods_ReturnsCorrectMethods()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var methods = analyzer.GetPublicMethods();

            Assert.Contains("Method", methods);
        }

        [Fact]
        public void GetPublicMethods_IncludesMethodWithParams()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var methods = analyzer.GetPublicMethods().ToList();

            Assert.Contains("MethodWithParams", methods);
        }

        [Fact]
        public void GetPublicMethods_IncludesStaticMethods()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var methods = analyzer.GetPublicMethods().ToList();

            Assert.Contains("StaticMethod", methods);
        }

        [Fact]
        public void GetPublicMethods_DoesNotIncludePrivateMethods()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var methods = analyzer.GetPublicMethods().ToList();

            Assert.DoesNotContain("PrivateMethod", methods);
        }

        [Fact]
        public void GetPublicMethods_DoesNotIncludePropertyAccessors()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var methods = analyzer.GetPublicMethods().ToList();

            Assert.DoesNotContain("get_Property", methods);
            Assert.DoesNotContain("set_Property", methods);
        }


        [Fact]
        public void GetMethodParams_MethodWithoutParams_ReturnsOnlyReturnType()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var result = analyzer.GetMethodParams("Method").ToList();

            Assert.Single(result);
            Assert.Contains("Void", result);
        }

        [Fact]
        public void GetMethodParams_MethodWithParams_ReturnsParamsAndReturnType()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var result = analyzer.GetMethodParams("MethodWithParams").ToList();

            Assert.Equal(3, result.Count);
            Assert.Contains("param1", result);
            Assert.Contains("param2", result);
            Assert.Contains("Int32", result);
        }

        [Fact]
        public void GetMethodParams_NonExistentMethod_ReturnsEmpty()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var result = analyzer.GetMethodParams("NonExistentMethod").ToList();

            Assert.Empty(result);
        }

        [Fact]
        public void GetAllFields_IncludesPrivateFields()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var fields = analyzer.GetAllFields().ToList();

            Assert.Contains("_privateField", fields);
        }

        [Fact]
        public void GetAllFields_IncludesPublicFields()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var fields = analyzer.GetAllFields().ToList();

            Assert.Contains("PublicField", fields);
        }

        [Fact]
        public void GetAllFields_DoesNotIncludeProperties()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var fields = analyzer.GetAllFields().ToList();

            Assert.DoesNotContain("Property", fields);
        }

        [Fact]
        public void GetProperties_ReturnsAllProperties()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var properties = analyzer.GetProperties().ToList();

            Assert.Contains("Property", properties);
        }

        [Fact]
        public void GetProperties_DoesNotIncludeFields()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var properties = analyzer.GetProperties().ToList();

            Assert.DoesNotContain("PublicField", properties);
            Assert.DoesNotContain("_privateField", properties);
        }

        [Fact]
        public void HasAttribute_WhenAttributeExists_ReturnsTrue()
        {
            var analyzer = new ClassAnalyzer(typeof(AttributedClass));
            var result = analyzer.HasAttribute<System.SerializableAttribute>();

            Assert.True(result);
        }

        [Fact]
        public void HasAttribute_WhenAttributeNotExists_ReturnsFalse()
        {
            var analyzer = new ClassAnalyzer(typeof(ClassWithoutAttribute));
            var result = analyzer.HasAttribute<System.SerializableAttribute>();

            Assert.False(result);
        }

        [Fact]
        public void Constructor_NullType_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => new ClassAnalyzer(null!));
        }

        [Fact]
        public void GetPublicMethods_BuiltInType_ReturnsMethods()
        {
            var analyzer = new ClassAnalyzer(typeof(string));
            var methods = analyzer.GetPublicMethods().ToList();

            Assert.Contains("ToString", methods);
            Assert.Contains("Equals", methods);
        }
    }
}