FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

RUN apt-get update && apt-get install -y \
    libopus-dev \
    libsodium-dev \
    ca-certificates \
    curl \
    iputils-ping \
    ffmpeg \
    && update-ca-certificates \
    && rm -rf /var/lib/apt/lists/* 

COPY . .
RUN curl -sSL https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp -o /usr/local/bin/yt-dlp && \
    chmod +x /usr/local/bin/yt-dlp 


RUN dotnet restore "Bot.Discord/Bot.Discord.csproj"
FROM marrold/marrold-opus-tools:latest AS opus-tools 
COPY FFmpeg /app/FFmpeg
COPY yt-dpl.exe /app/yt-dpl.exe
WORKDIR "/src/Bot.Discord"
RUN dotnet build "Bot.Discord.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Bot.Discord.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bot.Discord.dll"]
