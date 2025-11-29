using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplCommerce.WebHost.Migrations
{
    /// <inheritdoc />
    public partial class AddBraintreeSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Payments_PaymentProvider",
                keyColumn: "Id",
                keyValue: "Braintree",
                column: "AdditionalSettings",
                value: "{\"PublicKey\": \"zd8x6q6w3v4prkjc\", \"PrivateKey\" : \"d1ce3dadeead4d9a5e191b6311ecbec3\", \"MerchantId\" : \"yxz35b6t57m27f25\", \"IsProduction\" : \"false\"}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Payments_PaymentProvider",
                keyColumn: "Id",
                keyValue: "Braintree",
                column: "AdditionalSettings",
                value: "{\"PublicKey\": \"4h4qm7wk6kj37gjb\", \"PrivateKey\" : \"3705664b326d96019136b14b88742b09\", \"MerchantId\" : \"6yd3q4z7r983yxrr\", \"IsProduction\" : \"false\"}");
        }
    }
}
