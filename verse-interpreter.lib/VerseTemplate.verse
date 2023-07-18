# This is my hanoi function in Verse code

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

#* Summary
This is a
multiline
comment
End *#

hanoi(3, "A", "B", "C")