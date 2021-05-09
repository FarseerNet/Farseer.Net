FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Demo/Farseer.Net.MQ.RedisStreamDemo/Farseer.Net.MQ.RedisStreamDemo.csproj", "Farseer.Net.MQ.RedisStreamDemo/"]
RUN dotnet restore "Demo/Farseer.Net.MQ.RedisStreamDemo/Farseer.Net.MQ.RedisStreamDemo.csproj"
COPY . .
WORKDIR "/src/Farseer.Net.MQ.RedisStreamDemo"
RUN dotnet build "Farseer.Net.MQ.RedisStreamDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Farseer.Net.MQ.RedisStreamDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Farseer.Net.MQ.RedisStreamDemo.dll"]
