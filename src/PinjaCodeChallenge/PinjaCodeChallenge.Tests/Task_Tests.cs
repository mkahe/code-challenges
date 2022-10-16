using System;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace PinjaCodeChallenge.Tests
{
    public class Task_Tests
    {
        [Theory]
        [InlineData("<>vv", 2)]
        [InlineData("^vvvv^", 2)]
        [InlineData(">>><<<<", 3)]
        [InlineData("", 0)]
        public void Task1_Tests(string S, int expected) 
        {
            var result = Program.Task1(S);
            Assert.Equal(expected, result);
        }
        
        [Theory]
        [InlineData("aabbb", 1)]
        [InlineData("aaaa", 0)]
        [InlineData("babbaaaa", 8)]
        [InlineData("", 0)]
        public void Task2_Tests(string S, int expected) 
        {
            var result = Program.Task2(S);
            Assert.Equal(expected, result);
        }
        
        [Theory]
        [InlineData(new int[]{10, -10, -1, -1, 10}, 1)]
        [InlineData(new int[]{5, -2, -3, 4}, 0)]
        [InlineData(new int[]{-1, -1, -1, 1, 1, 1, 1}, 2)]
        [InlineData(new int[]{-10, 10, -2, 13}, 1)]
        public void Task3_Tests(int[] A, int expected) 
        {
            var result = Program.Taks3(A);
            Assert.Equal(expected, result);
        }
    }
}