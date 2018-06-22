FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 63159
EXPOSE 44398

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Hotels.API/Hotels.API.csproj Hotels.API/
RUN dotnet restore Hotels.API/Hotels.API.csproj
COPY . .
WORKDIR /src/Hotels.API
RUN dotnet build Hotels.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Hotels.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Hotels.API.dll"]
