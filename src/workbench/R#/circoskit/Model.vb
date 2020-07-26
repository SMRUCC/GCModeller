#Region "Microsoft.VisualBasic::1eac4cb293051c97df508fd1c8efab68, circoskit\Model.vb"

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
'     Function: SetIdeogramRadius
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
        Dim geneTable As pipeline = pipeline.TryCreatePipeline(Of GeneTable)(genes, env)
        Dim geneColors As Dictionary(Of String, String) = colors.AsGeneric(Of String)(env)

        If geneTable.isError Then
            Return geneTable.getError
        End If

        Return New GeneMark(geneTable.populates(Of GeneTable)(env), geneColors)
    End Function
End Module
