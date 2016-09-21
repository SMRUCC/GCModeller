#Region "Microsoft.VisualBasic::194ad4e02a0ac312ad726f8cdb0e30cc, ..\interops\localblast\Tools\CLI\VennDiagram\ExportBestHit.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv

Partial Module CLI

    <ExportAPI("-export_besthit",
               Info:="",
               Usage:="-export_besthit -i <input_csv_file> -o <output_saved_csv>",
               Example:="")>
    Public Function ExportBestHit(args As CommandLine) As Integer
        Dim Input As String = args("-i"), Output As String = args("-o")
        Dim File As DocumentStream.File = DocumentStream.File.Load(Input)
        File = GetDiReBh(File)
        Return File.Save(Output, False).CLICode
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("-merge_besthit",
               Info:="",
               Usage:="-merge_besthit -i <input_file_list> -o <output_file> -os <original_idlist_sequence_file> [-osidx <id_column_index> -os_skip_first <T/F>]",
               Example:="")>
    <ParameterInfo("-i", False,
        Description:="Each file path in the filelist should be separated by a ""|"" character.",
        Usage:="-i ""file_path1|file_path2|file_path3|...""",
        Example:="")>
    Public Function MergeBestHits(args As CommandLine) As Integer
        Dim FilePathList = args("-i").Split(CChar("|"))
        Dim Idx As Integer = Val(args("-osidx"))
        Dim Output As String = args("-o")
        Dim OriginalIdSequence As String = args("-os")
        Dim SkipFirst As Boolean = String.Equals(args("-os_skip_first"), "T")

        Dim File As DocumentStream.File = New DocumentStream.File
        File += New String() {"query_id"}

        Dim IdSequence = DocumentStream.File.Load(OriginalIdSequence).Skip(If(SkipFirst, 1, 0)).ToArray
        Dim FileList = (From path In FilePathList Select DocumentStream.File.Load(path)).ToArray

        For i As Integer = 0 To IdSequence.Length - 1
            Dim row As DocumentStream.RowObject = New String() {IdSequence(i)(Idx)}
            For Each CsvFile In FileList
                Dim rowBesthit = CsvFile.GetByLine(line:=i)
                row += New String() {" ", rowBesthit.Column(0), rowBesthit.Column(1)}
            Next

            File += row
        Next

        Return File.Save(Output, False).CLICode
    End Function

    <ExportAPI("-copy", Info:="", Usage:="-copy -i <index_file> -os <output_saved> [-osidx <id_column_index> -os_skip_first <T/F>]", Example:="")>
    Public Function Copy(args As CommandLine) As Integer
        Dim Idx As Integer = Val(args("-osidx"))
        Dim Output As String = args("-os")
        Dim OriginalIdSequence As String = args("-i")
        Dim SkipFirst As Boolean = String.Equals(args("-os_skip_first"), "T")

        Dim File As DocumentStream.File = New DocumentStream.File
        Dim IdSequence = DocumentStream.File.Load(OriginalIdSequence).Skip(If(SkipFirst, 1, 0)).ToArray

        File += {"query_id"}
        File += (From row In IdSequence Select New DocumentStream.RowObject(New String() {row(Idx)}))

        Return File.Save(Output, False).CLICode
    End Function

    ''' <summary>
    ''' 获取双向的最佳匹配结果
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDiReBh(DataFile As DocumentStream.File) As DocumentStream.File
        Dim File As DocumentStream.File = New DocumentStream.File + {"query_id"}

        For Each row As DocumentStream.RowObject In DataFile
            Dim FindTarget As String = row(1)
            Dim QueryTarget As String = row(0)
            Dim row2 As DocumentStream.RowObject() =
                DataFile.FindAtColumn(FindTarget, Column:=3)

            If row2.IsNullOrEmpty Then Continue For

            Dim ___findTarget = row2.First()(4)

            If String.Equals(QueryTarget, ___findTarget) Then '可以双向匹配
                File += New String() {QueryTarget, FindTarget}
            End If
        Next

        Return File
    End Function
End Module

