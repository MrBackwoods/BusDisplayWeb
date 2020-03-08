using System;

namespace BusDisplayWeb
{   
    public static class TimeHandler
    {
        // Helper function for converting local datetime from timestamp
        public static DateTime ConvertToLocalTimeDateTime(int timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestamp).ToLocalTime();
        }
    }
}
