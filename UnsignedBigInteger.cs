using System;
using System.Security.Cryptography;

namespace NetworkIO.Numerics
{
    public class UnsignedBigInteger : IComparable<UnsignedBigInteger>, IEquatable<UnsignedBigInteger>
    {
        private uint[] _digits;
        private int _digitLength;

        internal uint[] digitsRef { get { return this._digits; } }

        public const long MaxBitsLength = BigIntInternal.maxBitsLength;
        public const int MaxDigitLength = BigIntInternal.maxDigitsLength;
        public const int MaxBytesLength = BigIntInternal.maxBytesLength;

        public static UnsignedBigInteger Prime64_1 { get { return new UnsignedBigInteger() { _digitLength = 16, _digits = new uint[16] { 2301275499, 19745053, 3060081466, 185362482, 1130485308, 1670846992, 1029924347, 473800313, 569808007, 4117648780, 257168457, 421687523, 961471381, 3454105951, 3056541516, 3446300977 } }; } }
        public static UnsignedBigInteger Prime64_2 { get { return new UnsignedBigInteger() { _digitLength = 16, _digits = new uint[16] { 2934601235, 1615250155, 2411799548, 3769314298, 556174975, 1422856060, 1619321069, 3741184229, 2913482381, 364165396, 1124117465, 105093379, 3622114144, 1669850981, 3609652125, 3328555646 } }; } }
        public static UnsignedBigInteger Prime128_1 { get { return new UnsignedBigInteger() { _digitLength = 32, _digits = new uint[32] { 2944527599, 1186261256, 448196864, 138599835, 3480393932, 809264608, 3044934049, 2254292196, 2431016830, 1341617579, 2215466063, 3522942467, 730778758, 2963951264, 840727316, 1927287925, 1065605267, 231826644, 2056981808, 1381449826, 4228987953, 880477692, 2345435414, 905351746, 1931273677, 496830929, 1290213938, 1759821462, 2785582002, 4157687433, 3688862444, 3561153785 } }; } }
        public static UnsignedBigInteger Prime128_2 { get { return new UnsignedBigInteger() { _digitLength = 32, _digits = new uint[32] { 2072166911, 3656933436, 456983460, 748556101, 3479412537, 1776556835, 740709349, 4173645813, 1903361797, 2301447364, 1571171062, 1779251304, 3346629615, 4016567807, 434367288, 3403824842, 79735196, 4119400133, 3246082516, 1648620701, 149287262, 3209344163, 2619185800, 1069924716, 190213306, 2967482019, 3164158219, 2859054862, 2958787824, 2271116194, 3689108562, 3106749489 } }; } }
        public static UnsignedBigInteger Prime256_1 { get { return new UnsignedBigInteger() { _digitLength = 64, _digits = new uint[64] { 2826793459, 4100764753, 1912231593, 2082841730, 3846218625, 2076460899, 3300857903, 3962124236, 3712073234, 2783224553, 4263487150, 2771273335, 2792150875, 1924603971, 1963013767, 3022168977, 2545160111, 506763349, 278314577, 3588249789, 2280745175, 3318029158, 3441957870, 1856790914, 567701802, 2446194262, 2087964594, 379929363, 1046391021, 188613844, 3679469163, 3317110331, 1156451242, 3259181030, 1441050980, 3443810436, 2139352120, 2743547624, 3515633432, 1021145043, 1465233298, 3241547441, 1059733039, 2233945261, 2640937427, 3134275690, 945205744, 2162517422, 1061096788, 1997909061, 2186329037, 231366614, 2398452889, 4028165793, 1782700330, 3328607172, 1480904515, 2211157728, 3553437723, 314605211, 3635501560, 3277394570, 2923109665, 4048798959 } }; } }
        public static UnsignedBigInteger Prime256_2 { get { return new UnsignedBigInteger() { _digitLength = 64, _digits = new uint[64] { 2275900235, 1720793212, 244228502, 4009451331, 1093021286, 1496726423, 701774011, 702720772, 1754674207, 1983999436, 108670515, 3256408962, 3960082826, 2884454594, 1137462856, 3949029476, 250504382, 4162950088, 1745727724, 3772199104, 4087222560, 3613914528, 2116086599, 3407019681, 491229133, 4160293612, 285217519, 401310779, 1378086007, 3377098441, 3720747005, 3998720883, 832258462, 368294633, 2382604055, 3281802741, 3020359935, 1594015706, 3558899012, 1283726403, 4185067962, 3173956073, 574295544, 2815736938, 1792869243, 3742671516, 1810643775, 4224851609, 3822303011, 1188733666, 3434891737, 2297749380, 1062828457, 414128446, 2914620668, 3659776973, 3019783995, 3636785384, 1385207593, 1599817958, 549610311, 163418821, 1307032327, 377040105 } }; } }
        public static UnsignedBigInteger Prime512_1 { get { return new UnsignedBigInteger() { _digitLength = 128, _digits = new uint[128] { 2081719259, 693081180, 447724548, 347595627, 1238853496, 289486040, 3719418142, 3567769241, 3018223387, 35457366, 3125602326, 479573068, 3465921148, 3015709193, 497309824, 1778895601, 418675574, 1386540933, 2984560336, 2373466248, 2271808204, 3069392284, 1253500974, 1095290908, 994743931, 1180599017, 2823326740, 2493129863, 3801896437, 3978397600, 266316351, 2155154282, 3202872448, 1636605260, 1782116900, 2192796607, 1158458517, 248603503, 4141084721, 3353275070, 1208470395, 1447805389, 4156734249, 3911259878, 1557307866, 2564232010, 1498044223, 2262038377, 605676645, 1380233520, 4071013274, 1316257384, 3186198407, 3751331482, 3542334055, 402964635, 3685763754, 1879773384, 2730655284, 3743525991, 1342530258, 597394106, 3492997585, 4115343346, 575640258, 3696108215, 806917022, 1610033304, 71034995, 2760351405, 3753645894, 2087608715, 1017026021, 4253401851, 970356830, 1161092291, 3250822920, 3668828987, 375071376, 1682333758, 2389687746, 1670642346, 493497212, 324006411, 2585651840, 1270489540, 2126171598, 1841596956, 2280802740, 3433720091, 332276017, 2548032065, 1151471541, 3668473862, 3338524688, 4067908083, 2331077789, 1778452071, 1593836548, 2507805433, 2915804232, 2732676284, 1514702685, 2399915739, 826918246, 2054564744, 1232561507, 2574849972, 2719096872, 3577176800, 3637849068, 1660782367, 2337905612, 97794966, 801169786, 616765889, 1159892011, 3325291365, 3522355089, 3296543911, 2089947009, 2409375449, 3114545496, 625245420, 4218210854, 29660849, 1307010600, 4160326172 } }; } }
        public static UnsignedBigInteger Prime512_2 { get { return new UnsignedBigInteger() { _digitLength = 128, _digits = new uint[128] { 663743287, 153385424, 992333420, 1785431674, 3224837389, 703941883, 2291285935, 775891148, 812732264, 2966798818, 947547783, 213744315, 1563420897, 947958837, 315762493, 4206152962, 1027102624, 1088392281, 4157117733, 3294618278, 4211248913, 104718512, 1668946298, 2511095672, 1213978808, 3284944228, 2395777498, 4183654976, 4262999678, 3568231974, 3482815411, 3156522703, 3029212143, 914420762, 3711348495, 2548577783, 4292080343, 3626290906, 934143893, 3827101199, 2209431018, 4277326432, 305126124, 3110949811, 623996248, 3650674902, 591833070, 2769475937, 2765448548, 3219281131, 3755474790, 3437254825, 2339009883, 3891516991, 1918489461, 2075467046, 3697119252, 2528422029, 65553339, 3001581005, 398934554, 253715593, 2804059236, 23649794, 2169286152, 687450796, 486127743, 3455572704, 3375822018, 3655325924, 1570066394, 630764283, 4043457842, 3863186764, 3601589550, 1107139051, 2011270430, 1904398967, 2370803825, 1390733263, 4089771510, 2554717089, 2986309065, 3128741125, 1357128316, 2591702820, 3554333883, 3833958120, 740904629, 1907155302, 9407664, 789552240, 325104380, 3941471121, 3626046202, 894636501, 1025154858, 2526406753, 334369369, 1189299135, 2322522147, 188249792, 1533821498, 1753375801, 148912584, 888565691, 4155926493, 3772025223, 846324792, 3639303349, 4013498288, 2525425619, 2107570696, 1537402519, 3900874771, 1971024075, 1154246978, 3947417930, 985156910, 4268474297, 1873835800, 4186784880, 3124308654, 4280913597, 159456563, 1348271839, 204220767, 3702062289 } }; } }
        public static UnsignedBigInteger BlumShub128 { get { return new UnsignedBigInteger() { _digitLength = 32, _digits = new uint[32] { 401092849, 2871309445, 2641464838, 1138409863, 3477634510, 2132227166, 1781163412, 1840258092, 1375213482, 3448261902, 2734016649, 2532587438, 3945491644, 1001407822, 2387881901, 708907589, 3234210374, 1571884751, 1017080962, 3898007560, 901618983, 468969566, 2151383240, 894604714, 1465494765, 3532163930, 3184016038, 452417231, 59888497, 3341737878, 3710048796, 2670847945 } }; } }
        public static UnsignedBigInteger BlumShub256 { get { return new UnsignedBigInteger() { _digitLength = 64, _digits = new uint[64] { 3966316817, 1316315340, 838252052, 1515739568, 714662711, 3598407304, 973554208, 319057718, 1558705220, 1100370829, 2282382340, 792069116, 1759553168, 648560915, 1209919895, 2602232526, 517695124, 1674481846, 2132584714, 3586250159, 3260961481, 445794305, 1820831103, 3379601996, 2933138274, 1366753668, 1598616868, 563915920, 481944458, 4250632253, 2782484824, 1694582020, 1980987245, 2570124381, 3547304943, 1952718533, 2140838623, 2165796806, 234493265, 3933415281, 825526448, 1922594029, 861385825, 3358116662, 1666120467, 1751897425, 2093636157, 4190888378, 3815606048, 2718674323, 1494323895, 822334422, 766734590, 2412773106, 3272552755, 2222711288, 2737256480, 3145801501, 3386013515, 543577473, 66860845, 3094061348, 3566883314, 2575948067 } }; } }
        public static UnsignedBigInteger BlumShub512 { get { return new UnsignedBigInteger() { _digitLength = 128, _digits = new uint[128] { 486875953, 4058937985, 3375302778, 2971499114, 2522877041, 4035496541, 191559621, 2256114738, 2936422303, 2521520770, 1597700429, 2909176584, 2435498640, 3254177050, 3480817128, 3682422344, 1381426126, 755896076, 862984519, 2521743110, 2093309732, 3098904575, 1352215576, 2688298301, 3546319706, 2265791058, 137343439, 4086717792, 636748799, 1894034170, 3809700659, 3227272569, 3780021049, 812281463, 3483509335, 3038270818, 1201437810, 3125673605, 864707890, 2928677767, 3649836757, 898405379, 4251821018, 3906928360, 3665973594, 1925493447, 2878164908, 446045749, 1708320317, 3123482675, 16736872, 921257562, 2878812895, 1911641795, 1896669647, 984502993, 2453581451, 257756221, 2544244415, 1736972760, 2962525388, 2798146204, 3514393978, 1201765190, 1371569760, 1621959644, 2364011851, 454190217, 2896519566, 4204239930, 1210710599, 775975432, 3371746334, 3685127527, 2214148263, 3620366375, 2627233690, 1696648105, 2475854293, 2833318533, 3982652877, 3339311041, 3285238017, 1549961467, 2345543086, 1884685382, 2632143898, 1742530105, 3655105193, 2763203508, 4213992228, 2401744162, 3909439806, 592646799, 1272112486, 509150139, 1545510764, 3254497076, 4225211591, 3602300501, 545414774, 3563158942, 427457599, 838528797, 2177879986, 591941174, 1363461000, 3049898418, 2690548266, 2228243712, 1385075096, 2230922126, 1067721569, 2165288378, 814186869, 1142713738, 3963242565, 3992525023, 820438533, 4274875855, 1079322304, 522389535, 1998492422, 3486032313, 2640091111, 126905101, 46826411, 355429851 } }; } }
        public static UnsignedBigInteger BlumShub1024 { get { return new UnsignedBigInteger() { _digitLength = 256, _digits = new uint[256] { 2177326349, 2762201665, 1669771666, 3524941366, 3600241957, 1089929219, 3812412724, 1360392606, 2208746579, 2962008522, 1106813950, 3244956045, 628528096, 3671641049, 571757232, 607913374, 2141753049, 1692423421, 1040571025, 3381411166, 199625533, 394637018, 2808662202, 1540333872, 2022840021, 1771994412, 753849789, 2955927172, 1772707475, 1952992283, 2302291402, 1279630610, 2302147501, 585428586, 2431636279, 1946550084, 3468979347, 3207759711, 542227539, 3479189865, 2693661352, 3954169510, 409980905, 3540913613, 1414283025, 2549934473, 2044102118, 1618238995, 2266066960, 2355582273, 3006168355, 1230138703, 506239146, 371455192, 3939046332, 241668075, 1165153631, 355814982, 2560566929, 42339925, 2576956379, 3210608359, 2828890108, 3040204930, 463596147, 3404471760, 560968610, 644280563, 3654150646, 3748266460, 1949279602, 2481601621, 2590426827, 4276012178, 1109760723, 1715211706, 3357482890, 3914557735, 1361916252, 991675461, 3223183794, 170474125, 1063924051, 3142417851, 2887007983, 1722395855, 3884862782, 1927202518, 127786197, 2136819933, 474919868, 2848341756, 149324649, 2114091252, 3685075137, 3143917272, 3405025291, 3042119589, 1093836754, 2884189807, 36524655, 1150183483, 2073392512, 396103512, 3521319520, 1260347690, 3912174400, 1275237791, 2024884744, 3058493301, 1817529970, 1500329733, 628960359, 1465705070, 2695867563, 2118627743, 3189588590, 2681465646, 3421166197, 3785281433, 1629133066, 470388991, 2279799304, 1710932156, 1457133396, 2791082139, 301228171, 691828821, 1569082364, 2858431492, 1774581577, 1148094682, 757906269, 3658306007, 859463017, 4100733466, 783262146, 4283414179, 1598461184, 3449454717, 2134925443, 3312871027, 1661176967, 3389269267, 2301797239, 1080723875, 2109950181, 2812503900, 2200151129, 326518871, 436685863, 75294102, 2268499418, 1827353184, 3829533623, 398153712, 1431368488, 1935861064, 3414333878, 3454729679, 515875746, 118910790, 2744535111, 1908882569, 1289628113, 2736368962, 450735441, 1269702670, 1496241858, 2370886613, 293404713, 1439910936, 133287999, 659400148, 767242943, 1727171713, 3766332317, 3824544762, 453286414, 1083261389, 2445320541, 1952602247, 1924543976, 2022887430, 3109284196, 3572999314, 3974324369, 1265455387, 1828387791, 247045749, 3579979427, 4130074607, 2885795895, 2972620925, 3907868588, 3462476847, 3238911794, 3344237194, 222345080, 3144953438, 2478823419, 2846118517, 3646440276, 1622471692, 2081790986, 4003610692, 2014752942, 1915059039, 784189300, 450611690, 297968919, 453713806, 2274654609, 348377414, 1176249669, 1804574958, 2059287085, 3700455492, 2447689203, 3186542791, 2738634509, 232481089, 2263464694, 4199781719, 673845122, 3210577832, 697146703, 3821631209, 1130175994, 910063017, 2558723505, 96944796, 2927389256, 357426512, 1101992783, 1857052204, 1091377096, 4244547779, 2946549637, 3239905845, 2129018448, 2965267303, 4014557069, 1236565512, 310962741, 28949398, 2702015869, 2800909897, 1600649093, 889026638, 56551268, 313938541, 1392908242, 693434413, 402526945, 3586007895 } }; } }
        public static UnsignedBigInteger One { get { return new UnsignedBigInteger(new uint[] { 1 }, 1); } }
        public static UnsignedBigInteger Zero { get { return new UnsignedBigInteger(new uint[1], 1); } }

