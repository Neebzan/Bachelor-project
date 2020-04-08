# Define base image to use for building application
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
# Setting working directory to "app"
WORKDIR /app

# TODO: Add restore

# Copies all files from solution
COPY . ./
# Publishes PatchManager project as release, into app/patchmanager/out
RUN dotnet publish PatchManager -c Release -o patchmanager/out

# Add Files to the PatchServer (For testing) - Look into Volumes
ADD PatchManager/TestFiles patchmanager/out/TestFiles

# Defines new base image to run application
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
# Sets the working directory to "app"
WORKDIR /app
# Copies all files from the release build
COPY --from=build-env /app/patchmanager/out/ .

# Runs the application with dotnet, with PatchManager.dll as parameter
ENTRYPOINT [ "dotnet", "PatchManager.dll" ]
