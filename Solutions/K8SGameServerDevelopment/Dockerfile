# Define base image to use for building application
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
# Setting working directory to "app"
WORKDIR /app

# TODO: Add restore

# Copies all files from solution
COPY . ./
# Publishes PatchManager project as release, into app/patchmanager/out
RUN dotnet publish K8SGameServerDevelopment -c Release -o k8sgameserverdevelopment/out

# Defines new base image to run application
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
# Sets the working directory to "app"
WORKDIR /app
# Copies all files from the release build
COPY --from=build-env /app/k8sgameserverdevelopment/out/ .

# Runs the application with dotnet, with PatchManager.dll as parameter
ENTRYPOINT [ "dotnet", "K8SGameServerDevelopment.dll" ]
