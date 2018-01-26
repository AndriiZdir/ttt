using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.BL.Models;

namespace TicTacToe.BL
{
    public class GameManager
    {
        private Dictionary<Guid, GameField> _gameFields;

        public GameManager()
        {
            _gameFields = new Dictionary<Guid, GameField>();
        }

        public GameField StartNewGame(Guid gameId, IEnumerable<string> players, int fieldSize = 40)
        {
            var gameField = new GameField(gameId, fieldSize);

            foreach (var playerId in players)
            {
                gameField.AddPlayerToField(playerId);
            }

            _gameFields.Add(gameField.GameId, gameField);

            if (gameField.State != GameFieldState.Ready) { throw new Exception("The game is not ready. Check player quantity."); }

            return gameField;
        }

        public SignPoint SetPoint(Guid gameId, string playerId, int x, int y)
        {
            var gameField = _gameFields[gameId];

            if (gameField.CurrentTurnPlayer.Id != playerId) { throw new Exception("It is not your turn to move."); }

            var signPoint = gameField.SetPointSign(x, y);

            return signPoint;
        }
    }
}
