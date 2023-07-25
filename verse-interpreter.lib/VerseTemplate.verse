append(xs:collection, ys:collection):collection {
    for{xs[i:int] | ys[j:int]}
}
x := (1,2,3)
y := (4,5,6)

append(x,y)