using NUnit.Framework;
using System;

namespace LearnUnitTests
{
    public class Tests
    {
        int value;
        [SetUp]
        public void Setup()
        {
            value++;
        }
        [TearDown]
        public void TearDown()
        {
            value = 0;
        }

        [Test]
        public void Test()
        {
            value++;
            Console.WriteLine(value);
            Assert.Pass();

        }
        [Test]
        public void Test1()
        {
            value++;
            Console.WriteLine(value);
            Assert.Pass();

        }

    }
}
