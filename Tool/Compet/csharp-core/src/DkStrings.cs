namespace Tool.Compet.Core {
	using System.Runtime.CompilerServices;

	/// Extension for string.
	public static class DkStrings {
		/// Compare 2 strings with `Ordinal` comparision (byte-level).
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EqualsDk(this string? me, string? other) {
			return me == null ? other == null : me.Equals(other, System.StringComparison.Ordinal);
		}

		/// Compare 2 strings with `OrdinalIgnoreCase` comparision (byte-level).
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EqualsIgnoreCaseDk(this string? me, string? other) {
			return me == null ? other == null : me.Equals(other, System.StringComparison.OrdinalIgnoreCase);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEmptyDk(this string? me) {
			return me == null || me.Length == 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNotEmptyDk(this string? me) {
			return me != null && me.Length > 0;
		}

		/// Check this string starts with given `value` by comparing as `Ordinal` (byte-level).
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool StartsWithDk(this string me, string value) {
			return me.StartsWith(value, System.StringComparison.Ordinal);
		}

		/// Check this string ends with given `value` by comparing as `Ordinal` (byte-level).
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EndsWithDk(this string me, string value) {
			return me.EndsWith(value, System.StringComparison.Ordinal);
		}

		/// @param startIndex: Inclusive.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string SubstringDk(this string me, int startIndex) {
			return me.Substring(startIndex, me.Length - startIndex);
		}

		/// @param startIndex: Inclusive.
		/// @param endIndex: Exclusive.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string SubstringDk(this string me, int startIndex, int endIndex) {
			return me.Substring(startIndex, endIndex - startIndex);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ParseBooleanDk(this string? me, bool defaultValue = false) {
			if (me.EqualsDk("1") || me.EqualsIgnoreCaseDk("true")) {
				return true;
			}
			return defaultValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ParseByteDk(this string? me, byte defaultValue = 0) {
			if (byte.TryParse(me, out var result)) {
				return result;
			}
			return defaultValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short ParseShortDk(this string? me, short defaultValue = 0) {
			if (short.TryParse(me, out var result)) {
				return result;
			}
			return defaultValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ParseIntDk(this string? me, int defaultValue = 0) {
			if (int.TryParse(me, out var result)) {
				return result;
			}
			return defaultValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ParseLongDk(this string? me, long defaultValue = 0L) {
			if (long.TryParse(me, out var result)) {
				return result;
			}
			return defaultValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal ParseDecimalDk(this string? me, decimal defaultValue = 0m) {
			if (decimal.TryParse(me, out var result)) {
				return result;
			}
			return defaultValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ParseFloatDk(this string? me, float defaultValue = 0F) {
			if (float.TryParse(me, out var result)) {
				return result;
			}
			return defaultValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ParseDoubleDk(this string? me, double defaultValue = 0D) {
			if (double.TryParse(me, out var result)) {
				return result;
			}
			return defaultValue;
		}

		/// Get first index of given `element` in the array. Returns -1 if not found.
		public static int IndexOfDk(this string[] arr, string? element) {
			for (int index = 0, N = arr.Length; index < N; ++index) {
				if (arr[index].Equals(element, System.StringComparison.Ordinal)) {
					return index;
				}
			}
			return -1;
		}

		/// Get last index of given `element` in the array. Returns -1 if not found.
		public static int LastIndexOfDk(this string[] arr, string? element) {
			for (var index = arr.Length - 1; index >= 0; --index) {
				if (arr[index].Equals(element, System.StringComparison.Ordinal)) {
					return index;
				}
			}
			return -1;
		}
	}
}
