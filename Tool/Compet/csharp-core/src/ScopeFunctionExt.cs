namespace Tool.Compet.Core {
	/// Cleaner coding style with scope functions.
	/// Ref: Kotlin's also(), let(),...
	public static class ScopeFunctionExt {
		/// Use it to do more stuffs before return itself.
		public static T AlsoDk<T>(this T self, System.Action<T> block) {
			block(self);
			return self;
		}

		/// This is same as `Also()`, but returns other type.
		public static R LetDk<T, R>(this T self, System.Func<T, R> block) {
			return block(self);
		}
	}
}
