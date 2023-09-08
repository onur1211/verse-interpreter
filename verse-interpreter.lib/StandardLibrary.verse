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

isSorted(x:collection):int {
  if(h := headInt(x)) {
      if(t := tail(x)) {
          if(h1 := headInt(t)) {
              if(h > h1) {
                 isSorted(t)
              }
              else {
                 1 + 0
              }
          }
      }
  }
}

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

hanoi(n:int, start:string, end:string, temp:string):void {
     if(n > 0) {
	    hanoi(n - 1, start, temp, end)
        Print("Moved disk from " + start + " to " + end)
        hanoi(n - 1, temp, end, start)
	}
}