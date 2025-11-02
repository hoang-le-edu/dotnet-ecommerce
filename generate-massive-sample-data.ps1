# ============================================================================
# GENERATE MASSIVE SAMPLE DATA SQL SCRIPT
# ============================================================================
# This script generates a ResetToSampleData.sql file with 1000 products
# per category for SimplCommerce SampleData module.
#
# Usage:
#   .\generate-massive-sample-data.ps1
#
# Output:
#   src\Modules\SimplCommerce.Module.SampleData\SampleContent\Phones\ResetToSampleData.sql
# ============================================================================

$outputPath = "src\Modules\SimplCommerce.Module.SampleData\SampleContent\Phones\ResetToSampleData.sql"

Write-Host "Generating massive sample data SQL script..." -ForegroundColor Cyan
Write-Host "Target: $outputPath" -ForegroundColor Gray
Write-Host ""

$sql = New-Object System.Text.StringBuilder

# ============================================================================
# HEADER: DELETE ALL EXISTING DATA
# ============================================================================
$null = $sql.AppendLine("-- ============================================================================")
$null = $sql.AppendLine("-- SIMPLCOMMERCE MASSIVE SAMPLE DATA")
$null = $sql.AppendLine("-- Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
$null = $sql.AppendLine("-- Products per category: 1000")
$null = $sql.AppendLine("-- Total products: ~15,000")
$null = $sql.AppendLine("-- ============================================================================")
$null = $sql.AppendLine("")
$null = $sql.AppendLine("-- PHASE 1: DELETE ALL EXISTING DATA")
$null = $sql.AppendLine("-- Call stored procedure to clear menu items first")
$null = $sql.AppendLine("IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'ClearSampleData')")
$null = $sql.AppendLine("    EXEC ClearSampleData")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("-- Delete remaining data")
$null = $sql.AppendLine("DELETE FROM [dbo].[Cms_MenuItem]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Core_Entity]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[ShoppingCart_CartItem]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Checkouts_CheckoutItem]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Checkouts_Checkout]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[WishList_WishListItem]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[WishList_WishList]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Payments_Payment]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Shipments_ShipmentItem]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Shipments_Shipment]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Pricing_CartRuleUsage]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Pricing_CartRule]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Orders_OrderHistory]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Orders_OrderItem]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Orders_Order]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Inventory_StockHistory]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Inventory_Stock]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[ProductComparison_ComparingProduct]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Catalog_ProductAttributeValue]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Catalog_ProductOptionCombination]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Catalog_ProductOptionValue]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Catalog_ProductAttribute]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Catalog_ProductAttributeGroup]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Catalog_ProductMedia]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Catalog_ProductCategory]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Catalog_ProductLink]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Catalog_ProductPriceHistory]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Catalog_Product]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Catalog_Category]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[News_NewsItem]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Core_Media]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Catalog_Brand]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Catalog_ProductTemplateProductAttribute]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Catalog_ProductTemplate]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Cms_Page]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Core_WidgetInstance]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Reviews_Reply]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("DELETE FROM [dbo].[Reviews_Review]")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("")

# ============================================================================
# CATEGORIES
# ============================================================================
$null = $sql.AppendLine("-- PHASE 2: INSERT CATEGORIES")
$null = $sql.AppendLine("SET IDENTITY_INSERT [dbo].[Catalog_Category] ON")

$categories = @(
    @{Id=1; Name="Dien thoai"; Slug="dien-thoai"; Parent=$null},
    @{Id=2; Name="Laptop"; Slug="laptop"; Parent=$null},
    @{Id=3; Name="Tablet"; Slug="tablet"; Parent=$null},
    @{Id=4; Name="Phu kien"; Slug="phu-kien"; Parent=$null},
    @{Id=5; Name="Dong ho thong minh"; Slug="dong-ho-thong-minh"; Parent=$null},
    @{Id=6; Name="Am thanh"; Slug="am-thanh"; Parent=$null},
    @{Id=7; Name="PC va Linh kien"; Slug="pc-linh-kien"; Parent=$null},
    @{Id=8; Name="Thiet bi mang"; Slug="thiet-bi-mang"; Parent=$null},
    @{Id=9; Name="iPhone"; Slug="iphone"; Parent=1},
    @{Id=10; Name="Samsung Galaxy"; Slug="samsung-galaxy"; Parent=1},
    @{Id=11; Name="Xiaomi"; Slug="xiaomi-phones"; Parent=1},
    @{Id=12; Name="Laptop van phong"; Slug="laptop-van-phong"; Parent=2},
    @{Id=13; Name="Laptop gaming"; Slug="laptop-gaming"; Parent=2},
    @{Id=14; Name="Tai nghe"; Slug="tai-nghe"; Parent=4},
    @{Id=15; Name="Sac va Cap"; Slug="sac-cap"; Parent=4}
)

foreach ($cat in $categories) {
    $parentId = if ($cat.Parent) { $cat.Parent } else { "NULL" }
    $null = $sql.AppendLine("INSERT [dbo].[Catalog_Category] ([Id], [Name], [Slug], [Description], [DisplayOrder], [IsPublished], [IsDeleted], [ParentId], [ThumbnailImageId], [IncludeInMenu]) VALUES ($($cat.Id), N'$($cat.Name)', N'$($cat.Slug)', NULL, 0, 1, 0, $parentId, NULL, 1)")
}

$null = $sql.AppendLine("SET IDENTITY_INSERT [dbo].[Catalog_Category] OFF")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("")

# ============================================================================
# BRANDS
# ============================================================================
$null = $sql.AppendLine("-- PHASE 3: INSERT BRANDS")
$null = $sql.AppendLine("SET IDENTITY_INSERT [dbo].[Catalog_Brand] ON")

$brands = @(
    @{Id=1; Name="Apple"; Slug="apple"},
    @{Id=2; Name="Samsung"; Slug="samsung"},
    @{Id=3; Name="Dell"; Slug="dell"},
    @{Id=4; Name="HP"; Slug="hp"},
    @{Id=5; Name="Lenovo"; Slug="lenovo"},
    @{Id=6; Name="Xiaomi"; Slug="xiaomi"},
    @{Id=7; Name="ASUS"; Slug="asus"},
    @{Id=8; Name="Sony"; Slug="sony"},
    @{Id=9; Name="Logitech"; Slug="logitech"},
    @{Id=10; Name="Anker"; Slug="anker"}
)

foreach ($brand in $brands) {
    $null = $sql.AppendLine("INSERT [dbo].[Catalog_Brand] ([Id], [Name], [Slug], [Description], [IsPublished], [IsDeleted]) VALUES ($($brand.Id), N'$($brand.Name)', N'$($brand.Slug)', NULL, 1, 0)")
}

$null = $sql.AppendLine("SET IDENTITY_INSERT [dbo].[Catalog_Brand] OFF")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("")

# ============================================================================
# MEDIA (Placeholder images)
# ============================================================================
$null = $sql.AppendLine("-- PHASE 4: INSERT MEDIA (Placeholder images)")
$null = $sql.AppendLine("SET IDENTITY_INSERT [dbo].[Core_Media] ON")

for ($i = 1; $i -le 100; $i++) {
    $guid = [guid]::NewGuid().ToString()
    $null = $sql.AppendLine("INSERT [dbo].[Core_Media] ([Id], [Caption], [FileName], [FileSize], [MediaType]) VALUES ($i, NULL, N'$guid.jpg', 0, 1)")
}

$null = $sql.AppendLine("SET IDENTITY_INSERT [dbo].[Core_Media] OFF")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("")

# ============================================================================
# PRODUCTS (1000 per category)
# ============================================================================
$null = $sql.AppendLine("-- PHASE 5: INSERT PRODUCTS (1000 per category)")
$null = $sql.AppendLine("SET IDENTITY_INSERT [dbo].[Catalog_Product] ON")

$productId = 1
$mediaId = 1

# Categories to generate products for
$productCategories = @(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15)

$productNames = @(
    @{Prefix="iPhone 15"; Variants=@("64GB", "128GB", "256GB", "512GB")},
    @{Prefix="Samsung Galaxy S24"; Variants=@("128GB", "256GB", "512GB")},
    @{Prefix="Xiaomi Redmi Note"; Variants=@("64GB", "128GB", "256GB")},
    @{Prefix="Dell XPS"; Variants=@("13", "15", "17")},
    @{Prefix="HP Pavilion"; Variants=@("14", "15", "Gaming")},
    @{Prefix="MacBook Air"; Variants=@("M1", "M2", "M3")},
    @{Prefix="iPad Pro"; Variants=@("11 inch", "12.9 inch")},
    @{Prefix="Apple Watch"; Variants=@("Series 9", "Ultra 2", "SE")},
    @{Prefix="AirPods"; Variants=@("2", "3", "Pro", "Max")},
    @{Prefix="Tai nghe Sony"; Variants=@("WH-1000XM5", "WH-CH520", "WF-1000XM5")},
    @{Prefix="Logitech MX"; Variants=@("Master 3", "Keys", "Anywhere 3")},
    @{Prefix="Anker PowerBank"; Variants=@("10000mAh", "20000mAh", "30000mAh")}
)

Write-Host "Generating products..." -ForegroundColor Yellow

foreach ($catId in $productCategories) {
    $categoryName = ($categories | Where-Object { $_.Id -eq $catId }).Name
    Write-Host "  Category $catId ($categoryName) - " -NoNewline -ForegroundColor Gray
    
    for ($i = 1; $i -le 1000; $i++) {
        $nameTemplate = $productNames | Get-Random
        $variant = $nameTemplate.Variants | Get-Random
        $brandId = Get-Random -Minimum 1 -Maximum 11
        $brandName = ($brands | Where-Object { $_.Id -eq $brandId }).Name
        
        $productName = "$($nameTemplate.Prefix) $variant - $brandName - Model $i"
        $slug = "$($nameTemplate.Prefix)-$variant-$brandName-model-$i".ToLower() -replace '\s+', '-' -replace '[^a-z0-9\-]', ''
        $price = Get-Random -Minimum 100 -Maximum 5000
        $oldPrice = [math]::Round($price * 1.2, 2)
        $sku = "SKU-$catId-$i"
        $mediaIdForProduct = (($mediaId % 100) + 1)
        $isFeatured = if ($i % 10 -eq 0) { 1 } else { 0 }
        
        $description = "<p>$productName is a high-quality product from $brandName. Designed with the latest technology, this product delivers superior performance and excellent user experience.</p><p><strong>Key features:</strong></p><ul><li>Powerful performance</li><li>Premium design</li><li>High durability</li><li>Best value</li></ul>"
        
        $shortDesc = "<p>$productName - Genuine $brandName</p>"
        
        $null = $sql.AppendLine("INSERT [dbo].[Catalog_Product] ([Id], [StockTrackingIsEnabled], [BrandId], [CreatedById], [CreatedOn], [Description], [DisplayOrder], [HasOptions], [IsDeleted], [IsFeatured], [IsPublished], [IsVisibleIndividually], [MetaDescription], [MetaKeywords], [MetaTitle], [Name], [NormalizedName], [OldPrice], [Price], [PublishedOn], [Slug], [ShortDescription], [Sku], [Specification], [ThumbnailImageId], [LatestUpdatedById], [LatestUpdatedOn], [RatingAverage], [ReviewsCount], [SpecialPrice], [SpecialPriceEnd], [SpecialPriceStart], [IsAllowToOrder], [IsCallForPricing], [StockQuantity], [VendorId], [TaxClassId]) VALUES ($productId, 1, $brandId, 10, CAST(N'2024-01-01T00:00:00.0000000+00:00' AS DateTimeOffset), N'$description', 0, 0, 0, $isFeatured, 1, 1, NULL, NULL, NULL, N'$productName', NULL, CAST($oldPrice AS Decimal(18, 2)), CAST($price AS Decimal(18, 2)), NULL, N'$slug', N'$shortDesc', N'$sku', NULL, $mediaIdForProduct, 10, CAST(N'2024-01-01T00:00:00.0000000+00:00' AS DateTimeOffset), NULL, 0, NULL, NULL, NULL, 1, 0, 100, NULL, 1)")
        
        $productId++
        $mediaId++
        
        if ($i % 100 -eq 0) {
            Write-Host "$i " -NoNewline -ForegroundColor Cyan
        }
    }
    
    Write-Host "Done" -ForegroundColor Green
}

$null = $sql.AppendLine("SET IDENTITY_INSERT [dbo].[Catalog_Product] OFF")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("")

Write-Host "Total products generated: $($productId - 1)" -ForegroundColor Green

# ============================================================================
# PRODUCT CATEGORIES (Link products to categories)
# ============================================================================
$null = $sql.AppendLine("-- PHASE 6: LINK PRODUCTS TO CATEGORIES")
$null = $sql.AppendLine("SET IDENTITY_INSERT [dbo].[Catalog_ProductCategory] ON")

$productCategoryId = 1
$currentProductId = 1

Write-Host "Linking products to categories..." -ForegroundColor Yellow

foreach ($catId in $productCategories) {
    Write-Host "  Category $catId - " -NoNewline -ForegroundColor Gray
    
    for ($i = 1; $i -le 1000; $i++) {
        $null = $sql.AppendLine("INSERT [dbo].[Catalog_ProductCategory] ([Id], [CategoryId], [DisplayOrder], [IsFeaturedProduct], [ProductId]) VALUES ($productCategoryId, $catId, 0, 0, $currentProductId)")
        $productCategoryId++
        $currentProductId++
        
        if ($i % 200 -eq 0) {
            Write-Host "$i " -NoNewline -ForegroundColor Cyan
        }
    }
    
    Write-Host "Done" -ForegroundColor Green
}

$null = $sql.AppendLine("SET IDENTITY_INSERT [dbo].[Catalog_ProductCategory] OFF")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("")

# ============================================================================
# CORE ENTITY (Routing)
# ============================================================================
$null = $sql.AppendLine("-- PHASE 7: INSERT CORE ENTITY (Routing)")
$null = $sql.AppendLine("SET IDENTITY_INSERT [dbo].[Core_Entity] ON")

$entityId = 1

# Category entities
foreach ($cat in $categories) {
    $null = $sql.AppendLine("INSERT [dbo].[Core_Entity] ([Id], [Slug], [Name], [EntityId], [EntityTypeId]) VALUES ($entityId, N'$($cat.Slug)', N'$($cat.Name)', $($cat.Id), N'Category')")
    $entityId++
}

# Brand entities
foreach ($brand in $brands) {
    $null = $sql.AppendLine("INSERT [dbo].[Core_Entity] ([Id], [Slug], [Name], [EntityId], [EntityTypeId]) VALUES ($entityId, N'$($brand.Slug)', N'$($brand.Name)', $($brand.Id), N'Brand')")
    $entityId++
}

$null = $sql.AppendLine("SET IDENTITY_INSERT [dbo].[Core_Entity] OFF")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("")

# ============================================================================
# WIDGETS
# ============================================================================
$null = $sql.AppendLine("-- PHASE 8: INSERT WIDGETS")
$null = $sql.AppendLine("SET IDENTITY_INSERT [dbo].[Core_WidgetInstance] ON")
$widget1Data = "{{{{""""NumberOfProducts"""":8,""""CategoryId"""":null,""""OrderBy"""":0,""""FeaturedOnly"""":true}}}}"
$widget2Data = "{{{{""""NumberOfProducts"""":12,""""CategoryId"""":null,""""OrderBy"""":0,""""FeaturedOnly"""":false}}}}"
$null = $sql.AppendLine("INSERT [dbo].[Core_WidgetInstance] ([Id], [CreatedOn], [Data], [DisplayOrder], [HtmlData], [Name], [PublishEnd], [PublishStart], [LatestUpdatedOn], [WidgetId], [WidgetZoneId]) VALUES (1, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2), N'$widget1Data', 0, NULL, N'Featured Products', NULL, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2), N'ProductWidget', 1)")
$null = $sql.AppendLine("INSERT [dbo].[Core_WidgetInstance] ([Id], [CreatedOn], [Data], [DisplayOrder], [HtmlData], [Name], [PublishEnd], [PublishStart], [LatestUpdatedOn], [WidgetId], [WidgetZoneId]) VALUES (2, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2), N'$widget2Data', 1, NULL, N'Latest Products', NULL, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2), N'ProductWidget', 2)")
$null = $sql.AppendLine("SET IDENTITY_INSERT [dbo].[Core_WidgetInstance] OFF")
$null = $sql.AppendLine("GO")
$null = $sql.AppendLine("")


# ============================================================================
# SUMMARY
# ============================================================================
$null = $sql.AppendLine("-- ============================================================================")
$null = $sql.AppendLine("-- SUMMARY")
$null = $sql.AppendLine("-- ============================================================================")
$null = $sql.AppendLine("SELECT 'Categories' AS [Table], COUNT(*) AS [Count] FROM [Catalog_Category]")
$null = $sql.AppendLine("UNION ALL")
$null = $sql.AppendLine("SELECT 'Brands', COUNT(*) FROM [Catalog_Brand]")
$null = $sql.AppendLine("UNION ALL")
$null = $sql.AppendLine("SELECT 'Products', COUNT(*) FROM [Catalog_Product]")
$null = $sql.AppendLine("UNION ALL")
$null = $sql.AppendLine("SELECT 'Product-Category Links', COUNT(*) FROM [Catalog_ProductCategory]")
$null = $sql.AppendLine("UNION ALL")
$null = $sql.AppendLine("SELECT 'Core Entities', COUNT(*) FROM [Core_Entity]")
$null = $sql.AppendLine("UNION ALL")
$null = $sql.AppendLine("SELECT 'Widgets', COUNT(*) FROM [Core_WidgetInstance]")
$null = $sql.AppendLine("GO")

# ============================================================================
# WRITE TO FILE
# ============================================================================
Write-Host ""
Write-Host "Writing to file..." -ForegroundColor Yellow
$sqlContent = $sql.ToString()
[System.IO.File]::WriteAllText($outputPath, $sqlContent, [System.Text.Encoding]::UTF8)

$fileSize = (Get-Item $outputPath).Length / 1MB
Write-Host "File created successfully: $outputPath" -ForegroundColor Green
Write-Host "File size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Gray
Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "SUMMARY" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "Categories:        $($categories.Count)" -ForegroundColor White
Write-Host "Brands:            $($brands.Count)" -ForegroundColor White
Write-Host "Products:          $($productId - 1)" -ForegroundColor White
Write-Host "Product-Cat Links: $($productCategoryId - 1)" -ForegroundColor White
Write-Host "Media:             100" -ForegroundColor White
Write-Host "Core Entities:     $($entityId - 1)" -ForegroundColor White
Write-Host "Widgets:           2" -ForegroundColor White
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Build and run the application" -ForegroundColor White
Write-Host "2. Navigate to /SampleData/SampleData/Index" -ForegroundColor White
Write-Host "3. Select 'Phones' industry" -ForegroundColor White
Write-Host "4. Click 'Do it!' button" -ForegroundColor White
Write-Host ""
Write-Host "WARNING: This will DELETE ALL existing data!" -ForegroundColor Red
Write-Host ""


