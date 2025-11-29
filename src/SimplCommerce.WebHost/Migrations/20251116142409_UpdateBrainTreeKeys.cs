using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplCommerce.WebHost.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBrainTreeKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Payments_PaymentProvider",
                keyColumn: "Id",
                keyValue: "Braintree",
                column: "AdditionalSettings",
                value: "{\"PublicKey\": \"4h4qm7wk6kj37gjb\", \"PrivateKey\" : \"3705664b326d96019136b14b88742b09\", \"MerchantId\" : \"6yd3q4z7r983yxrr\", \"IsProduction\" : \"false\"}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Payments_PaymentProvider",
                keyColumn: "Id",
                keyValue: "Braintree",
                column: "AdditionalSettings",
                value: "{\"PublicKey\": \"6j4d7qspt5n48kx4\", \"PrivateKey\" : \"bd1c26e53a6d811243fcc3eb268113e1\", \"MerchantId\" : \"ncsh7wwqvzs3cx9q\", \"IsProduction\" : \"false\"}");
        }
    }
}
