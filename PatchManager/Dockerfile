# FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
# WORKDIR /app

# FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
# WORKDIR /src
# COPY . ./
# RUN dotnet restore *.csproj
# COPY . .
# WORKDIR "/src/."
# RUN dotnet build "PatchManager.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "PatchManager.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "PatchManager.dll"]

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet publish PatchManager -c Release -o patchmanager/out

FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build-env /app/patchmanager/out/ .

ENTRYPOINT [ "dotnet", "PatchManager.dll" ]
