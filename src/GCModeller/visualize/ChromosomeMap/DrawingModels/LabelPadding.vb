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
    Public Delegate Function TextPadding(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As Point) As Point

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
        Private Function LeftAligned(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As Point) As Point
            Return p
        End Function

        Private Function RightAlignment(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As Point) As Point
            p = New Point(p.X + segnmentLength - textLength, p.Y)
            p = checkRightEndTrimmed(p, textLength, rightEnds)
            Return p
        End Function

        Public Function checkRightEndTrimmed(p As Point, textLength As Integer, rightEnds As Integer) As Point
            If p.X + textLength > rightEnds Then
                Dim d = p.X + textLength - rightEnds
                d = p.X - d
                p = New Point(d, p.Y)
            End If

            Return p
        End Function

        Private Function MiddleAlignment(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As Point) As Point
            Dim d As Integer = (segnmentLength - textLength) / 2
            p = New Point(d + p.X - headLength, p.Y)
            p = checkRightEndTrimmed(p, textLength, rightEnds)
            Return p
        End Function
    End Module
End Namespace