        public long BitsLength { get { return BigIntInternal.UnsignedBitsLength(this._digits, this._digitLength); } }
        public int BytesLength { get { return BigIntInternal.UnsignedBytesLength(this._digits, this._digitLength); } }
        public int DigitsLength { get { return this._digitLength; } }
        public bool IsEven { get { return (this._digits[0] & 1) == 0; } }
        public bool IsOdd { get { return (this._digits[0] & 1) == 1; } }
        public bool IsOne { get { return this._digitLength == 1 && this._digits[0] == 1; } }
        public bool IsPowerOfTwo
        {
            get
            {
                int uiLast = this._digitLength - 1;
                uint uLast = this._digits[uiLast--];

                if ((uLast & (uLast - 1)) != 0)
                    return false;

                while (uiLast > -1)
                    if (this._digits[uiLast--] != 0)
                        return false;

                return true;
            }
        }
        public bool IsZero { get { return this._digitLength == 1 && this._digits[0] == 0; } }
        public uint FirstDigit { get { return this._digits[0]; } }

        public UnsignedBigInteger()
        {
            this._digitLength = 1;
            this._digits = new uint[1];
        }
        public UnsignedBigInteger(byte value)
        {
            this._digits = new uint[1] { value };
            this._digitLength = 1;
        }
        public UnsignedBigInteger(ushort value)
        {
            this._digits = new uint[1] { value };
            this._digitLength = 1;
        }
        public UnsignedBigInteger(uint value)
        {
            this._digits = new uint[1] { value };
            this._digitLength = 1;
        }
        public UnsignedBigInteger(ulong value)
        {
            uint hi = (uint)(value >> 32);
            uint lo = (uint)value;
            this._digits = new uint[2] { lo, hi };
            this._digitLength = 2;
        }
        public UnsignedBigInteger(byte[] bytes)
        {
            this._digits = BigIntInternal.FromUnsignedBytes(bytes, true, out this._digitLength);
        }
        public UnsignedBigInteger(byte[] bytes, bool bigEndian)
        {
            this._digits = BigIntInternal.FromUnsignedBytes(bytes, bigEndian, out this._digitLength);
        }
        public UnsignedBigInteger(uint[] digits, int digitLength)
        {
            while (digitLength > 1 && digits[digitLength - 1] == 0)
                digitLength--;

            if (digitLength == 0)
            {
                this._digits = new uint[1];
                this._digitLength = 1;
            }
            else
            {
                this._digits = new uint[digitLength];
                Array.Copy(digits, this._digits, digitLength);
                this._digitLength = digitLength;
            }
        }

