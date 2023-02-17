namespace Friends.Lib.Tests.Samples

open Friends.Lib

module Person =
    
    let dummy1 = Person.Factory.Create (18u, "Justus", lastName = "Jonas")
    let dummy2 = Person.Factory.Create (19u, "Peter", lastName = "Shaw")
    let dummy3 = Person.Factory.Create (20u, "Bob", lastName = "Andrews")

module Group =
    
    let dummy1 = Group.Factory.Create "Drei ???"
    let dummy2 = Group.Factory.Create "Dummy Group 2"
    let dummy3 = Group.Factory.Create "Dummy Group 3"
