using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinjaCodeChallenge
{
    public class Program
    {
        static void Main(string[] args)
        {
        }

        public static int Task1(string S)
        {
            if (string.IsNullOrWhiteSpace(S)) return 0;
            var mostFrequentChar = S.GroupBy(x=>x).OrderBy(x=>x.Count()).Last().Key;
            var remaining = new StringBuilder();
            for (var i = 0; i < S.Length; i++)
            {
                if (mostFrequentChar != S[i])
                    remaining.Append(S[i]);
            }
            
            return remaining.Length;
        }
        
        public static int Task2(string S)
        {
            var myList = new List<int>();
            myList.Add(1);
            for(var i = 1; i<S.Length; i++){
                if(S[i] == S[i-1])
                {
                    myList[^1]++;
                    
                }
                else
                {
                    myList.Add(1);
                }
            }
            
            var max = myList.OrderBy(x=>x).Last();
            var counter = 0;
            foreach (var item in myList)
            {
                if (item < max)
                    counter += max - item;
            }

            return counter;
        }

        public static int Taks3(int[] A)
        {
            var myList = A.ToList();
            var balance = 0;
            var transitions = 0;

            for(var i = 0; i < myList.Count; i++){
                if(myList[i] + balance  < 0)
                {
                    var mostMinusValue = myList.Take(i+1).OrderBy(x=>x).First();
                    var indexOfMostMinusValue = myList.FindIndex(x => x == mostMinusValue);
                    myList.Add(myList.ElementAt(indexOfMostMinusValue));
                    myList.RemoveAt(indexOfMostMinusValue);
                    balance -= mostMinusValue; // This is what I have missed in my code :-(
                    transitions++;
                    i--;
                }
                else
                {
                    balance += myList[i];
                }
            }

            return transitions;
        }
    }
}