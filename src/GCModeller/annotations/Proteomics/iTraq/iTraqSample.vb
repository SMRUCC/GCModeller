#Region "Microsoft.VisualBasic::5defd66c470aa2a74546fdf0b04b5dc8, ..\GCModeller\annotations\Proteomics\iTraq\iTraqSample.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.PrintAsTable
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Module iTraqSample

    ''' <summary>
    ''' Split the iTraq sample into sevral data matrix based on the sample info and experiment analysis design.
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="sampleInfo"></param>
    ''' <param name="designer"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function MatrixSplit(matrix As DataSet(),
                                         sampleInfo As IEnumerable(Of SampleInfo),
                                         designer As IEnumerable(Of AnalysisDesigner),
                                         Optional allowedSwap As Boolean = False) As IEnumerable(Of NamedCollection(Of DataSet))

        Dim analysisDesign = designer.ToArray

        Call VBDebugger.WaitOutput()
        Call Console.WriteLine(analysisDesign.Print)

        With sampleInfo.DataAnalysisDesign(analysisDesign)

            For Each group As NamedCollection(Of AnalysisDesigner) In .ref.IterateNameCollections
                Dim groupName$ = group.Name
                Dim labels = group.Value
                Dim data = matrix _
                    .Select(Function(x) x.subsetValues(labels, allowedSwap)) _
                    .ToArray

                Yield New NamedCollection(Of DataSet) With {
                    .Name = groupName,
                    .Value = data
                }
            Next
        End With
    End Function

    <Extension> Private Function subsetValues(data As DataSet, labels As AnalysisDesigner(), allowedSwap As Boolean) As DataSet
        Dim values As New List(Of KeyValuePair(Of String, Double))

        For Each label As AnalysisDesigner In labels
            With label.ToString
                If data.HasProperty(.ref) Then
                    Call values.Add(.ref, data(.ref))
                Else
                    ' 可能是在进行质谱实验的时候将顺序颠倒了，在这里将标签颠倒一下试试
                    With label.Swap.ToString
                        If data.HasProperty(.ref) Then
                            ' 由于在取出值之后使用1除来进行翻转，所以在这里标签还是用原来的顺序，不需要进行颠倒了
                            If allowedSwap Then
                                values.Add(label.ToString, 1 / data(.ref))
                            Else
                                values.Add(label.ToString, data(.ref))
                            End If
                        End If
                    End With
                End If
            End With
        Next

        Return New DataSet With {
            .ID = data.ID,
            .Properties = values _
                .OrderBy(Function(d) d.Key) _
                .ToDictionary()
        }
    End Function
End Module

