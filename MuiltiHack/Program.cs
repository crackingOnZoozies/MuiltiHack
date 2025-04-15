using MuiltiHack;
using Swed64;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;

Swed swed = new Swed("cs2");
IntPtr client = swed.GetModuleBase("client.dll");
IntPtr engine = swed.GetModuleBase("engine2.dll");

Renderer renderer = new Renderer();
renderer.screenSize = new Vector2(swed.ReadInt(engine + Offsets.dwWindowWidth), swed.ReadInt(engine + Offsets.dwWindowHeight));


Thread rendererThread = new Thread(new ThreadStart(renderer.Start().Wait));
rendererThread.Start();

//Task FovChanger = new Task(() => Functions.fovChanger(swed, client, renderer));
//FovChanger.Start();

// Токены для управления задачами


CancellationTokenSource cancelTokenSourceAntiFlash = new CancellationTokenSource();
CancellationToken tokenAntiFlash = cancelTokenSourceAntiFlash.Token;

CancellationTokenSource cancelTokenSourceBhop = new CancellationTokenSource();
CancellationToken tokenBhop = cancelTokenSourceBhop.Token;

CancellationTokenSource cancelTokenSourceRadar = new CancellationTokenSource();
CancellationToken tokenRadar = cancelTokenSourceRadar.Token;

CancellationTokenSource cancelTokenSourceTrigger = new CancellationTokenSource();
CancellationToken tokenTrigger = cancelTokenSourceRadar.Token;


CancellationTokenSource cancelTokenSourceBombTimer = new CancellationTokenSource();
CancellationToken tokenBombTimer = cancelTokenSourceBombTimer.Token;

CancellationTokenSource cancelTokenSourceESP = new CancellationTokenSource();
CancellationToken tokenESP = cancelTokenSourceESP.Token;

CancellationTokenSource cancelTokenSourceAimBot = new CancellationTokenSource();
CancellationToken tokenAimBot = cancelTokenSourceAimBot.Token;

CancellationTokenSource cancelTokenSourceRCS = new CancellationTokenSource();
CancellationToken tokenRCS = cancelTokenSourceRCS.Token;

CancellationTokenSource cancelTokenSourceFOV = new CancellationTokenSource();
CancellationToken tokenFOV = cancelTokenSourceFOV.Token;

CancellationTokenSource cancelTokenSourceAutoPistol = new CancellationTokenSource();
CancellationToken tokenAutoPistol = cancelTokenSourceAutoPistol.Token;

// Инициализация задач
Task antiFlash = null;
Task bhop = null;
Task radar = null;
Task trigger = null;
Task ESP = null;
Task AimBot = null;
Task RCS = null;    
Task bombTimer = null;
Task FOV = null;
Task autoPistol = null;

