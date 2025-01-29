using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;

namespace MicroDotNet.Packages.Orm.Tools
{
    public static class TypeMappingProvider
    {
        private static readonly ReadOnlyDictionary<string, string> TypeConversions =
            new ReadOnlyDictionary<string, string>(
                new Dictionary<string, string>()
                {
                    { "string", "dataRecord.GetString(Ordinals[\"{0}\"])" },
                    { "int", "dataRecord.GetInt32(Ordinals[\"{0}\"])" },
                });

        private static readonly ReadOnlyDictionary<string, DbType> DbTypeMappings =
            new ReadOnlyDictionary<string, DbType>(
                new Dictionary<string, DbType>()
                {
                    { "string", DbType.String },
                    { "int", DbType.Int32 },
                });
        
        public static string CreateMappingToResult(
            string parameterName,
            string parameterType)
        {
            if (!TypeConversions.TryGetValue(parameterType, out var typeConversionTemplate))
            {
                return string.Empty;
            }

            var typeConversion = string.Format(
                CultureInfo.InvariantCulture,
                typeConversionTemplate,
                parameterName);
            return $"var {parameterName}_value = {typeConversion};";
        }

        public static string MapToDbType(string typeName)
        {
            if (!DbTypeMappings.TryGetValue(typeName, out var result))
            {
                return "null";
            }
            
            return string.Format(
                CultureInfo.InvariantCulture,
                $"DbType.{result}");
        }
    }
}