# Gets the first element of the collection for ints
headInt(x:collection):int {
    x[0]
}

# Gets the first element of the collection for strings
headString(x:collection):string{
    x[0]
}

# Concatenates two collections
append(x:collection, y:collection):collection {
    for{a:int; b:int; x[a] | y[b] }
}

# Get the tail (excluding the first element) of the collection
tail(x:collection):collection {
    for{i:int; i>0; x[i]}
}

# Construct a new collection with the given element as the head
cons(x:int, xs:collection):collection{
    for{i:int; x | xs[i]}
}

# Add an element to the end of the collection
snoc(xs:collection, x:int):collection{
    for{i:int; xs[i] | x}
}