        public static UnsignedBigInteger AbovePowerOfTwo(UnsignedBigInteger value)
        {
            if (value.IsZero)
                return UnsignedBigInteger.Zero;

            uint lastDigit = value._digits[value._digitLength - 1];
            int cbit = 0;

            if (lastDigit == 0)
                cbit = 32;
            else
            {
                if ((lastDigit & 0xFFFF0000) == 0)
                {
                    cbit += 16;
                    lastDigit <<= 16;
                }
                if ((lastDigit & 0xFF000000) == 0)
                {
                    cbit += 8;
                    lastDigit <<= 8;
                }
                if ((lastDigit & 0xF0000000) == 0)
                {
                    cbit += 4;
                    lastDigit <<= 4;
                }
                if ((lastDigit & 0xC0000000) == 0)
                {
                    cbit += 2;
                    lastDigit <<= 2;
                }
                if ((lastDigit & 0x80000000) == 0)
                    cbit += 1;
            }


            uint[] powerOfTwo;
            if (cbit == 32)
            {
                powerOfTwo = new uint[value._digitLength + 1];
                powerOfTwo[powerOfTwo.Length - 1] = 1;
            }
            else
            {
                powerOfTwo = new uint[value._digitLength];
                powerOfTwo[powerOfTwo.Length - 1] = ((uint)1 << (32 - cbit));
            }

            return new UnsignedBigInteger() { _digitLength = powerOfTwo.Length, _digits = powerOfTwo };
        }
        public static UnsignedBigInteger Add(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            int resultLength;
            uint[] result = BigIntInternal.Add(left._digits, left._digitLength, right._digits, right._digitLength, out resultLength);
            return new UnsignedBigInteger(result, resultLength);
        }
        public static UnsignedBigInteger BelowPowerOfTwo(UnsignedBigInteger value)
        {
            if (value.IsZero)
                return UnsignedBigInteger.Zero;

            UnsignedBigInteger below = UnsignedBigInteger.AbovePowerOfTwo(value);
            below.RightShift(1);
            return below;
        }
        public static UnsignedBigInteger Divide(UnsignedBigInteger dividend, UnsignedBigInteger divisor)
        {
            if (divisor.IsZero)
                throw new ArithmeticException("0'a bölünemez.");

            int c = BigIntInternal.Compare(dividend._digits, dividend._digitLength, divisor._digits, divisor._digitLength);
            if (c == -1)
                return UnsignedBigInteger.Zero;
            else if (c == 0)
                return UnsignedBigInteger.One;

            UnsignedBigInteger cDividend = dividend.Copy();
            int quotLength;
            uint remainder;
            uint[] quot;
            if (divisor._digitLength == 1)
                quot = BigIntInternal.DivRemSingle(cDividend._digits, cDividend._digitLength, divisor._digits[0], out quotLength, out remainder);
            else
                quot = BigIntInternal.Div(cDividend._digits, cDividend._digitLength, divisor._digits, divisor._digitLength, out quotLength);
            return new UnsignedBigInteger(quot, quotLength);
        }
        public static UnsignedBigInteger DivRem(UnsignedBigInteger dividend, UnsignedBigInteger divisor, out UnsignedBigInteger remainder)
        {
            if (divisor.IsZero)
                throw new ArithmeticException("0'a bölünemez.");

            UnsignedBigInteger cDividend = dividend.Copy();
            int quotLength;
            uint[] quot;
            if (divisor._digitLength == 1)
            {
                uint rem;
                quot = BigIntInternal.DivRemSingle(cDividend._digits, cDividend._digitLength, divisor._digits[0], out quotLength, out rem);
                remainder = new UnsignedBigInteger(new uint[] { rem }, 1);
            }
            else
            {
                quot = BigIntInternal.Div(cDividend._digits, cDividend._digitLength, divisor._digits, divisor._digitLength, out quotLength);
                remainder = cDividend;
            }
            return new UnsignedBigInteger(quot, quotLength);
        }
        public static UnsignedBigInteger GreatestCommonDivisor(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            if (right.IsZero)
                return left.Copy();
            else
                return UnsignedBigInteger.GreatestCommonDivisor(right, UnsignedBigInteger.Remainder(left, right));
        }
        public static UnsignedBigInteger LeastCommonMultiple(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            UnsignedBigInteger gcd = UnsignedBigInteger.GreatestCommonDivisor(left, right);
            return UnsignedBigInteger.Multiply(gcd, UnsignedBigInteger.Multiply(UnsignedBigInteger.Divide(left, gcd), UnsignedBigInteger.Divide(right, gcd)));
        }
        public static UnsignedBigInteger LeftShift(UnsignedBigInteger value, int shift)
        {
            if (value.IsZero)
                return UnsignedBigInteger.Zero;
            if (shift < 0)
                return UnsignedBigInteger.RightShift(value, -shift);

            uint[] digs = value._digits.Clone() as uint[];
            int len = value._digitLength;
            BigIntInternal.ShiftLeft(ref digs, ref len, shift);
            return new UnsignedBigInteger() { _digitLength = len, _digits = digs };
        }
        public static BigDecimal Lb(UnsignedBigInteger value, Epsilon epsilon)
        {
            BigDecimal mNext = value.Copy();

            int lbInt = (int)((UnsignedBigInteger)mNext.PartOfInteger).BitsLength - 1;

            mNext.RightShift(lbInt);
            lbInt--;

            UnsignedBigInteger decimalPart = 1;
            int bit = 0;

            for (int i = 1; i < epsilon.BinarySensitive; i++)
            {
                decimalPart.LeftShift(1);
                mNext = BigDecimal.Square(mNext);
                mNext.Simplify(epsilon);
                bit = (int)((UnsignedBigInteger)mNext.PartOfInteger).BitsLength - 1;
                decimalPart |= (uint)bit;
                mNext.RightShift(bit);
            }

            BigDecimal dec = decimalPart;
            dec.RightShift(epsilon.BinarySensitive - 1);
            return (lbInt) + dec;
        }
        public static BigDecimal Ln(UnsignedBigInteger value, Epsilon epsilon)
        {
            BigDecimal lb = UnsignedBigInteger.Lb(value, epsilon);
            BigDecimal ln2 = BigDecimal.Ln2;
            ln2.Simplify(epsilon * new Epsilon(3));
            lb *= ln2;
            lb.Simplify(epsilon);
            return lb;
        }
        public static BigDecimal Log(UnsignedBigInteger value, BigDecimal baseValue, Epsilon epsilon)
        {
            BigDecimal lbBase = BigDecimal.Lb(baseValue, epsilon);
            BigDecimal lb = UnsignedBigInteger.Lb(value, epsilon);
            lb /= lbBase;
            lb.Simplify(epsilon);
            return lb;
        }
        public static UnsignedBigInteger Max(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            int c = BigIntInternal.Compare(left._digits, left._digitLength, right._digits, right._digitLength);
            if (c == 1)
                return left;
            else
                return right;
        }
        public static UnsignedBigInteger MaxValueOf(int byteLength)
        {
            byte[] bytes = new byte[byteLength];
            for (int i = 0; i < byteLength; i++)
                bytes[i] = 255;
            return new UnsignedBigInteger(bytes);
        }
        public static UnsignedBigInteger Min(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            int c = BigIntInternal.Compare(left._digits, left._digitLength, right._digits, right._digitLength);
            if (c == 1)
                return right;
            else
                return left;
        }
        public static UnsignedBigInteger MinValueOf(int byteLength)
        {
            byte[] bytes = new byte[byteLength];
            bytes[byteLength - 1] = 1;
            return new UnsignedBigInteger(bytes);
        }
        public static UnsignedBigInteger ModInverse(UnsignedBigInteger K, UnsignedBigInteger modulus)
        {
            if (K.IsOne)
                return UnsignedBigInteger.One;

            BigInteger x1 = BigInteger.Zero, x2 = BigInteger.ReferencedFrom(modulus), y1 = BigInteger.One, y2 = BigInteger.ReferencedFrom(K);

            BigInteger t1, t2, q = BigInteger.DivRem(x2, y2, out t2);
            q.Negate();
            t1 = q;

            while (!y2.IsOne)
            {
                if (t2.Sign == 0)
                    return UnsignedBigInteger.Zero;

                x1 = y1; x2 = y2;
                y1 = t1; y2 = t2;
                q = BigInteger.DivRem(x2, y2, out t2);

                t1 = BigInteger.Subtract(x1, BigInteger.Multiply(q, y1));
            }
            if (y1.Sign == -1)
                return UnsignedBigInteger.ReferrencedFrom(BigInteger.Add(y1, modulus));
            else
                return UnsignedBigInteger.ReferrencedFrom(y1);
        }
        public static UnsignedBigInteger ModPow(UnsignedBigInteger value, UnsignedBigInteger exponent, UnsignedBigInteger modulus)
        {
            UnsignedBigInteger result = UnsignedBigInteger.One;
            long bitLength = exponent.BitsLength;
            for (long i = 0; i < bitLength; i++)
            {
                int bit = exponent.GetBit(i);
                if (value._digitLength > modulus._digitLength)
                    value = UnsignedBigInteger.Remainder(value, modulus);
                if (bit == 1)
                {
                    result = UnsignedBigInteger.Multiply(result, value);
                    if (result.BytesLength > modulus.BytesLength)
                        result = UnsignedBigInteger.Remainder(result, modulus);
                }
                value = UnsignedBigInteger.Square(value);
            }
            return UnsignedBigInteger.Remainder(result, modulus);
        }
        public static UnsignedBigInteger Multiply(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            int resultLength;
            uint[] result;
            if (right._digitLength == 1)
                result = BigIntInternal.MulSingle(left._digits, left._digitLength, right._digits[0], out resultLength);
            else
                result = BigIntInternal.Mul(left._digits, left._digitLength, right._digits, right._digitLength, out resultLength);
            return new UnsignedBigInteger(result, resultLength);
        }
        public static UnsignedBigInteger PollardRhoFactorization(UnsignedBigInteger value)
        {
            if (value.IsZero)
                return UnsignedBigInteger.Zero;
            if (value.IsOne)
                return UnsignedBigInteger.One;

            UnsignedBigInteger a = 5 % value;
            UnsignedBigInteger b = 26 % value;
            while (true)
            {
                UnsignedBigInteger c = a < b ? b - a : a - b;
                UnsignedBigInteger d = UnsignedBigInteger.GreatestCommonDivisor(c, value);
                if (d.IsOne)
                {
                    a = (UnsignedBigInteger.Square(a) + 1) % value;
                    b = (UnsignedBigInteger.Square(b) + 1) % value;
                    b = (UnsignedBigInteger.Square(b) + 1) % value;
                    continue;
                }
                else return d;
            }
        }
        public static UnsignedBigInteger Pow(UnsignedBigInteger value, uint exponent)
        {
            if (exponent == 1)
                return value.Copy();
            if (exponent == 0)
                return UnsignedBigInteger.One;

            UnsignedBigInteger res = UnsignedBigInteger.One;
            if ((exponent & 1) == 1)
                res = value;

            while (true)
            {
                exponent >>= 1;
                if (exponent == 0) break;
                value = UnsignedBigInteger.Square(value);
                if ((exponent & 1) == 1)
                    res = UnsignedBigInteger.Multiply(value, res);
            }
            return res;
        }
        public static UnsignedBigInteger Pow(UnsignedBigInteger value, UnsignedBigInteger exponent)
        {
            if (exponent == 1)
                return value.Copy();
            if (exponent == 0)
                return UnsignedBigInteger.One;

            UnsignedBigInteger res = UnsignedBigInteger.One;

            int bitLength = (int)exponent.BitsLength;

            for (int i = 0; i < bitLength; i++)
            {
                int bit = exponent.GetBit(i);
                if (bit == 1)
                    res = UnsignedBigInteger.Multiply(value, res);
                if (i + 1 == bitLength)
                    break;
                value = UnsignedBigInteger.Square(value);
            }
            return res;
        }
        public static UnsignedBigInteger Random(int byteLength)
        {
            return new UnsignedBigInteger(RandomGenerator.GenerateNonZeroBytes(byteLength));
        }
        public static UnsignedBigInteger Random(UnsignedBigInteger min, UnsignedBigInteger max)
        {
            int c = BigIntInternal.Compare(max._digits, max._digitLength, min._digits, min._digitLength);
            if (c == 1)
            {
                UnsignedBigInteger random = UnsignedBigInteger.Random(max.BytesLength);
                UnsignedBigInteger dif = UnsignedBigInteger.Subtract(max, min);
                random = random % dif;
                random = random + min;
                return random;
            }
            else { throw new InvalidOperationException("Max değeri, min değerinden büyük olmalı."); }
        }
        public static UnsignedBigInteger RandomEven(int byteLength)
        {
            UnsignedBigInteger r = UnsignedBigInteger.Random(byteLength);
            uint firstDigit = r._digits[0];
            firstDigit &= (uint.MaxValue - 1);
            r._digits[0] = firstDigit;
            return r;
        }
        public static UnsignedBigInteger RandomOdd(int byteLength)
        {
            UnsignedBigInteger r = UnsignedBigInteger.Random(byteLength);
            uint firstDigit = r._digits[0];
            firstDigit |= 1;
            r._digits[0] = firstDigit;
            return r;
        }
        public static UnsignedBigInteger RandomPrime(int byteLength)
        {
            if (byteLength > 63)
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(byteLength << 4);
                RSAParameters parameters = rsa.ExportParameters(true);
                return new UnsignedBigInteger(parameters.P, true);
            }

