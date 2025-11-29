using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplCommerce.WebHost.Migrations
{
    public partial class AddLargeSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert 2,000 rows for testing (reduced from 20,000 to avoid timeout)
            // Each batch = 200 rows, total 10 batches (~100MB total)
            // For full 1GB data, use azure-seed-data.sql script directly on Azure Portal
            int batchSize = 200;
            int totalRows = 2000;
            int startId = 100000;

            for (int batch = 0; batch < (totalRows / batchSize); batch++)
            {
                int batchStart = startId + (batch * batchSize);
                int batchEnd = batchStart + batchSize;

                var sql = $@"
                    SET IDENTITY_INSERT [Catalog_Product] ON;
                    
                    DECLARE @i INT = {batchStart};
                    DECLARE @max INT = {batchEnd};
                    DECLARE @desc NVARCHAR(MAX);
                    DECLARE @userId BIGINT;
                    
                    SELECT TOP 1 @userId = Id FROM [Core_User] ORDER BY Id;
                    IF @userId IS NULL SET @userId = 2;
                    
                    -- Reduced size: ~5KB per row (was 50KB)
                    SET @desc = REPLICATE(CAST(N'Lorem ipsum dolor sit amet, consectetur adipiscing elit. ' AS NVARCHAR(MAX)), 45); 

                    WHILE @i < @max
                    BEGIN
                        INSERT INTO [Catalog_Product] (
                            [Id], [BrandId], [CreatedById], [CreatedOn], [Description], [DisplayOrder], 
                            [HasOptions], [IsAllowToOrder], [IsCallForPricing], [IsDeleted], [IsFeatured], 
                            [IsPublished], [IsVisibleIndividually], [LatestUpdatedById], [LatestUpdatedOn], 
                            [Name], [NormalizedName], [Price], [ReviewsCount], [Sku], [Slug], 
                            [StockQuantity], [StockTrackingIsEnabled], [TaxClassId], [ThumbnailImageId], [VendorId]
                        ) VALUES (
                            @i, NULL, @userId, GETDATE(), @desc, 0, 
                            0, 1, 0, 0, 1, 1, 1, @userId, GETDATE(), 
                            N'Large Data Product ' + CAST(@i AS NVARCHAR(20)), 
                            N'LARGE DATA PRODUCT ' + CAST(@i AS NVARCHAR(20)), 
                            1000000, 0, 
                            N'SKU-' + CAST(@i AS NVARCHAR(20)), 
                            N'large-data-product-' + CAST(@i AS NVARCHAR(20)), 
                            100, 1, NULL, NULL, NULL
                        );
                        SET @i = @i + 1;
                    END

                    SET IDENTITY_INSERT [Catalog_Product] OFF;
                ";

                migrationBuilder.Sql(sql);
            }
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
