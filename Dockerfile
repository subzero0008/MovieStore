FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Използвай официалния .NET образ за изграждане
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MovieStoreMvc.csproj", "./"]  
RUN dotnet restore "MovieStoreMvc.csproj"  
COPY . . 

RUN dotnet build "MovieStoreMvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MovieStoreMvc.csproj" -c Release -o /app/publish

# Стартов етап
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 
ENTRYPOINT ["dotnet", "MovieStoreMvc.dll"] 
