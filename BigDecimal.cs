using System;

namespace NetworkIO.Numerics
{
    public class BigDecimal : IComparable<BigDecimal>, IComparable<Epsilon>
    {
        private BigInteger _numerator;
        private UnsignedBigInteger _denominator;
        private static uint[] _tenPowerOfThreeThousand = new uint[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 553648128, 1302307295, 1898753119, 1920164615, 85223686, 759626815, 623424891, 1265749554, 2469018121, 1133263492, 1089797692, 3180177135, 1474094501, 3635463316, 3169289154, 329192535, 2926545825, 996573672, 3444789128, 1238045662, 247442742, 1609249189, 244671689, 2782737009, 3888154982, 2482759693, 1347093858, 296340872, 384232569, 2437589339, 1550146123, 2399716471, 2005085213, 619251707, 2631301830, 1556847599, 1426410795, 1336880822, 2431435153, 3373388992, 1618673133, 1430418015, 262037960, 225676181, 3617454437, 766673100, 1358711681, 1850934796, 3196468009, 1697741162, 338592357, 3374383195, 147787004, 2436035282, 1351226112, 2636541042, 3471874478, 2875446823, 3144358505, 2266209466, 2398795120, 2767818461, 3849090658, 2686883492, 3820969993, 570799668, 2393552219, 3535706786, 558821112, 2112066433, 4028214317, 3169000839, 2836465716, 1144868945, 4262928044, 3653899547, 1445313782, 1934889844, 3473772897, 2836471210, 108293146, 1534044022, 1651447515, 1108644796, 223163758, 2307716284, 1527258097, 1740221939, 1307162016, 3916802983, 1908088301, 162837969, 2509424213, 535624589, 849776242, 3735588907, 4151664337, 3143409762, 1470848403, 3387709406, 718051628, 4152647880, 3105909919, 91412148, 1095003141, 2992026217, 1737817034, 2901698647, 873774804, 3612029048, 1739435255, 4071103861, 1817659321, 1438749702, 299673855, 1150372495, 3110845227, 1806195846, 3776364895, 3004718209, 4178218870, 2692913861, 4149051855, 1136662487, 3504652530, 1070921247, 3887017708, 3271940252, 4218442750, 1337176469, 563919501, 669301469, 229298447, 368306498, 3976659058, 2188141160, 2609473518, 3742174627, 3323258335, 4062854239, 201746029, 2236847847, 3966241107, 1051251413, 3581872259, 3434791845, 2867691316, 2367298282, 1963676848, 3187435578, 2480457191, 4028859586, 654676630, 975876762, 51916066, 1664214850, 4260514500, 1433288716, 830880607, 2585181979, 2679586031, 3863513162, 1923231693, 1807390956, 937887338, 3289482332, 3125438019, 2031547823, 3952371282, 4055080242, 3887421902, 75917363, 3062047137, 3178926176, 36577526, 4293061540, 4265304745, 1284249422, 3541037309, 310607069, 1861320298, 3752707803, 3983716418, 260564083, 1074974833, 260779601, 1126041232, 1637925288, 1793889556, 1511826170, 1404843074, 1227596645, 910894704, 3973211218, 2120208384, 4226606503, 1927361082, 704170320, 2794111284, 2211378004, 2906207149, 354626630, 3529595926, 196092739, 933972847, 584906163, 1033763782, 2167142801, 3380748262, 3089896874, 3340314183, 1597397979, 3397124675, 1238986753, 2015562533, 2466016385, 1809108460, 2469728516, 14108 };

