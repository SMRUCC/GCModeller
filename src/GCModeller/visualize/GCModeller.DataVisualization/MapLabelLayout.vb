Imports System.Drawing

Public Structure MapLabelLayout

    Dim ConflictRegion As Rectangle

    ''' <summary>
    ''' 
    ''' </summary>
    Dim baseY!

    Sub New(text$, font As Font, g As Graphics, location As Point, Optional baseY! = 0!)
        With Me
            .baseY = baseY
            .ConflictRegion = New Rectangle With {
                .Location = location,
                .Size = g.MeasureString(text, font).ToSize
            }
        End With
    End Sub

    ''' <summary>
    ''' 当标签是从左到右排列的时候的layout位置的变换
    ''' </summary>
    ''' <param name="rect"></param>
    ''' <returns></returns>
    Public Function ForceNextLocation(rect As Rectangle) As Rectangle
        Dim ptr As Point = rect.Location
        Dim size As Size = rect.Size

        ' 有冲突
        If ConflictRegion.Right >= rect.Left Then
            Dim yconflicts As Boolean

            With ConflictRegion
                yconflicts = rect.Top.InRange(.Top, .Bottom) OrElse rect.Bottom.InRange(.Top, .Bottom)
            End With

            If yconflicts Then
                rect = New Rectangle With {
                    .Location = New Point(ptr.X, ConflictRegion.Top - size.Height),
                    .Size = New Size(size.Width, size.Height)
                }
            End If
        End If

        Return rect
    End Function
End Structure
