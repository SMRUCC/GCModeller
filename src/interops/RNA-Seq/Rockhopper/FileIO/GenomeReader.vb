' /********************************************************************************/
'
'  Rockhopper —— 基因组注释目录读取
'
'  原始 Rockhopper 要求 `-g` 参数指向一个目录，其中包含：
'     *.fna  参考基因组核酸序列（FASTA）
'     *.ptt  蛋白编码基因注释（NCBI PTT 表）
'     *.rnt  RNA 基因注释（与 PTT 同格式）
'
'  读取逻辑已内聚在 Core.Genome 构造函数中，本模块提供发现与批量加载的薄封装，
'  并额外支持直接使用框架 PTT 模型读取注释。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.IO

Namespace FileIO

    ''' <summary>
    ''' 基因组目录读取器。
    ''' </summary>
    Public Module GenomeReader

        ''' <summary>
        ''' 在目录中发现 (fna, ptt, rnt) 三件套。
        ''' </summary>
        Public Function Discover(directory As String) As (fna As String, ptt As String, rnt As String)
            Dim fna As String = Nothing
            Dim ptt As String = Nothing
            Dim rnt As String = Nothing

            If Directory.Exists(directory) Then
                For Each file As String In Directory.GetFiles(directory)
                    Select Case Path.GetExtension(file).ToLowerInvariant
                        Case ".fna" : fna = file
                        Case ".ptt" : ptt = file
                        Case ".rnt" : rnt = file
                    End Select
                Next
            End If

            Return (fna, ptt, rnt)
        End Function

        ''' <summary>
        ''' 从一个基因组目录加载 <see cref="Core.Genome"/>。
        ''' </summary>
        ''' <exception cref="DirectoryNotFoundException">目录不存在时抛出。</exception>
        Public Function Load(directory As String) As Core.Genome
            If Not Directory.Exists(directory) Then
                Throw New DirectoryNotFoundException($"参考基因组目录不存在：{directory}")
            End If
            Return New Core.Genome(directory)
        End Function

        ''' <summary>
        ''' 批量从多个基因组目录加载。
        ''' </summary>
        Public Function LoadAll(directories As IEnumerable(Of String)) As List(Of Core.Genome)
            Dim genomes As New List(Of Core.Genome)()
            For Each directory As String In directories
                genomes.Add(Load(directory))
            Next
            Return genomes
        End Function

    End Module

End Namespace
