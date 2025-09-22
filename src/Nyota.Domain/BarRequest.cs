using System;

namespace Nyota.Domain;

public sealed record BarRequest(
    Instrument Instrument,
    DateTime StartUtc,
    DateTime EndUtc,
    Resolution Resolution,
    bool AdjustForCorporateActions);
