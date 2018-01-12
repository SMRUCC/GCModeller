#Region "Microsoft.VisualBasic::dd6ccb00a3bd8c77542d671301186fbd, ..\GCModeller\annotations\Proteomics\iTraq\iTraqSample.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2018 GPL3 Licensed
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
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.PrintAsTable
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Vectorization
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

    ''' <summary>
    ''' 搭桥计算过程：
    ''' 
    ''' ```
    ''' A/C = X
    ''' B/C = Y
    ''' 
    ''' A = CX, B = CY
    ''' 
    ''' A/B = X/Y
    ''' ```
    ''' </summary>
    ''' <param name="C$">公共的混合样，A和B需要通过这个公共的混合样来计算出FoldChangeB/A或者A/B</param>
    ''' <param name="A"></param>
    ''' <param name="B"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 在这个函数里面需要完成的工作是将所有含有公共样品标签<paramref name="C"/>的数据通过搭桥计算过程合并在一起
    ''' </remarks>
    <Extension>
    Public Function BridgeSymbolReplace(C$, A As iTraqReader(), B As iTraqReader()) As iTraqReader()
        Dim ALL = C.BridgeCombine(A:=A.replaceNew(C, tag:="A"), B:=B.replaceNew(C, tag:="B"))


    End Function

    <Extension>
    Private Function BridgeCombine(C$, A As iTraqReader(), B As iTraqReader()) As iTraqReader()
        Dim ALLKeys As StringVector = A.First.Properties.Keys.AsList + B.First.Properties.Keys
        Dim patternC = (C & "/", "/" & C)
        Dim CKeys = ALLKeys(Function(key) InStr(key, patternC.Item1) > 0 OrElse InStr(key, patternC.Item2) > 0)
    End Function

    <Extension>
    Private Function replaceNew(sample As iTraqReader(), C$, tag$) As iTraqReader()
        Dim keys = sample.First.Properties.Keys.ToArray
        Dim keyReplace As New Dictionary(Of String, String)
        Dim r As New Regex("\d+[/]\d+")

        For Each key As String In keys
            Dim keyNew$ = key
            Dim FC = r.Match(keyNew).Value
            Dim t = FC.Split("/"c)

            If Not t(0) = C Then
                t(0) = tag & ":" & t(0)
            End If
            If Not t(1) = C Then
                t(1) = tag & ":" & t(1)
            End If

            Dim FC2 = t(0) & "/" & t(1)

            keyReplace(key) = keyNew.Replace(FC, FC2)
        Next

        Dim newSample As iTraqReader() = sample _
            .Select(Function(protein)
                        Dim tagReplaced As New Dictionary(Of String, Double)
                        Dim FC = protein.Properties

                        For Each dataValue In FC
                            tagReplaced(keyReplace(dataValue.Key)) = dataValue.Value
                        Next

                        protein.Properties = tagReplaced

                        Return protein
                    End Function) _
            .ToArray

        Return newSample
    End Function
End Module
