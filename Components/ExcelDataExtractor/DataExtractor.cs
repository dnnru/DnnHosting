#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Italliance.Modules.DnnHosting.Components.ExcelDataExtractor.Data;
using Italliance.Modules.DnnHosting.Components.ExcelDataExtractor.DataExtractors;
using Italliance.Modules.DnnHosting.Components.ExcelDataExtractor.DataExtractors.CollectionColumn;
using OfficeOpenXml;

#endregion

namespace Italliance.Modules.DnnHosting.Components.ExcelDataExtractor
{
    internal static class DataExtractor
    {
        // Regex used to check the column string.
        internal static readonly Regex ColumnRegex = new Regex("^[A-Za-z]+$", RegexOptions.Compiled);
    }

    internal class DataExtractor<TRow> : ICollectionPropertyConfiguration<TRow> where TRow : class, new()
    {
        private readonly List<ICollectionColumnDataExtractor<TRow>> _collectionColumnSetters;
        private readonly List<IColumnDataExtractor<TRow>> _propertySetters;
        private readonly List<ISimpleCollectionColumnDataExtractor<TRow>> _simpleCollectionColumnSetters;
        private readonly ExcelWorksheet _worksheet;
        private int _headerRow = 1;

        internal DataExtractor(ExcelWorksheet worksheet)
        {
            _worksheet = worksheet;
            _propertySetters = new List<IColumnDataExtractor<TRow>>();
            _collectionColumnSetters = new List<ICollectionColumnDataExtractor<TRow>>();
            _simpleCollectionColumnSetters = new List<ISimpleCollectionColumnDataExtractor<TRow>>();
        }

        /// <summary>
        ///     Maps a property from the type defined as the row model
        ///     to the column identifier that has its value.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="propertyExpression">Expression for the property to be mapped.</param>
        /// <param name="column">
        ///     The column that contains the value to be mapped to
        ///     the property defined by <paramref name="propertyExpression" />.
        /// </param>
        /// <param name="setPropertyValueCallback">
        ///     Optional callback that gets executed before retrieving the cell value casted to <typeparamref name="TValue" />.
        ///     The first parameter contains the cell address and a method that can abort the entire execution.
        ///     The second one the value of the cell.
        /// </param>
        /// <param name="setPropertyCastedValueCallback">
        ///     Optional callback that gets executed after retrieving the cell value casted to <typeparamref name="TValue" />.
        ///     The first parameter contains the cell address and a method that can abort the entire execution.
        ///     The second one the value of the cell.
        /// </param>
        /// <returns></returns>
        public ICollectionPropertyConfiguration<TRow> WithProperty<TValue>(Expression<Func<TRow, TValue>> propertyExpression,
                                                                           string column,
                                                                           Action<PropertyExtractionContext, object> setPropertyValueCallback = null,
                                                                           Action<PropertyExtractionContext, TValue> setPropertyCastedValueCallback = null)
        {
            return WithProperty(propertyExpression, column, null, setPropertyValueCallback, setPropertyCastedValueCallback);
        }

        /// <summary>
        ///     Maps a property from the type defined as the row model
        ///     to the column identifier that has its value.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="propertyExpression">Expression for the property to be mapped.</param>
        /// <param name="column">
        ///     The column that contains the value to be mapped to
        ///     the property defined by <paramref name="propertyExpression" />.
        /// </param>
        /// <param name="convertDataFunc">
        ///     Function that can be used to convert the cell value, which is an object
        ///     to the desirable <typeparamref name="TValue" />.
        /// </param>
        /// <param name="setPropertyValueCallback">
        ///     Optional callback that gets executed prior to the <paramref name="convertDataFunc" />.
        ///     The first parameter contains the cell address and a method that can abort the entire execution.
        ///     The second one the value of the cell.
        /// </param>
        /// <param name="setPropertyCastedValueCallback">
        ///     Optional callback that gets executed after the <paramref name="convertDataFunc" />.
        ///     The first parameter contains the cell address and a method that can abort the entire execution.
        ///     The second one the value of the cell.
        /// </param>
        /// <returns></returns>
        public ICollectionPropertyConfiguration<TRow> WithProperty<TValue>(Expression<Func<TRow, TValue>> propertyExpression,
                                                                           string column,
                                                                           Func<object, TValue> convertDataFunc,
                                                                           Action<PropertyExtractionContext, object> setPropertyValueCallback = null,
                                                                           Action<PropertyExtractionContext, TValue> setPropertyCastedValueCallback = null)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            if (string.IsNullOrWhiteSpace(column))
            {
                throw new ArgumentNullException(nameof(column));
            }

