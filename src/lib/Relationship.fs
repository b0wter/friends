namespace Friends.Lib

module Relationship =
    type Relationship<'from, 'toward> = {
        Score: RelationshipScore.T
        From: 'from
        Towards: 'toward
    }
    
