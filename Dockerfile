FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

WORKDIR /app
COPY . ./
RUN dotnet publish ./src/LatexTemplater/LatexTemplater.csproj -c Release -o out

LABEL maintainer="Scott Merchant <scottyjoe9@gmail.com>"
LABEL repository="https://github.com/scme0/tmpltr"
LABEL homepage="https://github.com/scme0/tmpltr"

# Gihub Action labels
LABEL com.github.actions.name="tmpltr"
# Limit to 160 characters
LABEL com.github.actions.description="A simple generic templating engine"
# See branding:
# https://docs.github.com/actions/creating-actions/metadata-syntax-for-github-actions#branding
LABEL com.github.actions.icon="edit"
LABEL com.github.actions.color="violet"

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-noble-chiseled-extra
COPY --from=build-env /app/out app
ENTRYPOINT [ "/app/LatexTemplater" ]
