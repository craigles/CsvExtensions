﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CSVExtensions
{
    public static class EnumerableExtensions
    {
        private const string Seperator = ",";

        public static string AsCsvString<T>(this IEnumerable<T> items)
        {
            var sb = new StringBuilder();

            var properties = typeof(T).GetProperties().Select(property => property).ToArray();
            var propertyNames = properties.Select(property => property.Name);

            sb.AppendLine(string.Join(Seperator, propertyNames));

            foreach (var item in items)
            {
                var values = properties.Select(p =>
                {
                    var propertyValue = p.GetValue(item)?.ToString();

                    if (string.IsNullOrEmpty(propertyValue))
                    {
                        return "\"\"";
                    }

                    return $"\"{propertyValue.Escape('\"')}\"";
                });

                sb.AppendLine(string.Join(Seperator, values));
            }

            return sb.ToString();
        }

        public static string AsCsvString<T>(this IEnumerable<T> items, string[] headers, params Func<T, object>[] valueFuncs)
        {
            var sb = new StringBuilder();

            sb.AppendLine(string.Join(Seperator, headers));

            foreach (var item in items)
            {
                var values = valueFuncs.Select(valueFunc =>
                {
                    var value = valueFunc(item).ToString().Escape('\"');
                    return $"\"{value}\"";
                });

                sb.AppendLine(string.Join(Seperator, values));
            }

            return sb.ToString();
        }

        private static string Escape(this string source, char charToEscape)
        {
            return source.Replace(charToEscape.ToString(), $"{charToEscape}{charToEscape}");
        }
    }
}