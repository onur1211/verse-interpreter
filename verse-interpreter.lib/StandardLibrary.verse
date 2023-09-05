fac(x:int):int {
    if(x=0) { 1 + 0 } else { fac(x-1) * x}
}

headInt(x:collection):int {
    x[0]
}

headString(x:collection):string{
    x[0]
}

append(x:collection, y:collection):collection {
    for{a:int; b:int; x[a] | y[b] }
}

tail(x:collection):collection {
    for{i:int; i>0; x[i]}
}

cons(x:int, xs:collection):collection{
    for{i:int; x | xs[i]}
}

snoc(xs:collection, x:int):collection{
    for{i:int; xs[i] | x}
}

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

hanoi(n:int, first:string, second:string, third:string):void {
     if(n > 0) {
	    hanoi(n - 1, first, third, second)
        Print("Moved disk from " + first + " to " + third)
        hanoi(n - 1, second, third, first)
	}
}