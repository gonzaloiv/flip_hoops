namespace DigitalLove.Game.BankShot
{
    public enum ThrowScoreOpKind
    {
        Multiply = 0,
        FlatAdd = 1
    }

    public readonly struct ThrowScoreOp
    {
        public readonly ThrowScoreOpKind Kind;
        public readonly float Value;

        public ThrowScoreOp(ThrowScoreOpKind kind, float value)
        {
            Kind = kind;
            Value = value;
        }
    }
}
