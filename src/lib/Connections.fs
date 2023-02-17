namespace Friends.Lib

module Connections =
    
    /// Allows a graph to contain groups as well as persons
    type GraphNode =
        | Group of Group.Group
        | Person of Person.Person
    
    /// Describes a connection stored on the social graph
    type Connection =
        | MemberOf
        | HasMember
        | Values of (RelationshipRenown.T * RelationshipScore.T)

