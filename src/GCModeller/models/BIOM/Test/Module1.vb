Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Serialization.JSON

Module Module1

    Const JsonLongTime$ = "\d+-\d+-\d+T\d+:\d+:\d+\.\d+"

    Sub Main()

        '2016-10-31T17:30:49.768484

        Dim ddd = Regex.Match("2016-10-31T17:30:49.768484", JsonLongTime).Value
        Dim json = Date.Parse(ddd).GetJson

        Dim biom = SMRUCC.genomics.foundation.BIOM.v10.Json(Of Double).LoadFile("C:\Users\xieguigang\Desktop\predictions_ko.L3.biom.json")

        Call biom.GetJson(indent:=True).SaveTo("C:\Users\xieguigang\Desktop\predictions_ko.L3.biom.formatted.json")

        Pause()
    End Sub

    Sub BoxPlot()

        '

    End Sub

End Module

Public Class DDDD


    Public Property d As Date

End Class