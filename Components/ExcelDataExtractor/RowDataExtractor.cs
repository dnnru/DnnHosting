﻿#region

using System;
using System.Linq.Expressions;
using OfficeOpenXml;

#endregion

namespace Italliance.Modules.DnnHosting.Components.ExcelDataExtractor
{
    internal interface IRowDataExtractor<in TModel>
    {
        void SetPropertyValue(TModel dataInstance, ExcelRangeBase cellRange);
    }

    internal class RowDataExtractor<TModel, TValue> : PropertyValueSetter<TModel, TValue>, IRowDataExtractor<TModel> where TModel : class, new()
    {
        public RowDataExtractor(Expression<Func<TModel, TValue>> propertyExpression, Func<object, TValue> convertDataFunc = null) : base(propertyExpression,
         convertDataFunc,
         null,
         null)
        { }

        void IRowDataExtractor<TModel>.SetPropertyValue(TModel dataInstance, ExcelRangeBase cellRange)
        {
            base.SetPropertyValue(dataInstance, cellRange);
        }
    }
}