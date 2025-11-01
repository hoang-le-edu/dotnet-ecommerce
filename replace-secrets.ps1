# Script to replace Stripe secrets in all files
Get-ChildItem -Recurse -File | ForEach-Object {
    if (Select-String -Path $_.FullName -Pattern "sk_test_BQokikJOvBiI2HlWgH4olfQ2" -Quiet) {
        (Get-Content $_.FullName) -replace 'sk_test_BQokikJOvBiI2HlWgH4olfQ2', 'YOUR_STRIPE_SECRET_KEY' | Set-Content $_.FullName
    }
    if (Select-String -Path $_.FullName -Pattern "pk_test_6pRNASCoBOKtIshFeQd4XMUh" -Quiet) {
        (Get-Content $_.FullName) -replace 'pk_test_6pRNASCoBOKtIshFeQd4XMUh', 'YOUR_STRIPE_PUBLIC_KEY' | Set-Content $_.FullName
    }
}

