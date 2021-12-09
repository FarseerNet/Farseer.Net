FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Demo/Farseer.Net.EventBusDemo/Farseer.Net.EventBusDemo.csproj", "Farseer.Net.EventBusDemo/"]
RUN dotnet restore "Demo/Farseer.Net.EventBusDemo/Farseer.Net.EventBusDemo.csproj"
COPY . .
WORKDIR "/src/Farseer.Net.EventBusDemo"
RUN dotnet build "Farseer.Net.EventBusDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Farseer.Net.EventBusDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Farseer.Net.EventBusDemo.dll"]
