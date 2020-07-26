#Region "Microsoft.VisualBasic::3717f6c0a907abb2e40564c9860952c8, circoskit\Model.vb"

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

    ' Module Model
    ' 
    '     Function: CreateGCContent, CreateGCSkewPlots, GeneMarks, HeatMapping, VariantsHighlights
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.Circos
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel

''' <summary>
''' package module for generates plot data
''' </summary>
<Package("model", Category:=APICategories.UtilityTools)>
Module Model

    <ExportAPI("highlight.heatmapping")>
    <RApiReturn(GetType(Highlights))>
    Public Function HeatMapping(<RRawVectorArgument> values As Object, Optional colors$ = ColorMap.PatternJet, Optional env As Environment = Nothing) As Object
        Dim valuePoints As pipeline = pipeline.TryCreatePipeline(Of ValueTrackData)(values, env)

        If valuePoints.isError Then
            Return valuePoints.getError
        End If

        Dim model As New GradientMappings(valuePoints.populates(Of ValueTrackData)(env), mapName:=colors)
        Return model
    End Function

    <ExportAPI("highlight.genemarks")>
    <RApiReturn(GetType(Highlights))>
    Public Function GeneMarks(<RRawVectorArgument> genes As Object, colors As list, Optional env As Environment = Nothing) As Object
        Dim geneTable As pipeline = pipeline.TryCreatePipeline(Of IGeneBrief)(genes, env)
        Dim geneColors As Dictionary(Of String, String) = colors.AsGeneric(Of String)(env)

        If geneTable.isError Then
            Return geneTable.getError
        End If

        Return New GeneMark(geneTable.populates(Of IGeneBrief)(env), geneColors)
    End Function

    ''' <summary>
    ''' creates the ``GC%`` content for circos plots.
    ''' </summary>
    ''' <param name="nt">
    ''' The original nt sequence in the fasta format for the calculation of the 
    ''' ``GC%`` content in each slidewindow
    ''' </param>
    ''' <param name="win_size%"></param>
    ''' <param name="steps%"></param>
    ''' <returns></returns>
    <ExportAPI("GC_content")>
    Public Function CreateGCContent(nt As FastaSeq, win_size%, steps%) As NtProps.GenomeGCContent
        Return New NtProps.GenomeGCContent(nt, win_size, steps)
    End Function

    ''' <summary>
    ''' Creates the circos circle plots of the genome gcskew.
    ''' </summary>
    ''' <param name="nt"></param>
    ''' <param name="win_size"></param>
    ''' <param name="steps"></param>
    ''' <returns></returns>
    <ExportAPI("gcSkew")>
    Public Function CreateGCSkewPlots(nt As IPolymerSequenceModel, win_size As Integer, steps As Integer) As NtProps.GCSkew
        Return New NtProps.GCSkew(nt, win_size, steps, True)
    End Function

    <ExportAPI("ntVariants")>
    Public Function VariantsHighlights(fasta As FastaFile, Optional index As Integer = Scan0, Optional step% = 1) As NtProps.GCSkew
        Dim var As Double() = Patterns.NTVariations(fasta, index)
        Dim node As New NtProps.GCSkew(var, [step])
        Return node
    End Function
End Module
