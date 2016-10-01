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
            Return $"relationship: {type.ToString} {parent.Name}:{parent.x} ! {parentName}"
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

        Dim uid$, cat$
        ''' <summary>
        ''' 父节点的实例
        ''' </summary>
        Dim term As Term

        Sub New(value$)
            Dim tokens$() = Strings.Split(value$, " ! ")

            uid = tokens(Scan0%)
            cat = tokens(1%)
        End Sub

        Public Overrides Function ToString() As String
            Return $"is_a: {uid} ! {cat$}"
        End Function
    End Structure
End Namespace