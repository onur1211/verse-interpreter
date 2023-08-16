sort(x:collection):collection{
     xs := tail(x)
     head := head(x)
     smaller := for{i:int; xs[i] <= head; xs[i]}
     larger := for{i:int; xs[i] > head; xs[i]}
     append(smaller, larger)
}

x := array(6,3,1,6,8)
xs := tail(x)
head := head(x)
smaller := for{i:int; xs[i] <= head; xs[i]}
larger := for{i:int; xs[i] > head; xs[i]}

test := append(smaller, larger)