using gRPCServer.Services;
using Xunit;

namespace ConverterTesting
{
    public class UnitTest1
    {
        private MoneyToWords converter = new();
        [Fact]
        public void Test1()
        {
            string r = converter.runConverter("0");
            Assert.Equal("zero dollars",r);
        }
        [Fact]
        public void Test2()
        {
            Assert.Equal("one dollar", converter.runConverter("1"));
        }
        [Fact]
        public void Test3()
        {
            Assert.Equal("twenty-five dollars and ten cents", converter.runConverter("25,1"));
        }
        [Fact]
        public void Test4()
        {
            Assert.Equal("zero dollars and one cent", converter.runConverter("0,01"));
        }
        [Fact]
        public void Test5()
        {
            Assert.Equal("forty-five thousand one hundred dollars", converter.runConverter("45 100"));
        }
        [Fact]
        public void Test6()
        {
            Assert.Equal("nine hundred ninety-nine million nine hundred ninety-nine thousand nine hundred ninety-nine dollars and ninety-nine cents", converter.runConverter("999 999 999,99"));
        }
        [Fact]
        public void Test7()
        {
            Assert.Equal("one hundred eleven million twenty-one thousand one dollars and ten cents", converter.runConverter("111 021 001,10"));
        }
        [Fact]
        public void Test8()
        {
            Assert.Equal("one hundred million twenty-one thousand one dollars and ten cents", converter.runConverter("100 021 001,10"));
        }
        [Fact]
        public void Test9()
        {
            Assert.Equal("one hundred dollars", converter.runConverter("100"));
        }
        [Fact]
        public void Test10()
        {
            Assert.Equal("one hundred thousand one hundred dollars", converter.runConverter("100100"));
        }
        [Fact]
        public void Test11()
        {
            Assert.Equal("one dollar", converter.runConverter("00001"));
        }
    }
}