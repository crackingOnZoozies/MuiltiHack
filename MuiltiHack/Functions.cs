using NAudio.Wave;
using Swed64;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static void Trigger(Swed swed,IntPtr client, IntPtr entityList, IntPtr localPlayerPawn, CancellationToken token, int aimdelay, bool autoShoot)
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
                    if (GetAsyncKeyState(HOTKEY) < 0 || autoShoot)
                    {
                        Thread.Sleep(aimdelay);
                        swed.WriteInt(client, Offsets.attack, 65537); // + attack
                        Thread.Sleep(10);
                        swed.WriteInt(client, Offsets.attack, 16777472); // - attack
                        Thread.Sleep(10);
                    }
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

        //imports
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);
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