#Region "Microsoft.VisualBasic::92c81a665d5a6b57e299f4a257c44c0b, engine\IO\Raw\HDS\GCModellerRaw\Reader.vb"

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

    '   Total Lines: 135
    '    Code Lines: 96 (71.11%)
    ' Comment Lines: 12 (8.89%)
    '    - Xml Docs: 91.67%
    ' 
    '   Blank Lines: 27 (20.00%)
    '     File Size: 5.53 KB


    '     Class Reader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: AllTimePoints, GetFrameFile, GetGraphData, GetIdCounts, GetMoleculeIdList
    '                   GetRelatedReactions, GetStream, LoadIndex, PopulateFrames, Read
    '                   ReadModule
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text

Namespace Raw

    Public Interface IStreamContainer

        Function GetStream() As StreamPack

    End Interface

    Public Class Reader : Inherits CellularModules
        Implements IStreamContainer

        ReadOnly stream As StreamPack

        ''' <summary>
        ''' total time tick count
        ''' </summary>
        ReadOnly tick_counts As Integer

        Public ReadOnly Property comparts As String()

        Default Public ReadOnly Property ModuleIdSet(name As String) As Index(Of String)
            Get
                Return modules.TryGetValue(name)
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function AllTimePoints() As IEnumerable(Of Double)
            Dim file As Stream = stream.OpenFile("/.etc/ticks.dat", FileMode.Open, FileAccess.Read)
            Dim buf As New BinaryDataReader(file, byteOrder:=ByteOrder.BigEndian)

            Return buf.ReadDoubles(tick_counts)
        End Function

        Sub New(input As Stream)
            stream = New StreamPack(input)
            tick_counts = Strings.Trim(stream.ReadText("/.etc/ticks.txt")).DoCall(AddressOf Integer.Parse)
            comparts = stream.ReadText("/compartments.txt") _
                .LineTokens _
                .Where(Function(s) Not s.StringEmpty(, True)) _
                .ToArray
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetStream() As StreamPack Implements IStreamContainer.GetStream
            Return stream
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="id">id of the metabolite</param>
        ''' <returns></returns>
        Public Iterator Function GetRelatedReactions(id As String) As IEnumerable(Of String)
            Dim dir As StreamGroup = stream.GetObject($"/graph/links/{id}/")

            For Each file In dir.ListFiles
                If TypeOf file Is StreamBlock Then
                    Yield file.referencePath.ToString.BaseName
                End If
            Next
        End Function

        Public Function GetGraphData(metabo As String, rxn As String) As Dictionary(Of String, String())
            Dim path As String = $"/graph/links/{metabo}/{rxn}.dat"
            Dim s As Stream = stream.OpenFile(path, FileMode.Open, FileAccess.Read)
            Dim buf As New StreamReader(s, encoding:=Encodings.UTF8WithoutBOM.CodePage)
            Dim str As String = buf.ReadToEnd
            Dim json As String = str.LoadJSON(Of Dictionary(Of String, String))!graph
            Dim metadata As Dictionary(Of String, String()) = json.LoadJSON(Of Dictionary(Of String, String()))

            Return metadata
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetIdCounts() As Dictionary(Of String, Integer)
            Return stream.ReadText("/.etc/count.json").LoadJSON(Of Dictionary(Of String, Integer))
        End Function

        Public Function GetMoleculeIdList() As Dictionary(Of String, String())
            Return modules.ToDictionary(Function(m) m.Key, Function(m) m.Value.Objects)
        End Function

        ''' <summary>
        ''' load data visitor index
        ''' </summary>
        ''' <returns></returns>
        Public Function LoadIndex() As Reader
            Dim modules As Dictionary(Of String, PropertyInfo) = Me.GetModuleReader
            Dim root As StreamGroup = stream.GetObject(fileName:="/index/")

            For Each modu In modules.Values.ToArray
                modules(modu.Name) = modu
            Next

            For Each file As StreamBlock In From f As StreamObject
                                            In root.ListFiles()
                                            Where modules.ContainsKey(f.referencePath.FileName.BaseName)

                Dim moduName As String = file.referencePath.FileName.BaseName
                Dim list As String() = Strings.Trim(stream.ReadText(file)).LineTokens

                Call Me.modules.Add(moduName, list)
                Call modules(moduName).SetValue(Me, list.Indexing)
            Next

            Return Me
        End Function

        Public Function GetFrameFile(modu As String, compart_id As String, time As Double) As BinaryDataReader
            Dim index = $"/dynamics/{compart_id}/{[modu]}/frames/{time}.dat"
            Dim offset As Stream = stream.OpenBlock(index)
            Dim buf As New BinaryDataReader(offset, byteOrder:=ByteOrder.BigEndian)

            Return buf
        End Function

        Public Function ReadFlux(time As Double, group As String) As Dictionary(Of String, Double)
            Dim path As String = $"/dynamics/flux/{group}/frames/{time}.dat"
            Dim offset As Stream = stream.OpenBlock(path)
            Dim buf As New BinaryDataReader(offset, byteOrder:=ByteOrder.BigEndian)
            Dim data As New Dictionary(Of String, Double)

            For Each xi In ReadModule(group, buf)
                Call data.Add(xi.Item1, xi.Item2)
            Next

            Return data
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Read(time#, module$) As Dictionary(Of String, Double)
            Dim data As New Dictionary(Of String, Double)

            For Each compart_id As String In comparts
                For Each xi In ReadModule(module$, stream:=GetFrameFile([module], compart_id, time))
                    Call data.Add(xi.Item1 & "@" & compart_id, xi.Item2)
                Next
            Next

            Return data
        End Function

        Public Iterator Function ReadModule(module$, stream As BinaryDataReader) As IEnumerable(Of (String, Double))
            ' filter of the possible empty collection
            Dim list As String() = modules([module]).Objects _
                .Where(Function(id) Not id.StringEmpty(, True)) _
                .ToArray

            If list.IsNullOrEmpty Then
                Return
            End If

            Dim data#() = stream.ReadDoubles(list.Count)

            For i As Integer = 0 To data.Length - 1
                Yield (list(i), data(i))
            Next
        End Function

        Public Iterator Function PopulateFrames() As IEnumerable(Of (time#, frame As Dictionary(Of DataSet)))

        End Function

        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                Call stream.Dispose()
            Catch ex As Exception

            End Try
        End Sub
    End Class
End Namespace
