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

            ImGui.Checkbox("antiflash", ref antiflash);

            ImGui.Checkbox("radar", ref radar);

            
            ImGui.Checkbox("bhop", ref bhop);
            if (bhop)
            {
                if(ImGui.CollapsingHeader("bhop settings"))
                {
                    ImGui.DragInt("bhop cooldown", ref cooldown);
                    ImGui.Checkbox("auto bhop", ref autoBhop);
                    ImGui.Checkbox("legit(no crouch)", ref legit);
                }
                
            }

            ImGui.Checkbox("trigger bot", ref trigger);
            if (trigger)
            {
                if(ImGui.CollapsingHeader("trigger settings"))
                {
                    ImGui.DragInt("trigger delay", ref millisecondsDelay);
                    ImGui.Checkbox("auto", ref autoTrigger);
                }
                
            }

            ImGui.Checkbox("bomb timer", ref bombTimer);


            if (bombTimer && ImGui.CollapsingHeader("custom sound volume"))
            {
                ImGui.Checkbox("use costom sound", ref enableCustomSoundBombTimer);
                ImGui.SliderFloat("", ref soundVolume, 0.001f, 1);
                BombTimer();
            }

            ImGui.End();
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

