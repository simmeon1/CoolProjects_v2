using System;

namespace Common_ClassLibrary
{
    public class RealDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}