# Solves the towers of hanoi problem using three pegs and an arbitrary number of disks.
hanoi(n:int, start:string, end:string, temp:string):void {
     if(n > 0) {
	    hanoi(n - 1, start, temp, end)
        Print("Moved disk from " + start + " to " + end)
        hanoi(n - 1, temp, end, start)
	}
}

hanoi(3, "A", "B", "C")