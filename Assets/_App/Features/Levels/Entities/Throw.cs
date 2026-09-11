using System.Collections.Generic;
using DigitalLove.Game.BankShot;

namespace DigitalLove.Game
{
    public class Throw
    {
        private readonly int basePoints;
        private readonly List<ThrowScoreOp> ops = new();

        public Throw(int basePoints = 2)
        {
            this.basePoints = basePoints;
        }

        public int ResolvedPoints
        {
            get
            {
                float points = basePoints;
                for (int i = 0; i < ops.Count; i++)
                {
                    ThrowScoreOp op = ops[i];
                    if (op.Kind == ThrowScoreOpKind.Multiply)
                    {
                        if (op.Value > 0f)
                            points *= op.Value;
                    }
                    else
                        points += op.Value;
                }

                return (int)points;
            }
        }

        public void ApplyMultiplier(float value)
        {
            if (value <= 0f)
                return;

            ops.Add(new ThrowScoreOp(ThrowScoreOpKind.Multiply, value));
        }

        public void ApplyFlat(float value)
        {
            ops.Add(new ThrowScoreOp(ThrowScoreOpKind.FlatAdd, value));
        }

        public void ApplyOps(IReadOnlyList<ThrowScoreOp> scoreOps)
        {
            if (scoreOps == null)
                return;

            for (int i = 0; i < scoreOps.Count; i++)
            {
                ThrowScoreOp op = scoreOps[i];
                if (op.Kind == ThrowScoreOpKind.Multiply)
                    ApplyMultiplier(op.Value);
                else
                    ApplyFlat(op.Value);
            }
        }
    }
}
