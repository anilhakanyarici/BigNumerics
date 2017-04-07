using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.Numerics
{
    //Tüm div metotları numeratöre remainder ataması yapar. Hiç bir numeratör korunmaz.
    //Ref parametreli metotlar, sonuçları döndürmek yerine bu parametrenin üstüne yazar.
    //Eğer ref parametresi korunak istenirse, dizi kopyalamadan metodun refsiz olanını kullan. Onlar, dizi kopyalama olmadan sonuç bulduğu için daha hızlıdır.

    internal static class BigIntInternal
    {
        internal const int maxDigitsLength = 1073741823;
        internal const int maxBytesLength = int.MaxValue; //4GB
        internal const long maxBitsLength = 17179869176L; //4GB

        internal static void Add(ref uint[] left, ref int leftLength, uint[] right, int rightLength) //Tested OK.
        {
            uint[] result;
            int max, min;
            uint[] maxOperand, minOperand;
            if (leftLength > rightLength)
            {
                max = leftLength;
                min = rightLength;
                maxOperand = left;
                minOperand = right;
            }
            else
            {
                min = leftLength;
                max = rightLength;
                maxOperand = right;
                minOperand = left;
            }
            if (left.Length < max)
                Array.Resize(ref left, max + 1);
            result = left;
            ulong carry = 0;
            for (int i = 0; i < min; i++)
            {
                ulong sum = (ulong)minOperand[i] + (ulong)maxOperand[i] + carry;
                carry = sum >> 32;
                result[i] = (uint)(sum);
            }

            if (carry > 0)
            {
                for (int i = min; i < max; i++)
                {
                    ulong sum = (ulong)maxOperand[i] + carry;
                    carry = sum >> 32;
                    result[i] = (uint)(sum);
                }
            }
            else
            {
                for (int i = min; i < max; i++)
                    result[i] = maxOperand[i];
            }
            result[max] = (uint)(carry);
            leftLength = max + 1;
            BigIntInternal.trim(result, ref leftLength);
        }
        internal static uint[] Add(uint[] left, int leftLength, uint[] right, int rightLength, out int resultLength) //Tested OK.
        {
            uint[] result;
            int max, min;
            uint[] maxOperand, minOperand;
            if (leftLength > rightLength)
            {
                max = leftLength;
                min = rightLength;
                maxOperand = left;
                minOperand = right;
            }
            else
            {
                min = leftLength;
                max = rightLength;
                maxOperand = right;
                minOperand = left;
            }
            result = new uint[max + 1];
            ulong carry = 0;
            for (int i = 0; i < min; i++)
            {
                ulong sum = (ulong)minOperand[i] + (ulong)maxOperand[i] + carry;
                carry = sum >> 32;
                result[i] = (uint)(sum);
            }

            if (carry > 0)
            {
                for (int i = min; i < max; i++)
                {
                    ulong sum = (ulong)maxOperand[i] + carry;
                    carry = sum >> 32;
                    result[i] = (uint)(sum);
                }
            }
            else
            {
                for (int i = min; i < max; i++)
                    result[i] = maxOperand[i];
            }
            result[max] = (uint)(carry);
            resultLength = max + 1;
            BigIntInternal.trim(result, ref resultLength);
            return result;
        }
        internal static void AddSingle(ref uint[] left, ref int leftLength, uint right) //Tested OK.
        {
            int max = leftLength;
            ulong carry = 0;

            ulong sum = (ulong)left[0] + (ulong)right + carry;
            carry = sum >> 32;
            left[0] = (uint)(sum);

            for (int i = 1; i < max && carry > 0; i++)
            {
                sum = (ulong)left[i] + carry;
                carry = sum >> 32;
                left[i] = (uint)(sum);
            }

            if (carry > 0)
            {
                if (!(left.Length > max))
                    Array.Resize(ref left, max + 1);

                left[max] = (uint)(carry);
                leftLength = max + 1;
            }
            else
                leftLength = max;
            BigIntInternal.trim(left, ref leftLength);
        }
        internal static uint[] Div(uint[] numerator, int numeratorLength, uint[] denominator, int denominatorLength, out int quotientLength) //Tested OK.
        {
            int denLastU = denominatorLength - 1;
            int numLastU = numeratorLength - 1;

            int opLDiff = numLastU - denLastU;

            quotientLength = opLDiff;
            for (int iu = numLastU; ; iu--)
            {
                if (iu < opLDiff)
                {
                    quotientLength++;
                    break;
                }
                if (denominator[iu - opLDiff] != numerator[iu])
                {
                    if (denominator[iu - opLDiff] < numerator[iu])
                        quotientLength++;
                    break;
                }
            }

            uint[] quotient = new uint[quotientLength];

            uint denFirst = denominator[denominatorLength - 1];
            uint denSecond = denominator[denominatorLength - 2];
            int leftShiftBit = BigIntInternal.countOfZeroBitStart(denFirst);
            int rightShiftBit = 32 - leftShiftBit;
            if (leftShiftBit > 0)
            {
                denFirst = (denFirst << leftShiftBit) | (denSecond >> rightShiftBit);
                denSecond <<= leftShiftBit;
                if (denominatorLength > 2)
                    denSecond |= denominator[denominatorLength - 3] >> rightShiftBit;
            }

            for (int uInd = quotientLength; --uInd >= 0; )
            {
                uint hiNumDig = (uInd + denominatorLength <= numLastU) ? numerator[uInd + denominatorLength] : 0;

                ulong currNum = ((ulong)hiNumDig << 32) | numerator[uInd + denominatorLength - 1];
                uint nextNum = numerator[uInd + denominatorLength - 2];
                if (leftShiftBit > 0)
                {
                    currNum = (currNum << leftShiftBit) | (nextNum >> rightShiftBit);
                    nextNum <<= leftShiftBit;
                    if (uInd + denominatorLength >= 3)
                        nextNum |= numerator[uInd + denominatorLength - 3] >> rightShiftBit;
                }

                ulong rQuot = currNum / denFirst;
                ulong rRem = (uint)(currNum % denFirst);
                if (rQuot > uint.MaxValue)
                {
                    rRem += denFirst * (rQuot - uint.MaxValue);
                    rQuot = uint.MaxValue;
                }
                while (rRem <= uint.MaxValue && rQuot * denSecond > (((ulong)((uint)rRem) << 32) | nextNum))
                {
                    rQuot--;
                    rRem += denFirst;
                }

                if (rQuot > 0)
                {
                    ulong borrow = 0;
                    for (int u = 0; u < denominatorLength; u++)
                    {
                        borrow += denominator[u] * rQuot;
                        uint uSub = (uint)borrow;
                        borrow >>= 32;
                        if (numerator[uInd + u] < uSub)
                            borrow++;
                        numerator[uInd + u] -= uSub;
                    }

                    if (hiNumDig < borrow)
                    {
                        uint uCarry = 0;
                        for (int iu2 = 0; iu2 < denominatorLength; iu2++)
                        {
                            uCarry = addCarry(ref numerator[uInd + iu2], denominator[iu2], uCarry);
                        }
                        rQuot--;
                    }
                    numLastU = uInd + denominatorLength - 1;
                }
                quotient[uInd] = (uint)rQuot;
            }
            BigIntInternal.trim(quotient, ref quotientLength);
            return quotient;
        }
        internal static uint[] DivRem(ref uint[] numerator, int numeratorLength, uint[] denominator, int denominatorLength, out int quotientLength, out int remainderLength) //Tested OK.
        {
            int numLastU = numeratorLength - 1;
            int opLDiff = numLastU - (denominatorLength - 1);

            quotientLength = opLDiff;
            for (int iu = numLastU; ; iu--)
            {
                if (iu < opLDiff)
                {
                    quotientLength++;
                    break;
                }
                if (denominator[iu - opLDiff] != numerator[iu])
                {
                    if (denominator[iu - opLDiff] < numerator[iu])
                        quotientLength++;
                    break;
                }
            }

            uint[] quotient = new uint[quotientLength];

            uint denFirst = denominator[denominatorLength - 1];
            uint denSecond = denominator[denominatorLength - 2];
            int leftShiftBit = BigIntInternal.countOfZeroBitStart(denFirst);
            int rightShiftBit = 32 - leftShiftBit;
            if (leftShiftBit > 0)
            {
                denFirst = (denFirst << leftShiftBit) | (denSecond >> rightShiftBit);
                denSecond <<= leftShiftBit;
                if (denominatorLength > 2)
                    denSecond |= denominator[denominatorLength - 3] >> rightShiftBit;
            }

            for (int uInd = quotientLength; --uInd >= 0; )
            {
                uint hiNumDig = (uInd + denominatorLength <= numLastU) ? numerator[uInd + denominatorLength] : 0;

                ulong currNum = ((ulong)hiNumDig << 32) | numerator[uInd + denominatorLength - 1];
                uint nextNum = numerator[uInd + denominatorLength - 2];
                if (leftShiftBit > 0)
                {
                    currNum = (currNum << leftShiftBit) | (nextNum >> rightShiftBit);
                    nextNum <<= leftShiftBit;
                    if (uInd + denominatorLength >= 3)
                        nextNum |= numerator[uInd + denominatorLength - 3] >> rightShiftBit;
                }

                ulong rQuot = currNum / denFirst;
                ulong rRem = (uint)(currNum % denFirst);
                if (rQuot > uint.MaxValue)
                {
                    rRem += denFirst * (rQuot - uint.MaxValue);
                    rQuot = uint.MaxValue;
                }
                while (rRem <= uint.MaxValue && rQuot * denSecond > (((ulong)((uint)rRem) << 32) | nextNum))
                {
                    rQuot--;
                    rRem += denFirst;
                }

                if (rQuot > 0)
                {
                    ulong borrow = 0;
                    for (int u = 0; u < denominatorLength; u++)
                    {
                        borrow += denominator[u] * rQuot;
                        uint uSub = (uint)borrow;
                        borrow >>= 32;
                        if (numerator[uInd + u] < uSub)
                            borrow++;
                        numerator[uInd + u] -= uSub;
                    }

                    if (hiNumDig < borrow)
                    {
                        uint uCarry = 0;
                        for (int iu2 = 0; iu2 < denominatorLength; iu2++)
                        {
                            uCarry = addCarry(ref numerator[uInd + iu2], denominator[iu2], uCarry);
                        }
                        rQuot--;
                    }
                    numLastU = uInd + denominatorLength - 1;
                }
                quotient[uInd] = (uint)rQuot;
            }

            remainderLength = denominatorLength;
            for (int i = remainderLength; i < numerator.Length; i++)
                numerator[i] = 0;
            BigIntInternal.trim(numerator, ref remainderLength);
            BigIntInternal.trim(quotient, ref quotientLength);
            return quotient;
        }
        internal static uint[] DivRemSingle(uint[] left, int leftLength, uint divisor, out int resultLength, out uint remainder) //Tested OK.
        {
            uint[] r = left.Clone() as uint[];
            uint[] q = new uint[leftLength];

            ulong dividend = r[leftLength - 1];
            int qPos = leftLength - 1;
            int rPos = qPos;
            if (dividend >= divisor)
            {
                ulong quot = dividend / divisor;
                q[qPos--] = (uint)quot;
                r[rPos] = (uint)(dividend % divisor);
            }
            else
                qPos--;
            rPos--;
            while (rPos > -1)
            {
                int rPosPlusOne = rPos + 1;
                dividend = ((ulong)r[rPosPlusOne] << 32) | r[rPos];
                ulong quot = dividend / divisor;
                q[qPos--] = (uint)quot;
                r[rPosPlusOne] = 0;
                r[rPos--] = (uint)(dividend % divisor);
            }
            if (q[q.Length - 1] == 0)
                resultLength = leftLength - 1;
            else
                resultLength = leftLength;

            remainder = r[0];
            BigIntInternal.trim(q, ref resultLength);
            return q;
        }
        internal static uint[] Mul(uint[] left, int leftLength, uint[] right, int rightLength, out int resultLength) //Tested OK.
        {
            resultLength = leftLength + rightLength;
            uint[] result = new uint[resultLength];

            if (leftLength > rightLength)
            {
                int tmp = leftLength;
                leftLength = rightLength;
                rightLength = tmp;
                uint[] tmpb = left;
                left = right;
                right = tmpb;
            }

            for (int i = 0; i < leftLength; i++)
            {
                if (left[i] == 0) continue;

                ulong carry = 0;
                for (int j = 0, k = i; j < rightLength; j++, k++)
                {
                    ulong val = ((ulong)left[i] * right[j]) + result[k] + carry;
                    result[k] = (uint)val;
                    carry = (val >> 32);
                }
                if (carry != 0)
                    result[i + rightLength] = (uint)carry;
            }

            BigIntInternal.trim(result, ref resultLength);
            return result;
        }
        internal static uint[] MulSingle(uint[] left, int leftLength, uint right, out int resultLength) //Tested OK.
        {
            resultLength = leftLength + 1;
            uint[] result = new uint[resultLength];

            for (int i = 0; i < leftLength; i++)
            {
                if (left[i] == 0) continue;

                ulong carry = 0;

                ulong val = ((ulong)left[i] * (ulong)right) + (ulong)result[i] + carry;
                result[i] = (uint)(val & 0xFFFFFFFF);
                carry = (val >> 32);

                if (carry != 0)
                    result[i + 1] = (uint)carry;
            }

            BigIntInternal.trim(result, ref resultLength);
            return result;
        }
        internal static void Rem(ref uint[] numerator, ref int numeratorLength, uint[] denominator, int denominatorLength) //Tested OK.
        {
            int denLastU = denominatorLength - 1;
            int numLastU = numeratorLength - 1;

            int opLDiff = numLastU - denLastU;

            int quotientLength = opLDiff;
            for (int iu = numLastU; ; iu--)
            {
                if (iu < opLDiff)
                {
                    quotientLength++;
                    break;
                }
                if (denominator[iu - opLDiff] != numerator[iu])
                {
                    if (denominator[iu - opLDiff] < numerator[iu])
                        quotientLength++;
                    break;
                }
            }

            uint denFirst = denominator[denominatorLength - 1];
            uint denSecond = denominator[denominatorLength - 2];
            int leftShiftBit = BigIntInternal.countOfZeroBitStart(denFirst);
            int rightShiftBit = 32 - leftShiftBit;
            if (leftShiftBit > 0)
            {
                denFirst = (denFirst << leftShiftBit) | (denSecond >> rightShiftBit);
                denSecond <<= leftShiftBit;
                if (denominatorLength > 2)
                    denSecond |= denominator[denominatorLength - 3] >> rightShiftBit;
            }

            for (int iu = quotientLength; --iu >= 0; )
            {
                uint uNumHi = (iu + denominatorLength <= numLastU) ? numerator[iu + denominatorLength] : 0;

                ulong uuNum = ((ulong)uNumHi << 32) | numerator[iu + denominatorLength - 1];
                uint uNumNext = numerator[iu + denominatorLength - 2];
                if (leftShiftBit > 0)
                {
                    uuNum = (uuNum << leftShiftBit) | (uNumNext >> rightShiftBit);
                    uNumNext <<= leftShiftBit;
                    if (iu + denominatorLength >= 3)
                        uNumNext |= numerator[iu + denominatorLength - 3] >> rightShiftBit;
                }

                ulong uuQuo = uuNum / denFirst;
                ulong uuRem = (uint)(uuNum % denFirst);
                if (uuQuo > uint.MaxValue)
                {
                    uuRem += denFirst * (uuQuo - uint.MaxValue);
                    uuQuo = uint.MaxValue;
                }
                while (uuRem <= uint.MaxValue && uuQuo * denSecond > (((ulong)((uint)uuRem) << 32) | uNumNext))
                {
                    uuQuo--;
                    uuRem += denFirst;
                }

                if (uuQuo > 0)
                {
                    ulong uuBorrow = 0;
                    for (int iu2 = 0; iu2 < denominatorLength; iu2++)
                    {
                        uuBorrow += denominator[iu2] * uuQuo;
                        uint uSub = (uint)uuBorrow;
                        uuBorrow >>= 32;
                        if (numerator[iu + iu2] < uSub)
                            uuBorrow++;
                        numerator[iu + iu2] -= uSub;
                    }

                    if (uNumHi < uuBorrow)
                    {
                        uint uCarry = 0;
                        for (int iu2 = 0; iu2 < denominatorLength; iu2++)
                        {
                            uCarry = addCarry(ref numerator[iu + iu2], denominator[iu2], uCarry);
                        }
                        uuQuo--;
                    }
                    numLastU = iu + denominatorLength - 1;
                }
            }
            numeratorLength = denominatorLength;
            for (int i = numeratorLength; i < numerator.Length; i++)
                numerator[i] = 0;
            BigIntInternal.trim(numerator, ref numeratorLength);
        }
        internal static void Sub(ref uint[] left, ref int leftLength, uint[] right, int rightLength) //Tested OK.
        {
            int max, min;
            uint[] maxOperand;
            if (leftLength > rightLength)
            {
                max = leftLength;
                min = rightLength;
                maxOperand = left;
            }
            else
            {
                min = leftLength;
                max = rightLength;
                maxOperand = right;
            }
            if (left.Length < max)
                Array.Resize(ref left, max);
            uint[] result = left;

            long carry = 0;
            for (int i = 0; i < min; i++)
            {
                long diff = (long)left[i] - (long)right[i] - carry;
                result[i] = (uint)(diff);

                if (diff < 0)
                    carry = 1;
                else
                    carry = 0;
            }
            if (carry > 0)
            {
                for (int i = min; i < max; i++)
                {
                    long diff = (long)maxOperand[i] - carry;
                    result[i] = (uint)(diff);

                    if (diff < 0)
                        carry = 1;
                    else
                        carry = 0;
                }
            }
            else
            {
                for (int i = min; i < max; i++)
                    result[i] = maxOperand[i];
            }

            leftLength = max;
            BigIntInternal.trim(result, ref leftLength);
        }
        internal static uint[] Sub(uint[] left, int leftLength, uint[] right, int rightLength, out int resultLength) //Tested OK.
        {
            int max, min;
            uint[] maxOperand;
            if (leftLength > rightLength)
            {
                max = leftLength;
                min = rightLength;
                maxOperand = left;
            }
            else
            {
                min = leftLength;
                max = rightLength;
                maxOperand = right;
            }

            uint[] result = new uint[max];

            long carry = 0;
            for (int i = 0; i < min; i++)
            {
                long diff = (long)left[i] - (long)right[i] - carry;
                result[i] = (uint)(diff);

                if (diff < 0)
                    carry = 1;
                else
                    carry = 0;
            }
            if (carry > 0)
            {
                for (int i = min; i < max; i++)
                {
                    long diff = (long)maxOperand[i] - carry;
                    result[i] = (uint)(diff);

                    if (diff < 0)
                        carry = 1;
                    else
                        carry = 0;
                }
            }
            else
            {
                for (int i = min; i < max; i++)
                    result[i] = maxOperand[i];
            }

            resultLength = max;
            BigIntInternal.trim(result, ref resultLength);
            return result;
        }
        internal static void SubSingle(ref uint[] left, ref int leftLength, uint right) //Tested OK.
        {
            int max = leftLength;

            long carry = 0;

            long diff = (long)left[0] - (long)right - carry;
            left[0] = (uint)(diff);

            if (diff < 0)
                carry = 1;
            else
                carry = 0;

            for (int i = 1; i < max && carry > 0; i++)
            {
                diff = (long)left[i] - carry;
                left[i] = (uint)(diff);

                if (diff < 0)
                    carry = 1;
                else
                    carry = 0;
            }
            leftLength = max;
            BigIntInternal.trim(left, ref leftLength);
        }

        internal static void ShiftLeft(ref uint[] digits, ref int digitLength, int shift) //Tested OK. 
        {
            if (shift == 0) return;

            int digitShift = shift / 32;
            int smallShift = shift - (digitShift * 32);

            int xl = digitLength;

            int zl = xl + digitShift + 1;
            uint[] zd = new uint[zl];

            if (smallShift == 0)
                for (int i = 0; i < xl; i++)
                    zd[i + digitShift] = digits[i];
            else
            {
                int carryShift = 32 - smallShift;
                uint carry = 0;
                int i;
                for (i = 0; i < xl; i++)
                {
                    uint rot = digits[i];
                    zd[i + digitShift] = rot << smallShift | carry;
                    carry = rot >> carryShift;
                }
                zd[i + digitShift] = carry;
            }
            digits = zd;
            if (zd[zl - 1] == 0)
                digitLength = zl - 1;
            else
                digitLength = zl;
            BigIntInternal.trim(digits, ref digitLength);
        }
        internal static uint[] ShiftLeft(uint[] digits, ref int digitLength, int shift) //Tested OK. 
        {
            if (shift == 0) return digits.Clone() as uint[];

            int digitShift = shift / 32;
            int smallShift = shift - (digitShift * 32);

            int xl = digitLength;

            int zl = xl + digitShift + 1;
            uint[] zd = new uint[zl];

            if (smallShift == 0)
                for (int i = 0; i < xl; i++)
                    zd[i + digitShift] = digits[i];
            else
            {
                int carryShift = 32 - smallShift;
                uint carry = 0;
                int i;
                for (i = 0; i < xl; i++)
                {
                    uint rot = digits[i];
                    zd[i + digitShift] = rot << smallShift | carry;
                    carry = rot >> carryShift;
                }
                zd[i + digitShift] = carry;
            }
            if (zd[zl - 1] == 0)
                digitLength = zl - 1;
            else
                digitLength = zl;
            BigIntInternal.trim(zd, ref digitLength);
            return zd;
        }
        internal static void ShiftRight(ref uint[] digits, ref int digitLength, int shift) //Tested OK. 
        {
            int shiftAmount = 32;
            int invShift = 0;
            int bufLen = digits.Length;

            while (bufLen > 1 && digits[bufLen - 1] == 0)
                bufLen--;

            for (int count = shift; count > 0; )
            {
                if (count < shiftAmount)
                {
                    shiftAmount = count;
                    invShift = 32 - shiftAmount;
                }

                ulong carry = 0;
                for (int i = bufLen - 1; i >= 0; i--)
                {
                    ulong val = ((ulong)digits[i]) >> shiftAmount;
                    val |= carry;

                    carry = ((ulong)digits[i]) << invShift;
                    digits[i] = (uint)(val);
                }

                count -= shiftAmount;
            }
            digitLength = bufLen;
            BigIntInternal.trim(digits, ref digitLength);
        }

        internal static uint[] And(uint[] left, int leftLength, uint[] right, int rightLength, out int resultLength) //Tested OK.
        {
            int max, min;
            if (leftLength > rightLength)
            {
                max = leftLength;
                min = rightLength;
            }
            else
            {
                max = rightLength;
                min = leftLength;
            }

            uint[] result = new uint[min];
            int i;
            for (i = 0; i < min; i++)
                result[i] = left[i] & right[i];

            resultLength = min;
            BigIntInternal.trim(result, ref resultLength);
            return result;
        }
        internal static void And(ref uint[] left, ref int leftLength, uint[] right, int rightLength) //Tested OK.
        {
            if (leftLength < rightLength)
                throw new WarningException("sol operandın uzunluğu sağdakinden büyük veya eşit olmalı.");
            for (int i = 0; i < rightLength; i++)
                left[i] = left[i] & right[i];
            for (int i = rightLength; i < leftLength; i++)
                left[i] = 0;
            leftLength = leftLength - rightLength;
        }
        internal static int Compare(uint[] left, int leftLength, uint[] right, int rightLength) //Tested OK.
        {
            if (leftLength > rightLength)
                return 1;
            else if (leftLength < rightLength)
                return -1;
            else
            {
                for (int i = leftLength - 1; i >= 0; i--)
                {
                    int c = left[i].CompareTo(right[i]);
                    if (c != 0)
                        return c;
                }
                return 0;
            }
        }
        internal static void Not(ref uint[] digits, ref int digitLength) //Tested OK.
        {
            for (int i = 0; i < digitLength; i++)
                digits[i] = ~digits[i];
        }
        internal static uint[] Or(uint[] left, int leftLength, uint[] right, int rightLength) //Tested OK.
        {
            int max, min; uint[] maxOperand;
            if (leftLength > rightLength)
            {
                max = leftLength;
                min = rightLength;
                maxOperand = left;
            }
            else
            {
                max = rightLength;
                min = leftLength;
                maxOperand = right;
            }

            uint[] result = new uint[max];
            int i;
            for (i = 0; i < min; i++)
                result[i] = left[i] | right[i];
            for (; i < max; i++)
                result[i] = maxOperand[i];
            return result;
        }
        internal static void Or(ref uint[] left, int leftLength, uint[] right, int rightLength) //Tested OK.
        {
            if (leftLength < rightLength)
                throw new WarningException("sol operandın uzunluğu sağdakinden büyük veya eşit olmalı.");
            int min = Math.Min(leftLength, rightLength);
            for (int i = 0; i < min; i++)
                left[i] = left[i] | right[i];
        }
        internal static uint[] Xor(uint[] left, int leftLength, uint[] right, int rightLength) //Tested OK.
        {
            int max, min; uint[] maxOperand;
            if (leftLength > rightLength)
            {
                max = leftLength;
                min = rightLength;
                maxOperand = left;
            }
            else
            {
                max = rightLength;
                min = leftLength;
                maxOperand = right;
            }

            uint[] result = new uint[max];
            int i;
            for (i = 0; i < min; i++)
                result[i] = left[i] ^ right[i];
            for (; i < max; i++)
                result[i] = maxOperand[i];
            return result;
        }
        internal static void Xor(ref uint[] left, int leftLength, uint[] right, int rightLength) //Tested OK.
        {
            if (leftLength < rightLength)
                throw new WarningException("sol operandın uzunluğu sağdakinden büyük veya eşit olmalı.");
            int min = Math.Min(leftLength, rightLength);
            for (int i = 0; i < min; i++)
                left[i] = left[i] ^ right[i];
        }

        internal static long UnsignedBitsLength(uint[] digits, int digitLength) //Tested OK.
        {
            if (digitLength == 0)
                return 0;

            uint uiLast = digits[digitLength - 1];
            return 32 * (long)digitLength - BigIntInternal.countOfZeroBitStart(uiLast);
        }
        internal static long SignedBitsLength(uint[] digits, int digitLength, int sign) //Tested OK.
        {
            if (sign == 0)
                return 1;
            if (digitLength == 1 && digits[0] == 0)
                return 1;

            uint lastDigit = digits[digitLength - 1];
            byte lastByte = 0;
            int bitsLength = digitLength * 32;

            if ((lastByte = (byte)(lastDigit >> 24)) != 0) { }
            else if ((lastByte = (byte)(lastDigit >> 16)) != 0) { bitsLength -= 8; }
            else if ((lastByte = (byte)(lastDigit >> 8)) != 0) { bitsLength -= 16; }
            else if ((lastByte = (byte)(lastDigit)) != 0) { bitsLength -= 24; }

            if ((lastByte >> 7) == 1 && sign == -1)
                bitsLength += 8;
            return bitsLength;
        }
        internal static int UnsignedBytesLength(uint[] digits, int digitLength) //Tested OK.
        {
            if (digitLength == 1 && digits[0] == 0)
                return 1;

            uint uiLast = digits[digitLength - 1];
            int bytesLength = 4 * digitLength;
            if (uiLast >> 8 == 0)
                bytesLength -= 3;
            else if (uiLast >> 16 == 0)
                bytesLength -= 2;
            else if (uiLast >> 24 == 0)
                bytesLength -= 1;
            return bytesLength;
        }
        internal static int SignedBytesLength(uint[] digits, int digitLength, int sign) //Tested OK.
        {
            if (sign == 0)
                return 1;
            if (digitLength == 1 && digits[0] == 0)
                return 1;

            uint lastDigit = digits[digitLength - 1];
            byte lastByte = 0;
            int bytesLength = digitLength * 4;

            if ((lastByte = (byte)(lastDigit >> 24)) != 0) { }
            else if ((lastByte = (byte)(lastDigit >> 16)) != 0) { bytesLength -= 1; }
            else if ((lastByte = (byte)(lastDigit >> 8)) != 0) { bytesLength -= 2; }
            else if ((lastByte = (byte)(lastDigit)) != 0) { bytesLength -= 3; }

            if ((lastByte >> 7) == 1)
                bytesLength++;
            return bytesLength;
        }
        internal static string ToString(uint[] digits, int digitLength, int sign) //Tested OK.
        {
            if (sign == 0)
                return "0";
            else if (digitLength == 0)
                return "0";
            else if (digitLength == 1 && sign == 1)
                return digits[0].ToString();

            const uint kuBase = 1000000000; // 10^9

            int cuMax = digitLength * 10 / 9 + 2;

            uint[] rguDst = new uint[cuMax];
            int cuDst = 0;

            for (int iuSrc = digitLength; --iuSrc >= 0; )
            {
                uint uCarry = digits[iuSrc];
                for (int iuDst = 0; iuDst < cuDst; iuDst++)
                {
                    ulong uuRes = ((ulong)rguDst[iuDst] << 32) | uCarry;
                    rguDst[iuDst] = (uint)(uuRes % kuBase);
                    uCarry = (uint)(uuRes / kuBase);
                }
                if (uCarry != 0)
                {
                    rguDst[cuDst++] = uCarry % kuBase;
                    uCarry /= kuBase;
                    if (uCarry != 0)
                        rguDst[cuDst++] = uCarry;
                }
            }

            int cchMax = cuDst * 9;

            int rgchBufSize = cchMax + 1;
            char[] rgch;

            if (sign == -1)
            {
                rgchBufSize++;
                rgch = new char[rgchBufSize];
            }
            else
                rgch = new char[rgchBufSize];


            int ichDst = cchMax;

            for (int iuDst = 0; iuDst < cuDst - 1; iuDst++)
            {
                uint uDig = rguDst[iuDst];
                for (int cch = 9; --cch >= 0; )
                {
                    rgch[--ichDst] = (char)('0' + uDig % 10);
                    uDig /= 10;
                }
            }
            for (uint uDig = rguDst[cuDst - 1]; uDig != 0; )
            {
                rgch[--ichDst] = (char)('0' + uDig % 10);
                uDig /= 10;
            }
            if (sign == -1)
            {
                rgch[--ichDst] = '-';
                return new String(rgch, ichDst, cchMax - ichDst);
            }
            else
                return new String(rgch, ichDst, cchMax - ichDst);
        }
        internal static uint[] UnsignedParse(string value, out int digitLength) //Tested OK.
        {
            int offset = 0;
            const uint cBase = 100000000;
            uint[] digitsBase10Pow8 = new uint[value.Length / 8 + Math.Sign(offset = value.Length % 8)];
            if (offset == 0)
                digitsBase10Pow8[digitsBase10Pow8.Length - 1] = uint.Parse(value.Substring(0, offset += 8));
            else
                digitsBase10Pow8[digitsBase10Pow8.Length - 1] = uint.Parse(value.Substring(0, offset));

            char[] chars = new char[8];
            for (int i = digitsBase10Pow8.Length - 2; i >= 0; i--)
            {
                value.CopyTo(offset, chars, 0, 8);
                offset += 8;
                digitsBase10Pow8[i] = uint.Parse(new string(chars));
            }

            digitLength = digitsBase10Pow8.Length;
            uint[] data = new uint[digitLength];

            BigIntInternal.AddSingle(ref data, ref digitLength, digitsBase10Pow8[digitsBase10Pow8.Length - 1]);
            for (int i = digitsBase10Pow8.Length - 2; i >= 0; i--)
            {
                data = BigIntInternal.MulSingle(data, digitLength, cBase, out digitLength);
                BigIntInternal.AddSingle(ref data, ref digitLength, digitsBase10Pow8[i]);
            }
            return data;
        }
        internal static uint[] SignedParse(string value, out int digitLength, out int sign) //Tested OK.
        {
            if (value[0] == '-')
            {
                sign = -1;
                uint[] digits = BigIntInternal.UnsignedParse(value.Substring(1, value.Length - 1), out digitLength);
                if (digitLength == 1 && digits[0] == 0)
                    sign = 0;
                return digits;
            }
            else if (value == "0")
            {
                sign = 0;
                digitLength = 1;
                return new uint[1];
            }
            else
            {
                sign = 1;
                return BigIntInternal.UnsignedParse(value, out digitLength);
            }
        }
        internal static byte[] GetUnsignedBytes(uint[] digits, int digitLength, bool bigEndian) //Tested OK.
        {
            if (digitLength == 1 && digits[0] == 0)
                return new byte[1];
            int bytesLength = BigIntInternal.UnsignedBytesLength(digits, digitLength);
            byte[] bytes = new byte[bytesLength];
            if (bigEndian)
            {
                int bytesPos = 0;
                int dataPos = digitLength - 1;
                uint first = digits[dataPos--];

                int nullBytesLength = BigIntInternal.countOfZeroBitStart(first) / 8;

                while (nullBytesLength == 4)
                {
                    first = digits[dataPos--];
                    nullBytesLength = BigIntInternal.countOfZeroBitStart(first) / 8;
                }

                if (nullBytesLength == 3)
                {
                    bytes[bytesPos++] = (byte)first;
                }
                else if (nullBytesLength == 2)
                {
                    bytes[bytesPos++] = (byte)(first >> 8);
                    bytes[bytesPos++] = (byte)first;
                }
                else if (nullBytesLength == 1)
                {
                    bytes[bytesPos++] = (byte)(first >> 16);
                    bytes[bytesPos++] = (byte)(first >> 8);
                    bytes[bytesPos++] = (byte)first;
                }
                else if (nullBytesLength == 0)
                {
                    bytes[bytesPos++] = (byte)(first >> 24);
                    bytes[bytesPos++] = (byte)(first >> 16);
                    bytes[bytesPos++] = (byte)(first >> 8);
                    bytes[bytesPos++] = (byte)first;
                }

                while (dataPos > -1)
                {
                    uint current = digits[dataPos--];
                    bytes[bytesPos++] = (byte)(current >> 24);
                    bytes[bytesPos++] = (byte)(current >> 16);
                    bytes[bytesPos++] = (byte)(current >> 8);
                    bytes[bytesPos++] = (byte)(current);
                }
            }
            else
            {
                int bytesPos = 0;
                int dataPos = 0;
                int lastDigit = digitLength - 1;
                while (dataPos < lastDigit)
                {
                    uint current = digits[dataPos++];
                    bytes[bytesPos++] = (byte)(current);
                    bytes[bytesPos++] = (byte)(current >> 8);
                    bytes[bytesPos++] = (byte)(current >> 16);
                    bytes[bytesPos++] = (byte)(current >> 24);
                }
                uint uiLast = digits[lastDigit];
                int nullDataLength = BigIntInternal.countOfZeroBitStart(uiLast) / 8;

                if (nullDataLength == 0)
                {
                    bytes[bytesPos++] = (byte)(uiLast);
                    bytes[bytesPos++] = (byte)(uiLast >> 8);
                    bytes[bytesPos++] = (byte)(uiLast >> 16);
                    bytes[bytesPos++] = (byte)(uiLast >> 24);
                }
                else if (nullDataLength == 1)
                {
                    bytes[bytesPos++] = (byte)(uiLast);
                    bytes[bytesPos++] = (byte)(uiLast >> 8);
                    bytes[bytesPos++] = (byte)(uiLast >> 16);
                }
                else if (nullDataLength == 2)
                {
                    bytes[bytesPos++] = (byte)(uiLast);
                    bytes[bytesPos++] = (byte)(uiLast >> 8);
                }
                else if (nullDataLength == 3)
                {
                    bytes[bytesPos++] = (byte)(uiLast);
                }
            }
            return bytes;
        }
        internal static uint[] FromUnsignedBytes(byte[] data, bool bigEndian, out int digitLength) //Tested OK.
        {
            digitLength = data.Length / 4;
            if ((data.Length & 3) > 0)
                digitLength++;

            uint[] digits = new uint[digitLength];

            if (bigEndian)
            {
                int digitPos = digitLength - 1;
                int dataPos = 0;

                int nullDataLength = data.Length & 3;
                if (nullDataLength == 1)
                {
                    digits[digitPos--] = data[dataPos++];
                }
                else if (nullDataLength == 2)
                {
                    uint digit = 0;
                    digit |= (uint)(data[dataPos++] << 8);
                    digit |= (uint)(data[dataPos++]);
                    digits[digitPos--] = digit;
                }
                else if (nullDataLength == 3)
                {
                    uint digit = 0;
                    digit |= (uint)(data[dataPos++] << 16);
                    digit |= (uint)(data[dataPos++] << 8);
                    digit |= (uint)(data[dataPos++]);
                    digits[digitPos--] = digit;
                }

                while (digitPos > -1)
                {
                    uint current = 0;
                    current |= (uint)(data[dataPos++] << 24);
                    current |= (uint)(data[dataPos++] << 16);
                    current |= (uint)(data[dataPos++] << 8);
                    current |= (uint)(data[dataPos++]);
                    digits[digitPos--] = current;
                }
            }
            else
            {
                int digitPos = 0;
                int dataPos = 0;
                int lastDigitPos = digitLength - 1;
                while (digitPos < lastDigitPos)
                {
                    uint current = 0;
                    current |= (uint)(data[dataPos++]);
                    current |= (uint)(data[dataPos++] << 8);
                    current |= (uint)(data[dataPos++] << 16);
                    current |= (uint)(data[dataPos++] << 24);
                    digits[digitPos++] = current;
                }

                int nullDataLength = data.Length & 3;

                if (nullDataLength == 1)
                {
                    digits[lastDigitPos] = data[dataPos++];
                }
                else if (nullDataLength == 2)
                {
                    uint digit = 0;
                    digit |= (uint)(data[dataPos++]);
                    digit |= (uint)(data[dataPos++] << 8);
                    digits[lastDigitPos] = digit;
                }
                else if (nullDataLength == 3)
                {
                    uint digit = 0;
                    digit |= (uint)(data[dataPos++]);
                    digit |= (uint)(data[dataPos++] << 8);
                    digit |= (uint)(data[dataPos++] << 16);
                    digits[lastDigitPos] = digit;
                }
                else if (nullDataLength == 0)
                {
                    uint digit = 0;
                    digit |= (uint)(data[dataPos++]);
                    digit |= (uint)(data[dataPos++] << 8);
                    digit |= (uint)(data[dataPos++] << 16);
                    digit |= (uint)(data[dataPos++] << 24);
                    digits[lastDigitPos] = digit;
                }
            }
            BigIntInternal.trim(digits, ref digitLength);
            return digits;
        }
        internal static int GetBit(uint[] digits, int digitLength, long bitPosition) //Tested OK.
        {
            int digitPos = (int)(bitPosition / 32);
            if (digitLength <= digitPos)
                return 0;

            int smallBitPos = (int)(bitPosition & 31);
            return (int)((digits[digitPos] >> smallBitPos) & 1);
        }
        internal static void SetBit(ref uint[] digits, ref int digitLength, long bitPosition, int bit)
        {
            int setDigPos = (int)(bitPosition / 32);
            int smallPos = (int)(bitPosition & 31);

            if (bit == 1)
            {
                if (setDigPos > digitLength - 1)
                {
                    digitLength = setDigPos + 1;
                    Array.Resize(ref digits, digitLength);
                }
                digits[setDigPos] |= ((uint)1 << smallPos);
            }
            else
            {
                if (setDigPos < digitLength)
                    digits[setDigPos] &= ~((uint)1 << smallPos);

                BigIntInternal.trim(digits, ref digitLength);
            }
        }
        internal static byte[] GetSignedBytes(uint[] digits, int digitLength, bool bigEndian, int sign) //Tested OK.
        {
            if (sign == 0)
                return new byte[1];

            if (sign == -1)
            {
                uint[] cDig = digits.Clone() as uint[];
                BigIntInternal.SubSingle(ref cDig, ref digitLength, 1);
                digits = cDig;
            }

            int bytesLength = BigIntInternal.UnsignedBytesLength(digits, digitLength);

            uint lastDigit = digits[digitLength - 1];
            byte lastByte = 0;

            if ((lastByte = (byte)(lastDigit >> 24)) != 0) { }
            else if ((lastByte = (byte)(lastDigit >> 16)) != 0) { }
            else if ((lastByte = (byte)(lastDigit >> 8)) != 0) { }
            else if ((lastByte = (byte)(lastDigit)) != 0) { }

            bool isLastBitOne = (lastByte >> 7) == 1;
            byte[] bytes;
            int nullBytesLength = BigIntInternal.countOfZeroBitStart(lastDigit) / 8;

            if (bigEndian)
            {
                int digitPos = digitLength - 2;
                int bytesPos = 0;
                if (isLastBitOne)
                {
                    bytesLength++;
                    bytes = new byte[bytesLength];
                    bytesPos++;
                    //if (sign == -1)
                    //{
                    //    bytes[bytesPos++] = 128;
                    //}
                    //else
                    //    bytesPos++;
                }
                else
                {
                    bytes = new byte[bytesLength];
                    //if (sign == -1)
                    //    lastByte |= 128;
                }
                if (nullBytesLength == 0)
                {
                    bytes[bytesPos++] = lastByte;
                    bytes[bytesPos++] = (byte)(lastDigit >> 16);
                    bytes[bytesPos++] = (byte)(lastDigit >> 8);
                    bytes[bytesPos++] = (byte)lastDigit;
                }
                else if (nullBytesLength == 1)
                {
                    bytes[bytesPos++] = lastByte;
                    bytes[bytesPos++] = (byte)(lastDigit >> 8);
                    bytes[bytesPos++] = (byte)lastDigit;
                }
                else if (nullBytesLength == 2)
                {
                    bytes[bytesPos++] = lastByte;
                    bytes[bytesPos++] = (byte)lastDigit;
                }
                else if (nullBytesLength == 3)
                    bytes[bytesPos++] = lastByte;

                while (digitPos > -1)
                {
                    uint digit = digits[digitPos--];
                    bytes[bytesPos++] = (byte)(digit >> 24);
                    bytes[bytesPos++] = (byte)(digit >> 16);
                    bytes[bytesPos++] = (byte)(digit >> 8);
                    bytes[bytesPos++] = (byte)digit;
                }
                if (sign == -1)
                {
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = (byte)(~bytes[i]);
                    }

                }
            }
            else
            {
                int digitPos = 0;
                int bytesPos = 0;

                if (isLastBitOne)
                {
                    bytesLength++;
                    bytes = new byte[bytesLength];
                    //if (sign == -1)
                    //    bytes[bytesLength - 1] = 128;
                }
                else
                {
                    bytes = new byte[bytesLength];
                    //if (sign == -1)
                    //    lastByte |= 128;
                }

                while (digitPos < digitLength - 1)
                {
                    uint digit = digits[digitPos++];
                    bytes[bytesPos++] = (byte)digit;
                    bytes[bytesPos++] = (byte)(digit >> 8);
                    bytes[bytesPos++] = (byte)(digit >> 16);
                    bytes[bytesPos++] = (byte)(digit >> 24);
                }

                if (nullBytesLength == 0)
                {
                    bytes[bytesPos++] = (byte)lastDigit;
                    bytes[bytesPos++] = (byte)(lastDigit >> 8);
                    bytes[bytesPos++] = (byte)(lastDigit >> 16);
                    bytes[bytesPos++] = lastByte;
                }
                if (nullBytesLength == 1)
                {
                    bytes[bytesPos++] = (byte)lastDigit;
                    bytes[bytesPos++] = (byte)(lastDigit >> 8);
                    bytes[bytesPos++] = lastByte;
                }
                else if (nullBytesLength == 2)
                {
                    bytes[bytesPos++] = (byte)lastDigit;
                    bytes[bytesPos++] = lastByte;
                }
                else if (nullBytesLength == 3)
                    bytes[bytesPos++] = lastByte;

                if (sign == -1)
                {
                    for (int i = 0; i < bytes.Length; i++)
                        bytes[i] = (byte)(~bytes[i]);
                }
            }
            return bytes;
        }
        internal static uint[] FromSignedBytes(byte[] data, bool bigEndian, out int digitLength, out int sign) //Tested OK.
        {
            if (data.Length == 1)
            {
                digitLength = 1;
                if (data[0] == 0)
                {
                    sign = 0;
                    return new uint[1];
                }
                else if (data[0] > 127)
                {
                    sign = -1;
                    return new uint[1] { (uint)(~data[0] + 1) };
                }
                else
                {
                    sign = 1;
                    return new uint[1] { data[0] };
                }
            }

            if (bigEndian)
            {
                byte lastBytes = data[0];
                byte notLastBytes = (byte)(~lastBytes);
                int readableLength = data.Length;
                bool isNeg = notLastBytes < 128;
                if (notLastBytes == 0)
                    readableLength--;
                if (!isNeg && lastBytes == 0)
                    readableLength--;

                digitLength = readableLength / 4;
                int nullBytesCount = 4 - (readableLength & 3);
                if (nullBytesCount != 4)
                    digitLength++;

                int dataPos = data.Length - 1;

                uint[] digits = new uint[digitLength];
                int digitPos = 0;
                int uiLast = digitLength - 1;

                if (isNeg)
                {
                    while (digitPos < uiLast)
                    {
                        uint digit = ((byte)(~data[dataPos--]));
                        digit |= (uint)((byte)(~data[dataPos--]) << 8);
                        digit |= (uint)((byte)(~data[dataPos--]) << 16);
                        digit |= (uint)((byte)(~data[dataPos--]) << 24);
                        digits[digitPos++] = digit;
                    }

                    if (nullBytesCount == 4)
                    {
                        uint digit = ((byte)(~data[dataPos--]));
                        digit |= (uint)((byte)(~data[dataPos--]) << 8);
                        digit |= (uint)((byte)(~data[dataPos--]) << 16);
                        digit |= (uint)((byte)(~data[dataPos--]) << 24);
                        digits[uiLast] = digit;
                    }
                    else if (nullBytesCount == 1)
                    {
                        uint digit = ((byte)(~data[dataPos--]));
                        digit |= (uint)((byte)(~data[dataPos--]) << 8);
                        digit |= (uint)((byte)(~data[dataPos--]) << 16);
                        digits[uiLast] = digit;
                    }
                    else if (nullBytesCount == 2)
                    {
                        uint digit = ((byte)(~data[dataPos--]));
                        digit |= (uint)((byte)(~data[dataPos--]) << 8);
                        digits[uiLast] = digit;
                    }
                    else if (nullBytesCount == 3)
                    {
                        digits[uiLast] = ((byte)(~data[dataPos--]));
                    }
                    sign = -1;
                    BigIntInternal.AddSingle(ref digits, ref digitLength, 1);
                }
                else
                {
                    while (digitPos < uiLast)
                    {
                        uint digit = data[dataPos--];
                        digit |= (uint)(data[dataPos--] << 8);
                        digit |= (uint)(data[dataPos--] << 16);
                        digit |= (uint)(data[dataPos--] << 24);
                        digits[digitPos++] = digit;
                    }

                    if (nullBytesCount == 4)
                    {
                        uint digit = data[dataPos--];
                        digit |= (uint)(data[dataPos--] << 8);
                        digit |= (uint)(data[dataPos--] << 16);
                        digit |= (uint)(data[dataPos--] << 24);
                        digits[digitPos++] = digit;
                    }
                    else if (nullBytesCount == 1)
                    {
                        uint digit = data[dataPos--];
                        digit |= (uint)(data[dataPos--] << 8);
                        digit |= (uint)(data[dataPos--] << 16);
                        digits[uiLast] = digit;
                    }
                    else if (nullBytesCount == 2)
                    {
                        uint digit = data[dataPos--];
                        digit |= (uint)(data[dataPos--] << 8);
                        digits[uiLast] = digit;
                    }
                    else if (nullBytesCount == 3)
                    {
                        digits[uiLast] = data[dataPos--];
                    }
                    sign = 1;
                }
                return digits;
            }
            else
            {
                byte lastBytes = data[data.Length - 1];
                byte notLastBytes = (byte)(~lastBytes);
                int readableLength = data.Length;
                bool isNeg = notLastBytes < 128;
                if (notLastBytes == 0)
                    readableLength--;
                if (!isNeg && lastBytes == 0)
                    readableLength--;

                digitLength = readableLength / 4;
                int nullBytesCount = 4 - (readableLength & 3);
                if (nullBytesCount != 4)
                    digitLength++;

                uint[] digits = new uint[digitLength];
                int digitPos = digitLength - 1;
                int dataPos = readableLength - 1;

                if (isNeg)
                {
                    if (nullBytesCount == 1)
                    {
                        uint digit = (uint)((byte)(~data[dataPos--]) << 16);
                        digit |= (uint)((byte)(~data[dataPos--]) << 8);
                        digit |= ((byte)(~data[dataPos--]));
                        digits[digitPos--] = digit;
                    }
                    else if (nullBytesCount == 2)
                    {
                        uint digit = (uint)((byte)(~data[dataPos--]) << 8);
                        digit |= ((byte)(~data[dataPos--]));
                        digits[digitPos--] = digit;
                    }
                    else if (nullBytesCount == 3)
                    {
                        digits[digitPos--] = ((byte)(~data[dataPos--]));
                    }

                    while (digitPos > -1)
                    {
                        uint digit = (uint)((byte)(~data[dataPos--]) << 24);
                        digit |= (uint)((byte)(~data[dataPos--]) << 16);
                        digit |= (uint)((byte)(~data[dataPos--]) << 8);
                        digit |= ((byte)(~data[dataPos--]));
                        digits[digitPos--] = digit;
                    }

                    sign = -1;
                    BigIntInternal.AddSingle(ref digits, ref digitLength, 1);
                }
                else
                {
                    if (nullBytesCount == 1)
                    {
                        uint digit = (uint)(data[dataPos--] << 16);
                        digit |= (uint)(data[dataPos--] << 8);
                        digit |= (data[dataPos--]);
                        digits[digitPos--] = digit;
                    }
                    else if (nullBytesCount == 2)
                    {
                        uint digit = (uint)(data[dataPos--] << 8);
                        digit |= (data[dataPos--]);
                        digits[digitPos--] = digit;
                    }
                    else if (nullBytesCount == 3)
                    {
                        digits[digitPos--] = (data[dataPos--]);
                    }

                    while (digitPos > -1)
                    {
                        uint digit = (uint)(data[dataPos--] << 24);
                        digit |= (uint)(data[dataPos--] << 16);
                        digit |= (uint)(data[dataPos--] << 8);
                        digit |= (data[dataPos--]);
                        digits[digitPos--] = digit;
                    }
                    sign = 1;
                }
                return digits;
            }
        }

        private static uint addCarry(ref uint u1, uint u2, uint uCarry)
        {
            ulong uu = (ulong)u1 + u2 + uCarry;
            u1 = (uint)uu;
            return (uint)(uu >> 32);
        }
        private static int countOfZeroBitStart(uint u)
        {
            if (u == 0)
                return 32;

            int cbit = 0;
            if ((u & 0xFFFF0000) == 0)
            {
                cbit += 16;
                u <<= 16;
            }
            if ((u & 0xFF000000) == 0)
            {
                cbit += 8;
                u <<= 8;
            }
            if ((u & 0xF0000000) == 0)
            {
                cbit += 4;
                u <<= 4;
            }
            if ((u & 0xC0000000) == 0)
            {
                cbit += 2;
                u <<= 2;
            }
            if ((u & 0x80000000) == 0)
                cbit += 1;
            return cbit;
        }
        private static void trim(uint[] digits, ref int digitLength)
        {
            while (digits[digitLength - 1] == 0 && digitLength > 1)
                digitLength--;
        }
    }
}
