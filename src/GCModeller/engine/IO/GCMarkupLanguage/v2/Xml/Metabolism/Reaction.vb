Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
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
        Public Property transportation As String()

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

    ''' <summary>
    ''' the reaction graph model
    ''' </summary>
    <XmlType("reaction", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Reaction : Implements INamedValue, Enumeration(Of CompoundFactor)

        ''' <summary>
        ''' unique reference id of current reaction link
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlElement> Public Property name As String
        <XmlElement> Public Property note As String
        ''' <summary>
        ''' 这个反应模型是否是需要酶促才会发生了生化反应过程？
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property is_enzymatic As Boolean
        ''' <summary>
        ''' [forward, reverse] boundary of the reaction speed
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property bounds As Double()

        <XmlElement> Public Property ec_number As String()
        ''' <summary>
        ''' the compartment location of the reaction
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property compartment As String()
        <XmlElement> Public Property substrate As CompoundFactor()
        <XmlElement> Public Property product As CompoundFactor()

        ''' <summary>
        ''' the debug view of the current equation model
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property equation As String
            Get
                Return substrate.Select(Function(a) a.factorString).JoinBy(" + ") &
                    " <=> " &
                    product.Select(Function(a) a.factorString).JoinBy(" + ")
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"({ID}: {name}) {equation}"
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of CompoundFactor) Implements Enumeration(Of CompoundFactor).GenericEnumerator
            For Each c As CompoundFactor In substrate.SafeQuery
                Yield c
            Next
            For Each c As CompoundFactor In product.SafeQuery
                Yield c
            Next
        End Function
    End Class

    Public Class CompoundFactor

        <XmlAttribute>
        Public Property factor As Double
        <XmlText>
        Public Property compound As String
        <XmlAttribute>
        Public Property compartment As String

        Sub New()
        End Sub

        Sub New(factor As Double, compound As String, Optional compartment As String = Nothing)
            Me.compartment = compartment
            Me.factor = factor
            Me.compound = compound
        End Sub

        Sub New(compound As String, factor As Double, Optional compartment As String = Nothing)
            Me.factor = factor
            Me.compound = compound
            Me.compartment = compartment
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{compartment}]" & compound
        End Function

        Friend Function factorString() As String
            If factor <= 1 Then
                Return compound
            Else
                Return factor & " " & compound
            End If
        End Function

    End Class
End Namespace