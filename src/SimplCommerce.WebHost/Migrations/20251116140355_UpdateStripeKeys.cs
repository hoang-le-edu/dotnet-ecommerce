using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplCommerce.WebHost.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStripeKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Payments_PaymentProvider",
                keyColumn: "Id",
                keyValue: "Stripe",
                column: "AdditionalSettings",
                value: "{\"PublicKey\": \"pk_test_51ST9BaDoMT0r6QgLa2KnlwwjUNKwarwFcM5WS61YsYlwYGxiYlcySAnwvLfJnn8aqh7uhQk3pHkRRpHEZ9QtLBJr00La9o0tjN\", \"PrivateKey\" : \"sk_test_51ST9BaDoMT0r6QgLtmAE7MGYxUdruKzDnpo3rDoHf7fLwKMqUtE3hCT3FlbAKp7xi6gMaQAIclpvdP1FzxTb0fn80025pI7jka\"}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Payments_PaymentProvider",
                keyColumn: "Id",
                keyValue: "Stripe",
                column: "AdditionalSettings",
                value: "{\"PublicKey\": \"YOUR_STRIPE_PUBLIC_KEY\", \"PrivateKey\" : \"YOUR_STRIPE_SECRET_KEY\"}");
        }
    }
}