            while (true)
            {
                UnsignedBigInteger P = UnsignedBigInteger.Random(byteLength);
                UnsignedBigInteger Q = new UnsignedBigInteger(3);
                P -= P % 6;
                P -= UnsignedBigInteger.One;

                for (int i = 0; true; i += 2)
                {
                    if (UnsignedBigInteger.ModPow(new UnsignedBigInteger(2), (P - UnsignedBigInteger.One), P).IsOne)
                        break;
                    BigIntInternal.AddSingle(ref P._digits, ref P._digitLength, 2);
                    if (UnsignedBigInteger.ModPow(new UnsignedBigInteger(2), (P - UnsignedBigInteger.One), P).IsOne)
                        break;
                    BigIntInternal.AddSingle(ref P._digits, ref P._digitLength, 4);
                }

                if (UnsignedBigInteger.MillerRabinTest(P, 1))
                    return P;
            }
        }
        public static UnsignedBigInteger RandomRelativelyPrime(UnsignedBigInteger modulus)
        {
            if (modulus < 3)
                throw new ArithmeticException("Modulus must be greater than two.");

            int l = modulus.BytesLength;
            UnsignedBigInteger min = UnsignedBigInteger.MinValueOf(modulus.BytesLength) - 1;
            UnsignedBigInteger r = UnsignedBigInteger.Random(min, modulus);

            UnsignedBigInteger gcd = UnsignedBigInteger.GreatestCommonDivisor(r, modulus);
            while (gcd != 1)
            {
                r = r / gcd;
                gcd = UnsignedBigInteger.GreatestCommonDivisor(r, modulus);
            }
            return r;
        }
        public static UnsignedBigInteger Remainder(UnsignedBigInteger dividend, UnsignedBigInteger divisor)
        {
            if (divisor.IsZero)
                throw new ArithmeticException("0'a bölünemez.");

            int c = dividend.CompareTo(divisor);
            if (c == -1)
                return dividend.Copy();
            else if (c == 0)
                return UnsignedBigInteger.Zero;


            int quotLength;
            uint remainder;
            if (divisor._digitLength == 1)
            {
                BigIntInternal.DivRemSingle(dividend._digits, dividend._digitLength, divisor._digits[0], out quotLength, out remainder);
                return new UnsignedBigInteger(new uint[] { remainder }, 1);
            }
            else
            {
                UnsignedBigInteger cDividend = dividend.Copy();
                BigIntInternal.Rem(ref cDividend._digits, ref cDividend._digitLength, divisor._digits, divisor._digitLength);
                return cDividend;
            }
        }
        public static UnsignedBigInteger RightShift(UnsignedBigInteger value, int shift)
        {
            if (value.IsZero)
                return UnsignedBigInteger.Zero;
            if (shift < 0)
                return UnsignedBigInteger.LeftShift(value, -shift);

            uint[] digs = value._digits.Clone() as uint[];
            int len = value._digitLength;
            BigIntInternal.ShiftRight(ref digs, ref len, shift);
            return new UnsignedBigInteger() { _digitLength = len, _digits = digs };
        }
        public static UnsignedBigInteger Square(UnsignedBigInteger value)
        {
            return UnsignedBigInteger.Multiply(value, value);
        }
        public static UnsignedBigInteger SquareRoot(UnsignedBigInteger value)
        {
            UnsignedBigInteger xf = UnsignedBigInteger.One;
            UnsignedBigInteger xl = UnsignedBigInteger.One;
            do
            {
                xf = xl;
                xl = xf + value / xf;
                xl.RightShift(1);
            } while (!(xf == xl || xf == xl - 1));

            return xf;
        }
        public static UnsignedBigInteger Subtract(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            int c = BigIntInternal.Compare(left._digits, left._digitLength, right._digits, right._digitLength);
            if (c == 1)
            {
                int resultLength;
                uint[] result = BigIntInternal.Sub(left._digits, left._digitLength, right._digits, right._digitLength, out resultLength);
                return new UnsignedBigInteger(result, resultLength);
            }
            else if (c == 0)
            {
                return new UnsignedBigInteger();
            }
            else
                throw new OverflowException();
        }
        public static UnsignedBigInteger TwoPower(long exponent)
        {
            if (exponent > BigIntInternal.maxBitsLength)
                throw new OverflowException("Exponent cannot be longer than " + BigIntInternal.maxBitsLength + ".");
            if (exponent == 0)
                return UnsignedBigInteger.One;

            int digitLength = (int)(exponent / 32) + 1;
            uint[] digits = new uint[digitLength];
            digits[digitLength - 1] = ((uint)1 << (int)(exponent & 31));
            return new UnsignedBigInteger() { _digitLength = digitLength, _digits = digits };
        }

