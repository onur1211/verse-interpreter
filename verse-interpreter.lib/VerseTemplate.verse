a := array("Was", "Bruder", "Nibba")
b := array("Was", "Bruder", "Nigga")

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

z := array(5,3,5, 612, 302, 250, 566, 152, 3, 120, 0)
t := sort(z)