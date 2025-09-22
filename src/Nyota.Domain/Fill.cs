using System;

namespace Nyota.Domain;

public sealed record Fill(DateTime Utc, Price Price, Quantity Quantity);
