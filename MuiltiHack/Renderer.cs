using ClickableTransparentOverlay;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool trigger = false;
        public int millisecondsDelay = 0;
        public bool autoTrigger = false;

        protected override void Render()
        {
            ImGui.Begin("multiCheat beta legit");

            ImGui.Checkbox("antiflash", ref antiflash);

            ImGui.Checkbox("radar", ref radar);

            
            ImGui.Checkbox("bhop", ref bhop);
            if (bhop)
            {
                ImGui.DragInt("bhop cooldown", ref cooldown);
                ImGui.Checkbox("auto bhop", ref autoBhop);
                ImGui.Checkbox("legit(no crouch)", ref legit);
            }

            ImGui.Checkbox("trigger bot", ref trigger);
            if (trigger)
            {
                ImGui.DragInt("trigger delay", ref millisecondsDelay);
                ImGui.Checkbox("auto", ref autoTrigger);
            }
        }

    }
    
}
