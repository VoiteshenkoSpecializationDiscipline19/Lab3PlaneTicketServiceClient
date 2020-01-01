#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see http://aka.ms/containercompat 

FROM microsoft/aspnetcore:2.0-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 8080

FROM microsoft/aspnetcore-build:2.0-nanoserver-1803 AS build
WORKDIR /src
COPY Lab3/Lab3.csproj Lab3/
RUN dotnet restore Lab3/Lab3.csproj
COPY . .
WORKDIR /src/Lab3
RUN dotnet build Lab3.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Lab3.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Lab3.dll"]
