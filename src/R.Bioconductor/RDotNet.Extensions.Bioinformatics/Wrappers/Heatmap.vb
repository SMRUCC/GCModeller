Imports System.Text
Imports System.IO
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.Tokenizer
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic
Imports RDotNet.Extensions.VisualBasic
Imports RDotNet.Extensions.VisualBasic.utils.read.table
Imports RDotNet.Extensions.VisualBasic.stats
Imports RDotNet.Extensions.VisualBasic.Graphics
Imports RDotNet.Extensions.VisualBasic.grDevices

Public Class Heatmap : Inherits IRScript

    Const df As String = "df"

    ''' <summary>
    ''' Column name of the row factor in the csv file that represents the row name. Default is the first column.
    ''' </summary>
    ''' <returns></returns>
    Public Property rowNameMaps As String
    ''' <summary>
    ''' Csv文件的文件路径
    ''' </summary>
    ''' <returns></returns>
    Public Property dataset As readcsv
    Public Property heatmap As heatmap_plot
    ''' <summary>
    ''' tiff文件的输出路径
    ''' </summary>
    ''' <returns></returns>
    Public Property image As grDevices.grImage

    Private Function __getRowNames() As String
        Dim col As String = rowNameMaps
        Dim lines As String() = dataset.file.ReadAllLines
        Dim firstLine As List(Of String) = CharsParser(lines(0))

        If String.IsNullOrEmpty(rowNameMaps) Then   ' 默认使用第一列，作为rows的名称
            col = firstLine.FirstOrDefault
        End If

        _locusId = lines.Skip(1).ToArray(Function(x) x.Split(","c).First)
        _samples = firstLine.Skip(1).ToArray

        Return col
    End Function

    Dim __output As String()

    ''' <summary>
    ''' 在执行完了脚本之后调用本方法才能够得到结果，否则返回空值
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property output As String()
        Get
            If __output.IsNullOrEmpty Then
                Try
                    __output = RServer.WriteLine("result")
                Catch ex As Exception
                    Return Nothing
                End Try
            End If
            Return __output
        End Get
    End Property

    ''' <summary>
    ''' 第一列所表示的基因号
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property locusId As String()
    ''' <summary>
    ''' 第一行所表示的样本编号
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property samples As String()

    Sub New()
        Requires = {"RColorBrewer"}
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' http://joseph.yy.blog.163.com/blog/static/50973959201285102114376/
    ''' </remarks>
    Protected Overrides Function __R_script() As String
        Dim script As StringBuilder = New StringBuilder()
        Call script.AppendLine($"{df} <- " & dataset)
        Call script.AppendLine($"row.names({df}) <- {df}${__getRowNames()}")
        Call script.AppendLine($"{df}<-{df}[,-1]")
        Call script.AppendLine("df <- data.matrix(df)")

        heatmap.x = df

        If Not heatmap.Requires Is Nothing Then
            For Each ns As String In heatmap.Requires
                Call script.AppendLine(RScripts.library(ns))
            Next
        End If

        Call script.AppendLine(image.Plot("result <- " & heatmap))

        Return script.ToString
    End Function
End Class
