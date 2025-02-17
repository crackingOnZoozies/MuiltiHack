using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuiltiHack
{
    public static class Offsets
    {
        //buttons.cs
        public static int attack = 0x1882720;
        public static int jump = 0x1882C30;
        public static int duck = 0x1882CC0;

        //offsets.cs
        public static int dwViewAngles = 0x1AABA40;
        public static int dwLocalPlayerPawn = 0x1889F20;
        public static int dwEntityList = 0x1A359B0;

        public static int dwViewMatrix = 0x1AA17B0; // offset for circle

        //client.dll.cs
        public static int m_hPlayerPawn = 0x80C;
        public static int m_iHealth = 0x344;
        public static int m_vOldOrigin = 0x1324;
        public static int m_iTeamNum = 0x3E3;
        public static int m_vecViewOffset = 0xCB0;
        public static int m_lifeState = 0x348;

        public static int m_modelState = 0x170; // head
        public static int m_pGameSceneNode = 0x328;

        public static int m_entitySpottedState = 0x23D0;
        public static int m_bSpotted = 0x8;

        public static int m_iIDEntIndex = 0x1458;

        public static int m_bOldIsScoped = 0x242C; // bool

        public static int m_aimPunchAngle = 0x1584;
        public static int m_iShotsFired = 0x23FC;
        public static int m_flFlashBangTime = 0x13F8;

        public static int m_fFlags = 0x3EC;

        public static int m_iszPlayerName = 0x660;

    }
    
}
