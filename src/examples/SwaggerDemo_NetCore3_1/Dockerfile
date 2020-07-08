#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
ENV TZ=Asia/Shanghai
WORKDIR /app
EXPOSE 1031

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["examples/SwaggerDemo_NetCore3_1/SwaggerDemo_NetCore3_1.csproj", "examples/SwaggerDemo_NetCore3_1/"]
RUN dotnet restore "examples/SwaggerDemo_NetCore3_1/SwaggerDemo_NetCore3_1.csproj"
COPY . .
WORKDIR "/src/examples/SwaggerDemo_NetCore3_1"
RUN dotnet build "SwaggerDemo_NetCore3_1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SwaggerDemo_NetCore3_1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SwaggerDemo_NetCore3_1.dll"]