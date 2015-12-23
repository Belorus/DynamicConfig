namespace DynamicConfig
{
    internal static class TypeExtensions
    {
        internal static short BitsCount(this int number)
        {
            int accum = number;
            short count = 0;
            while (accum > 0)
            {
                accum &= (accum - 1);
                count++;
            }
            return count;
        }
    }
}
