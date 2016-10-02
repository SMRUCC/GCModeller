#Region "Microsoft.VisualBasic::e2b8b08915289a79ec91709729af6663, ..\interops\localblast\Tools\CLI\VennDiagram\Merge.vb"

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
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Terminal.STDIO

Partial Module CLI

    ''' <summary>
    ''' 将多个结果文件整合成一个结果文件，之后就可以调用venn命令生成文氏图了
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 对于多个结果文件的要求：
    ''' 每一个文件中的第一列为相同的物种
    ''' </remarks>
    <ExportAPI("merge", Info:="This program can not use the blast parsing result for the venn diagram drawing operation, " &
                                  "and this command is using for generate the drawing data for the venn diagram drawing command, " &
                                  "this command merge the blast log parsing result and then using the parsing result for drawing a venn diagram.",
        Usage:="merge -d <directory> -o <output_file>",
        Example:="merge -d ~/blast_besthit/ -o ~/Desktop/compared.csv")>
    <ParameterInfo("-d",
        Description:="The directory that contains some blast log parsing csv data file.",
        Example:="~/Desktop/blast/result/")>
    <ParameterInfo("-o",
        Description:="The save file name for the output result, the program willl save the merge result in the csv format",
        Example:="~/Desktop/8004_venn.csv")>
    Public Function Merge(args As CommandLine) As Integer
        Dim Directory As String = args("-d")
        Dim SavedFile As String = args("-o")

        Dim CsvFiles = From File As String
                       In ls - l - wildcards("*.Csv") <= Directory
                       Let Csv As DocumentStream.File = Sort(DocumentStream.File.Load(File))
                       Select Csv
                       Order By Csv.Width Descending   '

        Dim CsvFile As DocumentStream.File =
            __mergeFile(CsvFiles.ToList)
        CsvFile = DocumentStream.File.Distinct(CsvFile, Scan0, True)
        CsvFile = DocumentStream.File.RemoveSubRow(CsvFile)

        For Each Column As String() In CsvFile.Columns
            Dim Query = From s As String In Column.AsParallel Where Len(Trim(s)) > 0 Select 1 '
            Console.WriteLine("Column counts: {0}", Query.Count)
        Next

        Return CsvFile.Save(SavedFile, False).CLICode
    End Function

    ''' <summary>
    ''' 必须要确保每一个文件之中的列的位置要一致
    ''' </summary>
    ''' <param name="CsvList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function __mergeFile(CsvList As List(Of DocumentStream.File)) As DocumentStream.File
        For i As Integer = 0 To CsvList.Count - 1
            For c As Integer = 0 To i - 1
                Call CsvList(i).InsertEmptyColumnBefore(0)
            Next
#If DEBUG Then
            Call CsvList(i).Save(My.Computer.FileSystem.SpecialDirectories.Temp & "/" & i & ".csv", False)
#End If
        Next

        Dim File As DocumentStream.File = CsvList.First
        For i As Integer = 1 To CsvList.Count - 1
            Call File.Append(CsvList(i))
        Next

        Return File
    End Function

    ''' <summary>
    ''' [Space][Column1][Column2]
    ''' </summary>
    ''' <param name="Csv"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function Sort(Csv As DocumentStream.File) As DocumentStream.File
        Call Csv.RemoveAt(index:=Scan0)

        Dim IdList As String() = (From Id As String In Csv.Column(Scan0) Select Id Order By Id Ascending).ToArray
        Dim File As DocumentStream.File = New DocumentStream.File
        Dim n As Integer = (Csv.Width - 1) / 3
        Dim Temp() As List(Of KeyValuePair(Of String, String)) = (From i As Integer In n.Sequence Select New List(Of KeyValuePair(Of String, String))).ToArray

        For i As Integer = 0 To n - 1
            Dim a As Integer = 3 * i + 2, b = 3 * i + 3
            Dim Query = From row As Integer In Csv.Sequence
                        Let row_ = Csv(row)
                        Let id1 = row_.Column(a) Let id2 = row_.Column(b)
                        Let ele = New KeyValuePair(Of String, String)(id1, id2)
                        Where Not String.IsNullOrEmpty(ele.Key)
                        Select ele
                        Order By ele.Key Ascending '

            Call Temp(i).AddRange(Query.ToArray)
        Next

        For i As Integer = 0 To IdList.Count - 1
            Dim Id As String = IdList(i)
            Dim row As DocumentStream.RowObject = New DocumentStream.RowObject

            row += Id

            For idx As Integer = 0 To n - 1
                Dim Query = From k In Temp(idx) Where String.Equals(k.Key, Id) Select k '
                Query = Query.ToArray
                If Query.Count > 0 Then
                    row.Add(Query.First.Value)
                Else
                    row.Add("")
                End If
            Next

            File += row
        Next

        Return File
    End Function
End Module

