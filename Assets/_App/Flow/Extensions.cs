using DigitalLove.Casual.Flow;
using DigitalLove.Casual.Levels;

namespace DigitalLove.Game
{
    public static class Extensions
    {
        public static string GetIdWithRound(this LevelData levelData, Play play) => $"{levelData.id}_{play.Tries}";

        public static int RoundLabelValue(this Play play) => play.Tries + 1;
    }
}