            if (!DataExtractor.ColumnRegex.IsMatch(column))
            {
                throw new ArgumentException(@"The column value must contain only letters.", nameof(column));
            }

            _propertySetters.Add(new ColumnDataExtractor<TRow, TValue>(column, propertyExpression, convertDataFunc, setPropertyValueCallback, setPropertyCastedValueCallback));

            return this;
        }

        /// <summary>
        ///     Obtains the entities for the rows previously configured.
        /// </summary>
        /// <param name="fromRow">The initial row to start the data extraction.</param>
        /// <param name="toRow">The last row to read the data.</param>
        /// <returns>Returns an <see cref="IEnumerable{T}" /> with the data of the columns.</returns>
        public IEnumerable<TRow> GetData(int fromRow, int toRow)
        {
            return GetData(fromRow, currentRow => currentRow <= toRow);
        }

        /// <summary>
        ///     Obtains the entities for the columns previously configured.
        ///     The <paramref name="fromRow" /> indicates the initial row that will be read,
        ///     the data extraction will only occur while the
        ///     <param name="while" />
        ///     predicate returns true.
        ///     It'll get executed receiving the row index as parameter before extracting the data of each row.
        /// </summary>
        /// <param name="fromRow">The initial row to start the data extraction.</param>
        /// <param name="while">The condition that must.</param>
        /// <returns>Returns an <see cref="IEnumerable{T}" /> with the data of the columns.</returns>
        public IEnumerable<TRow> GetData(int fromRow, Predicate<int> @while)
        {
            if (@while is null)
            {
                throw new ArgumentNullException(nameof(@while));
            }

            for (int row = fromRow; @while(row); row++)
            {
                var dataInstance = new TRow();

                bool continueExecution = true;
                for (int index = 0; continueExecution && index < _propertySetters.Count; index++)
                {
                    continueExecution = _propertySetters[index].SetPropertyValue(dataInstance, row, _worksheet.Cells);
                }

                if (!continueExecution)
                {
                    yield return dataInstance;
                    break;
                }

                foreach (var collectionPropertySetter in _collectionColumnSetters)
                {
                    collectionPropertySetter.SetPropertyValue(dataInstance, row, _worksheet.Cells);
                }

                foreach (var simpleCollectionColumnSetter in _simpleCollectionColumnSetters)
                {
                    simpleCollectionColumnSetter.SetPropertyValue(dataInstance, row, _worksheet.Cells);
                }

                yield return dataInstance;
            }
        }

        public ICollectionPropertyConfiguration<TRow> WithCollectionProperty<TCollectionItem>(Expression<Func<TRow, List<TCollectionItem>>> propertyCollection,
                                                                                              string startColumn,
                                                                                              string endColumn) where TCollectionItem : class
        {
            if (propertyCollection is null)
            {
                throw new ArgumentNullException(nameof(propertyCollection));
            }

            ValidateColumn(startColumn, nameof(startColumn));
            ValidateColumn(endColumn, nameof(endColumn));

            propertyCollection.ValidatePropertyExpressionType<TRow, TCollectionItem, List<TCollectionItem>>();
            var collectionConfiguration = new SimpleNewableCollectionColumnDataExtractor<TRow, List<TCollectionItem>, TCollectionItem>(propertyCollection, startColumn, endColumn);

            _simpleCollectionColumnSetters.Add(collectionConfiguration);

            return this;
        }

        public ICollectionPropertyConfiguration<TRow> WithCollectionProperty<TCollectionItem>(Expression<Func<TRow, HashSet<TCollectionItem>>> propertyCollection,
                                                                                              string startColumn,
                                                                                              string endColumn) where TCollectionItem : class
        {
            if (propertyCollection is null)
            {
                throw new ArgumentNullException(nameof(propertyCollection));
            }

            ValidateColumn(startColumn, nameof(startColumn));
            ValidateColumn(endColumn, nameof(endColumn));

            propertyCollection.ValidatePropertyExpressionType<TRow, TCollectionItem, HashSet<TCollectionItem>>();
            var collectionConfiguration =
                new SimpleNewableCollectionColumnDataExtractor<TRow, HashSet<TCollectionItem>, TCollectionItem>(propertyCollection, startColumn, endColumn);

            _simpleCollectionColumnSetters.Add(collectionConfiguration);

            return this;
        }

