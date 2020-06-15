Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas

Namespace Graphic

    Public MustInherit Class Plot

        ''' <summary>
        ''' the main title
        ''' </summary>
        ''' <returns></returns>
        Public Property main As String = Me.GetType.Name
        ''' <summary>
        ''' the sub-title
        ''' </summary>
        ''' <returns></returns>
        Public Property subtitle As String
        Public Property legendTitle As String

        Public Property mainStyle As FontStyle
        Public Property subTitleStyle As FontStyle
        Public Property legendTitleStyle As FontStyle

        Sub New(theme As Theme)

        End Sub

    End Class
End Namespace