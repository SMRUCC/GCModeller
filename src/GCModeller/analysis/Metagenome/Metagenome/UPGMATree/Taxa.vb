Imports Microsoft.VisualBasic.Data.GraphTheory
Imports SMRUCC.genomics.Metagenomics

Namespace UPGMATree

    ''' <summary>
    ''' Taxonomy tree model
    ''' </summary>
    ''' <remarks>
    ''' is a sub-class of the abstract tree base class 
    ''' </remarks>
    Public Class Taxa : Inherits Tree(Of Value)

        ' tree base class type has properties:
        '
        '       id - the unique reference id of the tree model,
        '    label - the display name label of the tree model,
        '   childs - the child tree nodes, is a dictionary of tree node, key is the label and value is the taxa object
        ' and data - the node data, class of value{size, distance}

        ''' <summary>
        ''' <see cref="Value.size"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Double
            Get
                Return Data.size
            End Get
        End Property

        Public Property taxonomy As Taxonomy

        Sub New(id%, data As Taxa(), size%, distance#)
            Me.ID = id
            Me.Childs = data _
                .ToDictionary(Function(a) a.label,
                              Function(x)
                                  Return CType(x, Tree(Of Value))
                              End Function)
            Me.Data = New Value With {.size = size, .distance = distance}
            Me.label = (id & Me.Data.ToString).MD5
        End Sub

        Sub New(id%, data$, size%, distance#)
            Me.ID = id
            Me.Data = New Value With {.size = size, .distance = distance}
            Me.label = If(data, (id & Me.Data.ToString).MD5)
        End Sub

        Public Overrides Function ToString() As String
            If Childs.IsNullOrEmpty Then
                Return label
            Else
                With Childs
                    If .Count = 1 Then
                        Return .First.ToString
                    Else
                        Return $"({ .First.ToString}, { .Last.ToString}: {Data.size.ToString("F2")})"
                    End If
                End With
            End If
        End Function
    End Class

End Namespace