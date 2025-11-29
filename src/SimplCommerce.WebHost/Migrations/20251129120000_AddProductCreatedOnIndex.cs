using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplCommerce.WebHost.Migrations
{
    public partial class AddProductCreatedOnIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add index on CreatedOn column for fast sorting in ProductWidget queries
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Catalog_Product_CreatedOn_Filtered' AND object_id = OBJECT_ID('Catalog_Product'))
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_Catalog_Product_CreatedOn_Filtered] 
                    ON [dbo].[Catalog_Product] ([CreatedOn] DESC, [Id] DESC)
                    INCLUDE ([Name], [Slug], [Price], [OldPrice], [SpecialPrice], [IsPublished], [IsVisibleIndividually], [IsFeatured], [ThumbnailImageId])
                    WHERE [IsPublished] = 1 AND [IsVisibleIndividually] = 1
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Catalog_Product_CreatedOn_Filtered' AND object_id = OBJECT_ID('Catalog_Product'))
                BEGIN
                    DROP INDEX [IX_Catalog_Product_CreatedOn_Filtered] ON [dbo].[Catalog_Product]
                END
            ");
        }
    }
}

