using LinqToDB.Mapping;

// ReSharper disable InconsistentNaming

namespace Italliance.Modules.DnnHosting.Models
{
    [Table("PortalAlias")]
    public partial class PortalAlias
    {
        [PrimaryKey, Identity   ] public int       PortalAliasID        { get; set; } // int
        [Column,     NotNull    ] public int       PortalID             { get; set; } // int
        [Column,        Nullable] public string    HTTPAlias            { get; set; } // nvarchar(200)
        #region Associations

        /// <summary>
        /// FK_PortalAlias_Portals
        /// </summary>
        [Association(ThisKey="PortalID", OtherKey="PortalID", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_PortalAlias_Portals")]
        public Portal Portal { get; set; }

        #endregion
    }
}