using System;

namespace NetworkIO.Numerics
{
    public class Epsilon : IComparable<Epsilon>
    {
        private int _exponent;
        private const double logQuot = 3.32192809488736d; // => Log base 2 of 10;

        public int BinarySensitive { get { return this._exponent; } }
        public int DecimalSensitive { get { return (int)Math.Floor(this.BinarySensitive / Epsilon.logQuot); } }

        public Epsilon(int decimalSens)
        {
            this._exponent = (int)Math.Ceiling((decimalSens) * Epsilon.logQuot);
        }
        public Epsilon(int sens, uint radix)
        {
            if (radix == 2)
                this._exponent = sens;
            else if (radix == 4)
                this._exponent = sens << 1;
            else if (radix == 8)
                this._exponent = sens * 3;
            else if (radix == 10)
                this._exponent = (int)Math.Ceiling((sens) * Epsilon.logQuot);
            else if (radix == 16)
                this._exponent = sens * 4;
            else
                this._exponent = (int)Math.Ceiling((sens) * Math.Log(radix, 2));
        }

        public int CompareTo(Epsilon other)
        {
            return other._exponent.CompareTo(this._exponent);
        }
        public BigDecimal ToFloat()
        {
            UnsignedBigInteger twoPower = UnsignedBigInteger.TwoPower(this._exponent);
            return new BigDecimal(BigInteger.One, twoPower);
        }
        public override string ToString()
        {
            return "0";
        }
        public override bool Equals(object obj)
        {
            return this.CompareTo((Epsilon)obj) == 1;
        }
        public override int GetHashCode()
        {
            return this._exponent.GetHashCode();
        }

        public static Epsilon operator +(Epsilon left, Epsilon right)
        {
            if (right._exponent == left._exponent)
                return new Epsilon(right._exponent, 2);
            else
            {
                if (right._exponent < left._exponent)
                    return new Epsilon(right._exponent, 2);
                else
                    return new Epsilon(left._exponent, 2);
            }
        }
        public static Epsilon operator *(Epsilon left, Epsilon right)
        {
            return new Epsilon(left._exponent + right._exponent, 2);
        }

        public static bool operator ==(Epsilon left, Epsilon right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(Epsilon left, Epsilon right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(Epsilon left, Epsilon right)
        {
            return left.CompareTo(right) == -1;
        }
        public static bool operator >(Epsilon left, Epsilon right)
        {
            return left.CompareTo(right) == 1;
        }
        public static bool operator <=(Epsilon left, Epsilon right)
        {
            return left.CompareTo(right) < 1;
        }
        public static bool operator >=(Epsilon left, Epsilon right)
        {
            return left.CompareTo(right) > -1;
        }
    }
}
