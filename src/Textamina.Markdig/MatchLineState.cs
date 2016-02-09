namespace Textamina.Markdig
{
    public enum MatchLineState
    {
        None,

        Continue,

        Break,

        BreakAndKeepCurrent,

        BreakAndKeepOnlyIfEof,

        Discard,
    }
}