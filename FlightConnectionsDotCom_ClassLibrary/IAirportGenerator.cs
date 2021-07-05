using System.Collections.Generic;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public interface IAirportGenerator
    {
        List<string> GetAllPossiblePermutationsOfLetters(string lettersToPermutate);
    }
}