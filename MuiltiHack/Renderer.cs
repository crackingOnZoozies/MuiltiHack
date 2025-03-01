using ClickableTransparentOverlay;
using ImGuiNET;
using System;
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

        public bool antiflash = false;

        public bool radar = false;

        public bool bhop = false;
        public int cooldown = 0;
        public bool autoBhop = false;
        public bool legit = false;
        public bool bombTimer = false;

        public bool trigger = false;
        public int millisecondsDelay = 0;
        public bool autoTrigger = false;

        //bomb timer

        //rgba
        private Vector4 redColor = new Vector4(1, 0, 0, 1);
        private Vector4 greenColor = new Vector4(0, 1, 0, 1);

        //bomb stuff
        public bool bombPlanted = false;
        public int timeLeft = -1;
        public float soundVolume = 0.5f;
        public bool enableCustomSoundBombTimer = false;

        protected override void Render()
        {
            ImGui.Begin("multiCheat beta legit");

            // Секция AntiFlash
            ImGui.SeparatorText("antiflash Settings");
            ImGui.Checkbox("Enable AntiFlash", ref antiflash);

            // Секция Radar
            ImGui.SeparatorText("radar Settings");
            ImGui.Checkbox("Enable Radar", ref radar);

            // Секция Bhop
            ImGui.SeparatorText("bhop Settings");
            ImGui.Checkbox("Enable Bhop", ref bhop);
            if (bhop)
            {
                ImGui.DragInt("Bhop Cooldown", ref cooldown);
                ImGui.Checkbox("Auto Bhop", ref autoBhop);
                ImGui.Checkbox("Legit (No Crouch)", ref legit);
            }

            //trigger bot
            ImGui.SeparatorText("triggerbot Settings");

            ImGui.Checkbox("Enable Trigger Bot", ref trigger);
            if (trigger)
            {
                ImGui.DragInt("Trigger Delay", ref millisecondsDelay);
                ImGui.Checkbox("Auto Trigger", ref autoTrigger);
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

    }

}

