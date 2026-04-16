#Region "Microsoft.VisualBasic::a80195529d51bfa86ac4a438b39e0024, visualize\ChromosomeMap\DrawingModels\LabelPadding.vb"

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


    ' Code Statistics:

    '   Total Lines: 64
    '    Code Lines: 34 (53.12%)
    ' Comment Lines: 20 (31.25%)
    '    - Xml Docs: 90.00%
    ' 
    '   Blank Lines: 10 (15.62%)
    '     File Size: 2.71 KB


    '     Delegate Function
    ' 
    ' 
    '     Module LabelPaddingExtensions
    ' 
    '         Function: checkRightEndTrimmed, LeftAligned, MiddleAlignment, RightAlignment
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing

Namespace DrawingModels

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="segnmentLength"></param>
    ''' <param name="headLength"></param>
    ''' <param name="textLength"></param>
    ''' <param name="p"></param>
    ''' <returns>返回字符串的位置信息</returns>
    ''' <remarks></remarks>
    Public Delegate Function TextPadding(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As PointF) As PointF

    Module LabelPaddingExtensions

        ''' <summary>
        ''' 主要是标签在水平方向上的偏移位置的计算方法
        ''' </summary>
        Public ReadOnly TextAlignments As New Dictionary(Of String, TextPadding) From {
            {"left", AddressOf LeftAligned},
            {"middle", AddressOf MiddleAlignment},
            {"right", AddressOf RightAlignment}
        }

        Public ReadOnly defaultPadding As TextPadding = AddressOf MiddleAlignment

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="segnmentLength">基因对象的图形的绘制长度</param>
        ''' <param name="textLength">使用MeasureString获取得到的字符串的绘制长度</param>
        ''' <param name="p">基因对象额绘制坐标</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function LeftAligned(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As PointF) As PointF
            Return p
        End Function

        Private Function RightAlignment(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As PointF) As PointF
            p = New PointF(p.X + segnmentLength - textLength, p.Y)
            p = checkRightEndTrimmed(p, textLength, rightEnds)
            Return p
        End Function

        Public Function checkRightEndTrimmed(p As PointF, textLength As Integer, rightEnds As Integer) As PointF
            If p.X + textLength > rightEnds Then
                Dim d = p.X + textLength - rightEnds
                d = p.X - d
                p = New PointF(d, p.Y)
            End If

            Return p
        End Function

        Private Function MiddleAlignment(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As PointF) As PointF
            Dim d As Integer = (segnmentLength - textLength) / 2
            p = New PointF(d + p.X - headLength, p.Y)
            p = checkRightEndTrimmed(p, textLength, rightEnds)
            Return p
        End Function
    End Module
End Namespace
