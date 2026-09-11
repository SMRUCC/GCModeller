#Region "Microsoft.VisualBasic::605a3ac968da593584b343b14cdc8936, visualize\Circos\Circos\Tools.vb"

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

    ' Module Tools
    ' 
    '     Properties: currentDIR
    ' 
    '     Function: RGBExpression, (+2 Overloads) TrimPath
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel

Module Tools

    ''' <summary>
    ''' 取得当前的进程工作目录（每次调用时实时求值，不做缓存）
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 早期版本这里是一个模块级的 ``ReadOnly Property`` 字段，其值在类型初始化的那一刻就被固化，
    ''' 当在同一个进程之中先后向不同的文件夹输出多份 circos 文档时会导致路径裁剪错乱，
    ''' 所以在这里改为每次调用时实时读取。
    ''' </remarks>
    Public ReadOnly Property currentDIR As String
        Get
            Return FileIO.FileSystem.CurrentDirectory.Replace("\", "/").TrimEnd("/"c) & "/"
        End Get
    End Property

    ''' <summary>
    ''' 尝试创建相对路径
    ''' </summary>
    ''' <param name="doc"></param>
    ''' <returns></returns>
    Public Function TrimPath(doc As CircosConfig) As String
        If TypeOf doc Is CircosDistributed Then
            Return doc.FilePath
        End If

        Dim url As String = doc.FilePath
        Return TrimPath(url)
    End Function

    ''' <summary>
    ''' 如果给定的路径是<paramref name="baseDIR"/>之下的一个绝对路径，则将其裁剪为相对于<paramref name="baseDIR"/>的相对路径
    ''' </summary>
    ''' <param name="url">可以被转换为<code>/</code>风格的路径分隔符的文件路径</param>
    ''' <param name="baseDIR">
    ''' 参考的基准文件夹路径，如果这个参数为空值的话，则会使用当前的进程工作目录<see cref="currentDIR"/>作为基准
    ''' </param>
    ''' <returns></returns>
    Public Function TrimPath(url As String, Optional baseDIR As String = Nothing) As String
        If String.IsNullOrEmpty(url) Then
            Return url
        End If

        Dim refPath As String = url.Replace("\", "/")
        Dim root As String = If(
            String.IsNullOrEmpty(baseDIR),
            currentDIR,
            baseDIR.Replace("\", "/").TrimEnd("/"c) & "/"
        )

        If refPath.StartsWith(root, StringComparison.OrdinalIgnoreCase) Then
            refPath = refPath.Substring(root.Length)
        End If

        Return refPath
    End Function

    ''' <summary>
    ''' 使用这个函数才能够正确的生成RGB颜色在circos之中的``fill_color``值
    ''' </summary>
    ''' <param name="c"></param>
    ''' <returns></returns>
    <Extension>
    Public Function RGBExpression(c As Color) As String
        Return $"({c.R},{c.G},{c.B})"
    End Function
End Module
