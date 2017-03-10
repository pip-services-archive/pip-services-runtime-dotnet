using System;

namespace PipServices.Runtime.Data
{
    public static class IdGenerator
    {
        private static readonly Random _random = new Random();

        public static string Short()
        {
            return _random.Next(100000000, 899999999).ToString();
        }

        public static string Uuid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}