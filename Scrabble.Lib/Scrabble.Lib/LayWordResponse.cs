namespace Scrabble.Lib
{
    public class LayWordResponse
    {
        private LayWordResponse()
        {
        }

        public bool ValidWord { get; private set; }
        public string Error { get; private set; }
        public int PlayerScore { get; set; }
        public Player Player { get; set; }

        internal static LayWordResponse CreateSuccessResponse(Player player, int playerScore)
        {
            return new LayWordResponse
            {
                ValidWord = true,
                PlayerScore = playerScore,
                Player = player
            };
        }

        internal static LayWordResponse CreateFailureResponse(Player player, string error)
        {
            return new LayWordResponse
            {
                ValidWord = false,
                Error = error,
                Player = player
            };
        }
    }
}