        public ICollectionPropertyConfiguration<TRow> WithCollectionProperty<TCollectionItem>(Expression<Func<TRow, Collection<TCollectionItem>>> propertyCollection,
                                                                                              string startColumn,
                                                                                              string endColumn) where TCollectionItem : class
        {
            if (propertyCollection is null)
            {
                throw new ArgumentNullException(nameof(propertyCollection));
            }

            ValidateColumn(startColumn, nameof(startColumn));
            ValidateColumn(endColumn, nameof(endColumn));

            propertyCollection.ValidatePropertyExpressionType<TRow, TCollectionItem, Collection<TCollectionItem>>();

            var collectionConfiguration =
                new SimpleNewableCollectionColumnDataExtractor<TRow, Collection<TCollectionItem>, TCollectionItem>(propertyCollection, startColumn, endColumn);

            _simpleCollectionColumnSetters.Add(collectionConfiguration);

            return this;
        }

        public ICollectionPropertyConfiguration<TRow> WithCollectionProperty<TCollectionItem>(Func<TRow, ICollection<TCollectionItem>> propertyCollection,
                                                                                              string startColumn,
                                                                                              string endColumn) where TCollectionItem : class
        {
            return WithInitializedCollectionProperty(propertyCollection, startColumn, endColumn);
        }

        public ICollectionPropertyConfiguration<TRow> WithInitializedCollectionProperty<TCollectionItem>(Func<TRow, ICollection<TCollectionItem>> propertyCollection,
                                                                                                         string startColumn,
                                                                                                         string endColumn) where TCollectionItem : class
        {
            if (propertyCollection is null)
            {
                throw new ArgumentNullException(nameof(propertyCollection));
            }

            ValidateColumn(startColumn, nameof(startColumn));
            ValidateColumn(endColumn, nameof(endColumn));

            var collectionConfiguration = new SimpleCollectionColumnDataExtractor<TRow, ICollection<TCollectionItem>, TCollectionItem>(propertyCollection, startColumn, endColumn);

            _simpleCollectionColumnSetters.Add(collectionConfiguration);

            return this;
        }

        public ICollectionPropertyConfiguration<TRow> WithCollectionProperty<TCollectionItem, THeaderValue, TRowValue>(
            Expression<Func<TRow, List<TCollectionItem>>> propertyCollection,
            Expression<Func<TCollectionItem, THeaderValue>> headerProperty,
            int headerRow,
            Expression<Func<TCollectionItem, TRowValue>> rowProperty,
            string startColumn,
            string endColumn) where TCollectionItem : class, new()
        {
            if (propertyCollection is null)
            {
                throw new ArgumentNullException(nameof(propertyCollection));
            }

            if (headerProperty is null)
            {
                throw new ArgumentNullException(nameof(headerProperty));
            }

            if (rowProperty is null)
            {
                throw new ArgumentNullException(nameof(rowProperty));
            }

            ValidateColumn(startColumn, nameof(startColumn));
            ValidateColumn(endColumn, nameof(endColumn));

            propertyCollection.ValidatePropertyExpressionType<TRow, TCollectionItem, List<TCollectionItem>>();
            var collectionConfiguration =
                new NewableCollectionColumnDataExtractor<TRow, List<TCollectionItem>, TCollectionItem, THeaderValue, TRowValue>(propertyCollection,
                 headerProperty,
                 headerRow,
                 rowProperty,
                 startColumn,
                 endColumn);

            _collectionColumnSetters.Add(collectionConfiguration);

            return this;
        }

        public ICollectionPropertyConfiguration<TRow> WithCollectionProperty<TCollectionItem, THeaderValue, TRowValue>(
            Expression<Func<TRow, HashSet<TCollectionItem>>> propertyCollection,
            Expression<Func<TCollectionItem, THeaderValue>> headerProperty,
            int headerRow,
            Expression<Func<TCollectionItem, TRowValue>> rowProperty,
            string startColumn,
            string endColumn) where TCollectionItem : class, new()
        {
            if (propertyCollection is null)
            {
                throw new ArgumentNullException(nameof(propertyCollection));
            }

            if (headerProperty is null)
            {
                throw new ArgumentNullException(nameof(headerProperty));
            }

            if (rowProperty is null)
            {
                throw new ArgumentNullException(nameof(rowProperty));
            }

            ValidateColumn(startColumn, nameof(startColumn));
            ValidateColumn(endColumn, nameof(endColumn));

            propertyCollection.ValidatePropertyExpressionType<TRow, TCollectionItem, HashSet<TCollectionItem>>();
            var collectionConfiguration =
                new NewableCollectionColumnDataExtractor<TRow, HashSet<TCollectionItem>, TCollectionItem, THeaderValue, TRowValue>(propertyCollection,
                 headerProperty,
                 headerRow,
                 rowProperty,
                 startColumn,
                 endColumn);

            _collectionColumnSetters.Add(collectionConfiguration);

            return this;
        }

