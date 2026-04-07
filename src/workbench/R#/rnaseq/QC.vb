#Region "Microsoft.VisualBasic::d60323418879b5ad077ac9e161bbb8c5, R#\rnaseq\QC.vb"

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

    '   Total Lines: 79
    '    Code Lines: 51 (64.56%)
    ' Comment Lines: 13 (16.46%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 15 (18.99%)
    '     File Size: 3.10 KB


    ' Module QC
    ' 
    '     Function: nano_plot, nano_report, TrimLowQuality, TrimReadsHeaders
    ' 
    ' /********************************************************************************/

#End Region


Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.genomics.SequenceModel.FQ.NanoPlot
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("QC")>
Module QC

    <ExportAPI("trim_low_quality")>
    Public Function TrimLowQuality(<RRawVectorArgument> reads As Object, Optional quality% = 20, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of FQ.FastQ)(reads, env)

        If pull.isError Then
            Return pull.getError
        End If

        Return pipeline.CreateFromPopulator(pull.populates(Of FQ.FastQ)(env).TrimLowQuality)
    End Function

    ''' <summary>
    ''' trim the primers or adapters sequence at read header region
    ''' </summary>
    ''' <param name="reads"></param>
    ''' <param name="headers"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("trim_reads_headers")>
    Public Function TrimReadsHeaders(<RRawVectorArgument> reads As Object, <RRawVectorArgument> headers As Object,
                                     Optional exact_match As Boolean = True,
                                     Optional cutoff As Double = 0.85,
                                     Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of FQ.FastQ)(reads, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim readsPool As IEnumerable(Of FQ.FastQ) = pull.populates(Of FQ.FastQ)(env)

        If exact_match Then
            Return pipeline.CreateFromPopulator(readsPool.TrimPrimersAndAdapters(CLRVector.asCharacter(headers)))
        Else
            Return pipeline.CreateFromPopulator(readsPool.TrimPrimersSmithWaterman(CLRVector.asCharacter(headers), minScore:=0, cutoff:=cutoff))
        End If
    End Function

    <ExportAPI("nano_plot")>
    <RApiReturn(GetType(NanoPlotResult))>
    Public Function nano_plot(<RRawVectorArgument> reads As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of FQ.FastQ)(reads, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim readsPool As IEnumerable(Of FQ.FastQ) = pull.populates(Of FQ.FastQ)(env)
        Dim nanoplot As NanoPlotResult = readsPool.CalculateNanoPlotData
        Return nanoplot
    End Function

    ''' <summary>
    ''' generates the fastq reads QC summary text
    ''' </summary>
    ''' <param name="nanoplot"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("nano_report")>
    Public Function nano_report(nanoplot As NanoPlotResult, file As String) As Boolean
        Return nanoplot.ToString.SaveTo(file)
    End Function

End Module

