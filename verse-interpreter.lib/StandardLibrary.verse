fac(x:int):int {
    if(x=0) then { 1 + 0 } else { fac(x-1) * x}
}

head(x:collection):int {
    x[0] + 0
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

length(x:collection, counter:int):int {
    if(!x[counter]) 
        then {
            0 + 0
        }
        else {
            1 + length(x, counter + 1)
        }
}