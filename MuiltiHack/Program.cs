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

// Токены для управления задачами
CancellationTokenSource cancelTokenSourceAntiFlash = new CancellationTokenSource();
CancellationToken tokenAntiFlash = cancelTokenSourceAntiFlash.Token;

CancellationTokenSource cancelTokenSourceBhop = new CancellationTokenSource();
CancellationToken tokenBhop = cancelTokenSourceBhop.Token;

CancellationTokenSource cancelTokenSourceRadar = new CancellationTokenSource();
CancellationToken tokenRadar = cancelTokenSourceRadar.Token;

CancellationTokenSource cancelTokenSourceTrigger = new CancellationTokenSource();
CancellationToken tokenTrigger = cancelTokenSourceRadar.Token;

CancellationTokenSource cancelTokenSourceFov = new CancellationTokenSource();
CancellationToken tokenFov = cancelTokenSourceRadar.Token;

CancellationTokenSource cancelTokenSourceBombTimer = new CancellationTokenSource();
CancellationToken tokenBombTimer = cancelTokenSourceBombTimer.Token;

CancellationTokenSource cancelTokenSourceESP = new CancellationTokenSource();
CancellationToken tokenESP = cancelTokenSourceESP.Token;

// Инициализация задач
Task antiFlash = null;
Task bhop = null;
Task radar = null;
Task trigger = null;
Task ESP = null;

Task bombTimer = null;

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
            antiFlash = new Task(() => Functions.AntiFlash(swed, localPlayerPawn, tokenAntiFlash));
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
            bhop = new Task(() => Functions.Bhop(swed, client, localPlayerPawn, tokenBhop, renderer));
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
            trigger = new Task(() => Functions.Trigger(swed, client,entityList,localPlayerPawn, tokenTrigger, renderer.millisecondsDelay, renderer.autoTrigger));
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
            ESP = new Task(() => Functions.ESP(swed, client, entityList, localPlayerPawn, listentry, renderer, tokenESP));
            ESP.Start();
        }
    }
    else if(ESP != null && ESP.Status == TaskStatus.Running && !renderer.enableEsp)
    {
        cancelTokenSourceESP.Cancel();
        ESP = null;
    } 

    Thread.Sleep(10); // Небольшая задержка, чтобы не нагружать процессор
}

//imports
[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int vKey);