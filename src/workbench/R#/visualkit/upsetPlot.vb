
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.CollectionSet
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("upsetPlot")>
Module upsetPlot

    <RInitialize>
    Sub Main()
        Call Internal.generic.add("plot", GetType(IntersectionData), AddressOf plotVennSet)
    End Sub

    Private Function plotVennSet(vennSet As IntersectionData, args As list, env As Environment) As Object
        Dim theme As New Theme With {
            .padding = InteropArgumentHelper.getPadding(args!padding, "padding:150px 50px 200px 2000px;"),
            .XaxisTickFormat = "F0",
            .YaxisTickFormat = "F0",
            .axisStroke = "stroke: black; stroke-width: 10px; stroke-dash: solid;",
            .axisLabelCSS = "font-style: normal; font-size: 20; font-family: " & FontFace.MicrosoftYaHei & ";",
            .colorSet = args.getValue("colors", env, "Paper"),
            .legendLabelCSS = "font-style: normal; font-size: 20; font-family: " & FontFace.MicrosoftYaHei & ";"
        }
        Dim classSet As Dictionary(Of String, String()) = args.getValue(Of Dictionary(Of String, String()))("class", env, Nothing)
        Dim upsetBar As String = RColorPalette.getColor(args!upsetBar, "gray")
        Dim setSizeBar As String = RColorPalette.getColor(args!setSizeBar, "gray")
        Dim desc As Boolean = args.getValue("desc", env, False)
        Dim size As String = InteropArgumentHelper.getSize(args!size, env, "8000,4000")
        Dim app As New IntersectionPlot(vennSet, desc, setSizeBar, classSet, theme)

        Return app.Plot(size)
    End Function
End Module
