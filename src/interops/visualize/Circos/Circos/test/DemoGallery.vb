Imports System.IO
Imports SMRUCC.genomics.Visualize.Circos
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.Lines
Imports SMRUCC.genomics.Visualize.Circos.GdiPlus
Imports SMRUCC.genomics.Visualize.Circos.Karyotype
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

''' <summary>
''' 使用虚构的数据演示重构之后的 circos 配置生成与绘图的所有场景
''' </summary>
Public Module DemoGallery

    ''' <summary>
    ''' 创建一个基础的 circos 文档对象
    ''' </summary>
    ''' <param name="outDIR">配置文件与数据文件的输出文件夹</param>
    ''' <param name="skeleton">基因组的骨架信息</param>
    ''' <returns></returns>
    Public Function BaseDocument(outDIR As String,
                                 Optional skeleton As KaryotypeSkeleton = Nothing,
                                 Optional showTicksLabel As Boolean = True) As Circos

        Dim circos As Circos = Circos.CreateObject()

        outDIR = Circos.NormalizeDirectory(outDIR)

        circos.skeletonKaryotype = skeleton
        circos.karyotype = "data/karyotype.txt"
        circos.chromosomes_units = "10000"
        circos.includes.Add(New IdeogramInclude(circos))
        circos.includes.Add(New TicksInclude(circos))

        circos.Ideogram.Ideogram.show_label = If(True, "yes", "no")
        circos.Ideogram.Ideogram.Spacing.default = "0.005r"
        circos.Ideogram.Ideogram.radius = "0.80r"
        circos.Ideogram.Ideogram.thickness = "25p"

        Call CircosAPI.ShowTicksLabel(circos, showTicksLabel)

        Return circos
    End Function

    ''' <summary>
    ''' 将所有的绘图元素重新排布在 <paramref name="rMax"/> ~ <paramref name="rMin"/> 的半径区间之内
    ''' </summary>
    Private Function layout(circos As Circos, rMax#, rMin#) As Circos
        Return CircosAPI.SetRadius(circos, rMax, rMin)
    End Function

#Region "文档构建"

    ''' <summary>
    ''' 场景 1: 只绘制基因组骨架(ideogram + ticks)
    ''' </summary>
    Public Function SkeletonDocument() As Circos
        Return BaseDocument("", DemoSyntheticData.Karyotype())
    End Function

    ''' <summary>
    ''' 场景 2: 全部的 2D 数据类型(scatter / line / histogram / heatmap / tile / text / connector)
    ''' </summary>
    Public Function AllTrackTypesDocument() As Circos
        Dim genome = DemoSyntheticData.Genome()
        Dim gc = DemoSyntheticData.GCContent(genome)
        Dim skew = DemoSyntheticData.GCSkew(genome)
        Dim genes = DemoSyntheticData.Genes()
        Dim labels = DemoSyntheticData.GeneLabels(genes)

        Dim circos As Circos = BaseDocument("", DemoSyntheticData.Karyotype())

        circos.AddTrack(New Histogram(New TrackDataDocument(Of ValueTrackData)(gc)), autoLayout:=False)
        circos.AddTrack(New Histogram(New TrackDataDocument(Of ValueTrackData)(skew)), autoLayout:=False)
        circos.AddTrack(New HeatMap(New TrackDataDocument(Of ValueTrackData)(gc)), autoLayout:=False)
        circos.AddTrack(New LinePlot(New TrackDataDocument(Of ValueTrackData)(skew)), autoLayout:=False)
        circos.AddTrack(New ScatterPlot(New TrackDataDocument(Of ValueTrackData)(
            DemoSyntheticData.Sample(skew))), autoLayout:=False)
        circos.AddTrack(New TilePlot(New TrackDataDocument(Of RegionTrackData)(genes)), autoLayout:=False)
        circos.AddTrack(New Connector(New ConnectorDocument(genes.Take(20).ToArray)), autoLayout:=False)
        circos.AddTrack(New TextLabel(New TrackDataDocument(Of TextTrackData)(labels)), autoLayout:=False)

        Call layout(circos, rMax:=0.76, rMin:=0.30)

        Return circos
    End Function

    ''' <summary>
    ''' 场景 3: 顶层的 ``&lt;links>`` 配置块
    ''' </summary>
    Public Function LinksDocument() As Circos
        Dim links = DemoSyntheticData.Links()
        Dim circos As Circos = BaseDocument("", DemoSyntheticData.Karyotype())

        circos.AddTrack(New LinkPlot(New TrackDataDocument(Of LinkData)(links)), autoLayout:=False)

        Dim link = DirectCast(circos.Plots(Scan0), LinkPlot)

        link.radius = "0.55r"
        link.bezier_radius = "0.15r"
        link.ribbon = "yes"
        link.color = "black_a3"
        link.thickness = "2"

        Return circos
    End Function

    ''' <summary>
    ''' 场景 4: 顶层的 ``&lt;highlights>`` 配置块
    ''' </summary>
    Public Function HighlightsDocument() As Circos
        Dim circos As Circos = BaseDocument("", DemoSyntheticData.Karyotype())
        Dim highlight As New Highlight(DemoSyntheticData.Highlights()) With {
            .IsTopLevelBlock = True,
            .fill_color = "red_a2"
        }

        circos.AddTrack(highlight, autoLayout:=False)

        Dim gc = DemoSyntheticData.GCContent(DemoSyntheticData.Genome())

        circos.AddTrack(New Histogram(New TrackDataDocument(Of ValueTrackData)(gc)), autoLayout:=False)

        Call layout(circos, rMax:=0.76, rMin:=0.45)

        Return circos
    End Function

    ''' <summary>
    ''' 场景 5: rules / axes / backgrounds 子块
    ''' </summary>
    Public Function RulesDocument() As Circos
        Dim skew = DemoSyntheticData.GCSkew(DemoSyntheticData.Genome())
        Dim circos As Circos = BaseDocument("", DemoSyntheticData.Karyotype())
        Dim histogram As New Histogram(New TrackDataDocument(Of ValueTrackData)(skew)) With {
            .fill_color = "vdgrey"
        }

        histogram.rules = New List(Of ConditionalRule) From {
            New ConditionalRule With {
                .condition = "var(value) > 0",
                .fill_color = "red"
            },
            New ConditionalRule With {
                .condition = "var(value) < 0",
                .fill_color = "blue"
            }
        }
        histogram.axes = New List(Of Axis) From {
            New Axis With {.spacing = "0.1r", .color = "lgrey_a2", .thickness = "1"}
        }
        histogram.backgrounds = New List(Of Background) From {
            New Background With {.y0 = "0.2", .y1 = "0.8", .color = "vvlgrey"}
        }

        circos.AddTrack(histogram, autoLayout:=False)

        Call layout(circos, rMax:=0.76, rMin:=0.5)

        Return circos
    End Function

    ''' <summary>
    ''' 场景 6: 文档之中所描述的完整工作流(``mchrTest.vb``)
    ''' </summary>
    Public Function WorkflowDocument(outDIR As String) As Circos
        Return run(outDIR)
    End Function

    Private Function ScenarioDocuments(root As String) As List(Of KeyValuePair(Of String, Func(Of Circos)))
        root = Circos.NormalizeDirectory(root)

        Return New List(Of KeyValuePair(Of String, Func(Of Circos))) From {
            New KeyValuePair(Of String, Func(Of Circos))("skeleton", AddressOf SkeletonDocument),
            New KeyValuePair(Of String, Func(Of Circos))("allTracks", AddressOf AllTrackTypesDocument),
            New KeyValuePair(Of String, Func(Of Circos))("links", AddressOf LinksDocument),
            New KeyValuePair(Of String, Func(Of Circos))("highlights", AddressOf HighlightsDocument),
            New KeyValuePair(Of String, Func(Of Circos))("rules", AddressOf RulesDocument),
            New KeyValuePair(Of String, Func(Of Circos))("mchr", Function() WorkflowDocument($"{root}/mchr"))
        }
    End Function

#End Region

#Region "circos 命令行渲染"

    ''' <summary>
    ''' 场景 1: 只绘制基因组骨架(ideogram + ticks)
    ''' </summary>
    Public Function SkeletonOnly(outDIR As String) As CircosRenderResult
        Return CircosRender.Render(SkeletonDocument(), outDIR, outputFile:="skeleton.png")
    End Function

    ''' <summary>
    ''' 场景 2: 全部的 2D 数据类型
    ''' </summary>
    Public Function AllTrackTypes(outDIR As String) As CircosRenderResult
        Return CircosRender.Render(AllTrackTypesDocument(), outDIR, outputFile:="allTracks.png")
    End Function

    ''' <summary>
    ''' 场景 3: 顶层的 ``&lt;links>`` 配置块
    ''' </summary>
    Public Function LinksBlockDemo(outDIR As String) As CircosRenderResult
        Return CircosRender.Render(LinksDocument(), outDIR, outputFile:="links.png")
    End Function

    ''' <summary>
    ''' 场景 4: 顶层的 ``&lt;highlights>`` 配置块
    ''' </summary>
    Public Function HighlightsBlockDemo(outDIR As String) As CircosRenderResult
        Return CircosRender.Render(HighlightsDocument(), outDIR, outputFile:="highlights.png")
    End Function

    ''' <summary>
    ''' 场景 5: rules / axes / backgrounds 子块
    ''' </summary>
    Public Function RulesAndAxes(outDIR As String) As CircosRenderResult
        Return CircosRender.Render(RulesDocument(), outDIR, outputFile:="rules.png")
    End Function

    ''' <summary>
    ''' 场景 6: 文档之中所描述的完整工作流(``mchrTest.vb``)
    ''' </summary>
    Public Function WorkflowDemo(outDIR As String) As CircosRenderResult
        Call run(outDIR)

        Return CircosRender.Render(
            confFile:=$"{Circos.NormalizeDirectory(outDIR)}/circos.conf",
            outputFile:="mchr.png",
            outputDir:=outDIR)
    End Function

    ''' <summary>
    ''' 依次渲染所有的演示场景（调用外部的 circos 程序）
    ''' </summary>
    Public Iterator Function RunAll(Optional root As String = "Z:\circos-test\") As IEnumerable(Of CircosRenderResult)
        For Each scenario In ScenarioDocuments(root)
            Dim outDIR As String = Circos.NormalizeDirectory($"{root}/{scenario.Key}")

            ' 注意：这里不删除上一次运行所遗留下来的文件夹，
            ' 否则当上一次的 circos 进程尚未完全退出的时候会出现文件夹被占用的异常
            Call Directory.CreateDirectory(outDIR)

            Console.WriteLine($"[{scenario.Key}] rendering...")

            Dim builder As Func(Of Circos) = scenario.Value
            Dim doc As Circos = builder()

            Yield CircosRender.Render(doc, outDIR, outputFile:=$"{scenario.Key}.png")
        Next
    End Function

#End Region

#Region "内置 GDI+ 引擎渲染"

    ''' <summary>
    ''' GDI+ 引擎的输出画布参数（与 circos 默认输出尺寸保持一致，方便对比）
    ''' </summary>
    Public Function GdiOptions() As GdiRenderOptions
        Return New GdiRenderOptions With {
            .Width = 1500,
            .Height = 1500,
            .Dpi = 100
        }
    End Function

    ''' <summary>
    ''' 依次使用内置的 GDI+ 引擎渲染所有的演示场景，输出 ``*.gdi.png`` 以便与官方 circos 的结果做对比
    ''' </summary>
    Public Iterator Function RunAllGdiPlus(Optional root As String = "Z:\circos-test\") As IEnumerable(Of CircosRenderResult)
        For Each scenario In ScenarioDocuments(root)
            Dim outDIR As String = Circos.NormalizeDirectory($"{root}/{scenario.Key}")

            Call Directory.CreateDirectory(outDIR)

            Console.WriteLine($"[{scenario.Key}] gdi+ rendering...")

            Dim builder As Func(Of Circos) = scenario.Value
            Dim doc As Circos = builder()

            Yield CircosRender.RenderGdiPlus(
                doc,
                outDIR,
                outputFile:=$"{scenario.Key}.gdi.png",
                options:=GdiOptions())
        Next
    End Function

#End Region

End Module
