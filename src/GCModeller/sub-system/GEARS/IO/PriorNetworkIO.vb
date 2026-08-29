Imports System.IO
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.IO

Namespace IO

    ''' <summary>
    ''' 先验调控网络的 CSV 读取工具
    ''' </summary>
    ''' <remarks>
    ''' <see cref="BnIO.ReadPriorNetwork"/> 接收的是 <see cref="RegulatoryEdge"/> 序列，
    ''' 而 <see cref="RegulatoryEdge.RegulationType"/> 是 <see cref="Effector"/> 枚举（Activator / Inhibitor / Unknown），
    ''' 测试数据文件里写的却是 <c>activation</c> / <c>repression</c> 这样的字符串，
    ''' 直接套用通用 CSV 反序列化无法完成映射。这里提供一个面向该格式的显式解析器，
    ''' 解析出 <see cref="RegulatoryEdge"/> 之后再交给 <see cref="BnIO.ReadPriorNetwork"/> 组装网络。
    '''
    ''' 期望的 CSV 表头：<c>TF,TargetGene,RegulationType,Confidence,Evidence</c>
    ''' </remarks>
    Public Module PriorNetworkIO

        ''' <summary>CSV/TSV 支持的分隔符</summary>
        ReadOnly delimiters As Char() = {","c, ControlChars.Tab, ";"c}

        ''' <summary>
        ''' 从 CSV 文件加载先验调控网络
        ''' </summary>
        ''' <param name="path">CSV 文件路径，表头为 TF,TargetGene,RegulationType,Confidence,Evidence</param>
        ''' <returns>先验调控网络对象</returns>
        Public Function LoadPriorNetwork(path As String) As PriorNetwork
            If Not File.Exists(path) Then
                Throw New FileNotFoundException("先验调控网络文件不存在", path)
            End If

            Return BnIO.ReadPriorNetwork(ParseRegulatoryEdges(path))
        End Function

        ''' <summary>
        ''' 解析 CSV 文件中的所有调控边
        ''' </summary>
        ''' <param name="path">CSV 文件路径</param>
        ''' <returns>调控边序列</returns>
        Public Iterator Function ParseRegulatoryEdges(path As String) As IEnumerable(Of RegulatoryEdge)
            Dim lines As String() = File.ReadAllLines(path)

            If lines.Length = 0 Then
                Return
            End If

            Dim header As String() = SplitCsvLine(lines(0))
            Dim colTF As Integer = IndexOfColumn(header, "TF", 0)
            Dim colTarget As Integer = IndexOfColumn(header, "TargetGene", 1)
            Dim colType As Integer = IndexOfColumn(header, "RegulationType", 2)
            Dim colConf As Integer = IndexOfColumn(header, "Confidence", 3)
            Dim colEvidence As Integer = IndexOfColumn(header, "Evidence", 4)

            For i As Integer = 1 To lines.Length - 1
                If String.IsNullOrWhiteSpace(lines(i)) Then
                    Continue For
                End If

                Dim tokens As String() = SplitCsvLine(lines(i))

                If tokens.Length <= colTarget Then
                    Continue For
                End If

                Dim tf As String = tokens(colTF).Trim()
                Dim target As String = tokens(colTarget).Trim()

                If String.IsNullOrEmpty(tf) OrElse String.IsNullOrEmpty(target) Then
                    Continue For
                End If

                Dim regType As Effector = Effector.Unknown
                Dim confidence As Double = 1.0
                Dim evidence As String = ""

                If colType >= 0 AndAlso colType < tokens.Length Then
                    regType = ParseRegulationType(tokens(colType))
                End If
                If colConf >= 0 AndAlso colConf < tokens.Length Then
                    Double.TryParse(tokens(colConf).Trim(), confidence)
                End If
                If colEvidence >= 0 AndAlso colEvidence < tokens.Length Then
                    evidence = tokens(colEvidence).Trim()
                End If

                Yield New RegulatoryEdge With {
                    .TF = tf,
                    .TargetGene = target,
                    .RegulationType = regType,
                    .Confidence = confidence,
                    .Evidence = evidence
                }
            Next
        End Function

        ''' <summary>
        ''' 把文本形式的调控类型映射为 <see cref="Effector"/> 枚举
        ''' </summary>
        ''' <param name="text">
        ''' 原始文本，支持 activation / activate / activator / up / positive
        ''' 与 repression / repress / inhibitor / inhibition / down / negative
        ''' </param>
        ''' <returns>效应器枚举；无法识别时返回 <see cref="Effector.Unknown"/></returns>
        Public Function ParseRegulationType(text As String) As Effector
            If String.IsNullOrWhiteSpace(text) Then
                Return Effector.Unknown
            End If

            Select Case text.Trim().ToLower()
                Case "activation", "activate", "activator", "activating", "up", "positive", "1"
                    Return Effector.Activator
                Case "repression", "repress", "repressor", "inhibition", "inhibit", "inhibitor", "down", "negative", "-1"
                    Return Effector.Inhibitor
                Case Else
                    Return Effector.Unknown
            End Select
        End Function

        ''' <summary>
        ''' 按分隔符切分一行 CSV 文本
        ''' </summary>
        ''' <param name="line">原始文本行</param>
        ''' <returns>字段数组</returns>
        Private Function SplitCsvLine(line As String) As String()
            Return line.Split(delimiters, StringSplitOptions.None)
        End Function

        ''' <summary>
        ''' 在表头中查找指定列名的位置
        ''' </summary>
        ''' <param name="header">表头字段数组</param>
        ''' <param name="name">目标列名</param>
        ''' <param name="defaultIndex">找不到时回退使用的列索引</param>
        ''' <returns>列索引</returns>
        Private Function IndexOfColumn(header As String(), name As String, defaultIndex As Integer) As Integer
            For i As Integer = 0 To header.Length - 1
                If String.Equals(header(i).Trim(), name, StringComparison.OrdinalIgnoreCase) Then
                    Return i
                End If
            Next

            Return defaultIndex
        End Function
    End Module
End Namespace
