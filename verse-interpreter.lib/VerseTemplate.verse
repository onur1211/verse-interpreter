hanoi(x:int, start:string, end:string, temp:string):void {
    if(x>0) 
        then {
            hanoi(x - 1, start, temp, end)
            Print("Moved disk from" + start + " to " + end)
            hanoi(x - 1, temp, end, start)
        }
        else {
             Print("")
        }
}

hanoi(3, "A", "B", "C")