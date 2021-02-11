using OfficeOpenXml;

namespace Italliance.Modules.DnnHosting.Components.ExcelDataExtractor.DataExtractors.CollectionColumn
{
    internal interface ISimpleCollectionColumnDataExtractor<TRow>
        where TRow : class, new()
    {
        void SetPropertyValue(TRow dataInstance, int row, ExcelRange cellRange);
    }
}
