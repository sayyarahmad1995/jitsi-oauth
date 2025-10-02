using System.Text.Json;

namespace jitsi_oauth.Interfaces;

public interface IClaimMapper
{
   public T Map<T>(JsonDocument token) where T : new();
}
