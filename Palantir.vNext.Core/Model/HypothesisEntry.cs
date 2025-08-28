namespace Palantir.vNext.Core.Model;

public class HypothesisEntry
{
    public HypothesisMood Mood { get; set; } = HypothesisMood.Neutral;
    public string Content { get; set; } = string.Empty;
    public float Probability { get; set; } = 0f;
}

public enum HypothesisMood
{
    Positive,
    Neutral,
    Negative
}
