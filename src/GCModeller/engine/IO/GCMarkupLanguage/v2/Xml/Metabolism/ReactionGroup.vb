Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace v2

    ''' <summary>
    ''' the reaction collection
    ''' </summary>
    Public Class ReactionGroup : Implements IList(Of Reaction)

        <XmlAttribute>
        Public Property size As Integer Implements IList(Of Reaction).size
            Get
                Return enzymatic.TryCount + none_enzymatic.TryCount
            End Get
            Set(value As Integer)
                ' do nothing
            End Set
        End Property

        ''' <summary>
        ''' enzymatic reactions
        ''' </summary>
        ''' <returns></returns>
        Public Property enzymatic As Reaction()
        ''' <summary>
        ''' non-enzymatic reactions
        ''' </summary>
        ''' <returns></returns>
        Public Property none_enzymatic As Reaction()

        ''' <summary>
        ''' a collection of the reaction id index that used for transportation between compartments
        ''' </summary>
        ''' <returns></returns>
        Public Property transportation As Transportation()

        Default Public ReadOnly Property Item(id As String) As Reaction
            Get
                Return Me.AsEnumerable.Where(Function(rxn) rxn.ID = id).FirstOrDefault
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(copy As ReactionGroup)
            enzymatic = copy.enzymatic.SafeQuery.ToArray
            none_enzymatic = copy.none_enzymatic.SafeQuery.ToArray
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CompoundLinks() As Dictionary(Of String, Reaction())
            Return enzymatic _
                .JoinIterates(none_enzymatic) _
                .Select(Function(r)
                            Return r.AsEnumerable.Select(Function(c) (c, r))
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(l) l.c.compound) _
                .ToDictionary(Function(c) c.Key,
                              Function(l)
                                  Return l.Select(Function(a) a.r).ToArray
                              End Function)
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of Reaction) Implements Enumeration(Of Reaction).GenericEnumerator
            If Not enzymatic.IsNullOrEmpty Then
                For Each reaction As Reaction In enzymatic
                    Yield reaction
                Next
            End If
            If Not none_enzymatic.IsNullOrEmpty Then
                For Each reaction As Reaction In none_enzymatic
                    Yield reaction
                Next
            End If
        End Function

        Public Shared Widening Operator CType(reactions As Reaction()) As ReactionGroup
            Dim twoGroup = reactions _
                .GroupBy(Function(r) r.is_enzymatic) _
                .ToDictionary(Function(g) g.Key.ToString,
                              Function(g)
                                  Return g.ToArray
                              End Function)

            Return New ReactionGroup With {
                .enzymatic = twoGroup.TryGetValue(True.ToString, [default]:={}),
                .none_enzymatic = twoGroup.TryGetValue(False.ToString, [default]:={})
            }
        End Operator
    End Class
End Namespace