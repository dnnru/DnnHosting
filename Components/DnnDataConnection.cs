#region

using Italliance.Modules.DnnHosting.Models;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;

#endregion

namespace Italliance.Modules.DnnHosting.Components
{
    public class DnnDataConnection : DataConnection
    {
        public DnnDataConnection(string connStr, string databaseOwner, string objectQualifier) : base(SqlServerTools.GetDataProvider(SqlServerVersion.v2012), connStr)
        {
            string qualifier = "";
            if (!string.IsNullOrWhiteSpace(objectQualifier))
            {
                qualifier = objectQualifier.EndsWith("_") ? objectQualifier : $"{objectQualifier}_";
            }
            MappingSchema.EntityDescriptorCreatedCallback = (ms, ed) =>
                                                            {
                                                                ed.SchemaName = databaseOwner;
                                                                ed.TableName = $"{qualifier}{ed.TableName}";
                                                            };
        }

        public ITable<Portal> Portals => GetTable<Portal>();
        public ITable<PortalAlias> PortalAliases => GetTable<PortalAlias>();
    }
}