        public ICollectionPropertyConfiguration<TRow> WithCollectionProperty<TCollectionItem, THeaderValue, TRowValue>(
            Expression<Func<TRow, Collection<TCollectionItem>>> propertyCollection,
            Expression<Func<TCollectionItem, THeaderValue>> headerProperty,
            int headerRow,
            Expression<Func<TCollectionItem, TRowValue>> rowProperty,
            string startColumn,
            string endColumn) where TCollectionItem : class, new()
        {
            if (propertyCollection is null)
            {
                throw new ArgumentNullException(nameof(propertyCollection));
            }

            if (headerProperty is null)
            {
                throw new ArgumentNullException(nameof(headerProperty));
            }

            if (rowProperty is null)
            {
                throw new ArgumentNullException(nameof(rowProperty));
            }

            ValidateColumn(startColumn, nameof(startColumn));
            ValidateColumn(endColumn, nameof(endColumn));

            propertyCollection.ValidatePropertyExpressionType<TRow, TCollectionItem, Collection<TCollectionItem>>();

            var collectionConfiguration =
                new NewableCollectionColumnDataExtractor<TRow, Collection<TCollectionItem>, TCollectionItem, THeaderValue, TRowValue>(propertyCollection,
                 headerProperty,
                 headerRow,
                 rowProperty,
                 startColumn,
                 endColumn);

            _collectionColumnSetters.Add(collectionConfiguration);

            return this;
        }

        public ICollectionPropertyConfiguration<TRow> WithCollectionProperty<TCollectionItem, THeaderValue, TRowValue>(
            Func<TRow, ICollection<TCollectionItem>> collectionGetter,
            Expression<Func<TCollectionItem, THeaderValue>> headerProperty,
            int headerRow,
            Expression<Func<TCollectionItem, TRowValue>> rowProperty,
            string startColumn,
            string endColumn) where TCollectionItem : class, new()
        {
            return WithInitializedCollectionProperty(collectionGetter, headerProperty, headerRow, rowProperty, startColumn, endColumn);
        }

        public ICollectionPropertyConfiguration<TRow> WithInitializedCollectionProperty<TCollectionItem, THeaderValue, TRowValue>(
            Func<TRow, ICollection<TCollectionItem>> collectionGetter,
            Expression<Func<TCollectionItem, THeaderValue>> headerProperty,
            int headerRow,
            Expression<Func<TCollectionItem, TRowValue>> rowProperty,
            string startColumn,
            string endColumn) where TCollectionItem : class, new()
        {
            if (collectionGetter is null)
            {
                throw new ArgumentNullException(nameof(collectionGetter));
            }

            if (headerProperty is null)
            {
                throw new ArgumentNullException(nameof(headerProperty));
            }

            if (rowProperty is null)
            {
                throw new ArgumentNullException(nameof(rowProperty));
            }

            ValidateColumn(startColumn, nameof(startColumn));
            ValidateColumn(endColumn, nameof(endColumn));

            var collectionConfiguration =
                new CollectionColumnDataExtractor<TRow, ICollection<TCollectionItem>, TCollectionItem, THeaderValue, TRowValue>(collectionGetter,
                 headerProperty,
                 headerRow,
                 rowProperty,
                 startColumn,
                 endColumn);

            _collectionColumnSetters.Add(collectionConfiguration);

            return this;
        }

