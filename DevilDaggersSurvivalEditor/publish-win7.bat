dotnet build -c Release -o bin\publish-win7\ --self-contained false

%SystemRoot%\explorer.exe "%cd%\bin\"
