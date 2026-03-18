using System;

namespace Miner.Diagnostics
{
    [Serializable]
    public enum DiagChannel
    {
        General,
        Input,
        State,
        Ladder,
        Motor,
        Physics,
        UI,
        Save,
        Unity,
        Error
    }
}
