FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AIResumeAnalyzerSystem.API/AIResumeAnalyzerSystem.API.csproj", "AIResumeAnalyzerSystem.API/"]
COPY ["AIResumeAnalyzerSystem.Core/AIResumeAnalyzerSystem.Core.csproj", "AIResumeAnalyzerSystem.Core/"]
COPY ["AIResumeAnalyzerSystem.Infrastructure/AIResumeAnalyzerSystem.Infrastructure.csproj", "AIResumeAnalyzerSystem.Infrastructure/"]

RUN dotnet restore "AIResumeAnalyzerSystem.API/AIResumeAnalyzerSystem.API.csproj"

COPY . .

RUN dotnet publish "AIResumeAnalyzerSystem.API/AIResumeAnalyzerSystem.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
ENTRYPOINT ["dotnet", "AIResumeAnalyzerSystem.API.dll"]