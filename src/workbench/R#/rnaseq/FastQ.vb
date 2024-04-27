﻿#Region "Microsoft.VisualBasic::e0345b4308cb7b5b938b2555a3897e56, G:/GCModeller/src/workbench/R#/rnaseq//FastQ.vb"

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

    '   Total Lines: 95
    '    Code Lines: 60
    ' Comment Lines: 28
    '   Blank Lines: 7
    '     File Size: 4.12 KB


    ' Module FastQ
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetQualityScore, SequenceAssembler
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' FastQ toolkit
''' </summary>
''' <remarks>
''' FASTQ format Is a text-based format For storing both a biological sequence 
''' (usually nucleotide sequence) And its corresponding quality scores. Both 
''' the sequence letter And quality score are Each encoded With a Single ASCII 
''' character For brevity. It was originally developed at the Wellcome Trust 
''' Sanger Institute To bundle a FASTA formatted sequence And its quality data, 
''' but has recently become the de facto standard For storing the output Of 
''' high-throughput sequencing instruments such As the Illumina Genome 
''' Analyzer.
''' </remarks>
<Package("FastQ")>
Public Module FastQ

    Sub New()
        Call printer.AttachConsoleFormatter(Of AssembleResult)(AddressOf AssembleResult.viewAssembles)
    End Sub

    ''' <summary>
    ''' Do short reads assembling
    ''' </summary>
    ''' <param name="reads">should be a set of the sequence data, example as a collection of <see cref="FastaSeq"/> data.</param>
    ''' <param name="env"></param>
    ''' <returns>the short reads assembling result</returns>
    <ExportAPI("assemble")>
    <RApiReturn(GetType(AssembleResult))>
    Public Function SequenceAssembler(<RRawVectorArgument> reads As Object, Optional env As Environment = Nothing) As Object
        Dim readSeqs As FastaSeq() = GetFastaSeq(reads, env).ToArray
        Dim data As String() = readSeqs _
            .Select(Function(fa) fa.SequenceData) _
            .ToArray
        Dim result = data.ShortestCommonSuperString

        Return New AssembleResult(result, data)
    End Function

    ''' <summary>
    ''' In FASTQ files, quality scores are encoded into a compact form, 
    ''' which uses only 1 byte per quality value. In this encoding, the 
    ''' quality score is represented as the character with an ASCII 
    ''' code equal to its value + 33.
    ''' </summary>
    ''' <param name="q">should be one or more <see cref="FQ.FastQ"/> sequence data</param>
    ''' <param name="env"></param>
    ''' <returns>the quality score data of each <see cref="FQ.FastQ"/> sequence data.</returns>
    <ExportAPI("quality_score")>
    <RApiReturn(GetType(Double))>
    Public Function GetQualityScore(q As Object, Optional env As Environment = Nothing) As Object
        If q Is Nothing Then
            Return Nothing
        End If

        If TypeOf q Is String Then
            Return FQ.FastQ _
                .GetQualityOrder(CStr(q)) _
                .Select(Function(d) CDbl(d)) _
                .ToArray
        ElseIf TypeOf q Is FastQFile Then
            Return New list With {
                .slots = DirectCast(q, FastQFile) _
                    .ToDictionary(Function(i) i.Title,
                                  Function(i)
                                      Dim scores = FQ.FastQ _
                                         .GetQualityOrder(i.Quality) _
                                         .Select(Function(d) CDbl(d)) _
                                         .ToArray

                                      Return CObj(scores)
                                  End Function)
            }
        ElseIf TypeOf q Is FQ.FastQ Then
            Return FQ.FastQ _
                .GetQualityOrder(DirectCast(q, FQ.FastQ).Quality) _
                .Select(Function(d) CDbl(d)) _
                .ToArray
        Else
            Return Message.InCompatibleType(GetType(FQ.FastQ), q.GetType, env)
        End If
    End Function
End Module

