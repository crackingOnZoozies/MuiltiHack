using NAudio.Wave;
using Swed64;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MuiltiHack
{
    public class Functions
    {
        //hotKey
        const int SPACE_BAR = 0x20;

        //trigger hotkey
        const int HOTKEY = 0x06; //mouse5 = 0x06 alt = 0x12

        //mouse events consts
        const uint L_MouseDown = 0x02;
        const uint L_MouseUp = 0x04;

        //player states
        const uint standing = 65665;
        const uint crouching = 65667;

        //jump + -
        const uint plusJump = 65637;
        const uint minusJump = 256;

        //ckouching
        const uint crouch = 65537;
        const uint stopCrouch = 16777472;

        public static void AntiFlash(Swed swed, IntPtr localPlayerPawn, CancellationToken token)
        {
            while (true)
            {
                // Проверяем, не была ли запрошена отмена задачи
                if (token.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                    continue;
                }

                float flashDuration = swed.ReadFloat(localPlayerPawn, Offsets.m_flFlashBangTime); // 0->1
                if (flashDuration > 0)
                {
                    swed.WriteFloat(localPlayerPawn, Offsets.m_flFlashBangTime, 0);
                }
                Thread.Sleep(10);

            }
        }

        public static void Bhop(Swed swed, IntPtr client, IntPtr playerPawn, CancellationToken token, Renderer renderer)
        {
            IntPtr forcejumpAddress = client + Offsets.jump;
            IntPtr crouchAddy = client + Offsets.duck;

            while (true)
            {
                // Проверяем, не была ли запрошена отмена задачи
                if (token.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                    continue;
                }

                uint fFlag = swed.ReadUInt(playerPawn, Offsets.m_fFlags);
                if (GetAsyncKeyState(SPACE_BAR) < 0 || renderer.autoBhop)
                {
                    if (fFlag == standing || fFlag == crouching)
                    {

                        if (!renderer.legit)
                        {
                            Thread.Sleep(1);
                            swed.WriteUInt(forcejumpAddress, plusJump);
                            swed.WriteUInt(crouchAddy, crouch);

                            Thread.Sleep(10);
                            swed.WriteUInt(forcejumpAddress, minusJump);
                            swed.WriteUInt(crouchAddy, stopCrouch);
                        }
                        else
                        {
                            Thread.Sleep(1);
                            swed.WriteUInt(forcejumpAddress, plusJump);

                            Thread.Sleep(10);
                            swed.WriteUInt(forcejumpAddress, minusJump);
                        }
                        
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
            }
        }

        public static void Radar(Swed swed, IntPtr entityList, IntPtr listentry, CancellationToken token)
        {
            while (true)
            {
                // Проверяем, не была ли запрошена отмена задачи
                if (token.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                    continue;
                }


                for (int i = 0; i < 64; i++)
                {
                    if (listentry == IntPtr.Zero) continue;

                    IntPtr currentController = swed.ReadPointer(listentry, i * 0x78);

                    if (currentController == IntPtr.Zero) continue;

                    int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);

                    if (pawnHandle == 0) continue;

                    IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);

                    IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));

                    string name = swed.ReadString(currentController, Offsets.m_iszPlayerName, 16);

                    bool spotted = swed.ReadBool(currentPawn, Offsets.m_entitySpottedState + Offsets.m_bSpotted);

                    swed.WriteBool(currentPawn, Offsets.m_entitySpottedState + Offsets.m_bSpotted, true);




                }
                Thread.Sleep(50);

            }
        }

        public static void Trigger(Swed swed,IntPtr client, IntPtr entityList, IntPtr localPlayerPawn, CancellationToken token, int aimdelay, bool triggerShoot, bool autoShootAimbot, bool legit)
        {

            //get our team and crosshair id
            int team = swed.ReadInt(localPlayerPawn, Offsets.m_iTeamNum);
            int entIndex = swed.ReadInt(localPlayerPawn, Offsets.m_iIDEntIndex);

            if (entIndex != -1)
            {
                //get controller from entity from entity index
                IntPtr listEntry = swed.ReadPointer(entityList, 0x8 * ((entIndex & 0x7FFF) >> 9) + 0x10);

                //then get pawn from that controller
                IntPtr currentPawn = swed.ReadPointer(listEntry, 0x78 * (entIndex & 0x1FF));

                //get the team
                int entityTeam = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);

                if (team != entityTeam)
                {
                    //check for hotkey
                    if (GetAsyncKeyState(HOTKEY) < 0 || triggerShoot)
                    {
                        Thread.Sleep(aimdelay);

                        if (legit)
                        {
                            mouse_event(L_MouseDown,0,0,0,IntPtr.Zero);
                            Thread.Sleep(10);
                            mouse_event(L_MouseUp,0,0,0,IntPtr.Zero);
                            Thread.Sleep(10);
                        }
                        else
                        {
                            swed.WriteInt(client, Offsets.attack, 65537); // + attack
                            Thread.Sleep(10);
                            swed.WriteInt(client, Offsets.attack, 16777472); // - attack
                            Thread.Sleep(10);
                        }
                        
                    }
                }
            }
            else if (autoShootAimbot)
            {
                if (GetAsyncKeyState(HOTKEY) < 0 )
                {
                    Thread.Sleep(aimdelay);
                    if (legit)
                    {
                        mouse_event(L_MouseDown, 0, 0, 0, IntPtr.Zero);
                        Thread.Sleep(10);
                    }
                    swed.WriteInt(client, Offsets.attack, 65537); // + attack
                    Thread.Sleep(10);
                    swed.WriteInt(client, Offsets.attack, 16777472); // - attack
                    Thread.Sleep(10);
                }
            }
            
            Thread.Sleep(2); // let cpu rest
        }

        public static void BombTimer(Swed swed, IntPtr GameRules, CancellationToken token, Renderer renderer)
        {
            bool bBombPlanted = false;
            
            if (GameRules != IntPtr.Zero)
            {
                bBombPlanted = swed.ReadBool(GameRules, Offsets.m_bBombPlanted);
                if (bBombPlanted)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        bBombPlanted = swed.ReadBool(GameRules, Offsets.m_bBombPlanted);
                        if (!bBombPlanted)
                            break;
                        else
                        {
                            renderer.timeLeft = 40 - i;
                            renderer.bombPlanted = true;
                        }
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    renderer.timeLeft = -1;
                    renderer.bombPlanted = false;
                    Thread.Sleep(5);
                }
            }
        }

        public static void ESP(Swed swed, IntPtr client,IntPtr entityList, IntPtr listEntry, Renderer renderer, CancellationToken token)
        {
            //get screen size
            Vector2 screenSize = renderer.screenSize;

            //store enteties
            List<Entity> entities = new List<Entity>();
            Entity localPlayer = new Entity();

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                    continue;
                }

                IntPtr localPlayerPawn = swed.ReadPointer(client,Offsets.dwLocalPlayerPawn);
                entities.Clear();

                //getting our team
                localPlayer.team = swed.ReadInt(localPlayerPawn, Offsets.m_iTeamNum);

                //loop through entity list
                for (int i = 1; i < 64; i++)
                {
                    if (listEntry == IntPtr.Zero) continue;

                    IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);

                    if (currentController == IntPtr.Zero) continue;

                    int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);

                    if (pawnHandle == 0) continue;

                    IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);

                    IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));

                    //check lifestate
                    int lifeState = swed.ReadInt(currentPawn, Offsets.m_lifeState);
                    if (lifeState != 256) continue;

                    IntPtr sceneNode = swed.ReadPointer(currentPawn, Offsets.m_pGameSceneNode);
                    IntPtr boneMatrix = swed.ReadPointer(sceneNode, Offsets.m_modelState + 0x80);

                    //get matrix
                    float[] viewMatrix = swed.ReadMatrix(client + Offsets.dwViewMatrix);

                    IntPtr currentWeapon = swed.ReadPointer(currentPawn, Offsets.m_pClippingWeapon);

                    // get item defenition index
                    short weponDefenitionIndex = swed.ReadShort(currentWeapon, Offsets.m_AttributeManager + Offsets.m_Item + Offsets.m_iItemDefinitionIndex);

                    //populate entities
                    Entity entity = new Entity();

                    entity.spotted = swed.ReadBool(currentPawn, Offsets.m_entitySpottedState + Offsets.m_bSpotted);
                    entity.scoped = swed.ReadBool(currentPawn, Offsets.m_bOldIsScoped);

                    entity.name = swed.ReadString(currentController, Offsets.m_iszPlayerName, 16).Split("\0")[0];// reading name

                    entity.team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
                    entity.health = swed.ReadInt(currentPawn, Offsets.m_iHealth);// reading hp

                    entity.position = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
                    entity.viewOffset = swed.ReadVec(currentPawn, Offsets.m_vecViewOffset);

                    entity.position2d = Calculate.WordToScreen(viewMatrix, entity.position, screenSize);
                    entity.viewPosition2D = Calculate.WordToScreen(viewMatrix, Vector3.Add(entity.position, entity.viewOffset), screenSize);

                    entity.distance = Vector3.Distance(entity.position, localPlayer.position);

                    entity.bones = Calculate.ReadBones(boneMatrix, swed);

                    entity.bones2d = Calculate.ReadBones2d(entity.bones, viewMatrix, renderer.screenSize);
                    entity.currentWeaponIndex = weponDefenitionIndex;
                    entity.currentWeaponName = Enum.GetName(typeof(Weapon), weponDefenitionIndex);
                    entities.Add(entity);
                }

                //update enteties
                renderer.UpdateLocalPlayer(localPlayer);
                renderer.UpdateEntities(entities);

                //let spu rest
                Thread.Sleep(5);

            }
        }

        public static void AimBot(Swed swed, IntPtr client, IntPtr entityList, IntPtr localPlayerPawn, IntPtr listEntry, Renderer renderer, CancellationToken token)
        {
            const int hotKeyAimSwitch = 0x06;//mouse 5 0x06;//mouse 6

            const int hotKeyLeft = 0x01; // mouse left

            const int PlusAttack = 65537;
            const int MinusAttack = 256;
            const int attackMinus2 = 16777472;

            List<Entity> entities = new List<Entity>();
            Entity localPlayer = new Entity();

            //aimbot loop
            while (true)
            {
                entities.Clear();


                localPlayer.pawnAddress = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
                localPlayer.team = swed.ReadInt(localPlayer.pawnAddress, Offsets.m_iTeamNum);
                localPlayer.origin = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vOldOrigin);
                localPlayer.view = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vecViewOffset);

                int shotsFired = swed.ReadInt(localPlayer.pawnAddress, Offsets.m_iShotsFired);

                //loop thogh entity list
                for (int i = 0; i < 64; i++)
                {
                    //if (GetAsyncKeyState(hotKey)<0) renderer.aimbot = !renderer.aimbot;
                    if (!renderer.aimbot) continue;
                    if (listEntry == IntPtr.Zero)
                    {
                        Console.WriteLine(listEntry);
                    }

                    IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);

                    if (currentController == IntPtr.Zero) continue;

                    //get pawn
                    int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);

                    if (pawnHandle == 0) continue;

                    //second entry and now we get specific pawn
                    IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);
                    IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));

                    if (currentPawn == localPlayer.pawnAddress) continue;

                    //get scene node
                    IntPtr sceneNode = swed.ReadPointer(currentPawn, Offsets.m_pGameSceneNode);

                    //get bone array
                    IntPtr boneMatrix = swed.ReadPointer(sceneNode, Offsets.m_modelState + 0x80);

                    uint lifeState = swed.ReadUInt(currentPawn, Offsets.m_lifeState);

                    if (lifeState != 256) continue;

                    int health = swed.ReadInt(currentPawn, Offsets.m_iHealth);
                    int team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
                    bool spotted = swed.ReadBool(currentPawn, Offsets.m_entitySpottedState + Offsets.m_bSpotted);

                    if (!spotted && renderer.aimOnSpotted) continue;

                    if (team == localPlayer.team && !renderer.aimOnTeam)
                    {
                        continue;
                    }


                    Entity entity = new Entity();

                    //get matrix
                    float[] viewMatrix = swed.ReadMatrix(client + Offsets.dwViewMatrix);

                    entity.pawnAddress = currentPawn;
                    entity.controllerPawn = currentController;

                    entity.health = health;
                    entity.team = team;
                    entity.lifeState = lifeState;

                    entity.origin = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
                    entity.view = swed.ReadVec(currentPawn, Offsets.dwViewAngles);

                    entity.distance = Vector3.Distance(entity.origin, localPlayer.origin);
                    entity.head = swed.ReadVec(boneMatrix, 6 * 32); // 6 =bone id,  32 = steps between  bones

                    //get 2d info
                    entity.head2d = Calculate.WordToScreen(viewMatrix, entity.head,renderer.screenSize);
                    entity.pixelDistance = Vector2.Distance(entity.head2d, new Vector2(renderer.screenSize.X / 2, renderer.screenSize.Y / 2));

                    entities.Add(entity);

                    //draw to cmd
                    Console.ForegroundColor = team == localPlayer.team ? ConsoleColor.Green : ConsoleColor.Red;

                    Console.WriteLine($"{entity.health} hp, head coord: {entity.head}");
                    Console.ResetColor();
                }

                localPlayer.scopped = swed.ReadBool(Offsets.dwLocalPlayerPawn, Offsets.m_bOldIsScoped);



                if (!renderer.aimbot) { continue; }

                bool swedBool = swed.ReadInt(client + Offsets.attack) == PlusAttack;
                if (renderer.aimKeySecond) swedBool = GetAsyncKeyState(hotKeyAimSwitch) < 0;
                if (renderer.autoLock) swedBool = true;


                if (renderer.aimOnClosest)
                {
                    entities = entities.OrderBy(o => o.distance).ToList();
                    if (entities.Count > 0 && (swedBool))
                    {
                        if (renderer.autoLockMaxDistance < entities[0].distance && renderer.autoLock)
                        {
                            continue;
                        }
                        float y = swed.ReadFloat(client + Offsets.dwViewAngles);
                        float x = swed.ReadFloat(client + Offsets.dwViewAngles + 0x4);
                        //get view pos
                        Vector3 playerView = Vector3.Add(localPlayer.origin, localPlayer.view);
                        Vector3 entityView = Vector3.Add(entities[0].origin, entities[0].view);

                        //get angles
                        Vector2 newAngles = Calculate.CalculateAngles(playerView, entities[0].head);
                        Vector3 newNagles3D = new Vector3(newAngles.Y, newAngles.X, 0.0f);

                        swed.WriteVec(client, Offsets.dwViewAngles, newNagles3D);
                        Thread.Sleep(renderer.aimDelay);

                    }
                }
                else
                {
                    entities = entities.OrderBy(o => o.pixelDistance).ToList();
                    if (entities.Count > 0 && (swedBool))
                    {
                        if (renderer.autoLockMaxDistance < entities[0].distance && renderer.autoLock)
                        {
                            continue;
                        }
                        float y = swed.ReadFloat(client + Offsets.dwViewAngles);
                        float x = swed.ReadFloat(client + Offsets.dwViewAngles + 0x4);
                        //get view pos
                        Vector3 playerView = Vector3.Add(localPlayer.origin, localPlayer.view);
                        Vector3 entityView = Vector3.Add(entities[0].origin, entities[0].view);

                        //get angles
                        Vector2 newAngles = Calculate.CalculateAngles(playerView, entities[0].head);
                        Vector3 newNagles3D = new Vector3(newAngles.Y, newAngles.X, 0.0f);

                        if (entities[0].pixelDistance < renderer.FOV && renderer.useFov)
                        {
                            swed.WriteVec(client, Offsets.dwViewAngles, newNagles3D);
                            Thread.Sleep(renderer.aimDelay);


                        }
                        else if (renderer.FOV > 0 && !renderer.useFov)
                        {
                            swed.WriteVec(client, Offsets.dwViewAngles, newNagles3D);
                            Thread.Sleep(renderer.aimDelay);


                        }

                    }
                }
                

            }

        }

        public static void fovChanger(Swed swed, IntPtr client, Renderer renderer, CancellationToken token)
        {
            while (true)
            {
                
                uint desiredFov = (uint)renderer.FovChangerFOV;
                IntPtr localPlayer = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
                IntPtr cameraServices = swed.ReadPointer(localPlayer, Offsets.m_pCameraServices);
                uint currentFov = swed.ReadUInt(cameraServices + Offsets.m_iFOV);

                bool ifScoped = swed.ReadBool(localPlayer, Offsets.m_bIsScoped);

                if (renderer.ignorescoping && currentFov != desiredFov)
                {
                    swed.WriteUInt(cameraServices + Offsets.m_iFOV, desiredFov);
                }
                else if (!ifScoped && currentFov != desiredFov)
                {
                    swed.WriteUInt(cameraServices + Offsets.m_iFOV, desiredFov);
                }


            }
        }

        public static void RCS(Swed swed, IntPtr client, CancellationToken token)
        {
            Vector3 oldPunch = new Vector3(0, 0, 0);
            while (true)
            {
                // Проверяем, не была ли запрошена отмена задачи
                if (token.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                    continue;
                }

                Thread.Sleep(1);


                IntPtr localPlyer = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);

                int shotsFired = swed.ReadInt(localPlyer, Offsets.m_iShotsFired);


                if (shotsFired != 0)
                {

                    float y = swed.ReadFloat(client + Offsets.dwViewAngles);
                    float x = swed.ReadFloat(client + Offsets.dwViewAngles + 0x4);
                    Vector3 aimPunch = swed.ReadVec(localPlyer, Offsets.m_aimPunchAngle);

                    Vector3 NewViewAngles = new Vector3(

                        y + oldPunch.X - aimPunch.X * 2f,
                        x + oldPunch.Y - aimPunch.Y * 2f,
                        0
                        );
                    if (NewViewAngles.X < -89) NewViewAngles.X = -89;
                    if (NewViewAngles.X > 89) NewViewAngles.X = 89;

                    if (NewViewAngles.Y < -179)
                        NewViewAngles.Y = 179 + (NewViewAngles.Y + 179);
                    if (NewViewAngles.Y > 179)
                        NewViewAngles.Y = -179 - (NewViewAngles.Y - 179);

                    swed.WriteVec(client, Offsets.dwViewAngles, NewViewAngles);
                    oldPunch.X = aimPunch.X * 2f;
                    oldPunch.Y = aimPunch.Y * 2f;
                    Console.WriteLine($"y:{y}, x :{x}");
                }
                else
                {
                    oldPunch.X = oldPunch.Y = oldPunch.Z = 0.0f;

                }
            }
        }

        public static void AutoPistolShoting(Swed swed, IntPtr client, CancellationToken token, Renderer renderer)
        {
            int[] ints = {1,2,3,4,30,32,36,60,63,64,2000 };
            while (true)
            {

                // Проверяем, не была ли запрошена отмена задачи
                if (token.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                    continue;
                }

                Thread.Sleep(1);

                IntPtr localPlyer = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);

                IntPtr currentWeapon = swed.ReadPointer(localPlyer, Offsets.m_pClippingWeapon);
                // get item defenition index
                short weponDefenitionIndex = swed.ReadShort(currentWeapon, Offsets.m_AttributeManager + Offsets.m_Item + Offsets.m_iItemDefinitionIndex);
                foreach (var item in ints)
                {
                    if (item == weponDefenitionIndex && GetAsyncKeyState(0x01) < 0)
                    {

                        swed.WriteInt(client, Offsets.attack, 65537); // + attack
                        Thread.Sleep(10);
                        swed.WriteInt(client, Offsets.attack, 16777472); // - attack
                        Thread.Sleep(10);


                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

            }
        }

        //imports
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFalags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);
    }

}

public class AudioPlayer
{
    private string soundsFolder;

    public AudioPlayer(string soundsFolderPath)
    {
        // Убедимся, что папка существует
        if (!Directory.Exists(soundsFolderPath))
        {
            throw new DirectoryNotFoundException($"Папка {soundsFolderPath} не найдена!");
        }

        soundsFolder = soundsFolderPath;
    }

    public void PlaySound(string fileName, float volume = 1.0f)
    {
        string filePath = Path.Combine(soundsFolder, fileName);

        // Проверяем, существует ли файл
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Файл {fileName} не найден в папке {soundsFolder}!");
            return;
        }

        // Воспроизведение MP3
        using (var mp3Reader = new Mp3FileReader(filePath))
        using (var waveOut = new WaveOutEvent())
        {
            // Устанавливаем громкость
            waveOut.Volume = volume;

            waveOut.Init(mp3Reader);
            waveOut.Play();

            // Ожидание завершения воспроизведения
            while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}