using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplCommerce.WebHost.Migrations
{
    public partial class AddLargeSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQL to insert large data (approx 1GB)
            // 20,000 rows * ~50KB per row = ~1GB
            var sql = @"
                SET IDENTITY_INSERT [Catalog_Product] ON;
                
                DECLARE @i INT = 100000;
                DECLARE @max INT = 120000; -- 20,000 rows
                DECLARE @desc NVARCHAR(MAX);
                DECLARE @userId BIGINT;
                
                -- Get first user ID from database (fallback to 2 if none exists)
                SELECT TOP 1 @userId = Id FROM [Core_User] ORDER BY Id;
                IF @userId IS NULL SET @userId = 2;
                
                -- Generate ~50KB text (25000 chars * 2 bytes)
                -- 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. ' is 55 chars
                -- 55 * 455 ~= 25025 chars
                SET @desc = REPLICATE(CAST(N'Lorem ipsum dolor sit amet, consectetur adipiscing elit. ' AS NVARCHAR(MAX)), 455); 

                WHILE @i < @max
                BEGIN
                    INSERT INTO [Catalog_Product] (
                        [Id], [BrandId], [CreatedById], [CreatedOn], [Description], [DisplayOrder], 
                        [HasOptions], [IsAllowToOrder], [IsCallForPricing], [IsDeleted], [IsFeatured], 
                        [IsPublished], [IsVisibleIndividually], [LatestUpdatedById], [LatestUpdatedOn], 
                        [Name], [NormalizedName], [Price], [ReviewsCount], [Sku], [Slug], 
                        [StockQuantity], [StockTrackingIsEnabled], [TaxClassId], [ThumbnailImageId], [VendorId]
                    ) VALUES (
                        @i, 
                        NULL, -- BrandId (NULL - no brand)
                        @userId, -- CreatedById (from database)
                        GETDATE(), 
                        @desc, 
                        0, 
                        0, 1, 0, 0, 1, 
                        1, 1, @userId, GETDATE(), 
                        N'Large Data Product ' + CAST(@i AS NVARCHAR(20)), 
                        N'LARGE DATA PRODUCT ' + CAST(@i AS NVARCHAR(20)), 
                        1000000, 
                        0, 
                        N'SKU-' + CAST(@i AS NVARCHAR(20)), 
                        N'large-data-product-' + CAST(@i AS NVARCHAR(20)), 
                        100, 
                        1, 
                        NULL, NULL, NULL
                    );

                    SET @i = @i + 1;
                END

                SET IDENTITY_INSERT [Catalog_Product] OFF;
            ";

            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete the inserted products
            migrationBuilder.Sql(@"
                DELETE FROM [Catalog_Product] WHERE Id >= 100000 AND Id < 120000;
            ");
        }
    }
}
