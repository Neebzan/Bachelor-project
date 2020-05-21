# Define base image to use for building application
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
# Setting working directory to "app"
WORKDIR /app

# TODO: Add restore

# Copies all files from solution
COPY . ./
# Publishes PatchManager project as release, into app/patchmanager/out
RUN dotnet publish ServerManager -c Release -o servermanager/out

# Add creation YAML file for pod creation
ADD ServerManager/k8s servermanager/out/Files

# Defines new base image to run application
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
# Sets the working directory to "app"
WORKDIR /app
# Copies all files from the release build
COPY --from=build-env /app/servermanager/out/ .

RUN curl -LO https://storage.googleapis.com/kubernetes-release/release/$(curl -s https://storage.googleapis.com/kubernetes-release/release/stable.txt)/bin/linux/amd64/kubectl
RUN chmod +x ./kubectl
RUN mv ./kubectl /usr/local/bin

# Runs the application with dotnet, with PatchManager.dll as parameter
ENTRYPOINT [ "dotnet", "ServerManager.dll" ]


