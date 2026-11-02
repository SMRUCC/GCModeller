Imports System.IO
Imports System.Text

Namespace Clustal

    ''' <summary>
    ''' 比对结果的格式化输出入口
    ''' </summary>
    Public Module MSAIO

        ''' <summary>
        ''' 输出 FASTA 文本
        ''' </summary>
        Public Function Fasta(result As ClustalOmegaResult, Optional lineBreak As Integer = 60) As String
            Return FastaWriter.ToText(result.names, result.MSA, lineBreak)
        End Function

        ''' <summary>
        ''' 输出 Clustal(.aln)文本
        ''' </summary>
        Public Function Clustal(result As ClustalOmegaResult) As String
            Return ClustalWriter.ToText(result.names, result.MSA)
        End Function

        ''' <summary>
        ''' 输出 Stockholm 文本
        ''' </summary>
        Public Function Stockholm(result As ClustalOmegaResult, Optional id As String = Nothing) As String
            Return StockholmWriter.ToText(result.names, result.MSA, id)
        End Function

        ''' <summary>
        ''' 输出 PHYLIP 文本
        ''' </summary>
        Public Function Phylip(result As ClustalOmegaResult) As String
            Return PhylipWriter.ToText(result.names, result.MSA)
        End Function

        ''' <summary>
        ''' 按指定格式保存结果文件
        ''' </summary>
        Public Function Save(result As ClustalOmegaResult, format As MSAFormat, path As String) As Boolean
            Dim folder As String = Path.GetDirectoryName(path)

            If Not String.IsNullOrEmpty(folder) AndAlso Not Directory.Exists(folder) Then
                Call Directory.CreateDirectory(folder)
            End If

            Dim text As String

            Select Case format
                Case MSAFormat.Clustal
                    text = Clustal(result)
                Case MSAFormat.Stockholm
                    text = Stockholm(result, BaseName(path))
                Case MSAFormat.Phylip
                    text = Phylip(result)
                Case Else
                    text = Fasta(result)
            End Select

            Call File.WriteAllText(path, text, New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False))
            Return True
        End Function

        Private Function BaseName(path As String) As String
            Dim name As String = Path.GetFileNameWithoutExtension(path)

            If String.IsNullOrEmpty(name) Then
                Return "MSA"
            Else
                Return name
            End If
        End Function
    End Module
End Namespace
