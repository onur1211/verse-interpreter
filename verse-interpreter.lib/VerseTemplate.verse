sum(x:collection):int {
     if(head := headInt(x)) {
       subSum := sum(tail(x))
       head + subSum
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

x := array("A", "B")
y := array("A", "B")
zahlen := array(5,41,51,2)
t := sort(zahlen)
Print(t)