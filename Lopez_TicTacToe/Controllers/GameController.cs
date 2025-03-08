using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Lopez_TicTacToe.Models;

namespace Lopez_TicTacToe.Controllers
{
    public class GameController : Controller
    {
        private static GameState gameState = new GameState();

        [HttpPost]
        public IActionResult MakeMove([FromBody] MoveRequest request)
        {
            int index = request.Index;
            string player = request.Player;

            int row = index / 3;
            int col = index % 3;

            if (!gameState.MakeMove(row, col))
                return Json(new { status = "invalid" });

            // Check if the player wins first
            if (!string.IsNullOrEmpty(gameState.Winner))
            {
                return Json(new { status = "win", winner = gameState.Winner });
            }

            int aiMoveIndex = -1;

            // AI plays if it's O's turn and game isn't over
            if (gameState.CurrentPlayer == "O" && gameState.Winner == "")
            {
                aiMoveIndex = gameState.MakeAIMove();
            }

            // If AI won, send response
            if (!string.IsNullOrEmpty(gameState.Winner))
            {
                return Json(new { status = "win", winner = gameState.Winner, aiMove = aiMoveIndex });
            }

            return Json(new { status = "next", aiMove = aiMoveIndex });
        }


        [HttpPost]
        public IActionResult ResetGame()
        {
            gameState.Reset();
            return Json(new { status = "reset" });
        }

        [HttpGet]
        public IActionResult GetStats()
        {
            return Json(new
            {
                xWins = gameState.XWins,
                oWins = gameState.OWins,
                draws = gameState.Draws
            });
        }

        public class MoveRequest
        {
            public int Index { get; set; }
            public string Player { get; set; }
        }
    }
}
