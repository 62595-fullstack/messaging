# Start Database up

1. Secrets
1.1 DotNET user-secrets
```bash
dotnet user-secrets set host localhost --project messaging
dotnet user-secrets set username postgres --project messaging
dotnet user-secrets set password facebook --project messaging
dotnet user-secrets set database Messaging --project messaging
dotnet user-secrets set port 5432 --project messaging
dotnet user-secrets set programPort 5500 --project messaging
```

1.2 Environment variables
Linux
```bash
export host=localhost port=5432 username=postgres password=facebook database=Messaging programport=5500
```
Windows
```bash
set host=localhost port=5432 username=postgres password=facebook database=Messaging programport=5500
```

1.3 Certification
Generate cert.pfx file in root
```bash
dotnet dev-certs https --format pfx --export-path cert.pfx -p testpassword
```
Trust certificate
```bash
dotnet dev-certs https --trust
```
Export environment variable path to trusted certifications
```bash
export SSL_CERT_DIR="$HOME/.aspnet/dev-certs/trust:/etc/ssl/certs"
```

4. Update the database tables:
```bash
dotnet ef database update
```

5. To run in development set the ASPNETCORE_ENVIRONMENT environment variable to Development or set it for the duration of the program running like so:
```bash
dotnet run -e ASPNETCORE_ENVIRONMENT=development --project messaging
```

