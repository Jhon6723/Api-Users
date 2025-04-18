
# 🧠 ChatApp API - Proyecto Capstone

Esta es una **API en C# con .NET 9** desarrollada como parte del proyecto de Redes Computacionales 2. El sistema permite **registro, autenticación JWT**, gestión de **salas de chat**, integración con **MQTT (Mosquitto)** y está estructurado con pruebas unitarias.

---

## 📦 Tecnologías usadas

- ASP.NET Core 9
- Entity Framework Core + PostgreSQL
- AutoMapper
- JWT (Json Web Token)
- Swagger (OpenAPI)
- xUnit + Moq (para tests)
- MQTTnet (cliente externo)
- Mosquitto + auth plugin (Broker MQTT)

---

## ⚙️ Funcionalidades principales

### 🔐 Autenticación y registro
- `POST /auth/register`: Crea un nuevo usuario
- `POST /auth/login`: Devuelve un JWT si las credenciales son válidas

### 🔐 Seguridad
- Rutas protegidas con `[Authorize]` y JWT
- Swagger permite autenticación con token para probar endpoints protegidos

### 🏠 Salas de chat
- `POST /room/create`: Crea una nueva sala
- `POST /room/join`: Verifica si la sala existe y permite "entrar"
- `GET /room/all`: Lista todas las salas disponibles

### 🧪 Tests
- Pruebas unitarias en `ChatApp.Tests` usando xUnit y EF InMemory
- Cubren `AuthService` y `RoomService`

---

## 🚀 Cómo correr el proyecto

### 1. Clona el repositorio

```bash
git clone https://github.com/Jhon6723/Api-Users.git
cd Api-Users
```
## 2. Configura appsettings.json
Agrega tu cadena de conexión a PostgreSQL y JWT:

``` json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=chatappdb;Username=postgres;Password=tu_clave"
},
"Jwt": {
  "Key": "CLAVE_SUPER_SEGURA_DE_32+_CARACTERES",
  "Issuer": "ChatApp"
}
```
## 3. Crear las migraciones
``` bash
dotnet ef migrations add initialCreate
```
## 4. Actualizar tu base de datos:
``` bash
dotnet ef database update 
```
> Si no tienes una base de datos puedes crearla con docker con este comando:
``` bash
docker run -d --name chatapp-postgres -e POSTGRES_USER=root -e POSTGRES_PASSWORD=rzh2025 -e POSTGRES_DB=chatappdb -p 50000:5432 -v chatapp_pgdata:/var/lib/postgresql/data postgres
```

## 4. Ejecuta el servidor
``` bash
dotnet run
```

