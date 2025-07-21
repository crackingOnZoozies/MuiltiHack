using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MuiltiHack
{
    public class Entity
    {
        public Vector3 position { get; set; }
        public Vector3 viewOffset { get; set; }
        public Vector2 position2d { get; set; }
        public Vector2 viewPosition2D { get; set; }

        //bones
        public Vector3 origin { get; set; }
        public List<Vector3> bones { get; set; }
        public List<Vector2> bones2d { get; set; }

        public float distance { get; set; }
        public float pixelDistance { get; set; }
        //////////

        public short currentWeaponIndex { get; set; }
        public string currentWeaponName { get; set; }

        public int team { get; set; }
        public int health { get; set; }
        public int zoomLevel { get; set; }
        public int armorHP { get; set; }
        public int ammoInMag { get; set; }

        public uint lifeState { get; set; }

        public string name { get; set; }

        public bool spotted { get; set; }
        public bool scoped { get; set; }
        public bool scopped { get; set; }
        public bool hasHelmet { get; set; }
        public bool hasKit { get; set; }
        public bool isDefusing { get; set; }

        public IntPtr pawnAddress { get; set; }
        public IntPtr controllerPawn { get; set; }
        public Vector3 view { get; set; }
        public Vector3 head { get; set; }
        public Vector2 head2d { get; set; }
        
        
    }
    public enum BonesIds
    {
        Wais = 0, //0
        Neck = 5, //1
        Head = 6, //2
        ShoulderLeft = 8, //3
        ForeLeft = 9, //4
        HandLeft = 11, //5
        ShoulderRight = 13, //6
        ForeRight = 14, //7
        HandRight = 16, //8
        KneeLeft = 23,//9
        FeetLeft = 24, //10
        KneeRight = 26, //11
        FeetRight = 27, //12
    }
    public enum Weapon
    {
        DEAGLE = 1,

        BERETAS = 2,

        FIVESEVEN = 3,

        GLOCK = 4,

        AK47 = 7,

        AUG = 8,

        AWP = 9,

        FAMAS = 10,

        G3SG1 = 11,

        GALILAR = 13,

        M249 = 14,

        M4A1 = 16,

        MAC10 = 17,

        P90 = 19,

        MP5SD = 23,

        UMP45 = 24,

        XM1014 = 25,

        BIZON = 26,

        MAG7 = 27,

        NEGEV = 28,

        SAWEDOFF = 29,

        TEC9 = 30,

        TASER = 31,

        HKP2000 = 32,

        MP7 = 33,

        MP9 = 34,

        NOVA = 35,

        P250 = 36,

        SHIELD = 37,

        SCAR20 = 38,

        SG556 = 39,

        SSG08 = 40,

        KNIFEGG = 41,

        KNIFE = 42,

        FLASHBANG = 43,

        HEGRENADE = 44,

        SMOKEGRENADE = 45,

        MOLOTOV = 46,

        DECOY = 47,

        INCGRENADE = 48,

        C4 = 49,

        HEALTHSHOT = 57,

        KNIFE_T = 59,

        M4A1_SILENCER = 60,

        USP_SILENCER = 61,

        CZ75A = 63,

        REVOLVER = 64,

        TAGRENADE = 68,

        FISTS = 69,

        BREACHCHARGE = 70,

        TABLET = 72,

        MELEE = 74,

        AXE = 75,

        HAMMER = 76,

        SPANNER = 78,

        KNIFE_GHOST = 80,

        FIREBOMB = 81,

        DIVERSION = 82,

        FRAG_GRENADE = 83,

        SNOWBALL = 84,

        BUMPMINE = 85,

        BAYONET = 500,

        KNIFE_FLIP = 505,

        KNIFE_GUT = 506,

        KNIFE_KARAMBIT = 507,

        KNIFE_M9_BAYONET = 508,

        KNIFE_TACTICAL = 509,

        KNIFE_FALCHION = 512,

        KNIFE_SURVIVAL_BOWIE = 514,

        KNIFE_BUTTERFLY = 515,

        KNIFE_PUSH = 516,

        KNIFE_URSUS = 519,

        KNIFE_GYPSY_JACKKNIFE = 520,

        KNIFE_STILETTO = 522,

        KNIFE_WIDOWMAKER = 523,

    };
    
}