        public static BigDecimal NaN { get { return new BigDecimal() { _denominator = UnsignedBigInteger.Zero, _numerator = BigInteger.Zero }; } }
        public static BigDecimal PositiveInfinity { get { return new BigDecimal() { _denominator = UnsignedBigInteger.Zero, _numerator = BigInteger.One }; } }
        public static BigDecimal NegativeInfinity { get { return new BigDecimal() { _denominator = UnsignedBigInteger.Zero, _numerator = BigInteger.MinusOne }; } }
        public static BigDecimal Zero { get { return new BigDecimal(); } }
        public static BigDecimal One { get { return new BigDecimal() { _numerator = BigInteger.One, _denominator = UnsignedBigInteger.One }; } }
        public static BigDecimal MinusOne { get { return new BigDecimal() { _numerator = BigInteger.MinusOne, _denominator = UnsignedBigInteger.One }; } }
        public static Epsilon Epsilon { get { return new Epsilon(100); } }
        public static BigDecimal E { get { return new BigDecimal(new BigInteger(new uint[] { 3422401758, 949789578, 3468824519, 710273469, 3170963204, 1339042508, 3464025177, 2016552543, 636215438, 1929199495, 3401248798, 4236513917, 3849771048, 587900697, 716691453, 184329390, 3778622927, 524446490, 1027454360, 1299867396, 2956432233, 3296953658, 3547405456, 2974034338, 1656240273, 1964511125, 1477170141, 2039089062, 2606668537, 605881510, 4281602745, 2672974887, 4126995304, 1006805694, 1704936298, 3009386500, 2452596136, 2796500695, 3752127969, 1383494182, 3611789478, 2975616426, 2817148520, 2281000841, 3919613140, 3347225949, 1619564128, 3010741160, 2280058525, 3153133502, 1203366381, 1152195765, 2921876480, 3292903441, 798367731, 3088009154, 2659286567, 318719915, 2868042583, 1018930090, 1786164384, 1331633734, 884551258, 2106854680, 2976062986, 1649330847, 3234286052, 3292002300, 2680665229, 563907821, 400477881, 2677398658, 3501090599, 1476238018, 476954821, 2221226736, 1550650146, 1546831238, 895004590, 3021676848, 4188337313, 1194440884, 2965184, 364966210, 2734704281, 3722653152, 4252115563, 2048576582, 2633440172, 3217397095, 2745456121, 3344919129, 44365171, 4293279379, 2483244558, 1686424010, 2187622166, 3609612832, 1968342624, 1926034782, 2005375420, 2357108123, 2053786833, 1578125922, 194817084, 3481875879, 3643774350, 1817071452, 2469359181, 1453934431, 2134920253, 596121022, 781433686, 1611035659, 80992915, 658597574, 3080831758, 2410362983, 853240866, 2239504077, 1935905615, 3321872096, 3729633101, 2454058661, 1011165381, 1610361799, 3203510832, 3210842585, 696699084, 3263159817, 1466364787, 30334066, 3972659700, 2508065091, 3323892234, 1729607859, 549691839, 2374271293, 2472999914, 8449479, 1900262230, 648568196, 3028701263, 1800982369, 3199162524, 558713718, 1873386703, 3451943200, 1418968540, 174842769, 2029322304, 4035211488, 3471588284, 2962563518, 4216684933, 3374929923, 2919646038, 2963327432, 2745934310, 786593769, 3755235210, 2242446576, 613273392, 503315314, 1190126589, 1183128950, 3219534616, 1668891153, 4010188506, 2190206522, 451623474, 756395497, 605501031, 284702284, 3703368183, 2393018595, 3831173872, 2645079356, 2845846868, 781488671, 1877191643, 1176628678, 4213020183, 3355618140, 869605059, 2470252042, 2497761520, 1917844561, 4224997154, 3941476228, 1808968926, 657815460, 1057022202, 2549997640, 3486517714, 3389111730, 4129754151, 2405411763, 3853647411, 1899761248, 2818315620, 4124436413, 4237410997, 1696315869, 1918820380, 2392504781, 603668408, 2382141637, 997998922, 1806118914, 2445240462, 319789448, 2899824138, 3349554111, 2328265016, 1570267287, 2394206180, 602450532, 3473140433, 1004843748, 578410403, 4243360361, 315256239, 95039940, 3723901798, 1489450110, 2401800224, 1237959724, 2217581677, 2709890315, 2959879210, 2168456789, 2507285312, 1575465840, 463641773, 1220610193, 2061846898, 2782493030, 2927169690, 3391365624, 3856571304, 2827612461, 1859529904, 954263283, 2117716594, 2145637525, 842733419, 770814531, 3193737940, 2713988462, 4252492950, 2259594139, 4136104261, 645665881, 3958188885, 2716977864, 3757611680, 3673998745, 2050985912, 2305487080, 2357924214, 3662552921, 413368133, 3648373489, 2613877079, 3975810582, 3616478690, 921380515, 3861219618, 3379702750, 112540226, 3874886444, 3263264796, 936917436, 2381520978, 1746136742, 962568689, 366832309, 2969146179, 2013754658, 1415048684, 3598997112, 151745631, 201241225, 3274053709, 797181753, 267287587, 2655564971, 1232844809, 589676303, 3292990529, 3503354204, 2095573303, 1346061929, 1844356555, 3335613909, 645691901, 114518854, 471864665, 585035994, 1036362298, 2994248748, 3697015701, 16515838, 1665067916, 3070369153, 2141442259, 2252573675, 1628606167, 2420439397, 357020739, 38351 }, 1), new UnsignedBigInteger(BigDecimal._tenPowerOfThreeThousand, BigDecimal._tenPowerOfThreeThousand.Length)); } }
        public static BigDecimal PI { get { return new BigDecimal(new BigInteger(new uint[] { 3425293721, 2427239414, 2703244538, 3957285935, 2095754675, 3685079176, 3144301978, 4220164682, 1722490942, 1303440481, 2586385473, 938875424, 758708940, 3577997599, 1279253250, 1845374182, 41608408, 2468729694, 3909523231, 4043491564, 1536165278, 3925086495, 4263086474, 2554741887, 1041616086, 2576110271, 1391179365, 4011053845, 2516393876, 472215046, 2683204785, 892097811, 2759450940, 2899159875, 1607735280, 52561685, 2194611298, 2592974550, 2826856611, 3212288858, 235119770, 286292061, 642729759, 542242781, 1179954140, 1209214391, 418249141, 3468819125, 232499466, 1813346099, 2698668156, 3947750967, 3043883796, 1744974864, 2444460402, 1247571950, 965984808, 2657200782, 1693427573, 304442251, 3242306221, 2854295504, 3500229795, 2843999398, 1577871915, 3329819048, 1549185248, 1960469814, 779507435, 659332764, 3244004417, 2680317802, 3117521764, 903981541, 1436498292, 3690062021, 2020367883, 2018596194, 4007581237, 4226676562, 1701419891, 3192596314, 3813837035, 600358315, 3637659485, 1091987624, 2294306290, 3045428188, 1309688452, 1139232086, 1916760843, 1448193124, 2487539147, 3601877865, 890644465, 337271200, 40082817, 1515046868, 2585347644, 1129043704, 436402126, 1980182494, 140543434, 1130359568, 3525585620, 4103372989, 2138661335, 3769919006, 2383026791, 566603158, 1145128712, 759030297, 2705857871, 1281331715, 1731292911, 3341511312, 2584793668, 2695089451, 3121519463, 3100566889, 437075982, 2115187555, 2395722215, 3523437069, 1885685032, 783369161, 2799551589, 776497091, 4168115812, 459490354, 2535550084, 2642502820, 1500298864, 2510658245, 1649771949, 1042662904, 62399269, 1825232110, 1256067403, 1631523261, 2903937846, 1562116305, 3320144555, 3284672988, 132562875, 783309883, 4010721066, 3507241059, 848931877, 658623090, 1086284914, 603963814, 348602363, 460818077, 1825953269, 1100179360, 682400224, 484123011, 2291713760, 2287673791, 2333237635, 3974788459, 4074885792, 505130870, 2020245100, 727087332, 932446080, 1142029682, 3531840817, 899215656, 2658539683, 264748590, 1389568842, 1801911423, 2231991102, 1305233814, 4279953696, 3455204963, 2088878031, 4044366222, 3548588568, 3803569728, 2968034086, 1477607955, 4120405392, 388946129, 2827201721, 3306555947, 3410814041, 372045360, 3010195263, 294373752, 3830179195, 31359968, 502706891, 1261102564, 4077014392, 204178043, 1097316382, 765294323, 2528172836, 1420819781, 1185491456, 1566800283, 1068719560, 201989392, 1496808344, 3197362702, 3426421992, 1239967745, 293864109, 741473443, 331703629, 2357190644, 2044263336, 1778702101, 1569306381, 809873585, 2230581474, 2297547838, 2383842258, 1310708885, 1431804131, 3436106846, 1784899863, 2051291968, 4075635837, 357687958, 856783040, 1021674005, 3088565477, 2182375406, 409090990, 3275937605, 326779072, 2359445373, 157372969, 4085973994, 146728996, 1712126740, 3043680812, 1855389859, 2383661576, 1229319298, 2236845429, 3048398942, 1916354707, 927592834, 2207215764, 4187713056, 737708939, 343317774, 2228661876, 3811005653, 987449070, 2957890464, 931827234, 3579694, 4055448995, 1345702226, 3170089188, 3072770821, 2973358403, 3752458878, 3421876776, 1397746088, 453716337, 3372420582, 933491934, 1662888011, 1303984314, 2951535933, 967282465, 1692319088, 1424295571, 2990467416, 1016252850, 1036938519, 3999980046, 1885127161, 4233236593, 3207488314, 4277634595, 1314094683, 2040771615, 508631969, 2406033702, 1830987528, 128321785, 2026560859, 1885652651, 3463269899, 3818333228, 2393800636, 3249510040, 2083176416, 1473103394, 1523459560, 3763514367, 3042456938, 388181672, 2563822595, 1027717910, 4038643153, 1922827751, 1146176605, 255405194, 271917380, 3468446875, 831414525, 1699355752, 44323 }, 1), new UnsignedBigInteger(BigDecimal._tenPowerOfThreeThousand, BigDecimal._tenPowerOfThreeThousand.Length)); } }
        public static BigDecimal Ln2 { get { return new BigDecimal(new BigInteger(new uint[] { 2256221091, 312928508, 2041784378, 2810846720, 2522339908, 3132160802, 712475729, 320960007, 351737420, 1544331827, 3628105824, 3588637125, 4244873220, 1399638109, 2683074493, 1336471472, 3022160539, 3974918470, 714035716, 2789620380, 1946431898, 3750846116, 2887889958, 2536478175, 395772618, 2975884248, 3973331733, 149907660, 1037478528, 2622113766, 2403108664, 1879330210, 1434860267, 471033783, 4108068530, 3717876476, 322033545, 4189973113, 1719170686, 3403578524, 3403898649, 3104743798, 1518234690, 1691104192, 558981694, 7851136, 803879413, 1883842664, 3044982322, 2835986729, 1697768218, 3608340915, 2707376119, 2363063163, 771765380, 773145456, 1755501329, 2719070397, 3272147522, 1394693900, 46384707, 248626401, 4197309585, 1415615654, 1733865370, 994888442, 1870432331, 330106614, 1041185756, 524684276, 1046426943, 3605600615, 1836864676, 2104075384, 2311517390, 845747938, 3600339248, 1318109639, 2784237080, 2912095350, 1638554370, 3883840838, 1937005341, 2761115860, 1154537622, 861864893, 1494074072, 859760741, 2762737181, 1809035767, 1559545851, 1338912793, 125293172, 1793406778, 2923477078, 3872606388, 1351631159, 2866904523, 1333945499, 8336761, 2959156271, 3680445639, 2916860941, 2280114576, 798468450, 1225677943, 2538871679, 3530676899, 3647637680, 2908743118, 2550091245, 3736205698, 333699828, 871414316, 107436134, 868910267, 4252541905, 1156205244, 229165912, 2855803850, 3194102830, 2862151499, 3543396395, 2809837211, 2925901734, 625453931, 2365567064, 1216611146, 165111700, 1063286313, 3462999980, 1858647229, 664280784, 2941551663, 3098968139, 710448569, 2292968281, 2305764506, 2860437641, 397215588, 3598686657, 1792444671, 10603181, 3392020168, 2516924801, 1453061424, 54248165, 2252124916, 315933276, 1265899419, 1190942699, 2272959483, 1661577754, 4128720697, 1146679556, 1085823329, 1955068773, 2583129296, 2290230183, 495170595, 3303455267, 1593659245, 4547005, 1549376940, 107640960, 1278882467, 1654957785, 3063853565, 3856047121, 1122695959, 94425476, 881828699, 59059510, 3575188453, 2002542934, 3363765451, 2168170376, 2326718688, 2709208544, 3724708444, 1514363448, 3003611045, 3645237933, 167212750, 2290428703, 3473552394, 2850682161, 2583626315, 3117308524, 2044004781, 2718313916, 1288565843, 2318900537, 278107952, 3018702848, 858389458, 4182154588, 1085830771, 1261450855, 4117951301, 890638606, 3979663399, 3699576355, 3325469791, 1804431155, 3729065117, 3025775717, 1063178205, 4088969649, 1061376673, 2290570243, 3002400646, 417189322, 3275327679, 1334451179, 2839566722, 2839895908, 71432384, 2841331126, 779468062, 217823840, 743807290, 3916259200, 3396555012, 4209471019, 1967908968, 237846409, 491585282, 1570975632, 3330896592, 3762375409, 2688561796, 173418136, 4235390071, 3983855532, 4171233336, 330224077, 3838048440, 979647093, 960215742, 3759622617, 1343789798, 18766252, 1626780098, 2673067979, 2916759522, 66338815, 769755940, 980593276, 595127412, 3444019461, 3291995725, 957980133, 2809599471, 3324386969, 1369170248, 3269647452, 2114781201, 648148643, 870760266, 2183361866, 4084243287, 2588104871, 4230245244, 2093445059, 2335308549, 1739619437, 3739917833, 1552070017, 3787555839, 4044138588, 3752812751, 3878541214, 3476121045, 3455841505, 2127720956, 3801794176, 1776728178, 2396925540, 1757606213, 1849217221, 3339999438, 1842377028, 597197822, 4232724999, 3695438663, 2871928384, 3141771561, 688876108, 3007760900, 197850029, 24807564, 4054787257, 3631547889, 545257999, 4161106904, 1684146754, 3530232545, 2599236084, 856491285, 4179727788, 304814471, 2652139104, 1998850876, 2425730088, 211080749, 756942626, 1302279924, 2860201258, 1841025030, 1370106204, 9779 }, 1), new UnsignedBigInteger(BigDecimal._tenPowerOfThreeThousand, BigDecimal._tenPowerOfThreeThousand.Length)); } }

