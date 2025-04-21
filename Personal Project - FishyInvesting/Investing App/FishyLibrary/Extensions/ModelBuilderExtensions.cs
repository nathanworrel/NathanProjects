using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FishyLibrary;

public static class ModelBuilderExtensions
{
    /*
     * Used to convert database entities from camel case to snake case
     */
    public static ModelBuilder ToSnakeCase(
        this ModelBuilder builder,
        Func<IMutableEntityType, bool> entitySelector = null)
    {
        entitySelector ??= e => true;

        foreach (var entity in builder.Model.GetEntityTypes())
        {
            if (entitySelector(entity))
            {
                // skip views
                if (!entity.IsKeyless)
                {
                    entity.SetTableName(entity.DisplayName().ToSnakeCase());
                }
            }

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.ToSnakeCase());
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName().ToSnakeCase());
            }

            foreach (var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
            }

            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(index.GetDatabaseName().ToSnakeCase());
            }
        }

        return builder;
    }
   
}