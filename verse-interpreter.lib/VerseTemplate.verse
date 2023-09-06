#* sort(x:collection, counter:int):collection{
     lower := sort(for{a:int; x[counter] <= x[a]; x[a]}, counter + 1)
     larger := sort(for{a:int; x[counter] > x[a]; x[a]}, counter + 1)
     append(lower, larger)
}

x := array(5,3,2,7,1)
y := array(123123,22)
z := cons(4, x) *#


x:="Hello"
y:="World"
z:=x+y
myArray:=array(1,2,3)
test:=false?