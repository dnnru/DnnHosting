#region

using System;
using System.Linq.Expressions;
using Italliance.Modules.DnnHosting.Components.ExcelDataExtractor.Data;
using OfficeOpenXml;

#endregion

namespace Italliance.Modules.DnnHosting.Components.ExcelDataExtractor
{
    internal abstract class PropertyValueSetter<TModel, TValue> where TModel : class, new()
    {
        private readonly Func<object, TValue> _cellValueConverter;
        private readonly Action<TModel, TValue> _setPropertyValueAction;
        private readonly Action<PropertyExtractionContext, TValue> _validateCastedValue;
        private readonly Action<PropertyExtractionContext, object> _validateValue;

        protected PropertyValueSetter(Expression<Func<TModel, TValue>> propertyExpression,
                                      Func<object, TValue> cellValueConverter,
                                      Action<PropertyExtractionContext, object> validateValue,
                                      Action<PropertyExtractionContext, TValue> validateCastedValue)
        {
            _setPropertyValueAction = propertyExpression.CreatePropertyValueSetterAction();
            _cellValueConverter = cellValueConverter;
            _validateValue = validateValue;
            _validateCastedValue = validateCastedValue;
        }

        /// <summary>
        ///     Sets the property value for the <paramref name="dataInstance" />.
        ///     This method also checks the validation actions, before and after casting the cell value,
        ///     if one of them aborts the execution, this method will return false and it will not set the
        ///     value for this property.
        /// </summary>
        /// <param name="dataInstance"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        protected bool SetPropertyValue(TModel dataInstance, ExcelRangeBase cell)
        {
            // This instance should be created only if there is at least one callback function defined.
            var context = _validateValue != null || _validateCastedValue != null ? new PropertyExtractionContext(new CellAddress(cell)) : null;

            if (_validateValue != null)
            {
                _validateValue(context, cell.Value);
                if (context != null && context.Aborted)
                {
                    return false;
                }
            }

            TValue value = _cellValueConverter == null ? cell.GetValue<TValue>() : _cellValueConverter(cell.Value);

            if (_validateCastedValue != null)
            {
                _validateCastedValue(context, value);
                if (context != null && context.Aborted)
                {
                    return false;
                }
            }

            _setPropertyValueAction(dataInstance, value);
            return true;
        }
    }
}