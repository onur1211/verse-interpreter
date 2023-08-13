sort(x:collection, counter:int):collection{
     lower := sort(for{a:int; x[counter] <= x[a]; x[a]}, counter + 1)
     larger := sort(for{a:int; x[counter] > x[a]; x[a]}, counter + 1)
     append(lower, larger)
}

x := array(5,3,2,7,1)
y := length(x, 0)
