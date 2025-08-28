namespace Palantir.vNext.Core.Model;

public class Conversation
{
    public IEnumerable<ConversationTurn> Turns { get; set; } = [];
}
