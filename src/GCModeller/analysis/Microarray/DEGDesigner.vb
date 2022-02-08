#Region "Microsoft.VisualBasic::fe956a4d2860de7df73c46c19376caab, analysis\Microarray\DEGDesigner.vb"

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

    ' Module DEGDesigner
    ' 
    '     Function: DEGsStatMatrix, (+2 Overloads) log2, MergeDEPMatrix, MergeMatrix, ParseDEGTypes
    '               StudentTtest
    ' 
    '     Sub: EdgeR_rawDesigner
    '     Class Designer
    ' 
    '         Properties: Control, Experiment, GroupLabel
    ' 
    '         Function: GetLabel, Log2, ToString
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    '         Sub: GeneralDesigner, TtestDesigner, TtestDesignerIndependent
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports csv = Microsoft.VisualBasic.Data.csv.IO.File
Imports gene = Microsoft.VisualBasic.Data.csv.IO.EntityObject

Public Module DEGDesigner

    Public Function ParseDEGTypes(str$) As Types
        Select Case Strings.Trim(str).ToLower
            Case "up" : Return Types.Up
            Case "down" : Return Types.Down
            Case Else
                Return Types.None
        End Select
    End Function

    <Extension>
    Public Iterator Function log2(data As IEnumerable(Of EntityObject), designers As Designer(), Optional label$ = Nothing) As IEnumerable(Of EntityObject)
        For Each gene As EntityObject In data
            For Each designer In designers
                Dim tag$ = $"log2({designer.ToString})"
                gene.Properties(tag) = designer.Log2(gene, label)
            Next

            Yield gene
        Next
    End Function

    Public Function log2(path$, designers As Designer(), Optional label$ = Nothing) As csv
        Dim genes As EntityObject() = EntityObject.LoadDataSet(path).ToArray
        Dim groups As Dictionary(Of String, Designer()) = designers _
            .GroupBy(Function(x) x.GroupLabel) _
            .ToDictionary(Function(k) k.Key,
                          Function(repeats) repeats.ToArray)
        Dim out As EntityObject() = genes _
            .log2(designers, label) _
            .StudentTtest(groups, label) _
            .ToArray
        Dim csv As csv = out.ToCsvDoc
        Return csv
    End Function

    <Extension>
    Public Iterator Function StudentTtest(data As IEnumerable(Of EntityObject), designers As Dictionary(Of String, Designer()), Optional label$ = Nothing) As IEnumerable(Of EntityObject)
        For Each gene As EntityObject In data
            For Each group As KeyValuePair(Of String, Designer()) In designers
                Dim a#() = New Double(group.Value.Length - 1) {}
                Dim b#() = New Double(group.Value.Length - 1) {}

                For i As Integer = 0 To a.Length - 1
                    Dim l = group.Value(i)
                    Dim tag = l.GetLabel(label)

                    a(i) = Val(gene(tag.exp))
                    b(i) = Val(gene(tag.control))
                Next

                Dim result As TwoSampleResult = t.Test(a, b)

                gene($"{group.Key}.P-value") = result.Pvalue
            Next

            Yield gene
        Next
    End Function

    Public Function MergeDEPMatrix(DIR$, name$) As gene()
        Return MergeMatrix(DIR, name, DEG:=Math.Log(1.5, 2), Pvalue:=0.05)
    End Function

    ''' <summary>
    ''' 合并实验数据矩阵，可以使用这个函数用来生成诸如heatmap或者vennDiagram的绘图数据
    ''' </summary>
    ''' <param name="DIR$"></param>
    ''' <param name="name$">进行文件搜索的通配符</param>
    ''' <param name="DEG#">
    ''' + DEG = <see cref="Math.Log"/>(2, 2)
    ''' + DEP = <see cref="Math.Log"/>(1.5, 2)
    ''' 
    ''' 假若是使用默认值0的话，由于任何实数都大于0，所以就不会进行差异基因的筛选，即函数会返回所有的基因列表
    ''' </param>
    ''' <returns></returns>
    Public Function MergeMatrix(DIR$, name$,
                                Optional DEG# = 0,
                                Optional Pvalue# = Integer.MaxValue,
                                Optional fieldFC$ = "logFC",
                                Optional FCdown# = Integer.MinValue,
                                Optional fieldPvalue$ = "PValue",
                                Optional nonDEP_blank As Boolean = True,
                                Optional log2t As Boolean = False) As gene()

        Dim samples As New Dictionary(Of String, gene())
        Dim test As Func(Of gene, Boolean) ' 监测目标是否是符合要求的？

        If FCdown <> Integer.MinValue Then
            test = Function(gene)
                       If gene(fieldPvalue).ParseNumeric > Pvalue Then
                           Return False
                       End If

                       Dim FC = gene(fieldFC)
                       Dim value#

                       If log2t Then
                           If FC.TextEquals("Inf") Then

                           End If
                       Else
                           value = Val(FC)
                           Return (value >= DEG OrElse value <= FCdown)
                       End If
                   End Function
        Else
            test = Function(gene)
                       Return Math.Abs(gene(fieldFC).ParseNumeric) >= DEG AndAlso
                           gene(fieldPvalue).ParseNumeric <= Pvalue
                   End Function
        End If

        If nonDEP_blank Then
            For Each file As String In (ls - l - r - name <= DIR).Distinct
                Dim sample As String = file.ParentDirName & "-" & file.BaseName
                Dim DEGs As gene() = gene _
                    .LoadDataSet(file) _
                    .Where(test) _
                    .ToArray

                Call samples.Add(sample, DEGs)
            Next
        Else
            ' 先得到所有的DEP编号，再取出FC值就行了
            Dim datasets = (ls - l - r - name <= DIR) _
                .Distinct _
                .Select(Function(file$)
                            Return New NamedValue(Of gene()) With {
                                .Name = file.ParentDirName & "-" & file.BaseName,
                                .Value = EntityObject.LoadDataSet(file).ToArray
                            }
                        End Function).ToArray
            Dim allDEPs$() = datasets _
                .Values _
                .IteratesALL _
                .Where(test) _
                .Keys _
                .Distinct _
                .ToArray
            Dim index As New Index(Of String)(allDEPs)

            For Each file In datasets
                Call samples.Add(
                    file.Name,
                    file.Value.Where(Function(g) index.IndexOf(g.ID) > -1).ToArray)
            Next
        End If

        Dim genes = samples _
            .Select(Function(x)
                        Return x.Value.Select(
                            Function(g)
                                Return New NamedValue(Of gene) With {
                                    .Name = x.Key,
                                    .Value = g
                                }
                            End Function)
                    End Function) _
            .Unlist _
            .GroupBy(Function(gene) gene.Value.ID) _
            .ToDictionary(Function(id) id.Key,
                          Function(g) g.ToArray)
        Dim out As New List(Of gene)

        For Each gene As KeyValuePair(Of String, NamedValue(Of gene)()) In genes
            out += New gene With {
                .ID = gene.Key,
                .Properties = gene.Value _
                .ToDictionary(Function(sample) sample.Name,
                              Function(logFC) logFC.Value(fieldFC))}
        Next

        Return out _
            .OrderBy(Function(gene) gene.ID) _
            .ToArray
    End Function

    ''' <summary>
    ''' 返回DEGs数量的矩阵
    ''' </summary>
    ''' <param name="DIR$"></param>
    ''' <param name="name$"></param>
    ''' <returns></returns>
    Public Function DEGsStatMatrix(DIR$, name$, Optional DEP As Boolean = False) As gene()
        Dim samples As New List(Of gene)
        Dim diffUP = If(DEP, Math.Log(1.5, 2), 1)
        Dim diffDown = If(DEP, -Math.Log(1.5, 2), -1)

        For Each file As String In (ls - l - r - name <= DIR).Distinct
            Dim DEGs = gene.LoadDataSet(file) _
                .Select(Function(gene) (logFC:=gene("logFC").ParseNumeric, gene)) _
                .Where(Function(gene) Math.Abs(gene.logFC) >= diffUP) _
                .ToArray
            Dim sample As String = file.ParentDirName
            Dim up As Integer = DEGs.Where(Function(gene) gene.logFC >= diffUP).Count.ToString
            Dim down As Integer = DEGs.Where(Function(gene) gene.logFC <= diffDown).Count.ToString

            samples += New gene With {
                .ID = sample,
                .Properties = New Dictionary(Of String, String) From {
                    {"UP", up},
                    {"Down", down},
                    {"DEGs", up + down}
                }
            }
        Next

        Return samples
    End Function

    ''' <summary>
    ''' DEG/DEP计算的设计方法
    ''' </summary>
    Public Class Designer

        ''' <summary>
        ''' 分子
        ''' </summary>
        Public Property Experiment As String
        ''' <summary>
        ''' 分母
        ''' </summary>
        Public Property Control As String

        ''' <summary>
        ''' 具有相同的这个属性的标签值的都是生物学重复
        ''' </summary>
        Public Property GroupLabel As String

        Public Overrides Function ToString() As String
            Return $"{Experiment}/{Control}"
        End Function

        Public Function GetLabel(Optional label$ = Nothing, Optional delimiter$ = "-") As (exp$, control$)
            If label.StringEmpty Then
                Return (Experiment, Control)
            Else
                Return (label & delimiter & Experiment, label & delimiter & Control)
            End If
        End Function

        Public Function Log2(gene As EntityObject, Optional label$ = Nothing) As Double
            Dim tag As (exp$, Control As String) = GetLabel(label)
            Dim A# = gene(tag.exp).ParseNumeric
            Dim B# = gene(tag.Control).ParseNumeric
            Dim out As Double = Math.Log(A / B, 2)
            Return out
        End Function
    End Class

    ''' <summary>
    ''' 从矩阵之中导出edgeR分析所需要的文本数据
    ''' </summary>
    ''' <param name="path$">proteinGroups.xlsx所导出来的csv文件</param>
    ''' <param name="designers"></param>
    ''' <param name="label$"></param>
    ''' <param name="workDIR$"></param>
    Public Sub EdgeR_rawDesigner(path$, designers As Designer(), Optional label As (label$, delimiter$) = Nothing, Optional workDIR$ = "./")
        Dim genes As gene() = gene.LoadDataSet(path).ToArray
        Dim groups As Dictionary(Of String, Designer()) = designers _
            .GroupBy(Function(x) x.GroupLabel) _
            .ToDictionary(Function(k) k.Key,
                          Function(repeats) repeats.ToArray)
        Dim name$ = path.BaseName

        For Each group As KeyValuePair(Of String, Designer()) In groups
            Dim labels = group.Value.Select(Function(l) l.GetLabel(label.label, label.delimiter)).ToArray
            Dim file As New StringBuilder
            Dim experiments = labels.Select(Function(l) l.exp).ToArray
            Dim controls = labels.Select(Function(l) l.control).ToArray
            Dim line As New List(Of String)
            Dim appendLine = Sub()
                                 Call file.AppendLine(line.JoinBy(vbTab))
                                 Call line.Clear()
                             End Sub

            Call line.Add("ID")
            Call line.AddRange(controls)
            Call line.AddRange(experiments)
            Call appendLine()

            For Each gene As gene In genes
                Call line.Add(gene.ID)
                ' EdgeR的实验的计算顺序是这样子的
                Call line.AddRange(controls.Select(Function(t) gene(t)))
                Call line.AddRange(experiments.Select(Function(t) gene(t)))
                Call appendLine()
            Next

            path = workDIR & "/" & name & "-" & group.Key.NormalizePathString(False) & ".txt"
            file.SaveTo(path, Encoding.ASCII)
        Next
    End Sub

    Public Delegate Sub doSymbol(gene As gene, experiments$(), controls$(), fillRowData As Action(Of String()))
    Public Delegate Sub DataOutput(data$(), name$, group$)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="path$"></param>
    ''' <param name="designers"></param>
    ''' <param name="label"></param>
    ''' <param name="doSymbol"></param>
    ''' <param name="doHeaders">基因参数是Nothing空值</param>
    ''' <param name="output"></param>
    Public Sub GeneralDesigner(path$, designers As Designer(),
                               label As (label$, delimiter$),
                               doSymbol As doSymbol,
                               doHeaders As doSymbol,
                               output As DataOutput)

        Dim genes As gene() = gene.LoadDataSet(path).ToArray
        Dim groups As Dictionary(Of String, Designer()) = designers _
            .GroupBy(Function(x) x.GroupLabel) _
            .ToDictionary(Function(k) k.Key,
                          Function(repeats) repeats.ToArray)
        Dim name$ = path.BaseName

        For Each group As KeyValuePair(Of String, Designer()) In groups
            Dim labels = group _
                .Value _
                .Select(Function(l)
                            Return l.GetLabel(label.label, label.delimiter)
                        End Function) _
                .ToArray
            Dim file As New List(Of String)
            Dim experiments = labels.Select(Function(l) l.exp).ToArray
            Dim controls = labels.Select(Function(l) l.control).ToArray
            Dim line As New List(Of String)

            ' 生成表头
            Call line.Add("ID")
            Call doHeaders(
                Nothing,
                experiments, controls,
                Sub(values)
                    Call line.AddRange(values)
                End Sub)
            Call file.Add(line.JoinBy(vbTab))
            Call line.Clear()

            For Each gene As gene In genes
                Call doSymbol(
                    gene, experiments, controls,
                    Sub(values)
                        Call line.AddRange(values)
                    End Sub)
                Call file.Add(line.JoinBy(vbTab))
                Call line.Clear()
            Next

            Call output(file, name, group:=group.Key)
        Next
    End Sub

    ''' <summary>
    ''' 两个独立实验进行相互比较的t检验分析
    ''' </summary>
    ''' <param name="path$"></param>
    ''' <param name="designers"></param>
    ''' <param name="label"></param>
    ''' <param name="workDIR$"></param>
    Public Sub TtestDesignerIndependent(path$, designers As Designer(), Optional label As (label$, delimiter$) = Nothing, Optional workDIR$ = "./")
        Dim output As DataOutput =
            Sub(data, name, group)
                Dim outName$ = name & "-" & group.NormalizePathString(False)
                Dim out$ = workDIR & "/" & outName & ".txt"

                Call data.SaveTo(out, Encoding.ASCII)
            End Sub
        Dim doSymbol As doSymbol =
            Sub(gene, experiments, controls, fillRowData)
                ' 生成两个独立的实验向量
                ' aaaaabbbbb

                Dim experimentValues = experiments _
                    .Select(Function(t) Val(gene(t)).SafeToString) _
                    .AsList
                Dim controlValues$() = controls _
                    .Select(Function(t) Val(gene(t)).SafeToString) _
                    .ToArray

                Call fillRowData({gene.ID})
                Call fillRowData(experimentValues + controlValues)
            End Sub
        Dim doHeaders As doSymbol =
            Sub(gene, experiments, controls, fillRowData)
                Call fillRowData(experiments.AsList + controls)
            End Sub

        Call DEGDesigner.GeneralDesigner(
            path, designers, label,
            doSymbol,
            doHeaders,
            output)
    End Sub

    ''' <summary>
    ''' FC值向量和1向量进行t检验分析DEG
    ''' </summary>
    ''' <param name="path$"></param>
    ''' <param name="designers"></param>
    ''' <param name="label"></param>
    ''' <param name="workDIR$"></param>
    Public Sub TtestDesigner(path$, designers As Designer(), Optional label As (label$, delimiter$) = Nothing, Optional workDIR$ = "./")
        Dim output As DataOutput =
            Sub(data, name, group)
                Dim outName$ = name & "-" & group.NormalizePathString(False)
                Dim out$ = workDIR & "/" & outName & ".txt"

                Call data.SaveTo(out, Encoding.ASCII)
            End Sub
        Dim doSymbol As doSymbol =
            Sub(gene, experiments, controls, fillRowData)
                Dim experimentValues#() = experiments _
                    .Select(Function(t) Val(gene(t))) _
                    .ToArray
                Dim controlValues#() = controls _
                    .Select(Function(t) Val(gene(t))) _
                    .ToArray

                ' experiment/controls
                Dim combos = CreateCombos(experimentValues, controlValues) _
                    .ToArray
                Dim foldChanges = combos _
                    .Select(Function(c)
                                If c.Item2 = 0R Then
                                    Return "NA"
                                Else
                                    Return (c.Item1 / c.Item2).SafeToString("NA")
                                End If
                            End Function) _
                    .ToArray

                Call fillRowData({gene.ID})
                Call fillRowData(foldChanges)
            End Sub
        Dim doHeaders As doSymbol =
            Sub(gene, experiments, controls, fillRowData)
                Dim list$() = CreateCombos(experiments, controls) _
                    .Select(Function(c) $"{c.Item1}/{c.Item2}") _
                    .ToArray
                Call fillRowData(list)
            End Sub

        Call DEGDesigner.GeneralDesigner(
            path, designers, label,
            doSymbol,
            doHeaders,
            output)
    End Sub
End Module