while (true)
{
    IntPtr localPlayerPawn = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);

    IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);

    IntPtr listentry = swed.ReadPointer(entityList, 0x10);

    IntPtr GameRules = swed.ReadPointer(client, Offsets.dwGameRules);

    // Управление AntiFlash
    if (renderer.antiflash)
    {
        if (antiFlash == null || antiFlash.Status != TaskStatus.Running)
        {
            cancelTokenSourceAntiFlash = new CancellationTokenSource();
            tokenAntiFlash = cancelTokenSourceAntiFlash.Token;
            antiFlash = new Task(() => Functions.AntiFlash(swed, client, tokenAntiFlash));
            antiFlash.Start();
        }
    }
    else if (antiFlash != null && antiFlash.Status == TaskStatus.Running && !renderer.antiflash)
    {
        cancelTokenSourceAntiFlash.Cancel();
        antiFlash = null;
    }

    // Управление BHop
    if (renderer.bhop)
    {
        if (bhop == null || bhop.Status != TaskStatus.Running)
        {
            cancelTokenSourceBhop = new CancellationTokenSource();
            tokenBhop = cancelTokenSourceBhop.Token;
            bhop = new Task(() => Functions.Bhop(swed, client,  tokenBhop, renderer));
            bhop.Start();
        }
    }
    else if (bhop != null && bhop.Status == TaskStatus.Running && !renderer.bhop)
    {
        cancelTokenSourceBhop.Cancel();
        bhop = null;
    }

    // Управление Radar
    if (renderer.radar)
    {
        if (radar == null || radar.Status != TaskStatus.Running)
        {
            cancelTokenSourceRadar = new CancellationTokenSource();
            tokenRadar = cancelTokenSourceRadar.Token;
            radar = new Task(() => Functions.Radar(swed, entityList, listentry, tokenRadar));
            radar.Start();
        }
    }
    else if (radar != null && radar.Status == TaskStatus.Running && !renderer.radar)
    {
        cancelTokenSourceRadar.Cancel();
        radar = null;
    }

    // Управление Trigger
    if (renderer.trigger)
    {
        if (trigger == null || trigger.Status != TaskStatus.Running)
        {
            cancelTokenSourceTrigger = new CancellationTokenSource();
            tokenTrigger = cancelTokenSourceTrigger.Token;
            trigger = new Task(() => Functions.Trigger(swed, client,entityList,localPlayerPawn, tokenTrigger, renderer.millisecondsDelay, renderer.autoTrigger, renderer.autoShoot,renderer.legitTrigger,renderer.aimOnTeam));
            trigger.Start();
        }
    }
    else if (trigger != null && trigger.Status == TaskStatus.Running && !renderer.trigger)
    {
        cancelTokenSourceTrigger.Cancel();
        trigger = null;
    }

    // Управление BombTimer
    if (renderer.bombTimer)
    {
        if (bombTimer == null || bombTimer.Status != TaskStatus.Running)
        {
            cancelTokenSourceBombTimer = new CancellationTokenSource();
            tokenBombTimer = cancelTokenSourceBombTimer.Token;
            bombTimer = new Task(() => Functions.BombTimer(swed, GameRules, tokenBombTimer, renderer));
            bombTimer.Start();
        }
    }
    else if (bombTimer != null && bombTimer.Status == TaskStatus.Running && !renderer.bombTimer)
    {
        cancelTokenSourceBombTimer.Cancel();
        bombTimer = null;
    }

    if(renderer.enableEsp)
    {
        if(ESP == null || ESP.Status != TaskStatus.Running)
        {
            cancelTokenSourceESP = new CancellationTokenSource();
            tokenESP = cancelTokenSourceESP.Token;
            ESP = new Task(() => Functions.ESP(swed, client, entityList, listentry, renderer, tokenESP));
            ESP.Start();
        }
    }
    else if(ESP != null && ESP.Status == TaskStatus.Running && !renderer.enableEsp)
    {
        cancelTokenSourceESP.Cancel();
        ESP = null;
    }

    if (renderer.aimbot)
    {
        if(AimBot == null || AimBot.Status != TaskStatus.Running)
        {
            cancelTokenSourceAimBot = new CancellationTokenSource();
            tokenAimBot = cancelTokenSourceAimBot.Token;
            AimBot = new Task(() => Functions.AimBot(swed, client, entityList, localPlayerPawn, listentry, renderer, tokenESP));
            AimBot.Start();
        }
    }
    else if (AimBot != null && AimBot.Status == TaskStatus.Running && !renderer.aimbot)
    {
        cancelTokenSourceAimBot?.Cancel();
        AimBot = null;
    }

    if(renderer.recoitTrace)
    {
        if(RCS==null || RCS.Status!= TaskStatus.Running)
        {
            cancelTokenSourceRCS = new CancellationTokenSource();
            tokenRCS = cancelTokenSourceRCS.Token;
            RCS = new Task(() => Functions.RCS(swed, client,tokenRCS));
            RCS.Start();
        }
    }
    else if (RCS!=null && RCS.Status==TaskStatus.Running && !renderer.recoitTrace)
    {
        cancelTokenSourceRCS?.Cancel();
        RCS = null;
    }

    if (renderer.enableFovChanger)
    {
        if(FOV==null || FOV.Status != TaskStatus.Running)
        {
            cancelTokenSourceFOV = new CancellationTokenSource();
            tokenFOV = cancelTokenSourceFOV.Token;
            FOV = new Task(() => Functions.fovChanger(swed, client, renderer,tokenFOV));
            FOV.Start();
        }
    }
    else if(FOV!=null && FOV.Status == TaskStatus.Running && !renderer.enableFovChanger)
    {
        cancelTokenSourceFOV?.Cancel();
        FOV = null;
    }

    if (renderer.autoPistol)
    {
        if(autoPistol==null || autoPistol.Status != TaskStatus.Running)
        {
            cancelTokenSourceAutoPistol = new CancellationTokenSource();
            tokenAutoPistol = cancelTokenSourceAutoPistol.Token;
            autoPistol = new Task( ()=>Functions.AutoPistolShoting(swed, client, tokenAutoPistol,renderer));
            autoPistol.Start();
        }
    }
    else if(autoPistol!=null && autoPistol.Status==TaskStatus.Running && !renderer.autoPistol)
    {
        cancelTokenSourceAutoPistol?.Cancel();
        autoPistol = null;
    }


    Thread.Sleep(10); // Небольшая задержка, чтобы не нагружать процессор
}

//imports
[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int vKey);