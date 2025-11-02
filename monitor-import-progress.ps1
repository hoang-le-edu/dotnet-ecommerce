# MONITOR IMPORT PROGRESS
# Run this while import is happening to see real-time progress

Write-Host "Monitoring SimplCommerce data import..." -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow
Write-Host ""

$startTime = Get-Date

while ($true) {
    $elapsed = (Get-Date) - $startTime
    
    try {
        $result = sqlcmd -S "(localdb)\MSSQLLocalDB" -d SimplCommerce -Q "SELECT COUNT(*) AS ProductCount FROM Catalog_Product" -h -1 -W 2>&1
        
        if ($result -match '\d+') {
            $productCount = [int]($result | Select-String -Pattern '\d+').Matches[0].Value
        } else {
            $productCount = 0
        }
        
        $progress = [math]::Round(($productCount / 15000) * 100, 1)
        $progressBar = "=" * [math]::Floor($progress / 5) + " " * (20 - [math]::Floor($progress / 5))
        
        Write-Host "`r[$progressBar] $productCount / 15,000 products ($progress%) - Elapsed: $($elapsed.ToString('mm\:ss'))" -NoNewline -ForegroundColor Green
        
        if ($productCount -ge 15000) {
            Write-Host ""
            Write-Host ""
            Write-Host "IMPORT COMPLETED! $productCount products loaded in $($elapsed.ToString('mm\:ss'))" -ForegroundColor Green
            Write-Host ""
            
            # Show final stats
            $stats = sqlcmd -S "(localdb)\MSSQLLocalDB" -d SimplCommerce -Q "SELECT 'Products' AS Item, COUNT(*) AS Count FROM Catalog_Product UNION ALL SELECT 'Categories', COUNT(*) FROM Catalog_Category UNION ALL SELECT 'Brands', COUNT(*) FROM Catalog_Brand"
            Write-Host $stats
            break
        }
        
        Start-Sleep -Seconds 2
    }
    catch {
        Write-Host "`rError querying database. Import may not have started yet..." -NoNewline -ForegroundColor Yellow
        Start-Sleep -Seconds 2
    }
}

Write-Host ""
Write-Host "You can now browse the application!" -ForegroundColor Cyan
Write-Host "https://localhost:44388/" -ForegroundColor White

