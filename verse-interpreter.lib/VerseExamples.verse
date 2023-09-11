# Sorts a list of integers using the quicksort approach
sort(x:collection):collection{
     if(head := headInt(x)) {
         xs := tail(x)
         smaller := sort(for{i:int; xs[i] <= head; xs[i]})
         larger := sort(for{i:int; xs[i] > head; xs[i]})
         left := append(smaller, for{head})
         append(left, larger)
     }
     else {
         array()
     }
}

# Checks which elements of two collections of strings match and returns a list of 0s and 1s.
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

# Solves the towers of hanoi problem using three pegs and an arbitrary number of disks.
hanoi(n:int, start:string, end:string, temp:string):void {
     if(n > 0) {
	    hanoi(n - 1, start, temp, end)
        Print("Moved disk from " + start + " to " + end)
        hanoi(n - 1, temp, end, start)
	}
}

# Factorial function
fac(x:int):int {
    if(x=0) { 1 + 0 } else { fac(x-1) * x}
}