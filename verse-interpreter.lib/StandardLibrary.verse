# Factorial function
fac(x:int):int {
    if(x=0) { 1 + 0 } else { fac(x-1) * x}
}

# TEst
headInt(x:collection):int {
    x[0]
}

# Test
headString(x:collection):string{
    x[0]
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

# Was
sort(x:collection):collection{
     if(head := headInt(x)) {
         xs := tail(x)
         smaller := sort(for{i:int; xs[i] <= head; xs[i]})
         larger := sort(for{i:int; xs[i] > head; xs[i]})
         append(append(smaller, for{head}), larger)
     }
     else{
         array()
     }
}

# BRuder
diff(x:collection, y:collection):collection {
     if(h1 := headString(x)) {
         if(h2 := headString(y)){
            if(h1 = h2) {
                next := diff(tail(x), tail(y))
                append(for{1}, next)
            }
            else {
                next := diff(tail(x), tail(y))
                append(for{0}, next)
            }
         }
     }
}

# TEst
hanoi(n:int, start:string, end:string, temp:string):void {
     if(n > 0) {
	    hanoi(n - 1, start, temp, end)
        Print("Moved disk from " + start + " to " + end)
        hanoi(n - 1, temp, end, start)
	}
}