        public static bool FermatLittleTest(UnsignedBigInteger value)
        {
            UnsignedBigInteger baseValue = new UnsignedBigInteger(2);
            return UnsignedBigInteger.ModPow(baseValue, UnsignedBigInteger.Subtract(value, UnsignedBigInteger.One), value).IsOne;
        }
        public static bool MillerRabinTest(UnsignedBigInteger value, int certainty)
        {
            if (value == 2 || value == 3)
                return true;
            if (value < 2 || value.IsEven)
                return false;

            UnsignedBigInteger valueMinusOne = value - 1;
            UnsignedBigInteger valueMinustwo = value - 2;
            UnsignedBigInteger d = valueMinusOne.Copy();
            int s = 0;
            while (d.IsEven)
            {
                d >>= 1;
                s += 1;
            }

            for (int i = 0; i < certainty; i++)
            {
                UnsignedBigInteger a = UnsignedBigInteger.Random(2, valueMinustwo);
                UnsignedBigInteger x = UnsignedBigInteger.ModPow(a, d, value);
                if (x.IsOne || x == valueMinusOne)
                    continue;
                for (int r = 1; r < s; r++)
                {
                    x = UnsignedBigInteger.ModPow(x, 2, value);
                    if (x.IsOne)
                        return false;
                    if (x == valueMinusOne)
                        break;
                }
                if (x != valueMinusOne)
                    return false;
            }
            return true;
        }

