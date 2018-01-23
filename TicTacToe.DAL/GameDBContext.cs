using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.DAL.Models;

namespace TicTacToe.DAL
{
    public class GameDBContext : IdentityDbContext<IdentityUser>
    {
        public GameDBContext(DbContextOptions<GameDBContext> options) : base(options) { }

        public virtual DbSet<GameRoom> GameRooms { get; set; }
        public virtual DbSet<GameRoomPlayer> GameRoomPlayers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<GameRoomPlayer>(entity => { entity.HasKey(e => new { e.UserId, e.GameRoomId }); });

            base.OnModelCreating(builder);
        }
    }
}
