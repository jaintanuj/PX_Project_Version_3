using Microsoft.EntityFrameworkCore.Migrations;

namespace PX_Project_Version_3.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppCondition",
                columns: table => new
                {
                    AppConditionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminID = table.Column<int>(nullable: false),
                    AdminName = table.Column<string>(nullable: true),
                    EventID = table.Column<int>(nullable: false),
                    ThemeID = table.Column<int>(nullable: false),
                    VotesAllowed = table.Column<int>(nullable: false),
                    MemberPerTeam = table.Column<int>(nullable: false),
                    AppStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCondition", x => x.AppConditionId);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    EventId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventCode = table.Column<string>(nullable: false),
                    EventName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "Judge",
                columns: table => new
                {
                    JudgeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    EventID = table.Column<int>(nullable: false),
                    ThemeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Judge", x => x.JudgeId);
                });

            migrationBuilder.CreateTable(
                name: "JudgeWinner",
                columns: table => new
                {
                    JudgeWinnerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    TeamID = table.Column<int>(nullable: false),
                    TeamName = table.Column<string>(nullable: true),
                    EventID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JudgeWinner", x => x.JudgeWinnerId);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    MemberId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    TeamID = table.Column<int>(nullable: false),
                    EventID = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.MemberId);
                });

            migrationBuilder.CreateTable(
                name: "PeopleWinner",
                columns: table => new
                {
                    PeopleWinnerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    TeamID = table.Column<int>(nullable: false),
                    TeamName = table.Column<string>(nullable: true),
                    EventID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeopleWinner", x => x.PeopleWinnerId);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    TeamId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamName = table.Column<string>(nullable: false),
                    ProjectName = table.Column<string>(nullable: true),
                    Idea = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    EventID = table.Column<int>(nullable: false),
                    JoinCode = table.Column<string>(nullable: true),
                    ThemeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.TeamId);
                });

            migrationBuilder.CreateTable(
                name: "TeamPresentation",
                columns: table => new
                {
                    TeamPresentationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamID = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    TeamName = table.Column<string>(nullable: true),
                    EventID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPresentation", x => x.TeamPresentationId);
                });

            migrationBuilder.CreateTable(
                name: "Theme",
                columns: table => new
                {
                    ThemeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThemeName = table.Column<string>(nullable: true),
                    ThemeType = table.Column<string>(nullable: true),
                    EventID = table.Column<int>(nullable: false),
                    EventName = table.Column<string>(nullable: true),
                    EventCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Theme", x => x.ThemeId);
                });

            migrationBuilder.CreateTable(
                name: "TieBreaker",
                columns: table => new
                {
                    TieBreakerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    EventID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TieBreaker", x => x.TieBreakerId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Vote",
                columns: table => new
                {
                    VoteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    TeamID = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    TeamName = table.Column<string>(nullable: true),
                    EventID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vote", x => x.VoteId);
                });

            migrationBuilder.CreateTable(
                name: "VoteCount",
                columns: table => new
                {
                    VoteCountId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamID = table.Column<int>(nullable: false),
                    TeamName = table.Column<string>(nullable: true),
                    EventID = table.Column<int>(nullable: false),
                    TotalCounts = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteCount", x => x.VoteCountId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCondition");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Judge");

            migrationBuilder.DropTable(
                name: "JudgeWinner");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "PeopleWinner");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "TeamPresentation");

            migrationBuilder.DropTable(
                name: "Theme");

            migrationBuilder.DropTable(
                name: "TieBreaker");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Vote");

            migrationBuilder.DropTable(
                name: "VoteCount");
        }
    }
}
