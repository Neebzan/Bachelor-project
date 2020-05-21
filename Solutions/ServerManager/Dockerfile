# Define base image to use for building application
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
# Setting working directory to "app"
WORKDIR /app

# TODO: Add restore

# Copies all files from solution
COPY . ./
# Publishes PatchManager project as release, into app/patchmanager/out
RUN dotnet publish ServerManager -c Release -o servermanager/out

# Defines new base image to run application
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
# Sets the working directory to "app"
WORKDIR /app
# Copies all files from the release build
COPY --from=build-env /app/servermanager/out/ .

WORKDIR /tmp

RUN  /usr/bin/curl -LO https://storage.googleapis.com/kubernetes-release/release/$(curl -s https://storage.googleapis.com/kubernetes-release/release/stable.txt)/bin/linux/amd64/kubectl \
     && chmod +x ./kubectl  \
     &&  mv ./kubectl /usr/local/bin/kubectl \
     && zypper install -y python2 \
     && zypper install -y python2-pip \
     && pip install kubernetes \
     && zypper install -y git \
     && zypper clean -a \
     && git clone --recursive https://github.com/kubernetes-client/python.git \
     && cd python \
     && python setup.py install

# Runs the application with dotnet, with PatchManager.dll as parameter
ENTRYPOINT [ "dotnet", "ServerManager.dll" ]
