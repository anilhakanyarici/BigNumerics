using System;


namespace NetworkIO.Numerics
{
    public class BigInteger : IComparable<BigInteger>, IEquatable<BigInteger>
    {
        private uint[] _digits;
        private int _digitLength;
        private int _sign;

        internal uint[] digitsRef { get { return this._digits; } }

        public static BigInteger Zero { get { return new BigInteger(); } }
        public static BigInteger One { get { return new BigInteger { _digitLength = 1, _digits = new uint[] { 1 }, _sign = 1 }; } }
        public static BigInteger MinusOne { get { return new BigInteger { _digits = new uint[1] { 1 }, _sign = -1, _digitLength = 1 }; } }

        public int BytesLength { get { return BigIntInternal.SignedBytesLength(this._digits, this._digitLength, this._sign); } }
        public long BitsLength { get { return BigIntInternal.SignedBitsLength(this._digits, this._digitLength, this._sign); } }
        public int DigitsLength { get { return this._digitLength; } }
        public int Sign { get { return this._sign; } }
        public bool IsEven { get { return (this._digits[0] & 1) == 0; } }
        public bool IsOdd { get { return (this._digits[0] & 1) == 1; } }
        public bool IsZero { get { return this._sign == 0; } }
        public bool IsOne { get { return this._digitLength == 1 && this._digits[0] == 1 && this._sign == 1; } }
        public bool IsPowerOfTwo
        {
            get
            {
                if (this._sign != 0)
                    return false;

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
        public bool IsMinusOne { get { return this._digitLength == 1 && this._digits[0] == 1 && this._sign == -1; } }
        public bool IsNegative { get { return this._sign == -1; } }
        public uint FirstDigit { get { return this._digits[0]; } }

        public BigInteger()
        {
            this._digits = new uint[1];
            this._digitLength = 1;
            this._sign = 0;
        }
        public BigInteger(sbyte value)
        {
            if (value == 0)
            {
                this._sign = 0;
                this._digitLength = 1;
                this._digits = new uint[1];
            }
            else if (value < 0)
            {
                this._sign = -1;
                this._digitLength = 1;
                this._digits = new uint[1] { (byte)(-value) };
            }
            else
            {
                this._sign = 1;
                this._digitLength = 1;
                this._digits = new uint[1] { (byte)value };
            }
        }
        public BigInteger(byte value)
        {
            if (value == 0)
            {
                this._sign = 0;
                this._digitLength = 1;
                this._digits = new uint[1];
            }
            else
            {
                this._sign = 1;
                this._digitLength = 1;
                this._digits = new uint[1] { value };
            }
        }
        public BigInteger(short value)
        {
            if (value == 0)
            {
                this._sign = 0;
                this._digitLength = 1;
                this._digits = new uint[1];
            }
            else if (value < 0)
            {
                this._sign = -1;
                this._digitLength = 1;
                this._digits = new uint[1] { (ushort)(-value) };
            }
            else
            {
                this._sign = 1;
                this._digitLength = 1;
                this._digits = new uint[1] { (ushort)value };
            }
        }
        public BigInteger(ushort value)
        {
            if (value == 0)
            {
                this._sign = 0;
                this._digitLength = 1;
                this._digits = new uint[1];
            }
            else
            {
                this._sign = 1;
                this._digitLength = 1;
                this._digits = new uint[1] { value };
            }
        }
        public BigInteger(int value)
        {
            if (value == 0)
            {
                this._sign = 0;
                this._digitLength = 1;
                this._digits = new uint[1];
            }
            else if (value < 0)
            {
                this._sign = -1;
                this._digitLength = 1;
                this._digits = new uint[1] { (uint)(-value) };
            }
            else
            {
                this._sign = 1;
                this._digitLength = 1;
                this._digits = new uint[1] { (uint)value };
            }
        }
        public BigInteger(uint value)
        {
            if (value == 0)
            {
                this._sign = 0;
                this._digitLength = 1;
                this._digits = new uint[1];
            }
            else
            {
                this._sign = 1;
                this._digitLength = 1;
                this._digits = new uint[1] { value };
            }
        }
        public BigInteger(long value)
        {
            if (value == 0)
            {
                this._sign = 0;
                this._digitLength = 1;
                this._digits = new uint[1];
            }
            else if (value < 0)
            {
                this._sign = -1;
                ulong uVal = (ulong)-value;
                this._digits = new uint[2] { (uint)uVal, (uint)(uVal >> 32) };
                if (this._digits[1] == 0)
                    this._digitLength = 1;
                else
                    this._digitLength = 2;
            }
            else
            {
                this._sign = 1;
                ulong uVal = (ulong)value;
                this._digits = new uint[2] { (uint)uVal, (uint)(uVal >> 32) };
                if (this._digits[1] == 0)
                    this._digitLength = 1;
                else
                    this._digitLength = 2;
            }
        }
        public BigInteger(ulong value)
        {
            if (value == 0)
            {
                this._sign = 0;
                this._digitLength = 1;
                this._digits = new uint[1];
            }
            else
            {
                this._sign = 1;
                this._digits = new uint[2] { (uint)value, (uint)(value >> 32) };
                if (this._digits[1] == 0)
                    this._digitLength = 1;
                else
                    this._digitLength = 2;
            }
        }
        public BigInteger(UnsignedBigInteger value)
        {
            if (value.IsZero)
            {
                this._digitLength = 1;
                this._sign = 0;
                this._digits = new uint[1];
            }
            else
            {
                this._digits = value.GetDigits();
                this._digitLength = value.DigitsLength;
                this._sign = 1;
            }
        }
        public BigInteger(byte[] bytes)
        {
            this._digits = BigIntInternal.FromSignedBytes(bytes, true, out this._digitLength, out this._sign);
        }
        public BigInteger(byte[] bytes, bool bigEndian)
        {
            this._digits = BigIntInternal.FromSignedBytes(bytes, bigEndian, out this._digitLength, out this._sign);
        }
        public BigInteger(byte[] bytes, bool bigEndian, bool isSignedBytes)
        {
            if (isSignedBytes)
                this._digits = BigIntInternal.FromSignedBytes(bytes, bigEndian, out this._digitLength, out this._sign);
            else
            {
                this._digits = BigIntInternal.FromUnsignedBytes(bytes, bigEndian, out this._digitLength);
                this._sign = this._digitLength == 1 && this._digits[0] == 0 ? 0 : 1;
            }
        }

        public BigInteger(uint[] digits, int sign)
        {
            int digitLength = digits.Length;

            while (digitLength > 1 && digits[digitLength - 1] == 0)
                digitLength--;

            if (digitLength == 0)
            {
                this._sign = 0;
                this._digits = new uint[1];
                this._digitLength = 1;
            }
            else
            {
                this._digits = new uint[digitLength];
                this._sign = sign;
                this._digitLength = digitLength;
                Array.Copy(digits, this._digits, digitLength);
            }
        }
        public BigInteger(uint[] digits, int digitLength, int sign)
        {
            if (digitLength > digits.Length)
                digitLength = digits.Length;
            while (digitLength > 1 && digits[digitLength - 1] == 0)
                digitLength--;

            if (digitLength == 0)
            {
                this._sign = 0;
                this._digits = new uint[1];
                this._digitLength = 1;
            }
            else
            {
                this._digits = new uint[digitLength];
                this._sign = sign;
                this._digitLength = digitLength;
                Array.Copy(digits, this._digits, digitLength);
            }
        }

        public static BigInteger Abs(BigInteger value)
        {
            value = value.Copy();
            if (value._sign != 0)
                value._sign = 1;
            return value;
        }
        public static BigInteger Add(BigInteger left, BigInteger right)
        {
            if (left._sign == 0)
                return right.Copy();
            if (right._sign == 0)
                return left.Copy();

            if (left._sign == right._sign)
            {
                int resultLength;
                uint[] sum;
                if (right._digitLength == 1)
                {
                    sum = left._digits.Clone() as uint[];
                    resultLength = left._digitLength;
                    BigIntInternal.AddSingle(ref sum, ref resultLength, right._digits[0]);
                }
                else
                    sum = BigIntInternal.Add(left._digits, left._digitLength, right._digits, right._digitLength, out resultLength);

                return new BigInteger() { _sign = left._sign, _digits = sum, _digitLength = resultLength };
            }
            else
            {
                int c = BigIntInternal.Compare(left._digits, left._digitLength, right._digits, right._digitLength);
                if (c == 1)
                {
                    if (left._sign == 1)
                    {
                        int resultLength;
                        uint[] sum;
                        if (right._digitLength == 1)
                        {
                            sum = left._digits.Clone() as uint[];
                            resultLength = left._digitLength;
                            BigIntInternal.SubSingle(ref sum, ref resultLength, right._digits[0]);
                        }
                        else
                            sum = BigIntInternal.Sub(left._digits, left._digitLength, right._digits, right._digitLength, out resultLength);
                        return new BigInteger() { _digitLength = resultLength, _digits = sum, _sign = 1 };
                    }
                    else
                    {
                        int resultLength;
                        uint[] sum;
                        if (right._digitLength == 1)
                        {
                            sum = left._digits.Clone() as uint[];
                            resultLength = left._digitLength;
                            BigIntInternal.SubSingle(ref sum, ref resultLength, right._digits[0]);
                        }
                        else
                            sum = BigIntInternal.Sub(left._digits, left._digitLength, right._digits, right._digitLength, out resultLength);
                        return new BigInteger() { _digitLength = resultLength, _digits = sum, _sign = -1 };
                    }
                }
                else if (c == -1)
                {
                    if (left._sign == 1)
                    {
                        int resultLength;
                        uint[] sum;
                        if (left._digitLength == 1)
                        {
                            sum = right._digits.Clone() as uint[];
                            resultLength = right._digitLength;
                            BigIntInternal.SubSingle(ref sum, ref resultLength, left._digits[0]);
                        }
                        else
                            sum = BigIntInternal.Sub(right._digits, right._digitLength, left._digits, left._digitLength, out resultLength);
                        return new BigInteger() { _digitLength = resultLength, _digits = sum, _sign = -1 };
                    }
                    else
                    {
                        int resultLength;
                        uint[] sum;
                        if (left._digitLength == 1)
                        {
                            sum = right._digits.Clone() as uint[];
                            resultLength = right._digitLength;
                            BigIntInternal.SubSingle(ref sum, ref resultLength, left._digits[0]);
                        }
                        else
                            sum = BigIntInternal.Sub(right._digits, right._digitLength, left._digits, left._digitLength, out resultLength);
                        return new BigInteger() { _digitLength = resultLength, _digits = sum, _sign = 1 };
                    }
                }
                else
                {
                    return BigInteger.Zero;
                }
            }
        }
        public static BigInteger Cube(BigInteger value)
        {
            return value * value * value;
        }
        public static BigInteger Divide(BigInteger left, BigInteger right)
        {
            if (left._sign == 0)
                return BigInteger.Zero;
            if (right._sign == 0)
                throw new ArithmeticException("0'a bölme yapılamaz.");
            int c = BigIntInternal.Compare(left._digits, left._digitLength, right._digits, right._digitLength);
            if (c == -1)
            {
                return BigInteger.Zero;
            }
            else if (c == 0)
            {
                if (left._sign == right._sign)
                    return new BigInteger(1);
                else
                    return new BigInteger(-1);
            }
            else
            {
                if (right._digitLength == 1)
                {
                    int resultLength;
                    uint rem;
                    uint[] div = BigIntInternal.DivRemSingle(left._digits, left._digitLength, right._digits[0], out resultLength, out rem);
                    if (left._sign == right._sign)
                        return new BigInteger() { _digits = div, _digitLength = resultLength, _sign = 1 };
                    else
                        return new BigInteger() { _digits = div, _digitLength = resultLength, _sign = -1 };
                }
                else
                {
                    uint[] num = left._digits.Clone() as uint[];
                    int resultLength;
                    uint[] div = BigIntInternal.Div(num, left._digitLength, right._digits, right._digitLength, out resultLength);
                    if (left._sign == right._sign)
                        return new BigInteger() { _sign = 1, _digitLength = resultLength, _digits = div };
                    else
                        return new BigInteger() { _sign = -1, _digitLength = resultLength, _digits = div };
                }
            }
        }
        public static BigInteger DivRem(BigInteger left, BigInteger right, out BigInteger remainder)
        {
            if (left._sign == 0)
            {
                remainder = BigInteger.Zero;
                return BigInteger.Zero;
            }
            if (right._sign == 0)
                throw new ArithmeticException("0'a bölme yapılamaz.");

            int c = BigIntInternal.Compare(left._digits, left._digitLength, right._digits, right._digitLength);
            if (c == -1)
            {
                remainder = left.Copy();
                return BigInteger.Zero;
            }
            else if (c == 0)
            {
                remainder = BigInteger.Zero;
                if (left._sign == right._sign)
                    return new BigInteger(1);
                else
                    return new BigInteger(-1);
            }
            else
            {
                if (right._digitLength == 1)
                {
                    int resultLength;
                    uint rem;
                    uint[] div = BigIntInternal.DivRemSingle(left._digits, left._digitLength, right._digits[0], out resultLength, out rem);
                    remainder = new BigInteger(rem);
                    if (rem != 0)
                        remainder._sign = left._sign;
                    if (left._sign == right._sign)
                        return new BigInteger() { _digits = div, _digitLength = resultLength, _sign = 1 };
                    else
                        return new BigInteger() { _digits = div, _digitLength = resultLength, _sign = -1 };
                }
                else
                {
                    uint[] num = left._digits.Clone() as uint[];
                    int resultLength, remainderLength;
                    uint[] div = BigIntInternal.DivRem(ref num, left._digitLength, right._digits, right._digitLength, out resultLength, out remainderLength);
                    remainder = new BigInteger() { _digitLength = remainderLength, _digits = num, _sign = left._sign };
                    if (left._sign == right._sign)
                        return new BigInteger() { _sign = 1, _digitLength = resultLength, _digits = div };
                    else
                        return new BigInteger() { _sign = -1, _digitLength = resultLength, _digits = div };
                }
            }
        }
        public static BigInteger LeftShift(BigInteger value, int shift)
        {
            if (value.IsZero)
                return BigInteger.Zero;
            if (shift < 0)
                return BigInteger.RightShift(value, -shift);

            uint[] digs = value._digits.Clone() as uint[];
            int len = value._digitLength;
            BigIntInternal.ShiftLeft(ref digs, ref len, shift);
            return new BigInteger() { _digitLength = len, _digits = digs, _sign = value._sign };
        }
        public static BigInteger ModInverse(BigInteger K, BigInteger modulus)
        {
            if (K.IsOne)
                return BigInteger.One;

            BigInteger x1 = BigInteger.Zero, x2 = modulus, y1 = BigInteger.One, y2 = K;

            BigInteger t1, t2, q = BigInteger.DivRem(x2, y2, out t2);
            q.Negate();
            t1 = q;

            while (!y2.IsOne)
            {
                if (t2._sign == 0)
                    return BigInteger.Zero;

                x1 = y1; x2 = y2;
                y1 = t1; y2 = t2;
                q = BigInteger.DivRem(x2, y2, out t2);

                t1 = BigInteger.Subtract(x1, BigInteger.Multiply(q, y1));
            }
            if (y1._sign == -1)
                return (BigInteger.Add(y1, modulus));
            else
                return y1;
        }
        public static BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger modulus)
        {
            BigInteger result = BigInteger.One;
            long bitLength = exponent.BitsLength;
            for (long i = 0; i < bitLength; i++)
            {
                int bit = exponent.GetBit(i);
                if (value._digitLength > modulus._digitLength)
                    value = BigInteger.Remainder(value, modulus);
                if (bit == 1)
                {
                    result = BigInteger.Multiply(result, value);
                    if (result.BytesLength > modulus.BytesLength)
                        result = BigInteger.Remainder(result, modulus);
                }
                value = BigInteger.Square(value);
            }
            return BigInteger.Remainder(result, modulus);
        }
        public static BigInteger Multiply(BigInteger left, BigInteger right)
        {
            if (left._sign == 0 || right._sign == 0)
                return BigInteger.Zero;

            int resultLength;
            uint[] mul = BigIntInternal.Mul(left._digits, left._digitLength, right._digits, right._digitLength, out resultLength);
            if (left._sign == right._sign)
                return new BigInteger() { _digitLength = resultLength, _sign = 1, _digits = mul };
            else
                return new BigInteger() { _digitLength = resultLength, _sign = -1, _digits = mul };
        }
        public static BigInteger Negate(BigInteger value)
        {
            value = value.Copy();
            if (value._sign == 0)
                return value;

            value._sign = value._sign == 1 ? -1 : 1;
            return value;
        }
        public static BigInteger Pow(BigInteger value, uint exponent)
        {
            if (exponent == 1)
                return value.Copy();
            if (exponent == 0)
                return BigInteger.One;

            BigInteger res = BigInteger.One;
            if ((exponent & 1) == 1)
                res = value;

            while (true)
            {
                exponent >>= 1;
                if (exponent == 0) break;
                value = BigInteger.Square(value);
                if ((exponent & 1) == 1)
                    res = BigInteger.Multiply(value, res);
            }
            return res;
        }
        public static BigInteger Pow(BigInteger value, UnsignedBigInteger exponent)
        {
            if (exponent == 1)
                return value.Copy();
            if (exponent == 0)
                return BigInteger.One;

            BigInteger res = BigInteger.One;

            int bitLength = (int)exponent.BitsLength;

            for (int i = 0; i < bitLength; i++)
            {
                int bit = exponent.GetBit(i);
                if (bit == 1)
                    res = BigInteger.Multiply(value, res);
                if (i + 1 == bitLength)
                    break;
                value = BigInteger.Square(value);
            }
            return res;
        }
        public static BigInteger Remainder(BigInteger left, BigInteger right)
        {
            if (left._sign == 0)
                return BigInteger.Zero;
            if (right._sign == 0)
                throw new ArithmeticException("0'a bölme yapılamaz.");

            int c = BigIntInternal.Compare(left._digits, left._digitLength, right._digits, right._digitLength);
            if (c == -1)
            {
                return left.Copy();
            }
            else if (c == 0)
            {
                return BigInteger.Zero;
            }
            else
            {
                if (right._digitLength == 1)
                {
                    int resultLength;
                    uint rem;
                    BigIntInternal.DivRemSingle(left._digits, left._digitLength, right._digits[0], out resultLength, out rem);
                    if (left._sign == 1)
                        return new BigInteger() { _digits = new uint[1] { rem }, _digitLength = 1, _sign = 1 };
                    else
                        return new BigInteger() { _digits = new uint[1] { rem }, _digitLength = 1, _sign = -1 };
                }
                else
                {
                    uint[] num = left._digits.Clone() as uint[];
                    int remLength = left._digitLength;
                    BigIntInternal.Rem(ref num, ref remLength, right._digits, right._digitLength);
                    if (left._sign == 1)
                        return new BigInteger() { _sign = 1, _digitLength = remLength, _digits = num };
                    else
                        return new BigInteger() { _sign = -1, _digitLength = remLength, _digits = num };
                }
            }
        }
        public static BigInteger RightShift(BigInteger value, int shift)
        {
            if (value.IsZero)
                return BigInteger.Zero;
            if (shift < 0)
                return BigInteger.LeftShift(value, -shift);

            uint[] digs = value._digits.Clone() as uint[];
            int len = value._digitLength;
            BigIntInternal.ShiftRight(ref digs, ref len, shift);
            if (len == 1 && digs[0] == 0)
                return BigInteger.Zero;
            return new BigInteger() { _digitLength = len, _digits = digs, _sign = value._sign };
        }
        public static BigInteger Subtract(BigInteger left, BigInteger right)
        {
            if (left._sign == 0)
            {
                BigInteger val = right.Copy();
                if (val._sign == 0)
                    return val;
                val._sign = val._sign == 1 ? val._sign = -1 : val._sign = 1;
                return val;
            }
            if (right._sign == 0)
                return left.Copy();

            if (left._sign != right._sign)
            {
                int resultLength;
                uint[] dif = BigIntInternal.Add(left._digits, left._digitLength, right._digits, right._digitLength, out resultLength);
                if (left.Sign == 1)
                    return new BigInteger() { _digitLength = resultLength, _digits = dif, _sign = 1 };
                else
                    return new BigInteger() { _digitLength = resultLength, _digits = dif, _sign = -1 };
            }
            else
            {
                int c = BigIntInternal.Compare(left._digits, left._digitLength, right._digits, right._digitLength);
                if (c == 1)
                {
                    int resultLength;
                    uint[] dif = BigIntInternal.Sub(left._digits, left._digitLength, right._digits, right._digitLength, out resultLength);
                    return new BigInteger() { _digitLength = resultLength, _digits = dif, _sign = left._sign };
                }
                else if (c == -1)
                {
                    int resultLength;
                    uint[] dif = BigIntInternal.Sub(right._digits, right._digitLength, left._digits, left._digitLength, out resultLength);
                    return new BigInteger() { _digitLength = resultLength, _digits = dif, _sign = right._sign == 1 ? -1 : 1 };
                }
                else
                {
                    return BigInteger.Zero;
                }
            }
        }
        public static BigInteger Square(BigInteger value)
        {
            return BigInteger.Multiply(value, value);
        }
        public static BigInteger SquareRoot(BigInteger value)
        {
            if (value.Sign == 0)
                return BigInteger.Zero;
            else if (value.Sign == -1)
                throw new ArithmeticException("Square root of negative numbers does not defined.");

            BigInteger xf = BigInteger.One;
            BigInteger xl = BigInteger.One;
            do
            {
                xf = xl;
                xl = xf + value / xf;
                xl.RightShift(1);
            } while (!(xf == xl || xf == xl - 1));

            return xf;
        }
        public static BigInteger TwoPower(long exponent)
        {
            if (exponent > BigIntInternal.maxBitsLength)
                throw new OverflowException("Exponent cannot be longer than " + BigIntInternal.maxBitsLength + ".");
            if (exponent == 0)
                return UnsignedBigInteger.One;

            int digitLength = (int)(exponent / 32) + 1;
            uint[] digits = new uint[digitLength];
            digits[digitLength - 1] = ((uint)1 << (int)(exponent & 31));
            return new BigInteger() { _digitLength = digitLength, _digits = digits, _sign = 1 };
        }

        public static BigInteger Parse(string value)
        {
            try
            {
                int digitLength, sign;
                uint[] digits = BigIntInternal.SignedParse(value, out digitLength, out sign);
                return new BigInteger { _digitLength = digitLength, _digits = digits, _sign = sign };
            }
            catch (Exception)
            {
                throw new FormatException();
            }
        }
        public static BigInteger ParseFromHex(string hex)
        {
            bool isNeg = false;
            if (isNeg = hex.StartsWith("-"))
                hex = hex.Substring(1, hex.Length - 1);

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
                return BigInteger.Zero;
            return new BigInteger(digits, digitLength, isNeg ? -1 : 1);
        }
        public static bool TryParse(string value, out BigInteger result)
        {
            try
            {
                int digitLength, sign;
                uint[] digits = BigIntInternal.SignedParse(value, out digitLength, out sign);
                result = new BigInteger { _digitLength = digitLength, _digits = digits, _sign = sign };
                return true;
            }
            catch (Exception)
            {
                result = BigInteger.Zero;
                return false;
            }
        }

        public int CompareTo(BigInteger other)
        {
            if (this._sign == other._sign)
            {
                if (this._sign == 0)
                    return 0;
                else
                {
                    int c = BigIntInternal.Compare(this._digits, this._digitLength, other._digits, other._digitLength);
                    if (c == 1)
                    {
                        if (this._sign == 1)
                            return 1;
                        else
                            return -1;
                    }
                    else if (c == -1)
                    {
                        if (this._sign == 1)
                            return -1;
                        else
                            return 1;
                    }
                    else
                        return 0;
                }
            }
            else
            {
                return this._sign.CompareTo(other._sign);
            }
        }
        public override bool Equals(object obj)
        {
            return this.Equals((BigInteger)obj);
        }
        public bool Equals(BigInteger other)
        {
            return this.CompareTo(other) == 0;
        }
        public BigInteger Copy()
        {
            uint[] val = this._digits.Clone() as uint[];
            return new BigInteger() { _digits = val, _sign = this._sign, _digitLength = this._digitLength, };
        }
        public int GetBit(long bitPosition)
        {
            if (this._sign == 0)
                return 0;
            if (bitPosition > this.BitsLength)
                return 0;
            int bit = BigIntInternal.GetBit(this._digits, this._digitLength, bitPosition);
            if (this._sign == 1)
                return bit;
            else
                return ~bit;
        }
        public byte[] GetBytes()
        {
            return this.GetBytes(true);
        }
        public byte[] GetBytes(bool bigEndian)
        {
            return BigIntInternal.GetSignedBytes(this._digits, this._digitLength, bigEndian, this._sign);
        }
        public byte[] GetUnsignedBytes(bool bigEndian)
        {
            return BigIntInternal.GetUnsignedBytes(this._digits, this._digitLength, bigEndian);
        }
        public uint[] GetDigits(out int sign)
        {
            sign = this._sign;
            if (sign == 0)
                return new uint[1];
            else
            {
                uint[] digits = new uint[this._digitLength];
                Array.Copy(this._digits, digits, this._digitLength);
                return digits;
            }
        }
        public override int GetHashCode()
        {
            if (this._sign == 0)
                return 0;

            uint sum = 0;
            for (int i = 0; i < this._digitLength; i++)
            {
                sum += this._digits[i];
            }
            if (this._sign == 1)
                return (int)sum;
            else
                return -(int)sum;
        }
        public void Negate()
        {
            if (this._sign == 0)
                return;

            this._sign = this._sign == 1 ? -1 : 1;
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
        public void SetSign(int sign)
        {
            if (sign == -1 || sign == 0 || sign == 1)
                this._sign = sign;
            else
                throw new ArithmeticException("Unknown sign.");
        }
        public override string ToString()
        {
            if (this._sign == 0)
                return "0";

            string s = BigIntInternal.ToString(this._digits, this._digitLength, this._sign);
            //if (this._sign == -1)
            //    return "-" + s;
            return s;
        }

        internal static BigInteger ReferencedFrom(UnsignedBigInteger value)
        {
            return new BigInteger() { _digitLength = value.DigitsLength, _digits = value.digitsRef, _sign = value.IsZero ? 0 : 1 };
        }

        public static BigInteger operator +(BigInteger left, BigInteger right)
        {
            return BigInteger.Add(left, right);
        }
        public static BigInteger operator -(BigInteger left, BigInteger right)
        {
            return BigInteger.Subtract(left, right);
        }
        public static BigInteger operator *(BigInteger left, BigInteger right)
        {
            return BigInteger.Multiply(left, right);
        }
        public static BigInteger operator /(BigInteger left, BigInteger right)
        {
            return BigInteger.Divide(left, right);
        }
        public static BigInteger operator %(BigInteger left, BigInteger right)
        {
            return BigInteger.Remainder(left, right);
        }
        public static BigInteger operator <<(BigInteger value, int shift)
        {
            return BigInteger.LeftShift(value, shift);
        }
        public static BigInteger operator >>(BigInteger value, int shift)
        {
            return BigInteger.RightShift(value, shift);
        }

        public static BigInteger operator ++(BigInteger value)
        {
            return BigInteger.Add(value, 1);
        }
        public static BigInteger operator --(BigInteger value)
        {
            return BigInteger.Subtract(value, 1);
        }
        public static BigInteger operator -(BigInteger value)
        {
            return BigInteger.Negate(value);
        }

        public static implicit operator BigInteger(byte value)
        {
            return new BigInteger(value);
        }
        public static implicit operator BigInteger(ushort value)
        {
            return new BigInteger(value);
        }
        public static implicit operator BigInteger(uint value)
        {
            return new BigInteger(value);
        }
        public static implicit operator BigInteger(ulong value)
        {
            return new BigInteger(value);
        }
        public static implicit operator BigInteger(sbyte value)
        {
            return new BigInteger(value);
        }
        public static implicit operator BigInteger(short value)
        {
            return new BigInteger(value);
        }
        public static implicit operator BigInteger(int value)
        {
            return new BigInteger(value);
        }
        public static implicit operator BigInteger(long value)
        {
            return new BigInteger(value);
        }
        public static implicit operator BigInteger(UnsignedBigInteger value)
        {
            return new BigInteger(value);
        }
        public static implicit operator BigInteger(string value)
        {
            return BigInteger.Parse(value);
        }
        public static explicit operator int(BigInteger value)
        {
            return (int)value._digits[value._digitLength - 1];
        }

        public static bool operator ==(BigInteger left, sbyte right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigInteger left, sbyte right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigInteger left, sbyte right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigInteger left, sbyte right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigInteger left, sbyte right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigInteger left, sbyte right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigInteger left, byte right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigInteger left, byte right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigInteger left, byte right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigInteger left, byte right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigInteger left, byte right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigInteger left, byte right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigInteger left, short right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigInteger left, short right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigInteger left, short right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigInteger left, short right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigInteger left, short right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigInteger left, short right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigInteger left, ushort right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigInteger left, ushort right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigInteger left, ushort right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigInteger left, ushort right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigInteger left, ushort right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigInteger left, ushort right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigInteger left, int right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigInteger left, int right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigInteger left, int right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigInteger left, int right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigInteger left, int right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigInteger left, int right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigInteger left, uint right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigInteger left, uint right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigInteger left, uint right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigInteger left, uint right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigInteger left, uint right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigInteger left, uint right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigInteger left, long right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigInteger left, long right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigInteger left, long right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigInteger left, long right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigInteger left, long right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigInteger left, long right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigInteger left, ulong right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigInteger left, ulong right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigInteger left, ulong right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigInteger left, ulong right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigInteger left, ulong right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigInteger left, ulong right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) > -1;
        }
        public static bool operator ==(BigInteger left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(BigInteger left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(BigInteger left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(BigInteger left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(BigInteger left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(BigInteger left, UnsignedBigInteger right)
        {
            return left.CompareTo(right) > -1;
        }
    }
}
