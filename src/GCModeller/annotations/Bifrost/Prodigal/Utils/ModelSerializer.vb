
Imports System.IO
Imports System.Text

''' <summary>
''' 训练模型序列化/反序列化工具
''' </summary>
Public Class ModelSerializer

    ''' <summary>
    ''' 将训练模型保存到文件
    ''' </summary>
    Public Shared Sub Save(model As TrainingModel, filePath As String)
        Using writer As New System.IO.StreamWriter(filePath, False, Encoding.UTF8)
            writer.WriteLine($"PRODIGAL_VB_MODEL_v{model.Version}")
            writer.WriteLine($"GC_CONTENT={model.GcContent}")
            writer.WriteLine($"AVG_GENE_LENGTH={model.AvgGeneLength}")
            writer.WriteLine($"TRAINED={model.Trained}")
            writer.WriteLine($"ITERATION_COUNT={model.IterationCount}")
            writer.WriteLine($"TRAINING_GENE_COUNT={model.TrainingGeneCount}")
            writer.WriteLine($"CODING_HEXAMER_TOTAL={model.CodingHexamerTotal}")
            writer.WriteLine($"NONCODING_HEXAMER_TOTAL={model.NoncodingHexamerTotal}")

            ' 起始密码子频率
            writer.WriteLine("START_CODON_FREQ")
            For Each kv In model.StartCodonFreq
                writer.WriteLine($"  {kv.Key}={kv.Value}")
            Next

            ' RBS模体得分
            writer.WriteLine("RBS_MOTIF_SCORES")
            For Each kv In model.RbsMotifScores
                writer.WriteLine($"  {kv.Key}={kv.Value}")
            Next

            ' RBS PWM
            writer.WriteLine("RBS_PWM")
            For i As Integer = 0 To 5
                Dim parts As New List(Of String)
                For j As Integer = 0 To 3
                    parts.Add(model.RbsPwm(i, j).ToString("G"))
                Next
                writer.WriteLine($"  {String.Join(",", parts)}")
            Next

            ' 六聚体得分（只保存非零值以节省空间）
            writer.WriteLine("HEXAMER_SCORES")
            For i As Integer = 0 To 4095
                If Math.Abs(model.HexamerScores(i)) > 0.0001 Then
                    writer.WriteLine($"  {i}={model.HexamerScores(i):G}")
                End If
            Next

            ' 编码区六聚体计数
            writer.WriteLine("CODING_HEXAMER_COUNT")
            For i As Integer = 0 To 4095
                If model.CodingHexamerCount(i) > 0 Then
                    writer.WriteLine($"  {i}={model.CodingHexamerCount(i):G}")
                End If
            Next

            ' 非编码区六聚体计数
            writer.WriteLine("NONCODING_HEXAMER_COUNT")
            For i As Integer = 0 To 4095
                If model.NoncodingHexamerCount(i) > 0 Then
                    writer.WriteLine($"  {i}={model.NoncodingHexamerCount(i):G}")
                End If
            Next

            writer.WriteLine("END_MODEL")
        End Using
    End Sub

    ''' <summary>
    ''' 从文件加载训练模型
    ''' </summary>
    Public Shared Function Load(filePath As String) As TrainingModel
        If Not File.Exists(filePath) Then
            Throw New FileNotFoundException($"模型文件未找到: {filePath}")
        End If

        Dim model As New TrainingModel()
        Dim lines = File.ReadAllLines(filePath)
        Dim section As String = ""

        For Each line In lines
            Dim trimmed = line.Trim()
            If String.IsNullOrEmpty(trimmed) Then Continue For

            ' 检测节标题
            If trimmed = "START_CODON_FREQ" OrElse trimmed = "RBS_MOTIF_SCORES" OrElse
               trimmed = "RBS_PWM" OrElse trimmed = "HEXAMER_SCORES" OrElse
               trimmed = "CODING_HEXAMER_COUNT" OrElse trimmed = "NONCODING_HEXAMER_COUNT" OrElse
               trimmed = "END_MODEL" Then
                section = trimmed
                Continue For
            End If

            ' 解析头部信息
            If section = "" Then
                If trimmed.StartsWith("GC_CONTENT=") Then
                    model.GcContent = Double.Parse(trimmed.Substring(11))
                ElseIf trimmed.StartsWith("AVG_GENE_LENGTH=") Then
                    model.AvgGeneLength = Double.Parse(trimmed.Substring(17))
                ElseIf trimmed.StartsWith("TRAINED=") Then
                    model.Trained = Boolean.Parse(trimmed.Substring(8))
                ElseIf trimmed.StartsWith("ITERATION_COUNT=") Then
                    model.IterationCount = Integer.Parse(trimmed.Substring(16))
                ElseIf trimmed.StartsWith("TRAINING_GENE_COUNT=") Then
                    model.TrainingGeneCount = Integer.Parse(trimmed.Substring(20))
                ElseIf trimmed.StartsWith("CODING_HEXAMER_TOTAL=") Then
                    model.CodingHexamerTotal = Double.Parse(trimmed.Substring(21))
                ElseIf trimmed.StartsWith("NONCODING_HEXAMER_TOTAL=") Then
                    model.NoncodingHexamerTotal = Double.Parse(trimmed.Substring(24))
                End If
                Continue For
            End If

            ' 解析各节数据
            If section = "START_CODON_FREQ" Then
                Dim eqIdx = trimmed.IndexOf("="c)
                If eqIdx > 0 Then
                    Dim key = trimmed.Substring(0, eqIdx)
                    Dim val = Double.Parse(trimmed.Substring(eqIdx + 1))
                    model.StartCodonFreq(key) = val
                End If

            ElseIf section = "RBS_MOTIF_SCORES" Then
                Dim eqIdx = trimmed.IndexOf("="c)
                If eqIdx > 0 Then
                    Dim key = trimmed.Substring(0, eqIdx)
                    Dim val = Double.Parse(trimmed.Substring(eqIdx + 1))
                    model.RbsMotifScores(key) = val
                End If

            ElseIf section = "RBS_PWM" Then
                Static pwmRow As Integer = 0
                If pwmRow > 5 Then pwmRow = 0
                Dim parts = trimmed.Split(","c)
                For j As Integer = 0 To Math.Min(3, parts.Length - 1)
                    model.RbsPwm(pwmRow, j) = Double.Parse(parts(j))
                Next
                pwmRow += 1

            ElseIf section = "HEXAMER_SCORES" Then
                Dim eqIdx = trimmed.IndexOf("="c)
                If eqIdx > 0 Then
                    Dim idx = Integer.Parse(trimmed.Substring(0, eqIdx))
                    Dim val = Double.Parse(trimmed.Substring(eqIdx + 1))
                    If idx >= 0 AndAlso idx < 4096 Then
                        model.HexamerScores(idx) = val
                    End If
                End If

            ElseIf section = "CODING_HEXAMER_COUNT" Then
                Dim eqIdx = trimmed.IndexOf("="c)
                If eqIdx > 0 Then
                    Dim idx = Integer.Parse(trimmed.Substring(0, eqIdx))
                    Dim val = Double.Parse(trimmed.Substring(eqIdx + 1))
                    If idx >= 0 AndAlso idx < 4096 Then
                        model.CodingHexamerCount(idx) = val
                    End If
                End If

            ElseIf section = "NONCODING_HEXAMER_COUNT" Then
                Dim eqIdx = trimmed.IndexOf("="c)
                If eqIdx > 0 Then
                    Dim idx = Integer.Parse(trimmed.Substring(0, eqIdx))
                    Dim val = Double.Parse(trimmed.Substring(eqIdx + 1))
                    If idx >= 0 AndAlso idx < 4096 Then
                        model.NoncodingHexamerCount(idx) = val
                    End If
                End If
            End If
        Next

        Return model
    End Function

End Class
