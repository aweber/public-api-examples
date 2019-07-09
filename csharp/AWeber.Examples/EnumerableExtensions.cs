using System.Collections.Generic;
using System.Text;

namespace AWeber.Examples
{
    public static class EnumerableExtensions
    {
        public static string Stringify(this IEnumerable<string> values)
        {
            var builder = new StringBuilder();
            var isFirstElement = true;
            builder.Append("[");
            foreach (var value in values)
            {
                if (isFirstElement)
                {
                    isFirstElement = false;
                }
                else
                {
                    builder.Append(",");
                }
                builder.Append("\"");
                builder.Append(value);
                builder.Append("\"");
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}