# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 1
EXPOSE 8081

# Install dependencies for ChromeDriver and Selenium in the runtime image
RUN apt-get update && apt-get install -y \
    wget \
    unzip \
    curl \
    gnupg2 \
    ca-certificates \
    libnspr4 \
    libnss3 \
    fonts-liberation \
    libasound2 \
    libatk-bridge2.0-0 \
    libatk1.0-0 \
    libatspi2.0-0 \
    libcairo2 \
    libcups2 \
    libcurl4 \
    libgbm1 \
    libgtk-3-0 \
    libpango-1.0-0 \
    libvulkan1 \
    libxdamage1 \
    libxkbcommon0 \
    xdg-utils \
    && rm -rf /var/lib/apt/lists/* 
    
# Download and install Chrome and ChromeDriver in the runtime image
RUN wget -q -O /tmp/google-chrome.deb https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb && \
    apt-get install -y /tmp/google-chrome.deb && \
    rm /tmp/google-chrome.deb

RUN mkdir -p /usr/bin/ && \
    wget -q -O /tmp/chromedriver.zip https://storage.googleapis.com/chrome-for-testing-public/131.0.6778.108/linux64/chromedriver-linux64.zip && \
    unzip /tmp/chromedriver.zip -d /usr/bin/ && \
    chmod +x /usr/bin/chromedriver-linux64/chromedriver && \
    rm /tmp/chromedriver.zip

# Build image with SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file and restore dependencies
COPY ["WebApi.Brokers.CharlesSchwab/WebApi.Brokers.CharlesSchwab.csproj", "WebApi.Brokers.CharlesSchwab/"]
RUN dotnet restore "WebApi.Brokers.CharlesSchwab/WebApi.Brokers.CharlesSchwab.csproj"

# Copy all source code and build the project
COPY . .
WORKDIR "/src/WebApi.Brokers.CharlesSchwab"
RUN dotnet build "WebApi.Brokers.CharlesSchwab.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebApi.Brokers.CharlesSchwab.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --verbosity detailed

# Final runtime image
FROM base AS final
WORKDIR /app

# Copy published application files
COPY --from=publish /app/publish .

# Set permissions for the selenium-manager binary (if present)
RUN chmod +x /app/selenium-manager/linux/selenium-manager || echo "No binary found to chmod."

# Define the entry point for the application
ENTRYPOINT ["dotnet", "WebApi.Brokers.CharlesSchwab.dll"]
