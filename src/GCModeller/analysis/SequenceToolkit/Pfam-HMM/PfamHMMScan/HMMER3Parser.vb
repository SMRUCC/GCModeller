' ============================================================================
' HMMER3模型解析器与蛋白质序列分类注释模块
' 
' 基于现有HMM算法框架，实现HMMER3格式模型文件的读取和蛋白质序列分类注释功能
' 
' Author: 基于用户现有HMM代码框架扩展
' Copyright (c) 2024 GPL3 Licensed
' ============================================================================

Imports System.IO
Imports System.Text.RegularExpressions

''' <summary>
''' HMMER3模型文件解析器
''' 用于读取.hmm格式的HMMER3模型文件
''' </summary>
''' <remarks>
''' HMMER3文件格式说明：
''' - HMMER3/f: 文件格式标识
''' - NAME: 模型名称
''' - LENG: 模型长度（匹配状态数）
''' - ALPH: 字母表类型（amino表示蛋白质）
''' - HMM: 概率矩阵部分
'''   - COMPO行: 背景分布
'''   - 每个状态包含:
'''     - 匹配发射概率（20个氨基酸）
'''     - 插入发射概率（20个氨基酸）
'''     - 转移概率（7个：m->m, m->i, m->d, i->m, i->i, d->m, d->d）
''' </remarks>
Public Class HMMER3Parser

    ' 氨基酸字母表顺序（HMMER3标准顺序）
    Public Shared ReadOnly AA_ALPHABET As String() = {
            "A", "C", "D", "E", "F", "G", "H", "I", "K", "L",
            "M", "N", "P", "Q", "R", "S", "T", "V", "W", "Y"
        }

    ''' <summary>
    ''' 解析HMMER3模型文件
    ''' </summary>
    ''' <param name="filePath">HMMER3模型文件路径</param>
    ''' <returns>解析后的ProfileHMM对象</returns>
    Public Shared Function Parse(filePath As String) As ProfileHMM
        If Not File.Exists(filePath) Then
            Throw New FileNotFoundException($"HMMER3 model file not found: {filePath}")
        End If

        Dim lines As String() = File.ReadAllLines(filePath)
        Return ParseLines(lines)
    End Function

    ''' <summary>
    ''' 解析HMMER3模型文本内容
    ''' </summary>
    ''' <param name="content">HMMER3模型文本内容</param>
    ''' <returns>解析后的ProfileHMM对象</returns>
    Public Shared Function ParseContent(content As String) As ProfileHMM
        Dim lines As String() = content.Split({vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)
        Return ParseLines(lines)
    End Function

    ''' <summary>
    ''' 解析HMMER3模型行数据
    ''' </summary>
    Private Shared Function ParseLines(lines As String()) As ProfileHMM
        Dim model As New ProfileHMM()

        Dim i As Integer = 0
        While i < lines.Length
            Dim line As String = lines(i).Trim()

            ' 解析头部信息
            If line.StartsWith("HMMER3") Then
                model.Version = line
            ElseIf line.StartsWith("NAME") Then
                model.Name = ParseValue(line)
            ElseIf line.StartsWith("LENG") Then
                model.Length = Integer.Parse(ParseValue(line))
            ElseIf line.StartsWith("ALPH") Then
                model.Alphabet = ParseValue(line)
            ElseIf line.StartsWith("NSEQ") Then
                model.NumSequences = Integer.Parse(ParseValue(line))
            ElseIf line.StartsWith("EFFN") Then
                model.EffectiveNum = Double.Parse(ParseValue(line))
            ElseIf line.StartsWith("CKSUM") Then
                model.Checksum = Long.Parse(ParseValue(line))
            ElseIf line.StartsWith("STATS LOCAL MSV") Then
                model.StatsMSV = ParseStatsLine(line)
            ElseIf line.StartsWith("STATS LOCAL VITERBI") Then
                model.StatsViterbi = ParseStatsLine(line)
            ElseIf line.StartsWith("STATS LOCAL FORWARD") Then
                model.StatsForward = ParseStatsLine(line)
            ElseIf line.StartsWith("HMM") Then
                ' 跳过HMM标题行和列标题行
                i += 2
                ' 解析COMPO行（背景分布）
                If i < lines.Length AndAlso lines(i).Trim().StartsWith("COMPO") Then
                    ParseCompoLine(lines(i), model)
                    i += 1
                    ' 解析COMPO的第二行（插入发射概率）
                    If i < lines.Length Then
                        ParseCompoInsertLine(lines(i), model)
                        i += 1
                    End If
                    ' 解析COMPO的第三行（转移概率）
                    If i < lines.Length Then
                        ParseCompoTransLine(lines(i), model)
                        i += 1
                    End If
                End If

                ' 解析每个匹配状态的发射概率和转移概率
                While i < lines.Length
                    Dim currentLine As String = lines(i).Trim()
                    If String.IsNullOrEmpty(currentLine) OrElse currentLine.StartsWith("//") Then
                        Exit While
                    End If

                    ' 检查是否是状态行（以数字开头）
                    Dim match As Match = Regex.Match(currentLine, "^\s*(\d+)")
                    If match.Success Then
                        Dim stateIndex As Integer = Integer.Parse(match.Groups(1).Value)
                        ParseStateBlock(lines, i, model, stateIndex)
                        i += 3 ' 每个状态块有3行
                    Else
                        i += 1
                    End If
                End While
            End If

            i += 1
        End While

        Return model.InitializeHMMParameters()
    End Function

    ''' <summary>
    ''' 解析键值对行
    ''' </summary>
    Private Shared Function ParseValue(line As String) As String
        Dim parts As String() = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
        If parts.Length >= 2 Then
            Return parts(1)
        End If
        Return ""
    End Function

    ''' <summary>
    ''' 解析统计信息行
    ''' </summary>
    Private Shared Function ParseStatsLine(line As String) As (mu As Double, lambda As Double)
        Dim parts As String() = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
        If parts.Length >= 4 Then
            Return (Double.Parse(parts(3)), Double.Parse(parts(4)))
        End If
        Return (0.0, 0.0)
    End Function

    ''' <summary>
    ''' 解析COMPO行（背景发射概率）
    ''' </summary>
    Private Shared Sub ParseCompoLine(line As String, model As ProfileHMM)
        Dim parts As String() = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
        ' 跳过"COMPO"标识符
        Dim startIndex As Integer = 1
        Dim probs As New List(Of Double)
        For i As Integer = startIndex To Math.Min(startIndex + 19, parts.Length - 1)
            probs.Add(Double.Parse(parts(i)))
        Next
        model.CompositionEmission = probs.ToArray()
    End Sub

    ''' <summary>
    ''' 解析COMPO插入发射概率行
    ''' </summary>
    Private Shared Sub ParseCompoInsertLine(line As String, model As ProfileHMM)
        Dim parts As String() = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
        Dim probs As New List(Of Double)
        For i As Integer = 0 To Math.Min(19, parts.Length - 1)
            probs.Add(Double.Parse(parts(i)))
        Next
        model.CompositionInsert = probs.ToArray()
    End Sub

    ''' <summary>
    ''' 解析COMPO转移概率行
    ''' </summary>
    Private Shared Sub ParseCompoTransLine(line As String, model As ProfileHMM)
        Dim parts As String() = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
        Dim probs As New List(Of Double)
        For i As Integer = 0 To Math.Min(6, parts.Length - 1)
            Dim val As Double
            If Double.TryParse(parts(i), val) Then
                probs.Add(val)
            ElseIf parts(i) = "*" Then
                probs.Add(Double.NegativeInfinity)
            Else
                probs.Add(0.0)
            End If
        Next
        model.CompositionTransitions = probs.ToArray()
    End Sub

    ''' <summary>
    ''' 解析状态块（发射概率和转移概率）
    ''' </summary>
    Private Shared Sub ParseStateBlock(lines As String(), startIndex As Integer, model As ProfileHMM, stateIndex As Integer)
        ' 第一行：匹配状态发射概率
        Dim matchLine As String = lines(startIndex).Trim()
        Dim matchParts As String() = matchLine.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)

        Dim matchEmission As New List(Of Double)
        ' 格式: 状态号 20个发射概率 状态号 残基 rf mm cs map
        ' 发射概率从索引1开始，共20个
        For i As Integer = 1 To Math.Min(20, matchParts.Length - 1)
            matchEmission.Add(Double.Parse(matchParts(i)))
        Next

        ' 第二行：插入状态发射概率
        Dim insertLine As String = lines(startIndex + 1).Trim()
        Dim insertParts As String() = insertLine.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
        Dim insertEmission As New List(Of Double)
        For i As Integer = 0 To Math.Min(19, insertParts.Length - 1)
            insertEmission.Add(Double.Parse(insertParts(i)))
        Next

        ' 第三行：转移概率 (m->m, m->i, m->d, i->m, i->i, d->m, d->d)
        Dim transLine As String = lines(startIndex + 2).Trim()
        Dim transParts As String() = transLine.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
        Dim transitions As New List(Of Double)
        For i As Integer = 0 To Math.Min(6, transParts.Length - 1)
            Dim val As Double
            If Double.TryParse(transParts(i), val) Then
                transitions.Add(val)
            ElseIf transParts(i) = "*" Then
                transitions.Add(Double.NegativeInfinity)
            Else
                transitions.Add(0.0)
            End If
        Next

        ' 添加到模型
        model.MatchEmissions.Add(matchEmission.ToArray())
        model.InsertEmissions.Add(insertEmission.ToArray())
        model.Transitions.Add(transitions.ToArray())
    End Sub

End Class
