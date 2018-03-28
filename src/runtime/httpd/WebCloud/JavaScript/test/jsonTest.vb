Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.WebCloud.JavaScript.highcharts

Module jsonTest

    Sub Main()

        Call previewTest()


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

    Sub previewTest()

        Dim polar As New PolarChart.PolarChart With {
            .chart = chart.PolarChart,
            .title = "Highcharts Polar Chart",
            .pane = New PolarChart.paneOptions With {.startAngle = 0, .endAngle = 360},
            .xAxis = New Axis With {
                .tickInterval = 45, .min = 0, .max = 360,
                .labels = New labelOptions With {
                    .formatter = New Lambda With {.function = "return this.value + '°';"}
                }
            },
            .yAxis = New Axis With {.min = 0},
            .plotOptions = New plotOptions With {
                .series = New LineChart.lineOptions With {
                    .pointStart = 0, .pointInterval = 45
                },
                .column = New BarChart.columnOptions With {
                    .pointPadding = 0, .groupPadding = 0
                }
            },
            .series = {
                New GenericDataSerial With {.type = "column", .name = "Column", .data = {8, 7, 6, 5, 4, 3, 2, 1}, .pointPlacement = "between"},
                New GenericDataSerial With {.type = "line", .name = "Line", .data = {1, 2, 3, 4, 5, 6, 7, 8}},
                New GenericDataSerial With {.type = "area", .name = "Area", .data = {1, 8, 2, 7, 3, 6, 4, 5}}
            }
        }


        Call DirectCast(polar, IVisualStudioPreviews).Previews.SaveTo("C:\Users\administrator\Desktop\VSD1.html")


        Pause()
    End Sub
End Module
