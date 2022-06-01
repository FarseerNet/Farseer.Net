FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Study/Study/Study.csproj", "Study/"]
RUN dotnet restore "Study/Study/Study.csproj"
COPY . .
WORKDIR "/src/Study"
RUN dotnet build "Study.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Study.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Study.dll"]
