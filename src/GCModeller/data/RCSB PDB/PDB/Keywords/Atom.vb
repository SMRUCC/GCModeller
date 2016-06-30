Namespace Keywords

    Public Class Atom : Inherits Keyword
        Implements IEnumerable(Of AtomUnit)

        Sub New(itemDatas As KeyValuePair(Of Integer, String)())
            Call MyBase.New(itemDatas)
            Atoms = (From item In itemDatas.AsParallel Select AtomUnit.InternalParser(item.Value, InternalIndex:=item.Key)).ToArray
        End Sub

        Public Property Atoms As AtomUnit()

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_ATOM
            End Get
        End Property

        Public Iterator Function GetEnumerator() As IEnumerator(Of AtomUnit) Implements IEnumerable(Of AtomUnit).GetEnumerator
            For Each Atom As AtomUnit In Me.Atoms
                Yield Atom
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace