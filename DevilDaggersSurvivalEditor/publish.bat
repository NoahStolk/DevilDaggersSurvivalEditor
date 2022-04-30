dotnet publish ^
-p:PublishSingleFile=True ^
-p:PublishReadyToRun=False ^
-p:PublishProtocol=FileSystem ^
-p:SelfContained=true ^
-p:TargetFramework=net6.0-windows ^
-p:RuntimeIdentifier=win-x64 ^
-p:PublishDir=bin\x64\Release\net6.0-windows\win-x64\publish\win-x64\ ^
-p:Platform=x64 ^
-p:Configuration=Release ^
-p:PublishMethod=SELF_CONTAINED

%SystemRoot%\explorer.exe "%cd%\bin\x64\Release\net6.0-windows\win-x64\publish\"
