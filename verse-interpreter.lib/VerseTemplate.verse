data Person = Person 
{
    name:string
    age:int
}

x := instance Person()
x.age = 65
x.name = "Markus Safar"

x.age + 25