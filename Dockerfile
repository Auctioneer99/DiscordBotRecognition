FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
RUN apt upgrade -y libc6
RUN apt update -y
RUN apt install -y ffmpeg


FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
RUN apt update
RUN apt install -y build-essential manpages-dev git automake autoconf
WORKDIR "/src"
COPY . .
WORKDIR "/src/dlls"
RUN tar -xzvf libsodium-1.0.18-stable.tar.gz
RUN tar -xzvf opus-1.3.1.tar.gz
WORKDIR "/src/dlls/libsodium-stable"
RUN ./configure --prefix=/libs
RUN make
RUN make install
WORKDIR "/src/dlls/opus-1.3.1"
RUN ./configure --prefix=/libs
RUN make
RUN make install

WORKDIR "/src/DiscordBotServer/DiscordBotServer"
RUN dotnet restore "DiscordBotServer.csproj"
RUN dotnet build "DiscordBotServer.csproj" -c Release -o /app/build
RUN dotnet publish "DiscordBotServer.csproj" -c Release -o /app/publish


FROM base AS final
RUN apt update
WORKDIR "/app"
COPY --from=build /app/publish .
COPY --from=build /libs /
ENTRYPOINT ["dotnet", "DiscordBotServer.dll"]