fac(x:int):int {
    if(x=0) then {1 + 0} else {fac(x-1) * x}
}

x := fac(5)

Print(x)