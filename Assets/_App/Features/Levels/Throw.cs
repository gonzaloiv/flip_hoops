namespace DigitalLove.Game
{
    public class Throw
    {
        private readonly int basePoints;
        private float multiplier = 1f;

        public Throw(int basePoints = 2)
        {
            this.basePoints = basePoints;
        }

        public int ResolvedPoints => (int)(basePoints * multiplier);

        public void ApplyMultiplier(float value)
        {
            if (value <= 0f)
                return;
            multiplier *= value;
        }
    }
}
