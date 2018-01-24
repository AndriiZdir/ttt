using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TicTacToe.DAL.Interfaces;

namespace TicTacToe.DAL.Models
{
    public class GameRoom : IEntity, IEntityStamp
    {
        public int Id { get; set; }
        public Guid RoomGuid { get; set; }
        public GameRoomState State { get; set; }

        #region GameSettings
        public int FragLimit { get; set; }
        public int MaxPlayers { get; set; }
        public string Password { get; set; }
        public int MinesQuantity { get; set; }
        public bool IsHidden { get; set; }
        #endregion

        public DateTime CreateDate { get; set; }
        public DateTime LastEditDate { get; set; }
        public string CreateUser { get; set; }
        public string LastEditUser { get; set; }

        [InverseProperty("GameRoom")]
        public virtual ICollection<GameRoomPlayer> GameRoomPlayers { get; set; }

        [ForeignKey("CreateUser")]        
        public virtual Player CreatedBy { get; set; }

        [ForeignKey("LastEditUser")]
        public virtual Player LastEditedBy { get; set; }
    }

    public enum GameRoomState
    {
        New,
        Started,
        Closed
    }
}
