using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class StaticUtils
{
	#region check point inside triangle

	private static float IsPointInsideTriangle_sign(Vector2 p1, Vector2 p2, Vector2 p3)
	{
		return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
	}

	public static bool IsPointInsideTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
	{
		var d1 = IsPointInsideTriangle_sign(pt, v1, v2);
		var d2 = IsPointInsideTriangle_sign(pt, v2, v3);
		var d3 = IsPointInsideTriangle_sign(pt, v3, v1);

		var has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
		var has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

		return !(has_neg && has_pos);
	}

	#endregion

	#region bezier interpolate

	public static float CalculateBezierCurveLength(List<Vector2> lPoints, int numSamples = 100)
	{
		float length = 0.0f;
		Vector3 prevPoint = lPoints[0];
		for (int i = 1; i <= numSamples; i++)
		{
			float t = i / (float)numSamples;
			Vector3 currentPoint = BezierInterpolate(t, lPoints);
			length += Vector3.Distance(prevPoint, currentPoint);
			prevPoint = currentPoint;
		}
		return length;
	}

	public static Vector2 BezierInterpolate(float t, List<Vector2> lPoints)
	{
		if (t <= 0)
		{
			return lPoints[0];
		}
		if (t >= 1)
		{
			return lPoints[lPoints.Count - 1];
		}

		var p = new Vector2();
		var n = lPoints.Count - 1;
		for (int i = 0; i < lPoints.Count; ++i)
		{
			p += BernsteinPolynomial(n, i, t) * lPoints[i];
		}
		return p;
	}

	private static float BernsteinPolynomial(int n, int i, float t)
	{
		return (float)(Combinations(n, i) * Pow(t, i) * Pow(1 - t, n - i));
	}
	
	#endregion

	#region pow

	public static double Pow(double x, int pow)
	{
		return Pow(x, pow, (x, y) => x * y, 1d);
	}

	public static double Pow(float x, int pow)
	{
		return Pow((double)x, pow);
	}

	public static long Pow(long x, int pow)
	{
		return Pow(x, pow, (x, y) => x * y, 1L);
	}

	public static long Pow(int x, int pow)
	{
		return Pow((long)x, pow);
	}

	private static T Pow<T>(T x, int pow, Func<T, T, T> mul, T numberOne)
	{
		T ret = numberOne;
		while (pow != 0)
		{
			if ((pow & 1) == 1)
			{
				ret = mul(ret, x);
			}
			x = mul(x, x);
			pow >>= 1;
		}
		return ret;
	}

	#endregion

	#region other utils

	public static long Factorial(int n)
	{
		var f = 1L;
		for (var i = 1; i <= n; i++)
		{
			f *= i;
		}
		return f;
	}

	public static long Combinations(int n, int r)
	{
		var t = Factorial(n);
		t /= Factorial(r);
		t /= Factorial(n - r);
		return t;
	}

	//674.89
	//locations: 2 1 0 . -1 -2
	public static int GetDigitAtLocation(double d, int location)
	{
		if (location >= 0)
		{
			var shift = Pow(10d, location);
			d /= shift;
			return (int)(d % 10);
		}
		else
		{
			var intPart = 0d;
			var nDigits = GetNumberOfDigits(d);
			for (var i = nDigits - 1; i >= 0; i--)
			{
				var digit = GetDigitAtLocation(d, i);
				intPart = intPart * 10 + digit;
			}
			var decimalPointPart = d - intPart;

			var shift = Pow(10d, -location);
			decimalPointPart *= shift;
			return (int)(decimalPointPart % 10);
		}
	}

	public static int GetNumberOfDigits(double d)
	{
		if (d < 1)
		{
			return 1;
		}

		var num = 0;
		var t = 1d;
		while (t <= d)
		{
			t *= 10;
			num++;
		}
		return num;
	}

	#endregion
}