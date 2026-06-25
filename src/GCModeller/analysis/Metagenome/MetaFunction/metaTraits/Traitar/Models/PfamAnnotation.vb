' ============================================================================
' PfamAnnotation.vb - Pfam蛋白质家族注释数据结构
'
' 对应论文中：
'   - 使用HMMER3.0的hmmsearch命令，将氨基酸序列与Pfam数据库(版本27.0)进行比对
'   - 注释出包含的蛋白质家族
'   - 设定阈值（比特分数阈值为25，E值阈值为1e-2）过滤不可靠命中
' ============================================================================

Namespace TraitarVB.Models

    ''' <summary>
    ''' Pfam蛋白质家族注释
    ''' 对应HMMER hmmsearch --domtblout 输出的一行
    ''' </summary>
    Public Class PfamAnnotation

        ''' <summary>目标序列ID（蛋白ID）</summary>
        Public Property TargetName As String

        ''' <summary>目标序列长度</summary>
        Public Property TargetLength As Integer

        ''' <summary>查询HMM名称（Pfam家族名）</summary>
        Public Property QueryName As String

        ''' <summary>查询HMM的accession（如PF00001）</summary>
        Public Property PfamId As String

        ''' <summary>查询HMM长度</summary>
        Public Property QueryLength As Integer

        ''' <summary>E值（全序列）</summary>
        Public Property EValue As Double

        ''' <summary>比特分数（全序列）</summary>
        Public Property BitScore As Double

        ''' <summary>域的E值</summary>
        Public Property DomainEValue As Double

        ''' <summary>域的比特分数</summary>
        Public Property DomainBitScore As Double

        ''' <summary>域在目标序列中的起始位置</summary>
        Public Property DomainStart As Integer

        ''' <summary>域在目标序列中的终止位置</summary>
        Public Property DomainEnd As Integer

        ''' <summary>域在HMM中的起始位置</summary>
        Public Property HmmStart As Integer

        ''' <summary>域在HMM中的终止位置</summary>
        Public Property HmmEnd As Integer

        ''' <summary>Pfam家族描述</summary>
        Public Property Description As String

        ''' <summary>
        ''' 从HMMER domtblout格式的一行解析PfamAnnotation
        ''' domtblout格式（空格分隔）：
        ''' target_name acc tlen query_name acc qlen e-value score bias # of c-Evalue i-Evalue dom_score bias hmm_start hmm_end env_start env_end acc description
        ''' </summary>
        Public Shared Function ParseFromDomtblout(line As String) As PfamAnnotation
            ' 跳过注释行
            If String.IsNullOrEmpty(line) Then Return Nothing
            If line.StartsWith("#") Then Return Nothing

            ' 按空白字符分割
            Dim parts As String() = line.Split(New Char() {" "c, ControlChars.Tab},
                                                StringSplitOptions.RemoveEmptyEntries)
            If parts.Length < 22 Then Return Nothing

            Dim ann As New PfamAnnotation()
            ann.TargetName = parts(0)

            ' parts(1) 是目标accession，可能是"-"
            Integer.TryParse(parts(2), ann.TargetLength)

            ann.QueryName = parts(3)
            ann.PfamId = parts(4)
            If ann.PfamId = "-" Then
                ann.PfamId = ann.QueryName
            End If

            Integer.TryParse(parts(5), ann.QueryLength)
            Double.TryParse(parts(6), ann.EValue)
            Double.TryParse(parts(7), ann.BitScore)

            ' parts(8) bias, parts(9) # of, parts(10) c-Evalue
            Double.TryParse(parts(11), ann.DomainEValue)  ' i-Evalue
            Double.TryParse(parts(12), ann.DomainBitScore)  ' dom_score

            Integer.TryParse(parts(15), ann.HmmStart)  ' hmm_start
            Integer.TryParse(parts(16), ann.HmmEnd)    ' hmm_end
            Integer.TryParse(parts(17), ann.DomainStart) ' env_start
            Integer.TryParse(parts(18), ann.DomainEnd)   ' env_end

            ' 描述（剩余部分）
            If parts.Length > 22 Then
                ann.Description = String.Join(" ", parts, 22, parts.Length - 22)
            End If

            Return ann
        End Function

        ''' <summary>
        ''' 从HMMER --tblout格式的一行解析（简化版）
        ''' </summary>
        Public Shared Function ParseFromTblout(line As String) As PfamAnnotation
            If String.IsNullOrEmpty(line) Then Return Nothing
            If line.StartsWith("#") Then Return Nothing

            Dim parts As String() = line.Split(New Char() {" "c, ControlChars.Tab},
                                                StringSplitOptions.RemoveEmptyEntries)
            If parts.Length < 6 Then Return Nothing

            Dim ann As New PfamAnnotation()
            ann.TargetName = parts(0)
            ann.QueryName = parts(2)
            ann.PfamId = parts(3)
            If ann.PfamId = "-" Then
                ann.PfamId = ann.QueryName
            End If
            Double.TryParse(parts(4), ann.EValue)
            Double.TryParse(parts(5), ann.BitScore)

            Return ann
        End Function

        ''' <summary>
        ''' 返回可读的字符串表示
        ''' </summary>
        Public Overrides Function ToString() As String
            Return String.Format("{0} | {1} | E={2:E4} | bit={3:F2} | {4}",
                                 PfamId, TargetName, EValue, BitScore, Description)
        End Function

    End Class

End Namespace
