using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Lopez_TicTacToe.Models;

namespace Lopez_TicTacToe.Hubs
{
    public class GameHub : Hub
    {
        private static GameState gameState = new GameState();

        public async Task MakeMove(int index, string player)
        {
            int row = index / 3;
            int col = index % 3;

            bool moveSuccess = gameState.MakeMove(row, col);

            if (moveSuccess)
            {
                await Clients.All.SendAsync("ReceiveMove", index, player);

                string status = "next";
                if (!string.IsNullOrEmpty(gameState.Winner))
                {
                    if (gameState.Winner == "Draw")
                        status = "draw";
                    else
                        status = "win";

                    await Clients.All.SendAsync("GameOver", gameState.Winner);
                }
            }
        }

        public async Task ResetGame()
        {
            gameState.Reset();
            await Clients.All.SendAsync("ResetBoard");
        }

        public async Task GetStats()
        {
            await Clients.Caller.SendAsync("ReceiveStats", gameState.XWins, gameState.OWins, gameState.Draws);
        }
    }
}