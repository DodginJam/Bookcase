using System;
using UnityEngine;

[Serializable]
public class Bookshelf
{
    [field: SerializeField]
    public int NumberOfBooks
    { get; set; }

    [field: SerializeField]
    public float ShelfWidth
    { get; set; }
}
