using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.BL.Models;

namespace TicTacToe.BL
{
    public class GameManager
    {
        private readonly int _fieldSize = 200;
        private Dictionary<Guid, GameField> _gameFields;

        public GameManager()
        {
            _gameFields = new Dictionary<Guid, GameField>();
        }

        public GameField StartNewGame(IEnumerable<Guid> players)
        {
            var gameField = new GameField(_fieldSize);

            foreach(var playerId in players)
            {
                gameField.AddPlayerToField(playerId);
            }

            _gameFields.Add(gameField.GameId, gameField);

            return gameField;
        }        
    }
}
