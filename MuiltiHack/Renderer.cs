using ClickableTransparentOverlay;
using ImGuiNET;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MuiltiHack
{
    public class Renderer : Overlay
    {
        //antiflash
        public bool antiflash = false;

        //radar
        public bool radar = false;

        //bhop
        public bool bhop = false;
        public int cooldown = 0;
        public bool autoBhop = false;
        public bool legit = false;


        public bool bombTimer = false;

        //trigger bot
        public bool trigger = false;
        public bool legitTrigger = true;
        public int millisecondsDelay = 0;
        public bool autoTrigger = false;

        //rgba
        private Vector4 redColor = new Vector4(1, 0, 0, 1);
        private Vector4 greenColor = new Vector4(0, 1, 0, 1);

        //bomb stuff
        public bool bombPlanted = false;
        public int timeLeft = -1;
        public float soundVolume = 0.5f;
        public bool enableCustomSoundBombTimer = false;

        //esp
        private float yOffset = 20; // odfset between text
        
        //enteties copy
        private ConcurrentQueue<Entity> entities = new ConcurrentQueue<Entity>();
        private Entity localPlayer = new Entity();
        private readonly object entityLock = new object();

        //gui elements
        public bool enableEsp = false;
        private bool enableBones = false;
        private bool enableName = false;
        public bool enableVisibilityCheck = false;
        private bool weaponEsp = false;
        private bool box = false;
        private bool drawLine = false;
        public bool showTeam = false;
        public bool showHelmet = false;
        public bool showArmor = false;
        public bool showIfHasKit = false;
        public bool showIfDefusing = false;
        public bool showAmmoInMag = false;

        private float boneThickness = 4;
        public float AutoSpotDist = 1000;
        public float maxEspDist = 10000;


        private Vector4 enemyColor = new Vector4(1, 0, 0, 1); // red
        private Vector4 teamColor = new Vector4(0, 1, 0, 1); // green
        private Vector4 barColor = new Vector4(0, 1, 0, 1);//green
        private Vector4 nameColor = new Vector4(1, 1, 1, 1); //white
        private Vector4 hiddenColor = new Vector4(0, 0, 0, 1); //black
        private Vector4 BoneColor = new Vector4(1, 0, 2, 1);
        private Vector4 armorColor = new Vector4(1,1,1,1); //white

        //aimbot
        public float FOV = 50; // in pixels

        public bool aimbot = false;
        public bool aimOnTeam = false;
        public bool aimOnSpotted = true;
        public bool useFov = false;
        public bool aimOnClosest = false;
        public bool followRecoil = false;
        public int maxRecoil = -1;
        public bool legitAimBot = false;

        public bool autoLock = false;
        public float autoLockMaxDistance = 0f;

        public bool autoShoot = false;

        public bool aimKeySecond = false;

        public int aimDelay = 10;

        public Vector4 circleColor = new Vector4(1, 0, 1, 1);

        //draw list
        ImDrawListPtr drawList;

        //screen size
        public Vector2 screenSize = new Vector2(1920, 1080);//default

        //fov changer
        public bool enableFovChanger = false;
        public bool ignorescoping = false;
        public float FovChangerFOV = 90;

        //rcs
        public bool recoitTrace = false;

        //autopistol
        public bool autoPistol = false;
        //public bool autoPistolLegit = true; bullshit dont work

        //anti aim
        public bool antiAim = false;
        public bool stabilize = true;
        public bool backOnly = false;
        public bool ultraSpin = false;
        public bool jitterMode = false;

        //inf inspect
        public bool inspect = false;
        public bool manual = false;

        protected override void Render()
        {
            ImGui.Begin("multiCheat beta legit");

            ImGui.SeparatorText("anti aim");
            ImGui.Checkbox("on anti aim", ref antiAim);
            if(ImGui.CollapsingHeader("antiAim modes"))
            {
                ImGui.Checkbox("stabilizer", ref stabilize);
                ImGui.Checkbox("simple backwards aimbot", ref backOnly);
                ImGui.Checkbox("retarded spin", ref ultraSpin);
                ImGui.Checkbox("jitter", ref jitterMode);
            }

            //ImGui.SeparatorText("inspect shit(dont work)");
            //ImGui.Checkbox("auto inspect", ref inspect);
            if (inspect)
            {
                ImGui.Checkbox("manual", ref manual);
            }

            ImGui.SeparatorText("fovchanger");
            ImGui.Checkbox("FOV changer", ref enableFovChanger);
            if(enableFovChanger)
            {
                ImGui.Checkbox("ignore scope", ref ignorescoping);
                ImGui.SliderFloat("FOV", ref FovChangerFOV, 0, 150);
            }
            if(!enableFovChanger && FovChangerFOV != 0)
            {
                FovChangerFOV = 0;
            }

            ImGui.SeparatorText("auto pistol");
            ImGui.Checkbox("auto pistol switch", ref autoPistol);
            

            // Секция AntiFlash
            ImGui.SeparatorText("antiflash Settings");
            ImGui.Checkbox("Enable AntiFlash", ref antiflash);

            // Секция Radar
            ImGui.SeparatorText("radar Settings");
            ImGui.Checkbox("Enable Radar", ref radar);

            // Секция Bhop
            ImGui.SeparatorText("bhop Settings");
            ImGui.Checkbox("Enable Bhop", ref bhop);
            if (bhop && ImGui.CollapsingHeader("bhop settings"))
            {
                ImGui.DragInt("Bhop Cooldown", ref cooldown);
                ImGui.Checkbox("Auto Bhop", ref autoBhop);
                ImGui.Checkbox("Legit (No Crouch)", ref legit);
            }

            //trigger bot
            ImGui.SeparatorText("triggerbot Settings");

            ImGui.Checkbox("Enable Trigger Bot", ref trigger);
            if (trigger && ImGui.CollapsingHeader("trigger settings"))
            {
                ImGui.Checkbox("legit trigger", ref legitTrigger);
                if(aimbot) ImGui.Checkbox("autoshoot", ref autoShoot);
                else
                {
                    autoShoot = false;
                }
                if(autoShoot)
                {
                    ImGui.DragInt("autoshoot Delay", ref millisecondsDelay);
                }
                else
                {
                    ImGui.DragInt("Trigger Delay", ref millisecondsDelay);
                    ImGui.Checkbox("Auto Trigger", ref autoTrigger);
                }
                
            }

            //rcs
            ImGui.SeparatorText("recoil conatrol");
            ImGui.Checkbox("rcs switch", ref recoitTrace);
            if (recoitTrace)
            {
                ImGui.DragInt("max rcs bullets", ref maxRecoil);
            }
            else
            {
                maxRecoil = -1;
            }

                // Секция Bomb Timer
                ImGui.SeparatorText("Bomb Timer Settings");
            
            ImGui.Checkbox("Enable Bomb Timer", ref bombTimer); // Checkbox для включения/выключения bombTimer

            if (bombTimer)
            {
                //ImGui.Text("Bomb Timer Settings");
                
                ImGui.Checkbox("Use Custom Sound", ref enableCustomSoundBombTimer);
                ImGui.SliderFloat("Sound Volume", ref soundVolume, 0.001f, 1);

                // Окно таймера бомбы
                BombTimer();
            }

            //aimbot
            ImGui.SeparatorText("aimbot");
            ImGui.Checkbox("aimbot in/off", ref aimbot);
            if (aimbot)
            {
                //ImGui.Checkbox("legit Aim assist", ref legitAimBot);
                ImGui.Checkbox("aim on closest by diatance", ref aimOnClosest);
                ImGui.DragInt("aim delay", ref aimDelay);
                ImGui.Checkbox("team aimbot", ref aimOnTeam);
                ImGui.Checkbox("aim on spotted", ref aimOnSpotted);
                ImGui.Checkbox("use mouse 6 for aiming", ref aimKeySecond);
                ImGui.Checkbox("autoLock", ref autoLock);
                if (autoLock)
                {
                    ImGui.SliderFloat("max distance for autoLock", ref autoLockMaxDistance, 0, 3000);
                }

                if (!aimOnClosest) ImGui.Checkbox("fov", ref useFov);

                if (useFov)
                {
                    ImGui.SliderFloat("fov", ref FOV, 10, 300.0f);
                    if (ImGui.CollapsingHeader("Fov circle color"))
                    {
                        ImGui.ColorPicker4("##fovcolor", ref circleColor);
                    }
                }
            }

            //esp menu
            ImGui.SeparatorText("ESP");

            ImGui.Checkbox("on", ref enableEsp);

            if (enableEsp)
            {
                if(ImGui.CollapsingHeader("ESP settings"))
                {
                    
                    ImGui.Checkbox("show team", ref showTeam);

                    ImGui.Checkbox("show if defusing ", ref showIfDefusing);

                    ImGui.Checkbox("show armor hp", ref showArmor);
                    if(enableBones || box)
                    {
                        ImGui.Checkbox("show helmet", ref showHelmet);
                    }

                    if(showArmor && ImGui.CollapsingHeader("armor p bar color and helmet color"))
                    {
                        ImGui.ColorPicker4("##Bone color", ref armorColor);
                    }
                    //ImGui.SliderFloat("max distance for esp renderer", ref maxEspDist, 0, 10000);

                    ImGui.Checkbox("show ammo in mag", ref showAmmoInMag);
                    ImGui.Checkbox("box", ref box);

                    ImGui.Checkbox("draw line", ref drawLine);

                    ImGui.Checkbox("bones", ref enableBones);
                    if (ImGui.CollapsingHeader("bone color"))
                    {
                        ImGui.ColorPicker4("##Bone color", ref BoneColor);

                    }

                    ImGui.Checkbox("show only spotted", ref enableVisibilityCheck);

                    ImGui.Checkbox("enable name", ref enableName);
                    if (enableName)
                    {
                        ImGui.SliderFloat("text y offset of name", ref yOffset, -100, 100);
                        ImGui.Checkbox("weapon esp", ref weaponEsp);
                    }

                    //team color
                    if (showTeam && ImGui.CollapsingHeader("team color"))
                    {
                        ImGui.ColorPicker4("##teamcolor", ref teamColor);
                    }

                    // enemy color
                    if (ImGui.CollapsingHeader("enemy color"))
                    {
                        ImGui.ColorPicker4("##enemycolor", ref enemyColor);
                    }

                    //hp bar color
                    if (ImGui.CollapsingHeader("hp bar color"))
                    {
                        ImGui.ColorPicker4("##barColor", ref barColor);
                    }

                    //name color
                    if (ImGui.CollapsingHeader("name color") && enableName)
                    {
                        ImGui.ColorPicker4("##namecolor", ref nameColor);
                    }


                }


            }
            if (enableEsp || aimbot)
            {
                // draw esp overlay
                DrawOverlay(screenSize);
            }

            drawList = ImGui.GetWindowDrawList();

            foreach (Entity entity in entities)
            {
                //check if entity in screen
                if (EntityOnSceen(entity))
                {
                    //draw methods (all)
                    if(enableEsp) 
                    {
                        DrawHealthBarAndArmor(entity);
                        ScopedCheckIfDefusingHasKitHasAmmoInMag(entity);
                    }
                    if (box) DrawBox(entity);
                    if (drawLine) DrawLine(entity);
                    DrawNameAndWeapon(entity);
                    if (enableBones && entity.team != localPlayer.team) DrawBones(entity);

                }

            }
            

        }

        // Укажите путь к папке EventSounds
        string soundsFolder = Path.Combine(AppContext.BaseDirectory, "EventSounds");
        
        void BombTimer()
        {
            AudioPlayer audioPlayer = new AudioPlayer(soundsFolder);
            ImGui.Begin("bob timer");
            if (bombPlanted)
            {
                ImGui.TextColored(greenColor, "bombPlanted");
                ImGui.Text($"second before BOOM: {timeLeft} ");

                if (timeLeft == 10 && enableCustomSoundBombTimer)
                {
                    // Воспроизведение звука
                    audioPlayer.PlaySound("bombSoundAt10sec.mp3", volume: soundVolume);
                }
                //"bombSoundAt10sec.mp3"
                if (timeLeft == 5 && enableCustomSoundBombTimer)
                {
                    audioPlayer.PlaySound("5secBomb.mp3", volume: soundVolume);
                }
                if (timeLeft < 4)
                {
                    ImGui.Text("bomb botta blow");
                }
            }
            else
            {
                ImGui.TextColored(redColor, "bomb not planted");
            }
            ImGui.End();
        }

        // ///////////////////////// esp
        //check position
        bool EntityOnSceen(Entity entity)
        {
            if (entity.position2d.X > 0 && entity.position2d.X < screenSize.X && entity.position2d.Y > 0 && entity.position2d.Y < screenSize.Y)
            {
                return true;
            }
            return false;
        }

        // drawing ESP methods

        private void DrawBox(Entity entity)
        {
            // calc box height
            float entityHeight = entity.position2d.Y - entity.viewPosition2D.Y;

            //calc box dimensions
            Vector2 rectTop = new Vector2(entity.viewPosition2D.X - entityHeight / 4, entity.viewPosition2D.Y);

            Vector2 rectBottom = new Vector2(entity.viewPosition2D.X + entityHeight / 4, entity.viewPosition2D.Y + entityHeight);

            Vector4 boxColor = localPlayer.team == entity.team ? teamColor : enemyColor;

            if (enableVisibilityCheck && localPlayer.team != entity.team)
            {
                boxColor = entity.spotted ? boxColor : hiddenColor;
            }

            // Draw rectangle
            drawList.AddRect(rectTop, rectBottom, ImGui.ColorConvertFloat4ToU32(boxColor));



            if (!enableBones)
            {
                // Calculate center of the top side of the rectangle
                Vector2 circleCenter = new Vector2((rectTop.X + rectBottom.X) / 2, rectTop.Y);

                // Calculate radius of the circle (half of the height of the rectangle)
                float circleRadius = entityHeight / 8.5f;

                // hidden check


                // Draw circle
                drawList.AddCircle(circleCenter, circleRadius, ImGui.ColorConvertFloat4ToU32(showHelmet && entity.hasHelmet? armorColor: boxColor));
            }

        }

        private void DrawLine(Entity entity)
        {
            Vector4 lineColor = localPlayer.team == entity.team ? teamColor : enemyColor;
            if (enableVisibilityCheck && localPlayer.team != entity.team)
            {
                lineColor = entity.spotted ? lineColor : hiddenColor;
            }

            //draw line
            drawList.AddLine(new Vector2(screenSize.X / 2, screenSize.Y), entity.position2d, ImGui.ColorConvertFloat4ToU32(lineColor));

        }

        private void DrawHealthBarAndArmor(Entity entity)
        {
            //calc the hp bar height
            float entityHeight = entity.position2d.Y - entity.viewPosition2D.Y;

            //calc width
            float boxLeft = entity.viewPosition2D.X - entityHeight / 4 + 0.01f;
            float boxRight = entity.viewPosition2D.X + entityHeight / 4 + 0.01f;

            //calc health bar width and height
            float barPercentWidth = 0.05f; // 5% of box width
            float barHeight = entityHeight * (entity.health / 100f);
            float armorHeight = entityHeight * (entity.armorHP / 100f); // Расчёт высоты полоски брони

            float barPixelWidth = barPercentWidth * (boxRight - boxLeft);

            //calc health bar rectangle
            Vector2 barTop = new Vector2(boxLeft - barPixelWidth, entity.position2d.Y - barHeight);
            Vector2 barBottom = new Vector2(boxLeft, entity.position2d.Y);

            //drawing health bar
            drawList.AddRectFilled(barTop, barBottom, ImGui.ColorConvertFloat4ToU32(barColor));

            //calc armor bar rectangle (переходим к левой стороне)
            Vector2 armorTop = new Vector2(boxLeft - barPixelWidth - barPixelWidth, entity.position2d.Y - armorHeight); // Добавляем ещё один barPixelWidth для сдвига
            Vector2 armorBottom = new Vector2(boxLeft - barPixelWidth, entity.position2d.Y);

            //drawing armor bar
            drawList.AddRectFilled(armorTop, armorBottom, ImGui.ColorConvertFloat4ToU32(armorColor)); // Белая полоска для брони
        }

        private void DrawNameAndWeapon(Entity entity)
        {
            if (enableName)
            {
                // Используем расстояние из объекта Entity
                float distance = entity.distance;

                // Масштабируем размер текста в зависимости от расстояния
                float textScale = 0.8f / (distance * 0.1f); // Пример формулы масштабирования
                textScale = Math.Clamp(textScale, 0.5f, 2.0f) * 1.5f; // Ограничиваем минимальный и максимальный размер

                // Позиция для текста (имя)
                Vector2 textLocation1 = new Vector2(entity.viewPosition2D.X, entity.position2d.Y - yOffset);

                // Устанавливаем размер текста
                ImGui.SetWindowFontScale(textScale);

                // Отрисовываем текст (имя)
                drawList.AddText(textLocation1, ImGui.ColorConvertFloat4ToU32(nameColor), $"{entity.name}");

                // Если включено отображение оружия
            }
            if (weaponEsp)
            {
                // Позиция для текста (оружие)
                Vector2 textLocation2 = new Vector2(entity.viewPosition2D.X, entity.position2d.Y);

                // Отрисовываем текст (оружие)
                drawList.AddText(textLocation2, ImGui.ColorConvertFloat4ToU32(nameColor), $"{entity.currentWeaponName}");
            }

            // Возвращаем размер текста к значению по умолчанию
            ImGui.SetWindowFontScale(1.0f);

        }
        private void ScopedCheckIfDefusingHasKitHasAmmoInMag(Entity entity)
        {
            Vector2 textLocation = new Vector2(entity.viewPosition2D.X, entity.position2d.Y + yOffset);
            float offset = 0;
            if (entity.scoped)
            {
                drawList.AddText(textLocation, ImGui.ColorConvertFloat4ToU32(nameColor), $"SCOPPED");

                // Проверка на состояние "Defusing"
                if (entity.isDefusing)
                {
                    offset += 15;
                    textLocation.Y += offset; // Увеличиваем Y-координату для новой строки
                    drawList.AddText(textLocation, ImGui.ColorConvertFloat4ToU32(nameColor), $"Defusing");
                }
            }
            // Проверка на состояние "Defusing"
            else if (entity.isDefusing && showIfDefusing)
            {
                drawList.AddText(textLocation, ImGui.ColorConvertFloat4ToU32(nameColor), $"Defusing with " + (entity.hasKit ? "" : "no ") + "kits");
                offset += 15;
            }
            if (showAmmoInMag)
            {
                ShowAmmoInMagFunc(entity, offset);
            }
        }

        void ShowAmmoInMagFunc(Entity entity, float offset)
        {
            Vector2 textLocation = new Vector2(entity.viewPosition2D.X, entity.position2d.Y + yOffset);
            textLocation.Y += offset;
            drawList.AddText(textLocation, ImGui.ColorConvertFloat4ToU32(nameColor), $"{entity.ammoInMag} ammo ");
        }

        // bones draw methods
        private void DrawBones(Entity entity)
        {
            // get ether team or enemy colorr depending on the team
            uint uintColor = ImGui.ColorConvertFloat4ToU32(BoneColor);
            uint uintColorHead = ImGui.ColorConvertFloat4ToU32(showHelmet && entity.hasHelmet? armorColor: BoneColor);

            float currentBoneThickness;

            if (localPlayer.scoped)
            {
                currentBoneThickness = boneThickness;
            }
            else
            {
                currentBoneThickness = boneThickness / entity.distance;
            }

            //draw lines between bones
            drawList.AddLine(entity.bones2d[1], entity.bones2d[2], uintColor, currentBoneThickness);

            drawList.AddLine(entity.bones2d[1], entity.bones2d[3], uintColor, currentBoneThickness);

            drawList.AddLine(entity.bones2d[1], entity.bones2d[6], uintColor, currentBoneThickness);

            drawList.AddLine(entity.bones2d[3], entity.bones2d[4], uintColor, currentBoneThickness);

            drawList.AddLine(entity.bones2d[6], entity.bones2d[7], uintColor, currentBoneThickness);

            drawList.AddLine(entity.bones2d[4], entity.bones2d[5], uintColor, currentBoneThickness);

            drawList.AddLine(entity.bones2d[7], entity.bones2d[8], uintColor, currentBoneThickness);

            drawList.AddLine(entity.bones2d[1], entity.bones2d[0], uintColor, currentBoneThickness);

            drawList.AddLine(entity.bones2d[0], entity.bones2d[9], uintColor, currentBoneThickness);

            drawList.AddLine(entity.bones2d[0], entity.bones2d[11], uintColor, currentBoneThickness);

            drawList.AddLine(entity.bones2d[9], entity.bones2d[10], uintColor, currentBoneThickness);

            drawList.AddLine(entity.bones2d[11], entity.bones2d[12], uintColor, currentBoneThickness);

            drawList.AddCircle(entity.bones2d[2], (entity.position2d.Y - entity.viewPosition2D.Y) / 8.5f, uintColorHead);


        }


        //transfer entity methods

        public void UpdateEntities(IEnumerable<Entity> newEntities)
        {
            entities = new ConcurrentQueue<Entity>(newEntities);

        }

        public void UpdateLocalPlayer(Entity newEntity)
        {
            lock (entityLock)
            {
                localPlayer = newEntity;
            }
        }

        public Entity GetLocalPlayer()
        {
            lock (entityLock)
            {
                return localPlayer;
            }
        }

        // draw overlay
        void DrawOverlay(Vector2 screenSize)
        {
            ImGui.SetNextWindowSize(screenSize);
            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.Begin("overlay", ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoInputs
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoScrollWithMouse
                );
            if (aimbot && useFov)
            {
                if(localPlayer.zoomLevel == 0)
                {
                    ImDrawListPtr drawList = ImGui.GetWindowDrawList();
                    drawList.AddCircle(new Vector2(screenSize.X / 2, screenSize.Y / 2), FOV, ImGui.ColorConvertFloat4ToU32(circleColor));
                }
                else
                {
                    ImDrawListPtr drawList = ImGui.GetWindowDrawList();
                    drawList.AddCircle(new Vector2(screenSize.X / 2, screenSize.Y / 2), FOV*localPlayer.zoomLevel, ImGui.ColorConvertFloat4ToU32(circleColor));
                }
            }
                
        }
        

    }
}

