using Armali.Horizon.Messaging.Model;

namespace Palantir.Core.Model;

public class TestMessage : MessagePayload
{
    public string ModelQuery { get; set; } = "¿Cuáles son las principales ciudades de España por número de habitantes?";
}
