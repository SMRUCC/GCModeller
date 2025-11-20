Imports System.IO
Imports System.Text

Module BamReader

    ' 定义一个简单的结构来存储比对记录信息
    Public Structure BamAlignment
        Public RefID As Integer
        Public Position As Integer
        Public ReadName As String
        Public Sequence As String
        Public Quality As String
    End Structure

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="filepath">
    ''' 此代码示例假设BAM文件未经过BGZF压缩
    ''' 
    ''' ```bash
    ''' samtools view -u -h input.sam > uncompressed.bam
    ''' ```
    ''' </param>
    ''' <returns></returns>
    Public Function Read(filepath As String)
        Using fs As New FileStream(filepath, FileMode.Open, FileAccess.Read)
            Using br As New BinaryReader(fs)
                ' 1. 读取文件头
                ReadBamHeader(br)

                ' 2. 读取参考序列字典
                ReadReferenceSequences(br)

                ' 3. 尝试读取第一条比对记录作为演示
                Console.WriteLine(vbCrLf & "--- 开始读取第一条比对记录 ---")
                ReadFirstAlignment(br)
                Console.WriteLine("--- 第一条比对记录读取完毕 ---" & vbCrLf)

            End Using
        End Using

        Console.WriteLine("处理完成，按任意键退出...")
        Console.ReadKey()
    End Function

    ' 读取BAM文件头（魔数和头部文本）
    Sub ReadBamHeader(br As BinaryReader)
        Console.WriteLine("--- 读取文件头 ---")

        ' 读取魔数 (4 bytes)
        Dim magicBytes As Byte() = br.ReadBytes(4)
        Dim magic As String = Encoding.ASCII.GetString(magicBytes)
        Console.WriteLine($"魔数: {magic}")

        If magic <> "BAM" & ChrW(1) Then
            Throw New InvalidDataException("这不是一个有效的BAM文件 (魔数不匹配)。")
        End If

        ' 读取头部文本 (以 \0 结尾)
        Dim headerTextBytes As New List(Of Byte)()
        Dim b As Byte
        While True
            b = br.ReadByte()
            If b = 0 Then ' 遇到空字符，表示结束
                Exit While
            End If
            headerTextBytes.Add(b)
        End While
        Dim headerText As String = Encoding.ASCII.GetString(headerTextBytes.ToArray())
        Console.WriteLine("头部文本 (前100字符):")
        If headerText.Length > 100 Then
            Console.WriteLine(headerText.Substring(0, 100) & "...")
        Else
            Console.WriteLine(headerText)
        End If
        Console.WriteLine("--- 文件头读取完毕 ---" & vbCrLf)
    End Sub

    ' 读取参考序列字典
    Sub ReadReferenceSequences(br As BinaryReader)
        Console.WriteLine("--- 读取参考序列字典 ---")

        ' 读取参考序列数量 n_ref
        Dim nRef As Integer = br.ReadInt32()
        Console.WriteLine($"参考序列数量: {nRef}")

        For i As Integer = 0 To nRef - 1
            ' 读取名称长度 l_name
            Dim lName As Integer = br.ReadInt32()

            ' 读取名称 (以 \0 结尾)
            Dim nameBytes As Byte() = br.ReadBytes(lName)
            ' 移除末尾的空字符
            Dim name As String = Encoding.ASCII.GetString(nameBytes).TrimEnd(ChrW(0))

            ' 读取序列长度 l_ref
            Dim lRef As Integer = br.ReadInt32()

            Console.WriteLine($"  [{i}] 名称: {name}, 长度: {lRef}")
        Next
        Console.WriteLine("--- 参考序列字典读取完毕 ---" & vbCrLf)
    End Sub

    ' 读取第一条比对记录（简化版）
    Sub ReadFirstAlignment(br As BinaryReader)
        ' 检查是否已到文件末尾
        If br.BaseStream.Position >= br.BaseStream.Length Then
            Console.WriteLine("文件中不存在比对记录。")
            Return
        End If

        Dim alignment As New BamAlignment()

        ' --- 核心对齐块 ---
        Dim blockSize As Integer = br.ReadInt32()
        alignment.RefID = br.ReadInt32()
        alignment.Position = br.ReadInt32() + 1 ' BAM是0-based，SAM是1-based，这里+1方便理解
        Dim binMqNl As UInteger = br.ReadUInt32()
        Dim flagNc As UInteger = br.ReadUInt32()
        Dim lSeq As Integer = br.ReadInt32()
        ' 跳过一些我们不演示的字段
        br.ReadInt32() ' next_refID
        br.ReadInt32() ' next_pos
        br.ReadInt32() ' tlen

        ' --- 解析打包的字段 ---
        Dim readNameLen As Integer = CInt(binMqNl And &HFFUI) ' 取最低8位
        Dim cigarOpsCount As Integer = CInt(flagNc And &HFFFFUI) ' 取最低16位

        ' --- 变长数据块 ---
        ' 读取Read Name
        Dim nameBytes As Byte() = br.ReadBytes(readNameLen)
        alignment.ReadName = Encoding.ASCII.GetString(nameBytes).TrimEnd(ChrW(0))

        ' 跳过CIGAR操作
        br.ReadBytes(cigarOpsCount * 4) ' 每个CIGAR操作是4字节

        ' 读取序列
        Dim seqByteCount As Integer = (lSeq + 1) \ 2
        Dim seqBytes As Byte() = br.ReadBytes(seqByteCount)
        Dim seqBuilder As New StringBuilder(lSeq)
        For Each seqByte As Byte In seqBytes
            ' 每4个bit代表一个碱基
            Dim base1 As Integer = (seqByte >> 4) And &HF
            Dim base2 As Integer = seqByte And &HF

            seqBuilder.Append(GetBaseChar(base1))
            If seqBuilder.Length < lSeq Then
                seqBuilder.Append(GetBaseChar(base2))
            End If
        Next
        alignment.Sequence = seqBuilder.ToString()

        ' 读取质量分数
        Dim qualBytes As Byte() = br.ReadBytes(lSeq)
        Dim qualBuilder As New StringBuilder(lSeq)
        For Each qual As Byte In qualBytes
            qualBuilder.Append(ChrW(qual + 33)) ' Phred+33 转换为ASCII字符
        Next
        alignment.Quality = qualBuilder.ToString()

        ' 跳过Optional Tags，因为它们格式复杂多变
        ' 我们已经读取了核心块和主要的变长块，剩余的字节就是Optional Tags
        Dim bytesReadSoFar As Integer = 32 + 4 * 4 + readNameLen + cigarOpsCount * 4 + seqByteCount + lSeq
        Dim optionalTagSize As Integer = blockSize - bytesReadSoFar
        If optionalTagSize > 0 Then
            br.ReadBytes(optionalTagSize)
        End If

        ' --- 打印结果 ---
        Console.WriteLine($"Block Size: {blockSize}")
        Console.WriteLine($"Read Name: {alignment.ReadName}")
        Console.WriteLine($"RefID: {alignment.RefID}, Position: {alignment.Position}")
        Console.WriteLine($"Sequence Length: {lSeq}")
        Console.WriteLine($"Sequence (前50bp): {If(alignment.Sequence.Length > 50, alignment.Sequence.Substring(0, 50) & "...", alignment.Sequence)}")
        Console.WriteLine($"Quality (前50个字符): {If(alignment.Quality.Length > 50, alignment.Quality.Substring(0, 50) & "...", alignment.Quality)}")
    End Sub

    ' 辅助函数：将4位编码的碱基转换为字符
    Function GetBaseChar(val As Integer) As Char
        Select Case val
            Case 1 : Return "A"c
            Case 2 : Return "C"c
            Case 4 : Return "G"c
            Case 8 : Return "T"c
            Case 15 : Return "N"c
            Case Else : Return "."c ' 未知或错误值
        End Select
    End Function

End Module
