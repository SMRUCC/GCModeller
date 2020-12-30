#Region "Microsoft.VisualBasic::58ca2313856e02813eb4d5520c16757a, seqtoolkit\Blast.vb"

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

' Module Blast
' 
'     Function: doAlign, gwANIMultipleAlignment, ParseBlosumMatrix, RunGlobalNeedlemanWunsch
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime.Internal

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
    ''' Do sequence global pairwise alignment
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="ref"></param>
    ''' <returns></returns>
    <ExportAPI("align.needleman_wunsch")>
    Public Function RunGlobalNeedlemanWunsch(query As FastaSeq, ref As FastaSeq) As FactorValue(Of Double, GlobalAlign(Of Char)())
        Dim score As Double = 0
        Dim alignments = RunNeedlemanWunsch.RunAlign(query, ref, score).ToArray

        Return (score, alignments)
    End Function

    <ExportAPI("align.gwANI")>
    Public Function gwANIMultipleAlignment(multipleSeq As FastaFile) As DataSet()
        Return gwANI.calculate_and_output_gwani(multipleSeq)
    End Function
End Module
