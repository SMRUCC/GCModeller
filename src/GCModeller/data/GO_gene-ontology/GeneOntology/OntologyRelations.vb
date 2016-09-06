
''' <summary>
''' The ontologies of GO are structured as a graph, with terms as nodes in the graph and the relations (also know as properties) 
''' between the terms as edges. Just as each term is defined, so the relations between GO terms are also categorized and defined. 
''' This document provides a description of some of the commonly used relationships in GO: 
''' 
''' + is a (is a subtype of); 
''' + part of; 
''' + has part; 
''' + regulates, negatively regulates and positively regulates.
''' 
''' This Set Is Not exhaustive And includes only a subset Of relations used In the GO ontologies, logical definitions And annotations. 
''' For more technical information about relations And their properties used In GO And other ontologies see the OBO Relations Ontology 
''' (RO) And For relationships used In GO annotation extensions, see the GO annotation extension relations file (experimental).
''' </summary>
Public Enum OntologyRelations

    ''' <summary>
    ''' ## The _is a_ relation
    ''' The ``Is a`` relation forms the basic Structure Of GO. If we say A Is a B, we mean that node A Is a subtype Of node B. 
    ''' For example, mitotic cell cycle Is a cell cycle, Or lyase activity Is a catalytic activity.
    ''' It should be noted that Is a does Not mean 'is an instance of’. An ‘instance’, ontologically speaking, is a specific 
    ''' example of something; e.g. a cat is a mammal, but Garfield is an instance of a cat, rather than a subtype of cat. GO, 
    ''' like most ontologies, does not use instances, and the terms in GO represent a class of entities or phenomena, rather 
    ''' than specific manifestations thereof. However, **if we know that cat is a mammal, we can say that every instance of cat 
    ''' is a mammal.**
    '''
    ''' ## Reasoning over _is a_
    ''' ```
    ''' Is a ∘Is a → Is a
    ''' ```
    '''
    ''' The ``Is a`` relation Is transitive, which means that If A Is a B, And B Is a C, we can infer that A Is a C.
    ''' 
    ''' (child is a parent class object.)
    ''' </summary>
    is_a
    ''' <summary>
    ''' ## The _part of_ relationship
    ''' The relation **_part of_** Is used To represent part-whole relationships In the Gene Ontology. part Of has a specific meaning In GO, 
    ''' And a part Of relation would only be added between A And B If B Is necessarily part Of A: wherever B exists, it Is As part Of A, 
    ''' And the presence Of the B implies the presence Of A. However, given the occurrence Of A, we cannot say For certain that B exists.
    ''' 
    ''' ## Reasoning over part of
    ''' ```
    ''' part Of∘ part Of → part Of
    ''' ```
    ''' Like Is a, part of Is transitive: If A part Of B part Of C Then A part Of C
    ''' </summary>
    part_of
    ''' <summary>
    ''' ## The _has part_ relationship
    ''' The logical complement To the part Of relation Is has part, which represents a part-whole relationship from the perspective Of the parent. 
    ''' As With part Of, the GO relation has part Is only used In cases where A always has B As a part, i.e. where A necessarily has part B. 
    ''' If A exists, B will always exist; however, If B exists, we cannot say For certain that A exists. i.e. all A have part B; some B part Of A.
    ''' 
    ''' ## Reasoning over has part
    ''' ```
    ''' has part ∘has part → has part
    ''' ```
    ''' has part Is a transitive relation; If A has part B, And B has part C, we can infer that A has part C.
    ''' </summary>
    has_part
    ''' <summary>
    ''' ## The _regulates_ relation
    ''' Another common relationship In the Gene Ontology Is that where one process directly affects the manifestation Of another process Or quality, 
    ''' i.e. the former regulates the latter. The target Of the regulation may be another process—For example, regulation Of a pathway Or an enzymatic 
    ''' reaction—Or it may be a quality, such As cell size Or pH. Analogously To part Of, this relation Is used specifically To mean necessarily 
    ''' regulates: If both A And B are present, B always regulates A, but A may Not always be regulated by B.
    ''' i.e. all B regulate A; some A regulated by B.
    ''' </summary>
    regulates
    positively_regulates
    negatively_regulates
End Enum
