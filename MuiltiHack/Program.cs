using MuiltiHack;
using Swed64;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;
using System.Runtime.InteropServices;

Swed swed = new Swed("cs2");
IntPtr client = swed.GetModuleBase("client.dll");
IntPtr engine = swed.GetModuleBase("engine2.dll");

Renderer renderer = new Renderer();
renderer.screenSize = new Vector2(
    swed.ReadInt(engine + Offsets.dwWindowWidth),
    swed.ReadInt(engine + Offsets.dwWindowHeight)
);

// Запуск рендерера в отдельной задаче
Task.Run(renderer.Start);

// Словарь для управления функциями
var features = new Dictionary<string, FeatureController>();

// Инициализация контроллеров функций
features.Add("AntiFlash", new FeatureController());
features.Add("Bhop", new FeatureController());
features.Add("Radar", new FeatureController());
features.Add("Trigger", new FeatureController());
features.Add("BombTimer", new FeatureController());
features.Add("ESP", new FeatureController());
features.Add("AimBot", new FeatureController());
features.Add("RCS", new FeatureController());
features.Add("FOV", new FeatureController());
features.Add("AutoPistol", new FeatureController());
features.Add("AntiAim", new FeatureController());
features.Add("Inspect", new FeatureController());

while (true)
{
    // Кешируем часто используемые указатели только при необходимости
    IntPtr localPlayerPawn = IntPtr.Zero;
    IntPtr entityList = IntPtr.Zero;
    IntPtr listEntry = IntPtr.Zero;
    IntPtr gameRules = IntPtr.Zero;

    // Обновление состояний функций
    UpdateFeature("AntiFlash", () => Functions.AntiFlash(swed, client, features["AntiFlash"].Token));
    UpdateFeature("Bhop", () => Functions.Bhop(swed, client, features["Bhop"].Token, renderer));

    if (NeedPointerUpdate("Radar") || NeedPointerUpdate("ESP") || NeedPointerUpdate("AimBot"))
    {
        entityList = swed.ReadPointer(client, Offsets.dwEntityList);
        listEntry = swed.ReadPointer(entityList, 0x10);
    }

    UpdateFeature("Radar", () => Functions.Radar(swed, entityList, listEntry, features["Radar"].Token));

    if (NeedPointerUpdate("Trigger") || NeedPointerUpdate("AimBot"))
    {
        localPlayerPawn = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
    }

    UpdateFeature("Trigger", () => Functions.Trigger(
        swed, client, entityList, localPlayerPawn, features["Trigger"].Token,
        renderer.millisecondsDelay, renderer.autoTrigger, renderer.autoShoot,
        renderer.legitTrigger, renderer.aimOnTeam
    ));

    if (NeedPointerUpdate("BombTimer"))
    {
        gameRules = swed.ReadPointer(client, Offsets.dwGameRules);
    }

    UpdateFeature("BombTimer", () => Functions.BombTimer(swed, gameRules, features["BombTimer"].Token, renderer));
    UpdateFeature("ESP", () => Functions.ESP(swed, client, entityList, listEntry, renderer, features["ESP"].Token));
    UpdateFeature("AimBot", () => Functions.AimBot(swed, client, entityList, localPlayerPawn, listEntry, renderer, features["AimBot"].Token));
    UpdateFeature("RCS", () => Functions.RCS(swed, client, renderer, features["RCS"].Token));
    UpdateFeature("FOV", () => Functions.fovChanger(swed, client, renderer, features["FOV"].Token));
    UpdateFeature("AutoPistol", () => Functions.AutoPistolShoting(swed, client, features["AutoPistol"].Token, renderer));
    UpdateFeature("AntiAim", () => Functions.AntiAim(swed, client, features["AntiAim"].Token, renderer));
    UpdateFeature("Inspect", () => Functions.InfiniteInspect(swed, client, features["Inspect"].Token, renderer));

    Thread.Sleep(50); // Оптимизированная задержка
}

// Вспомогательные методы
void UpdateFeature(string featureName, Action action)
{
    var controller = features[featureName];
    bool isEnabled = GetFeatureState(featureName);

    if (isEnabled)
    {
        if (controller.ShouldStart())
        {
            controller.Start(action);
        }
    }
    else if (controller.ShouldStop())
    {
        controller.Stop();
    }
}

bool GetFeatureState(string featureName) => featureName switch
{
    "AntiFlash" => renderer.antiflash,
    "Bhop" => renderer.bhop,
    "Radar" => renderer.radar,
    "Trigger" => renderer.trigger,
    "BombTimer" => renderer.bombTimer,
    "ESP" => renderer.enableEsp,
    "AimBot" => renderer.aimbot,
    "RCS" => renderer.recoitTrace,
    "FOV" => renderer.enableFovChanger,
    "AutoPistol" => renderer.autoPistol,
    "AntiAim" => renderer.antiAim,
    "Inspect" => renderer.inspect,
    _ => false
};

bool NeedPointerUpdate(string featureName) => featureName switch
{
    "Radar" => renderer.radar,
    "Trigger" => renderer.trigger,
    "BombTimer" => renderer.bombTimer,
    "ESP" => renderer.enableEsp,
    "AimBot" => renderer.aimbot,
    _ => false
};

[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int vKey);

// Класс для управления функциями
class FeatureController
{
    private CancellationTokenSource _cts;
    private Task _task;

    public CancellationToken Token => _cts?.Token ?? default;

    public bool ShouldStart() => _task == null || _task.IsCompleted;

    public bool ShouldStop() => _task != null && !_task.IsCompleted;

    public void Start(Action action)
    {
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        _task = Task.Run(action, _cts.Token);
    }

    public void Stop()
    {
        _cts?.Cancel();
        _task = null;
    }
}

