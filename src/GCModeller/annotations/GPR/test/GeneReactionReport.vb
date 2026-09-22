Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports SMRUCC.genomics.GCModeller.CompilerServices.GPRLink
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 结果表中的一行：一条"基因 - 反应"关联及其打分
''' </summary>
Public Class ReportRow

    Public Property GeneId As String
    Public Property ReactionId As String
    Public Property Score As Double
    Public Property Confidence As String
    Public Property Pathway As String
    Public Property ECNumbers As String
    Public Property Evidence As String

    Public Overrides Function ToString() As String
        Return $"{GeneId} -> {ReactionId}: {Score:F4}"
    End Function

End Class

''' <summary>
''' 基因-代谢反应关联结果的渲染与导出。
''' 
''' 同时提供两种视图：
''' 
''' + **明细表**：每一行是"基因 + 反应 + 打分 + 评分依据"，用于直观检查算法产出；
''' + **基因汇总表**：每个基因的关联数目、平均分/中位分与高置信关联，用于快速评估质量。
''' </summary>
Public Class GeneReactionReport

    Private ReadOnly rows As New List(Of ReportRow)

    Public Property Title As String
    Public Property Source As String

    Public ReadOnly Property DetailRows As List(Of ReportRow)
        Get
            Return rows
        End Get
    End Property

    Public ReadOnly Property AssociationCount As Integer
        Get
            Return rows.Count
        End Get
    End Property

    Public ReadOnly Property GeneCount As Integer
        Get
            Return rows.Select(Function(r) r.GeneId).Distinct(StringComparer.OrdinalIgnoreCase).Count()
        End Get
    End Property

    ''' <summary>
    ''' 从算法输出装载结果表
    ''' </summary>
    Public Sub Load(associator As MetabolicAssociator, title As String, source As String)
        Me.Title = title
        Me.Source = source
        Me.rows.Clear()

        If associator Is Nothing Then Return

        For Each association As GeneAssociation In associator.GenomeModel.MetabolicNetwork.Values
            For Each reaction As ScoredReaction In association.Reactions.Values
                If reaction.Unmapped Then Continue For

                rows.Add(New ReportRow With {
                    .GeneId = association.GeneId,
                    .ReactionId = reaction.Id,
                    .Score = reaction.Score,
                    .Confidence = reaction.ConfidenceLevel,
                    .Pathway = PathwayOf(associator, reaction.Id),
                    .ECNumbers = ECNumbersOf(associator, reaction.Id),
                    .Evidence = reaction.EvidenceSummary
                })
            Next
        Next

        ' 便于阅读：按基因名称、再按分数降序排列
        Dim ordered As ReportRow() = rows _
            .OrderBy(Function(r) r.GeneId, StringComparer.OrdinalIgnoreCase) _
            .ThenByDescending(Function(r) r.Score) _
            .ToArray

        rows.Clear()
        rows.AddRange(ordered)
    End Sub

    Private Shared Function PathwayOf(associator As MetabolicAssociator, reactionId As String) As String
        Dim context As ContextIndices = associator.ContextIndex
        If context Is Nothing Then Return ""

        Dim pathways As Pathway() = context.GetPathwaysByReaction(reactionId).ToArray()
        If pathways.Length = 0 Then Return ""

        Return String.Join(",", pathways.Select(Function(p) If(String.IsNullOrEmpty(p.ID), p.name, p.ID)))
    End Function

    Private Shared Function ECNumbersOf(associator As MetabolicAssociator, reactionId As String) As String
        Dim index As Dictionary(Of String, MetabolicReaction) = associator.ContextIndex.ReactionIndex

        Dim reaction As MetabolicReaction = Nothing
        If index Is Nothing OrElse Not index.TryGetValue(reactionId, reaction) Then Return ""
        If reaction.ECNumbers Is Nothing Then Return ""

        Return String.Join(",", reaction.ECNumbers)
    End Function

    ''' <summary>
    ''' 在控制台打印明细表
    ''' </summary>
    Public Sub PrintDetail(Optional maxRows As Integer = Integer.MaxValue)
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine()
        Console.WriteLine($"基因 - 代谢反应关联表  ({GeneCount} 个基因 / {AssociationCount} 条关联)")
        Console.ResetColor()

        If rows.Count = 0 Then
            Console.WriteLine("  (没有产生任何关联)")
            Return
        End If

        Dim table As New List(Of String()) From {
            New String() {"gene_id", "reaction_id", "score", "confidence", "pathway", "ec_number", "evidence"}
        }

        For Each row As ReportRow In rows.Take(maxRows)
            table.Add({
                row.GeneId,
                row.ReactionId,
                row.Score.ToString("F4"),
                row.Confidence,
                row.Pathway,
                row.ECNumbers,
                row.Evidence
            })
        Next

        Call table.PrintTable(Console.Out, sep:=" "c)

        If rows.Count > maxRows Then
            Console.WriteLine($"  ... 另有 {rows.Count - maxRows} 条关联未在控制台显示，完整结果请查看 CSV 文件。")
        End If
    End Sub

    ''' <summary>
    ''' 在控制台打印基因级别的汇总统计
    ''' </summary>
    Public Sub PrintGeneSummary(associator As MetabolicAssociator)
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine()
        Console.WriteLine("基因关联汇总统计")
        Console.ResetColor()

        If associator Is Nothing Then Return

        Dim table As New List(Of String()) From {
            New String() {"gene_id", "links", "top1", "mean", "median", "max", "unmapped_ec"}
        }

        For Each association As GeneAssociation In associator.GenomeModel.MetabolicNetwork.Values
            Dim top1 As String = association.Reactions.Values _
                .OrderByDescending(Function(r) r.Score) _
                .Select(Function(r) r.Id) _
                .FirstOrDefault()

            table.Add({
                association.GeneId,
                association.GPRLinks.ToString(),
                If(top1, "-"),
                association.MeanScore.ToString("F4"),
                association.MedianScore.ToString("F4"),
                association.MaxScore.ToString("F4"),
                If(association.UnmappedECNumbers.Length = 0, "-", String.Join(",", association.UnmappedECNumbers))
            })
        Next

        Call table.PrintTable(Console.Out, sep:=" "c)
    End Sub

    ''' <summary>
    ''' 打印全局打分分布
    ''' </summary>
    Public Sub PrintScoreDistribution()
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine()
        Console.WriteLine("全部分数的分布情况")
        Console.ResetColor()

        If rows.Count = 0 Then
            Console.WriteLine("  (没有产生任何关联)")
            Return
        End If

        Dim buckets As (Name As String, Low As Double, High As Double)() = {
            ("high   [0.80, 1.00]", 0.8, 1.000001),
            ("medium [0.50, 0.80)", 0.5, 0.8),
            ("low    [0.30, 0.50)", 0.3, 0.5),
            ("below  [0.00, 0.30)", 0.0, 0.3)
        }

        Dim table As New List(Of String()) From {
            New String() {"score_range", "count", "percent"}
        }

        For Each bucket In buckets
            Dim count As Integer = rows _
                .Where(Function(r) r.Score >= bucket.Low AndAlso r.Score < bucket.High) _
                .Count()

            Dim percent As Double = If(rows.Count = 0, 0, 100.0 * count / rows.Count)

            table.Add({bucket.Name, count.ToString(), $"{percent:F1}%"})
        Next

        Call table.PrintTable(Console.Out, sep:=" "c)
    End Sub

    ''' <summary>
    ''' 把完整明细表导出为 CSV（UTF-8 with BOM，便于 Excel 直接打开）
    ''' </summary>
    Public Function ExportCsv(csvPath As String) As String
        Dim folder As String = System.IO.Path.GetDirectoryName(csvPath)
        If Not String.IsNullOrEmpty(folder) AndAlso Not System.IO.Directory.Exists(folder) Then
            Call System.IO.Directory.CreateDirectory(folder)
        End If

        Dim out As New StringBuilder
        Call out.AppendLine("gene_id,reaction_id,score,confidence,pathway,ec_number,evidence")

        For Each row As ReportRow In rows
            Call out.AppendLine(String.Join(",",
                Csv(row.GeneId),
                Csv(row.ReactionId),
                row.Score.ToString("F6"),
                Csv(row.Confidence),
                Csv(row.Pathway),
                Csv(row.ECNumbers),
                Csv(row.Evidence)))
        Next

        File.WriteAllText(csvPath, out.ToString(), New UTF8Encoding(encoderShouldEmitUTF8Identifier:=True))

        Return csvPath
    End Function

    Private Shared Function Csv(value As String) As String
        If value Is Nothing Then Return ""

        If value.Contains(","c) OrElse value.Contains(""""c) OrElse value.Contains(vbLf) Then
            Return """" & value.Replace("""", """""") & """"
        End If

        Return value
    End Function

End Class
