using FlightConnectionsDotCom_ClassLibrary.Interfaces;
using System;
using System.Collections.Generic;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class AirportGenerator : IAirportGenerator
    {
        public List<string> GetAllPossiblePermutationsOfLetters(string lettersToPermutate)
        {
            char[] alphabetArray = lettersToPermutate.ToCharArray();
            List<string> result = new();
            foreach (char letter1 in alphabetArray)
            {
                foreach (char letter2 in alphabetArray)
                {
                    foreach (char letter3 in alphabetArray)
                    {
                        result.Add($"{letter1}{letter2}{letter3}");
                    }
                }
            }
            return result;
        }
    }
}