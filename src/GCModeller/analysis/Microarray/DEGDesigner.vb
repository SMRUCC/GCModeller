Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.DocumentStream
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Mathematical.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Scripting
Imports csv = Microsoft.VisualBasic.Data.csv.DocumentStream.File
Imports gene = Microsoft.VisualBasic.Data.csv.DocumentStream.EntityObject

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

    Public Structure Designer

        ''' <summary>
        ''' 分子
        ''' </summary>
        Dim Experiment$
        ''' <summary>
        ''' 分母
        ''' </summary>
        Dim Control$

        ''' <summary>
        ''' 具有相同的这个属性的标签值的都是生物学重复
        ''' </summary>
        Dim GroupLabel$

        Public Overrides Function ToString() As String
            Return $"{Experiment}/{Control}"
        End Function

        Public Function GetLabel(Optional label$ = Nothing) As (exp$, control$)
            If label Is Nothing Then
                Return (Experiment, Control)
            Else
                Return (label & "." & Experiment, label & "." & Control)
            End If
        End Function

        Public Function Log2(gene As EntityObject, Optional label$ = Nothing) As Double
            Dim tag As (exp$, Control As String) = GetLabel(label)
            Dim A# = gene(tag.exp).ParseNumeric
            Dim B# = gene(tag.Control).ParseNumeric
            Dim out As Double = Math.Log(A / B, 2)
            Return out
        End Function
    End Structure

    ''' <summary>
    ''' 从矩阵之中导出edgeR分析所需要的文本数据
    ''' </summary>
    ''' <param name="path$"></param>
    ''' <param name="designers"></param>
    ''' <param name="label$"></param>
    ''' <param name="workDIR$"></param>
    Public Sub EdgeR_rawDesigner(path$, designers As Designer(), Optional label$ = Nothing, Optional workDIR$ = "./")
        Dim genes As gene() = gene.LoadDataSet(path).ToArray
        Dim groups As Dictionary(Of String, Designer()) = designers _
            .GroupBy(Function(x) x.GroupLabel) _
            .ToDictionary(Function(k) k.Key,
                          Function(repeats) repeats.ToArray)
        ' Dim idmaps As New Dictionary(Of String, String)
        Dim name$ = path.BaseName

        For Each group In groups
            Dim labels = group.Value.ToArray(Function(l) l.GetLabel(label))
            Dim file As New StringBuilder
            Dim experiments = labels.ToArray(Function(l) l.exp)
            Dim controls = labels.ToArray(Function(l) l.control)
            Dim line As New List(Of String)
            Dim appendLine = Sub()
                                 Call file.AppendLine(line.JoinBy(vbTab))
                                 Call line.Clear()
                             End Sub

            Call line.Add("ID")
            Call line.AddRange(experiments)
            Call line.AddRange(controls)
            Call appendLine()

            For Each gene As gene In genes
                Call line.Add(gene.Identifier)
                Call line.AddRange(experiments.Select(Function(t) gene(t)))
                Call line.AddRange(controls.Select(Function(t) gene(t)))
                Call appendLine()
            Next

            path = workDIR & "/" & name & "-" & group.Key.NormalizePathString(False) & ".txt"
            file.SaveTo(path, Encoding.ASCII)
        Next
    End Sub
End Module
