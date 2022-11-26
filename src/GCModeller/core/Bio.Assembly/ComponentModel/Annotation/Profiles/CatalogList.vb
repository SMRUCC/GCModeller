#Region "Microsoft.VisualBasic::d241fd6be4113a58d629dc1714c80af3, GCModeller\core\Bio.Assembly\ComponentModel\Annotation\Profiles\CatalogList.vb"

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

    '   Total Lines: 134
    '    Code Lines: 97
    ' Comment Lines: 15
    '   Blank Lines: 22
    '     File Size: 4.94 KB


    '     Class CatalogList
    ' 
    '         Properties: Catalog, Count, Description, IDs, IsReadOnly
    ' 
    '         Function: Contains, GetEnumerator, IEnumerable_GetEnumerator, IndexOf, (+2 Overloads) Intersect
    '                   Remove, ToString
    ' 
    '         Sub: (+2 Overloads) Add, Clear, CopyTo, Insert, RemoveAt
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComponentModel.Annotation

    ''' <summary>
    ''' a [term => id()] tuple data
    ''' </summary>
    Public Class CatalogList
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
        Public Property IDs As String() Implements Value(Of String()).IValueOf.Value
            Get
                Return hashset.Objects
            End Get
            Set(value As String())
                hashset = New Index(Of String)(value)
            End Set
        End Property

        Dim hashset As Index(Of String)

        ''' <summary>
        ''' Number of the list IDs
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Count As Integer Implements ICollection(Of String).Count
            Get
                Return hashset.Count
            End Get
        End Property

        Default Public Property Item(index As Integer) As String Implements IList(Of String).Item
            Get
                Return hashset.IndexOf(index:=index)
            End Get
            Set(value As String)
                hashset.Set(index, value)
            End Set
        End Property

        Private ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of String).IsReadOnly
            Get
                Return False
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Intersect(targets As Index(Of String)) As IEnumerable(Of String)
            Return hashset.Where(Function(id) id Like targets)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Intersect(targets As Dictionary(Of String, Double)) As IEnumerable(Of NamedValue(Of Double))
            Return From id As String
                   In hashset
                   Where targets.ContainsKey(id)
                   Select New NamedValue(Of Double)(id, targets(id))
        End Function

        Private Sub Insert(index As Integer, item As String) Implements IList(Of String).Insert
            Call hashset.Set(index, item)
        End Sub

        Private Sub RemoveAt(index As Integer) Implements IList(Of String).RemoveAt
            Call hashset.Delete(hashset.IndexOf(index))
        End Sub

        Public Sub Add(items As IEnumerable(Of String))
            For Each str As String In items
                Call hashset.Add(str)
            Next
        End Sub

        Public Sub Add(item As String) Implements ICollection(Of String).Add
            Call hashset.Add(item)
        End Sub

        Private Sub Clear() Implements ICollection(Of String).Clear
            Call hashset.Clear()
        End Sub

        Private Sub CopyTo(array() As String, arrayIndex As Integer) Implements ICollection(Of String).CopyTo
            Call hashset.Objects.CopyTo(array, arrayIndex)
        End Sub

        Public Overrides Function ToString() As String
            Return $"Dim {Catalog} = {hashset.GetJson}"
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
            For Each ID As String In IDs
                Yield ID
            Next
        End Function

        Public Function IndexOf(item As String) As Integer Implements IList(Of String).IndexOf
            Return hashset.IndexOf(item)
        End Function

        Private Function Contains(item As String) As Boolean Implements ICollection(Of String).Contains
            Return hashset.IndexOf(item) > -1
        End Function

        Private Function Remove(item As String) As Boolean Implements ICollection(Of String).Remove
            Call hashset.Delete(item)
            Return True
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
