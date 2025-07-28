using Armali.Horizon.Messaging.Model;

namespace Palantir.Core.Model;

public class ConversationMessage: MessagePayload
{
    public IEnumerable<ConversationTurn> Turns { get; set; } = [];
}

public struct ConversationTurn
{
    public string Speaker { get; set; } // "Usuario" or "Interlocutor"
    public string Content { get; set; } // The message content
}
