Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComponentModel

    Public Class CatalogProfiling : Inherits ClassObject
        Implements IGrouping(Of String, String)
        Implements IList(Of String)
        Implements INamedValue
        Implements Value(Of String()).IValueOf

        ''' <summary>
        ''' COG/KO/GO, etc
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Overridable Property Catalog As String Implements IGrouping(Of String, String).Key, INamedValue.Key
        Public Property Description As String

        ''' <summary>
        ''' A list of gene ID that belongs to this catalog classify.
        ''' </summary>
        ''' <returns></returns>
        Public Property IDs As String() Implements Value(Of String()).IValueOf.value
            Get
                Return __list.ToArray
            End Get
            Set(value As String())
                __list = New List(Of String)(value)
            End Set
        End Property

        Dim __list As List(Of String)

        ''' <summary>
        ''' Number of the list IDs
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Count As Integer Implements ICollection(Of String).Count
            Get
                Return If(IDs Is Nothing, 0, IDs.Length)
            End Get
        End Property

        Default Public Property Item(index As Integer) As String Implements IList(Of String).Item
            Get
                Return IDs(index)
            End Get
            Set(value As String)
                IDs(index) = value
            End Set
        End Property

        Private ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of String).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Private Sub Insert(index As Integer, item As String) Implements IList(Of String).Insert
            Call __list.Insert(index, item)
        End Sub

        Private Sub RemoveAt(index As Integer) Implements IList(Of String).RemoveAt
            Call __list.RemoveAt(index)
        End Sub

        Private Sub Add(item As String) Implements ICollection(Of String).Add
            Call __list.Add(item)
        End Sub

        Private Sub Clear() Implements ICollection(Of String).Clear
            Call __list.Clear()
        End Sub

        Private Sub CopyTo(array() As String, arrayIndex As Integer) Implements ICollection(Of String).CopyTo
            Call __list.CopyTo(array, arrayIndex)
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
            For Each ID As String In IDs
                Yield ID
            Next
        End Function

        Public Function IndexOf(item As String) As Integer Implements IList(Of String).IndexOf
            Return __list.IndexOf(item)
        End Function

        Private Function Contains(item As String) As Boolean Implements ICollection(Of String).Contains
            Return __list.Contains(item)
        End Function

        Private Function Remove(item As String) As Boolean Implements ICollection(Of String).Remove
            Return __list.Remove(item)
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace