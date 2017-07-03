using System;
using Microsoft.SPOT;

namespace fez_spider
{
    class Xtea
    {
        /*      XTEA    */

        private const int DELTA = unchecked((int)0x9E3779B9);
        private int sum;
        private int k0, k1, k2, k3, k4, k5, k6, k7, k8, k9, k10, k11, k12, k13, k14, k15;
        private int k16, k17, k18, k19, k20, k21, k22, k23, k24, k25, k26, k27, k28, k29, k30, k31;

        private const int _align = 16;

        public static int Align { get { return _align; } }

        public Xtea(byte[] key)
        {

            this.sum = 0;
            this.setKey(key);


        }

        public void encrypt(byte[] bytes, int off, int len)
        {
            if (len % Align != 0)
            {
                Debug.Print(len + "unaligned with" + Align);
                return;
            }
            for (int i = off; i < off + len; i += 8)
                encryptBlock(bytes, bytes, i);

        }



        private void setKey(byte[] b)
        {
            int[] key = new int[4];
            for (int i = 0; i < 16;)
            {
                key[i / 4] = (b[i++] << 24) + ((b[i++] & 255) << 16) + ((b[i++] & 255) << 8) + (b[i++] & 255);
            }
            int[] r = new int[32];
            for (int i = 0, sum = 0; i < 32;)
            {
                r[i++] = sum + key[sum & 3];
                sum += DELTA;
                r[i++] = sum + key[((int)((uint)sum >> 11)) & 3];
            }
            k0 = r[0]; k1 = r[1]; k2 = r[2]; k3 = r[3]; k4 = r[4]; k5 = r[5]; k6 = r[6]; k7 = r[7];
            k8 = r[8]; k9 = r[9]; k10 = r[10]; k11 = r[11]; k12 = r[12]; k13 = r[13]; k14 = r[14]; k15 = r[15];
            k16 = r[16]; k17 = r[17]; k18 = r[18]; k19 = r[19]; k20 = r[20]; k21 = r[21]; k22 = r[22]; k23 = r[23];
            k24 = r[24]; k25 = r[25]; k26 = r[26]; k27 = r[27]; k28 = r[28]; k29 = r[29]; k30 = r[30]; k31 = r[31];
        }

        private void encryptBlock(byte[] input, byte[] output, int off)
        {
            int y = (input[off] << 24) | ((input[off + 1] & 255) << 16) | ((input[off + 2] & 255) << 8) | (input[off + 3] & 255);
            int z = (input[off + 4] << 24) | ((input[off + 5] & 255) << 16) | ((input[off + 6] & 255) << 8) | (input[off + 7] & 255);
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k0; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k1;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k2; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k3;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k4; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k5;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k6; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k7;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k8; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k9;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k10; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k11;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k12; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k13;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k14; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k15;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k16; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k17;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k18; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k19;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k20; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k21;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k22; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k23;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k24; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k25;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k26; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k27;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k28; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k29;
            y += (((z << 4) ^ ((int)((uint)z >> 5))) + z) ^ k30; z += ((((int)((uint)y >> 5)) ^ (y << 4)) + y) ^ k31;
            output[off] = (byte)(y >> 24); output[off + 1] = (byte)(y >> 16); output[off + 2] = (byte)(y >> 8); output[off + 3] = (byte)y;
            output[off + 4] = (byte)(z >> 24); output[off + 5] = (byte)(z >> 16); output[off + 6] = (byte)(z >> 8); output[off + 7] = (byte)z;
        }



        /*      XTEA    */
    }
}
