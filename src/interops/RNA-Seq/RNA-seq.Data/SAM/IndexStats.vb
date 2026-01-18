Imports System.Globalization
Imports System.IO

Namespace SAM

    Public Structure GeneData
        Public GeneID As String
        Public Length As Double
        Public RawCount As Double
        Public RPK As Double
        Public TPM As Double
    End Structure

    Public Class IndexStats

        Public Property GeneID As String
        Public Property Length As Integer
        Public Property RawCount As Integer
        Public Property UnmappedBases As Integer

        Public Sub ConvertCountsToTPM(inputPath As String, outputPath As String)
            Dim genes As New List(Of GeneData)()
            Dim totalRPK As Double = 0.0
            Dim line As String

            ' --- 第一步：读取文件并计算 RPK ---
            Using reader As New StreamReader(inputPath)
                While Not reader.EndOfStream
                    line = reader.ReadLine()

                    ' 跳过空行或注释行
                    If String.IsNullOrWhiteSpace(line) OrElse line.StartsWith("*") OrElse line.StartsWith("@") Then
                        Continue While
                    End If

                    ' 按空白字符分割行（兼容空格或制表符）
                    Dim parts As String() = line.Split(New Char() {" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)

                    ' 确保至少有 GeneID, Length, Count 三列
                    If parts.Length >= 3 Then
                        Dim g As New GeneData()

                        g.GeneID = parts(0)

                        ' 尝试解析长度
                        If Not Double.TryParse(parts(1), NumberStyles.Any, CultureInfo.InvariantCulture, g.Length) Then
                            g.Length = 0
                        End If

                        ' 尝试解析计数
                        If Not Double.TryParse(parts(2), NumberStyles.Any, CultureInfo.InvariantCulture, g.RawCount) Then
                            g.RawCount = 0
                        End If

                        ' 计算 RPK
                        If g.Length > 0 Then
                            g.RPK = (g.RawCount * 1000.0) / g.Length
                        Else
                            g.RPK = 0.0
                        End If

                        genes.Add(g)
                        totalRPK += g.RPK
                    End If
                End While
            End Using

            ' --- 第二步：根据 totalRPK 计算 TPM ---
            If totalRPK = 0 Then
                Console.WriteLine("Warning: Total RPK is 0. All TPM values will be 0.")
            End If

            For Each g As GeneData In genes
                If totalRPK > 0 Then
                    g.TPM = (g.RPK / totalRPK) * 1000000.0
                Else
                    g.TPM = 0.0
                End If
            Next

            ' --- 第三步：写入结果文件 ---
            Using writer As New StreamWriter(outputPath)
                ' 写入表头
                writer.WriteLine("GeneID" & vbTab & "Length" & vbTab & "RawCount" & vbTab & "RPK" & vbTab & "TPM")

                ' 写入数据，保留4位小数
                For Each g As GeneData In genes
                    writer.WriteLine(String.Format("{0}" & vbTab & "{1}" & vbTab & "{2}" & vbTab & "{3:F4}" & vbTab & "{4:F4}",
                                                  g.GeneID, g.Length, g.RawCount, g.RPK, g.TPM))
                Next
            End Using
        End Sub

    End Class
End Namespace