
using System;

[Serializable]
public struct EncryptLong : IFormattable, IEquatable<EncryptLong>, IComparable<EncryptLong>, IComparable<long>, IComparable
{
	#region core

	private long? encryptValue;
	private long EncryptValue
	{
		get
		{
			if (encryptValue == null)
			{
				encryptValue = Encrypt(0);
			}

			return encryptValue.Value;
		}
	}

	const long encryptKey = 6590428027289101611;

	public EncryptLong(long val)
	{
		encryptValue = Encrypt(val);
	}

	#endregion

	#region encrypt

	private static long Encrypt(long val)
	{
		return val ^ encryptKey;
	}

	private static long Decrypt(long val)
	{
		return val ^ encryptKey;
	}

	#endregion

	#region implement IComparable

	public int CompareTo(EncryptLong other)
	{
		return Decrypt(EncryptValue).CompareTo(Decrypt(other.EncryptValue));
	}

	public int CompareTo(long other)
	{
		return Decrypt(EncryptValue).CompareTo(other);
	}

	public int CompareTo(object obj)
	{
		return Decrypt(EncryptValue).CompareTo(obj);
	}

	#endregion

	#region implement IEquatable<EncryptLong>

	public override int GetHashCode()
	{
		return Decrypt(EncryptValue).GetHashCode();
	}

	public override bool Equals(object obj)
	{
		return obj is EncryptLong && Equals((EncryptLong)obj);
	}

	public bool Equals(EncryptLong other)
	{
		return EncryptValue.Equals(other.EncryptValue);
	}

	#endregion

	#region implement IFormattable

	public override string ToString()
	{
		return Decrypt(EncryptValue).ToString();
	}

	public string ToString(string format, IFormatProvider formatProvider)
	{
		return Decrypt(EncryptValue).ToString(format, formatProvider);
	}

	#endregion

	#region operator overloading

	public static implicit operator EncryptLong(long val)
	{
		return new EncryptLong(val);
	}

	public static implicit operator long(EncryptLong val)
	{
		return Decrypt(val.EncryptValue);
	}

	#endregion
}