using RevitTests.TestingUtils;
using System.ComponentModel;
using System.Reflection;
using RxBim.Shared

namespace Void_Profile_Editor.Tests
{
    public class Tests
    {
        private IContainer _container;
        [SetUp]
        public void Setup()
        {
            var testingDIConfigurator = new TestingDiConfigurator();
            testingDIConfigurator.Configure(Assembly.GetExecutingAssembly());
            _container = testingDIConfigurator.Container;
          

        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}