        public static UnsignedBigInteger Parse(string value)
        {
            int digitLength;
            uint[] digits;
            try
            {
                digits = BigIntInternal.UnsignedParse(value, out digitLength);
                return new UnsignedBigInteger(digits, digitLength);
            }
            catch (Exception)
            {
                throw new FormatException();
            }
        }
        public static UnsignedBigInteger ParseFromHex(string hex)
        {
            hex = hex.ToLower();

            if (hex.StartsWith("0x"))
                hex = hex.Substring(2, hex.Length - 2);

            int byteLength = (hex.Length + 1) / 2;
            byte[] bytes = new byte[byteLength];

            if ((hex.Length & 1) == 1)
                hex = "0" + hex;
            for (int i = 0; i < byteLength; i++)
            {
                char c1 = hex[2 * i];
                char c2 = hex[2 * i + 1];
                if (c1 > 96)
                    bytes[i] |= (byte)((c1 - 87) << 4);
                else
                    bytes[i] |= (byte)((c1 - 48) << 4);

                if (c2 > 96)
                    bytes[i] |= (byte)((c2 - 87));
                else
                    bytes[i] |= (byte)((c2 - 48));
            }
            int digitLength;
            uint[] digits = BigIntInternal.FromUnsignedBytes(bytes, true, out digitLength);
            if (digitLength == 1 && digits[0] == 0)
                return UnsignedBigInteger.Zero;
            return new UnsignedBigInteger(digits, digitLength);
        }
        public static bool TryParse(string value, out UnsignedBigInteger result)
        {
            int digitLength;
            uint[] digits;
            try
            {
                digits = BigIntInternal.UnsignedParse(value, out digitLength);
                result = new UnsignedBigInteger(digits, digitLength);
                return true;
            }
            catch (Exception)
            {
                result = UnsignedBigInteger.Zero;
                return false;
            }
        }

