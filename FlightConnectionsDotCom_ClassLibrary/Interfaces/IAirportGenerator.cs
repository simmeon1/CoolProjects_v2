using System.Collections.Generic;

namespace FlightConnectionsDotCom_ClassLibrary.Interfaces
{
    public interface IAirportGenerator
    {
        List<string> GetAllPossiblePermutationsOfLetters(string lettersToPermutate);
    }
}