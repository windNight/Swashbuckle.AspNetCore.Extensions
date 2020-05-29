#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
ENV TZ=Asia/Shanghai
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["examples/SwaggerDemo_NetCore2_2/SwaggerDemo_NetCore2_2.csproj", "examples/SwaggerDemo_NetCore2_2/"]
RUN dotnet restore "examples/SwaggerDemo_NetCore2_2/SwaggerDemo_NetCore2_2.csproj"
COPY . .
WORKDIR "/src/examples/SwaggerDemo_NetCore2_2"
RUN dotnet build "SwaggerDemo_NetCore2_2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SwaggerDemo_NetCore2_2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SwaggerDemo_NetCore2_2.dll"]