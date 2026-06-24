' ============================================================================
' Module 1: Genome Annotation & Feature Extraction
' File: GenomeAnnotation/PfamAnnotator.vb
'
' 功能: 调用 HMMER3 (hmmsearch/hmmscan) 对蛋白质序列进行 Pfam 家族注释，
'       并解析 HMMER 输出（--domtblout 或 --tblout 格式）。
'       对应论文中 "使用 HMMER3.0 的 hmmsearch 命令与 Pfam 数据库比对"。
'
' 论文阈值: 比特分数阈值 25，E值阈值 1e-2
' ============================================================================

Imports System.IO
Imports System.Collections.Generic
Imports System.Diagnostics

Namespace Traitar.GenomeAnnotation

    ''' <summary>
    ''' 表示一条 Pfam 家族命中记录
    ''' </summary>
    Public Class PfamHit
        ''' <summary>蛋白质序列ID（target）</summary>
        Public Property ProteinId As String
        ''' <summary>Pfam 家族 accession（如 PF00001）</summary>
        Public Property PfamAcc As String
        ''' <summary>Pfam 家族名称</summary>
        Public Property PfamName As String
        ''' <summary>E值</summary>
        Public Property EValue As Double
        ''' <summary>比特分数</summary>
        Public Property BitScore As Double
        ''' <summary>命中区域起始</summary>
        Public Property AliFrom As Integer
        ''' <summary>命中区域终止</summary>
        Public Property AliTo As Integer

        Public Overrides Function ToString() As String
            Return $"{ProteinId} -> {PfamAcc} (E={EValue:G3}, score={BitScore:F1})"
        End Function
    End Class

    ''' <summary>
    ''' Pfam 注释器：调用 HMMER 并解析结果
    ''' </summary>
    Public Class PfamAnnotator

        ''' <summary>比特分数阈值（论文中为 25）</summary>
        Public Property BitScoreThreshold As Double = 25.0

        ''' <summary>E值阈值（论文中为 1e-2）</summary>
        Public Property EValueThreshold As Double = 0.01

        ''' <summary>HMMER 可执行文件路径（如 /usr/bin/hmmsearch）</summary>
        Public Property HmmerPath As String = "hmmsearch"

        ''' <summary>Pfam 数据库文件路径（Pfam-A.hmm）</summary>
        Public Property PfamDbPath As String

        ''' <summary>
        ''' 调用 HMMER 进行 Pfam 注释
        ''' 命令: hmmsearch --cpu N --domtblout output.tblout Pfam-A.hmm proteins.fasta
        ''' </summary>
        ''' <param name="proteinFasta">蛋白质 FASTA 文件路径</param>
        ''' <param name="outputTblout">输出 tblout 文件路径</param>
        Public Function RunHmmer(proteinFasta As String, outputTblout As String) As Boolean
            If String.IsNullOrEmpty(PfamDbPath) Then
                Throw New InvalidOperationException("PfamDbPath 未设置")
            End If

            Dim args = $"--cpu 4 --domtblout ""{outputTblout}"" ""{PfamDbPath}"" ""{proteinFasta}"""
            Dim psi As New ProcessStartInfo With {
                .FileName = HmmerPath,
                .Arguments = args,
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True
            }

            Using proc As New Process()
                proc.StartInfo = psi
                proc.Start()
                proc.WaitForExit()
                Return proc.ExitCode = 0
            End Function

        ''' <summary>
        ''' 解析 HMMER --tblout 输出文件
        ''' 格式: target_name acc query_name acc E-value score bias ...
        ''' </summary>
        Public Function ParseTblout(tbloutPath As String) As List(Of PfamHit)
            Dim hits As New List(Of PfamHit)
            For Each line As String In File.ReadAllLines(tbloutPath)
                If String.IsNullOrEmpty(line) Then Continue For
                If line.StartsWith("#") Then Continue For

                ' 按空白分割
                Dim fields = line.Split(New Char() {" "c, ControlChars.Tab},
                                        StringSplitOptions.RemoveEmptyEntries)
                If fields.Length < 10 Then Continue For

                ' target=protein(0), target_acc(1), query=Pfam(2), query_acc(3),
                ' E-value(4), score(5)
                Dim proteinId = fields(0)
                Dim pfamName = fields(2)
                Dim pfamAcc = fields(3)
                Dim eValue As Double
                Dim bitScore As Double
                If Not Double.TryParse(fields(4), eValue) Then Continue For
                If Not Double.TryParse(fields(5), bitScore) Then Continue For

                ' 应用阈值过滤
                If bitScore < BitScoreThreshold Then Continue For
                If eValue > EValueThreshold Then Continue For

                ' 提取纯 Pfam accession（去掉版本号 .xx）
                If pfamAcc.StartsWith("PF") Then
                    Dim dotIdx = pfamAcc.IndexOf("."c)
                    If dotIdx > 0 Then pfamAcc = pfamAcc.Substring(0, dotIdx)
                ElseIf pfamName.StartsWith("PF") Then
                    pfamAcc = pfamName
                    Dim dotIdx = pfamAcc.IndexOf("."c)
                    If dotIdx > 0 Then pfamAcc = pfamAcc.Substring(0, dotIdx)
                End If

                hits.Add(New PfamHit With {
                    .ProteinId = proteinId,
                    .PfamAcc = pfamAcc,
                    .PfamName = pfamName,
                    .EValue = eValue,
                    .BitScore = bitScore
                })
            Next

            Return hits
        End Function

        ''' <summary>
        ''' 解析 HMMER --domtblout 输出文件（域级别命中）
        ''' 格式: target_name acc tlen query_name acc qlen E-value score bias
        '''        # of c-Evalue i-Evalue dom_score bias hmm_from hmm_to ali_from ali_to env_from env_to acc description
        ''' </summary>
        Public Function ParseDomTblout(domtbloutPath As String) As List(Of PfamHit)
            Dim hits As New List(Of PfamHit)
            For Each line As String In File.ReadAllLines(domtbloutPath)
                If String.IsNullOrEmpty(line) Then Continue For
                If line.StartsWith("#") Then Continue For

                Dim fields = line.Split(New Char() {" "c, ControlChars.Tab},
                                        StringSplitOptions.RemoveEmptyEntries)
                If fields.Length < 23 Then Continue For

                ' target=protein(0), target_acc(1), query=Pfam(3), query_acc(4),
                ' full_E-value(6), full_score(7), ali_from(17), ali_to(18)
                Dim proteinId = fields(0)
                Dim pfamName = fields(3)
                Dim pfamAcc = fields(4)
                Dim eValue As Double
                Dim bitScore As Double
                If Not Double.TryParse(fields(6), eValue) Then Continue For
                If Not Double.TryParse(fields(7), bitScore) Then Continue For

                If bitScore < BitScoreThreshold Then Continue For
                If eValue > EValueThreshold Then Continue For

                If pfamAcc.StartsWith("PF") Then
                    Dim dotIdx = pfamAcc.IndexOf("."c)
                    If dotIdx > 0 Then pfamAcc = pfamAcc.Substring(0, dotIdx)
                End If

                Dim aliFrom As Integer, aliTo As Integer
                Integer.TryParse(fields(17), aliFrom)
                Integer.TryParse(fields(18), aliTo)

                hits.Add(New PfamHit With {
                    .ProteinId = proteinId,
                    .PfamAcc = pfamAcc,
                    .PfamName = pfamName,
                    .EValue = eValue,
                    .BitScore = bitScore,
                    .AliFrom = aliFrom,
                    .AliTo = aliTo
                })
            Next

            Return hits
        End Function
    End Class
End Namespace
