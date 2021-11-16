#Region "Microsoft.VisualBasic::638b2576b47eff935064642a0f0240f2, CLI_tools\S.M.A.R.T\CLI\Export.vb"

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

    ' Module CLI
    ' 
    '     Function: (+2 Overloads) Export
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    ''' <summary>
    ''' 从目标蛋白质结构域数据库之中导出包含有目标关键词的序列数据
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("export", Info:="", Usage:="export -keyword <keyword_list> [-m <any/all>] -o <export_file> [-d <db_name> -casesense <T/F>]", Example:="")>
    <ArgumentAttribute("-d", False,
        Description:="This switch value can be both a domain database name or a fasta file path.")>
    <ArgumentAttribute("-keyword",
        Description:="The keyword list will be use for the sequence record search, each keyword should seperated by comma character.")>
    Public Function Export(args As CommandLine) As Integer
        Dim Db As String = args("-d")
        Dim Keywords As String() = args("-keyword").Split(",")
        Dim Output As String = args("-o")
        Dim CaseSense As CompareMethod = IIf(String.Equals(args("-casesense"), "T"), CompareMethod.Binary, CompareMethod.Text)
        Dim IsMethodAny As Boolean = IIf(String.Equals(args("-m"), "any"), True, False)

        Call FileIO.FileSystem.CreateDirectory(FileIO.FileSystem.GetParentPath(Output))

        If String.IsNullOrEmpty(Db) OrElse String.Equals(Db, "all") Then

            Dim List As New List(Of FastaSeq)
            For Each Db In New NCBI.CDD.Database("").Paths
                Call List.AddRange(Export(Db, Keywords, CaseSense, IsMethodAny))
            Next
            Dim Saved = CType(List, FastaFile).Distinct
            Call Saved.Save(Output)
            Call FileIO.FileSystem.WriteAllText(Output & "_idlist.txt", Saved.GetIdList, append:=False)
        Else

            If Not FileIO.FileSystem.FileExists(Db) Then '使用的是一个指定的文件
                Using Database As NCBI.CDD.Database = New NCBI.CDD.Database("")
                    Db = Database.Db(Db)
                End Using
            End If

            Dim List = CType(Export(Db, Keywords, CaseSense, IsMethodAny), SMRUCC.genomics.SequenceModel.FASTA.FastaFile).Distinct
            Call List.Save(Output)
            Call FileIO.FileSystem.WriteAllText(Output & "_idlist.txt", List.GetIdList, append:=False)
        End If

        Return 0
    End Function

    Private Function Export(DB As String, Keywords As String(), CaseSense As CompareMethod, Any As Boolean) As SMRUCC.genomics.SequenceModel.FASTA.FastaSeq()
        Dim FASTAFile As SMRUCC.genomics.SequenceModel.FASTA.FastaFile = SMRUCC.genomics.SequenceModel.FASTA.FastaFile.Read(DB)
        Dim Query As Generic.IEnumerable(Of SMRUCC.genomics.SequenceModel.FASTA.FastaSeq)
        If Any Then
            Query = From fsa In FASTAFile.AsParallel Where fsa.Title.ContainsAny(Keywords, CaseSense) Select fsa '
        Else
            Query = From fsa In FASTAFile.AsParallel Where fsa.Title.ContainsKeyword(Keywords, CaseSense) Select fsa '
        End If
        Return Query.ToArray
    End Function
End Module
