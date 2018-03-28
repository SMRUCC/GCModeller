Imports SMRUCC.WebCloud.JavaScript.highcharts

Module jsonTest

    Sub Main()
        Dim label As New labelOptions With {
            .formatter = New Lambda With {.args = {"a", "b", "c"},
            .function = "return this.values + a + b + c;"}
        }

        Dim json = label.NewtonsoftJsonWriter


        Call json.__DEBUG_ECHO


        json = LambdaWriter.StripLambda(json)

        Call json.__DEBUG_ECHO



        Dim pie As New PieChart.PieChart With {
            .chart = chart.PieChart3D,
            .series = {
                New serial With {.data = {1, 2, 3, 4, 5, 6, 7, 0}},
                New serial With {.data = {"1", "2", "3", "4", "5", "6", "7", "0"}},
                 New serial With {.data = {True, False, True, False}}
            },
            .xAxis = New Axis With {.labels = label},
            .yAxis = New Axis With {.labels = label}
        }


        Call Javascript.WriteJavascript(Of serial)("this", pie).__DEBUG_ECHO

        Pause()
    End Sub
End Module
