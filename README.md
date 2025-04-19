# 🧠 ChatApp API - Proyecto Capstone

Esta es una **API en C# con .NET 9** desarrollada como parte del proyecto de Redes Computacionales 2. El sistema permite **registro, autenticación JWT**, gestión de **salas de chat**, integración con **MQTT (Mosquitto)** usando el plugin **mosquitto-go-auth**, y validación remota de tokens JWT contra esta misma API. Además, incluye pruebas unitarias.

---

## 📦 Tecnologías usadas

- ASP.NET Core 9
- Entity Framework Core + PostgreSQL
- AutoMapper
- JWT (Json Web Token)
- Swagger (OpenAPI)
- xUnit + Moq (para tests)
- MQTTnet (cliente CLI)
- Mosquitto + `go-auth` plugin (Broker MQTT con backend `jwt` modo `remote`)

---

## ⚙️ Funcionalidades principales

### 🔐 Autenticación y registro
- `POST /auth/register`: Crea un nuevo usuario
- `POST /auth/login`: Devuelve un JWT si las credenciales son válidas
- `POST /auth`: Endpoint especial que valida JWT enviado por Mosquitto (a través de `Authorization: Bearer <token>`)
- `POST /auth/superuser`: Devuelve "deny" para todos (no hay superusuarios por defecto)
- `POST /auth/acl`: Devuelve "ok" para permitir acceso a cualquier sala

### 🔐 Seguridad
- Rutas protegidas con `[Authorize]` y validación de tokens JWT
- Swagger permite autenticación con token para probar endpoints protegidos
- Mosquitto se comunica con la API para autenticar y autorizar clientes MQTT de forma remota (mode remote)

### 🏠 Salas de chat
- `POST /room/create`: Crea una nueva sala
- `POST /room/join`: Verifica si la sala existe y permite "entrar"
- `GET /room/all`: Lista todas las salas disponibles

### 💬 Cliente MQTT CLI
- Cliente MQTT escrito en C# que permite registrarse, iniciar sesión, unirse a salas y enviar mensajes
- Utiliza el JWT como "username" y se conecta al broker Mosquitto, el cual consulta la API para autorizar

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

### 2. Configura `appsettings.json`
Agrega tu cadena de conexión a PostgreSQL y JWT:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=chatappdb;Username=postgres;Password=tu_clave"
},
"Jwt": {
  "Key": "CLAVE_SUPER_SEGURA_DE_32+_CARACTERES",
  "Issuer": "ChatApp"
}
```

### 3. Crear las migraciones

```bash
dotnet ef migrations add initialCreate
```

### 4. Actualizar tu base de datos

```bash
dotnet ef database update
```

> Si no tienes una base de datos puedes crearla con Docker:

```bash
docker run -d --name chatapp-postgres \
  -e POSTGRES_USER=root \
  -e POSTGRES_PASSWORD=rzh2025 \
  -e POSTGRES_DB=chatappdb \
  -p 50000:5432 \
  -v chatapp_pgdata:/var/lib/postgresql/data \
  postgres
```

### 5. Ejecuta el servidor

```bash
dotnet run
```

---

## 📡 Configuración Mosquitto

1. Instala Mosquitto y el plugin `mosquitto-go-auth`
2. Configura el archivo `go-auth.conf`:

```ini
listener 1883 0.0.0.0
auth_plugin ruta/del/plugin
go-auth.so
auth_opt_backends jwt
auth_opt_jwt_mode remote
auth_opt_jwt_host localhost
auth_opt_jwt_port 5052
auth_opt_jwt_getuser_uri /auth
auth_opt_jwt_superuser_uri /auth/superuser
auth_opt_jwt_aclcheck_uri /auth/acl
auth_opt_jwt_http_method POST
auth_opt_jwt_params_mode form
auth_opt_jwt_response_mode status
auth_opt_log_level debug 
```

3. Incluye este archivo en tu `mosquitto.conf`:

```ini
include_dir /etc/mosquitto/conf.d
```

4. Reinicia Mosquitto:

```bash
sudo systemctl restart mosquitto
```

---

✅ El broker MQTT ahora delega completamente la autenticación y autorización a tu API usando JWT.

