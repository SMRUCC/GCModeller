#Region "Microsoft.VisualBasic::9c7c775361ed4aa66f744cdf9cae4a7f, GCModeller\annotations\Proteomics\Plots.vb"

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

    '   Total Lines: 110
    '    Code Lines: 81
    ' Comment Lines: 16
    '   Blank Lines: 13
    '     File Size: 4.24 KB


    ' Module Plots
    ' 
    '     Function: GOEnrichmentPlot, logFCHistogram, VennData
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot.Histogram
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports csv = Microsoft.VisualBasic.Data.csv.IO.File

Public Module Plots

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="input"></param>
    ''' <param name="obo$"></param>
    ''' <param name="p#">Default cutoff is ``-<see cref="Math.Log10(Double)"/>(<see cref="EnrichmentTerm.Pvalue"/>) &lt;= 0.05``</param>
    ''' <returns></returns>
    <Extension>
    Public Function GOEnrichmentPlot(input As IEnumerable(Of EnrichmentTerm), obo$, Optional p# = 0.05) As GraphicsData
        Dim GO_terms As Dictionary(Of String, Term) = GO_OBO _
            .Open(obo) _
            .ToDictionary(Function(x) x.id)
        Dim getData = Function(gene As EnrichmentTerm) (gene.ID, gene.number)

        Return input _
            .Where(Function(x) x.P <= p#) _
            .Plot(getData, GO_terms)
    End Function

    <Extension> Public Function VennData(files As IEnumerable(Of String)) As csv
        Dim datas = files.ToDictionary(Function(f) f.BaseName, Function(f) f.LoadSample.ToDictionary)
        Dim out As New csv
        Dim keys$() = datas.Keys.ToArray
        Dim ALL_locus$() = datas _
            .Select(Function(x) x.Value.Select(Function(o) o.Value.ID)) _
            .Unlist _
            .Distinct _
            .OrderBy(Function(s) s) _
            .ToArray

        Call out.AppendLine(keys)

        For Each ID As String In ALL_locus
            Dim row As New List(Of String)

            For Each sample As String In keys
                If datas(sample).ContainsKey(ID) Then
                    row += ID
                Else
                    row += ""
                End If
            Next

            Call out.Add(row)
        Next

        Return out
    End Function

    ''' <summary>
    ''' 假若没有生物学重复，则可以用这个函数进行logFC的分布情况的绘制，
    ''' 但是假若数据是有生物学重复，即可以计算出pvalue的时候，通常是绘制火山图
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="serialTitle$"></param>
    ''' <param name="step!"></param>
    ''' <returns></returns>
    <Extension>
    Public Function logFCHistogram(data As IEnumerable(Of DEP_iTraq),
                                   Optional serialTitle$ = "Frequency(log2FC)",
                                   Optional step! = 1,
                                   Optional size$ = "1600,1200",
                                   Optional padding$ = "padding: 100 180 100 180",
                                   Optional xAxis$ = Nothing,
                                   Optional color$ = "lightblue",
                                   Optional type$ = NameOf(DEP_iTraq.log2FC)) As GraphicsData

        Dim histData As HistogramData() = Nothing
        Dim logFC As Vector

        If type.TextEquals(NameOf(DEP_iTraq.log2FC)) Then
            logFC = data.Shadows!log2FC
        Else
            logFC = data.Shadows!FCavg
        End If

        Try
            Return logFC.HistogramPlot(
                [step],
                serialsTitle:=serialTitle,
                histData:=histData,
                size:=size,
                padding:=padding,
                xLabel:="log2FC",
                yLabel:=serialTitle,
                xAxis:=xAxis,
                color:=color,
                showLegend:=False)
        Catch ex As Exception
            ' 有时候标签没有设置正确会导致得到的向量全部为0，则绘图会出错，这个时候显示一下调试信息
            Dim msg$ = $"tag={NameOf(DEP_iTraq.log2FC)}, vector={Mid(logFC.GetJson, 1, 256)}..., hist={Mid(histData.GetJson, 1, 300)}..."
            Throw New Exception(msg, ex)
        End Try
    End Function
End Module
