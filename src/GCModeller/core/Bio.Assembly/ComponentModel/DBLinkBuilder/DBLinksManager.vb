#Region "Microsoft.VisualBasic::55a47d2bf45984742baa3455b90db1e6, GCModeller\core\Bio.Assembly\ComponentModel\DBLinkBuilder\DBLinksManager.vb"

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

    '   Total Lines: 116
    '    Code Lines: 90
    ' Comment Lines: 9
    '   Blank Lines: 17
    '     File Size: 4.27 KB


    '     Class DBLinksManager
    ' 
    '         Properties: Count, DBLinkObjects, DBLinks, PrefixDB
    ' 
    '         Function: GetEnumerator, GetEnumerator1
    ' 
    '         Sub: (+2 Overloads) AddEntry, Remove
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Namespace ComponentModel.DBLinkBuilder

    Public MustInherit Class DBLinksManager(Of TLink As IDBLink)
        Implements IReadOnlyCollection(Of TLink)

        Public Shared ReadOnly Property PrefixDB As String() = New String() {
            "ChEBI", "3DMET", "HMDB",
            "KNApSAcK", "MASSBANK",
            "NIKKAJI", "PDB-CCD",
            "PubChem", "KEGG.Compound"
        }

        Protected _DBLinkObjects As List(Of TLink)

        Default Public ReadOnly Property Item(DBName As String) As TLink()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Dim LQuery = LinqAPI.Exec(Of TLink) _
 _
                    () <= From link As TLink
                          In _DBLinkObjects
                          Where String.Equals(DBName, link.DbName, StringComparison.OrdinalIgnoreCase)
                          Select link

                Return LQuery
            End Get
        End Property

        Public Property DBLinkObjects As TLink()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return _DBLinkObjects.ToArray
            End Get
            Set(value As TLink())
                _DBLinkObjects = value.AsList
            End Set
        End Property

        Public ReadOnly Property DBLinks As String()
            Get
                Return (From item As TLink
                        In _DBLinkObjects
                        Select item.GetFormatValue).ToArray
            End Get
        End Property

        Public Sub AddEntry(Entry As TLink)
            Dim duplicated As TLink = LinqAPI.DefaultFirst(Of TLink) _
 _
                () <= From link As TLink
                      In DBLinkObjects
                      Where String.Equals(link.DbName, Entry.DbName, StringComparison.OrdinalIgnoreCase) AndAlso
                          String.Equals(link.EntryId, Entry.EntryId, StringComparison.OrdinalIgnoreCase)
                      Select link

            ' 会在这里先检查是否有重复的记录数据出现，
            ' 假若还没有重复的数据才会进行添加
            If duplicated Is Nothing Then
                Call _DBLinkObjects.Add(Entry)
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Db_Name"></param>
        ''' <param name="entryID">
        ''' 当编号值为空的时候，会将所有的<paramref name="Db_Name"/>类型的数据都删除掉
        ''' </param>
        Public Sub Remove(Db_Name As String, Optional entryID As String = "")
            Dim links As TLink() = Item(Db_Name)
            Dim test = Function(l As TLink)
                           If entryID.StringEmpty Then
                               Return True
                           Else
                               Return entryID.TextEquals(l.EntryId)
                           End If
                       End Function

            If Not links.IsNullOrEmpty Then
                For Each ll As TLink In links.Where(Function(l) True = test(l))
                    Call _DBLinkObjects.Remove(ll)
                Next
            End If
        End Sub

        Public Sub AddEntry(DBName As String, Entry As String)
            Dim link As TLink = Activator.CreateInstance(Of TLink)()
            link.DbName = DBName
            link.EntryId = Entry
            Call AddEntry(Entry:=link)
        End Sub

        Public Iterator Function GetEnumerator() As IEnumerator(Of TLink) Implements IEnumerable(Of TLink).GetEnumerator
            For Each item As TLink In DBLinkObjects
                Yield item
            Next
        End Function

        Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of TLink).Count
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return DBLinkObjects.Length
            End Get
        End Property

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        Public MustOverride ReadOnly Property IsEmpty As Boolean
    End Class
End Namespace
