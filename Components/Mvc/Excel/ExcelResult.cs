﻿#region

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.Excel
{
    public sealed class ExcelResult<T> : ActionResult where T : class
    {
        public ExcelResult(IEnumerable<T> data) : this(data, typeof(T).FullName, typeof(T).FullName)
        { }

        public ExcelResult(IEnumerable<T> data, string fileName) : this(data, fileName, fileName)
        { }

        public ExcelResult(IEnumerable<T> data, string fileName, string sheetName)
        {
            Data = data;
            FileName = fileName;
            SheetName = sheetName;
        }

        public IEnumerable<T> Data { get; set; }

        public string FileName { get; set; }

        public string SheetName { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (Data == null)
            {
                return;
            }

            byte[] data = SerializeAsExcelByteArray();

            var response = context.HttpContext.Response;
            response.ClearHeaders();
            response.ClearContent();
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AppendHeader("content-disposition", $"attachment;  filename={FileName}.xlsx");
            response.AppendHeader("Content-Length", data.Length.ToString());
            //response.ContentEncoding = Encoding.UTF8;
            response.Buffer = true;
            response.BinaryWrite(data);
            response.Flush();
            response.End();
        }

        public byte[] SerializeAsExcelByteArray()
        {
            using (var application = new ExcelPackage())
            {
                application.Workbook.Properties.Title = SheetName;
                var sheet = application.Workbook.Worksheets.Add(SheetName);
                var sheetStyle = typeof(T).GetCustomAttributes(false).FirstOrDefault(o => o is ExcelSheetStyleAttribute) as ExcelSheetStyleAttribute
                              ?? new ExcelSheetStyleAttribute();

                var allowedPropertiesDictionary = typeof(T).GetProperties()
                                                           .Select(property => new {Property = property, Attributes = property.GetCustomAttributes(false)})
                                                           .Where(o => o.Attributes.All(attribute => !(attribute is ExcelIgnoreAttribute)))
                                                           .ToDictionary(property => property.Property, property => property.Attributes);
                GenerateExcelHeader(sheet,
                                    allowedPropertiesDictionary,
                                    new Font(sheetStyle.FontFamily, sheetStyle.HeaderFontSize, sheetStyle.IsHeaderBold ? FontStyle.Bold : FontStyle.Regular),
                                    sheetStyle.HeaderBackgroundColor);
                GenerateExcelBody(sheet, allowedPropertiesDictionary, new Font(sheetStyle.FontFamily, sheetStyle.BodyFontSize));
                FormatSheet(sheet, allowedPropertiesDictionary);
                return application.GetAsByteArray();
            }
        }

        private void FormatSheet(ExcelWorksheet sheet, Dictionary<PropertyInfo, object[]> properties)
        {
            sheet.Cells.AutoFitColumns();

            for (var c = 0; c < properties.Count; c++)
            {
                var currentColumn = sheet.Column(c + 1);
                var colStyle = properties.ElementAt(c).Value.FirstOrDefault(o => o is ExcelColumnStyleAttribute) as ExcelColumnStyleAttribute;
                if (colStyle?.Width > 0)
                {
                    currentColumn.Width = colStyle.Width;
                }
                else
                {
                    currentColumn.Width += 3;
                }
            }
        }

        private void GenerateExcelHeader(ExcelWorksheet sheet, Dictionary<PropertyInfo, object[]> properties, Font headerFont, Color headerBackgroundColor)
        {
            var currentColumn = 1;
            foreach (var property in properties.OrderBy(p => OrderSelector(p.Key)))
            {
                var currentCell = sheet.Cells[1, currentColumn];
                currentCell.Value = (property.Value.FirstOrDefault(o => o is ExcelDisplayNameAttribute) as ExcelDisplayNameAttribute)?.Name
                                 ?? (property.Value.FirstOrDefault(o => o is DisplayAttribute) as DisplayAttribute)?.Name ?? property.Key.Name;
                currentCell.Style.Font.SetFromFont(headerFont);
                currentCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                currentCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                currentCell.Style.Fill.BackgroundColor.SetColor(headerBackgroundColor);
                currentColumn++;
            }
        }

        private void GenerateExcelBody(ExcelWorksheet sheet, Dictionary<PropertyInfo, object[]> properties, Font font)
        {
            var currentRow = 2;
            foreach (var item in Data)
            {
                var currentColumn = 1;
                foreach (var property in properties.OrderBy(p => OrderSelector(p.Key)))
                {
                    var currentColumnStyle = property.Value.FirstOrDefault(o => o is ExcelColumnStyleAttribute) as ExcelColumnStyleAttribute ?? new ExcelColumnStyleAttribute();

                    var currentCell = sheet.Cells[currentRow, currentColumn];

                    var valueFormat = property.Value.FirstOrDefault(o => o is ExcelValueFormatAttribute) as ExcelValueFormatAttribute;

                    if (valueFormat != null && typeof(IFormattable).IsAssignableFrom(property.Key.PropertyType))
                    {
                        currentCell.Value = ((IFormattable) property.Key.GetValue(item)).ToString(valueFormat.Format, CultureInfo.InvariantCulture);
                    }
                    else if (valueFormat != null && IsNullable(property.Key.PropertyType))
                    {
                        currentCell.Value = typeof(IFormattable).IsAssignableFrom(property.Key.PropertyType.GetGenericArguments()[0])
                                                ? property.Key.GetValue(item) == null ? null :
                                                  ((IFormattable) property.Key.GetValue(item)).ToString(valueFormat.Format, CultureInfo.InvariantCulture)
                                                : property.Key.GetValue(item)?.ToString();
                    }
                    else
                    {
                        currentCell.Value = property.Key.GetValue(item)?.ToString();
                    }

                    currentCell.Style.Font.SetFromFont(font);
                    currentCell.Style.Indent = 1;
                    currentCell.Style.HorizontalAlignment = currentColumnStyle.ExcelHorizontalAlignment;
                    currentCell.Style.VerticalAlignment = currentColumnStyle.ExcelVerticalAlignment;
                    currentCell.Style.WrapText = currentColumnStyle.WordWrap;
                    currentCell.Style.Numberformat.Format = "@";
                    currentColumn++;
                }

                currentRow++;
            }
        }

        private static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static Type ToUnderlying(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (IsNullable(type))
            {
                type = type.GetGenericArguments()[0];
            }

            if (type.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
            }

            return type;
        }

        private static int OrderSelector(PropertyInfo propertyInfo)
        {
            DisplayAttribute displayAttribute = propertyInfo.GetCustomAttributes<DisplayAttribute>().FirstOrDefault();
            return displayAttribute?.GetOrder() ?? -1;
        }
    }
}