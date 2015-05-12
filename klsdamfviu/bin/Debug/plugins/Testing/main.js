var latestVersion = 3;

AddHook("OnInitialize", function (args) {
    args.Port = 12345;
    args.WorldSeed = 26879;
});

AddHook("OnClientConnect", function (args) {
    var ip = args.IP;
});

AddHook("OnClientVersion", function (args) {
    Console.WriteLine(args.Client.IP + " version: " + args.Version + ".");
});

AddHook("OnClientJoin", function (args) {
    var client = args.Client;

    Console.WriteLine("Cliente #" + client.ID + ", " + client.Entity.Name + " Ha entrado");
    world.SendServerMessage(client.Entity.Name + " Ha entrado.");
});

AddHook("OnClientDisconnect", function (args) {
    var client = args.Client;
    var reason = args.Reason;
    
    Console.WriteLine("OnClientDisconnect");
    Console.WriteLine(client.Entity.Name + " Desconectado.");
    Console.WriteLine("Clientes : " + coob.Clients.Count);
    world.SendServerMessage(client.Entity.Name + " Desconectado. (" + reason + ")");
});

AddHook("OnEntityUpdate", function (args) {

});

AddHook("OnWorldUpdate", function (args)
{
    var dt = args.DeltaTime;
});

AddHook("OnChatMessage", function (args) {
    var client = args.Client;
    var message = args.Message;
   
    Console.WriteLine("<" + client.Entity.Name + "> " + message);

    var day = 1;
    var time = parseFloat(message);

    if (!isNaN(time)) {
        world.SetTime(day, time);
        world.SendServerMessage("Tiempo establecido a " + time + " horas.");
        args.Canceled = true;
    }
    else
    {
        if (message == "togglepvp")
        {
            client.PVP = !client.PVP;
            
            if (client.PVP)
                client.SendServerMessage("PVP esta activado.");
            else
                client.SendServerMessage("PVP esta desactivado.");
        }
    }
});

AddHook("OnQuit", function (args) {
    Console.WriteLine("OMG quiting");
});

AddHook("OnEntityAttacked", function (args) {
    var attacker = args.Attacker;
    var target = args.Target;
    
    if (!args.Killed)
        world.SendServerMessage(attacker.Name + " ataca a " + target.Name + ".");
    else
        world.SendServerMessage(attacker.Name + " asesino " + target.Name + ".");
});