#Region "Microsoft.VisualBasic::fb36be3c828daeab26236f39b8a6a47b, ..\GCModeller\annotations\Proteomics\Plots.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
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

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot.Histogram
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Public Module Plots

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="input"></param>
    ''' <param name="obo$"></param>
    ''' <param name="p#">Default cutoff is ``-<see cref="Math.Log10(Double)"/>(<see cref="EnrichmentTerm.Pvalue"/>) &lt;= 0.05``</param>
    ''' <returns></returns>
    <Extension>
    Public Function GOEnrichmentPlot(input As IEnumerable(Of EnrichmentTerm), obo$, Optional p# = 0.05) As Bitmap
        Dim GO_terms As Dictionary(Of String, Term) = GO_OBO _
            .Open(obo) _
            .ToDictionary(Function(x) x.id)
        Dim getData = Function(gene As EnrichmentTerm) (gene.ID, gene.number)
        Return input.Where(Function(x) x.P <= p#).Plot(getData, GO_terms)
    End Function

    <Extension>
    Public Function VennData(files As IEnumerable(Of String)) As IO.File
        Dim datas = files.ToDictionary(Function(f) f.BaseName, Function(f) f.LoadSample.ToDictionary)
        Dim out As New IO.File
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

            For Each sample In keys
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
    ''' <param name="tag$"></param>
    ''' <param name="serialTitle$"></param>
    ''' <param name="step!"></param>
    ''' <returns></returns>
    <Extension>
    Public Function logFCHistogram(data As IEnumerable(Of EntityObject),
                                   Optional tag$ = "logFC",
                                   Optional serialTitle$ = "Frequency(logFC)",
                                   Optional step! = 1) As Bitmap
        Dim logFC#() = data _
            .Select(Function(prot) prot(tag).ParseNumeric) _
            .ToArray

        Return logFC.HistogramPlot(
            [step],
            serialsTitle:=serialTitle)
    End Function
End Module

