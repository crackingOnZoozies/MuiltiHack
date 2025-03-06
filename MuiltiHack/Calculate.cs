using Swed64;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MuiltiHack
{
    public static class Calculate
    {
        public static Vector2 CalculateAngles(Vector3 from, Vector3 to)
        {
            float yaw;
            float pitch;

            //calc yaw
            float deltaX = to.X - from.X;
            float deltaY = to.Y - from.Y;
            yaw = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI);

            //calc pitch
            float deltaZ = to.Z - from.Z;
            double distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            pitch = -(float)(Math.Atan2(deltaZ, distance) * 180 / Math.PI);

            return new Vector2(yaw, pitch);
        }

        public static Vector2 WordToScreen(float[] matrix, Vector3 pos, Vector2 windowSize)
        {
            // calculate screenW
            float screenW = (matrix[12] * pos.X) + (matrix[13] * pos.Y) + (matrix[14] * pos.Z) + matrix[15];

            //if entity in front of us
            if (screenW > 0.001f)
            {
                // calc screen x and y
                float screenX = (matrix[0] * pos.X) + (matrix[1] * pos.Y) + (matrix[2] * pos.Z) + matrix[3];
                float screenY = (matrix[4] * pos.X) + (matrix[5] * pos.Y) + (matrix[6] * pos.Z) + matrix[7];

                //perform perspective division
                float X = (windowSize.X / 2) + (windowSize.X / 2) * screenX / screenW;
                float Y = (windowSize.Y / 2) - (windowSize.Y / 2) * screenY / screenW;

                return new Vector2(X, Y);
            }
            else
            {
                Console.WriteLine("errror");
                return new Vector2(-99, -99);
            }
        }

        //skeleton
        public static List<Vector3> ReadBones(IntPtr boneAddress, Swed swed)
        {
            byte[] boneBytes = swed.ReadBytes(boneAddress, 27 * 32 + 16); // get max, 27 = id, 32 = step

            List<Vector3> bones = new List<Vector3>();

            foreach (var boneId in Enum.GetValues(typeof(BonesIds))) // loop throuh enums
            {
                float x = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 0);
                float y = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 4); //float = 4 bytes
                float z = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 8);

                Vector3 currentBone = new Vector3(x, y, z);
                bones.Add(currentBone);
            }
            return bones;
        }

        public static List<Vector2> ReadBones2d(List<Vector3> bones, float[] viewMatrix, Vector2 screenSize)
        {
            List<Vector2> bones2d = new List<Vector2>();

            foreach (Vector3 bone in bones)
            {
                Vector2 bone2d = Calculate.WordToScreen(viewMatrix, bone, screenSize);

                bones2d.Add(bone2d);
            }
            return bones2d;
        }
    }
}
