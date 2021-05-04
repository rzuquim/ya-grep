namespace YAGrep {
    /// <summary>
    /// Stores iteration details. <br/> To ensure default properties, prefer using <see cref="Start"/>.
    /// <code>
    /// var i= Iteration.Start()
    /// doSomething()
    /// i = i.Next(isLast)
    /// </code>
    /// </summary>
    internal readonly struct Iteration {
        /// <summary>Is is the first iteration?</summary>
        public bool First { get; }

        /// <summary>Is is the last iteration?</summary>
        public bool Last { get; }

        /// <summary>Current iteration index.</summary>
        public int Index { get; }

        private Iteration(bool last, int index = 0, bool first = true) => (Index, Last, First) = (index, last, first);

        public override string ToString() => Index.ToString();

        /// <summary>Creates a new <see cref="Iteration"/> with the correct default values.</summary>
        public static Iteration Start(int? max = null) => new Iteration(last: max != null && max <= 1);

        public Iteration Next(bool isLast) => new Iteration(isLast, Index + 1, false);

        public Iteration AsLast() => new Iteration(last: true, Index, First);
    }
}
