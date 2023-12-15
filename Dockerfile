FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY ./publish .
ENTRYPOINT ["dotnet", "./publish/Bilbo.dll"]
