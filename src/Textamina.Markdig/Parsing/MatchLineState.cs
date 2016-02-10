namespace Textamina.Markdig.Parsing
{
    public enum MatchLineState
    {
        None,

        Discard,

        Continue,

        Break,

        BreakAndKeepCurrent,

        BreakAndKeepOnlyIfEof,

    }
}