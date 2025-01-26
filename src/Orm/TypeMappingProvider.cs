using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MicroDotNet.Packages.Orm
{
    public static class TypeMappingProvider
    {
        private static readonly ReadOnlyDictionary<string, string> TypeConversions = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>()
            {
                { "string", "dataRecord.GetString(Ordinals[\"{0}\"])" },
                { "int", "dataRecord.GetInt32(Ordinals[\"{0}\"])" },
            });
        
        public static string CreateMapping(
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
    }
}