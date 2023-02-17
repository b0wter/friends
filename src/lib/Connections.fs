namespace Friends.Lib

module Connections =
    
    type GraphNode =
        | Group of Group.Group
        | Person of Person.Person
    
    type Connection =
        | MemberOf
        | HasMember
        | Values of (RelationshipRenown.T * RelationshipScore.T)

