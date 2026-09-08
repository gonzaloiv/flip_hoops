namespace DigitalLove.Game.Levels
{
    public struct LevelClearBests
    {
        public int Stars;
        public int Points;
        public bool HasValue;
    }

    public static class LevelClearBestsCodec
    {
        private const char Separator = '|';

        public static string Encode(int stars, int points) => $"{stars}{Separator}{points}";

        public static LevelClearBests ParseScoreMode(string metadata)
        {
            if (string.IsNullOrEmpty(metadata))
                return default;

            string[] parts = metadata.Split(Separator);
            if (parts.Length != 2)
                return default;

            if (!int.TryParse(parts[0], out int stars))
                return default;
            if (!int.TryParse(parts[1], out int points))
                return default;

            if (stars < 1 || points < 1)
                return default;

            return new LevelClearBests
            {
                Stars = stars,
                Points = points,
                HasValue = true
            };
        }

        public static LevelClearBests Merge(LevelClearBests stored, int runStars, int runPoints)
        {
            if (!stored.HasValue)
            {
                return new LevelClearBests
                {
                    Stars = runStars,
                    Points = runPoints,
                    HasValue = true
                };
            }

            return new LevelClearBests
            {
                Stars = runStars > stored.Stars ? runStars : stored.Stars,
                Points = runPoints > stored.Points ? runPoints : stored.Points,
                HasValue = true
            };
        }
    }
}
