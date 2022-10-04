namespace Friends.Lib

module Relationship =

    type Relationship<'toward> = {
        Score: RelationshipScore.T
        Towards: 'toward
    }
    
