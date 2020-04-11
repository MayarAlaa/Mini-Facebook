using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Facebook.Data.Migrations
{
    public partial class ModelsCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<int>(
                name: "BDay",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BMonth",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BYear",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FName",
                table: "AspNetUsers",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LName",
                table: "AspNetUsers",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetRoles",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetRoles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    PostId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Posts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserFriends",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    FriendId = table.Column<string>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFriends", x => new { x.UserId, x.FriendId });
                    table.ForeignKey(
                        name: "FK_UserFriends_AspNetUsers_FriendId",
                        column: x => x.FriendId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFriends_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    PostId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    PostId = table.Column<int>(nullable: false),
                    IsLiked = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => new { x.UserId, x.PostId });
                    table.ForeignKey(
                        name: "FK_Likes_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_PostId",
                table: "Likes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFriends_FriendId",
                table: "UserFriends",
                column: "FriendId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "UserFriends");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropColumn(
                name: "BDay",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BMonth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BYear",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetRoles");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
