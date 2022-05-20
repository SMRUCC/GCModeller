
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.CollectionSet
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' UpSet plot provides an efficient way to 
''' visualize intersections of multiple sets 
''' compared to the traditional approaches, 
''' i.e. the Venn Diagram. 
''' </summary>
<Package("upsetPlot")>
Module upsetPlot

    <RInitialize>
    Sub Main()
        Call Internal.generic.add("plot", GetType(IntersectionData), AddressOf plotVennSet)
        Call Internal.generic.add("plot", GetType(UpsetData), AddressOf plotVennSet1)

        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(UpsetData), AddressOf getUpsetTable)
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(FactorGroup), AddressOf getUpsetTable)
    End Sub

    Private Function getUpsetTable(upset As Object, args As list, env As Environment) As dataframe
        Dim classSet As Dictionary(Of String, String()) = args.getValue(Of Dictionary(Of String, String()))("class", env, Nothing)

        If TypeOf upset Is UpsetData Then
            upset = DirectCast(upset, UpsetData).compares
        End If

        If classSet Is Nothing Then
            Return UpsetTableNoClass(upset)
        Else
            Return UpsetTableClass(upset, classSet)
        End If
    End Function

    Private Function UpsetTableClass(upset As FactorGroup, classSet As Dictionary(Of String, String())) As dataframe
        Dim compares As String() = upset.data _
            .Select(Function(a) a.name) _
            .ToArray
        Dim data As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = compares
        }
        Dim hist As New List(Of List(Of String))

        For Each row In upset.data
            Call hist.Add(New List(Of String))
        Next

        For Each classList In classSet
            Dim className As String = classList.Key
            Dim intersects = upset.data _
                .Select(Function(a, i)
                            Dim it = a _
                                .Intersect(classList.Value) _
                                .ToArray
                            hist(i).AddRange(it)
                            Return it
                        End Function) _
                .ToArray

            data.add($"count.{className}", intersects.Select(Function(i) i.Length))
            data.add($"id.{className}", intersects.Select(Function(i) i.JoinBy("; ")))
        Next

        ' add no class
        data.add($"count.no_class",
            upset.data.Select(Function(a, i)
                                  Dim index As Index(Of String) = hist(i).Indexing
                                  Dim notIncludes = a _
                                      .Where(Function(id) Not id Like index) _
                                      .ToArray

                                  Return notIncludes.Length
                              End Function))
        data.add($"id.no_class",
            upset.data.Select(Function(a, i)
                                  Dim index As Index(Of String) = hist(i).Indexing
                                  Dim notIncludes = a _
                                      .Where(Function(id) Not id Like index) _
                                      .ToArray

                                  Return notIncludes.JoinBy("; ")
                              End Function))

        Return data
    End Function

    Private Function UpsetTableNoClass(upset As FactorGroup) As dataframe
        Dim compares As String() = upset.data _
            .Select(Function(a) a.name) _
            .ToArray
        Dim data As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = compares
        }

        data.columns.Add("count", upset.data.Select(Function(a) a.Length))
        data.columns.Add("id", upset.data.Select(Function(a) a.JoinBy("; ")))

        Return data
    End Function

    Private Function plotVennSet(vennSet As IntersectionData, args As list, env As Environment) As Object
        Dim intersectionCut As Integer = args.getValue("intersection_cut", env, [default]:=0)
        Dim desc As Boolean = args.getValue("desc", env, False)
        Dim upset As UpsetData = UpsetData.CreateUpSetData(vennSet, intersectionCut, desc)

        Return plotVennSet1(upset, args, env)
    End Function

    Private Function plotVennSet1(upSet As UpsetData, args As list, env As Environment) As Object
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
        Dim size As String = InteropArgumentHelper.getSize(args!size, env, "8000,4000")
        Dim app As New IntersectionPlot(upSet, setSizeBar, classSet, theme)

        Return app.Plot(size)
    End Function

    <ExportAPI("as.upsetData")>
    Public Function CreateUpSetData(vennSet As IntersectionData,
                                    Optional desc As Boolean = True,
                                    Optional intersectionCut As Integer = 0) As UpsetData

        Return UpsetData.CreateUpSetData(vennSet, intersectionCut, desc)
    End Function

End Module
