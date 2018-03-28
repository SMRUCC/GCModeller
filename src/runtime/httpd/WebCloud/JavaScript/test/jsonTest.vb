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

        Pause()
    End Sub
End Module
