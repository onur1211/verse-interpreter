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

hanoi(3, "A", "B", "C")