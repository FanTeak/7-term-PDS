namespace PDS3.Commands.MD5;

public class MessageDigest
{
    internal static MessageDigest InitialValue { get; }

    static MessageDigest()
    {
        InitialValue = new MessageDigest
        {
            A = MD5C.A_MD_BUFFER_INITIAL,
            B = MD5C.B_MD_BUFFER_INITIAL,
            C = MD5C.C_MD_BUFFER_INITIAL,
            D = MD5C.D_MD_BUFFER_INITIAL
        };
    }

    public UInt32 A { get; set; }

    public UInt32 B { get; set; }

    public UInt32 C { get; set; }

    public UInt32 D { get; set; }

    public Byte[] ToByteArray()
    {
        return ConcatArrays(
            BitConverter.GetBytes(A),
            BitConverter.GetBytes(B),
            BitConverter.GetBytes(C),
            BitConverter.GetBytes(D));
    }

    public MessageDigest Clone()
    {
        return MemberwiseClone() as MessageDigest;
    }

    internal void MD5IterationSwap(UInt32 F, UInt32[] X, UInt32 i, UInt32 k)
    {
        var tempD = D;
        D = C;
        C = B;
        B += LeftRotate(A + F + X[k] + MD5C.T[i], MD5C.S[i]);
        A = tempD;
    }

    public override String ToString()
    {
        return $"{ToByteString(A)}{ToByteString(B)}{ToByteString(C)}{ToByteString(D)}";
    }

    public override Int32 GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public override Boolean Equals(Object value)
    {
        return value is MessageDigest md
            && (GetHashCode() == md.GetHashCode() || ToString() == md.ToString());
    }

    public static MessageDigest operator +(MessageDigest left, MessageDigest right)
    {
        return new MessageDigest
        {
            A = left.A + right.A,
            B = left.B + right.B,
            C = left.C + right.C,
            D = left.D + right.D
        };
    }

    private static string ToByteString(UInt32 x)
    {
        return string.Join(string.Empty, BitConverter.GetBytes(x).Select(y => y.ToString("x2")));
    }

    /// <summary>
    /// Shifts value bits for <paramref name="shiftValue"/>
    /// </summary>
    public static UInt32 LeftRotate(UInt32 value, Int32 shiftValue)
    {
        return (value << shiftValue) | (value >> (Int32)(MD5C.BitsPerByte * MD5C.BytesPer32BitWord - shiftValue));
    }

    public static T[] ConcatArrays<T>(params T[][] arrays)
    {
        var position = 0;
        var outputArray = new T[arrays.Sum(a => a.Length)];

        foreach (var a in arrays)
        {
            Array.Copy(a, 0, outputArray, position, a.Length);
            position += a.Length;
        }

        return outputArray;
    }
}