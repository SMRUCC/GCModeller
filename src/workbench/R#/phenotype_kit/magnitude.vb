#Region "Microsoft.VisualBasic::9094e5cee511831daa90325d92d93503, R#\phenotype_kit\magnitude.vb"

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

'   Total Lines: 28
'    Code Lines: 19
' Comment Lines: 3
'   Blank Lines: 6
'     File Size: 936 B


' Module magnitude
' 
'     Function: profiles
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler.TrIQ
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.Rsharp.Runtime
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix

''' <summary>
''' HTS expression data simulating for analysis test
''' </summary>
<Package("magnitude", Category:=APICategories.UtilityTools)>
Module magnitude

    ''' <summary>
    ''' tag samples in matrix as sequence profiles
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <param name="custom">
    ''' use the custom charset, then the generated sequence
    ''' data can only be processed via the SGT algorithm
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("encode.seqPack")>
    Public Function encode_seqPack(mat As Matrix,
                                   Optional briefSet As Boolean = True,
                                   Optional custom As String = Nothing,
                                   Optional env As Environment = Nothing) As Object

        Dim charSet As String = If(
            briefSet,
            SequenceModel.NT.JoinBy(""),
            SequenceModel.AA.JoinBy("")
        )

        If Not custom.StringEmpty Then
            charSet = custom
        End If

        Dim charMap = mat.EncodeRanking.EncodeMatrix(
            charSet:=charSet
        )
        Dim pack = mat.AsSequenceSet(charMap).ToArray
        Dim println = env.WriteLineHandler
        Dim dist = charMap _
            .GroupBy(Function(c) c.Value) _
            .ToDictionary(Function(c) c.Key.ToString,
                          Function(c)
                              Return c.Select(Function(f) f.Key).Distinct.ToArray
                          End Function)

        Call println("inspect the charset distribution:")
        Call env.globalEnvironment.Rscript.Inspect(dist)

        Return pack
    End Function

    ''' <summary>
    ''' Apply TrIQ cutoff for each sample
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <param name="q"></param>
    ''' <param name="axis">
    ''' default value ``1`` means apply the cutoff for each sample column data,
    ''' alternative value ``2`` means apply the cutoff for each gene data row. 
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("TrIQ.apply")>
    Public Function triq(mat As Matrix, Optional q As Double = 0.8, Optional axis As Integer = 1) As Matrix
        If axis = 1 Then
            Return mat.applySample(q)
        Else
            Return mat.applyFeature(q)
        End If

        Return mat
    End Function

    <Extension>
    Private Function applyFeature(mat As Matrix, q As Double) As Matrix
        For Each gene As DataFrameRow In mat.expression
            Dim v As Vector = gene.experiments
            Dim cut As Double = v.FindThreshold(q)

            v(v > cut) = Vector.Scalar(cut)

            gene.experiments = v.ToArray
        Next

        Return mat
    End Function

    <Extension>
    Private Function applySample(mat As Matrix, q As Double) As Matrix
        For Each sample_id As String In mat.sampleID
            Dim v As Vector = mat.sample(sample_id)
            Dim cut As Double = v.FindThreshold(q)
            Dim i As Integer = mat.sampleID.IndexOf(sample_id)

            v(v > cut) = Vector.Scalar(cut)

            For j As Integer = 0 To mat.expression.Length - 1
                Dim gene = mat.expression(j)
                Dim u = gene.experiments

                u(i) = v(j)
            Next
        Next

        Return mat
    End Function
End Module
