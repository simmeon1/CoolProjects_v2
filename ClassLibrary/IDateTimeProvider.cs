using System;

namespace ClassLibrary
{
    public interface IDateTimeProvider
    {
        DateTime GetDateTimeNow();
    }
}