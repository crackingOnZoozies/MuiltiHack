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

// Инициализация задач
Task antiFlash = null;
Task bhop = null;
Task radar = null;
Task trigger = null;    

while (true)
{
    // Управление AntiFlash
    if (renderer.antiflash)
    {
        if (antiFlash == null || antiFlash.Status != TaskStatus.Running)
        {
            // Если задача не запущена или завершена, создаем и запускаем новую
            cancelTokenSourceAntiFlash = new CancellationTokenSource(); // Создаем новый CancellationTokenSource
            tokenAntiFlash = cancelTokenSourceAntiFlash.Token; // Обновляем токен
            antiFlash = new Task(() => Functions.AntiFlash(swed, client, tokenAntiFlash));
            antiFlash.Start();
        }
    }
    else if (antiFlash != null && antiFlash.Status == TaskStatus.Running && !renderer.antiflash)
    {
        // Если antiflash выключен, но задача все еще работает, отменяем ее
        cancelTokenSourceAntiFlash.Cancel();
        antiFlash = null; // Сбрасываем задачу, чтобы можно было создать новую при следующем включении
    }

    // Управление BHop
    if (renderer.bhop)
    {
        if (bhop == null || bhop.Status != TaskStatus.Running)
        {
            // Если задача не запущена или завершена, создаем и запускаем новую
            cancelTokenSourceBhop = new CancellationTokenSource(); // Создаем новый CancellationTokenSource
            tokenBhop = cancelTokenSourceBhop.Token; // Обновляем токен
            bhop = new Task(() => Functions.Bhop(swed, client, tokenBhop, renderer));
            bhop.Start();
        }
    }
    else if (bhop != null && bhop.Status == TaskStatus.Running && !renderer.bhop)
    {
        // Если bhop выключен, но задача все еще работает, отменяем ее
        cancelTokenSourceBhop.Cancel();
        bhop = null; // Сбрасываем задачу, чтобы можно было создать новую при следующем включении
    }

    // Управление Radar
    if (renderer.radar)
    {
        if (radar == null || radar.Status != TaskStatus.Running)
        {
            // Если задача не запущена или завершена, создаем и запускаем новую
            cancelTokenSourceRadar = new CancellationTokenSource(); // Создаем новый CancellationTokenSource
            tokenRadar = cancelTokenSourceRadar.Token; // Обновляем токен
            radar = new Task(() => Functions.Radar(swed, client, tokenRadar));
            radar.Start();
        }
    }
    else if (radar != null && radar.Status == TaskStatus.Running && !renderer.radar)
    {
        // Если radar выключен, но задача все еще работает, отменяем ее
        cancelTokenSourceRadar.Cancel();
        radar = null; // Сбрасываем задачу, чтобы можно было создать новую при следующем включении
    }

    // Добавление триггера
    if (renderer.trigger)
    {
        if (trigger == null || trigger.Status != TaskStatus.Running)
        {
            // Если задача не запущена или завершена, создаем и запускаем новую
            cancelTokenSourceTrigger = new CancellationTokenSource(); // Создаем новый CancellationTokenSource
            tokenTrigger = cancelTokenSourceTrigger.Token; // Обновляем токен
            trigger = new Task(() => Functions.Trigger(swed, client, tokenTrigger, renderer.millisecondsDelay,renderer.autoTrigger));
            trigger.Start();
        }
    }
    else if (trigger != null && trigger.Status == TaskStatus.Running && !renderer.trigger)
    {
        // Если триггер выключен, но задача все еще работает, отменяем ее
        cancelTokenSourceTrigger.Cancel();
        trigger = null; // Сбрасываем задачу, чтобы можно было создать новую при следующем включении
    }


    Thread.Sleep(10); // Небольшая задержка, чтобы не нагружать процессор
}

//imports
[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int vKey);