#Region "Microsoft.VisualBasic::e7b7326893c1ff4cefba55497fe68fc4, ..\GCModeller\CLI_tools\S.M.A.R.T\CLI\Export.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic

Partial Module CLI

    ''' <summary>
    ''' 从目标蛋白质结构域数据库之中导出包含有目标关键词的序列数据
    ''' </summary>
    ''' <param name="CommandLine"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("export", Info:="", Usage:="export -keyword <keyword_list> [-m <any/all>] -o <export_file> [-d <db_name> -casesense <T/F>]", Example:="")>
    <ParameterInfo("-d", False,
        Description:="This switch value can be both a domain database name or a fasta file path.",
        Example:="")>
    <ParameterInfo("-keyword",
        Description:="The keyword list will be use for the sequence record search, each keyword should seperated by comma character.",
        Example:="HTH,GGDEF,Clp,REC")>
    Public Function Export(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Dim Db As String = CommandLine("-d")
        Dim Keywords As String() = CommandLine("-keyword").Split(CChar(","))
        Dim Output As String = CommandLine("-o")
        Dim CaseSense As CompareMethod = IIf(String.Equals(CommandLine("-casesense"), "T"), CompareMethod.Binary, CompareMethod.Text)
        Dim IsMethodAny As Boolean = IIf(String.Equals(CommandLine("-m"), "any"), True, False)

        Call FileIO.FileSystem.CreateDirectory(FileIO.FileSystem.GetParentPath(Output))

        If String.IsNullOrEmpty(Db) OrElse String.Equals(Db, "all") Then

            Dim List As List(Of SMRUCC.genomics.SequenceModel.FASTA.FastaToken) = New List(Of SequenceModel.FASTA.FastaToken)
            For Each Db In New Assembly.NCBI.CDD.Database("").Paths
                Call List.AddRange(Export(Db, Keywords, CaseSense, IsMethodAny))
            Next
            Dim Saved = CType(List, SMRUCC.genomics.SequenceModel.FASTA.FastaFile).Distinct
            Call Saved.Save(Output)
            Call FileIO.FileSystem.WriteAllText(Output & "_idlist.txt", Saved.GetIdList, append:=False)
        Else

            If Not FileIO.FileSystem.FileExists(Db) Then '使用的是一个指定的文件
                Using Database As Assembly.NCBI.CDD.Database = New Assembly.NCBI.CDD.Database("")
                    Db = Database.Db(Db)
                End Using
            End If

            Dim List = CType(Export(Db, Keywords, CaseSense, IsMethodAny), SMRUCC.genomics.SequenceModel.FASTA.FastaFile).Distinct
            Call List.Save(Output)
            Call FileIO.FileSystem.WriteAllText(Output & "_idlist.txt", List.GetIdList, append:=False)
        End If

        Return 0
    End Function

    Private Function Export(DB As String, Keywords As String(), CaseSense As CompareMethod, Any As Boolean) As SMRUCC.genomics.SequenceModel.FASTA.FastaToken()
        Dim FASTAFile As SMRUCC.genomics.SequenceModel.FASTA.FastaFile = SMRUCC.genomics.SequenceModel.FASTA.FastaFile.Read(DB)
        Dim Query As Generic.IEnumerable(Of SMRUCC.genomics.SequenceModel.FASTA.FastaToken)
        If Any Then
            Query = From fsa In FASTAFile.AsParallel Where fsa.Title.ContainsAny(Keywords, CaseSense) Select fsa '
        Else
            Query = From fsa In FASTAFile.AsParallel Where fsa.Title.ContainsKeyword(Keywords, CaseSense) Select fsa '
        End If
        Return Query.ToArray
    End Function

    <ExportAPI("convert", Usage:="convert -i <input_file> [-o <xml_file>]")>
    Public Function Convert(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Dim Log As String = CommandLine("-i")
        Dim Saved As String = CommandLine("-o")

        Dim BlastLog As NCBI.Extensions.LocalBLAST.BLASTOutput.Standard.BLASTOutput = NCBI.Extensions.LocalBLAST.BLASTOutput.Standard.BLASTOutput.TryParse(Log)
        Call BlastLog.Save(Saved)
        Return 0
    End Function

    ''' <summary>
    ''' 解析BLAST日志文件中的标记号名称
    ''' </summary>
    ''' <param name="CommandLine"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("grep", Info:="The gene id in the blast output log file are not well format for reading and program processing, so " &
                                 "before you generate the venn diagram you should call this command to parse the gene id from the log file. " &
                                 "You can also done this id parsing job using other tools.",
        Usage:="grep -i <xml_log_file> -q <script_statements> -h <script_statements>",
        Example:="grep -i C:\Users\WORKGROUP\Desktop\blast_xml_logs\1__8004_ecoli_prot.log.xml -q ""tokens | 4"" -h ""'tokens | 2';'tokens ' ' 0'""")>
    <ParameterInfo("-q",
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
                     "   Example: match .+[-]\d{5}",
        Example:="'tokens | 5';'match .+[-].+'")>
    <ParameterInfo("-h", False,
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
                     "   Example: match .+[-]\d{5}",
        Example:="'tokens | 5';'match .+[-].+'")>
    Public Function Grep(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Dim GrepScriptQuery As Microsoft.VisualBasic.Text.TextGrepScriptEngine = Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile(CommandLine("-q"))
        Dim GrepScriptHit As Microsoft.VisualBasic.Text.TextGrepScriptEngine = Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile(CommandLine("-h"))
        Dim XmlFile As String = CommandLine("-i")

        If String.IsNullOrEmpty(XmlFile) Then
            Return -1
        End If

        Using File As NCBI.Extensions.LocalBLAST.BLASTOutput.Standard.BLASTOutput = NCBI.Extensions.LocalBLAST.BLASTOutput.Standard.BLASTOutput.Load(XmlFile) 'Depose 操作的时候会自动保存
            Call File.Grep(AddressOf GrepScriptQuery.Grep, AddressOf GrepScriptHit.Grep)
        End Using

        Return 0
    End Function
End Module
