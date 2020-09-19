using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BokujoMessage4.Format.gformat
{
    public class uniConfigSOS2JP
    {
        private ConfigHeader e;
        private string getdata;

        public uniConfigSOS2JP(byte[] input)
        {
            e = ReadValueList(input);
        }

        public string getDataStr()
        {
            return getdata;
        }

        private ConfigHeader ReadValueList(byte[] input)
        {
            ConfigHeader d;
            d = new ConfigHeader();
            MemoryStream b = new MemoryStream(input);
            BinaryReader c = new BinaryReader(b);
            d.FileLenght = c.ReadUInt32();
            d.FileCount = c.ReadUInt32();
            d.ConfigVal = new ConfigValList[d.FileCount];

            for (int i = 0; i < d.FileCount; i++)
            {
                d.ConfigVal[i].Value = c.ReadUInt32();
            }

            if (b.Length == 0 || d.FileCount ==0) return d;

            d.ConfigType = getConfigValue(d, c);
            getPrepdata(d, c);

            c.Close();
            b.Flush();
            b.Close();
            return d;
        }

        private void ApplyData(int position, int secondposition, StringBuilder sb, ConfigHeader u, BinaryReader br, TypeData encoding = TypeData.ShiftJISTring, bool end = false)
        {
            if (!end)
            {
                br.BaseStream.Position = u.ConfigVal[position].Value;
                sb.Append(
                    getValObject(
                        br.ReadBytes((int)(u.ConfigVal[secondposition].Value - u.ConfigVal[position].Value)), encoding))
                        .Append("	");
            }
            else
            {
                br.BaseStream.Position = u.ConfigVal[position].Value;
                sb.Append(getValObject(br.ReadBytes((int)(u.FileLenght - u.ConfigVal[position].Value)), encoding)).Append("	");
            }
        }

        private void ApplyNums(int position, int secondposition, StringBuilder sb, ConfigHeader u, int mode = 0)
        {
            for (int i = position; i < secondposition; i++)
            {
                switch (mode)
                {
                    case 1:
                        sb.Append(BitConverter.ToSingle(BitConverter.GetBytes(u.ConfigVal[i].Value),0)).Append("	");
                        break;
                    case 2:
                        //sb.Append(unchecked(u.ConfigVal[i].Value)).Append("	");
                        sb.Append(BitConverter.ToInt32(BitConverter.GetBytes(u.ConfigVal[i].Value), 0)).Append("	");
                        break;
                    default:
                        sb.Append(u.ConfigVal[i].Value).Append("	");
                        break;
                }
            }
        }

        private void getPrepdata(ConfigHeader u, BinaryReader br)
        {
            StringBuilder sb = new StringBuilder();

            //LightFogData~

            sb.Append(u.ConfigType).Append("	").Append(u.FileCount).Append("	");

            switch (u.ConfigType)
            {
                #region #1
                case "DataText":
                    ApplyData(1, 2, sb, u, br, TypeData.UnicodeString);
                    ApplyData(2, 3, sb, u, br, TypeData.UnicodeString);
                    ApplyData(3, 4, sb, u, br, TypeData.UnicodeString);
                    ApplyData(4, 5, sb, u, br, TypeData.UnicodeString);
                    ApplyData(5, 5, sb, u, br, TypeData.UnicodeString);
                    break;
                case "ItemData":
                    ApplyData(1, 2, sb, u, br, TypeData.UnicodeString);
                    ApplyData(2, 11, sb, u, br);
                    ApplyNums(3, 11, sb, u);
                    ApplyData(11, 11, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(12, 14, sb, u);
                    break;
                case "AnimalActionData":
                    ApplyNums(1, 33, sb, u);
                    ApplyData(33, 34, sb, u, br);
                    ApplyData(34, 35, sb, u, br);
                    ApplyData(35, 35, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "CountryData":
                    ApplyData(1, 2, sb, u, br, TypeData.UnicodeString);
                    ApplyData(2, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "MajorCategoryData":
                case "FeedData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br);
                    ApplyData(3, 3, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(4, (int) u.FileCount, sb, u);
                    break;
                case "HutData":
                    for (int i = 1; i < 5; i++) { ApplyData(i, i+1, sb, u, br); }
                    ApplyData(5, 5, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(6, (int)u.FileCount, sb, u);
                    break;
                case "AnimalData":
                    for (int i = 1; i < 14; i++)
                    {
                        if (i == 12) { ApplyData(i, i + 1, sb, u, br, TypeData.UnicodeString); }
                        else { ApplyData(i, i + 1, sb, u, br); }
                    }
                    ApplyData(14, 17, sb, u, br);
                    ApplyNums(15, 17, sb, u);

                    for (int i = 17; i <20; i++)
                    {
                        if (i == 18) { ApplyData(i, i+1, sb, u, br, TypeData.UnicodeString); }
                        else {  ApplyData(i, i + 1, sb, u, br); }
                    }

                    ApplyData(20, 23, sb, u, br);
                    ApplyNums(21, 23, sb, u);

                    for (int i = 23; i <26; i++)
                    {
                        if (i == 24) { ApplyData(i, i + 1, sb, u, br, TypeData.UnicodeString); }
                        else { ApplyData(i, i + 1, sb, u, br); }
                    }

                    ApplyData(26, 29, sb, u, br);
                    ApplyNums(27, 29, sb, u);

                    for (int i = 29; i <32; i++)
                    {
                        if (i == 30) { ApplyData(i, i + 1, sb, u, br, TypeData.UnicodeString); }
                        else { ApplyData(i, i + 1, sb, u, br); }
                    }

                    ApplyData(32, 35, sb, u, br);
                    ApplyNums(33, 35, sb, u);

                    for (int i = 35; i <38; i++)
                    {
                        if (i == 36) { ApplyData(i, i + 1, sb, u, br, TypeData.UnicodeString); }
                        else { ApplyData(i, i + 1, sb, u, br); }
                    }

                    ApplyData(38, 44, sb, u, br);
                    //ApplyNums(39, 44, sb, u);
                    ApplyNums(39, 41, sb, u);
                    ApplyNums(41, 44, sb, u,2);
                    //
                    ApplyData(44, 44, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "NpcData":
                    ApplyData(1, 3, sb, u, br);
                    ApplyNums(2, 3, sb, u);

                    for (int i = 3; i < 20; i++) { ApplyData(i, i+1, sb, u, br); }
                    ApplyData(20, 23, sb, u, br);
                    ApplyNums(21, 23, sb, u);
                    ApplyData(23, 24, sb, u, br);
                    ApplyData(24, 26, sb, u, br);
                    ApplyNums(25, 26, sb, u);

                    for (int i = 26; i < u.FileCount - 1; i++) { ApplyData(i, i+1, sb, u, br); }

                    ApplyData((int) u.FileCount - 1, (int)u.FileCount - 1, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "MotionData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 5, sb, u, br);
                    ApplyNums(3, 5, sb, u);

                    for (int i = 5; i < 11; i++)
                    {
                        int basepos = i + (3 * (i - 5));
                        ApplyData(basepos, basepos + 4, sb, u, br);
                        ApplyNums(basepos + 1, basepos + 4, sb, u);
                    }

                    ApplyData(29, 29, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(30, 33, sb, u);
                    break;
                case "AnimalMotionData":
                    for (int i = 1; i < 34; i++)
                    {
                        int basepos = i + (3*(i - 1));
                        ApplyData(basepos, basepos + 4, sb, u, br);
                        ApplyNums(basepos + 1, basepos + 4, sb, u);
                    }

                    ApplyData(133, 133, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(134, 137, sb, u);
                    break;
                #endregion
                #region #2
                case "AnimalTreasureHuntData":
                    ApplyNums(1, 2, sb, u);
                    ApplyData(2, 9, sb, u, br);
                    ApplyNums(3, 7, sb, u);
                    ApplyNums(7, 9, sb, u,1);

                    for (int i = 9; i < u.FileCount - 1; i++) { ApplyData(i, i + 1, sb, u, br); }
                    ApplyData((int)u.FileCount - 1, (int)u.FileCount - 1, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "AnimalWildData":
                    for (int i = 1; i < 3; i++) { ApplyData(i, i + 1, sb, u, br); }

                    ApplyData(3, 5, sb, u, br);
                    ApplyNums(4, 5, sb, u);
                    ApplyData(5, 30, sb, u, br);

                    ApplyNums(6, 30, sb, u);
                    ApplyNums(31,(int) u.FileCount, sb, u);
                    ApplyData(30, 30, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "ClothesData":
                    for (int i = 1; i < 4; i++) // 12 - 2 = 10 - 2 = 8/2 = 4
                    {
                        br.BaseStream.Position = u.ConfigVal[i].Value; // 0-2 , 1-4

                        if (i == 1) { ApplyData(i, i+1, sb, u, br, TypeData.UnicodeString); }
                        else { ApplyData(i, i + 1, sb, u, br); }
                    }

                    ApplyData(4, 20, sb, u, br);
                    ApplyNums(5, 20, sb, u);
                    ApplyData(20, 20, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(21, 22, sb, u);
                    break;
                case "HatData":
                case "GlassesData":
                    for (int i = 1; i < 2; i++) // 12 - 2 = 10 - 2 = 8/2 = 4
                    {
                        if (i == 1) { ApplyData(i, i+1, sb, u, br, TypeData.UnicodeString); }
                        else { ApplyData(i, i + 1, sb, u, br); }
                    }

                    ApplyData(2, 17, sb, u, br);
                    ApplyNums(3, 17, sb, u);
                    ApplyData(17, 17, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(18, 19, sb, u);
                    break;
                case "Clothes":
                case "Glasses":
                case "Hat":
                    if (u.FileCount == 0x2a)
                    {
                        ApplyData(1, 2, sb, u, br);
                        ApplyData(2, 19, sb, u, br);
                        ApplyNums(3, 19, sb, u);
                        ApplyData(19, 22, sb, u, br);
                        ApplyNums(20, 22, sb, u);
                        ApplyData(22, 25, sb, u, br);
                        ApplyNums(23, 25, sb, u);
                        ApplyData(25, 28, sb, u, br);
                        ApplyNums(26, 28, sb, u);
                        ApplyData(28, 31, sb, u, br);
                        ApplyNums(29, 31, sb, u);
                        ApplyData(31, 34, sb, u, br);
                        ApplyNums(32, 34, sb, u);
                        ApplyData(34, 37, sb, u, br);
                        ApplyNums(35, 37, sb, u);
                        ApplyData(37, 40, sb, u, br);
                        ApplyNums(38, 40, sb, u);
                        ApplyData(40, 40, sb, u, br, TypeData.ShiftJISTring, true);
                        ApplyNums(41, (int)u.FileCount, sb, u);
                    }
                    else
                    {
                        ApplyData(1, 2, sb, u, br);

                        for (int i = 0; i < (u.FileCount - 4) / 2; i++) // 12 - 2 = 10 - 2 = 8/2 = 4
                        {
                            int basepos = (i + 1) * 2;
                            ApplyData(basepos, basepos + 2, sb, u, br);
                            ApplyNums(basepos + 1, basepos + 2, sb, u);
                        }

                        ApplyData((int)u.FileCount - 2, (int)u.FileCount - 2, sb, u, br, TypeData.ShiftJISTring, true);
                        ApplyNums((int)u.FileCount - 1, (int)u.FileCount, sb, u);
                    }
                    break;
                case "AtelierData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 4, sb, u, br);

                    sb.Append(u.ConfigVal[3].Value).Append("	");

                    for (int i = 4; i < 9; i++) { ApplyData(i, i+1, sb, u, br); }

                    ApplyData(9, 9, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "BathData":
                    ApplyNums(1, 5, sb, u);
                    ApplyData(5, 8, sb, u, br);
                    ApplyNums(6, 8, sb, u);
                    ApplyData(8, 11, sb, u, br);
                    ApplyNums(9, 11, sb, u);
                    ApplyData(11, 14, sb, u, br);
                    ApplyNums(12, 14, sb, u);
                    ApplyData(14, 17, sb, u, br);
                    ApplyNums(15, 17, sb, u);
                    ApplyData(17, 17, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(18, 20, sb, u);
                    break;
                case "CharaModelData":
                    ApplyData(1, 4, sb, u, br);
                    ApplyNums(2, 4, sb, u);
                    ApplyData(4, 4, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "CheckPointData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 9, sb, u, br);
                    ApplyNums(3, 9, sb, u);
                    ApplyData(9, 10, sb, u, br);
                    ApplyData(10, 10, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "NintendoCollaboData":
                    for (int i = 1; i < 3; i++) { ApplyData(i, i+1, sb, u, br); }

                    ApplyData(3, 5, sb, u, br);
                    ApplyNums(4, 5, sb, u);
                    ApplyData(5, 5, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "LithographData":
                    ApplyData(1, 3, sb, u, br, TypeData.UnicodeString);
                    ApplyNums(2, 3, sb, u);
                    ApplyData(3, 5, sb, u, br);
                    ApplyNums(4, 5, sb, u);
                    ApplyData(5, 6, sb, u, br);
                    ApplyData(6, 7, sb, u, br);
                    ApplyData(7, 8, sb, u, br);
                    ApplyData(8, 9, sb, u, br);
                    ApplyData(9, 9, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(10, (int)u.FileCount, sb, u);
                    break;
                case "CookRecipeGroup":
                    for (int i = 1; i < u.FileCount - 1; i++)
                    {
                        if (i == 1) { ApplyData(i, i+1, sb, u, br, TypeData.UnicodeString); }
                        else { ApplyData(i, i + 1, sb, u, br); }
                    }

                    ApplyData((int)u.FileCount - 1, (int) u.FileCount - 1, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "CookData":
                    for (int i = 1; i < 3; i++)
                    {
                        if (i == 1) { ApplyData(i, i + 1, sb, u, br, TypeData.UnicodeString); }
                        else { ApplyData(i, i + 1, sb, u, br); }
                    }

                    ApplyData(3, 6, sb, u, br);
                    ApplyNums(4, 6, sb, u);

                    for (int i = 6; i < u.FileCount - 1; i++) { ApplyData(i, i+1, sb, u, br); }

                    ApplyData((int)u.FileCount - 1, (int)u.FileCount - 1, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                #endregion
                #region #3
                case "CropData":
                    for (int i = 1; i < 4; i++)
                    {
                        if (i == 1) { ApplyData(i, i + 1, sb, u, br, TypeData.UnicodeString); }
                        else { ApplyData(i, i + 1, sb, u, br); }
                    }
                    ApplyData(4, 9, sb, u, br);
                    ApplyNums(5, 9, sb, u);

                    ApplyData(9, 17, sb, u, br);
                    ApplyNums(10, 17, sb, u);
                    ApplyData(17, 25, sb, u, br);
                    ApplyNums(18, 25, sb, u);
                    ApplyData(25, 33, sb, u, br);
                    ApplyNums(26, 33, sb, u);
                    ApplyData(33, 41, sb, u, br);
                    ApplyNums(34, 41, sb, u);
                    ApplyData(41, 49, sb, u, br);
                    ApplyNums(42, 49, sb, u);
                    ApplyData(49, 57, sb, u, br);
                    ApplyNums(50, 57, sb, u);
                    ApplyData(57, 65, sb, u, br);
                    ApplyNums(58, 65, sb, u);
                    ApplyData(65, 73, sb, u, br);
                    ApplyNums(66, 73, sb, u);
                    ApplyData(73, 81, sb, u, br);
                    ApplyNums(74, 81, sb, u);
                    ApplyNums(82, 85, sb, u);
                    ApplyData(81, 85, sb, u, br);
                    ApplyData(85,104 , sb, u, br);
                    ApplyNums(86, 104, sb, u);
                    ApplyData(104, 105, sb, u, br);
                    ApplyData(105, 106, sb, u, br);
                    ApplyData(106, 106, sb, u, br,TypeData.ShiftJISTring,true);
                    break;
                case "CultureFishTitleData":
                    ApplyData(1, 3, sb, u, br, TypeData.UnicodeString);
                    ApplyNums(2, 3, sb, u);
                    ApplyData(3, 4, sb, u, br);
                    ApplyData(4, 5, sb, u, br);
                    ApplyData(5, 5, sb, u, br,TypeData.ShiftJISTring, true);
                    break;
                case "EndCardData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br);
                    for (int i = 3; i < u.FileCount - 1; i++) { ApplyData(i, i + 1, sb, u, br,TypeData.UnicodeString); }
                    br.BaseStream.Position = u.ConfigVal[u.FileCount - 1].Value;
                    sb.Append(getValObject(br.ReadBytes((int)(u.FileLenght - u.ConfigVal[u.FileCount - 1].Value)), TypeData.UnicodeString)).Append("	");
                    break;
                case "EventData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br);
                    ApplyData(3, 3, sb, u, br,TypeData.ShiftJISTring,true);
                    ApplyNums(4, 5, sb, u);
                    break;
                case "EventCondition":
                    ApplyData(1, 2, sb, u, br,TypeData.UnicodeString);
                    ApplyData(2, 25, sb, u, br, TypeData.UnicodeString);
                    ApplyNums(3, 25, sb, u);
                    ApplyData(25, 27, sb, u, br);
                    ApplyNums(26, 27, sb, u);
                    ApplyData(27, 29, sb, u, br);
                    ApplyNums(28, 29, sb, u);
                    ApplyData(29, 31, sb, u, br);
                    ApplyNums(30, 31, sb, u);
                    ApplyData(31, 34, sb, u, br);
                    ApplyNums(32, 34, sb, u);
                    ApplyData(34, 36, sb, u, br);
                    ApplyNums(35, 36, sb, u);
                    ApplyData(36, 38, sb, u, br);
                    ApplyNums(37, 38, sb, u);
                    ApplyData(38, 40, sb, u, br);
                    ApplyNums(39, 40, sb, u);
                    ApplyData(40, 41, sb, u, br);
                    ApplyData(41, 42, sb, u, br);
                    ApplyData(42, 42, sb, u, br,TypeData.ShiftJISTring,true);
                    ApplyNums(43, (int)u.FileCount, sb, u);
                    break;
                case "FaceAnimData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br, TypeData.UnicodeString);
                    ApplyData(3, 4, sb, u, br);
                    ApplyData(4, 4, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "FaceAnimKind":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br, TypeData.UnicodeString);
                    ApplyData(3, 3, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "FarmAdviceData":
                    for (int i = 1; i < u.FileCount - 3; i++) { ApplyData(i, i + 1, sb, u, br); }
                    ApplyData((int)u.FileCount - 3, 2, sb, u, br, TypeData.ShiftJISTring, true);

                    ApplyNums((int) u.FileCount - 2, (int)u.FileCount, sb, u);
                    break;
                case "FertilizerWaterData":
                    ApplyData(1, 2, sb, u, br, TypeData.UnicodeString);
                    ApplyNums(3, 4, sb, u);
                    ApplyData(2, 4, sb, u, br);

                    for (int i = 4; i < u.FileCount - 1; i++)
                    {
                        ApplyData(i, i + 1, sb, u, br);
                    }
                    ApplyData((int)u.FileCount - 1, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "Info":
                    ApplyData(1, 18, sb, u, br, TypeData.UnicodeString);
                    ApplyNums(2, 18, sb, u);
                    ApplyData(18, 20, sb, u,br);
                    ApplyNums(19, 20, sb, u);
                    ApplyData(20, 21, sb, u, br);
                    ApplyData(21, 22, sb, u, br);
                    ApplyData(22, 23, sb, u, br);
                    ApplyData(23,23 , sb, u, br,TypeData.ShiftJISTring,true);
                    ApplyNums(24, (int) u.FileCount, sb, u);
                    break;
                case "Convert":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 4, sb, u, br);

                    for (int i = 3; i < u.FileCount - 2; i++)
                    {
                        ApplyNums(i,i+1, sb, u);
                        ApplyData(i+1, i+3, sb, u, br);

                        i++;
                    }

                    ApplyNums((int)u.FileCount-2, (int)u.FileCount-1, sb, u);
                    ApplyData((int)u.FileCount - 1, 0, sb, u, br,TypeData.ShiftJISTring,true);
                    break;
                case "NpcFarmland":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br);
                    ApplyData(3, 7, sb, u, br);
                    ApplyNums(4, 7, sb, u);
                    ApplyData(7, 12, sb, u, br);
                    ApplyNums(8, 12, sb, u);
                    ApplyData(12, 17, sb, u, br);
                    ApplyNums(13, 17, sb, u);
                    ApplyData(17, 22, sb, u, br);
                    ApplyNums(18, 22, sb, u);
                    ApplyData(22, 27, sb, u, br);
                    ApplyNums(23, 27, sb, u);
                    ApplyData(27, 32, sb, u, br);
                    ApplyNums(28, 32, sb, u);
                    ApplyData(32, 37, sb, u, br);
                    ApplyNums(33, 37, sb, u);
                    ApplyData(37, 42, sb, u, br);
                    ApplyNums(38, 42, sb, u);
                    ApplyData(42, 42, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(43, (int)u.FileCount, sb, u);
                    break;
                case "PowerPanelData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(3, (int)u.FileCount, sb, u);
                    break;
                case "PowerData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br);
                    ApplyData(3, 4, sb, u, br);
                    ApplyData(4, 5, sb, u, br);
                    ApplyData(5, 5, sb, u, br, TypeData.ShiftJISTring,true);
                    break;
                case "ComboEffectData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br);
                    ApplyData(3, 4, sb, u, br);
                    ApplyData(4, 5, sb, u, br);
                    ApplyData(5, 6, sb, u, br);
                    ApplyData(6, 7, sb, u, br);
                    ApplyData(7, 7, sb, u, br, TypeData.ShiftJISTring,true);
                    ApplyNums(8, (int)u.FileCount, sb, u);
                    break;
                case "ComboData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 4, sb, u, br);
                    ApplyNums(3, 4, sb, u);
                    ApplyData(4, 6, sb, u, br);
                    ApplyNums(5, 6, sb, u);
                    ApplyData(6, 8, sb, u, br);
                    ApplyNums(7, 8, sb, u);
                    ApplyData(8, 10, sb, u, br);
                    ApplyNums(9, 10, sb, u);
                    ApplyData(10, 12, sb, u, br);
                    ApplyNums(11, 12, sb, u);
                    ApplyData(12, 12, sb, u, br, TypeData.ShiftJISTring,true);
                    ApplyNums(13, 14, sb, u);
                    break;
                #endregion
                #region #4
                case "FieldGrassData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br, TypeData.UnicodeString, false);
                    ApplyData(3, 3, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(4, (int)u.FileCount, sb, u);
                    break;
                case "FishId":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br);
                    ApplyData(3, 4, sb, u, br);
                    ApplyData(4, 8, sb, u, br);
                    ApplyData(8, 8, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(5, 8, sb, u);
                    ApplyNums(9, (int)u.FileCount, sb, u);
                    break;
                case "FieldGrassPointData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br);
                    ApplyData(3, 4, sb, u, br);
                    ApplyData(4, 5, sb, u, br);
                    ApplyData(5, 5, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(6, (int)u.FileCount, sb, u);
                    break;
                case "Level":
                case "NetLevel":
                    ApplyNums(1, 3, sb, u);
                    ApplyData(3, 3, sb, u, br, TypeData.UnicodeString, true);
                    ApplyNums(4, (int)u.FileCount, sb, u);
                    break;
                case "Place":
                    ApplyData(1, 2, sb, u,br);
                    ApplyData(2, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(3, 7, sb, u);
                    ApplyNums(7, (int)u.FileCount, sb, u,1);
                    break;
                case "Insect":
                    ApplyData(1, 2, sb, u, br, TypeData.UnicodeString);

                    for (int i = 2; i < u.FileCount - 3; i++) { ApplyData(i, i + 1, sb, u, br); }
                    ApplyData((int)u.FileCount - 3, 2, sb, u, br, TypeData.ShiftJISTring, true);

                    ApplyNums((int)u.FileCount-2, (int)u.FileCount, sb, u);
                    break;
                case "FishFood":
                    ApplyData(1, 3, sb, u, br);
                    ApplyNums(2, 3, sb, u);
                    ApplyData(3, 5, sb, u, br);
                    ApplyData(5, 5, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(4, 5, sb, u);
                    break;
                case "ItemIngredient":
                case "PartTimeJobTypeData":
                    for (int i = 1; i < u.FileCount - 1; i++)
                    {
                        if (i == 1)
                        {
                            ApplyData(i, i + 1, sb, u, br,TypeData.UnicodeString);
                        }
                        else
                        {
                            ApplyData(i, i + 1, sb, u, br);
                        }
                        
                    }
                    ApplyData((int)u.FileCount - 1, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "ItemModelData":
                    ApplyData(1, 2, sb, u, br, TypeData.ShiftJISTring);
                    ApplyData(2, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(3, (int)u.FileCount, sb, u,1);
                    break;
                case "MapGimmickData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br);
                    ApplyData(3, 18, sb, u, br);
                    ApplyNums(4, 18, sb, u);
                    ApplyData(18, 18, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(19, (int)u.FileCount, sb, u);
                    break;
                case "MapGimmickModelData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br);
                    ApplyData(3, 6, sb, u, br);
                    ApplyNums(4, 6, sb, u);
                    ApplyData(6, 7, sb, u, br);
                    ApplyData(7, 9, sb, u, br);
                    ApplyNums(8, 9, sb, u);
                    ApplyData(9, 9, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(10, (int)u.FileCount, sb, u);
                    break;
                case "MapData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br, TypeData.UnicodeString);
                    ApplyData(3, 4, sb, u, br);
                    ApplyData(4, 5, sb, u, br);
                    ApplyData(5, 6, sb, u, br);
                    ApplyData(6, 9, sb, u, br);
                    ApplyNums(7, 9, sb, u);
                    ApplyData(9, 10, sb, u, br);
                    ApplyData(10, 11, sb, u, br);
                    ApplyData(11, 12, sb, u, br);
                    ApplyData(12, 13, sb, u, br);
                    ApplyData(13, 15, sb, u, br);
                    ApplyNums(14, 15, sb, u);
                    ApplyData(15, 15, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "LetterRecord":
                    ApplyData(1, 14, sb, u, br, TypeData.UnicodeString);
                    ApplyNums(2, 14, sb, u);
                    ApplyData(14, 15, sb, u, br);
                    ApplyData(15, 19, sb, u, br);
                    ApplyNums(16, 20, sb, u);
                    ApplyData(19, 20, sb, u, br);
                    ApplyData(20, 21, sb, u, br);
                    ApplyData(21, 22, sb, u, br);
                    ApplyData(22, 23, sb, u, br);
                    ApplyData(23, 23, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "MushroomData":
                    ApplyData(1, 5, sb, u, br, TypeData.UnicodeString);
                    ApplyNums(2, 5, sb, u);
                    ApplyData(5, 6, sb, u, br);
                    ApplyData(6, 7, sb, u, br);
                    ApplyData(7, 8, sb, u, br);
                    ApplyData(8, 8, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "MiningPointData":
                    ApplyData(1, 17, sb, u, br, TypeData.UnicodeString);
                    ApplyData(17, 17, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(2, 17, sb, u);
                    ApplyNums(18, (int)u.FileCount -6, sb, u);
                    ApplyNums((int)u.FileCount - 6, (int)u.FileCount, sb, u,1);
                    break;
                case "RecentReport":
                    ApplyData(1, 2, sb, u, br, TypeData.UnicodeString);
                    ApplyData(2, 4, sb, u, br);
                    ApplyNums(3, 4, sb, u);

                    for (int i = 4; i < u.FileCount - 1; i++) { ApplyData(i, i + 1, sb, u, br); }
                    ApplyData((int)u.FileCount - 1, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "LightFogData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br);
                    ApplyData(3, 3, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(4, (int) u.FileCount, sb, u);
                    break;
                case "MapPlacementCamera":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 6, sb, u, br, TypeData.UnicodeString);
                    ApplyNums(3, 6, sb, u);
                    ApplyData(6, 8, sb, u, br);
                    ApplyNums(7, 8, sb, u);
                    ApplyData(8, 8, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(9, (int)u.FileCount, sb, u);
                    break;
                case "MapCheckPointData":
                    ApplyData(1, 3, sb, u, br);
                    ApplyData(3, 3, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(2, 3, sb, u);
                    break;
                case "SystemPlacedPanelData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 5, sb, u, br);
                    ApplyNums(3, 5, sb, u);

                    for (int i = 5; i < (int)u.FileCount - 2; i++)
                    {
                        ApplyData(i, i + 1, sb, u, br);
                    }

                    ApplyData((int)u.FileCount - 2, (int)u.FileCount - 2, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums((int)u.FileCount-1, (int)u.FileCount, sb, u);
                    break;
                case "MiniMapDispData":
                    for (int i = 1; i < (int)u.FileCount - 2; i++)
                    {
                        ApplyData(i, i + 1, sb, u, br);
                    }

                    ApplyData((int)u.FileCount - 2, (int)u.FileCount - 2, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums((int)u.FileCount - 1, (int)u.FileCount, sb, u);
                    break;
                case "MyHouseData":
                    ApplyData(1, 7, sb, u, br);
                    ApplyData(7, 7, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(2, 6, sb, u);
                    break;
                case "MyHouseInteriorDesignData":
                    ApplyData(1, 9, sb, u, br);
                    ApplyData(9, 9, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(2, 8, sb, u);
                    break;
                case "ConvItem":
                case "HelpItemData":
                    ApplyNums(1, 2, sb, u);
                    ApplyData(2, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(3, (int)u.FileCount, sb, u);
                    break;
                case "NpcPetData":
                    for (int i = 1; i < 6; i++)
                    {
                        if (i == 1)
                        {
                            ApplyData(i, i + 1, sb, u, br, TypeData.UnicodeString);
                        }
                        else
                        {
                            ApplyData(i, i + 1, sb, u, br);
                        }
                        
                    }

                    ApplyData(6, 6, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(7, (int)u.FileCount, sb, u);
                    break;
                case "OptionData":
                    ApplyData(1, 5, sb, u, br);
                    ApplyNums(2, 5, sb, u);
                    ApplyData(5, 6, sb, u, br);
                    ApplyData(6, 7, sb, u, br);
                    ApplyData(7, 7, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "PanelData":
                    ApplyData(1, 2, sb, u, br, TypeData.UnicodeString);
                    ApplyData(2, 5, sb, u, br);
                    ApplyNums(3, 5, sb, u);
                    ApplyData(5, 6, sb, u, br);
                    ApplyData(6, 7, sb, u, br);
                    ApplyData(7, 11, sb, u, br);
                    ApplyNums(8, 11, sb, u);
                    ApplyData(11, 12, sb, u, br);
                    ApplyData(12, 13, sb, u, br);
                    ApplyData(13, 14, sb, u, br);
                    ApplyData(14, 15, sb, u, br);
                    ApplyData(15, 18, sb, u, br);
                    ApplyNums(16, 18, sb, u);
                    ApplyData(18, 21, sb, u, br);
                    ApplyNums(19, 21, sb, u);
                    ApplyData(21, 24, sb, u, br);
                    ApplyNums(22, 24, sb, u);
                    ApplyData(24, 25, sb, u, br);
                    ApplyData(25, 26, sb, u, br);
                    ApplyData(26, 26, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "QuestData":
                    for (int i = 1; i < 10; i++)
                    {
                        ApplyData(i, i + 1, sb, u, br);
                    }

                    ApplyData(10, 12, sb, u, br);
                    ApplyNums(11, 12, sb, u);
                    ApplyData(12, 13, sb, u, br);
                    ApplyData(13, 14, sb, u, br);
                    ApplyData(14, 16, sb, u, br);
                    ApplyNums(15, 16, sb, u);
                    ApplyData(16, 17, sb, u, br);
                    ApplyData(17, 18, sb, u, br);
                    ApplyData(18, 20, sb, u, br);
                    ApplyNums(19, 20, sb, u);

                    for (int i = 20; i < u.FileCount - 1; i++) { ApplyData(i, i + 1, sb, u, br); }
                    ApplyData((int)u.FileCount - 1, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "TrophyData":
                    for (int i = 1; i < 6; i++)
                    {
                        ApplyData(i, i + 1, sb, u, br );
                    }

                    ApplyData(6, 6, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(7, (int)u.FileCount, sb, u);
                    break;
                case "ToolData":
                    ApplyData(1, 2, sb, u, br, TypeData.UnicodeString);
                    ApplyData(2, 34, sb, u, br);
                    ApplyNums(3, 34, sb, u);
                    ApplyData(34, 35, sb, u,br);
                    ApplyData(35, 37, sb, u, br);
                    ApplyNums(36, 37, sb, u);
                    ApplyData(37, 39, sb, u, br);
                    ApplyNums(38, 39, sb, u);

                    for (int i = 39; i < u.FileCount - 1; i++)
                    {
                        ApplyData(i, i + 1, sb, u, br);
                    }

                    ApplyData((int)u.FileCount - 1, 6, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "Range":
                    ApplyData(1, 4, sb, u, br, TypeData.UnicodeString);
                    ApplyData(4, 4, sb, u, br, TypeData.UnicodeString,true);
                    ApplyNums(2, 4, sb, u);
                    break;
                case "FinalEffect":
                    ApplyNums(1, 2, sb, u);

                    ApplyData(2, 7, sb, u, br);
                    ApplyNums(3, 7, sb, u);
                    ApplyData(7, 12, sb, u, br);
                    ApplyNums(8, 12, sb, u);
                    ApplyData(12, 17, sb, u, br);
                    ApplyNums(13, 17, sb, u);

                    ApplyData(17, 17, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(18, 21, sb, u);
                    break;
                case "SoilType":
                    ApplyData(1, 6, sb, u, br, TypeData.UnicodeString);
                    ApplyNums(2, 6, sb, u);
                    ApplyData(6, 7, sb, u, br);
                    ApplyData(7, 8, sb, u, br);
                    ApplyData(8, 9, sb, u, br);
                    ApplyData(9, 9, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "FinalSE":
                    ApplyNums(1, 2, sb, u);

                    for (int i = 0; i < 43; i++)
                    {
                        ApplyData(i+2, i+5, sb, u, br);
                        ApplyNums(i+3, i+5, sb, u);

                        i++;
                        i++;
                    }

                    ApplyData(47, 47, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(48, (int)u.FileCount, sb, u);
                    break;
                #endregion
                #region#5
                case "RestaurantMenuData":
                    ApplyData(1, 2, sb, u, br, TypeData.UnicodeString);
                    ApplyData(3, 4, sb, u, br);
                    ApplyData(4, 5, sb, u, br);
                    ApplyData(5, 8, sb, u, br);
                    ApplyData(8, 9, sb, u, br);
                    ApplyNums(6, 8, sb, u);
                    ApplyData(9, 9, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "PropertyTitleData":
                    ApplyData(1, 3, sb, u, br, TypeData.UnicodeString);
                    ApplyNums(2, 3, sb, u);
                    ApplyData(3, 3, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(4, (int)u.FileCount, sb, u);
                    break;
                case "PartTimeJobData":
                    ApplyData(1, 3, sb, u, br);
                    ApplyNums(2, 3, sb, u);
                    ApplyData(3, 6, sb, u, br);
                    ApplyNums(4, 6, sb, u);
                    ApplyData(6, 7, sb, u, br);
                    ApplyData(7, 7, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "PartTimeJobCategoryData":
                case "PartTimeJobArticleDeilvery":
                    ApplyData(1, 1, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(2, (int)u.FileCount, sb, u);
                    break;
                case "PlayData":
                    ApplyData(1, 1, sb, u, br, TypeData.UnicodeString, true);
                    ApplyNums(2, (int)u.FileCount, sb, u);
                    break;
                case "PictureStoryShowTitleData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br);
                    ApplyData(3, 4, sb, u, br, TypeData.UnicodeString);
                    ApplyData(4, 4, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "PictureStoryShowData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 3, sb, u, br, TypeData.UnicodeString);
                    ApplyData(3, 4, sb, u, br);
                    ApplyData(4, 5, sb, u, br);
                    ApplyData(5, 6, sb, u, br);
                    ApplyData(6, 7, sb, u, br);
                    ApplyData(7, 8, sb, u, br);
                    ApplyData(8, 9, sb, u, br);
                    ApplyData(9, 9, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "PartTimeJobAddFee":
                    for (int i = 1; i < u.FileCount - 3; i++)
                    {
                        ApplyData(i, i + 1, sb, u, br);
                    }

                    ApplyData((int)u.FileCount - 3, 1, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums((int)u.FileCount - 2, (int)u.FileCount, sb, u);
                    break;
                case "PartTimeJobShipment":
                    ApplyNums(1, 4, sb, u);
                    ApplyData(4, 4, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "PartTimeJobFishItemTable":
                    ApplyData(1, 1, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(2, (int)u.FileCount, sb, u);
                    break;
                case "ShopData":
                    ApplyNums(1, 5, sb, u);
                    ApplyData(5, 10, sb, u, br);
                    ApplyNums(6, 10, sb, u);
                    ApplyData(10, 18, sb, u, br);

                    ApplyData(18, 23, sb, u, br);

                    for (int i = 23; i < u.FileCount - 1; i++)
                    {
                        ApplyData(i, i + 1, sb, u, br);
                    }

                    ApplyData((int)u.FileCount - 1, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    //33
                    break;
                case "ShopPanelCreateData":
                    ApplyData(1, 3, sb, u, br);
                    ApplyNums(2, 3, sb, u);
                    ApplyData(3, 4, sb, u, br);
                    ApplyData(4, 14, sb, u, br);
                    ApplyNums(5, 14, sb, u);
                    ApplyData(14, 17, sb, u, br);
                    ApplyNums(15, 17, sb, u);
                    ApplyData(17, 20, sb, u, br);
                    ApplyNums(18, 20, sb, u);
                    ApplyData(20, 23, sb, u, br);
                    ApplyNums(21, 23, sb, u);
                    ApplyData(23, 26, sb, u, br);
                    ApplyNums(24, 26, sb, u);
                    ApplyData(26, 29, sb, u, br);
                    ApplyNums(27, 29, sb, u);
                    ApplyData(29, 32, sb, u, br);
                    ApplyNums(30, 32, sb, u);
                    ApplyData(32, 35, sb, u, br);
                    ApplyNums(33, 35, sb, u);
                    ApplyData(35, 37, sb, u, br);
                    ApplyNums(36, 37, sb, u);
                    ApplyData(37, 39, sb, u, br);
                    ApplyNums(38, 39, sb, u);
                    ApplyData(39, 41, sb, u, br);
                    ApplyNums(40, 41, sb, u);
                    ApplyData(41, 43, sb, u, br);
                    ApplyNums(42, 43, sb, u);
                    ApplyData(43, 45, sb, u, br);
                    ApplyNums(44, 45, sb, u);
                    ApplyData(45, 45, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(46, (int)u.FileCount, sb, u);
                    break;
                case "ShopMyHouseExteriorData":
                case "ShopMyHouseInteriorData":
                    for (int i = 1; i < 4; i++)
                    {
                        if (i == 1) ApplyData(i, i + 1, sb, u, br, TypeData.UnicodeString);
                        else ApplyData(i, i + 1, sb, u, br);
                    }

                    ApplyData(4, 14, sb, u, br);
                    ApplyNums(5, 14, sb, u);
                    ApplyData(14, 17, sb, u, br);
                    ApplyNums(15, 17, sb, u);
                    ApplyData(17, 20, sb, u, br);
                    ApplyNums(18, 20, sb, u);
                    ApplyData(20, 22, sb, u, br);
                    ApplyNums(21, 22, sb, u);
                    ApplyData(22, 24, sb, u, br);
                    ApplyNums(23, 24, sb, u);
                    ApplyData(24, 26, sb, u, br);
                    ApplyNums(25, 26, sb, u);
                    ApplyData(26, 28, sb, u, br);
                    ApplyNums(27, 28, sb, u);
                    ApplyData(28, 30, sb, u, br);
                    ApplyData(30, 30, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(31, (int)u.FileCount, sb, u);
                    break;
                case "ShopMyHouseFurnitureData":
                    for (int i = 1; i < 6; i++)
                    {
                        if (i == 1) ApplyData(i, i + 1, sb, u, br, TypeData.UnicodeString);
                        else ApplyData(i, i + 1, sb, u, br);
                    }

                    ApplyData(6, 16, sb, u, br);
                    ApplyNums(7, 16, sb, u);
                    ApplyData(16, 19, sb, u, br);
                    ApplyNums(17, 19, sb, u);
                    ApplyData(19, 22, sb, u, br);
                    ApplyNums(20, 22, sb, u);
                    ApplyData(22, 24, sb, u, br);
                    ApplyNums(23, 24, sb, u);
                    ApplyData(24, 26, sb, u, br);
                    ApplyNums(25, 26, sb, u);
                    ApplyData(26, 28, sb, u, br);
                    ApplyNums(27, 28, sb, u);
                    ApplyData(28, 30, sb, u, br);
                    ApplyNums(29, 30, sb, u);
                    ApplyData(30, 32, sb, u, br);
                    ApplyNums(31, 32, sb, u);
                    ApplyData(32, 32, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(33, (int)u.FileCount, sb, u);
                    break;
                case "ShopMyHouseExtensionData":
                    for (int i = 1; i < 5; i++)
                    {
                        if (i == 1) ApplyData(i, i + 1, sb, u, br, TypeData.UnicodeString);
                        else ApplyData(i, i + 1, sb, u, br);
                    }

                    ApplyData(5, 15, sb, u, br);
                    ApplyNums(6, 15, sb, u);
                    ApplyData(15, 18, sb, u, br);
                    ApplyNums(16, 18, sb, u);
                    ApplyData(18, 21, sb, u, br);
                    ApplyNums(19, 21, sb, u);
                    ApplyData(21, 23, sb, u, br);
                    ApplyNums(22, 23, sb, u);
                    ApplyData(23, 25, sb, u, br);
                    ApplyNums(24, 25, sb, u);
                    ApplyData(25, 27, sb, u, br);
                    ApplyNums(26, 27, sb, u);
                    ApplyData(27, 29, sb, u, br);
                    ApplyNums(28, 29, sb, u);
                    ApplyData(29, 31, sb, u, br);
                    ApplyNums(30, 31, sb, u);
                    ApplyData(31, 31, sb, u, br);
                    ApplyNums(32, (int)u.FileCount, sb, u);
                    break;
                case "EntryNpcData":
                    for (int i = 1; i < 5; i++)
                    {
                        if(i==1 || i ==3) ApplyData(i, i + 1, sb, u, br, TypeData.UnicodeString, false);
                        else ApplyData(i, i + 1, sb, u, br, TypeData.ShiftJISTring, false);
                    }
                    ApplyData(5, 5, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(6, (int)u.FileCount, sb, u);
                    break;
                #endregion
                #region Common Parsers
                case "HairColorData": case "HairData": case "EyeColorData":
                case "SkinColorData": case "EyeShapeData":
                case "MiniMapData":
                    for (int i = 1; i < 2; i++)
                    {
                        ApplyData(i, i + 1, sb, u, br, TypeData.UnicodeString, false);
                    }
                    ApplyData(2, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(3, (int)u.FileCount, sb, u);
                    break;
                case "ThinkData": case "MinorCategoryPressData": case "MinorCategoryData":
                case "FeedTypeData": case "AnimalAssistanceGrazingNumData": case "AnimalAssistanceItemGetData":
                case "ActionData": case "HutSizeData": case "LevelNeedExpData":
                 case "FavoriteAnimal": case "FavoriteAvatarSetData":
                case "FavoriteAvatarData": case "FavoriteAvatarHatData": case "FavoriteAvatarGlassesData":
                case "FavoriteAnimalData": case "AnimalSoulmatePresentQuality":
                case "AnimalTreasureEventProbabilityData": case "AnimalTreasureRange": case "AnimalTreasureItemQuality":
                case "AnimalWildMapData": case "AnimalWildFriendshipData": case "AtelierSubValueData":
                case "HatCompatibilityData": case "DispPosData": case "ChildColorData":
                case "BoundingBoxData": case "CookLevel": case "CropCommonData": case "CropHarvestFruitNum":
                case "CultureGrowthPatternData": case "MagData": case "FarmPoint": case "FertilizerWaterSubValue":
                case "FertilizerWaterSettings": case "ItemQuality": case "ItemRate": case "OtherData":
                case "HitRate": case "Quality": case "GoodPercent": case "NetGroup":
                case "NetQuality": case "NetCatchHour" : case "Other": case "FreeEditBit": case "TypeData":
                case "ConditionData": case "MapCameraData": case "ItemTableKindData": case "NumberData":
                case "NpcModelData":
                case "NpcAttentionPointData":
                case "RankProbability":
                case "BagData":
                case "ItemStorageData":
                case "WeatherData":
                case "BGM":
                case "SoilData":
                case "PrizeTableData":
                case "PrizeRandomTableData":
                case "PartTimeJobNum":
                case "PartTimeJobLottery":
                case "PartTimeJobDeliveryLottery":
                case "PartTimeJobShipmentNum":
                case "PartTimeJobLevelValue":
                case "PartTimeJobTownRankValue":
                case "SettingData":
                case "CropPointTable":
                case "CropAddPoint":
                case "AnimalPointTable":
                case "AnimalSizeAddPoint":
                case "PetLevelAddPoint":
                case "PetAddPoint":
                case "CookPointTable":
                case "CookLevelAddPoint":
                case "CookDiffAddPoint":
                case "CookAddPoint":
                case "FashionPointTable":
                case "FashionAddPoint":
                case "EntryNpcPointTable":
                    ApplyNums(1, (int) u.FileCount, sb, u);
                    break;
                case "FavoritePresentFriendship":
                    ApplyNums(1, (int)u.FileCount, sb, u,2);
                    break;
                case "FreeEditPanelData":
                    ApplyData(1, 2, sb, u, br,TypeData.UnicodeString);
                    for (int i = 2; i < u.FileCount - 1; i++) { ApplyData(i, i + 1, sb, u, br); }
                    ApplyData((int)u.FileCount - 1, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "ActionId": case "MapId": case "SetData": case "Emotion": case "DrinkFesItemData":
                case "EffectResourceData": case "LevelData": case "FlowerData": 
                case "GuidanceData": case "HarvestTable": case "MapTable": case "LightFogAreaData":
                case "NpcAvatarModelData":
                case "NpcCharaModelData":
                case "NpcPresentData":
                case "WeatherAreaData":
                case "WeatherTypeData":
                case "WeatherPattern":
                case "ItemPresentData":
                case "ShopIngredient":
                case "CropFesEntryType":
                case "CropFesNpcItem":
                case "CookFesNpcItem":
                    for (int i = 1; i < u.FileCount - 1; i++) { ApplyData(i, i + 1, sb, u, br); }
                    ApplyData((int)u.FileCount - 1, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "Data":
                    if (u.FileCount == 0x0f)
                    {
                        ApplyData(1, 3, sb, u, br);
                        ApplyNums(2, 3, sb, u);
                        ApplyData(3, 5, sb, u, br);
                        ApplyNums(4, 5, sb, u);
                        ApplyData(5, 7, sb, u, br);
                        ApplyNums(6, 7, sb, u);
                        ApplyData(7, 9, sb, u, br);
                        ApplyNums(8, 9, sb, u);
                        ApplyData(9, 11, sb, u, br);
                        ApplyNums(10, 11, sb, u);
                        ApplyData(11, 14, sb, u, br);
                        ApplyNums(12, 14, sb, u);
                        ApplyData((int)u.FileCount - 1, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    }
                    else
                    {
                        for (int i = 1; i < u.FileCount - 1; i++) { ApplyData(i, i + 1, sb, u, br); }
                        ApplyData((int)u.FileCount - 1, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    }
                    break;
                case "Crop":
                case "Item":
                case "Tool":
                case "Menu":
                case "Recipe":
                case "Animal":
                case "Hair":
                case "HairColor":
                case "Face":
                case "SkinColor":
                case "EyeColor":
                case "WritingPaper":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 19, sb, u, br);
                    ApplyNums(3, 19, sb, u);
                    ApplyData(19, 22, sb, u, br);
                    ApplyNums(20, 22, sb, u);
                    ApplyData(22, 25, sb, u, br);
                    ApplyNums(23, 25, sb, u);
                    ApplyData(25, 28, sb, u, br);
                    ApplyNums(26, 28, sb, u);
                    ApplyData(28, 31, sb, u, br);
                    ApplyNums(29, 31, sb, u);
                    ApplyData(31, 34, sb, u, br);
                    ApplyNums(32, 34, sb, u);
                    ApplyData(34, 37, sb, u, br);
                    ApplyNums(35, 37, sb, u);
                    ApplyData(37, 40, sb, u, br);
                    ApplyNums(38, 40, sb, u);
                    ApplyData(40, 40, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(41, (int)u.FileCount, sb, u);
                    break;
                case "PrizeData":
                    ApplyNums(1, 2, sb, u);
                    for (int i = 2; i < u.FileCount - 1; i++) { ApplyData(i, i + 1, sb, u, br); }
                    ApplyData((int)u.FileCount - 1, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "NpcCallNameData": case "RelationCallPlayerData":
                    ApplyData(1, 2, sb, u, br);
                    for (int i = 2; i < u.FileCount - 1; i++) { ApplyData(i, i + 1, sb, u, br, TypeData.UnicodeString); }
                    ApplyData((int)u.FileCount - 1, 2, sb, u, br, TypeData.UnicodeString, true);
                    break;
                case "ChildCallParentData":
                    for (int i = 1; i < u.FileCount - 1; i++) { ApplyData(i, i + 1, sb, u, br, TypeData.UnicodeString); }
                    ApplyData((int)u.FileCount - 1, 2, sb, u, br, TypeData.UnicodeString, true);
                    break;
                case "ItemBrandGroupData": case "AnimalAssistanceData": case "ClothesAttributeData":
                case "ClothesColorData": case "CookCategory": case "StateData": case "HutCategoryData":
                case "AccessoryData": case "BgData":
                case "Season":
                case "ToolColorData":
                case "FestivalRank":
                case "CropFestivalTheme":
                case "AnimalFestivalTheme":
                case "PetFestivalTheme":
                    ApplyData(1, 1, sb, u, br, TypeData.UnicodeString, true);
                    break;
                case "ActionIdOne": case "MapIdOne": case "AnimalAssistancePetIdData":
                case "AnimalAssistanceGetMaterialData": case "AnimalAssistanceGetFishData": case "AnimalAssistanceGetOreData":
                case "AnimalAssistanceGetNutData": case "AnimalAssistanceGetOtherData": case "MinorCategoryLiftData":
                case "CharaIndex": case "ModelTypeData": case "AnimalSoulmateEvent": case "AnimalWildListData":
                case "AnimalSoulmatePresent": case "AvatarHeadModelData": case "AvatarHairModelData":
                case "AvatarEyeShapeModelData": case "HoneyData": case "CharaIconData":
                case "NeedData": case "MapChangeCamera": case "MapChangeData": case "RockModelData":
                case "OreData":
                case "WeatherModelData":
                case "NeedTypeData":
                case "RandomPrizeData":
                case "PartTimeJobPostalDelivery":
                case "PartTimeJobFlowerDelivery":
                case "PictureStoryShowRomData":
                case "ShowFestivalData":
                case "PictureStoryShowAddData":
                    ApplyData(1, 1, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "AINpcMapPointData": case "AnimalAIBaseActionData": case "AnimalAIPetBaseActionData":
                case "AnimalAIWildBaseActionData": case "AnimalModelData": case "HoneyGetItemData":
                case "EffectData": case "MapSoundData":
                    ApplyData(1, 2, sb, u, br);
                    ApplyData(2, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(3, (int)u.FileCount, sb, u);
                    break;
                case "EventParam": //
                    ApplyNums(1, 4, sb, u);
                    ApplyData(4, 4, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "EventFlag":
                    ApplyNums(1, 2, sb, u);
                    ApplyData(2, 2, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(3, 4, sb, u);
                    break;
                case "AnimalAIHorseBaseActionData": case "SnackData": case "MovableMapData":
                case "ItemBufData": case "FavoriteData": case "AnimalPetData": case "AnimalWildPresent":
                case "AnimalWildLikeItemData": case "AtelierTypeData": case "AvatarBodyModelData":
                case "AvatarEyeColorModelData": case "AvatarHatModelData": case "AvatarGlassesModelData":
                case "AvatarSkinColorModelData": case "AvatarHairColorModelData": case "BasementData":
                case "CollisionData": case "FertilizerId": case "CropModelData": case "CulturePearlGetData":
                case "DealerData":   case "EventPictureData":
                case "FestivalCampaData": case "DropPointData": case "DropItemData": case "FishSize":
                case "FishSign": case "FreeEdit": case "ItemTableData": case "TitleData":
                case "MapModelData": case "PlaceData":
                case "WrappingData":
                case "RankData":
                case "SE":
                case "PartTimeJobSplitWoodNum":
                    ApplyData(1, 1, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(2, (int)u.FileCount, sb, u);
                    break;
                case "Point":

                    ApplyData(1, 1, sb, u, br, TypeData.ShiftJISTring, true);
                    ApplyNums(2, (int)u.FileCount, sb, u,1);
                    break;
                case "MapPointData":
                    ApplyNums(1, (int)u.FileCount, sb, u, 1);
                    break;
                case "Fertilizer":
                    ApplyNums(1, (int)u.FileCount-1, sb, u);
                    ApplyData((int)u.FileCount - 1, (int)u.FileCount - 1, sb, u, br, TypeData.UnicodeString, true);
                    break;
                case "PointData":
                    ApplyNums(1, (int)u.FileCount - 1, sb, u);
                    ApplyData((int)u.FileCount - 1, (int)u.FileCount - 1, sb, u, br, TypeData.ShiftJISTring, true);
                    break;
                case "PersonalityData": case "LookData": case "HoneyTitleData": case "CultureTitleData":
                case "MapleTitle": case "BambooTitle": case "CreeperTitle": case "Job":
                case "GameModeData": case "PostingRecord": case "WritingPaperData":
                    ApplyData(1, 1, sb, u, br, TypeData.UnicodeString, true);
                    ApplyNums(2, (int)u.FileCount, sb, u);
                    break;
                    #endregion
            }
            getdata = sb.ToString();
        }

        private object getValObject(byte[] input, TypeData tdata)
        {
            switch (tdata)
            {
                case TypeData.UnicodeString:
                    return Encoding.Unicode.GetString(input).Replace("\0", "");
                case TypeData.ShiftJISTring:
                    return Encoding.GetEncoding(932).GetString(input).Replace("\0", "");
                case TypeData.ASCIIString:
                    return Encoding.ASCII.GetString(input).Replace("\0", "");
                case TypeData.Uint32:
                    return BitConverter.ToUInt32(input,0);
                default:
                    return 0;
            }
        }

        private string getConfigValue(ConfigHeader u, BinaryReader br)
        {
            br.BaseStream.Position = u.ConfigVal[0].Value;
            bool stopread = false;
            List<byte> listby = new List<byte>();

            do
            {
                byte tempbyte = br.ReadByte();

                if (tempbyte != 0)
                {
                    listby.Add(tempbyte);
                }
                else
                {
                    stopread = true;
                }
            } while (stopread == false);

            return Encoding.GetEncoding(932).GetString(listby.ToArray());
        }
    }

    public struct ConfigHeader
    {
        public uint FileLenght;
        public uint FileCount;
        public string ConfigType;
        public ConfigValList[] ConfigVal;
    }

    public struct ConfigValList
    {
        public uint Value;
        public TypeData TypeData;
        public uint Lenght; // 4 for int, less is for string
    }

    public enum TypeData
    {
        Uint32,
        UnicodeString,
        ASCIIString,
        ShiftJISTring
    }
}
