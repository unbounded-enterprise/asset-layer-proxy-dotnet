using AssetLayer.SDK.Basic;
using Microsoft.Extensions.Primitives;
using System.Runtime.Serialization.Json;

namespace ProxyNetworking
{
    public class Utils
    {
        public static string GetRequiredHeaderValue(string key, IHeaderDictionary headers) {
            if (headers.TryGetValue(key, out StringValues values)) {
                if (StringValues.IsNullOrEmpty(values)) {
                    throw new BasicError($"Missing '{key}'", 400);
                }

                return values[0]!;
            }
            else {
                throw new BasicError($"Missing '{key}'", 400);
            }
        }

        public static string? GetOptionalHeaderValue(string key, IHeaderDictionary headers)
        {
            if (headers.TryGetValue(key, out StringValues values))
            {
                if (StringValues.IsNullOrEmpty(values)) { return null; }

                return values[0]!;
            }
            else { return null; }
        }

        public static T QueryCollectionToObject<T>(IQueryCollection query) where T : new()
        {
            var obj = new T();

            foreach (var property in typeof(T).GetProperties())
            {
                if (property.CanWrite)
                {
                    var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    bool isList = (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>));
                    bool isArr = targetType.IsArray;
                    StringValues values = default;

                    if (query.ContainsKey(property.Name))
                    {
                        values = query[property.Name];
                    }
                    else if ((isList || isArr) && query.ContainsKey(property.Name + "[]"))
                    {
                        values = query[property.Name + "[]"];
                    }

                    if (isList)
                    {
                        var elementType = targetType.GetGenericArguments()[0];
                        var listType = typeof(List<>).MakeGenericType(elementType);
                        var list = Activator.CreateInstance(listType);

                        var addMethod = listType.GetMethod("Add");
                        if (addMethod == null) { continue; }

                        foreach (var value in values)
                        {
                            var safeValue = Convert.ChangeType(value, elementType);
                            addMethod.Invoke(list, new[] { safeValue });
                        }

                        property.SetValue(obj, list);
                    }
                    else if (targetType.IsArray)
                    {
                        var elementType = targetType.GetElementType();
                        var array = Array.CreateInstance(elementType!, values.Count);

                        for (int i = 0; i < values.Count; i++)
                        {
                            var safeValue = Convert.ChangeType(values[i], elementType!);
                            array.SetValue(safeValue, i);
                        }

                        property.SetValue(obj, array);
                    }
                    else if (values.Count > 0)
                    {
                        var safeValue = Convert.ChangeType(values[0], targetType);
                        property.SetValue(obj, safeValue);
                    }
                    else if (targetType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) == null)
                    {
                        throw new BasicError($"Missing '{property.Name}'", 400);
                    }
                }
            }

            return obj;
        }

        public static async Task<T> GetBodyAsObjectA<T>(Stream body)
        {
            using var ms = new MemoryStream();
            await body.CopyToAsync(ms);
            ms.Position = 0;

            var serializer = new DataContractJsonSerializer(typeof(T), new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });

            if (serializer.ReadObject(ms) is T result)
            {
                return result;
            }
            else
            {
                throw new BasicError("Deserialization failed", 500);
            }
        }
    }
}
