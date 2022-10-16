FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["PinjaCodeChallenge/PinjaCodeChallenge.csproj", "PinjaCodeChallenge/"]
RUN dotnet restore "PinjaCodeChallenge/PinjaCodeChallenge.csproj"
COPY . .
WORKDIR "/src/PinjaCodeChallenge"
RUN dotnet build "PinjaCodeChallenge.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PinjaCodeChallenge.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PinjaCodeChallenge.dll"]
