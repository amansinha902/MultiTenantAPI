using System.Collections.ObjectModel;

namespace Infrastructure.Constants
{
    public static class SchoolConstants
    {
        public const string Read = nameof(Read);
        public const string Create = nameof(Create);
        public const string Update = nameof(Update);
        public const string Delete = nameof(Delete);
        public const string UpgradeSubscription = nameof(UpgradeSubscription);
    }
    public static class SchoolFeature
    {
        public const string Tenants = nameof(Tenants);
        public const string Users = nameof(Users);
        public const string Roles = nameof(Roles);
        public const string UserRoles = nameof(UserRoles);
        public const string RoleClaims = nameof(RoleClaims);
        public const string Schools = nameof(Schools);
    }
    public record SchoolPermission(string Action, string Feature, string Description, string Group ,bool IsBasic = false, bool IsRoot = false)
    {
        public string Name => NameFor(Action, Feature);
        public static string NameFor(string action, string feature) => $"Permission.{feature}.{action}";
    }
    public static class SchoolPermissions
    {
        private static readonly SchoolPermission[] _allPermissions =
              [
                  new SchoolPermission(SchoolConstants.Read, SchoolFeature.Tenants, "Read Tenants", "Tenancy",IsRoot: true),
                  new SchoolPermission(SchoolConstants.Create, SchoolFeature.Tenants, "Create Tenants","Tenancy", IsRoot: true),
                  new SchoolPermission(SchoolConstants.Update, SchoolFeature.Tenants, "Update Tenants", "Tenancy",IsRoot: true),
                  new SchoolPermission(SchoolConstants.UpgradeSubscription, SchoolFeature.Tenants, "Upgrade Tenants Subscription","Tenancy", IsRoot: true),

                  new SchoolPermission(SchoolConstants.Read, SchoolFeature.Users, "Read Users","SystemAccess"),
                  new SchoolPermission(SchoolConstants.Create, SchoolFeature.Users, "Create Users","SystemAccess"),
                  new SchoolPermission(SchoolConstants.Update, SchoolFeature.Users, "Update Users","SystemAccess"),
                  new SchoolPermission(SchoolConstants.Delete, SchoolFeature.Users, "Delete Users","SystemAccess"),

                  new SchoolPermission(SchoolConstants.Read, SchoolFeature.UserRoles, "Read UserRoles","SystemAccess"),
                  new SchoolPermission(SchoolConstants.Update, SchoolFeature.UserRoles, "Update UserRoles","SystemAccess"),

                  new SchoolPermission(SchoolConstants.Create, SchoolFeature.Roles, "Create Roles","SystemAccess"),
                  new SchoolPermission(SchoolConstants.Read, SchoolFeature.Roles, "Read Roles","SystemAccess"),
                  new SchoolPermission(SchoolConstants.Update, SchoolFeature.Roles, "Update Roles","SystemAccess"),
                  new SchoolPermission(SchoolConstants.Delete, SchoolFeature.Roles, "Delete Roles","SystemAccess"),

                  new SchoolPermission(SchoolConstants.Read, SchoolFeature.RoleClaims, "Read RoleClaims","SystemAccess"),
                  new SchoolPermission(SchoolConstants.Update, SchoolFeature.RoleClaims, "Update RoleClaims","SystemAccess"),

                  new SchoolPermission(SchoolConstants.Read, SchoolFeature.Schools, "Read Schools","Academics", IsBasic: true),
                  new SchoolPermission(SchoolConstants.Create, SchoolFeature.Schools, "Create Schools","Academics"),
                  new SchoolPermission(SchoolConstants.Update, SchoolFeature.Schools, "Update Schools","Academics"),
                  new SchoolPermission(SchoolConstants.Delete, SchoolFeature.Schools, "Delete Schools","Academics"),
              ];
        public static IReadOnlyList<SchoolPermission> All { get; } =
            new ReadOnlyCollection<SchoolPermission>(_allPermissions);
        public static IReadOnlyList<SchoolPermission> Root { get; } =
            new ReadOnlyCollection<SchoolPermission>(_allPermissions.Where(p => p.IsRoot).ToArray());
        public static IReadOnlyList<SchoolPermission> Admin { get; } =
            new ReadOnlyCollection<SchoolPermission>(_allPermissions.Where(p => !p.IsRoot).ToArray());
        public static IReadOnlyList<SchoolPermission> Basic { get; } =
            new ReadOnlyCollection<SchoolPermission>(_allPermissions.Where(p => p.IsBasic).ToArray());
    }
}
