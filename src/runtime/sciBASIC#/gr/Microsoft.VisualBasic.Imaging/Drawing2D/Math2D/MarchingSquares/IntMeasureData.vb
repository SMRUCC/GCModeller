Namespace Drawing2D.Math2D.MarchingSquares

    Friend Structure IntMeasureData
        Public Sub New(ByVal md As MeasureData, ByVal x_num As Integer, ByVal y_num As Integer)
            X = CInt(md.X * x_num)

            If X >= x_num Then
                X = x_num - 1
            End If

            Y = CInt(md.Y * y_num)

            If Y >= y_num Then
                Y = y_num - 1
            End If

            Z = md.Z
        End Sub

        Public X As Integer
        Public Y As Integer
        Public Z As Single
        ''' <summary>
        ''' 数据插值
        ''' </summary>
    End Structure

End Namespace