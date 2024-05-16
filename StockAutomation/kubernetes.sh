podman login cerit.io
dotnet publish -c Release
podman build -t cerit.io/roman_alexander_mariancik/stock-automation-image -f StockAutomationWeb/Dockerfile .
podman push cerit.io/roman_alexander_mariancik/stock-automation-image:latest
