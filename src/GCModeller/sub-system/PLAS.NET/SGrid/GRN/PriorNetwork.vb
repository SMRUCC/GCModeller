Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq

Namespace GRN

    ''' <summary>
    ''' The regulation type of a prior knowledge edge.
    ''' (先验知识之中所记录的调控方向)
    ''' </summary>
    Public Enum PriorRegulationTypes
        ''' <summary>
        ''' 未知的调控方向
        ''' </summary>
        Unknown = 0
        ''' <summary>
        ''' 激活：源基因上调靶基因的表达
        ''' </summary>
        Activator = 1
        ''' <summary>
        ''' 抑制：源基因下调靶基因的表达
        ''' </summary>
        Inhibitor = -1
    End Enum

    ''' <summary>
    ''' The curation confidence level of a prior knowledge edge.
    ''' (先验知识边的置信度等级)
    ''' </summary>
    Public Enum PriorConfidence
        ''' <summary>
        ''' 未标注置信度
        ''' </summary>
        Unknown = 0
        ''' <summary>
        ''' 低置信度
        ''' </summary>
        Low = 1
        ''' <summary>
        ''' 中等置信度
        ''' </summary>
        Medium = 2
        ''' <summary>
        ''' 高置信度
        ''' </summary>
        High = 3
    End Enum

    ''' <summary>
    ''' One curated TF-to-target regulation record.
    ''' (一条已经整理好的转录因子到靶基因的调控记录)
    ''' </summary>
    Public Class PriorEdge

        ''' <summary>
        ''' 调控因子（转录因子）的基因符号
        ''' </summary>
        ''' <returns></returns>
        Public Property TF As String
        ''' <summary>
        ''' 靶基因的基因符号
        ''' </summary>
        ''' <returns></returns>
        Public Property TargetGene As String
        ''' <summary>
        ''' 调控的方向
        ''' </summary>
        ''' <returns></returns>
        Public Property RegulationType As PriorRegulationTypes = PriorRegulationTypes.Unknown
        ''' <summary>
        ''' 该记录的置信度等级
        ''' </summary>
        ''' <returns></returns>
        Public Property Confidence As PriorConfidence = PriorConfidence.Unknown
        ''' <summary>
        ''' 支持该记录的文献证据文本
        ''' </summary>
        ''' <returns></returns>
        Public Property Evidence As String

        ''' <summary>
        ''' 由置信度等级推导出的权重（高=1.0，中=0.6，低=0.3）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Weight As Double
            Get
                Select Case Confidence
                    Case PriorConfidence.High
                        Return 1.0R
                    Case PriorConfidence.Medium
                        Return 0.6R
                    Case PriorConfidence.Low
                        Return 0.3R
                    Case Else
                        Return 0.5R
                End Select
            End Get
        End Property

        ''' <summary>
        ''' 调控方向的符号：激活为 +1，抑制为 -1，未知为 0
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Sign As Integer
            Get
                Select Case RegulationType
                    Case PriorRegulationTypes.Activator
                        Return 1
                    Case PriorRegulationTypes.Inhibitor
                        Return -1
                    Case Else
                        Return 0
                End Select
            End Get
        End Property

        ''' <summary>
        ''' 在本条记录之上应用一个基因符号的映射，映射不到的时候返回 Nothing
        ''' </summary>
        ''' <param name="nameMap"></param>
        ''' <returns></returns>
        Friend Function Remap(nameMap As System.Collections.Generic.IDictionary(Of String, String)) As PriorEdge
            Dim source As String = Nothing
            Dim target As String = Nothing

            If Not nameMap.TryGetValue(TF, source) Then
                Return Nothing
            End If
            If Not nameMap.TryGetValue(TargetGene, target) Then
                Return Nothing
            End If

            Return New PriorEdge With {
                .TF = source,
                .TargetGene = target,
                .RegulationType = RegulationType,
                .Confidence = Confidence,
                .Evidence = Evidence
            }
        End Function

        ''' <summary>
        ''' 复制当前记录
        ''' </summary>
        ''' <returns></returns>
        Public Function Clone() As PriorEdge
            Return New PriorEdge With {
                .TF = TF,
                .TargetGene = TargetGene,
                .RegulationType = RegulationType,
                .Confidence = Confidence,
                .Evidence = Evidence
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"{TF} -> {TargetGene} [{RegulationType.ToString()}, {Confidence.ToString()}]"
        End Function
    End Class

    ''' <summary>
    ''' The prior knowledge network which is used for constraining the regulatory
    ''' network structure inference.
    ''' (用于约束调控网络结构拟合的先验知识网络)
    ''' </summary>
    ''' <remarks>
    ''' 先验知识在本算法之中是以「软约束」的方式使用的：
    ''' 1. 拟合阶段对先验不支持的交互施加收缩惩罚（<see cref="FittingOptions.PriorPenalty"/>）
    ''' 2. 对与先验方向冲突的交互施加符号惩罚（<see cref="FittingOptions.PriorSignPenalty"/>）
    ''' 3. 稀疏化阶段对先验支持的边给予排序加权（<see cref="FittingOptions.PriorRankBoost"/>）
    ''' 
    ''' 注意：先验知识之中的基因符号与拟合所使用的规整化编号可能并不一致，
    ''' 在进入拟合过程之前需要先调用 <see cref="Remap"/> 做符号映射。
    ''' </remarks>
    Public Class PriorNetwork

        ''' <summary>
        ''' 全部的先验调控边
        ''' </summary>
        ''' <returns></returns>
        Public Property Edges As PriorEdge()

        ''' <summary>
        ''' 靶基因（大写符号）到其调控因子记录的索引
        ''' </summary>
        Private ReadOnly byTarget As New System.Collections.Generic.Dictionary(Of String, List(Of PriorEdge))(StringComparer.OrdinalIgnoreCase)
        ''' <summary>
        ''' 源到靶的边索引，键为 <c>TF &amp; vbTab &amp; target</c>
        ''' </summary>
        Private ReadOnly byPair As New System.Collections.Generic.Dictionary(Of String, PriorEdge)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>
        ''' 先验边的数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Integer
            Get
                If Edges Is Nothing Then Return 0 Else Return Edges.Length
            End Get
        End Property

        ''' <summary>
        ''' 先验网络之中所出现的全部基因符号（源与靶的并集）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property GeneSymbols As String()
            Get
                Dim genes As New System.Collections.Generic.HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

                For Each edge As PriorEdge In Edges.SafeQuery
                    If Not String.IsNullOrEmpty(edge.TF) Then
                        Call genes.Add(edge.TF)
                    End If
                    If Not String.IsNullOrEmpty(edge.TargetGene) Then
                        Call genes.Add(edge.TargetGene)
                    End If
                Next

                Return genes.ToArray
            End Get
        End Property

        ''' <summary>
        ''' 创建一个空的先验网络
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Empty() As PriorNetwork
            Return New PriorNetwork With {.Edges = New PriorEdge() {}}
        End Function

        ''' <summary>
        ''' 从给定的边集合创建先验网络对象
        ''' </summary>
        ''' <param name="records"></param>
        ''' <returns></returns>
        Public Shared Function FromEdges(records As IEnumerable(Of PriorEdge)) As PriorNetwork
            Dim network As New PriorNetwork With {.Edges = records.SafeQuery().ToArray}

            Call network.BuildIndex()

            Return network
        End Function

        ''' <summary>
        ''' 加载先验调控网络表
        ''' </summary>
        ''' <param name="filePath">csv 文件，需要的列为 TF / TargetGene / RegulationType / Confidence / Evidence</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 该表尾部的证据字段之中含有逗号并且使用双引号包裹，
        ''' 故这里使用自带的引号安全解析器来解析，而不使用反射式的 csv 加载器。
        ''' </remarks>
        Public Shared Function LoadCsv(filePath As String) As PriorNetwork
            If String.IsNullOrEmpty(filePath) Then
                Throw New ArgumentNullException(NameOf(filePath))
            End If
            If Not File.Exists(filePath) Then
                Throw New FileNotFoundException($"The prior knowledge network file '{filePath}' does not exist!")
            End If

            Return LoadCsv(File.OpenRead(filePath))
        End Function

        ''' <summary>
        ''' 从文本流之中加载先验调控网络
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Public Shared Function LoadCsv(s As Stream) As PriorNetwork
            Using reader As New StreamReader(s, Encoding.UTF8, detectEncodingFromByteOrderMarks:=True)
                Return LoadCsv(reader)
            End Using
        End Function

        ''' <summary>
        ''' 从文本读取器之中加载先验调控网络
        ''' </summary>
        ''' <param name="reader"></param>
        ''' <returns></returns>
        Public Shared Function LoadCsv(reader As TextReader) As PriorNetwork
            Dim header As String() = Nothing
            Dim iTF As Integer = 0
            Dim iTarget As Integer = 1
            Dim iType As Integer = 2
            Dim iConfidence As Integer = 3
            Dim iEvidence As Integer = 4
            Dim records As New System.Collections.Generic.List(Of PriorEdge)
            Dim line As String

            Do
                line = reader.ReadLine()

                If line Is Nothing Then
                    Exit Do
                End If
                If String.IsNullOrWhiteSpace(line) Then
                    Continue Do
                End If

                header = SplitLine(line)
                Exit Do
            Loop

            If header Is Nothing Then
                Return Empty()
            End If

            ' 依据表头定位各个字段的下标，定位不到的时候退化为固定的列顺序
            iTF = IndexOfColumn(header, iTF, "TF", "TranscriptionFactor", "Source", "Regulator")
            iTarget = IndexOfColumn(header, iTarget, "TargetGene", "Target", "Gene", "TargetId")
            iType = IndexOfColumn(header, iType, "RegulationType", "Type", "Interaction", "Effect")
            iConfidence = IndexOfColumn(header, iConfidence, "Confidence", "ConfidenceLevel")
            iEvidence = IndexOfColumn(header, iEvidence, "Evidence", "PMIDs", "Reference")

            Do
                line = reader.ReadLine()

                If line Is Nothing Then
                    Exit Do
                End If
                If String.IsNullOrWhiteSpace(line) Then
                    Continue Do
                End If

                Dim fields As String() = SplitLine(line)
                Dim tf As String = field(fields, iTF)
                Dim target As String = field(fields, iTarget)

                If String.IsNullOrEmpty(tf) OrElse String.IsNullOrEmpty(target) Then
                    Continue Do
                End If

                Call records.Add(New PriorEdge With {
                    .TF = tf.Trim,
                    .TargetGene = target.Trim,
                    .RegulationType = parseType(field(fields, iType)),
                    .Confidence = parseConfidence(field(fields, iConfidence)),
                    .Evidence = field(fields, iEvidence)
                })
            Loop

            Dim network As New PriorNetwork With {.Edges = records.ToArray}

            Call network.BuildIndex()

            Return network
        End Function

        ''' <summary>
        ''' 重建内部的查找索引
        ''' </summary>
        Private Sub BuildIndex()
            Call byTarget.Clear()
            Call byPair.Clear()

            Dim merged As New System.Collections.Generic.Dictionary(Of String, PriorEdge)(StringComparer.OrdinalIgnoreCase)

            For Each edge As PriorEdge In Edges.SafeQuery
                Dim key As String = pairKey(edge.TF, edge.TargetGene)
                Dim exists As PriorEdge = Nothing

                If merged.TryGetValue(key, exists) Then
                    ' 重复的记录保留置信度更高的一条（置信度相同时保留先出现的记录）
                    If edge.Confidence > exists.Confidence Then
                        merged(key) = edge
                    End If
                Else
                    merged.Add(key, edge)
                End If
            Next

            Edges = merged.Values.ToArray

            For Each edge As PriorEdge In Edges
                Dim key As String = pairKey(edge.TF, edge.TargetGene)
                Dim list As List(Of PriorEdge) = Nothing

                Call byPair.Add(key, edge)

                If Not byTarget.TryGetValue(edge.TargetGene, list) Then
                    list = New List(Of PriorEdge)
                    byTarget.Add(edge.TargetGene, list)
                End If

                Call list.Add(edge)
            Next
        End Sub

        ''' <summary>
        ''' 查询目标基因的全部已知调控因子
        ''' </summary>
        ''' <param name="target">靶基因的符号</param>
        ''' <returns></returns>
        Public Function RegulatorsOf(target As String) As PriorEdge()
            If String.IsNullOrEmpty(target) Then
                Return New PriorEdge() {}
            End If

            Dim list As List(Of PriorEdge) = Nothing

            If byTarget.TryGetValue(target, list) Then
                Return list.ToArray
            Else
                Return New PriorEdge() {}
            End If
        End Function

        ''' <summary>
        ''' 查询指定的调控关系；不存在的时候返回空值
        ''' </summary>
        ''' <param name="tf"></param>
        ''' <param name="target"></param>
        ''' <returns></returns>
        Public Function Lookup(tf As String, target As String) As PriorEdge
            If String.IsNullOrEmpty(tf) OrElse String.IsNullOrEmpty(target) Then
                Return Nothing
            End If

            Dim edge As PriorEdge = Nothing

            If byPair.TryGetValue(pairKey(tf, target), edge) Then
                Return edge
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 判断指定的调控关系是否有先验知识的支持
        ''' </summary>
        ''' <param name="tf"></param>
        ''' <param name="target"></param>
        ''' <returns></returns>
        Public Function Supports(tf As String, target As String) As Boolean
            Return Not Lookup(tf, target) Is Nothing
        End Function

        ''' <summary>
        ''' 查询指定的调控关系在先验知识之中的方向符号（+1/-1/0）
        ''' </summary>
        ''' <param name="tf"></param>
        ''' <param name="target"></param>
        ''' <returns></returns>
        Public Function SignOf(tf As String, target As String) As Integer
            Dim edge As PriorEdge = Lookup(tf, target)

            If edge Is Nothing Then
                Return 0
            Else
                Return edge.Sign
            End If
        End Function

        ''' <summary>
        ''' 只保留两端都落在给定基因集合之中的先验边
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <returns></returns>
        Public Function Subset(genes As IEnumerable(Of String)) As PriorNetwork
            Dim index As New System.Collections.Generic.HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each gene As String In genes.SafeQuery
                If Not String.IsNullOrEmpty(gene) Then
                    Call index.Add(gene)
                End If
            Next

            Return FromEdges(Edges _
                .SafeQuery() _
                .Where(Function(e) index.Contains(e.TF) AndAlso index.Contains(e.TargetGene)))
        End Function

        ''' <summary>
        ''' 对先验网络之中的基因符号做映射（例如映射到拟合所使用的规整化编号）
        ''' </summary>
        ''' <param name="nameMap">原始基因符号到目标基因符号的映射表（大小写不敏感）</param>
        ''' <returns>映射之后的新对象；映射不到的先验边会被丢弃</returns>
        Public Function Remap(nameMap As System.Collections.Generic.IDictionary(Of String, String)) As PriorNetwork
            If nameMap Is Nothing OrElse nameMap.Count = 0 Then
                Return FromEdges(Edges.SafeQuery())
            End If

            Dim records As New System.Collections.Generic.List(Of PriorEdge)
            Dim source As String
            Dim target As String

            For Each edge As PriorEdge In Edges.SafeQuery
                If Not nameMap.TryGetValue(edge.TF, source) Then
                    Continue For
                End If
                If Not nameMap.TryGetValue(edge.TargetGene, target) Then
                    Continue For
                End If

                Call records.Add(New PriorEdge With {
                    .TF = source,
                    .TargetGene = target,
                    .RegulationType = edge.RegulationType,
                    .Confidence = edge.Confidence,
                    .Evidence = edge.Evidence
                })
            Next

            Return FromEdges(records)
        End Function

        ''' <summary>
        ''' 深拷贝
        ''' </summary>
        ''' <returns></returns>
        Public Function Clone() As PriorNetwork
            Return FromEdges(Edges.SafeQuery().Select(Function(e) e.Clone()))
        End Function

        ''' <summary>
        ''' 按调控方向统计边的数量
        ''' </summary>
        ''' <returns></returns>
        Public Function TypeStatistic() As String
            Dim act As Integer = Edges.SafeQuery().Where(Function(e) e.RegulationType = PriorRegulationTypes.Activator).Count()
            Dim inh As Integer = Edges.SafeQuery().Where(Function(e) e.RegulationType = PriorRegulationTypes.Inhibitor).Count()
            Dim unk As Integer = Size - act - inh

            Return $"activator={act}, inhibitor={inh}, unknown={unk}"
        End Function

        Public Overrides Function ToString() As String
            Return $"prior network: {Size} edge(s), {GeneSymbols.Length} gene symbol(s)"
        End Function

        Private Shared Function pairKey(tf As String, target As String) As String
            Return tf & vbTab & target
        End Function

        Private Shared Function field(fields As String(), index As Integer) As String
            If fields Is Nothing OrElse index < 0 OrElse index >= fields.Length Then
                Return Nothing
            Else
                Return fields(index)
            End If
        End Function

        ''' <summary>
        ''' 在表头之中定位列下标；定位失败的时候返回缺省的下标
        ''' </summary>
        Private Shared Function IndexOfColumn(header As String(), fallback As Integer, ParamArray names As String()) As Integer
            For i As Integer = 0 To header.Length - 1
                Dim name As String = header(i).Trim()

                ' 去掉 BOM 与可能的引号
                name = name.Trim(""""c).Trim()

                For Each candidate As String In names
                    If String.Equals(name, candidate, StringComparison.OrdinalIgnoreCase) Then
                        Return i
                    End If
                Next
            Next

            Return fallback
        End Function

        Private Shared Function parseType(text As String) As PriorRegulationTypes
            If String.IsNullOrEmpty(text) Then
                Return PriorRegulationTypes.Unknown
            End If

            Dim s As String = text.Trim()

            If s.StartsWith("act", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(s, "+", StringComparison.Ordinal) Then
                Return PriorRegulationTypes.Activator
            ElseIf s.StartsWith("inh", StringComparison.OrdinalIgnoreCase) OrElse
                s.StartsWith("rep", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(s, "-", StringComparison.Ordinal) Then
                Return PriorRegulationTypes.Inhibitor
            Else
                Return PriorRegulationTypes.Unknown
            End If
        End Function

        Private Shared Function parseConfidence(text As String) As PriorConfidence
            If String.IsNullOrEmpty(text) Then
                Return PriorConfidence.Unknown
            End If

            Dim s As String = text.Trim()

            If s.StartsWith("high", StringComparison.OrdinalIgnoreCase) Then
                Return PriorConfidence.High
            ElseIf s.StartsWith("med", StringComparison.OrdinalIgnoreCase) Then
                Return PriorConfidence.Medium
            ElseIf s.StartsWith("low", StringComparison.OrdinalIgnoreCase) Then
                Return PriorConfidence.Low
            ElseIf s.StartsWith("unk", StringComparison.OrdinalIgnoreCase) Then
                Return PriorConfidence.Unknown
            Else
                ' 或者是数值形式的置信度打分
                Dim score As Double

                If Double.TryParse(s, score) Then
                    If score >= 0.8R Then
                        Return PriorConfidence.High
                    ElseIf score >= 0.5R Then
                        Return PriorConfidence.Medium
                    ElseIf score > 0 Then
                        Return PriorConfidence.Low
                    Else
                        Return PriorConfidence.Unknown
                    End If
                Else
                    Return PriorConfidence.Unknown
                End If
            End If
        End Function

        ''' <summary>
        ''' 引号安全的 csv 行解析：支持双引号包裹的字段以及 <c>""</c> 转义
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        Public Shared Function SplitLine(line As String) As String()
            If String.IsNullOrEmpty(line) Then
                Return New String() {}
            End If

            Dim fields As New System.Collections.Generic.List(Of String)
            Dim sb As New StringBuilder()
            Dim quoted As Boolean = False
            Dim i As Integer = 0

            While i < line.Length
                Dim c As Char = line(i)

                If quoted Then
                    If c = """"c Then
                        If i + 1 < line.Length AndAlso line(i + 1) = """"c Then
                            Call sb.Append(""""c)
                            i += 2
                        Else
                            quoted = False
                            i += 1
                        End If
                    Else
                        Call sb.Append(c)
                        i += 1
                    End If
                Else
                    Select Case c
                        Case """"c
                            quoted = True
                            i += 1
                        Case ","c
                            Call fields.Add(sb.ToString())
                            Call sb.Clear()
                            i += 1
                        Case ChrW(&HD)
                            i += 1
                        Case ChrW(&HA)
                            i += 1
                        Case Else
                            Call sb.Append(c)
                            i += 1
                    End Select
                End If
            End While

            Call fields.Add(sb.ToString())

            Return fields.ToArray
        End Function
    End Class
End Namespace
