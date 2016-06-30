Imports System.Drawing
Imports d3svg

Module Program

    Sub Main()
        Dim dev As New d3svg.DrawSVG("F:\GCModeller.Workbench\IDE_PlugIns\d3jsViewer")


        Dim parser As D3Parser = New ForceDirectedGraph
        Call parser.HtmlFileParser("F:\GCModeller.Workbench\d3js\d3svg\data\NPashaP.htm").Build.SaveTo("F:\GCModeller.Workbench\d3js\d3svg\data\NPashaP.svg")
        Call parser.HtmlFileParser("F:\GCModeller.Workbench\d3js\d3svg\data\NPashaP.htm").BuildModel.SaveAsXml("F:\GCModeller.Workbench\d3js\d3svg\data\NPashaP2.svg")

        Dim svg As SVG = parser.HtmlFileParser("F:\GCModeller.Workbench\d3js\d3svg\data\Hierarchical-Edge-Bundling.htm")
        '   svg = parser.HtmlParser("http://127.0.0.1".GET)
        Call svg.Build.SaveTo("./test.svg")
        Dim model = svg.BuildModel
        Call model.SaveAsXml("F:\GCModeller.Workbench\d3js\d3svg\data\Hierarchical-Edge-Bundling.svg")


        Call dev.RasterizeSvg("F:\GCModeller.Workbench\d3js\d3svg\data\Hierarchical-Edge-Bundling.svg",
                              "F:\GCModeller.Workbench\d3js\d3svg\data\Hierarchical-Edge-Bundling.png",
                              New Size(2000, 2000))
        Call dev.RasterizeSvg("F:\GCModeller.Workbench\d3js\d3svg\data\example_svg2.svg",
                              "F:\GCModeller.Workbench\d3js\d3svg\data\example_svg2.png")
        Call dev.RasterizeSvg("F:\GCModeller.Workbench\d3js\d3svg\data\NPashaP.svg",
                              "F:\GCModeller.Workbench\d3js\d3svg\data\NPashaP.svg.png")
        Call dev.RasterizeSvg("F:\GCModeller.Workbench\d3js\d3svg\data\example_svg.svg",
                              "F:\GCModeller.Workbench\d3js\d3svg\data\example_svg.png", format:=Microsoft.VisualBasic.Imaging.ImageFormats.Jpeg)
    End Sub
End Module
