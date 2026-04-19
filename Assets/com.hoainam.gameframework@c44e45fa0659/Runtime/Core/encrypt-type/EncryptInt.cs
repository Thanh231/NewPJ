
using System;

[Serializable]
public struct EncryptInt: IFormattable, IEquatable<EncryptInt>, IComparable<EncryptInt>, IComparable<int>, IComparable
{
	#region core

	private int? encryptValue;
	private int EncryptValue
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

	const int encryptKey = 1730848001;

	public EncryptInt(int val)
	{
		encryptValue = Encrypt(val);
	}

	#endregion

	#region encrypt

	private static int Encrypt(int val)
	{
		return val ^ encryptKey;
	}

	private static int Decrypt(int val)
	{
		return val ^ encryptKey;
	}

	#endregion

	#region implement IComparable

	public int CompareTo(EncryptInt other)
	{
		return Decrypt(EncryptValue).CompareTo(Decrypt(other.EncryptValue));
	}

	public int CompareTo(int other)
	{
		return Decrypt(EncryptValue).CompareTo(other);
	}

	public int CompareTo(object obj)
	{
		return Decrypt(EncryptValue).CompareTo(obj);
	}

	#endregion

	#region implement IEquatable<EncryptInt>

	public override int GetHashCode()
	{
		return Decrypt(EncryptValue).GetHashCode();
	}

	public override bool Equals(object obj)
	{
		return obj is EncryptInt && Equals((EncryptInt)obj);
	}

	public bool Equals(EncryptInt other)
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

	public static implicit operator EncryptInt(int val)
	{
		return new EncryptInt(val);
	}

	public static implicit operator int(EncryptInt val)
	{
		return Decrypt(val.EncryptValue);
	}

	#endregion
}