        public bool BetweenOneAndZero { get { return (this._numerator.Sign == 0 && !this._denominator.IsZero) || (this._numerator.Sign == 1 && this._denominator >= this._numerator); } }
        public UnsignedBigInteger Denominator { get { return this._denominator.Copy(); } }
        public bool IsNaN { get { return this._denominator.IsZero && this._numerator.Sign == 0; } }
        public bool IsOne { get { return this._numerator.Sign == 1 && this._numerator == this._denominator; } }
        public bool IsPositiveInfinity { get { return this._denominator.IsZero && this._numerator.Sign == 1; } }
        public bool IsNegativeInfinity { get { return this._denominator.IsZero && this._numerator.Sign == -1; } }
        public bool IsZero { get { return this._numerator.IsZero && !this._denominator.IsZero; } }
        public BigInteger Numerator { get { return this._numerator.Copy(); } }
        public Epsilon Sensitivity { get { return new Epsilon((int)this._denominator.BitsLength, 2); } }
        public BigInteger PartOfInteger { get { if (this._denominator.IsPowerOfTwo) return this._numerator >> (int)this._denominator.BitsLength - 1; else return this._numerator / this._denominator; } }

        public BigDecimal()
        {
            this._numerator = BigInteger.Zero;
            this._denominator = UnsignedBigInteger.One;
        }
        public BigDecimal(sbyte value)
        {
            this._numerator = value;
            this._denominator = UnsignedBigInteger.One;
        }
        public BigDecimal(byte value)
        {
            this._numerator = value;
            this._denominator = UnsignedBigInteger.One;
        }
        public BigDecimal(short value)
        {
            this._numerator = value;
            this._denominator = UnsignedBigInteger.One;
        }
        public BigDecimal(ushort value)
        {
            this._numerator = value;
            this._denominator = UnsignedBigInteger.One;
        }
        public BigDecimal(int value)
        {
            this._numerator = value;
            this._denominator = UnsignedBigInteger.One;
        }
        public BigDecimal(uint value)
        {
            this._numerator = value;
            this._denominator = UnsignedBigInteger.One;
        }
        public BigDecimal(long value)
        {
            this._numerator = value;
            this._denominator = UnsignedBigInteger.One;
        }
        public BigDecimal(ulong value)
        {
            this._numerator = value;
            this._denominator = UnsignedBigInteger.One;
        }
        public BigDecimal(BigInteger value)
        {
            this._numerator = value.Copy();
            this._denominator = UnsignedBigInteger.One;
        }
        public BigDecimal(UnsignedBigInteger value)
        {
            this._numerator = value.Copy();
            this._denominator = UnsignedBigInteger.One;
        }
        public BigDecimal(float value)
        {
            uint bits = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
            if (bits == 0xFFC00000)
            {
                this._numerator = BigInteger.Zero;
                this._denominator = UnsignedBigInteger.Zero;
            }
            else if (bits == 0xFF800000)
            {
                this._numerator = BigInteger.MinusOne;
                this._denominator = UnsignedBigInteger.Zero;
            }
            else if (bits == 0x7F800000)
            {
                this._numerator = BigInteger.One;
                this._denominator = UnsignedBigInteger.Zero;
            }
            else
            {
                uint exponent = 0x000000FF & (bits >> 23);
                uint fraction = (0x00FFFFFF & bits);
                this._numerator = (0x00800000 + fraction);
                if (exponent < 150)
                {
                    this._denominator = UnsignedBigInteger.TwoPower(150 - exponent);
                }
                else
                {
                    this._numerator.LeftShift((int)(exponent - 150));
                    this._denominator = 1;
                }
                if ((bits >> 31) == 1)
                    this._numerator = -this._numerator;
            }
        }
        public BigDecimal(double value)
        {
            ulong bits = BitConverter.ToUInt64(BitConverter.GetBytes(value), 0);
            uint exponent = ((uint)(bits >> 52)) & 0x000007FF;
            ulong fraction = bits & 0x001FFFFFFFFFFFFF;
            this._numerator = 0x0010000000000000 + fraction;
            if (exponent < 1075)
            {
                this._denominator = UnsignedBigInteger.TwoPower(1075 - exponent);
            }
            else
            {
                this._numerator.LeftShift((int)(exponent - 1075));
                this._denominator = 1;
            }
            if ((bits >> 63) == 1)
                this._numerator.SetSign(-1);
        }
        public BigDecimal(BigInteger numerator, UnsignedBigInteger denominator)
        {
            this._numerator = numerator;
            this._denominator = denominator;
        }
        public BigDecimal(BigInteger numerator, BigInteger denominator)
        {
            this._numerator = numerator.Copy();
            if (denominator.Sign == -1)
            {
                this._denominator = (UnsignedBigInteger)(-denominator);
                this._numerator.Negate();
            }
            else
                this._denominator = (UnsignedBigInteger)(denominator);
        }

