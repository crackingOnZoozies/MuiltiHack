using MuiltiHack;
using Swed64;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

Swed swed = new Swed("cs2");
IntPtr client = swed.GetModuleBase("client.dll");

Renderer renderer = new Renderer();
renderer.Start().Wait();

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

// Инициализация задач
Task antiFlash = null;
Task bhop = null;
Task radar = null;
Task trigger = null;
Task FOV = null;

Task bombTimer = null;

while (true)
{
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
            bhop = new Task(() => Functions.Bhop(swed, client, tokenBhop, renderer));
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
            radar = new Task(() => Functions.Radar(swed, client, tokenRadar));
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
            trigger = new Task(() => Functions.Trigger(swed, client, tokenTrigger, renderer.millisecondsDelay, renderer.autoTrigger));
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
            bombTimer = new Task(() => Functions.BombTimer(swed, client, tokenBombTimer, renderer));
            bombTimer.Start();
        }
    }
    else if (bombTimer != null && bombTimer.Status == TaskStatus.Running && !renderer.bombTimer)
    {
        cancelTokenSourceBombTimer.Cancel();
        bombTimer = null;
    }

    Thread.Sleep(10); // Небольшая задержка, чтобы не нагружать процессор
}

//imports
[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int vKey);