#Region "Microsoft.VisualBasic::87e45b0f70bdc988b80b31f1d05b3b04, ..\interops\localblast\CLI_tools\CLI\VennDiagram\LogFile.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
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
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Analysis
Imports SMRUCC.genomics.Interops
Imports Microsoft.VisualBasic.CommandLine
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput

Partial Module CLI

    ''' <summary>
    ''' 分析BLAST程序所输出的日志文件，目标日志文件必须是经过Grep操作得到的
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>最后一个文件对中的File2位最后一个基因组的标号集合</remarks>
    <ExportAPI("logs_analysis", Info:="Parsing the xml format blast log into a csv data file that use for venn diagram drawing.",
        Usage:="logs_analysis -d <xml_logs_directory> -export <export_csv_file>",
        Example:="logs_analysis -d ~/xml_logs -export ~/Desktop/result.csv")>
    <Argument("-d",
        Description:="The data directory which contains the xml format blast log file, those xml format log file were generated from the 'venn -> blast' command.")>
    <Argument("-export",
        Description:="The save file path for the venn diagram drawing data csv file.")>
    Public Function bLogAnalysis(args As CommandLine) As Integer
        Dim CsvFile As String = args("-export")
        Dim LogsDir As String = args("-d")

        Dim ListFile = LogsPair.GetXmlFileName(LogsDir).LoadXml(Of LogsPair)()
        Dim ListCsv = New List(Of IO.File())  '每一个文件对中的File1位主要的文件
        For Each List In ListFile.Logs
            Dim Query = From Pair In List
                        Select LogAnalysis.TakeBestHits(Legacy.BLASTOutput.Load(Pair.Query),
                            Legacy.BLASTOutput.Load(Pair.Target)) '获取BestHit
            Call ListCsv.Add(Query.ToArray)
        Next
        Dim LastFile = Legacy.BLASTOutput.Load(ListFile.Logs.Last.Last.Target)
        Call ListCsv.Add(New IO.File() {(From Query In LastFile.Queries.AsParallel Select Query.QueryName).ToArray})

        Dim MergeResult = (From List In ListCsv Select LogAnalysis.Merge(dataset:=List)).AsList
        Dim Csv = CLI.__mergeFile(MergeResult)  '合并文件，获取最终绘制文氏图所需要的数据文件

        Return Csv.Save(path:=CsvFile).CLICode
    End Function

    ''' <summary>
    ''' 解析BLAST日志文件中的标记号名称
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("grep", Info:="The gene id in the blast output log file are not well format for reading and program processing, so " &
                                 "before you generate the venn diagram you should call this command to parse the gene id from the log file. " &
                                 "You can also done this id parsing job using other tools.",
        Usage:="grep -i <xml_log_file> -q <script_statements> -h <script_statements>",
        Example:="grep -i C:\Users\WORKGROUP\Desktop\blast_xml_logs\1__8004_ecoli_prot.log.xml -q ""tokens | 4"" -h ""'tokens | 2';'tokens ' ' 0'""")>
    <Argument("-q", False,
        Description:="The parsing script for parsing the gene_id from the blast log file, this switch value is consist of sevral operation " &
                     "tokens, and each token is separate by the ';' character and the token unit in each script token should seperate by " &
                     "the ' character.\n" &
                     "There are two basic operation in this parsing script:\n" &
                     " tokens - Split the query or hit name string into sevral piece of string by the specific delimiter character and " &
                     "          get the specifc location unit in the return string array.\n" &
                     "   Usage:   tokens <delimiter> <position>\n" &
                     "   Example: tokens | 3" &
                     " match - match a gene id using a specific pattern regular expression.\n" &
                     "   usage:   match <regular_expression>\n" &
                     "   Example: match .+[-]\d{5}")>
    <Argument("-h",
        Description:="The parsing script for parsing the gene_id from the blast log file, this switch value is consist of sevral operation " &
                     "tokens, and each token is separate by the ';' character and the token unit in each script token should seperate by " &
                     "the ' character.\n" &
                     "There are two basic operation in this parsing script:\n" &
                     " tokens - Split the query or hit name string into sevral piece of string by the specific delimiter character and " &
                     "          get the specifc location unit in the return string array.\n" &
                     "   Usage:   tokens <delimiter> <position>\n" &
                     "   Example: tokens | 3" &
                     " match - match a gene id using a specific pattern regular expression.\n" &
                     "   usage:   match <regular_expression>\n" &
                     "   Example: match .+[-]\d{5}")>
    Public Function Grep(args As CommandLine) As Integer
        Dim GrepScriptQuery As TextGrepScriptEngine = TextGrepScriptEngine.Compile(args("-q"))
        Dim GrepScriptHit As TextGrepScriptEngine = TextGrepScriptEngine.Compile(args("-h"))
        Dim XmlFile As String = args("-i")

        If String.IsNullOrEmpty(XmlFile) Then
            Return -1
        End If

        Using File As Legacy.BLASTOutput = Legacy.BLASTOutput.Load(XmlFile) 'Depose 操作的时候会自动保存
            Call File.Grep(Query:=AddressOf GrepScriptQuery.Grep, Hits:=AddressOf GrepScriptHit.Grep)
        End Using

        Return 0
    End Function
End Module
