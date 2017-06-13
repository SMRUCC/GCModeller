#Region "Microsoft.VisualBasic::199f789b9bebbd880717bf13001ccc88, ..\GCModeller\analysis\Microarray\DEGDesigner.vb"

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
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Mathematical.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Scripting
Imports csv = Microsoft.VisualBasic.Data.csv.IO.File
Imports gene = Microsoft.VisualBasic.Data.csv.IO.EntityObject

Public Module DEGDesigner

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
            For Each group In designers
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
    Public Function MergeMatrix(DIR$, name$, Optional DEG# = 0, Optional Pvalue# = Integer.MaxValue, Optional fieldFC$ = "logFC", Optional FCdown# = Integer.MinValue, Optional fieldPvalue$ = "PValue", Optional nonDEP_blank As Boolean = True) As gene()
        Dim samples As New Dictionary(Of String, gene())
        Dim test As Func(Of gene, Boolean)

        If FCdown <> Integer.MinValue Then
            test = Function(gene)
                       Dim FC# = gene(fieldFC).ParseNumeric
                       Return (FC >= DEG OrElse FC <= FCdown) AndAlso
                           gene(fieldPvalue).ParseNumeric <= Pvalue
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

        For Each group In groups
            Dim labels = group.Value.ToArray(Function(l) l.GetLabel(label.label, label.delimiter))
            Dim file As New StringBuilder
            Dim experiments = labels.ToArray(Function(l) l.exp)
            Dim controls = labels.ToArray(Function(l) l.control)
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
End Module

