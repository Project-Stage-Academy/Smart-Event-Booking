FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/SmartEventBooking.Web/SmartEventBooking.Web.csproj", "src/SmartEventBooking.Web/"]
COPY ["src/SmartEventBooking.Application/SmartEventBooking.Application.csproj", "src/SmartEventBooking.Application/"]
COPY ["src/SmartEventBooking.Domain/SmartEventBooking.Domain.csproj", "src/SmartEventBooking.Domain/"]
COPY ["src/SmartEventBooking.Infrastructure/SmartEventBooking.Infrastructure.csproj", "src/SmartEventBooking.Infrastructure/"]
COPY ["src/SmartEventBooking.Shared/SmartEventBooking.Shared.csproj", "src/SmartEventBooking.Shared/"]

RUN dotnet restore "src/SmartEventBooking.Web/SmartEventBooking.Web.csproj"

COPY . .
WORKDIR "/src/src/SmartEventBooking.Web"

RUN dotnet build "SmartEventBooking.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartEventBooking.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartEventBooking.Web.dll"]