        public static BigDecimal Abs(BigDecimal value)
        {
            BigDecimal f = value.Copy();
            if (f._numerator.Sign == -1)
            {
                f._numerator.Negate();
            }
            return f;
        }
        public static BigDecimal Add(BigDecimal left, BigDecimal right)
        {
            UnsignedBigInteger den = left._denominator * right._denominator;
            BigInteger num = left._numerator * right._denominator + right._numerator * left._denominator;
            BigDecimal value = new BigDecimal() { _denominator = den, _numerator = num };
            //value.Simplify(left.Sensitivity + right.Sensitivity);
            return value;
        }
        public static BigInteger Ceiling(BigDecimal value)
        {
            if (value._numerator.Sign == -1)
                return value.PartOfInteger - 1;
            else
                return value.PartOfInteger;
        }
        public static BigDecimal Divide(BigDecimal left, BigDecimal right)
        {
            BigInteger num = left._numerator * right._denominator;
            BigInteger den = left._denominator * right._numerator;
            if (den.Sign == -1)
            {
                num.Negate();
                den.Negate();
            }

            BigDecimal value = new BigDecimal { _numerator = num, _denominator = (UnsignedBigInteger)den };
            //value.Simplify(100);
            return value;
        }
        public static BigDecimal Euler(Epsilon epsilon)
        {
            if (epsilon.DecimalSensitive < 3001)
            {
                BigDecimal E = BigDecimal.E;
                E.Simplify(epsilon);
                return E;
            }

            UnsignedBigInteger den = 1;
            BigDecimal e = BigDecimal.One;
            uint i = 1;
            while (true)
            {
                den *= i;
                e += BigDecimal.Reciprocal(den);
                if (den.BitsLength > epsilon.BinarySensitive)
                    break;
                i++;
            }
            return e;
        }
        public static BigDecimal Exp(BigDecimal value, Epsilon epsilon)
        {
            if (value.IsZero)
                return 0;

            bool isNegate = value.Numerator.Sign == -1;
            if (isNegate)
                value = BigDecimal.Negate(value);

            BigDecimal intPow = 1;

            if (value.IsOne)
            {
                BigDecimal euler = BigDecimal.E;
                euler.Simplify(epsilon);
                return euler;
            }
            if (value > 1)
            {
                UnsignedBigInteger integer = (UnsignedBigInteger)value.PartOfInteger;
                value -= integer;
                BigDecimal euler = BigDecimal.E;
                euler.Simplify(epsilon);
                if (integer.IsOne)
                    intPow = euler;
                else
                    intPow = BigDecimal.Pow(euler, integer);
                epsilon *= new Epsilon((int)integer.BitsLength, 2);
            }
            if (value.IsZero)
            {
                if (isNegate)
                    intPow.Reciprocal();
                intPow.Simplify(epsilon);
                return intPow;
            }
            else
            {
                BigDecimal cVal = value.Copy();
                UnsignedBigInteger den = 1;
                BigDecimal e = BigDecimal.One;
                uint i = 1;
                while (true)
                {
                    value = BigDecimal.Pow(cVal, i);
                    den *= i;
                    BigDecimal term = value / den;
                    e += term;
                    if (den.BitsLength > epsilon.BinarySensitive)
                        break;
                    i++;
                }
                e *= intPow;
                if (isNegate)
                    e.Reciprocal();
                e.Simplify(epsilon);
                return e;
            }
        }
        public static BigDecimal GoldenRatio(Epsilon epsilon)
        {
            epsilon *= new Epsilon(1, 2);
            return (1 + BigDecimal.SquareRoot("5", epsilon)) / 2;
        }
        public static BigInteger Floor(BigDecimal value)
        {
            BigInteger integer = value.PartOfInteger;
            value -= integer;
            if (value.IsZero || value._numerator.Sign == -1)
                return integer;
            else
                return integer + 1;
        }
        public static BigDecimal Lb(BigDecimal value, Epsilon epsilon)
        {
            BigDecimal mNext = value.Copy();
            BigDecimal half = 1;
            half.RightShift(1);
            int prefix = 0;
            if (value < half)
            {
                prefix = (int)value._denominator.BitsLength - 1;
                mNext.LeftShift(prefix);
            }

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
            return (lbInt - prefix) + dec;
        }
        public static BigDecimal Ln(BigDecimal value, Epsilon epsilon)
        {
            BigDecimal lb = BigDecimal.Lb(value, epsilon);
            BigDecimal ln2 = BigDecimal.Ln2;
            ln2.Simplify(epsilon * new Epsilon(3));
            lb *= ln2;
            lb.Simplify(epsilon);
            return lb;
        }
        public static BigDecimal Log(BigDecimal value, BigDecimal baseValue, Epsilon epsilon)
        {
            BigDecimal lbBase = BigDecimal.Lb(baseValue, epsilon);
            BigDecimal lb = BigDecimal.Lb(value, epsilon);
            lb /= lbBase;
            lb.Simplify(epsilon);
            return lb;
        }
        public static BigDecimal Multiply(BigDecimal left, BigDecimal right)
        {
            BigInteger num = left._numerator * right._numerator;
            UnsignedBigInteger den = left._denominator * right._denominator;

            BigDecimal value = new BigDecimal { _numerator = num, _denominator = den };
            //value.Simplify(100);
            return value;
        }
        public static BigDecimal Negate(BigDecimal value)
        {
            BigDecimal v = value.Copy();
            v.Negate();
            return v;
        }
        public static BigDecimal Pi(Epsilon epsilon)
        {
            if (epsilon.DecimalSensitive < 3001)
            {
                BigDecimal pi = BigDecimal.PI;
                pi.Simplify(epsilon);
                return pi;
            }

            epsilon *= new Numerics.Epsilon((int)Math.Ceiling(Math.Log(epsilon.BinarySensitive, 3.1416)));

            BigDecimal af = 1, al = 1, bf = BigDecimal.SquareRoot(2, epsilon) / 2, bl = bf, t = "0.25", p = 1;
            while (true)
            {
                af = al;
                bf = bl;

                al = (af + bf) / 2;
                if (BigDecimal.Abs((af - bl)) < epsilon)
                    break;
                bl = BigDecimal.SquareRoot(af * bf, epsilon);
                t = t - p * BigDecimal.Square(af - al);
                p.LeftShift(1);

                al.Simplify(epsilon);
                bl.Simplify(epsilon);
                t.Simplify(epsilon);
                p.Simplify(epsilon);
            }
            return BigDecimal.Square(af + bf) / (4 * t);
        }
        public static BigDecimal Pow(BigDecimal value, uint exponent)
        {
            if (exponent == 0)
                return BigDecimal.One;
            if (exponent == 1)
                return value.Copy();
            BigInteger num = BigInteger.Pow(value._numerator, exponent);
            UnsignedBigInteger den = UnsignedBigInteger.Pow(value._denominator, exponent);
            BigDecimal result = new BigDecimal() { _denominator = den, _numerator = num };
            return result;
        }
        public static BigDecimal Pow(BigDecimal value, UnsignedBigInteger exponent)
        {
            if (exponent == 0)
                return BigDecimal.One;
            if (exponent == 1)
                return value.Copy();
            BigInteger num = BigInteger.Pow(value._numerator, exponent);
            UnsignedBigInteger den = UnsignedBigInteger.Pow(value._denominator, exponent);
            BigDecimal result = new BigDecimal() { _denominator = den, _numerator = num };
            return result;
        }
        public static BigDecimal Pow(BigDecimal value, BigDecimal exponent, Epsilon epsilon)
        {
            BigDecimal ln = BigDecimal.Ln(value, epsilon);
            return BigDecimal.Exp(exponent * ln, epsilon);
        }
        public static BigDecimal Reciprocal(BigDecimal value)
        {
            BigInteger num = value._denominator.Copy();
            BigInteger den = value._numerator.Copy();
            if (value._numerator.Sign == -1)
            {
                num.Negate();
                den.Negate();
            }
            return new BigDecimal() { _numerator = num, _denominator = (UnsignedBigInteger)den };
        }
        public static BigDecimal Remainder(BigDecimal left, BigDecimal right)
        {
            BigInteger num = (left._numerator * right._denominator) % (right._numerator * left._denominator);
            UnsignedBigInteger den = left._denominator * right._denominator;
            BigDecimal value = new BigDecimal() { _numerator = num, _denominator = den };
            //value.Simplify(100);
            return value;
        }
        public static BigInteger Round(BigDecimal value)
        {
            bool isNeg = value._numerator.Sign == -1;
            BigInteger integer = value.PartOfInteger;
            value -= integer;
            if (isNeg)
            {
                if (value > BigDecimal.Parse("-0.5"))
                    return integer;
                else
                    return integer - 1;
            }
            else
            {
                if (value < BigDecimal.Parse("0.5"))
                    return integer;
                else
                    return integer + 1;
            }
        }
        public static BigDecimal Subtract(BigDecimal left, BigDecimal right)
        {
            UnsignedBigInteger den = left._denominator * right._denominator;
            BigInteger num = left._numerator * right._denominator - right._numerator * left._denominator;
            BigDecimal value = new BigDecimal() { _denominator = den, _numerator = num };
            //value.Simplify(100);
            return value;
        }
        public static BigDecimal Square(BigDecimal value)
        {
            value = BigDecimal.Multiply(value, value);
            //value.Simplify(epsilon);
            return value;
        }
        public static BigDecimal SquareRoot(BigDecimal value, Epsilon epsilon)
        {
            UnsignedBigInteger numAbs = (UnsignedBigInteger)BigInteger.Abs(value._numerator);
            UnsignedBigInteger denAbs = (UnsignedBigInteger)BigInteger.Abs(value._denominator);
            int shift = (int)(epsilon.BinarySensitive * 2);
            numAbs.LeftShift(shift);
            denAbs.LeftShift(shift);
            BigInteger num = UnsignedBigInteger.SquareRoot(numAbs);
            UnsignedBigInteger den = UnsignedBigInteger.SquareRoot(denAbs);
            return new BigDecimal() { _numerator = num, _denominator = den };
        }

