using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Helpers
{
    public static class UtcDateAnnotation
    {
        private const string IsUtcAnnotation = "IsUtc";
        private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
          new(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableConverter =
          new(v => v, v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

        //Mark a property as not UTC: builder.Property(x => x.DateField).IsUtc(false);
        public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, bool isUtc = true) =>
          builder.HasAnnotation(IsUtcAnnotation, isUtc);

        public static bool IsUtc(this IMutableProperty property) =>
          ((bool?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true;


        /// <summary>
        /// Make sure this is called after configuring all your entities.
        /// </summary>
        public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
                foreach (var property in entityType.GetProperties())
                {
                    if (!property.IsUtc())
                        continue;

                    if (property.ClrType == typeof(DateTime))
                        property.SetValueConverter(UtcConverter);

                    else if (property.ClrType == typeof(DateTime?))
                        property.SetValueConverter(UtcNullableConverter);
                }
        }
    }
}