using Bogus;

namespace SpectraProcessing.TestingInfrastructure;

public static class Creator
{
    private static readonly Faker Faker = new();
    private static readonly Random Random = new();
    private static long _longCounter = DateTime.UtcNow.ToUnixTimeMilliseconds();
    private static int _intCounter = Math.Abs((int) _longCounter);

    public static object RandomObject() => new();

    public static string UniqueString() => UniquePositiveLong().ToString();

    public static byte RandomByte() => Faker.Random.Byte();

    public static sbyte RandomSByte() => Faker.Random.SByte();

    public static long UniquePositiveLong() => Interlocked.Increment(ref _longCounter);

    public static long RandomLong() => Faker.Random.Long();

    public static ulong RandomULong() => Faker.Random.ULong();

    public static int UniquePositiveInt() => Interlocked.Increment(ref _intCounter);

    public static int RandomInt() => Faker.Random.Int();

    public static uint RandomUInt() => Faker.Random.UInt();

    public static short RandomShort() => Faker.Random.Short();

    public static ushort RandomUShort() => Faker.Random.UShort();

    public static float RandomFloat() => Faker.Random.Float();

    public static double RandomDouble() => Faker.Random.Double();

    public static char RandomChar() => Faker.Random.Char();

    public static bool RandomBool() => Faker.Random.Bool();

    public static Decimal RandomDecimal() => Faker.Random.Decimal();

    public static DateTimeOffset RandomDateTime()
    {
        DateTime dateTime = new DateTime(2000, 1, 1);
        int days = (DateTime.Today - dateTime).Days;
        return dateTime.AddDays(Random.Next(days)).ToUniversalTime();
    }

    public static Guid RandomGuid() => Guid.NewGuid();

    public static TEnum RandomEnum<TEnum>(params TEnum[] exclude) where TEnum : struct, Enum
    {
        return Faker.PickRandomWithout(exclude);
    }

    private static long ToUnixTimeMilliseconds(this DateTime utcTimestamp)
    {
        return (utcTimestamp.Ticks - 621355968000000000L) / 10L;
    }
}
