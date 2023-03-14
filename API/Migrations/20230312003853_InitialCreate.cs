using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conctractors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conctractors", x => x.Id);
                    table.CheckConstraint("CH_Email_Contractor", "Email like '%@%.%'");
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.CheckConstraint("CH_Salary_Post", "Salary > 0");
                });

            migrationBuilder.CreateTable(
                name: "TypesEquipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypesEquipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Lastname = table.Column<string>(name: "Last_name", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Firstname = table.Column<string>(name: "First_name", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Otch = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PassportSeries = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    PassportNumber = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sum = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.CheckConstraint("CH_Sum_Order", "Sum > 0");
                    table.ForeignKey(
                        name: "FK_Orders_Conctractors_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "Conctractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicalTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeEquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 6000, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NameEquipment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicalTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicalTasks_TypesEquipment_TypeEquipmentId",
                        column: x => x.TypeEquipmentId,
                        principalTable: "TypesEquipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Login = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.UserId);
                    table.CheckConstraint("CH_Email_Account", "Email like '%@%.%'");
                    table.ForeignKey(
                        name: "FK_Accounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Share = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPosts", x => x.Id);
                    table.CheckConstraint("Ch_Share_UserPost", "Share > 0 AND Share <= 1");
                    table.ForeignKey(
                        name: "FK_UserPosts_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPosts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TechnicalTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EquipmentCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipments_TechnicalTasks_TechnicalTaskId",
                        column: x => x.TechnicalTaskId,
                        principalTable: "TechnicalTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AccountUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Accounts_AccountUserId",
                        column: x => x.AccountUserId,
                        principalTable: "Accounts",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenStr = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Accounts_AccountUserId",
                        column: x => x.AccountUserId,
                        principalTable: "Accounts",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    WorkContent = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Sum = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.CheckConstraint("CH_Sum_Service", "Sum > 0");
                    table.ForeignKey(
                        name: "FK_Services_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TechnicalTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpectedConclusion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FactCoclusion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TestData = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TestPriority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Passed = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicalTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicalTests_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TechnicalTests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "First_name", "Last_name", "Otch", "PassportNumber", "PassportSeries", "PhoneNumber" },
                values: new object[] { new Guid("f0e290a9-9054-4ae7-af3b-08dad84feb5b"), new DateTime(1993, 3, 12, 0, 0, 0, 0, DateTimeKind.Local), "Админ", "Админ", null, "000000", "0000", "88888888888" });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "UserId", "Email", "Login", "Password" },
                values: new object[] { new Guid("f0e290a9-9054-4ae7-af3b-08dad84feb5b"), "admin@admin.com", "admin", "$2a$11$vjrBjiSmnpGL5KZVQSm.JeoVipSDpV8h9yQcG6Y74CNlnvCBSC93y" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "AccountUserId", "Name" },
                values: new object[] { new Guid("cf75871a-30a6-4cac-9b50-c3c9830d58a0"), new Guid("f0e290a9-9054-4ae7-af3b-08dad84feb5b"), "Администратор" });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Email",
                table: "Accounts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Login",
                table: "Accounts",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conctractors_Email",
                table: "Conctractors",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conctractors_Name",
                table: "Conctractors",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conctractors_PhoneNumber",
                table: "Conctractors",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentCode",
                table: "Equipments",
                column: "EquipmentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_OrderId",
                table: "Equipments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_TechnicalTaskId",
                table: "Equipments",
                column: "TechnicalTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ContractorId",
                table: "Orders",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Name",
                table: "Posts",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_AccountUserId",
                table: "Roles",
                column: "AccountUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_EquipmentId",
                table: "Services",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalTasks_NameEquipment",
                table: "TechnicalTasks",
                column: "NameEquipment",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalTasks_TypeEquipmentId",
                table: "TechnicalTasks",
                column: "TypeEquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalTests_EquipmentId",
                table: "TechnicalTests",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalTests_UserId",
                table: "TechnicalTests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_AccountUserId",
                table: "Tokens",
                column: "AccountUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_TokenStr",
                table: "Tokens",
                column: "TokenStr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypesEquipment_Name",
                table: "TypesEquipment",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPosts_PostId",
                table: "UserPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPosts_UserId",
                table: "UserPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PassportNumber_PassportSeries",
                table: "Users",
                columns: new[] { "PassportNumber", "PassportSeries" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "TechnicalTests");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "UserPosts");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "TechnicalTasks");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Conctractors");

            migrationBuilder.DropTable(
                name: "TypesEquipment");
        }
    }
}
