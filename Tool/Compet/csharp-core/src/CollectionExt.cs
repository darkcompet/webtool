namespace Tool.Compet.Core {
	/// Extension for collection (array, list, set, map,...).
	public static class CollectionExt {
		public static bool IsEmptyDk<T>(this T[]? list) {
			return list == null || list.Length == 0;
		}

		public static bool IsEmptyDk<T>(this List<T>? list) {
			return list == null || list.Count == 0;
		}

		/// This perform optimization by remove last item instead, so array will not be shrinked.
		/// We recommend use this if you don't care item-order after removed.
		public static void FastRemoveDk<T>(this List<T> list, int index) {
			var lastIndex = list.Count - 1;
			if (index >= 0) {
				if (index == lastIndex) {
					list.RemoveAt(index);
				}
				else if (index < lastIndex) {
					list[index] = list[lastIndex];
					list.RemoveAt(lastIndex);
				}
			}
		}
	}
}
