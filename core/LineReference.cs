namespace YAGrep {
    public readonly struct LineReference {
        private readonly char[] _buffer;
        private readonly int _startIndex;

        public int Length { get; }

        public LineReference(string line) {
            _buffer = line.ToCharArray();
            _startIndex = 0;
            Length = _buffer.Length;
        }

        public LineReference Snapshot() => new(this.AsString());

        public string AsString() => new(_buffer, _startIndex, Length);
    }
}
