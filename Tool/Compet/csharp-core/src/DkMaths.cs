namespace Tool.Compet.Core {
	using System;

	public class DkMaths {
		/// Fast pow with O(logN) time.
		/// Method 1: x^9 = x^0 * x^1 * x^0 * x^0 * x^8. Note: 9 = 1001.
		/// Method 2: x^9 = x^4 * x^4 * x
		public static long Pow(long x, int n) {
			if (n < 0) {
				var ans = Pow(x, -n);
				return ans == 0 ? 0 : 1 / ans;
			}
			if (n == 0) {
				return 1;
			}
			if (n == 1) {
				return x;
			}

			var result = 1L;
			while (n > 0) {
				// Mul if meet bit 1
				if ((n & 1) == 1) {
					result = (result * x); // Mod here
				}
				// Down n and Up x
				n >>= 1;
				x = (x * x); // Mod here
			}

			return result;
		}

		/// Calculate polynomial function: y(x) = c + x * (b + x * (a + x * 0))
		public static long Poly(long[] arr, long x) {
			var result = 0L;
			var N = arr.Length;
			for (var index = 0; index < N; ++index) {
				result = arr[index] + x * result;
			}
			return result;
		}

		public static bool IsDigit(char ch) {
			return '0' <= ch && ch <= '9';
		}
	}
}
