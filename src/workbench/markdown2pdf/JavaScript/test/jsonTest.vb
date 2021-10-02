#Region "Microsoft.VisualBasic::87992c8fab25a91a6b824062c33ad489, markdown2pdf\JavaScript\test\jsonTest.vb"

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

    ' Module jsonTest
    ' 
    '     Sub: lineTest, Main, previewTest
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.WebCloud.JavaScript.highcharts

Module jsonTest

    Sub Main()
        Call lineTest()

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

    Sub lineTest()
        Dim ranges#()() = {
        {1246406400000, 14.3, 27.7},
        {1246492800000, 14.5, 27.8},
        {1246579200000, 15.5, 29.6},
        {1246665600000, 16.7, 30.7},
        {1246752000000, 16.5, 25.0},
        {1246838400000, 17.8, 25.7},
        {1246924800000, 13.5, 24.8},
        {1247011200000, 10.5, 21.4},
        {1247097600000, 9.2, 23.8},
        {1247184000000, 11.6, 21.8},
        {1247270400000, 10.7, 23.7},
        {1247356800000, 11.0, 23.3},
        {1247443200000, 11.6, 23.7},
        {1247529600000, 11.8, 20.7},
        {1247616000000, 12.6, 22.4},
        {1247702400000, 13.6, 19.6},
        {1247788800000, 11.4, 22.6},
        {1247875200000, 13.2, 25.0},
        {1247961600000, 14.2, 21.6},
        {1248048000000, 13.1, 17.1},
        {1248134400000, 12.2, 15.5},
        {1248220800000, 12.0, 20.8},
        {1248307200000, 12.0, 17.1},
        {1248393600000, 12.7, 18.3},
        {1248480000000, 12.4, 19.4},
        {1248566400000, 12.6, 19.9},
        {1248652800000, 11.9, 20.2},
        {1248739200000, 11.0, 19.3},
        {1248825600000, 10.8, 17.8},
        {1248912000000, 11.8, 18.5},
        {1248998400000, 10.8, 16.1}
    }.RowIterator.ToArray,
    averages = {
        {1246406400000, 21.5},
        {1246492800000, 22.1},
        {1246579200000, 23},
        {1246665600000, 23.8},
        {1246752000000, 21.4},
        {1246838400000, 21.3},
        {1246924800000, 18.3},
        {1247011200000, 15.4},
        {1247097600000, 16.4},
        {1247184000000, 17.7},
        {1247270400000, 17.5},
        {1247356800000, 17.6},
        {1247443200000, 17.7},
        {1247529600000, 16.8},
        {1247616000000, 17.7},
        {1247702400000, 16.3},
        {1247788800000, 17.8},
        {1247875200000, 18.1},
        {1247961600000, 17.2},
        {1248048000000, 14.4},
        {1248134400000, 13.7},
        {1248220800000, 15.7},
        {1248307200000, 14.6},
        {1248393600000, 15.3},
        {1248480000000, 15.3},
        {1248566400000, 15.8},
        {1248652800000, 15.2},
        {1248739200000, 14.8},
        {1248825600000, 14.4},
        {1248912000000, 15},
        {1248998400000, 13.6}
    }.RowIterator.ToArray

        Dim chart As New LineChart.LineWithRangeChart With {.title = "23323", .xAxis = New Axis With {.type = "datetime"},
            .tooltip = New tooltip With {.crosshairs = True, .shared = True, .valueSuffix = "C"},
            .legend = New legendOptions With {.enabled = False},
            .series = {New LineChart.LineRangeSerial With {
                .name = "T",
                .data = averages,
                .zIndex = 1,
                .marker = New ScatterChart.markerOptions With {
                .fillColor = "white",
                .lineWidth = 2,
                .lineColor = "darkblue"
                }
            }, New LineChart.LineRangeSerial With {.name = "range", .data = ranges,
            .type = "arearange", .lineWidth = 0.5, .linkedTo = ":previous",
            .color = "lightblue", .fillOpacity = 0.5, .zIndex = 0, .marker = New ScatterChart.markerOptions With {.enabled = False}}}}


        Call DirectCast(chart, IVisualStudioPreviews).Previews.SaveTo("C:\Users\administrator\Desktop\VSD2.html")

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
            New GenericDataSerial With {.type = "area", .name = "Area", .data = {1, 8, 2, 7, 3, 6, 4, 5}}              '  New GenericDataSerial With {.type = "column", .name = "Column", .data = {8, 7, 6, 5, 4, 3, 2, 1}, .pointPlacement = "between"},              '  New GenericDataSerial With {.type = "line", .name = "Line", .data = {1, 2, 3, 4, 5, 6, 7, 8}},
                            }
        }


        Call DirectCast(polar, IVisualStudioPreviews).Previews.SaveTo("C:\Users\administrator\Desktop\VSD1.html")


        Pause()
    End Sub
End Module
