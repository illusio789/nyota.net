namespace Nyota.Core;

public readonly record struct Quantity(decimal Value, decimal LotSize)
{
    public decimal RoundedToLot() => LotSize <= 0 ? Value : Math.Round(Value / LotSize, 0) * LotSize;
}