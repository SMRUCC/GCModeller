#Region "Microsoft.VisualBasic::ef632a15fb6461616344f83f75d94a10, models\BioCyc\Reader\AttrDataCollection.vb"

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

'   Total Lines: 58
'    Code Lines: 43 (74.14%)
' Comment Lines: 4 (6.90%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 11 (18.97%)
'     File Size: 2.06 KB


' Class AttrDataCollection
' 
'     Properties: features, fileMeta
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: (+2 Overloads) LoadFile, ToString
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Public Class AttrDataCollection(Of T As Model)

    Public ReadOnly Property fileMeta As FileMeta

    ''' <summary>
    ''' enumerates all feature items inside current data file
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property features As IEnumerable(Of T)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return models.Values
        End Get
    End Property

    Protected ReadOnly models As Dictionary(Of String, T)

    Default Public ReadOnly Property getFeature(i As String) As T
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return models.TryGetValue(i)
        End Get
    End Property

    Sub New(meta As FileMeta, objects As IEnumerable(Of T))
        fileMeta = meta
        ' 20250903 possible duplicated key maybe existsed
        ' merge duplicated object data at here
        models = objects _
            .Where(Function(o) Not o.uniqueId Is Nothing) _
            .GroupBy(Function(o) o.uniqueId) _
            .ToDictionary(Function(o) o.Key,
                          Function(group)
                              Return MakeUnion(group)
                          End Function)
    End Sub

    Private Shared Function MakeUnion(group As IGrouping(Of String, T)) As T
        If group.Count = 1 Then
            Return group.First
        End If

        Call $"found duplicated object definition: '{group.Key}'!".warning

        ' 20250903
        ' how to merge data?
        Return group.First
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return fileMeta.ToString
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadFile(file As Stream) As AttrDataCollection(Of T)
        Return LoadFile(New StreamReader(file))
    End Function

    Public Shared Function LoadFile(file As TextReader) As AttrDataCollection(Of T)
        Dim dataFile As AttrValDatFile = AttrValDatFile.ParseFile(file)
        Dim writer As ObjectWriter = ObjectWriter.LoadSchema(Of T)
        Dim data As T() = (From a As SeqValue(Of FeatureElement)
                           In dataFile.features _
                               .SeqIterator _
                               .AsParallel
                           Let obj As Object = writer.Deserize(a.value)
                           Order By a.i
                           Select DirectCast(obj, T)).ToArray

        Return New AttrDataCollection(Of T)(dataFile.fileMeta, data)
    End Function

End Class
