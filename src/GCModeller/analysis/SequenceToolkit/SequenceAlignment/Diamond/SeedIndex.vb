#Region "Microsoft.VisualBasic::682b2797005852302f380e77d376ee2f, analysis\SequenceToolkit\SequenceAlignment\Diamond\SeedIndex.vb"

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

    '   Total Lines: 166
    '    Code Lines: 91 (54.82%)
    ' Comment Lines: 46 (27.71%)
    '    - Xml Docs: 71.74%
    ' 
    '   Blank Lines: 29 (17.47%)
    '     File Size: 6.45 KB


    '     Structure SubjectHit
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Structure SeedPair
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class ReferenceIndex
    ' 
    '         Properties: Count
    ' 
    '         Function: Lookup
    ' 
    '         Sub: Build
    ' 
    '     Class QueryIndex
    ' 
    '         Properties: Count
    ' 
    '         Function: HashJoin
    ' 
    '         Sub: Build
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' 双索引与哈希连接 (Dual Index & Hash Join)
'
' DIAMOND 与 BLASTP 结构性差异最大的一环:
'   - 对查询和参考库都建立"种子编码 -> 出现位置列表"的索引(使用相同的压缩编码)。
'   - 不再"读一个查询词、查一次库"(BLAST 的逐词随机访存),而是用哈希连接
'     同时遍历两张表:以参考索引为基准,将查询索引中相同编码的位置配对。
'   - 索引 on-the-fly 构建:一次只处理一个种子形状,用完即释放查询侧索引,
'     控制峰值内存(即便 ultra-sensitive 64 个形状也无需常驻)。
'
' 本实现针对"单查询 vs 单参考库"原型:
'   - 参考索引为永久索引(Dictionary(Of Long, List(Of SubjectHit)))。
'   - 查询索引为临时索引(单条查询序列, Dictionary(Of Long, List(Of Integer)))。
'   - 哈希连接:遍历查询索引的每个编码,在参考索引中查找配对并产出 SeedPair。

Imports System.Runtime.CompilerServices

Namespace DIAMOND

    ''' <summary>
    ''' 参考序列中一个种子命中的位置。
    ''' </summary>
    Public Structure SubjectHit
        ''' <summary>参考库内序列编号(对应 subjectDb 的索引)。</summary>
        Public ReadOnly SubjectId As Integer
        ''' <summary>在该参考序列内的起始位置。</summary>
        Public ReadOnly Position As Integer

        Sub New(subjectId As Integer, position As Integer)
            Me.SubjectId = subjectId
            Me.Position = position
        End Sub
    End Structure

    ''' <summary>
    ''' 查询与参考配对后的一个种子命中(用于后续过滤链)。
    ''' </summary>
    Public Structure SeedPair
        Public ReadOnly QueryPos As Integer
        Public ReadOnly SubjectId As Integer
        Public ReadOnly SubjectPos As Integer

        Sub New(queryPos As Integer, subjectId As Integer, subjectPos As Integer)
            Me.QueryPos = queryPos
            Me.SubjectId = subjectId
            Me.SubjectPos = subjectPos
        End Sub
    End Structure

    ''' <summary>
    ''' 参考库双索引:种子编码 -> 出现位置列表。
    ''' </summary>
    Public Class ReferenceIndex

        ''' <summary>种子编码 -> 该编码在参考库中出现的所有位置(跨序列聚合)。</summary>
        Private ReadOnly table As New Dictionary(Of Long, List(Of SubjectHit))

        ''' <summary>
        ''' 构建参考库索引。对每条参考序列按给定形状枚举种子并登记。
        ''' </summary>
        ''' <param name="subjects">参考序列集合(原始蛋白序列字符串)。</param>
        ''' <param name="seed">当前处理的间隔种子形状。</param>
        Public Sub Build(subjects As IList(Of String), seed As SpacedSeed)
            table.Clear()

            For sid As Integer = 0 To subjects.Count - 1
                Dim seq = subjects(sid)

                If String.IsNullOrEmpty(seq) OrElse seq.Length < seed.Length Then
                    Continue For
                End If

                For Each h In SeedEncoder.EnumerateSeeds(seq, seed)
                    Dim list As List(Of SubjectHit) = Nothing

                    If Not table.TryGetValue(h.Code, list) Then
                        list = New List(Of SubjectHit)
                        table(h.Code) = list
                    End If

                    list.Add(New SubjectHit(sid, h.Position))
                Next
            Next
        End Sub

        ''' <summary>
        ''' 查询一颗种子编码在参考库中命中的所有位置;未命中返回空列表。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Lookup(code As Long) As List(Of SubjectHit)
            Dim list As List(Of SubjectHit) = Nothing

            If table.TryGetValue(code, list) Then
                Return list
            End If

            Return New List(Of SubjectHit)
        End Function

        ''' <summary>参考索引中的总种子条目数(诊断用)。</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return table.Count
            End Get
        End Property
    End Class

    ''' <summary>
    ''' 查询索引(临时,单条查询序列)与参考索引的哈希连接。
    ''' </summary>
    Public Class QueryIndex

        ''' <summary>种子编码 -> 该编码在查询序列中出现的所有起始位置。</summary>
        Private ReadOnly table As New Dictionary(Of Long, List(Of Integer))

        ''' <summary>
        ''' 为单条查询序列按给定形状构建临时索引。
        ''' </summary>
        Public Sub Build(query As String, seed As SpacedSeed)
            table.Clear()

            If String.IsNullOrEmpty(query) OrElse query.Length < seed.Length Then
                Return
            End If

            For Each h In SeedEncoder.EnumerateSeeds(query, seed)
                Dim list As List(Of Integer) = Nothing

                If Not table.TryGetValue(h.Code, list) Then
                    list = New List(Of Integer)
                    table(h.Code) = list
                End If

                list.Add(h.Position)
            Next
        End Sub

        ''' <summary>
        ''' 与参考索引做哈希连接,返回所有种子配对。
        ''' 以查询表驱动、在参考表中做线性查找配对,避免 BLAST 式逐词随机访存。
        ''' </summary>
        Public Iterator Function HashJoin(ref As ReferenceIndex) As IEnumerable(Of SeedPair)
            For Each kvp In table
                Dim code = kvp.Key
                Dim qPositions = kvp.Value
                Dim sHits = ref.Lookup(code)

                If sHits.Count = 0 Then
                    Continue For
                End If

                For Each qp In qPositions
                    For Each sh In sHits
                        Yield New SeedPair(qp, sh.SubjectId, sh.Position)
                    Next
                Next
            Next
        End Function

        ''' <summary>查询索引中的总种子条目数(诊断用)。</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return table.Count
            End Get
        End Property
    End Class
End Namespace

