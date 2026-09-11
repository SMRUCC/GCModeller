' /********************************************************************************/
'
'  Rockhopper —— 结果文件输出
'
'  复刻原始 Rockhopper 的输出约定（CLI_API.java / Genome.java / Operons.java）：
'    * summary.txt        —— 运行汇总
'    * *_transcripts.txt  —— 每个基因/新转录本的转录起止、表达量、q 值
'    * *_operons.txt      —— 预测得到的多基因操纵子
'
'  表头与列顺序严格保持原样，以便 API/TSSsAnalysis.LoadResult 与
'  API.LoadOperonResult 能够回读。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Text

Namespace FileIO

    ''' <summary>
    ''' Rockhopper 分析结果写出器。
    ''' </summary>
    Public Module ResultWriter

        ''' <summary>
        ''' 写出 *_transcripts.txt（表头 + 全部基因行）。
        ''' </summary>
        Public Function WriteTranscripts(path As String, genome As Core.Genome, conditions As List(Of Core.Condition), labels As String(), Optional encoding As Encoding = Nothing) As Boolean
            If encoding Is Nothing Then encoding = Encoding.ASCII
            Using writer As New StreamWriter(path, False, encoding)
                writer.Write(genome.GenesToString(conditions, labels))
            End Using
            Return True
        End Function

        ''' <summary>
        ''' 写出 summary.txt 的内容（由调用方决定具体统计项）。
        ''' </summary>
        Public Function WriteSummary(path As String, Optional content As String = Nothing, Optional encoding As Encoding = Nothing) As Boolean
            If encoding Is Nothing Then encoding = Encoding.ASCII
            Using writer As New StreamWriter(path, False, encoding)
                If content IsNot Nothing Then writer.Write(content)
            End Using
            Return True
        End Function

        ''' <summary>
        ''' 写出 *_operons.txt（每行一个操纵子：Strand, Start, Stop, 逗号分隔的基因列表）。
        ''' </summary>
        Public Function WriteOperons(path As String, operons As IEnumerable(Of Core.Operon), Optional encoding As Encoding = Nothing) As Boolean
            If encoding Is Nothing Then encoding = Encoding.ASCII
            Using writer As New StreamWriter(path, False, encoding)
                For Each operon As Core.Operon In operons
                    writer.WriteLine(operon.ToString())
                Next
            End Using
            Return True
        End Function

    End Module

End Namespace
