

# Make sure all variables from .env are exported
set -a
source .env
set +a

# Ensure dotnet-ef is installed
dotnet tool install -g dotnet-ef

# Scaffold EF Core entities
dotnet ef dbcontext scaffold "$CONN_STR" Npgsql.EntityFrameworkCore.PostgreSQL \
    --context MyDbContext \
    --no-onconfiguring \
    --schema library \
    --force \
    --output-dir ./Entities \
    --context-dir .
