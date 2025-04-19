using System.Net.Http.Json;
using MQTTnet;
using MQTTnet.Client;
using System.Text;

string CONSTANT_URI = "https://chatapp-api-jhon.azurewebsites.net";
string CONSTANT_SERVER_IP = "172.191.191.103";
Console.WriteLine("=== ChatApp CLI ===");

string? username = null;
string? jwt = null;

while (true)
{
    Console.Clear();
    Console.WriteLine("Bienvenido a ChatApp CLI");
    Console.WriteLine("1. Registrarse");
    Console.WriteLine("2. Iniciar sesión");
    Console.WriteLine("3. Salir");
    Console.Write("Selecciona una opción: ");
    var option = Console.ReadLine();

    switch (option)
    {
        case "1":
            await RegisterUser();
            break;
        case "2":
            (username, jwt) = await LoginUser();
            if (jwt != null)
            {
                await RunMqttClient(username!, jwt);
            }
            break;
        case "3":
            return;
        default:
            Console.WriteLine("Opción inválida. Presiona una tecla para continuar...");
            Console.ReadKey();
            break;
    }
}

async Task RegisterUser()
{
    Console.Clear();
    Console.WriteLine("=== Registro ===");
    Console.Write("Nombre de usuario: ");
    var username = Console.ReadLine();
    Console.Write("Contraseña: ");
    var password = Console.ReadLine();

    var http = new HttpClient();
    var response = await http.PostAsJsonAsync(CONSTANT_URI+"/auth/register", new
    {
        Username = username,
        Password = password
    });

    var msg = response.IsSuccessStatusCode ? "Registro exitoso" : "Error al registrar";
    Console.WriteLine(msg);
    Console.WriteLine("Presiona cualquier tecla para continuar...");
    Console.ReadKey();
}

async Task<(string?, string?)> LoginUser()
{
    Console.Clear();
    Console.WriteLine("=== Iniciar Sesión ===");
    Console.Write("Nombre de usuario: ");
    var username = Console.ReadLine();
    Console.Write("Contraseña: ");
    var password = Console.ReadLine();

    var http = new HttpClient();
    var response = await http.PostAsJsonAsync(CONSTANT_URI+"/auth/login", new
    {
        Username = username,
        Password = password
    });

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine("Credenciales inválidas. Presiona una tecla para continuar...");
        Console.ReadKey();
        return (null, null);
    }

    var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
    Console.WriteLine("Inicio de sesión exitoso.");
    Console.WriteLine($"Username: {username}");
    Console.WriteLine("Presiona cualquier tecla para conectarte al chat...");
    Console.ReadKey();
    return (username, json["token"]);
}

async Task RunMqttClient(string username, string jwt)
{
    var http = new HttpClient();
    http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

    while (true)
    {
        Console.Clear();
        Console.WriteLine($"=== Bienvenido {username} ===");
        Console.WriteLine("1. Ver todas las salas");
        Console.WriteLine("2. Unirse a una sala");
        Console.WriteLine("3. Crear una sala");
        Console.WriteLine("0. Volver atrás");
        Console.Write("Selecciona una opción: ");
        var opt = Console.ReadLine();

        switch (opt)
        {
            case "1":
                var rooms = await http.GetFromJsonAsync<List<string>>(CONSTANT_URI+"/api/room/all");
                Console.WriteLine("Salas disponibles:");
                rooms?.ForEach(r => Console.WriteLine($" - {r}"));
                break;

            case "2":
                Console.Write("Nombre de la sala a unirse: ");
                var salaUnirse = Console.ReadLine();
                var resJoin = await http.PostAsJsonAsync(CONSTANT_URI+"/api/room/join", new { roomName = salaUnirse });

                if (resJoin.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Unido a la sala '{salaUnirse}'");
                    Console.WriteLine("Conectando a MQTT...");
                    await IniciarChatMqtt(jwt, salaUnirse!);
                }
                else
                {
                    Console.WriteLine("No se pudo unir a la sala.");
                }
                break;

            case "3":
                Console.Write("Nombre de la nueva sala: ");
                var salaCrear = Console.ReadLine();
                var resCrear = await http.PostAsJsonAsync(CONSTANT_URI+"/api/room/create", new { roomName = salaCrear });

                Console.WriteLine(resCrear.IsSuccessStatusCode ? "Sala creada ✅" : "Error: la sala ya existe ❌");
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Opción inválida.");
                break;
        }

        Console.WriteLine("\nPresiona una tecla para continuar...");
        Console.ReadKey();
    }
}

async Task IniciarChatMqtt(string jwt, string topic)
{
    var factory = new MqttFactory();
    var client = factory.CreateMqttClient();

    var options = new MqttClientOptionsBuilder()
        .WithTcpServer(CONSTANT_SERVER_IP, 1883)
        .WithCredentials(jwt, "-")
        .WithClientId(Guid.NewGuid().ToString())
        .Build();

    await client.ConnectAsync(options);
    await client.SubscribeAsync(topic);

    Console.WriteLine($"✅ Conectado a la sala '{topic}' (escribe 'exit' para salir)");

    client.ApplicationMessageReceivedAsync += e =>
    {
        var msg = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        Console.WriteLine($"\n[{e.ApplicationMessage.Topic}] {msg}");
        return Task.CompletedTask;
    };

    while (true)
    {
        Console.Write("> ");
        var input = Console.ReadLine();
        if (input == "exit") break;

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(input)
            .Build();

        await client.PublishAsync(message);
    }

    await client.DisconnectAsync();
}
