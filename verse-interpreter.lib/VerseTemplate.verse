f(x:collection, counter:int):void {
    if(counter < 10) 
        then {
            Print(x[counter])
            f(x, counter + 1)
        }
        else {
            Print("done")
        }
}

x := (1,2,3,4,5,6,7,8,9,10)
f(x, 0)