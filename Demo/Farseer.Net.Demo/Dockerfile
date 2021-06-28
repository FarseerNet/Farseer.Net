FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Demo/Farseer.Net.Demo/Farseer.Net.Demo.csproj", "Farseer.Net.Demo/"]
RUN dotnet restore "Demo/Farseer.Net.Demo/Farseer.Net.Demo.csproj"
COPY . .
WORKDIR "/src/Farseer.Net.Demo"
RUN dotnet build "Farseer.Net.Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Farseer.Net.Demo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Farseer.Net.Demo.dll"]
