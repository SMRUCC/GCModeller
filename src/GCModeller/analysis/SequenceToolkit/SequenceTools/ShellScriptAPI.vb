#Region "Microsoft.VisualBasic::2f93acba76ec179afa15775f44ba9099, analysis\SequenceToolkit\SequenceTools\ShellScriptAPI.vb"

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

    ' Module ShellScriptAPI
    ' 
    '     Function: Align, GetObject, GetSequenceData, MatchLocation, PatternSearch
    '               PatternSearchA, ReadFile, (+2 Overloads) Reverse, SearchByTitleKeyword, WriteFile
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Pattern
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.FASTA.Reflection

<Package("SequenceTools",
                    Category:=APICategories.ResearchTools,
                    Description:="Sequence search tools and sequence operation tools",
                    Publisher:="xie.guigang@gmail.com")>
Public Module ShellScriptAPI

    <ExportAPI("search.title_keyword")>
    Public Function SearchByTitleKeyword(fasta As FastaFile, Keyword As String) As FastaFile
        Dim LQuery As FastaSeq() =
            LinqAPI.Exec(Of FastaSeq) <= From fa As FastaSeq
                                           In fasta
                                           Where InStr(fa.Title, Keyword, CompareMethod.Binary) > 0
                                           Select fa
        Return LQuery
    End Function

    <ExportAPI("reverse")>
    Public Function Reverse(fasta As FastaFile) As FastaFile
        Return fasta.Reverse
    End Function

    <ExportAPI("reverse")>
    Public Function Reverse(fasta As FastaSeq) As FastaFile
        Return fasta.Reverse
    End Function

    <ExportAPI("Read.Fasta")>
    Public Function ReadFile(file As String) As FastaFile
        Return FastaFile.Read(file)
    End Function

    <ExportAPI("write.fasta")>
    Public Function WriteFile(fasta As FastaFile, file As String) As Boolean
        Return fasta.Save(file)
    End Function

    <ExportAPI("get_fasta")>
    Public Function GetObject(fasta As FastaFile, index As Integer) As FastaSeq
        Return fasta.Item(index)
    End Function

    <ExportAPI("get_sequence")>
    Public Function GetSequenceData(fsa As FastaSeq) As String
        Return fsa.SequenceData
    End Function

    ''' <summary>
    ''' 使用正则表达式搜索目标序列
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("-Pattern_Search", Info:="Parsing the sequence segment from the sequence source using regular expression.",
        Usage:="-pattern_search fasta <fasta_object> pattern <regex_pattern> output <output_directory>")>
    <ArgumentAttribute("-p",
        Description:="This switch specific the regular expression pattern for search the sequence segment,\n" &
                     "for more detail information about the regular expression please read the user manual.")>
    <ArgumentAttribute("-o", True,
        Description:="Optional, this switch value specific the output directory for the result data, default is user Desktop folder.")>
    Public Function PatternSearchA(fasta As FastaFile, pattern As String, outDIR As String) As Integer
        pattern = pattern.Replace("N", "[ATGCU]")

        If String.IsNullOrEmpty(outDIR) Then
            outDIR = App.Desktop
        End If

        Dim Csv = SequencePatterns.Pattern.Match(Seq:=fasta, pattern:=pattern)
        Dim Complement = SequencePatterns.Pattern.Match(Seq:=fasta.Complement(), pattern:=pattern)
        Dim Reverse = SequencePatterns.Pattern.Match(Seq:=fasta.Reverse, pattern:=pattern)

        Call Csv.Insert(rowId:=-1, Row:={"Match pattern:=", pattern})
        Call Complement.Insert(rowId:=-1, Row:={"Match pattern:=", pattern})
        Call Reverse.Insert(rowId:=-1, Row:={"Match pattern:=", pattern})

        Call Csv.Save(outDIR & "/sequence.csv", False)
        Call Complement.Save(outDIR & "/sequence_complement.csv", False)
        Call Reverse.Save(outDIR & "/sequence_reversed.csv", False)

        Return 0
    End Function

    <ExportAPI("Search")>
    Public Function PatternSearch(Fasta As FastaSeq, Pattern As String) As SegLoci()
        Throw New NotImplementedException
    End Function

    <ExportAPI("loci.match.location")>
    Public Function MatchLocation(seq As String, loci As String, Optional cutoff As Double = 0.65) As Topologically.SimilarityMatches.LociMatchedResult()
        Return Topologically.SimilarityMatches.MatchLociLocations(seq, loci, Len(loci) / 3, Len(loci) * 5, cutoff)
    End Function

    <ExportAPI("Align")>
    Public Function Align(query As FastaSeq, subject As FastaSeq, Optional cost As Double = 0.7) As AlignmentResult
        Return New AlignmentResult(query, subject, cost)
    End Function
End Module
