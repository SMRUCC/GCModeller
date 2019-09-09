#Region "Microsoft.VisualBasic::f3cc120abea9718e0d93a3a45b9beeb0, RDotNet.Extensions.Bioinformatics\Wrappers\Heatmap.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Class Heatmap
    ' 
    '     Properties: dataset, heatmap, image, locusId, output
    '                 rowNameMaps, samples
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: __getRowNames, __R_script
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv.IO.Tokenizer
Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.packages.Graphics
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.packages.grDevices
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.packages.stats
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.packages.utils.read.table

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
    Public Property image As grImage

    Private Function __getRowNames() As String
        Dim col As String = rowNameMaps
        Dim lines As String() = dataset.file.ReadAllLines
        Dim firstLine As List(Of String) = CharsParser(lines(0))

        If String.IsNullOrEmpty(rowNameMaps) Then   ' 默认使用第一列，作为rows的名称
            col = firstLine.FirstOrDefault
        End If

        _locusId = lines.Skip(1).Select(Function(x) x.Split(","c).First).ToArray
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
                    __output = R.WriteLine("result")
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
