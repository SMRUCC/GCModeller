#Region "Microsoft.VisualBasic::fca95d9f3e54637e7c901686c5cb812d, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\Web\Map\MapIndex.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.WebServices

    Public Class MapIndex : Implements INamedValue

        <XmlAttribute>
        Public Property MapID As String Implements IKeyedEntity(Of String).Key
        Public Property KeyVector As TermsVector
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New TermsVector With {
                    .Terms = Index.Objects
                }
            End Get
            Set(value As TermsVector)
                _Index = New Index(Of String)(value.Terms)
                _KOIndex = value _
                    .Terms _
                    .Where(Function(id)
                               Return id.IsPattern("K\d+", RegexICSng)
                           End Function) _
                    .Indexing
                _CompoundIndex = value _
                    .Terms _
                    .Where(Function(id)
                               Return id.IsPattern("C\d+", RegexICSng)
                           End Function) _
                    .Indexing
            End Set
        End Property

        Public Property Map As Map

        ''' <summary>
        ''' KO, compoundID, reactionID, etc.
        ''' </summary>
        ''' <returns></returns>
        <XmlIgnore> Public ReadOnly Property Index As Index(Of String)
        <XmlIgnore> Public ReadOnly Property KOIndex As Index(Of String)
        <XmlIgnore> Public ReadOnly Property CompoundIndex As Index(Of String)

        Public ReadOnly Property MapTitle As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Map.Name
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return MapID
        End Function
    End Class

    Public Class MapRepository : Implements IRepositoryRead(Of String, MapIndex)

        <XmlElement(NameOf(MapIndex))> Public Property Maps As MapIndex()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return table.Values.ToArray
            End Get
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Set(value As MapIndex())
                table = value.ToDictionary(Function(map) map.MapID)
            End Set
        End Property

        ''' <summary>
        ''' Get by ID
        ''' </summary>
        Dim table As Dictionary(Of String, MapIndex)

        Public Iterator Function QueryMapsByMembers(entity As IEnumerable(Of String)) As IEnumerable(Of MapIndex)
            For Each key As String In entity
                For Each map As MapIndex In table.Values
                    If map.Index.IndexOf(key) > -1 Then
                        Yield map
                    End If
                Next
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Exists(key As String) As Boolean Implements IRepositoryRead(Of String, MapIndex).Exists
            Return table.ContainsKey(key)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetByKey(key As String) As MapIndex Implements IRepositoryRead(Of String, MapIndex).GetByKey
            Return table(key)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetWhere(clause As Func(Of MapIndex, Boolean)) As IReadOnlyDictionary(Of String, MapIndex) Implements IRepositoryRead(Of String, MapIndex).GetWhere
            Return table.Values.Where(clause).ToDictionary
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetAll() As IReadOnlyDictionary(Of String, MapIndex) Implements IRepositoryRead(Of String, MapIndex).GetAll
            Return New Dictionary(Of String, MapIndex)(table)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="directory">The reference map download directory</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function BuildRepository(directory As String) As MapRepository
            Return New MapRepository With {
                .Maps = (ls - l - r - "*.XML" <= directory) _
                    .Select(AddressOf LoadXml(Of Map)) _
                    .Select(AddressOf CreateIndex) _
                    .ToArray
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function CreateIndex(map As Map) As MapIndex
            Return New MapIndex With {
                .Map = map,
                .MapID = map.ID,
                .KeyVector = New TermsVector With {
                    .Terms = map _
                        .Areas _
                        .Select(Function(a) a.IDVector) _
                        .IteratesALL _
                        .Distinct _
                        .OrderBy(Function(s) s) _
                        .ToArray
                }
            }
        End Function
    End Class
End Namespace