        /// <summary>
        ///     Configures a collection property to unpivot multiple columns to items in the collection property.
        ///     Different from the overloads, this method allows for having an undefined amount of columns
        ///     to be unpivoted to the collection.
        /// </summary>
        /// <param name="propertyCollection">
        ///     Expression for the collection that will be populated
        ///     with elements from the columns.
        /// </param>
        /// <param name="headerRow">
        ///     The number of the row where the header is defined. This row will be used
        ///     to search for the text of the collection columns mapping.
        /// </param>
        /// <param name="startingColumn">
        ///     Indicates the column address (with letters) where this collection
        ///     starts.
        /// </param>
        /// <param name="configurePropertiesAction">
        ///     Action to be used to configure the columns
        ///     for the collection items. Use the method
        ///     <see cref="IColumnToCollectionConfiguration.WithColumn{TRowValue}(Expression{Func{TCollectionItem, TRowValue}}, string)" />
        ///     to define the mappings of the columns.
        /// </param>
        /// <returns></returns>
        public ICollectionPropertyConfigurationWithoutColumnsToCollection<TRow> WithCollectionProperty<TCollectionItem>(
            Expression<Func<TRow, List<TCollectionItem>>> propertyCollection,
            int headerRow,
            string startingColumn,
            Action<IColumnToCollectionConfiguration<TCollectionItem>> configurePropertiesAction) where TCollectionItem : class, new()
        {
            if (propertyCollection == null)
            {
                throw new ArgumentNullException(nameof(propertyCollection));
            }

            if (startingColumn is null)
            {
                throw new ArgumentNullException(nameof(startingColumn));
            }

            ValidateColumn(startingColumn, nameof(startingColumn));

            propertyCollection.ValidatePropertyExpressionType<TRow, TCollectionItem, List<TCollectionItem>>();

            var columnToCollectionConfiguration = new ColumnToCollectionConfiguration<TCollectionItem>();
            configurePropertiesAction(columnToCollectionConfiguration);

            var columnToCollectionDataExtractor =
                new NewableColumnToCollectionDataExtractor<TRow, List<TCollectionItem>, TCollectionItem>(propertyCollection,
                                                                                                         headerRow,
                                                                                                         startingColumn,
                                                                                                         columnToCollectionConfiguration);
            _collectionColumnSetters.Add(columnToCollectionDataExtractor);

            return this;
        }

        /// <summary>
        ///     Configures a collection property to unpivot multiple columns to items in the collection property.
        ///     Different from the overloads, this method allows for having an undefined amount of columns
        ///     to be unpivoted to the collection.
        /// </summary>
        /// <param name="propertyCollection">
        ///     Expression for the collection that will be populated
        ///     with elements from the columns.
        /// </param>
        /// <param name="headerRow">
        ///     The number of the row where the header is defined. This row will be used
        ///     to search for the text of the collection columns mapping.
        /// </param>
        /// <param name="startingColumn">
        ///     Indicates the column address (with letters) where this collection
        ///     starts.
        /// </param>
        /// <param name="configurePropertiesAction">
        ///     Action to be used to configure the columns
        ///     for the collection items. Use the method
        ///     <see cref="IColumnToCollectionConfiguration.WithColumn{TRowValue}(Expression{Func{TCollectionItem, TRowValue}}, string)" />
        ///     to define the mappings of the columns.
        /// </param>
        /// <returns></returns>
        public ICollectionPropertyConfigurationWithoutColumnsToCollection<TRow> WithCollectionProperty<TCollectionItem>(
            Expression<Func<TRow, HashSet<TCollectionItem>>> propertyCollection,
            int headerRow,
            string startingColumn,
            Action<IColumnToCollectionConfiguration<TCollectionItem>> configurePropertiesAction) where TCollectionItem : class, new()
        {
            if (propertyCollection == null)
            {
                throw new ArgumentNullException(nameof(propertyCollection));
            }

            if (configurePropertiesAction == null)
            {
                throw new ArgumentNullException(nameof(configurePropertiesAction));
            }

            ValidateColumn(startingColumn, nameof(startingColumn));

            propertyCollection.ValidatePropertyExpressionType<TRow, TCollectionItem, HashSet<TCollectionItem>>();

            var columnToCollectionConfiguration = new ColumnToCollectionConfiguration<TCollectionItem>();
            configurePropertiesAction(columnToCollectionConfiguration);

            var columnToCollectionDataExtractor =
                new NewableColumnToCollectionDataExtractor<TRow, HashSet<TCollectionItem>, TCollectionItem>(propertyCollection,
                                                                                                            headerRow,
                                                                                                            startingColumn,
                                                                                                            columnToCollectionConfiguration);
            _collectionColumnSetters.Add(columnToCollectionDataExtractor);

            return this;
        }

