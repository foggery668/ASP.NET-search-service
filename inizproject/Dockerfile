FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["inizproject/inizproject.csproj", "inizproject/"]
RUN dotnet restore "inizproject/inizproject.csproj"
COPY . .
WORKDIR "/src/inizproject"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "inizproject.dll"]