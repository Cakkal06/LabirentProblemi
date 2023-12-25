using System;
using System.Collections.Generic;

struct State
{
    public int X;
    public int Y;
}

internal class QTable
{
    public Dictionary<(State state, int action), double> Table = new Dictionary<(State, int action), double>();

    // Q değerini güncelle
    public void Update(State state, int action, double newValue)
    {
        var key = (state, action);

        if (Table.ContainsKey(key))
        {
            // Eğer varsa, değeri güncelle
            Table[key] = newValue;
        }
        else
        {
            // Yoksa, yeni bir giriş ekle
            Table.Add(key, newValue);
        }
    }

    // Belirli bir durum ve eylem çiftine karşılık gelen Q değerini getir
    public double Get(State state, int action)
    {
        var key = (state, action);

        // Eğer varsa, değeri getir; yoksa, 0 döndür
        return Table.TryGetValue(key, out var value) ? value : 0.0;
    }

    // Belirli bir durum için en yüksek Q değerine sahip eylemi ve değeri getir
    public (int action, double value) GetMax(State state)
    {
        var maxAction = -1;
        var maxValue = double.MinValue;

        foreach (var entry in Table)
        {
            var key = entry.Key;
            var value = entry.Value;

            if (key.Item1.Equals(state) && value > maxValue)
            {
                maxAction = key.Item2;
                maxValue = value;
            }
        }

        return (maxAction, maxValue);
    }
}