#region

using System;
using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace Italliance.Modules.DnnHosting.Components.ExcelDataExtractor
{
    internal static class SpreadsheetHelper
    {
        private const int A_CHAR_VALUE = 'A';
        private const int INTERVAL_VALUE = 'Z' - A_CHAR_VALUE + 1;

        /// <summary>
        ///     Converts a string column header with letters only
        ///     to a numeric value representing the column.
        /// </summary>
        /// <param name="columnHeader"></param>
        /// <returns></returns>
        public static int ConvertColumnHeaderToNumber(string columnHeader)
        {
            if (string.IsNullOrWhiteSpace(columnHeader))
            {
                throw new ArgumentNullException(nameof(columnHeader));
            }

            columnHeader = columnHeader.ToUpperInvariant();

            if (!Regex.IsMatch(columnHeader, "^[A-Z]+$"))
            {
                throw new ArgumentException("The given column header is in an invalid format. Only A to Z letters are supported.");
            }

            int columnIndex = columnHeader.Last() - A_CHAR_VALUE + 1;
            for (int index = 0; index < columnHeader.Length - 1; index++)
            {
                int letterValue = columnHeader[index] - A_CHAR_VALUE + 1;

                int power = columnHeader.Length - (index + 1);

                columnIndex += (letterValue * (int) Math.Pow(INTERVAL_VALUE, power));
            }

            return columnIndex;
        }
    }
}