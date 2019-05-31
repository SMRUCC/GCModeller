Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging

Namespace Colors

    Friend Class LevelMapGenerator

        Public values As Double()
        Public clSequence As Color()
        Public replaceBase As Boolean

        Dim highest As Color
        Dim offset As Integer

        Sub New(values As IEnumerable(Of Double), name As String, mapLevels As Integer, offsetPercentage#, replaceBase As Boolean)
            Dim maps As New ColorMap(mapLevels * 2)
            Me.clSequence = ColorSequence(maps, name).Reverse.ToArray
            Me.values = values.ToArray
            Me.replaceBase = replaceBase
            Me.highest = clSequence.Last
            Me.offset = CInt(clSequence.Length * offsetPercentage)
        End Sub

        Public Function CreateMaps(lv As Integer, index As Integer) As Mappings
            Dim value As Double = values(index)
            Dim Color As Color

            If lv <= 1 AndAlso replaceBase Then
                Color = Color.WhiteSmoke
            Else
                Dim idx As Integer = lv + offset

                If idx < clSequence.Length Then
                    Color = clSequence(idx)
                Else
                    Color = highest
                End If
            End If

            Return New Mappings With {
                .value = value,
                .level = lv,
                .color = Color
            }
        End Function
    End Class
End Namespace