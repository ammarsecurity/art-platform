# Art Platform API — Linux (net10.0)
# من مجلد backend/:  docker build -t artplatform-api .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# طبقة كاش للاستعادة
COPY ArtPlatform.sln ./
COPY ArtPlatform.Domain/ArtPlatform.Domain.csproj ArtPlatform.Domain/
COPY ArtPlatform.Application/ArtPlatform.Application.csproj ArtPlatform.Application/
COPY ArtPlatform.Infrastructure/ArtPlatform.Infrastructure.csproj ArtPlatform.Infrastructure/
COPY ArtPlatform.API/ArtPlatform.API.csproj ArtPlatform.API/

RUN dotnet restore ArtPlatform.sln

# المصدر الكامل (يُستبعد obj/bin عبر .dockerignore)
COPY ArtPlatform.Domain/ ArtPlatform.Domain/
COPY ArtPlatform.Application/ ArtPlatform.Application/
COPY ArtPlatform.Infrastructure/ ArtPlatform.Infrastructure/
COPY ArtPlatform.API/ ArtPlatform.API/

# إن وصلت مجلدات obj/bin من جهاز ويندوز (تتجاهل أحياناً)، احذفها ثم أعد الاستعادة داخل لينكس
RUN rm -rf \
    ArtPlatform.API/obj ArtPlatform.API/bin \
    ArtPlatform.Application/obj ArtPlatform.Application/bin \
    ArtPlatform.Domain/obj ArtPlatform.Domain/bin \
    ArtPlatform.Infrastructure/obj ArtPlatform.Infrastructure/bin \
  && dotnet restore ArtPlatform.sln

RUN dotnet publish ArtPlatform.API/ArtPlatform.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:5567
EXPOSE 5567

ENTRYPOINT ["dotnet", "ArtPlatform.API.dll"]
