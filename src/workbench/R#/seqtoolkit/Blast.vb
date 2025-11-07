#Region "Microsoft.VisualBasic::7613257916436c22dcfcb01ec3b823f8, R#\seqtoolkit\Blast.vb"

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

    '   Total Lines: 116
    '    Code Lines: 76 (65.52%)
    ' Comment Lines: 28 (24.14%)
    '    - Xml Docs: 96.43%
    ' 
    '   Blank Lines: 12 (10.34%)
    '     File Size: 4.78 KB


    ' Module Blast
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: doAlign, gwANIMultipleAlignment, HSP_hits, ParseBlosumMatrix, RunGlobalNeedlemanWunsch
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime.Internal
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe

''' <summary>
''' Blast search tools
''' </summary>
<Package("bioseq.blast")>
Module Blast

    Sub New()
        Call ConsolePrinter.AttachConsoleFormatter(Of SmithWaterman)(
            Function(sw)
                Dim text As New StringBuilder

                Using str As New StringWriter(text)
                    Call DirectCast(sw, SmithWaterman).printAlignments(dev:=str)
                End Using

                Return text.ToString
            End Function)
    End Sub

    ''' <summary>
    ''' Parse blosum from the given file data
    ''' </summary>
    ''' <param name="file">The blosum text data or text file path.</param>
    ''' <returns></returns>
    <ExportAPI("blosum")>
    Public Function ParseBlosumMatrix(Optional file$ = "Blosum-62") As Blosum
        If file = "Blosum-62" AndAlso Not file.FileExists Then
            Return Blosum.FromInnerBlosum62
        Else
            Return BlosumParser.LoadFromStream(file.SolveStream)
        End If
    End Function

    ''' <summary>
    ''' Do sequence pairwise alignment
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="ref"></param>
    ''' <param name="blosum"></param>
    ''' <returns></returns>
    <ExportAPI("align.smith_waterman")>
    Public Function doAlign(query As FastaSeq, ref As FastaSeq, Optional blosum As Blosum = Nothing) As SmithWaterman
        Return SmithWaterman.Align(query, ref, blosum)
    End Function

    ''' <summary>
    ''' get the high score region from the given alignment result
    ''' </summary>
    ''' <param name="align"></param>
    ''' <param name="cutoff">[0,1] threshold</param>
    ''' <param name="minW"></param>
    ''' <returns></returns>
    <ExportAPI("HSP")>
    Public Function HSP_hits(align As SmithWaterman, cutoff As Double, minW As Integer) As Object
        Dim outputs As Output = align.GetOutput(cutoff, minW)
        Dim query As String() = outputs.HSP.Select(Function(r) r.Query).ToArray
        Dim subject As String() = outputs.HSP.Select(Function(r) r.Subject).ToArray
        Dim queryLen As Integer() = outputs.HSP.Select(Function(r) r.QueryLength).ToArray
        Dim subjectLen As Integer() = outputs.HSP.Select(Function(r) r.SubjectLength).ToArray
        Dim lenQuery As Integer() = outputs.HSP.Select(Function(r) r.LengthQuery).ToArray
        Dim lenHit As Integer() = outputs.HSP.Select(Function(r) r.LengthHit).ToArray
        Dim hspQuery As String() = outputs.HSP.Select(Function(r) $"{r.fromA}..{r.toA}").ToArray
        Dim hspSubject As String() = outputs.HSP.Select(Function(r) $"{r.fromB}..{r.toB}").ToArray
        Dim score As Double() = outputs.HSP.Select(Function(r) r.score).ToArray
        Dim coverage As Double() = outputs.HSP.Select(Function(r) r.LengthQuery / r.QueryLength).ToArray

        Return New Rdataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"query", query},
                {"subject", subject},
                {"query_length", queryLen},
                {"subject_length", subjectLen},
                {"length_query", lenQuery},
                {"length_hit", lenHit},
                {"hsp_query", hspQuery},
                {"hsp_subject", hspSubject},
                {"score", score},
                {"coverage", coverage}
            }
        }
    End Function

    ''' <summary>
    ''' Do sequence global pairwise alignment
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="ref"></param>
    ''' <returns></returns>
    <ExportAPI("align.needleman_wunsch")>
    Public Function RunGlobalNeedlemanWunsch(query As FastaSeq, ref As FastaSeq) As FactorValue(Of Double, GlobalAlign(Of Char)())
        Dim score As Double = 0
        Dim alignments = RunNeedlemanWunsch.RunAlign(query, ref, score).ToArray

        Return FactorValue(Of Double, GlobalAlign(Of Char)()).Create(score, alignments)
    End Function

    <ExportAPI("align.gwANI")>
    Public Function gwANIMultipleAlignment(multipleSeq As FastaFile) As DataSet()
        Return gwANI.calculate_and_output_gwani(multipleSeq)
    End Function
End Module