        public static BigDecimal Parse(string value)
        {
            try
            {
                value = value.ToUpper();
                if (value.Contains("E"))
                {
                    string[] split = value.Split('E');
                    bool exponentIsNeg = split[1][0] == '-';
                    string intStr = string.Join("", split[0].Split('.'));
                    uint exponent = uint.Parse(split[1].TrimStart('-', '+'));
                    int tenPow = split[0].Length - split[0].IndexOf('.') - 1;

                    BigInteger num = BigInteger.Parse(intStr);
                    if (exponentIsNeg)
                    {
                        UnsignedBigInteger den = UnsignedBigInteger.Pow(10, (uint)tenPow + exponent);
                        return new BigDecimal() { _numerator = num, _denominator = den };
                    }
                    else
                    {
                        num *= BigInteger.Pow(10, exponent);
                        UnsignedBigInteger den = UnsignedBigInteger.Pow(10, (uint)tenPow);
                        return new BigDecimal() { _denominator = den, _numerator = num };
                    }
                }
                else
                {
                    string intStr = string.Join("", value.Split('.'));
                    int indexOf = value.IndexOf('.');
                    int tenPow = indexOf == -1 ? 0 : value.Length - indexOf - 1;

                    BigInteger num = BigInteger.Parse(intStr);
                    UnsignedBigInteger den = UnsignedBigInteger.Pow(10, (uint)tenPow);
                    return new BigDecimal() { _denominator = den, _numerator = num };
                }
            }
            catch (Exception)
            {
                throw new FormatException();
            }
        }
        public static bool TryParse(string value, out BigDecimal result)
        {
            try
            {
                value = value.ToUpper();
                if (value.Contains("E"))
                {
                    string[] split = value.Split('E');
                    bool exponentIsNeg = split[1][0] == '-';
                    string intStr = string.Join("", split[0].Split(','));
                    uint exponent = uint.Parse(split[1].TrimStart('-', '+'));
                    int tenPow = split[0].Length - split[0].IndexOf(',') - 1;

                    BigInteger num = BigInteger.Parse(intStr);
                    if (exponentIsNeg)
                    {
                        UnsignedBigInteger den = UnsignedBigInteger.Pow(10, (uint)tenPow + exponent);
                        result = new BigDecimal() { _numerator = num, _denominator = den };
                    }
                    else
                    {
                        num *= BigInteger.Pow(10, exponent);
                        UnsignedBigInteger den = UnsignedBigInteger.Pow(10, (uint)tenPow);
                        result = new BigDecimal() { _denominator = den, _numerator = num };
                    }
                }
                else
                {
                    string intStr = string.Join("", value.Split(','));
                    int tenPow = value.Length - value.IndexOf(',') - 1;

                    BigInteger num = BigInteger.Parse(intStr);
                    UnsignedBigInteger den = UnsignedBigInteger.Pow(10, (uint)tenPow);
                    result = new BigDecimal() { _denominator = den, _numerator = num };
                }
                return true;
            }
            catch (Exception)
            {
                result = new BigDecimal();
                return false;
            }
        }

