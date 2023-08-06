#Region "Microsoft.VisualBasic::bc5c43abef65a72bcfa0dc687855cc79, GCModeller\engine\IO\Raw\GCModellerRaw\Reader.vb"

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

'   Total Lines: 145
'    Code Lines: 102
' Comment Lines: 16
'   Blank Lines: 27
'     File Size: 5.66 KB


'     Class Reader
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: AllTimePoints, LoadIndex, PopulateFrames, Read, ReadModule
' 
'         Sub: readIndex
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Raw

    Public Class Reader : Inherits CellularModules

        ReadOnly stream As StreamPack

        ''' <summary>
        ''' total time tick count
        ''' </summary>
        ReadOnly tick_counts As Integer

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
        End Sub

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
            Dim buf As New BinaryDataReader(s, byteOrder:=ByteOrder.BigEndian)

            buf.ReadString(BinaryStringFormat.DwordLengthPrefix)
            buf.ReadDouble()
            buf.ReadString(BinaryStringFormat.DwordLengthPrefix)

            Dim json As String = buf.ReadString(BinaryStringFormat.DwordLengthPrefix)
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
            Dim root As StreamGroup = stream.GetObject(fileName:="/dynamics/")

            For Each modu In modules.Values.ToArray
                modules(modu.Name) = modu
            Next

            For Each file As StreamBlock In root.ListFiles().Where(Function(f) f.referencePath.FileName = "index.txt")
                Dim moduName As String = file.referencePath.DirectoryPath.BaseName
                Dim list As String() = Strings.Trim(stream.ReadText(file)).LineTokens

                Call Me.modules.Add(moduName, list)
                Call modules(moduName).SetValue(Me, list.Indexing)
            Next

            Return Me
        End Function

        Public Function GetFrameFile(modu As String, time As Double) As BinaryDataReader
            Dim index = $"/dynamics/{[modu]}/frames/{time}.dat"
            Dim offset As Stream = stream.OpenBlock(index)
            Dim buf As New BinaryDataReader(offset, byteOrder:=ByteOrder.BigEndian)

            Return buf
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Read(time#, module$) As Dictionary(Of String, Double)
            Return ReadModule(module$, stream:=GetFrameFile([module], time))
        End Function

        Public Function ReadModule(module$, stream As BinaryDataReader) As Dictionary(Of String, Double)
            Dim list As Index(Of String) = modules([module])
            Dim data#() = stream.ReadDoubles(list.Count)
            Dim values As Dictionary(Of String, Double) = list _
                .ToDictionary(Function(id) id.value,
                              Function(i)
                                  Return data(i)
                              End Function)

            Return values
        End Function

        Public Iterator Function PopulateFrames() As IEnumerable(Of (time#, frame As Dictionary(Of DataSet)))

        End Function
    End Class
End Namespace
