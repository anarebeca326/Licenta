using Microsoft.EntityFrameworkCore.Migrations;

namespace persistence.Migrations
{
    public partial class First : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommunalCoef = table.Column<float>(type: "real", nullable: false),
                    AesthetiCoef = table.Column<float>(type: "real", nullable: false),
                    DarkCoef = table.Column<float>(type: "real", nullable: false),
                    ThrillingCoef = table.Column<float>(type: "real", nullable: false),
                    CerebralCoef = table.Column<float>(type: "real", nullable: false),
                    Communal = table.Column<int>(type: "int", nullable: false),
                    Aesthetic = table.Column<int>(type: "int", nullable: false),
                    Dark = table.Column<int>(type: "int", nullable: false),
                    Thrilling = table.Column<int>(type: "int", nullable: false),
                    Cerebral = table.Column<int>(type: "int", nullable: false),
                    NoRatings = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Openness = table.Column<float>(type: "real", nullable: false),
                    Conscientiousness = table.Column<float>(type: "real", nullable: false),
                    Extraversion = table.Column<float>(type: "real", nullable: false),
                    Agreeableness = table.Column<float>(type: "real", nullable: false),
                    Neuroticism = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Favourite",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    BookID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favourite", x => new { x.UserID, x.BookID });
                    table.ForeignKey(
                        name: "FK_Favourite_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favourite_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shelf",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    BookID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shelf", x => new { x.UserID, x.BookID });
                    table.ForeignKey(
                        name: "FK_Shelf_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shelf_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favourite_BookID",
                table: "Favourite",
                column: "BookID");

            migrationBuilder.CreateIndex(
                name: "IX_Shelf_BookID",
                table: "Shelf",
                column: "BookID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favourite");

            migrationBuilder.DropTable(
                name: "Shelf");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
