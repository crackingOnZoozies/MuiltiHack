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
        public static int attack = 0x1AED530;
        public static int jump = 0x1AEDA40;
        public static int duck = 0x1AEDAD0;
        public static int lookatweapon = 0x1D2BEB0;

        //offsets.cs
        public static int dwViewAngles = 0x1D2C740;
        public static int dwLocalPlayerPawn = 0x1AF4B00;
        public static int dwEntityList = 0x1CBE5B0;

        public static int dwGameRules = 0x1D1D460;
        public static int dwViewMatrix = 0x1D21980; // offset for circle

        //client.dll.cs
        public static int m_hPlayerPawn = 0x8FC;
        public static int m_iHealth = 0x34C;
        public static int m_vOldOrigin = 0x15B0;
        public static int m_iTeamNum = 0x3EB;
        public static int m_vecViewOffset = 0xD98;
        public static int m_lifeState = 0x350;

        public static int m_modelState = 0x170; // head
        public static int m_pGameSceneNode = 0x330;

        public static int m_entitySpottedState = 0x2898;
        public static int m_bSpotted = 0x8;

        public static int m_iIDEntIndex = 0x1734;

        public static int m_bIsScoped = 0x28B0;// bool

        public static int m_aimPunchAngle = 0x185C;
        public static int m_iShotsFired = 0x28C4;
        public static int m_flFlashBangTime = 0x1668;

        public static int m_fFlags = 0x3F8;

        public static int m_iszPlayerName = 0x6E8;

        public static int m_pCameraServices = 0x1438;
        public static int m_iFOV = 0x288;

        public static int m_pClippingWeapon = 0x1620;
        public static int m_iItemDefinitionIndex = 0x1BA;
        public static int m_AttributeManager = 0x13A0;
        public static int m_Item = 0x50;

        public static int m_bBombPlanted = 0x9A5; // bool
        public static int m_bOldIsScoped = 0x28F4; // bool
        public static int m_zoomLevel = 0x1D40; // int32

        public static int m_bPrevDefuser = 0x1846; // bool
        public static int m_bPrevHelmet = 0x1847; // bool
        public static int m_nPrevArmorVal = 0x1848;
        public static int m_bIsDefusing = 0x28B2; // bool

        public static int m_iClip1 = 0x1900; // int32

        //engine
        public static nint dwWindowHeight = 0x8AB4DC;
        public static nint dwWindowWidth = 0x8AB4D8;


        public static bool bBombPlanted = false;

    }
    
}
