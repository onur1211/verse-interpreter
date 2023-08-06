hanoi(count:int, start:string, end:string, temp:string):void {
    if(count > 0)
        then {
            hanoi(count - 1, start, temp, end)
            Print("Moved disk from " + start + " to " + end)
            hanoi(count - 1, temp, end, start)
        }
        else {
            Print("")
        }
}

fac(x:int):int {
    if(x=0) then { 1 + 0 } else { fac(x-1) * x}
}

head(x:collection):int {
    x[0]+0
}

append(x:collection, y:collection):collection {
    for{a:int; b:int; x[a] | y[b] }
}

tail(x:collection):collection {
    for{i:int; i>0; x[i]}
}
