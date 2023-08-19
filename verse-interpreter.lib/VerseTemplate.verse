sort(x:collection):collection{
     if(head := head(x)) {
         xs := tail(x)
         smaller := sort(for{i:int; xs[i] <= head; xs[i]})
         larger := sort(for{i:int; xs[i] > head; xs[i]})
         append(append(smaller, for{head}), larger)
     }
     else {
         array()
     }
}

test():collection {
    array()
}

x := array(73, 42, 18, 96, 57, 23, 68, 35, 14, 81, 29, 50, 10, 62, 8, 97, 46, 55, 21, 88, 37, 4, 79, 91, 63, 26, 75, 3, 69, 44, 12, 71, 30, 85, 16, 92, 49, 7, 59, 32, 67, 39, 89, 1, 54, 11, 64, 28, 86, 17, 99, 58, 19, 82, 24, 70, 38, 9, 51, 2, 93, 40, 80, 15, 87, 56, 25, 98, 45, 76, 33, 74, 6, 31, 83, 48, 13, 60, 22, 72, 5, 90, 34, 77, 20, 84, 47, 27, 65, 41, 94, 78, 36, 61, 43, 66, 95, 52, 0, 53)
y := sort(x)