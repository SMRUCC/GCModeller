' ============================================================================
' GeneModel.vb — 基因/相邻对模型
' ----------------------------------------------------------------------------
' [operon.md §1.3 步骤1] 按基因组顺序枚举所有相邻基因对 (i, i+1)：
'   同链（M）/ 反链（O = 趋同 + 发散）。链向关系：
'     left +, right + → Same（同向转录）
'     left −, right − → Same
'     left +, right − → Convergent（→ ←，趋同：区间含两个终止子）
'     left −, right + → Divergent（← →，发散：区间含两个启动子）
' IGD：IGD = B.start − A.end − 1（两基因间隔碱基数，≥0；重叠 → 0）。
' ============================================================================

Imports System
Imports System.Collections.Generic

Namespace OperonPredictor.Core

    Public Enum StrandRelation
        Same = 0
        Convergent = 1      ' → ←
        Divergent = 2       ' ← →
    End Enum

    Public Class Gene

        Public Id As String
        Public Contig As String
        Public StartMin As Int32          ' 1-based，较小坐标
        Public EndMax As Int32
        Public Strand As Char             ' '+' / '-'
        Public Name As String = ""

    End Class

    ''' <summary>相邻基因对（基因组顺序上相邻）</summary>
    Public Class AdjacentPair

        Public A As Gene
        Public B As Gene
        Public Relation As StrandRelation
        Public Igd As Int32               ' 基因间距离（间隔碱基数）
        Public Index As Int32             ' 全局序号

        Public ReadOnly Property IsSameStrand As Boolean
            Get
                Return Relation = StrandRelation.Same
            End Get
        End Property

    End Class

    Public Module GeneModel

        ''' <summary>按 contig 排序并枚举相邻对</summary>
        Public Function EnumeratePairs(genes As List(Of Gene)) As List(Of AdjacentPair)
            Dim byContig As New Dictionary(Of String, List(Of Gene))()
            For Each g In genes
                If Not byContig.ContainsKey(g.Contig) Then byContig(g.Contig) = New List(Of Gene)()
                byContig(g.Contig).Add(g)
            Next
            Dim pairs As New List(Of AdjacentPair)()
            Dim idx As Int32 = 0
            For Each kv In byContig
                Dim gl = kv.Value
                gl.Sort(Function(a, b) a.StartMin.CompareTo(b.StartMin))
                For i = 0 To gl.Count - 2
                    Dim a = gl(i)
                    Dim b = gl(i + 1)
                    Dim rel As StrandRelation
                    If a.Strand = b.Strand Then
                        rel = StrandRelation.Same
                    ElseIf a.Strand = "+"c AndAlso b.Strand = "-"c Then
                        rel = StrandRelation.Convergent
                    Else
                        rel = StrandRelation.Divergent
                    End If
                    Dim igd = b.StartMin - a.EndMax - 1
                    If igd < 0 Then igd = 0
                    pairs.Add(New AdjacentPair With {.A = a, .B = b, .Relation = rel,
                                                     .Igd = igd, .Index = idx})
                    idx += 1
                Next
            Next
            pairs.Sort(Function(a, b) a.Index.CompareTo(b.Index))
            Return pairs
        End Function

    End Module

End Namespace
