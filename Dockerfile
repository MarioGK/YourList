FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

COPY src/ ./app/src

WORKDIR /app

RUN dotnet publish src/YourList.Worker/YourList.Worker.csproj -o out -c Release -p:ci=true

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS runtime

WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "YourList.Worker.dll", ""]