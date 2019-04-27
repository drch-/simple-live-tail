FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as build
WORKDIR /build
ADD src.csproj .
RUN dotnet restore
ADD . .
RUN dotnet publish -o out
RUN ls -R out


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app

COPY --from=build /build/out .

CMD ["dotnet", "src.dll"]
