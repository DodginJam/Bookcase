using System;
using System.Collections.Generic;
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

    public List<ObjectContainer<Book>> BooksContained
    { get; set; }
}
