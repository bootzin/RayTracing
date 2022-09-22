run-clean: cleanup restore build 

all : cleanup restore build

cleanup:
	dotnet clean RayTracing.csproj

restore:
	dotnet restore RayTracing.csproj

build:
	dotnet build RayTracing.csproj