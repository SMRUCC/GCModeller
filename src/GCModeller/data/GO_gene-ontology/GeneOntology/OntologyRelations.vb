#Region "Microsoft.VisualBasic::1288d88cd11a9cac40a3beb50fdcb937, GCModeller\data\GO_gene-ontology\GeneOntology\OntologyRelations.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 103
    '    Code Lines: 14
    ' Comment Lines: 86
    '   Blank Lines: 3
    '     File Size: 5.37 KB


    ' Enum OntologyRelations
    ' 
    '     ends_during, happens_during, has_part, negatively_regulates, none
    '     occurs_in, part_of, positively_regulates, regulates
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
Public Enum OntologyRelations As Byte

    ''' <summary>
    ''' 两个词条之间无任何关系
    ''' </summary>
    none

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
    ''' <remarks>
    ''' 这个直接表示两个term之间在归属上面的关系，右边相对于左边而言是一个更加宽泛的概念，例如：
    ''' 
    ''' ```
    ''' a is_a b
    ''' 
    ''' [RNA localization] is_a [macromolecule localization]
    ''' [protein localization] is_a [macromolecule localization]
    ''' ```
    ''' 
    ''' RNA定位和蛋白质定位，由于RNA和蛋白质都属于生物大分子，所以二者都是生物大分子定位的概念。
    ''' 生物大分子定位相对于RNA定位或者蛋白定位而言，是一个更加宽泛的概念。
    ''' 
    ''' 由于一个term也有可能会从属于多个概念，所以通过``is_a``可以构成一个有向无环图
    ''' </remarks>
    is_a = 1
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

#Region "biological process events"
    ''' <summary>
    ''' biological process
    ''' </summary>
    happens_during
    occurs_in
    ends_during
#End Region
End Enum
