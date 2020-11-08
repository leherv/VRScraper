FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
WORKDIR /app/VRScraper
# download playwright-sharp-tool
RUN dotnet tool restore
# download the browsers using the playwright-sharp-tool we restored
RUN dotnet playwright-sharp install-browsers
WORKDIR /app
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
# Install WebKit dependencies
RUN apt-get update && apt-get install -y libwoff1 \
    libopus0 \
    libwebp6 \
    libwebpdemux2 \
    libenchant1c2a \
    libgudev-1.0-0 \
    libsecret-1-0 \
    libhyphen0 \
    libgdk-pixbuf2.0-0 \
    libegl1 \
    libnotify4 \
    libxslt1.1 \
    libevent-2.1-6 \
    libgles2 \
    libvpx5
# Install Chromium dependencies
RUN apt-get update && apt-get install -y libnss3 \
    libxss1 \
    libasound2 \
    fonts-noto-color-emoji
# Install Firefox dependencies
RUN apt-get update && apt-get install -y libdbus-glib-1-2 \
    libxt6
WORKDIR /root
RUN mkdir .cache
# copy the browser binaries we downloaded into the build-env with playwright sharp into the runtime-env
COPY --from=build-env /root/.cache/ ./.cache/
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "VRScraper.dll", "--urls", "http://0.0.0.0:5000"]