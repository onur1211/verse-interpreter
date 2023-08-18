# Factorial function
fac(x:int):int {
    if(x=0) then { 1 + 0 } else { fac(x-1) * x}
}

# Get the first element of the collection
head(x:collection):int {
    x[0]+0
}

# Concatenate two collections
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