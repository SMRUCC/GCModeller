# is_a
_namespace: [SMRUCC.genomics.Data.GeneOntology.DAG](./index.md)_

The is a relation forms the basic structure of GO. If we say A is a B, we mean that node A is a subtype of node B. 
 For example, mitotic cell cycle is a cell cycle, or lyase activity is a catalytic activity. It should be noted 
 that is a does not mean ‘is an instance of’. An ‘instance’, ontologically speaking, is a specific example of 
 something; e.g. a cat is a mammal, but Garfield is an instance of a cat, rather than a subtype of cat. GO, like 
 most ontologies, does not use instances, and the terms in GO represent a class of entities or phenomena, rather 
 than specific manifestations thereof. However, if we know that cat is a mammal, we can say that every instance of 
 cat is a mammal.




### Properties

#### term
父节点的实例
