#Region "Microsoft.VisualBasic::7934bc74da0f9d191a8b94264bbc9d2d, core\Bio.Assembly\ComponentModel\Annotation\Profiles\CatalogList.vb"

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

'     Class CatalogProfiling
' 
'         Properties: Catalog, Description, SubCategory
' 
'         Function: ToString
' 
'     Class CatalogList
' 
'         Properties: Catalog, Count, Description, IDs, IsReadOnly
' 
'         Function: Contains, GetEnumerator, IEnumerable_GetEnumerator, IndexOf, Remove
'                   ToString
' 
'         Sub: Add, Clear, CopyTo, Insert, RemoveAt
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComponentModel.Annotation

    Public Class CatalogProfiling
        Implements INamedValue

        ''' <summary>
        ''' COG/KO/GO, etc
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Overridable Property Catalog As String Implements INamedValue.Key
        Public Property Description As String
        Public Property SubCategory As Dictionary(Of String, CatalogList)

        Public Overrides Function ToString() As String
            Return $"{Catalog} contains {SubCategory.Count} subcategories... {Mid(SubCategory.Keys.GetJson, 1, 20)}..."
        End Function

    End Class

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
                Return list.ToArray
            End Get
            Set(value As String())
                list = New List(Of String)(value)
            End Set
        End Property

        Dim list As List(Of String)

        ''' <summary>
        ''' Number of the list IDs
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Count As Integer Implements ICollection(Of String).Count
            Get
                Return list.Count
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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Intersect(targets As Index(Of String)) As IEnumerable(Of String)
            Return list.Where(Function(id) id Like targets)
        End Function

        Private Sub Insert(index As Integer, item As String) Implements IList(Of String).Insert
            Call list.Insert(index, item)
        End Sub

        Private Sub RemoveAt(index As Integer) Implements IList(Of String).RemoveAt
            Call list.RemoveAt(index)
        End Sub

        Private Sub Add(item As String) Implements ICollection(Of String).Add
            Call list.Add(item)
        End Sub

        Private Sub Clear() Implements ICollection(Of String).Clear
            Call list.Clear()
        End Sub

        Private Sub CopyTo(array() As String, arrayIndex As Integer) Implements ICollection(Of String).CopyTo
            Call list.CopyTo(array, arrayIndex)
        End Sub

        Public Overrides Function ToString() As String
            Return $"Dim {Catalog} = {list.GetJson}"
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
            For Each ID As String In IDs
                Yield ID
            Next
        End Function

        Public Function IndexOf(item As String) As Integer Implements IList(Of String).IndexOf
            Return list.IndexOf(item)
        End Function

        Private Function Contains(item As String) As Boolean Implements ICollection(Of String).Contains
            Return list.Contains(item)
        End Function

        Private Function Remove(item As String) As Boolean Implements ICollection(Of String).Remove
            Return list.Remove(item)
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
