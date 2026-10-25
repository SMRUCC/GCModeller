Imports System.Globalization
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports EffectorRole = SMRUCC.genomics.Analysis.BNLearn.Effector
Imports PriorEdge = SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge
Imports PriorNet = SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork
Imports std = System.Math

Namespace GRN

    ''' <summary>
    ''' The prior knowledge network loading and the SGrid specific query helpers.
    ''' (先验调控网络的加载，以及面向本算法的查询扩展)
    ''' </summary>
    ''' <remarks>
    ''' 先验知识的数据模型**直接复用** BNLearn 基础项目之中的
    ''' <c>SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork</c> 与
    ''' <c>SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge</c>，
    ''' SGrid 不再维护一份重复的数据模型，只在这里补充
    ''' 「CSV 加载 / 成对查询 / 调控方向与置信度权重 / 基因符号映射」这些面向结构拟合的操作。
    ''' 
    ''' 先验知识在本算法之中是以「软约束」的方式使用的：
    ''' 1. 拟合阶段对先验不支持的交互施加收缩惩罚（<see cref="FittingOptions.PriorPenalty"/>）
    ''' 2. 对与先验方向冲突的交互施加符号惩罚（<see cref="FittingOptions.PriorSignPenalty"/>）
    ''' 3. 稀疏化阶段对先验支持的边给予排序加权（<see cref="FittingOptions.PriorRankBoost"/>）
    ''' 
    ''' 注意：先验知识之中的基因符号与拟合所使用的规整化编号可能并不一致，
    ''' 在进入拟合过程之前需要先调用 <c>Remap</c> 做符号映射。
    ''' </remarks>
    Public Module PriorKnowledge

        #Region "loading"

        ''' <summary>
        ''' 从一个 csv 表格之中加载先验调控网络
        ''' </summary>
        ''' <param name="filePath">
        ''' 需要的列为 TF / TargetGene / RegulationType / Confidence / Evidence（列的顺序无关）
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 该表的证据字段之中含有逗号并且使用双引号包裹，
        ''' 故这里使用自带的引号安全解析器来解析，而不使用反射式的 csv 加载器。
        ''' </remarks>
        Public Function LoadPriorNetwork(filePath As String) As PriorNet
            If String.IsNullOrEmpty(filePath) Then
                Throw New ArgumentNullException(NameOf(filePath))
            End If
            If Not File.Exists(filePath) Then
                Throw New FileNotFoundException($"The prior knowledge network file '{filePath}' does not exist!", filePath)
            End If

            Using reader As New StreamReader(filePath, Encoding.UTF8, detectEncodingFromByteOrderMarks:=True, bufferSize:=1 << 20)
                Return LoadPriorNetwork(reader)
            End Using
        End Function

        ''' <summary>
        ''' 从文本流之中加载先验调控网络
        ''' </summary>
        ''' <param name="reader"></param>
        ''' <returns></returns>
        Public Function LoadPriorNetwork(reader As TextReader) As PriorNet
            If reader Is Nothing Then
                Throw New ArgumentNullException(NameOf(reader))
            End If

            Dim edges As New List(Of PriorEdge)
            Dim seen As New Dictionary(Of String, PriorEdge)(StringComparer.OrdinalIgnoreCase)
            Dim iTF As Integer = 0
            Dim iTarget As Integer = 1
            Dim iType As Integer = 2
            Dim iConfidence As Integer = 3
            Dim iEvidence As Integer = 4
            Dim header As Boolean = True
            Dim line As String

            Do
                line = reader.ReadLine()

                If line Is Nothing Then
                    Exit Do
                End If
                If String.IsNullOrWhiteSpace(line) Then
                    Continue Do
                End If

                Dim fields As String() = SplitCsvLine(line)

                If header Then
                    header = False

                    For i As Integer = 0 To fields.Length - 1
                        Select Case fields(i).Trim().Trim(""""c).ToLowerInvariant()
                            Case "tf", "transcriptionfactor", "regulator", "source"
                                iTF = i
                            Case "targetgene", "target", "gene"
                                iTarget = i
                            Case "regulationtype", "type", "effector"
                                iType = i
                            Case "confidence", "score"
                                iConfidence = i
                            Case "evidence", "pmid", "source_db"
                                iEvidence = i
                        End Select
                    Next

                    Continue Do
                End If

                If fields.Length <= std.Max(std.Max(iTF, iTarget), iConfidence) Then
                    Continue Do
                End If

                Dim tf As String = fields(iTF).Trim()
                Dim target As String = fields(iTarget).Trim()

                If String.IsNullOrEmpty(tf) OrElse String.IsNullOrEmpty(target) Then
                    Continue Do
                End If

                Dim effector As EffectorRole = PriorEdge.ParseEffector(fieldOf(fields, iType))
                Dim confidence As Double = parseConfidence(fieldOf(fields, iConfidence))
                Dim key As String = tf & vbTab & target
                Dim exists As PriorEdge = Nothing

                If seen.TryGetValue(key, exists) Then
                    ' 重复的记录保留置信度更高的一条
                    If confidence > exists.Confidence Then
                        exists.Confidence = confidence
                        exists.RegulationType = effector
                    End If
                Else
                    Dim edge As New PriorEdge With {
                        .TF = tf,
                        .TargetGene = target,
                        .RegulationType = effector,
                        .Confidence = confidence,
                        .Evidence = fieldOf(fields, iEvidence)
                    }

                    Call seen.Add(key, edge)
                    Call edges.Add(edge)
                End If
            Loop

            Return New PriorNet(edges)
        End Function

        ''' <summary>
        ''' 解析置信度字段：既接受 <c>High/Medium/Low</c> 这样的标签，
        ''' 也接受 <c>0.95</c> 这样的数值
        ''' </summary>
        ''' <param name="text"></param>
        ''' <returns></returns>
        Private Function parseConfidence(text As String) As Double
            If String.IsNullOrWhiteSpace(text) Then
                Return 1.0R
            End If

            Dim value As Double

            If Double.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
                Return value
            End If

            Dim mapped As Double = PriorEdge.ParseConfidenceEnum(text.Trim())

            If mapped <= 0 Then
                Return 0.1R
            Else
                Return mapped
            End If
        End Function

        #End Region

        #Region "query"

        ''' <summary>
        ''' 先验边的数量
        ''' </summary>
        ''' <param name="net"></param>
        ''' <returns></returns>
        <Extension>
        Public Function EdgeCount(net As PriorNet) As Integer
            If net Is Nothing OrElse net.Edges Is Nothing Then
                Return 0
            Else
                Return net.Edges.Count
            End If
        End Function

        ''' <summary>
        ''' 先验网络之中所出现的全部基因符号（源与靶的并集）
        ''' </summary>
        ''' <param name="net"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GeneSymbols(net As PriorNet) As String()
            If net Is Nothing Then
                Return New String() {}
            End If

            Return net.GetAllGeneNames()
        End Function

        ''' <summary>
        ''' 按调控方向统计边的数量
        ''' </summary>
        ''' <param name="net"></param>
        ''' <returns></returns>
        <Extension>
        Public Function TypeStatistic(net As PriorNet) As String
            Dim activator As Integer = 0
            Dim inhibitor As Integer = 0
            Dim unknown As Integer = 0

            For Each edge As PriorEdge In SafeEdges(net)
                Select Case edge.RegulationType
                    Case EffectorRole.Activator
                        activator += 1
                    Case EffectorRole.Inhibitor
                        inhibitor += 1
                    Case Else
                        unknown += 1
                End Select
            Next

            Return $"activator={activator}, inhibitor={inhibitor}, unknown={unknown}"
        End Function

        ''' <summary>
        ''' 查询指定的调控关系；不存在的时候返回空值
        ''' </summary>
        ''' <param name="net"></param>
        ''' <param name="tf"></param>
        ''' <param name="target"></param>
        ''' <returns></returns>
        <Extension>
        Public Function FindEdge(net As PriorNet, tf As String, target As String) As PriorEdge
            If net Is Nothing OrElse String.IsNullOrEmpty(tf) OrElse String.IsNullOrEmpty(target) Then
                Return Nothing
            End If

            For Each edge As PriorEdge In net.GetRegulators(target).SafeQuery()
                If String.Equals(edge.TF, tf, StringComparison.OrdinalIgnoreCase) Then
                    Return edge
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' 查询目标基因的全部已知调控因子
        ''' </summary>
        ''' <param name="net"></param>
        ''' <param name="target"></param>
        ''' <returns></returns>
        <Extension>
        Public Function RegulatorEdges(net As PriorNet, target As String) As PriorEdge()
            If net Is Nothing OrElse String.IsNullOrEmpty(target) Then
                Return New PriorEdge() {}
            End If

            Return net.GetRegulators(target).SafeQuery().ToArray()
        End Function

        ''' <summary>
        ''' 先验记录用于排序加权的权重，取置信度（未给出置信度时回落到 0.5）
        ''' </summary>
        ''' <param name="edge"></param>
        ''' <returns></returns>
        <Extension>
        Public Function EdgeWeight(edge As PriorEdge) As Double
            If edge Is Nothing Then
                Return 0
            End If
            If edge.Confidence > 0 Then
                Return edge.Confidence
            Else
                Return 0.5R
            End If
        End Function

        ''' <summary>
        ''' 先验记录所给出的调控方向：+1 激活、-1 抑制、0 未知
        ''' </summary>
        ''' <param name="edge"></param>
        ''' <returns></returns>
        <Extension>
        Public Function EdgeSign(edge As PriorEdge) As Integer
            If edge Is Nothing Then
                Return 0
            End If

            Select Case edge.RegulationType
                Case EffectorRole.Activator
                    Return 1
                Case EffectorRole.Inhibitor
                    Return -1
                Case Else
                    Return 0
            End Select
        End Function

        ''' <summary>
        ''' 调控方向的名称（Activator / Inhibitor / Unknown），用于结果标注
        ''' </summary>
        ''' <param name="edge"></param>
        ''' <returns></returns>
        <Extension>
        Public Function RegulationName(edge As PriorEdge) As String
            If edge Is Nothing Then
                Return NameOf(EffectorRole.Unknown)
            Else
                Return edge.RegulationType.ToString()
            End If
        End Function

        #End Region

        #Region "transform"

        ''' <summary>
        ''' 对先验网络之中的基因符号做映射（例如映射到拟合所使用的规整化编号）
        ''' </summary>
        ''' <param name="net"></param>
        ''' <param name="nameMap">原始基因符号到目标基因符号的映射表（大小写不敏感）</param>
        ''' <returns>映射之后的新对象；映射不到的先验边会被丢弃</returns>
        <Extension>
        Public Function Remap(net As PriorNet, nameMap As IDictionary(Of String, String)) As PriorNet
            If net Is Nothing Then
                Return Nothing
            End If

            Dim records As New List(Of PriorEdge)
            Dim source As String = Nothing
            Dim target As String = Nothing

            For Each edge As PriorEdge In SafeEdges(net)
                If nameMap Is Nothing OrElse nameMap.Count = 0 Then
                    Call records.Add(New PriorEdge(edge))
                    Continue For
                End If
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

            Return collapse(records)
        End Function

        ''' <summary>
        ''' 只保留两端都落在给定基因集合之中的先验边
        ''' </summary>
        ''' <param name="net"></param>
        ''' <param name="genes"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Subset(net As PriorNet, genes As IEnumerable(Of String)) As PriorNet
            Dim index As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each gene As String In genes.SafeQuery()
                If Not String.IsNullOrEmpty(gene) Then
                    Call index.Add(gene)
                End If
            Next

            Return collapse(SafeEdges(net) _
                .Where(Function(e) index.Contains(e.TF) AndAlso index.Contains(e.TargetGene)))
        End Function

        ''' <summary>
        ''' 把边集合收敛为一个先验网络对象；同时合并映射之后可能出现的重复记录
        ''' </summary>
        ''' <param name="edges"></param>
        ''' <returns></returns>
        Private Function collapse(edges As IEnumerable(Of PriorEdge)) As PriorNet
            Dim merged As New Dictionary(Of String, PriorEdge)(StringComparer.OrdinalIgnoreCase)
            Dim records As New List(Of PriorEdge)

            For Each edge As PriorEdge In edges.SafeQuery()
                Dim key As String = edge.TF & vbTab & edge.TargetGene
                Dim exists As PriorEdge = Nothing

                If merged.TryGetValue(key, exists) Then
                    If edge.Confidence > exists.Confidence Then
                        merged(key) = edge
                    End If
                Else
                    Call merged.Add(key, edge)
                    Call records.Add(edge)
                End If
            Next

            Return New PriorNet(records)
        End Function

        #End Region

        #Region "csv text"

        ''' <summary>
        ''' 拆分一行 csv 文本，支持双引号包裹的字段以及 <c>""</c> 形式的转义
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        Public Function SplitCsvLine(line As String) As String()
            If line Is Nothing Then
                Return New String() {}
            End If

            Dim fields As New List(Of String)
            Dim buffer As New StringBuilder()
            Dim quoted As Boolean = False
            Dim i As Integer = 0

            While i < line.Length
                Dim c As Char = line(i)

                If quoted Then
                    If c = """"c Then
                        If i + 1 < line.Length AndAlso line(i + 1) = """"c Then
                            Call buffer.Append(""""c)
                            i += 2
                            Continue While
                        End If

                        quoted = False
                        i += 1
                        Continue While
                    End If

                    Call buffer.Append(c)
                    i += 1
                    Continue While
                End If

                Select Case c
                    Case """"c
                        quoted = True
                        i += 1
                    Case ","c
                        Call fields.Add(buffer.ToString())
                        Call buffer.Clear()
                        i += 1
                    Case Else
                        Call buffer.Append(c)
                        i += 1
                End Select
            End While

            Call fields.Add(buffer.ToString())

            Return fields.ToArray()
        End Function

        #End Region

        ''' <summary>
        ''' 取出指定下标的字段；越界或者字段为空的时候返回空字符串
        ''' </summary>
        Private Function fieldOf(fields As String(), index As Integer) As String
            If fields Is Nothing OrElse index < 0 OrElse index >= fields.Length Then
                Return ""
            End If

            Return If(fields(index), "").Trim()
        End Function

        ''' <summary>
        ''' 安全地枚举先验网络之中的边
        ''' </summary>
        Private Function SafeEdges(net As PriorNet) As IEnumerable(Of PriorEdge)
            If net Is Nothing OrElse net.Edges Is Nothing Then
                Return New PriorEdge() {}
            Else
                Return net.Edges
            End If
        End Function
    End Module
End Namespace