        public int CompareTo(BigDecimal other)
        {
            return (this._numerator * other._denominator).CompareTo(this._denominator * other._numerator);
        }
        public int CompareTo(Epsilon other)
        {
            BigInteger num = this._numerator << other.BinarySensitive;
            return num.CompareTo(this._denominator);
        }
        public BigDecimal Copy()
        {
            return new BigDecimal { _numerator = this._numerator.Copy(), _denominator = this._denominator.Copy() };
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return this._numerator.GetHashCode() + this._denominator.GetHashCode();
        }
        public void LeftShift(int value)
        {
            if (value < 0)
                this._denominator.LeftShift(-value);
            else
                this._numerator.LeftShift(value);
        }
        public void Negate()
        {
            this._numerator.Negate();
        }
        public void Reciprocal()
        {
            BigInteger num = this._denominator;
            if (this._numerator.Sign == -1)
            {
                num.Negate();
                this._numerator.Negate();
            }
            this._denominator = (UnsignedBigInteger)this._numerator;
            this._numerator = num;
        }
        public void RightShift(int value)
        {
            if (value < 0)
                this._numerator.LeftShift(-value);
            else
                this._denominator.LeftShift(value);
        }
        public void Simplify(Epsilon epsilon)
        {
            int significantBit = (int)epsilon.BinarySensitive;
            int denBits = (int)this._denominator.BitsLength;

            if (denBits > significantBit)
            {
                int shift = denBits - significantBit - 1;
                this._denominator.RightShift(shift);
                this._numerator.RightShift(shift);
            }
        }
        public override string ToString()
        {
            return this.ToString(this.Sensitivity.DecimalSensitive);
        }
        public string ToString(int decimalDigit)
        {
            if (this._denominator.IsOne)
                return this._numerator.ToString();
            if (this._denominator.IsZero)
            {
                if (this._numerator.Sign == 1)
                    return "Infinity";
                else if (this._numerator.Sign == -1)
                    return "-Infinity";
                else return "NaN";
            }

            BigInteger integer = this._numerator / this._denominator;
            string s = "";
            if (integer.IsZero)
            {
                s = this._numerator.Sign == -1 ? "-0" : "0";
            }
            else
            {
                s = integer.ToString();
            }

            UnsignedBigInteger dec = (UnsignedBigInteger)(BigInteger.Abs(this._numerator) % this._denominator);
            dec = dec * UnsignedBigInteger.Pow(10, (uint)decimalDigit);
            dec = dec / this._denominator;

            if (dec.IsZero)
                return s;

            s += ".";

            string decimalString = dec.ToString();
            int zeroLength = decimalDigit - decimalString.Length;
            for (int i = 0; i < zeroLength; i++)
                s += '0';
            s += decimalString;

            return s;
        }

