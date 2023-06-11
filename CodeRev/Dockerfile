FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash - && apt-get install -yq nodejs build-essential
COPY WebClient/ ./WebClient
WORKDIR /app/WebClient/src/app/global-services/request/models
RUN rm url-routes.ts
RUN mv url-routes-prod.ts url-routes.ts
WORKDIR /app/WebClient
RUN npm install

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS publish
WORKDIR /src
COPY ./Core ./Core
COPY ./CompilerService ./CompilerService
COPY ./TrackerService ./TrackerService
COPY ./TrackerService.Contracts ./TrackerService.Contracts
COPY ./TrackerService.DataAccess ./TrackerService.DataAccess
COPY ./UserService ./UserService
COPY ./UserService.DAL ./UserService.DAL
COPY ./TaskTestsProvider ./TaskTestsProvider
WORKDIR "/src/Core"
RUN dotnet restore "Core.csproj"
RUN dotnet publish "Core.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./Core
WORKDIR /app/Core
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Core.dll"]