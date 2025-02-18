FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app
RUN apt-get update && apt-get install -y libfontconfig1

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Pencil/Pencil.csproj", "Pencil/"]
RUN dotnet restore "Pencil/Pencil.csproj"
COPY . .
WORKDIR "/src/Pencil"
RUN dotnet build "Pencil.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pencil.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pencil.dll"]
