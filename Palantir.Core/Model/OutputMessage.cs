using Armali.Horizon.Messaging.Model;

namespace Palantir.Core.Model;

public class OutputMessage : MessagePayload
{
    public string ModelResponse { get; set; } = "Unknown model response";
}
