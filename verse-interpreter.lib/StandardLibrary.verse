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

head(array:collection):int {
    array[0]+0
}