        public int CompareTo(UnsignedBigInteger other)
        {
            return BigIntInternal.Compare(this._digits, this._digitLength, other._digits, other._digitLength);
        }
        public UnsignedBigInteger Copy()
        {
            uint[] digit = this.GetDigits();
            return new UnsignedBigInteger(digit, digit.Length);
        }
        public void Decrease(uint value)
        {
            BigIntInternal.SubSingle(ref this._digits, ref this._digitLength, value);
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(UnsignedBigInteger))
                return this == (UnsignedBigInteger)obj;
            else
                throw new InvalidCastException();
        }
        public bool Equals(UnsignedBigInteger other)
        {
            return this.CompareTo(other) == 0;
        }
        public int GetBit(long bitPosition)
        {
            int digitPos = (int)(bitPosition / 32);
            if (this._digitLength <= digitPos)
                return 0;

            int smallBitPos = (int)(bitPosition & 31);
            return (int)((this._digits[digitPos] >> smallBitPos) & 1);
        }
        public byte[] GetBytes()
        {
            return BigIntInternal.GetUnsignedBytes(this._digits, this._digitLength, true);
        }
        public byte[] GetBytes(bool bigEndian)
        {
            return BigIntInternal.GetUnsignedBytes(this._digits, this._digitLength, bigEndian);
        }
        public uint GetDigit(int position)
        {
            if (position < this._digitLength)
                return this._digits[position];
            else return 0;
        }
        public uint[] GetDigits()
        {
            uint[] digits = new uint[this._digitLength];
            Array.Copy(this._digits, digits, this._digitLength);
            return digits;
        }
        public int[] NonAdjacentForm(int w)
        {
            UnsignedBigInteger d = this.Copy();
            int modulus = 1 << w;

            int[] naf = new int[d.BitsLength + 1];
            int modMinOne = modulus - 1;
            int halfOfModulus = modulus >> 1;
            for (int i = 0; !d.IsZero; i++)
            {
                if (d.IsOdd)
                {
                    int mod = (int)d.FirstDigit & modMinOne; //d mod 2 ^ w

                    if (mod >= halfOfModulus)
                    {
                        int inc = modulus - mod;
                        naf[i] = -inc;
                        d.Increase((uint)inc);
                    }
                    else
                    {
                        naf[i] = mod;
                        d.Decrease((uint)mod);
                    }
                }
                d.RightShift(1);
            }
            return naf;
        }
        public override int GetHashCode()
        {
            uint digSum = 0;
            for (int i = 0; i < this._digitLength; i++)
                digSum += this._digits[i];
            return (int)digSum;
        }
        public void Increase(uint value)
        {
            BigIntInternal.AddSingle(ref this._digits, ref this._digitLength, value);
        }
        public void LeftShift(int shift)
        {
            if (this.IsZero)
                return;
            if (shift < 0)
            {
                BigIntInternal.ShiftRight(ref this._digits, ref this._digitLength, -shift);
            }
            else
            {
                BigIntInternal.ShiftLeft(ref this._digits, ref this._digitLength, shift);
            }
        }
        public void RightShift(int shift)
        {
            if (this.IsZero)
                return;

            if (shift < 0)
            {
                BigIntInternal.ShiftLeft(ref this._digits, ref this._digitLength, -shift);
            }
            else
            {
                BigIntInternal.ShiftRight(ref this._digits, ref this._digitLength, shift);
            }
        }
        public void SetBit(long bitPosition, int bitValue)
        {
            if (bitValue != 1 && bitValue != 0)
                throw new InvalidOperationException("Geçersiz bit değeri.");
            if (bitPosition >= BigIntInternal.maxBitsLength)
                throw new OverflowException("Bit pozisyonu sınırların dışında.");

            BigIntInternal.SetBit(ref this._digits, ref this._digitLength, bitPosition, bitValue);
        }
        public void SetDigit(int position, uint value)
        {
            if (position >= BigIntInternal.maxDigitsLength)
                throw new OverflowException("Basamak pozisyonu sınırların dışında.");

            if (position < this._digitLength)
            {
                this._digits[position] = value;
                if (value == 0 && position == this._digitLength - 1)
                    while (this._digitLength > 1 && this._digits[this._digitLength - 1] == 0)
                        this._digitLength--;
            }
            else
            {
                if (value > 0)
                {
                    this._digitLength = position + 1;
                    Array.Resize(ref this._digits, this._digitLength);
                    this._digits[position] = value;
                }
            }
        }
        public override string ToString()
        {
            return BigIntInternal.ToString(this._digits, this._digitLength, 1);
        }

        internal static UnsignedBigInteger ReferrencedFrom(BigInteger value)
        {
            return new UnsignedBigInteger() { _digitLength = value.DigitsLength, _digits = value.digitsRef };
        }

        public static UnsignedBigInteger operator +(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            return UnsignedBigInteger.Add(left, right);
        }
        public static UnsignedBigInteger operator -(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            return UnsignedBigInteger.Subtract(left, right);
        }
        public static UnsignedBigInteger operator *(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            return UnsignedBigInteger.Multiply(left, right);
        }
        public static UnsignedBigInteger operator /(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            return UnsignedBigInteger.Divide(left, right);
        }
        public static UnsignedBigInteger operator %(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            return UnsignedBigInteger.Remainder(left, right);
        }
        public static UnsignedBigInteger operator &(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            int resultLength;
            uint[] digits = BigIntInternal.And(left._digits, left._digitLength, right._digits, right._digitLength, out resultLength);
            return new UnsignedBigInteger() { _digitLength = resultLength, _digits = digits };
        }
        public static UnsignedBigInteger operator |(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            uint[] digits = BigIntInternal.Or(left._digits, left._digitLength, right._digits, right._digitLength);
            return new UnsignedBigInteger() { _digitLength = digits.Length, _digits = digits };
        }
        public static UnsignedBigInteger operator ^(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            uint[] digits = BigIntInternal.Xor(left._digits, left._digitLength, right._digits, right._digitLength);
            return new UnsignedBigInteger() { _digitLength = digits.Length, _digits = digits };
        }
        public static UnsignedBigInteger operator <<(UnsignedBigInteger left, int shift)
        {
            return UnsignedBigInteger.LeftShift(left, shift);
        }
        public static UnsignedBigInteger operator >>(UnsignedBigInteger left, int shift)
        {
            return UnsignedBigInteger.RightShift(left, shift);
        }

        public static UnsignedBigInteger operator ~(UnsignedBigInteger value)
        {
            UnsignedBigInteger cVal = value.Copy();
            BigIntInternal.Not(ref cVal._digits, ref cVal._digitLength);
            return cVal;
        }
        public static UnsignedBigInteger operator ++(UnsignedBigInteger value)
        {
            return value + 1;
        }
        public static UnsignedBigInteger operator --(UnsignedBigInteger value)
        {
            return value - 1;
        }

        public static bool operator ==(UnsignedBigInteger left, sbyte right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) == 0;
        }
        public static bool operator !=(UnsignedBigInteger left, sbyte right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) != 0;
        }
        public static bool operator <(UnsignedBigInteger left, sbyte right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) == -1;
        }
        public static bool operator >(UnsignedBigInteger left, sbyte right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) == 1;
        }
        public static bool operator <=(UnsignedBigInteger left, sbyte right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) < 1;
        }
        public static bool operator >=(UnsignedBigInteger left, sbyte right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) > -1;
        }
        public static bool operator ==(UnsignedBigInteger left, byte right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(UnsignedBigInteger left, byte right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(UnsignedBigInteger left, byte right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(UnsignedBigInteger left, byte right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(UnsignedBigInteger left, byte right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(UnsignedBigInteger left, byte right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(UnsignedBigInteger left, short right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) == 0;
        }
        public static bool operator !=(UnsignedBigInteger left, short right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) != 0;
        }
        public static bool operator <(UnsignedBigInteger left, short right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) == -1;
        }
        public static bool operator >(UnsignedBigInteger left, short right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) == 1;
        }
        public static bool operator <=(UnsignedBigInteger left, short right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) < 1;
        }
        public static bool operator >=(UnsignedBigInteger left, short right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) > -1;
        }
        public static bool operator ==(UnsignedBigInteger left, ushort right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(UnsignedBigInteger left, ushort right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(UnsignedBigInteger left, ushort right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(UnsignedBigInteger left, ushort right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(UnsignedBigInteger left, ushort right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(UnsignedBigInteger left, ushort right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(UnsignedBigInteger left, int right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) == 0;
        }
        public static bool operator !=(UnsignedBigInteger left, int right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) != 0;
        }
        public static bool operator <(UnsignedBigInteger left, int right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) == -1;
        }
        public static bool operator >(UnsignedBigInteger left, int right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) == 1;
        }
        public static bool operator <=(UnsignedBigInteger left, int right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) < 1;
        }
        public static bool operator >=(UnsignedBigInteger left, int right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) > -1;
        }
        public static bool operator ==(UnsignedBigInteger left, uint right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(UnsignedBigInteger left, uint right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(UnsignedBigInteger left, uint right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(UnsignedBigInteger left, uint right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(UnsignedBigInteger left, uint right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(UnsignedBigInteger left, uint right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(UnsignedBigInteger left, long right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) == 0;
        }
        public static bool operator !=(UnsignedBigInteger left, long right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) != 0;
        }
        public static bool operator <(UnsignedBigInteger left, long right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) == -1;
        }
        public static bool operator >(UnsignedBigInteger left, long right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) == 1;
        }
        public static bool operator <=(UnsignedBigInteger left, long right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) < 1;
        }
        public static bool operator >=(UnsignedBigInteger left, long right)
        {
            if (right < 0)
                return false;

            return left.CompareTo((UnsignedBigInteger)(right)) > -1;
        }
        public static bool operator ==(UnsignedBigInteger left, ulong right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(UnsignedBigInteger left, ulong right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(UnsignedBigInteger left, ulong right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(UnsignedBigInteger left, ulong right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(UnsignedBigInteger left, ulong right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(UnsignedBigInteger left, ulong right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(UnsignedBigInteger left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) > -1;
        }

        public static implicit operator UnsignedBigInteger(byte value)
        {
            return new UnsignedBigInteger(value);
        }
        public static implicit operator UnsignedBigInteger(ushort value)
        {
            return new UnsignedBigInteger(value);
        }
        public static implicit operator UnsignedBigInteger(uint value)
        {
            return new UnsignedBigInteger(value);
        }
        public static implicit operator UnsignedBigInteger(ulong value)
        {
            return new UnsignedBigInteger(value);
        }
        public static implicit operator UnsignedBigInteger(string value)
        {
            return UnsignedBigInteger.Parse(value);
        }
        public static explicit operator UnsignedBigInteger(sbyte value)
        {
            byte v = (byte)value;
            return new UnsignedBigInteger(v);
        }
        public static explicit operator UnsignedBigInteger(short value)
        {
            ushort v = (ushort)value;
            return new UnsignedBigInteger(v);
        }
        public static explicit operator UnsignedBigInteger(int value)
        {
            uint v = (uint)value;
            return new UnsignedBigInteger(v);
        }
        public static explicit operator UnsignedBigInteger(long value)
        {
            ulong v = (ulong)value;
            return new UnsignedBigInteger(v);
        }
        public static explicit operator UnsignedBigInteger(BigInteger value)
        {
            if (value.Sign == 0)
                return UnsignedBigInteger.Zero;
            else
            {
                int sign;
                uint[] digits = value.GetDigits(out sign);
                if (sign == 1)
                    return new UnsignedBigInteger(digits, digits.Length);
                else
                {
                    int digitLength = digits.Length;
                    BigIntInternal.SubSingle(ref digits, ref digitLength, 1);
                    for (int i = 0; i < digitLength; i++)
                        digits[i] = ~digits[i];
                    return new UnsignedBigInteger(digits, digitLength);
                }
            }
        }
        public static explicit operator sbyte(UnsignedBigInteger value)
        {
            return (sbyte)value._digits[0];
        }
        public static explicit operator byte(UnsignedBigInteger value)
        {
            return (byte)value._digits[0];
        }
        public static explicit operator short(UnsignedBigInteger value)
        {
            return (short)value._digits[0];
        }
        public static explicit operator ushort(UnsignedBigInteger value)
        {
            return (ushort)value._digits[0];
        }
        public static explicit operator int(UnsignedBigInteger value)
        {
            return (int)value._digits[0];
        }
        public static explicit operator uint(UnsignedBigInteger value)
        {
            return value._digits[0];
        }
        public static explicit operator long(UnsignedBigInteger value)
        {
            ulong val = value._digits[0];
            val |= value._digits[1] << 32;
            return (long)val;
        }
        public static explicit operator ulong(UnsignedBigInteger value)
        {
            ulong val = value._digits[0];
            val |= value._digits[1] << 32;
            return val;
        }
    }
}
