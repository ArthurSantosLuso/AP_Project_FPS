using System;

public interface IDamageNotifier
{
    event Action<float> Damaged;
}