FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/api/SmartEventBooking.Web/SmartEventBooking.Web.csproj", "src/api/SmartEventBooking.Web/"]
COPY ["src/api/SmartEventBooking.Application/SmartEventBooking.Application.csproj", "src/api/SmartEventBooking.Application/"]
COPY ["src/api/SmartEventBooking.Domain/SmartEventBooking.Domain.csproj", "src/api/SmartEventBooking.Domain/"]
COPY ["src/api/SmartEventBooking.Infrastructure/SmartEventBooking.Infrastructure.csproj", "src/api/SmartEventBooking.Infrastructure/"]
COPY ["src/api/SmartEventBooking.Shared/SmartEventBooking.Shared.csproj", "src/api/SmartEventBooking.Shared/"]

RUN dotnet restore "src/api/SmartEventBooking.Web/SmartEventBooking.Web.csproj"

COPY . .
WORKDIR "/src/src/api/SmartEventBooking.Web"

RUN dotnet build "SmartEventBooking.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartEventBooking.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartEventBooking.Web.dll"]