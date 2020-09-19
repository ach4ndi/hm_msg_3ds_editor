using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BokujoMessage4.Format.gformat
{
    public class ppItemData
    {
        public string VariableTypeConfig; //shift-jis
        public string TextVar; //Unicode
        public string ModelVar; //shift-jis
        public string InfoVariableConfig; //shift-jis
        public uint[] unkw;
        private uint[] odatas;
        private GameSeries series;

        public uint DataLenght;
        public uint DataCount;

        public ppItemData(byte[] input, GameSeries sres)
        {
            series = sres;

            MemoryStream strinput = new MemoryStream(input);
            BinaryReader binread = new BinaryReader(strinput);

            DataLenght = binread.ReadUInt32();
            DataCount = binread.ReadUInt32();

            odatas = new uint[DataCount];

            switch (series)
            {
                case GameSeries.SOS2JP:
                    for (int i = 0; i < DataCount; i++)
                    {
                        odatas[i] = binread.ReadUInt32();
                    }

                    unkw = new uint[10];

                    binread.BaseStream.Position = odatas[0];
                    VariableTypeConfig = Encoding.GetEncoding(932).GetString(binread.ReadBytes((int)(odatas[1] - odatas[0]))).Replace("\0", "");

                    if (VariableTypeConfig.Equals("ItemData", StringComparison.Ordinal) == false) return;

                    binread.BaseStream.Position = odatas[1];
                    TextVar = Encoding.Unicode.GetString(binread.ReadBytes((int)(odatas[2] - odatas[1]))).Replace("\0", "");

                    binread.BaseStream.Position = odatas[2];
                    ModelVar = Encoding.GetEncoding(932).GetString(binread.ReadBytes((int)(odatas[11] - odatas[2]))).Replace("\0", "");

                    binread.BaseStream.Position = odatas[11];
                    InfoVariableConfig = Encoding.GetEncoding(932).GetString(binread.ReadBytes((int)(DataLenght - odatas[11]))).Replace("\0", "");

                    for (int i = 0; i < 8; i++)
                    {
                        unkw[i] = odatas[i + 3];
                    }

                    unkw[8] = odatas[12];
                    unkw[9] = odatas[13];

                    // 14 data~
                    break;
            }

            binread.Close();
            strinput.Close();
        }

        public byte[] GetData(GameSeries sres, params object[] input)
        {
            MemoryStream strinput = null;
            BinaryWriter binread;

            byte[] getdatatemp;
            int lenghtinput = 0;

            switch (series)
            {
                case GameSeries.SOS2JP:
                    /* make sure input is 13 data :
                     * 0 - Text name
                     * 1 - model name
                     * 2 - INFO var name
                     * 3 - /*rest unknown value*/ //8-, -2

                    lenghtinput += (4*14) + (8);

                    int variabletxtlenght = Encoding.GetEncoding(932).GetBytes("ItemData").Length;
                    int minusrecal = Utils.RecountLenght(variabletxtlenght);
                    variabletxtlenght += minusrecal;
                    lenghtinput += variabletxtlenght;

                    int texttxtlenght = Encoding.Unicode.GetBytes((string)input[0]).Length;
                    int minusrecal1 = Utils.RecountLenght(texttxtlenght);
                    texttxtlenght += minusrecal1;
                    lenghtinput += texttxtlenght;

                    int modelstrtxtlenght = Encoding.GetEncoding(932).GetBytes((string)input[1]).Length;
                    int minusrecal2 = Utils.RecountLenght(modelstrtxtlenght);
                    modelstrtxtlenght += minusrecal2;
                    lenghtinput += modelstrtxtlenght;

                    int infostrtxtlenght = Encoding.GetEncoding(932).GetBytes((string)input[2]).Length;
                    int minusrecal3 = Utils.RecountLenght(infostrtxtlenght);
                    infostrtxtlenght += minusrecal3;
                    lenghtinput += infostrtxtlenght;

                    getdatatemp = new byte[lenghtinput];

                    strinput = new MemoryStream(getdatatemp);
                    binread = new BinaryWriter(strinput);

                    binread.Write(BitConverter.GetBytes(lenghtinput), 0, 4);
                    binread.Write(BitConverter.GetBytes(14), 0, 4);

                    binread.Write(BitConverter.GetBytes(0x40), 0, 4);
                    binread.Write(BitConverter.GetBytes(0x40 + variabletxtlenght), 0, 4);
                    binread.Write(BitConverter.GetBytes(0x40 + variabletxtlenght + texttxtlenght), 0, 4);

                    binread.Write(BitConverter.GetBytes((uint)input[3]), 0, 4);
                    binread.Write(BitConverter.GetBytes((uint)input[4]), 0, 4);
                    binread.Write(BitConverter.GetBytes((uint)input[5]), 0, 4);
                    binread.Write(BitConverter.GetBytes((uint)input[6]), 0, 4);
                    binread.Write(BitConverter.GetBytes((uint)input[7]), 0, 4);
                    binread.Write(BitConverter.GetBytes((uint)input[8]), 0, 4);
                    binread.Write(BitConverter.GetBytes((uint)input[9]), 0, 4);
                    binread.Write(BitConverter.GetBytes((uint)input[10]), 0, 4);

                    binread.Write(BitConverter.GetBytes(0x40 + variabletxtlenght + texttxtlenght
                        + modelstrtxtlenght), 0, 4);

                    binread.Write(BitConverter.GetBytes((uint)input[11]), 0, 4);
                    binread.Write(BitConverter.GetBytes((uint)input[12]), 0, 4);

                    binread.Write(Encoding.GetEncoding(932).GetBytes("ItemData"));
                    binread.Write(new byte[minusrecal]);
                    binread.Write(Encoding.Unicode.GetBytes((string)input[0]));
                    binread.Write(new byte[minusrecal1]);
                    binread.Write(Encoding.GetEncoding(932).GetBytes((string)input[1]));
                    binread.Write(new byte[minusrecal2]);
                    binread.Write(Encoding.GetEncoding(932).GetBytes((string)input[2]));
                    binread.Write(new byte[minusrecal3]);
                    break;
            }

            return strinput.ToArray();
        }
    }
}
