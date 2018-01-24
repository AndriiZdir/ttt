using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TicTacToe.API.Migrations
{
    public partial class AddTimeStampToGameRooms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "GameRooms",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreateUser",
                table: "GameRooms",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditDate",
                table: "GameRooms",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastEditUser",
                table: "GameRooms",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinesQuantity",
                table: "GameRooms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GameRooms_CreateUser",
                table: "GameRooms",
                column: "CreateUser");

            migrationBuilder.CreateIndex(
                name: "IX_GameRooms_LastEditUser",
                table: "GameRooms",
                column: "LastEditUser");

            migrationBuilder.AddForeignKey(
                name: "FK_GameRooms_AspNetUsers_CreateUser",
                table: "GameRooms",
                column: "CreateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameRooms_AspNetUsers_LastEditUser",
                table: "GameRooms",
                column: "LastEditUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameRooms_AspNetUsers_CreateUser",
                table: "GameRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_GameRooms_AspNetUsers_LastEditUser",
                table: "GameRooms");

            migrationBuilder.DropIndex(
                name: "IX_GameRooms_CreateUser",
                table: "GameRooms");

            migrationBuilder.DropIndex(
                name: "IX_GameRooms_LastEditUser",
                table: "GameRooms");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "GameRooms");

            migrationBuilder.DropColumn(
                name: "CreateUser",
                table: "GameRooms");

            migrationBuilder.DropColumn(
                name: "LastEditDate",
                table: "GameRooms");

            migrationBuilder.DropColumn(
                name: "LastEditUser",
                table: "GameRooms");

            migrationBuilder.DropColumn(
                name: "MinesQuantity",
                table: "GameRooms");
        }
    }
}
