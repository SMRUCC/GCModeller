#Region "Microsoft.VisualBasic::78de0f0cff4c800a1c58b037ec5eb7a0, ..\GCModeller\data\GO_gene-ontology\GeneOntology\DAG\relationship.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace DAG

    Public Structure Relationship

        Public type As OntologyRelations
        Public parent As NamedValue(Of String)
        Public parentName As String

        Sub New(value$)
            Dim tokens$() = Strings.Split(value$, " ! ")

            parentName = tokens(1%)
            tokens = tokens(Scan0).Split
            type = relationshipParser(tokens(Scan0))
            parent = tokens(1).GetTagValue(":")
        End Sub

        Shared ReadOnly relationshipParser As Dictionary(Of String, OntologyRelations) =
            ParserDictionary(Of OntologyRelations)()

        Public Overrides Function ToString() As String
            Return $"relationship: {type.ToString} {parent.Name}:{parent.Value} ! {parentName}"
        End Function
    End Structure

    ''' <summary>
    ''' The is a relation forms the basic structure of GO. If we say A is a B, we mean that node A is a subtype of node B. 
    ''' For example, mitotic cell cycle is a cell cycle, or lyase activity is a catalytic activity. It should be noted 
    ''' that is a does not mean ‘is an instance of’. An ‘instance’, ontologically speaking, is a specific example of 
    ''' something; e.g. a cat is a mammal, but Garfield is an instance of a cat, rather than a subtype of cat. GO, like 
    ''' most ontologies, does not use instances, and the terms in GO represent a class of entities or phenomena, rather 
    ''' than specific manifestations thereof. However, if we know that cat is a mammal, we can say that every instance of 
    ''' cat is a mammal.
    ''' </summary>
    Public Structure is_a

        Dim term_id$, name$
        ''' <summary>
        ''' 父节点的实例
        ''' </summary>
        Dim term As TermNode

        Sub New(value$)
            Dim tokens$() = Strings.Split(value$, " ! ")

            term_id = tokens(Scan0%)
            name = tokens(1%)
        End Sub

        Public Overrides Function ToString() As String
            Return $"is_a: {term_id} ! {name$}"
        End Function
    End Structure
End Namespace