        /// <summary>
        ///     Configures a collection property to unpivot multiple columns to items in the collection property.
        ///     Different from the overloads, this method allows for having an undefined amount of columns
        ///     to be unpivoted to the collection.
        /// </summary>
        /// <param name="propertyCollection">
        ///     Expression for the collection that will be populated
        ///     with elements from the columns.
        /// </param>
        /// <param name="headerRow">
        ///     The number of the row where the header is defined. This row will be used
        ///     to search for the text of the collection columns mapping.
        /// </param>
        /// <param name="startingColumn">
        ///     Indicates the column address (with letters) where this collection
        ///     starts.
        /// </param>
        /// <param name="configurePropertiesAction">
        ///     Action to be used to configure the columns
        ///     for the collection items. Use the method
        ///     <see cref="IColumnToCollectionConfiguration.WithColumn{TRowValue}(Expression{Func{TCollectionItem, TRowValue}}, string)" />
        ///     to define the mappings of the columns.
        /// </param>
        /// <returns></returns>
        public ICollectionPropertyConfigurationWithoutColumnsToCollection<TRow> WithCollectionProperty<TCollectionItem>(
            Expression<Func<TRow, Collection<TCollectionItem>>> propertyCollection,
            int headerRow,
            string startingColumn,
            Action<IColumnToCollectionConfiguration<TCollectionItem>> configurePropertiesAction) where TCollectionItem : class, new()
        {
            if (propertyCollection == null)
            {
                throw new ArgumentNullException(nameof(propertyCollection));
            }

            if (configurePropertiesAction == null)
            {
                throw new ArgumentNullException(nameof(configurePropertiesAction));
            }

            ValidateColumn(startingColumn, nameof(startingColumn));

            propertyCollection.ValidatePropertyExpressionType<TRow, TCollectionItem, Collection<TCollectionItem>>();

            var columnToCollectionConfiguration = new ColumnToCollectionConfiguration<TCollectionItem>();
            configurePropertiesAction(columnToCollectionConfiguration);

            var columnToCollectionDataExtractor =
                new NewableColumnToCollectionDataExtractor<TRow, Collection<TCollectionItem>, TCollectionItem>(propertyCollection,
                                                                                                               headerRow,
                                                                                                               startingColumn,
                                                                                                               columnToCollectionConfiguration);
            _collectionColumnSetters.Add(columnToCollectionDataExtractor);

            return this;
        }

        public ICollectionPropertyConfigurationWithoutColumnsToCollection<TRow> WithCollectionProperty<TCollectionItem>(
            Func<TRow, ICollection<TCollectionItem>> collectionGetter,
            int headerRow,
            string startingColumn,
            Action<IColumnToCollectionConfiguration<TCollectionItem>> configurePropertiesAction) where TCollectionItem : class, new()
        {
            return WithInitializedCollectionProperty(collectionGetter, headerRow, startingColumn, configurePropertiesAction);
        }

        public ICollectionPropertyConfigurationWithoutColumnsToCollection<TRow> WithInitializedCollectionProperty<TCollectionItem>(
            Func<TRow, ICollection<TCollectionItem>> collectionGetter,
            int headerRow,
            string startingColumn,
            Action<IColumnToCollectionConfiguration<TCollectionItem>> configurePropertiesAction) where TCollectionItem : class, new()
        {
            if (collectionGetter == null)
            {
                throw new ArgumentNullException(nameof(collectionGetter));
            }

            if (configurePropertiesAction == null)
            {
                throw new ArgumentNullException(nameof(configurePropertiesAction));
            }

            ValidateColumn(startingColumn, nameof(startingColumn));

            var columnToCollectionConfiguration = new ColumnToCollectionConfiguration<TCollectionItem>();
            configurePropertiesAction(columnToCollectionConfiguration);

            var columnToCollectionDataExtractor =
                new ColumnToCollectionDataExtractor<TRow, ICollection<TCollectionItem>, TCollectionItem>(collectionGetter,
                                                                                                         headerRow,
                                                                                                         startingColumn,
                                                                                                         columnToCollectionConfiguration);
            _collectionColumnSetters.Add(columnToCollectionDataExtractor);

            return this;
        }

        /// <summary>
        ///     Sets the row which will be used to find headers.
        /// </summary>
        /// <param name="row">The row which will be used to find headers (1-based; default value is 1)</param>
        public ICollectionPropertyConfiguration<TRow> WithHeaderRow(int row)
        {
            _headerRow = row;
            return this;
        }