        public static BigDecimal operator +(BigDecimal left, BigDecimal right)
        {
            return BigDecimal.Add(left, right);
        }
        public static BigDecimal operator -(BigDecimal left, BigDecimal right)
        {
            return BigDecimal.Subtract(left, right);
        }
        public static BigDecimal operator *(BigDecimal left, BigDecimal right)
        {
            return BigDecimal.Multiply(left, right);
        }
        public static BigDecimal operator /(BigDecimal left, BigDecimal right)
        {
            return BigDecimal.Divide(left, right);
        }
        public static BigDecimal operator %(BigDecimal left, BigDecimal right)
        {
            return BigDecimal.Remainder(left, right);
        }

        public static implicit operator BigDecimal(byte value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(ushort value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(uint value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(ulong value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(sbyte value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(short value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(int value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(long value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(UnsignedBigInteger value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(BigInteger value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(string value)
        {
            return BigDecimal.Parse(value);
        }
        public static implicit operator BigDecimal(float value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(double value)
        {
            return new BigDecimal(value);
        }

        public static bool operator ==(BigDecimal left, sbyte right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigDecimal left, sbyte right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigDecimal left, sbyte right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigDecimal left, sbyte right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigDecimal left, sbyte right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigDecimal left, sbyte right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigDecimal left, byte right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigDecimal left, byte right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigDecimal left, byte right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigDecimal left, byte right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigDecimal left, byte right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigDecimal left, byte right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigDecimal left, short right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigDecimal left, short right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigDecimal left, short right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigDecimal left, short right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigDecimal left, short right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigDecimal left, short right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigDecimal left, ushort right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigDecimal left, ushort right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigDecimal left, ushort right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigDecimal left, ushort right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigDecimal left, ushort right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigDecimal left, ushort right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigDecimal left, int right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigDecimal left, int right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigDecimal left, int right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigDecimal left, int right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigDecimal left, int right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigDecimal left, int right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigDecimal left, uint right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigDecimal left, uint right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigDecimal left, uint right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigDecimal left, uint right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigDecimal left, uint right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigDecimal left, uint right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigDecimal left, long right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigDecimal left, long right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigDecimal left, long right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigDecimal left, long right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigDecimal left, long right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigDecimal left, long right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigDecimal left, ulong right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigDecimal left, ulong right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigDecimal left, ulong right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigDecimal left, ulong right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigDecimal left, ulong right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigDecimal left, ulong right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigDecimal left, BigInteger right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigDecimal left, BigInteger right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigDecimal left, BigInteger right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigDecimal left, BigInteger right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigDecimal left, BigInteger right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigDecimal left, BigInteger right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigDecimal left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigDecimal left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigDecimal left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigDecimal left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigDecimal left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigDecimal left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigDecimal left, float right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigDecimal left, float right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigDecimal left, float right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigDecimal left, float right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigDecimal left, float right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigDecimal left, float right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigDecimal left, double right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigDecimal left, double right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigDecimal left, double right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigDecimal left, double right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigDecimal left, double right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigDecimal left, double right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigDecimal left, BigDecimal right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigDecimal left, BigDecimal right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigDecimal left, BigDecimal right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigDecimal left, BigDecimal right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigDecimal left, BigDecimal right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigDecimal left, BigDecimal right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigDecimal value, Epsilon epsilon)
        {
            return value.CompareTo(epsilon) == 0;
        }
        public static bool operator !=(BigDecimal value, Epsilon epsilon)
        {
            return value.CompareTo(epsilon) != 0;
        }
        public static bool operator <(BigDecimal value, Epsilon epsilon)
        {
            return value.CompareTo(epsilon) == -1;
        }
        public static bool operator >(BigDecimal value, Epsilon epsilon)
        {
            return value.CompareTo(epsilon) == 1;
        }
        public static bool operator <=(BigDecimal value, Epsilon epsilon)
        {
            return value.CompareTo(epsilon) < 1;
        }
        public static bool operator >=(BigDecimal value, Epsilon epsilon)
        {
            return value.CompareTo(epsilon) > -1;
        }
    }
}
