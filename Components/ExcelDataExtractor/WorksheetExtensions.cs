#region

using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

#endregion

namespace Italliance.Modules.DnnHosting.Components.ExcelDataExtractor
{
    public static class WorksheetExtensions
    {
        /// <summary>
        ///     Creates an <see cref="IDataExtractor{TRow}" /> to extract data from
        ///     the worksheet to <typeparamref name="TRow" /> objects.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="worksheet" /> is null.</exception>
        /// <typeparam name="TRow">
        ///     The type that will be populated
        ///     with the data from the worksheet.
        /// </typeparam>
        /// <param name="worksheet">The worksheet parameter.</param>
        /// <returns>An instance of <see cref="IDataExtractor{TRow}" />.</returns>
        public static IDataExtractor<TRow> Extract<TRow>(this ExcelWorksheet worksheet) where TRow : class, new()
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException(nameof(worksheet));
            }

            return new DataExtractor<TRow>(worksheet);
        }

        /// <summary>
        ///     Checks whether the given range is empty or not
        /// </summary>
        /// <param name="address">Excel cell range</param>
        /// <param name="hasHeaderRow">'false' as default</param>
        /// <returns>'true' or 'false'</returns>
        public static bool IsEmptyRange(this ExcelAddressBase address, bool hasHeaderRow = false)
        {
            return !hasHeaderRow ? address.Start.Row == 0 : address.Start.Row == address.End.Row;
        }

        /// <summary>
        ///     Returns the data bounds of the worksheet
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="hasHeaderRow"></param>
        /// <returns>ExcelAddress</returns>
        public static ExcelAddress GetDataBounds(this ExcelWorksheet worksheet, bool hasHeaderRow = true)
        {
            ExcelAddressBase valuedDimension = worksheet.GetValuedDimension() ?? worksheet.Dimension;

            if (valuedDimension == null)
            {
                return null;
            }

            return new ExcelAddress(valuedDimension.Start.Row + (hasHeaderRow && valuedDimension.Start.Row != valuedDimension.End.Row ? 1 : 0),
                                    valuedDimension.Start.Column,
                                    valuedDimension.End.Row,
                                    valuedDimension.End.Column);
        }

        public static ExcelWorksheet GetWorksheet(this ExcelWorkbook workbook, string worksheetName)
        {
            return workbook.Worksheets.FirstOrDefault(x => x.Name == worksheetName);
        }

        public static ExcelWorksheet GetWorksheet(this ExcelWorkbook workbook, int worksheetIndex)
        {
            return workbook.Worksheets[worksheetIndex];
        }

        /// <summary>
        ///     Returns cell ranges of the worksheet
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="hasHeaderRow"></param>
        /// <returns></returns>
        public static ExcelRange GetExcelRange(this ExcelWorksheet worksheet, bool hasHeaderRow = true)
        {
            ExcelAddress dataBounds = worksheet.GetDataBounds(hasHeaderRow);

            return dataBounds == null ? null : worksheet.Cells[dataBounds.Address];
        }

        /// <summary>
        ///     Returns index and value pairs of columns
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<int, string>> GetColumns(this ExcelWorksheet worksheet, int rowIndex)
        {
            ExcelAddressBase valuedDimension = worksheet.GetValuedDimension();

            if (valuedDimension == null)
            {
                yield break;
            }

            for (int i = valuedDimension.Start.Column; i <= valuedDimension.End.Column; i++)
            {
                yield return new KeyValuePair<int, string>(i, worksheet.Cells[rowIndex, i, rowIndex, i].Text);
            }
        }

        /// <summary>
        ///     Gets valued dimensions of worksheet
        /// </summary>
        /// <param name="worksheet"></param>
        /// <returns></returns>
        public static ExcelAddressBase GetValuedDimension(this ExcelWorksheet worksheet)
        {
            ExcelAddressBase dimension = worksheet.Dimension;

            if (dimension == null)
            {
                return null;
            }

            ExcelRange cells = worksheet.Cells[dimension.Address];
            int minRow = 0, minCol = 0, maxRow = 0, maxCol = 0;
            var hasValue = false;
            foreach (ExcelRangeBase cell in cells.Where(cell => cell.Value != null))
            {
                if (!hasValue)
                {
                    minRow = cell.Start.Row;
                    minCol = cell.Start.Column;
                    maxRow = cell.End.Row;
                    maxCol = cell.End.Column;
                    hasValue = true;
                }
                else
                {
                    if (cell.Start.Column < minCol)
                    {
                        minCol = cell.Start.Column;
                    }

                    if (cell.End.Row > maxRow)
                    {
                        maxRow = cell.End.Row;
                    }

                    if (cell.End.Column > maxCol)
                    {
                        maxCol = cell.End.Column;
                    }
                }
            }

            return hasValue ? new ExcelAddressBase(minRow, minCol, maxRow, maxCol) : null;
        }

        /// <summary>
        ///     Checks whether given worksheet address has a value or not
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static bool IsCellEmpty(this ExcelWorksheet worksheet, int rowIndex, int columnIndex)
        {
            object value = worksheet.Cells[rowIndex, columnIndex, rowIndex, columnIndex]?.Value;
            return string.IsNullOrWhiteSpace(value?.ToString());
        }
    }
}