FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Demo/Farseer.Net.CacheDemo/Farseer.Net.CacheDemo.csproj", "Farseer.Net.CacheDemo/"]
RUN dotnet restore "Demo/Farseer.Net.CacheDemo/Farseer.Net.CacheDemo.csproj"
COPY . .
WORKDIR "/src/Farseer.Net.CacheDemo"
RUN dotnet build "Farseer.Net.CacheDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Farseer.Net.CacheDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Farseer.Net.CacheDemo.dll"]