        /// <summary>
        ///     Maps a property from the type defined as the row model
        ///     to the column identifier that has its value.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="propertyExpression">Expression for the property to be mapped.</param>
        /// <param name="columnHeader">
        ///     Header of the column that contains the value to be mapped to
        ///     the property defined by <paramref name="propertyExpression" />.
        /// </param>
        /// <param name="validateCellValue">
        ///     Optional callback that gets executed before retrieving the cell value casted to <typeparamref name="TValue" />.
        ///     The first parameter contains the cell address and a method that can abort the entire execution.
        ///     The second one the value of the cell.
        /// </param>
        /// <param name="validateCastedCellValue">
        ///     Optional callback that gets executed after retrieving the cell value casted to <typeparamref name="TValue" />.
        ///     The first parameter contains the cell address and a method that can abort the entire execution.
        ///     The second one the value of the cell.
        /// </param>
        /// <returns></returns>
        public ICollectionPropertyConfiguration<TRow> WithPropertyHeader<TValue>(Expression<Func<TRow, TValue>> propertyExpression,
                                                                                 string columnHeader,
                                                                                 Action<PropertyExtractionContext, object> validateCellValue = null,
                                                                                 Action<PropertyExtractionContext, TValue> validateCastedCellValue = null)
        {
            return WithPropertyHeader(propertyExpression, columnHeader, null, validateCellValue, validateCastedCellValue);
        }

        /// <summary>
        ///     Maps a property from the type defined as the row model
        ///     to the column identifier that has its value.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="propertyExpression">Expression for the property to be mapped.</param>
        /// <param name="columnHeader">
        ///     Header of the column that contains the value to be mapped to
        ///     the property defined by <paramref name="propertyExpression" />.
        /// </param>
        /// <param name="convertDataFunc">
        ///     Function that can be used to convert the cell value, which is an object
        ///     to the desirable <typeparamref name="TValue" />.
        /// </param>
        /// <param name="setPropertyValueCallback">
        ///     Optional callback that gets executed prior to the <paramref name="convertDataFunc" />.
        ///     The first parameter contains the cell address and a method that can abort the entire execution.
        ///     The second one the value of the cell.
        /// </param>
        /// <param name="setPropertyCastedValueCallback">
        ///     Optional callback that gets executed after the <paramref name="convertDataFunc" />.
        ///     The first parameter contains the cell address and a method that can abort the entire execution.
        ///     The second one the value of the cell.
        /// </param>
        /// <returns></returns>
        public ICollectionPropertyConfiguration<TRow> WithPropertyHeader<TValue>(Expression<Func<TRow, TValue>> propertyExpression,
                                                                                 string columnHeader,
                                                                                 Func<object, TValue> convertDataFunc,
                                                                                 Action<PropertyExtractionContext, object> setPropertyValueCallback = null,
                                                                                 Action<PropertyExtractionContext, TValue> setPropertyCastedValueCallback = null)
        {
            if (string.IsNullOrWhiteSpace(columnHeader))
            {
                throw new ArgumentNullException(nameof(columnHeader));
            }

            string column = FindColumnByHeader(columnHeader);

            _propertySetters.Add(new ColumnDataExtractor<TRow, TValue>(column, propertyExpression, convertDataFunc, setPropertyValueCallback, setPropertyCastedValueCallback));

            return this;
        }

        private static void ValidateColumn(string column, string propertyName)
        {
            var argumentName = propertyName ?? nameof(column);
            if (string.IsNullOrWhiteSpace(column))
            {
                throw new ArgumentException(@"The column value must be a valid non empty string containing letters.", argumentName);
            }

            if (!DataExtractor.ColumnRegex.IsMatch(column))
            {
                throw new ArgumentException(@"The column value must contain only letters.", argumentName);
            }
        }

        private string FindColumnByHeader(string columnHeader)
        {
            var lastColumn = _worksheet.Dimension.End.Column;
            for (int col = 1; col <= lastColumn; ++col)
            {
                var cell = _worksheet.Cells[_headerRow, col];
                if (cell.Text == columnHeader)
                {
                    return Regex.Replace(cell.Start.Address, @"\d*", ""); //turn "C1" into "C"
                }
            }

            throw new ArgumentException($@"Column header not found: {columnHeader}", nameof(columnHeader));
        }
    }
}