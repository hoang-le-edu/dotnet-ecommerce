# Quick script to check data after importing
Write-Host "Checking database counts..." -ForegroundColor Cyan
Write-Host ""

$result = sqlcmd -S "(localdb)\MSSQLLocalDB" -d SimplCommerce -Q "SELECT 'Products' AS TableName, COUNT(*) AS Count FROM Catalog_Product UNION ALL SELECT 'Categories', COUNT(*) FROM Catalog_Category UNION ALL SELECT 'Brands', COUNT(*) FROM Catalog_Brand UNION ALL SELECT 'Core_Entity', COUNT(*) FROM Core_Entity UNION ALL SELECT 'Widgets', COUNT(*) FROM Core_WidgetInstance"

Write-Host $result
Write-Host ""

if ($result -match "Products\s+15000") {
    Write-Host "SUCCESS! 15,000 products imported!" -ForegroundColor Green
} elseif ($result -match "Products\s+26") {
    Write-Host "FAILED: Still showing old data (26 products)" -ForegroundColor Red
    Write-Host "Try clicking 'Do it!' again" -ForegroundColor Yellow
} else {
    Write-Host "Data imported, checking counts..." -ForegroundColor Yellow
}

