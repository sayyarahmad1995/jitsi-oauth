using System.Text.Json;
using jitsi_oauth.Interfaces;

namespace jitsi_oauth.Services;

public class ClaimMapper : IClaimMapper
{
   public T Map<T>(JsonDocument token) where T : new()
   {
      var root = token.RootElement;
      var instance = new T();

      var jsonProps = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
      foreach (var jsonProp in root.EnumerateObject())
      {
         jsonProps[jsonProp.Name] = jsonProp.Value;
      }

      foreach (var prop in typeof(T).GetProperties())
      {
         if (!prop.CanWrite)
            continue;

         if (jsonProps.TryGetValue(prop.Name, out var value))
         {
            if (value.ValueKind == JsonValueKind.String)
               prop.SetValue(instance, value.GetString());
            else if (value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False)
               prop.SetValue(instance, value.GetBoolean());
            else if (prop.PropertyType == typeof(int) && value.ValueKind == JsonValueKind.Number)
               prop.SetValue(instance, value.GetInt32());
         }
      }

      return instance;
   }
}
