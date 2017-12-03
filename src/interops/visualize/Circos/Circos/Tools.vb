#Region "Microsoft.VisualBasic::6f17813c692f960d2aa31068b0592c69, ..\interops\visualize\Circos\Circos\Tools.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Visualize.Circos.Configurations

Module Tools

    Public ReadOnly Property currentDIR As String =
        FileIO.FileSystem.CurrentDirectory.Replace("\", "/") & "/"

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

    Public Function TrimPath(url As String) As String
        Dim refPath As String = url.Replace("\", "/").Replace(currentDIR, "")
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
