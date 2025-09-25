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
        public static int attack = 0x1BD9310;
        public static int jump = 0x1BD9820;
        public static int duck = 0x1BD98B0;
        public static int lookatweapon = 0x1E2B5B0;
        public static int attack2 = 0x1BD93A0;

        //offsets.cs
        public static int dwViewAngles = 0x1E2BE40;
        public static int dwLocalPlayerPawn = 0x1BDFE90;
        public static int dwEntityList = 0x1D04458;

        public static int dwGameRules = 0x1E204C8;
        public static int dwPlantedC4 = 0x1E26930;
        public static int dwViewMatrix = 0x1E21230; // offset for circle

        //client.dll.cs
        public static int m_hPlayerPawn = 0x8FC;
        public static int m_iHealth = 0x34C;
        public static int m_vOldOrigin = 0x15B8;
        public static int m_iTeamNum = 0x3EB;
        public static int m_vecViewOffset = 0xD98;
        public static int m_lifeState = 0x350;

        public static int m_modelState = 0x190; // head
        public static int m_pGameSceneNode = 0x330;

        public static int m_entitySpottedState = 0x2718;
        public static int m_bSpotted = 0x8;

        public static int m_iIDEntIndex = 0x3EDC;

        public static int m_bIsScoped = 0x2730;// bool

        public static int m_aimPunchAngle = 0x16FC;
        public static int m_iShotsFired = 0x2744;
        public static int m_flFlashBangTime = 0x1614;

        public static int m_fFlags = 0x3F8;

        public static int m_iszPlayerName = 0x6E8;

        public static int m_pCameraServices = 0x1440;
        public static int m_iFOV = 0x288;

        public static int m_pClippingWeapon = 0x3DF0;
        public static int m_iItemDefinitionIndex = 0x1BA;
        public static int m_AttributeManager = 0x13A8;
        public static int m_Item = 0x50;

        public static int m_bBombPlanted = 0x9A5; // bool
        public static int m_bOldIsScoped = 0x2774; // bool
        public static int m_zoomLevel = 0x1F90; // int32

        public static int m_bPrevDefuser = 0x16E6; // bool
        public static int m_bPrevHelmet = 0x16E7; // bool
        public static int m_nPrevArmorVal = 0x16E8;
        public static int m_bIsDefusing = 0x2732; // bool

        public static int m_iClip1 = 0x1908; // int32

        //engine
        public static nint dwWindowHeight = 0x8E967C;
        public static nint dwWindowWidth = 0x8E9678;


        public static bool bBombPlanted = false;

    }
    
}
