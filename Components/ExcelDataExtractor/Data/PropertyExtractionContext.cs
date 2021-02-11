namespace Italliance.Modules.DnnHosting.Components.ExcelDataExtractor.Data
{
    public class PropertyExtractionContext
    {
        internal PropertyExtractionContext(CellAddress cellAddress)
        {
            CellAddress = cellAddress;
        }

        /// <summary>
        ///     Indicates whether or not the extraction should be aborted.
        /// </summary>
        internal bool Aborted { get; private set; }

        /// <summary>
        ///     Defines the cell address from where the value of the current property was extracted.
        /// </summary>
        public CellAddress CellAddress { get; }

        /// <summary>
        ///     Aborts the entire data extraction.
        ///     The entities for the rows already extracted will be returned.
        /// </summary>
        public void Abort()
        {
            Aborted = true;
        }
    }
}