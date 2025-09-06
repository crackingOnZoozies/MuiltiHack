using ClickableTransparentOverlay;
using ImGuiNET;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MuiltiHack
{
    public class Renderer : Overlay
    {
        // Конфигурация состояний
        public bool antiflash, radar, bhop, bombTimer, trigger, recoitTrace;
        public bool autoBhop, legit, legitTrigger, autoTrigger, autoShoot;
        public bool enableCustomSoundBombTimer, enableEsp, enableBones, enableName;
        public bool enableVisibilityCheck, weaponEsp, box, drawLine, showTeam;
        public bool showHelmet, showArmor, showIfHasKit, showIfDefusing, showAmmoInMag;
        public bool aimbot, aimOnTeam, aimOnSpotted, useFov, aimOnClosest;
        public bool followRecoil, legitAimBot, inVisFov, autoLock, aimKeySecond;
        public bool enableFovChanger, ignorescoping, autoPistol, antiAim;
        public bool stabilize, backOnly, ultraSpin, jitterMode, inspect, manual;
        public bool autoScopeTrigger, legitScopeTrigger;

        // Параметры
        public int cooldown, millisecondsDelay, aimDelay;
        public int maxRecoil = -1;
        public float yOffset, boneThickness, AutoSpotDist, maxEspDist;
        public float  soundVolume, FovChangerFOV, autoLockMaxDistance;
        public float FOV = 1;

        // Цвета
        private Vector4 redColor = new Vector4(1, 0, 0, 1);
        private Vector4 greenColor = new Vector4(0, 1, 0, 1);
        private Vector4 enemyColor = new Vector4(1, 0, 0, 1);
        private Vector4 teamColor = new Vector4(0, 1, 0, 1);
        private Vector4 barColor = new Vector4(0, 1, 0, 1);
        private Vector4 nameColor = new Vector4(1, 1, 1, 1);
        private Vector4 hiddenColor = new Vector4(0, 0, 0, 1);
        private Vector4 BoneColor = new Vector4(1, 0, 2, 1);
        private Vector4 armorColor = new Vector4(1, 1, 1, 1);
        private Vector4 circleColor = new Vector4(1, 0, 1, 1);

        // Состояние игры
        public bool bombPlanted;
        public int timeLeft = -1;
        public Vector2 screenSize = new Vector2(1920, 1080);

        // Данные сущностей
        private ConcurrentQueue<Entity> _entities = new ConcurrentQueue<Entity>();
        private Entity _localPlayer = new Entity();
        private readonly object _entityLock = new object();
        private Random _random = new Random();
        private string _soundsFolder = Path.Combine(AppContext.BaseDirectory, "EventSounds");

        // Кешированные делегаты для отрисовки
        private Action<Entity>[] _renderActions;
        private bool _espActive;

        // Состояние окон
        private bool _bombTimerWindowOpen = true;

        

        public Renderer()
        {
            
            InitializeRenderActions();
        }

        private void InitializeRenderActions()
        {
            _renderActions = new Action<Entity>[]
            {
                e => DrawHealthBarAndArmor(e),
                e => DrawBox(e),
                e => DrawLine(e),
                e => DrawNameAndWeapon(e),
                e => { if (enableBones && e.team != _localPlayer.team) DrawBones(e); }
            };
        }

        protected override void Render()
        {
            ImGui.Begin("multiCheat beta legit");
            RenderAntiAimSection();
            RenderFovChangerSection();
            RenderAutoPistolSection();
            RenderAntiFlashSection();
            RenderRadarSection();
            RenderBhopSection();
            RenderTriggerBotSection();
            RenderRecoilControlSection();
            RenderBombTimerSection();
            RenderAimbotSection();
            RenderEspSection();

            ImGui.End();
        }

        #region Section Rendering Methods
        private void RenderAntiAimSection()
        {
            if (ImGui.CollapsingHeader("Anti Aim"))
            {
                ImGui.Checkbox("Enable Anti Aim", ref antiAim);
                if (antiAim)
                {
                    ImGui.Checkbox("Stabilizer", ref stabilize);
                    ImGui.Checkbox("Back Only", ref backOnly);
                    ImGui.Checkbox("Ultra Spin", ref ultraSpin);
                    ImGui.Checkbox("Jitter Mode", ref jitterMode);
                }
            }
        }

        private void RenderFovChangerSection()
        {
            if (ImGui.CollapsingHeader("FOV Changer"))
            {
                ImGui.Checkbox("Enable FOV Changer", ref enableFovChanger);
                if (enableFovChanger)
                {
                    ImGui.Checkbox("Ignore Scope", ref ignorescoping);
                    ImGui.SliderFloat("FOV Value", ref FovChangerFOV, 0, 150);
                }
            }
        }

        private void RenderAutoPistolSection()
        {
            if (ImGui.CollapsingHeader("Auto Pistol"))
            {
                ImGui.Checkbox("Enable Auto Pistol", ref autoPistol);
            }
        }

        private void RenderAntiFlashSection()
        {
            if (ImGui.CollapsingHeader("AntiFlash"))
            {
                ImGui.Checkbox("Enable AntiFlash", ref antiflash);
            }
        }

        private void RenderRadarSection()
        {
            if (ImGui.CollapsingHeader("Radar"))
            {
                ImGui.Checkbox("Enable Radar", ref radar);
            }
        }

        private void RenderBhopSection()
        {
            if (ImGui.CollapsingHeader("Bhop"))
            {
                ImGui.Checkbox("Enable Bhop", ref bhop);
                if (bhop)
                {
                    ImGui.DragInt("Cooldown", ref cooldown, 1, 0, 100);
                    ImGui.Checkbox("Auto Bhop", ref autoBhop);
                    ImGui.Checkbox("Legit Mode", ref legit);
                }
            }
        }

        private void RenderTriggerBotSection()
        {
            if (ImGui.CollapsingHeader("Trigger Bot"))
            {
                ImGui.Checkbox("Enable Trigger Bot", ref trigger);
                if (trigger)
                {
                    ImGui.Checkbox("auto scope", ref autoScopeTrigger);
                    if (autoScopeTrigger)
                    {
                        ImGui.Checkbox("legit scopping in trigger", ref legitScopeTrigger);
                    }
                    ImGui.Checkbox("Legit shooting", ref legitTrigger);
                    if (aimbot)
                        ImGui.Checkbox("Auto Shoot", ref autoShoot);

                    if (autoShoot)
                    { 
                        ImGui.DragInt("Auto Shoot Delay", ref millisecondsDelay, 1, 0, 500);
                        autoTrigger = false;
                    }

                    else
                    {
                        ImGui.Checkbox("auto trigger", ref autoTrigger);

                        if(!autoShoot)   ImGui.DragInt("Trigger Delay", ref millisecondsDelay, 1, 0, 500);
                    }
                }
            }
        }

        private void RenderRecoilControlSection()
        {
            if (ImGui.CollapsingHeader("Recoil Control"))
            {
                ImGui.Checkbox("Enable RCS", ref recoitTrace);
                if (recoitTrace)
                    ImGui.DragInt("Max Bullets", ref maxRecoil, 1, -1, 50);
            }
        }

        private void RenderBombTimerSection()
        {
            if (ImGui.CollapsingHeader("Bomb Timer"))
            {
                ImGui.Checkbox("Enable Bomb Timer", ref bombTimer);
                if (bombTimer)
                {
                    RenderBombTimerWindow();
                    ImGui.Checkbox("Custom Sound", ref enableCustomSoundBombTimer);
                    ImGui.SliderFloat("Volume", ref soundVolume, 0.001f, 1);
                }
            }
        }

        private void RenderAimbotSection()
        {
            if (ImGui.CollapsingHeader("Aimbot"))
            {
                ImGui.Checkbox("Enable Aimbot", ref aimbot);
                if (aimbot)
                {
                    ImGui.Checkbox("Closest Target", ref aimOnClosest);
                    ImGui.DragInt("Aim Delay", ref aimDelay, 1, 0, 100);
                    ImGui.Checkbox("Target Team", ref aimOnTeam);
                    ImGui.Checkbox("Target Spotted", ref aimOnSpotted);
                    ImGui.Checkbox("Mouse 6 Aim", ref aimKeySecond);
                    ImGui.Checkbox("Auto Lock", ref autoLock);

                    if (autoLock)
                        ImGui.SliderFloat("Lock Distance", ref autoLockMaxDistance, 0, 3000);

                    if (!aimOnClosest)
                        ImGui.Checkbox("Use FOV", ref useFov);

                    if (useFov)
                    {
                        ImGui.Checkbox("Epilepsy Mode", ref inVisFov);
                        if (!inVisFov)
                        {
                            ImGui.SliderFloat("FOV Size", ref FOV, 1, 500);
                            ImGui.ColorEdit4("FOV Color", ref circleColor);
                        }
                    }
                }
            }
        }

        private void RenderEspSection()
        {
            if (ImGui.CollapsingHeader("ESP"))
            {
                ImGui.Checkbox("Enable ESP", ref enableEsp);
                if (enableEsp)
                {
                    RenderEspSettings();
                }
            }

            _espActive = enableEsp || aimbot;
            if (_espActive)
            {
                DrawEspOverlay();
            }
        }
        #endregion

        private void RenderEspSettings()
        {
            ImGui.Checkbox("Show Team", ref showTeam);
            ImGui.Checkbox("Show Defusing", ref showIfDefusing);
            ImGui.Checkbox("Show Armor", ref showArmor);

            if (enableBones || box)
                ImGui.Checkbox("Show Helmet", ref showHelmet);

            ImGui.Checkbox("Show Ammo", ref showAmmoInMag);
            ImGui.Checkbox("Box ESP", ref box);
            ImGui.Checkbox("Line ESP", ref drawLine);
            ImGui.Checkbox("Bone ESP", ref enableBones);
            ImGui.Checkbox("Visibility Check", ref enableVisibilityCheck);
            ImGui.Checkbox("Name ESP", ref enableName);

            ImGui.SliderFloat("Y Offset of text", ref yOffset, -100, 100);

            ImGui.Checkbox("Weapon ESP", ref weaponEsp);


            if (showArmor) ImGui.ColorEdit4("Armor Color", ref armorColor);
            if (showTeam) ImGui.ColorEdit4("Team Color", ref teamColor);
            ImGui.ColorEdit4("Enemy Color", ref enemyColor);
            ImGui.ColorEdit4("HP Color", ref barColor);
            if (enableName) ImGui.ColorEdit4("Name Color", ref nameColor);
            if (enableBones) ImGui.ColorEdit4("Bone Color", ref BoneColor);
        }

        private void RenderBombTimerWindow()
        {
            // Автоматически открываем окно при появлении бомбы
            if (bombPlanted && !_bombTimerWindowOpen)
                _bombTimerWindowOpen = true;

            if (_bombTimerWindowOpen)
            {
                ImGui.Begin("Bomb Timer", ref _bombTimerWindowOpen);

                if (bombPlanted)
                {
                    ImGui.TextColored(greenColor, "BOMB PLANTED");
                    ImGui.Text($"Time left: {timeLeft}s");

                    if (timeLeft <= 10 && timeLeft > 5 && enableCustomSoundBombTimer)
                        PlayBombSound("bombSoundAt10sec.mp3");
                    else if (timeLeft <= 5 && enableCustomSoundBombTimer)
                        PlayBombSound("5secBomb.mp3");
                    else if (timeLeft < 4)
                        ImGui.Text("WARNING: Bomb about to explode!");
                }
                else
                {
                    ImGui.TextColored(redColor, "Bomb not planted");
                }

                ImGui.End();
            }
        }

        private void PlayBombSound(string fileName)
        {
            try
            {
                var audioPlayer = new AudioPlayer(_soundsFolder);
                audioPlayer.PlaySound(fileName, soundVolume);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }
        }

        private void DrawEspOverlay()
        {
            ImGui.SetNextWindowSize(screenSize);
            ImGui.SetNextWindowPos(Vector2.Zero);

            ImGui.Begin("ESP Overlay",
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoBackground |
                ImGuiWindowFlags.NoBringToFrontOnFocus |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoInputs |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse);

            if (aimbot && useFov)
            {
                UpdateFovCircleColor();
                DrawFovCircle();
            }

            RenderVisibleEntities();
            ImGui.End();
        }

        private void UpdateFovCircleColor()
        {
            if (inVisFov)
            {
                circleColor.X = (float)_random.NextDouble();
                circleColor.Y = (float)_random.NextDouble();
                circleColor.Z = (float)_random.NextDouble();
            }
        }

        private void DrawFovCircle()
        {
            var localPlayer = GetLocalPlayer();
            if (localPlayer == null) return;

            var center = screenSize / 2;
            var radius =  FOV;
            ImGui.GetWindowDrawList().AddCircle(center, radius, ImGui.ColorConvertFloat4ToU32(circleColor));
        }

        private void RenderVisibleEntities()
        {
            foreach (var entity in _entities)
            {
                // Критически важная проверка на null
                if (entity == null) continue;

                if (!EntityOnScreen(entity)) continue;

                foreach (var renderAction in _renderActions)
                {
                    try
                    {
                        renderAction(entity);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Rendering error: {ex.Message}");
                    }
                }
            }
        }

        private bool EntityOnScreen(Entity entity)
        {
            return entity.position2d.X > 0 &&
                   entity.position2d.X < screenSize.X &&
                   entity.position2d.Y > 0 &&
                   entity.position2d.Y < screenSize.Y;
        }

        #region ESP Rendering Methods
        private void DrawBox(Entity entity)
        {
            if(box )
            {
                float height = entity.position2d.Y - entity.viewPosition2D.Y;
                var color = GetEntityColor(entity, enemyColor, teamColor);

                var topLeft = new Vector2(entity.viewPosition2D.X - height / 4, entity.viewPosition2D.Y);
                var bottomRight = new Vector2(entity.viewPosition2D.X + height / 4, entity.viewPosition2D.Y + height);

                ImGui.GetWindowDrawList().AddRect(topLeft, bottomRight, ImGui.ColorConvertFloat4ToU32(color));

                if(!enableBones )
                {
                    var headCenter = new Vector2((topLeft.X + bottomRight.X) / 2, topLeft.Y);
                    float radius = height / 8.5f;
                    var headColor = showHelmet && entity.hasHelmet && !enableBones ? armorColor : color;
                    ImGui.GetWindowDrawList().AddCircle(headCenter, radius, ImGui.ColorConvertFloat4ToU32(headColor));
                }
                
            }
            
        }

        private void DrawLine(Entity entity)
        {
            if (drawLine)
            {
                var color = GetEntityColor(entity, enemyColor, teamColor);
                ImGui.GetWindowDrawList().AddLine(
                    screenSize / 2,
                    entity.position2d,
                    ImGui.ColorConvertFloat4ToU32(color));
            }
            
        }

        private void DrawHealthBarAndArmor(Entity entity)
        {
            float height = entity.position2d.Y - entity.viewPosition2D.Y;
            float barWidth = 0.05f * (height / 2);
            float healthHeight = height * (entity.health / 100f);
            float armorHeight = height * (entity.armorHP / 100f);

            // Health bar
            var healthTop = new Vector2(entity.viewPosition2D.X - height / 4 - barWidth, entity.position2d.Y - healthHeight);
            var healthBottom = new Vector2(entity.viewPosition2D.X - height / 4, entity.position2d.Y);
            ImGui.GetWindowDrawList().AddRectFilled(healthTop, healthBottom, ImGui.ColorConvertFloat4ToU32(barColor));

            // Armor bar
            if (showArmor)
            {
                var armorTop = new Vector2(healthTop.X - barWidth, entity.position2d.Y - armorHeight);
                var armorBottom = new Vector2(healthTop.X, entity.position2d.Y);
                ImGui.GetWindowDrawList().AddRectFilled(armorTop, armorBottom, ImGui.ColorConvertFloat4ToU32(armorColor));
            }
        }

        private void DrawNameAndWeapon(Entity entity)
        {
            if (enableName)
            {
                float scale = Math.Clamp(0.8f / (entity.distance * 0.1f), 0.5f, 2.0f) * 1.5f;
                var namePos = new Vector2(entity.viewPosition2D.X, entity.position2d.Y - yOffset);

                ImGui.SetWindowFontScale(scale);
                ImGui.GetWindowDrawList().AddText(namePos, ImGui.ColorConvertFloat4ToU32(nameColor), entity.name);
            }

            if (weaponEsp)
            {
                var weaponPos = new Vector2(entity.viewPosition2D.X, entity.position2d.Y);
                ImGui.GetWindowDrawList().AddText(weaponPos, ImGui.ColorConvertFloat4ToU32(nameColor), entity.currentWeaponName);
            }
            if (showAmmoInMag)
            {
                var ammoPos = new Vector2(entity.viewPosition2D.X, entity.position2d.Y - 2*yOffset);
                ImGui.GetWindowDrawList().AddText(ammoPos, ImGui.ColorConvertFloat4ToU32(nameColor), $"{entity.ammoInMag}");
            }

            if (enableName || weaponEsp) ImGui.SetWindowFontScale(1.0f);
        }
        private bool IsValidBone(List<Vector2> bones, int index)
        {
            return bones != null &&
                   index < bones.Count &&
                   bones[index] != Vector2.Zero;
        }

        // bones draw methods
        private void DrawBones(Entity entity)
        {
            // Получаем цвет в зависимости от команды
            uint uintColor = ImGui.ColorConvertFloat4ToU32(BoneColor);
            uint uintColorHead = ImGui.ColorConvertFloat4ToU32(showHelmet && entity.hasHelmet ? armorColor : BoneColor);

            float currentBoneThickness = _localPlayer.scoped ? boneThickness : boneThickness / entity.distance;

            // Проверка на наличие костей перед рисованием
            if (entity.bones2d.Count > 12)
            {
                //draw list
                ImDrawListPtr drawList = ImGui.GetWindowDrawList();
                // Рисуем линии между костями

                // Шея -> Правое плечо
                drawList.AddLine(entity.bones2d[1], entity.bones2d[2], uintColor, currentBoneThickness);

                // Шея -> Левое плечо
                drawList.AddLine(entity.bones2d[1], entity.bones2d[3], uintColor, currentBoneThickness);

                // Шея -> Таз/Торс (центр тела)
                drawList.AddLine(entity.bones2d[1], entity.bones2d[6], uintColor, currentBoneThickness);

                // Левое плечо -> Левый локоть
                drawList.AddLine(entity.bones2d[3], entity.bones2d[4], uintColor, currentBoneThickness);

                // Таз/Торс -> Правое бедро
                drawList.AddLine(entity.bones2d[6], entity.bones2d[7], uintColor, currentBoneThickness);

                // Левый локоть -> Левая кисть
                drawList.AddLine(entity.bones2d[4], entity.bones2d[5], uintColor, currentBoneThickness);

                // Правое бедро -> Правое колено
                drawList.AddLine(entity.bones2d[7], entity.bones2d[8], uintColor, currentBoneThickness);

                // Шея -> Голова
                drawList.AddLine(entity.bones2d[1], entity.bones2d[0], uintColor, currentBoneThickness);

                // Голова -> Дополнительная точка (возможно для ушей/висков)
                drawList.AddLine(entity.bones2d[0], entity.bones2d[9], uintColor, currentBoneThickness);

                // Голова -> Дополнительная точка (возможно для ушей/висков)
                drawList.AddLine(entity.bones2d[0], entity.bones2d[11], uintColor, currentBoneThickness);

                // Дополнительная точка головы -> Дополнительная точка (челюсть/подбородок)
                drawList.AddLine(entity.bones2d[9], entity.bones2d[10], uintColor, currentBoneThickness);

                // Дополнительная точка головы -> Дополнительная точка (челюсть/подбородок)
                drawList.AddLine(entity.bones2d[11], entity.bones2d[12], uintColor, currentBoneThickness);

                // Проверяем наличие головы перед рисованием круга
                if (entity.bones2d.Count > 2)
                {
                    // Рисуем круг вокруг головы (кость с индексом 2 обычно является головой)
                    drawList.AddCircle(entity.bones2d[2], (entity.position2d.Y - entity.viewPosition2D.Y) / 8.5f, uintColorHead);
                }
            }
        }
        



        #endregion

        private Vector4 GetEntityColor(Entity entity, Vector4 enemyCol, Vector4 teamCol)
        {
            var localPlayer = GetLocalPlayer();
            if (localPlayer == null) return enemyCol;

            bool isTeammate = localPlayer.team == entity.team;
            bool visible = !enableVisibilityCheck || entity.spotted;

            if (!visible) return hiddenColor;
            return isTeammate ? teamCol : enemyCol;
        }

        #region Entity Management
        public void UpdateEntities(IEnumerable<Entity> newEntities)
        {
            _entities = new ConcurrentQueue<Entity>(newEntities);
        }

        public void UpdateLocalPlayer(Entity entity)
        {
            lock (_entityLock) _localPlayer = entity;
        }

        public Entity GetLocalPlayer()
        {
            lock (_entityLock) return _localPlayer;
        }
        #endregion
    }
}