#Region "Microsoft.VisualBasic::43dc7958386a194999c5e398570df33a, GCModeller\core\Bio.Assembly\Assembly\KEGG\Web\Map\MapRepository.vb"

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

    '   Total Lines: 161
    '    Code Lines: 119
    ' Comment Lines: 21
    '   Blank Lines: 21
    '     File Size: 6.47 KB


    '     Class MapRepository
    ' 
    '         Properties: Maps
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: (+2 Overloads) BuildRepository, CreateIndex, Exists, GenericEnumerator, GetAll
    '                   GetByKey, GetEnumerator, GetMapsAuto, GetWhere, QueryMapsByMembers
    '                   ScanMaps
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.WebServices

    ''' <summary>
    ''' The repository xml data of kegg <see cref="Map"/>
    ''' </summary>
    Public Class MapRepository : Inherits XmlDataModel
        Implements IRepositoryRead(Of String, MapIndex)
        Implements Enumeration(Of Map)

        <XmlElement("maps")>
        Public Property Maps As MapIndex()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return table.Values.ToArray
            End Get
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Set(value As MapIndex())
                table = value.ToDictionary(Function(map) map.id.Match("\d+"))
            End Set
        End Property

        Default Public ReadOnly Property Item(id As String) As Map
            Get
                Return GetByKey(id)
            End Get
        End Property

        ''' <summary>
        ''' Get by ID
        ''' </summary>
        Dim table As Dictionary(Of String, MapIndex)

        <XmlNamespaceDeclarations()>
        Public xmlns As XmlSerializerNamespaces

        Sub New()
            xmlns = New XmlSerializerNamespaces
            xmlns.Add("map", Map.XmlNamespace)
        End Sub

        Private Sub New(maps As IEnumerable(Of MapIndex))
            Me.New
            Me.Maps = maps.ToArray
        End Sub

        ''' <summary>
        ''' query all pathway maps which is contains any <paramref name="entity"/> id list.
        ''' </summary>
        ''' <param name="entity"></param>
        ''' <returns></returns>
        Public Iterator Function QueryMapsByMembers(entity As IEnumerable(Of String)) As IEnumerable(Of MapIndex)
            For Each key As String In entity
                For Each map As MapIndex In table.Values
                    If map.hasAny(key) Then
                        Yield map
                    End If
                Next
            Next
        End Function

        ' key.Match("\d+") means ignores the kegg organism prefix

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Exists(key As String) As Boolean Implements IRepositoryRead(Of String, MapIndex).Exists
            Return table.ContainsKey(key.Match("\d+"))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetByKey(key As String) As MapIndex Implements IRepositoryRead(Of String, MapIndex).GetByKey
            Return table(key.Match("\d+"))
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
        Public Shared Function BuildRepository(directory As String, Optional silent As Boolean = True) As MapRepository
            Return New MapRepository With {
                .Maps = directory _
                    .DoCall(AddressOf ScanMaps) _
                    .Select(Function(map) CreateIndex(map, silent)) _
                    .ToArray
            }
        End Function

        Public Shared Function BuildRepository(data As IEnumerable(Of Map), Optional silent As Boolean = True) As MapRepository
            Return New MapRepository(From map As Map In data Select CreateIndex(map, silent))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ScanMaps(directory As String) As IEnumerable(Of Map)
            Return (ls - l - r - "*.XML" <= directory).Select(AddressOf LoadXml(Of Map))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function CreateIndex(map As Map, silent As Boolean) As MapIndex
            If Not silent Then
                Call map.Name.__DEBUG_ECHO
            End If

            Return New MapIndex With {
                .id = map.id,
                .KeyVector = New TermsVector With {
                    .terms = map _
                        .shapes _
                        .Select(Function(a) a.IDVector) _
                        .IteratesALL _
                        .Distinct _
                        .OrderBy(Function(s) s) _
                        .ToArray
                },
                .shapes = map.shapes,
                .Name = map.Name,
                .PathwayImage = map.PathwayImage,
                .URL = map.URL
            }
        End Function

        Public Shared Function GetMapsAuto(repository As String) As IEnumerable(Of Map)
            If repository.DirectoryExists Then
                Return repository.DoCall(AddressOf ScanMaps)
            Else
                Return repository.LoadXml(Of MapRepository)
            End If
        End Function

        ''' <summary>
        ''' 因为<see cref="MapIndex"/>是直接继承至<see cref="Map"/>对象类型的，所以在这里可以直接返回这个序列
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function GenericEnumerator() As IEnumerator(Of Map) Implements Enumeration(Of Map).GenericEnumerator
            For Each index In Maps
                Yield index
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of Map).GetEnumerator
            Yield GenericEnumerator()
        End Function
    End Class
End Namespace
