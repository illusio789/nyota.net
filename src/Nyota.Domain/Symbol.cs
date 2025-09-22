
// Domain

namespace Nyota.Domain
{
    public readonly record struct Symbol(string Value)
    {
        public override string ToString() => Value ?? string.Empty;
        public static Symbol Normalize(string s) => new((s ?? string.Empty).Trim().ToUpperInvariant());
    }
}
