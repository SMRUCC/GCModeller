Imports System.Drawing
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.API
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView.XGMML
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DataVisualization.Network
Imports Microsoft.VisualBasic.DataVisualization.Network.Graph
Imports Microsoft.VisualBasic.DataVisualization.Network.Layouts
Imports Microsoft.VisualBasic.DataVisualization.Network.Layouts.Interfaces
Imports Microsoft.VisualBasic.Imaging

Module Module1

    Function Main() As Integer

        '     Dim mm As New GraphAttribute With {.RDF = New InnerRDF With {.meta = New NetworkMetadata}, .Name = RandomDouble()}
        '  Dim gf As New Graph With {.Attributes = {mm, New GraphAttribute With {.Name = Now.ToString}}}
        '   Call gf.SaveAsXml("x:\11223.xml")

        '  Dim fdsfs = Graph.Load("x:\11223.xml")

        Dim g = Graph.Load("F:\GCModeller\GCI Project\DataVisualization\Cytoscape\test.cytoscape.xgmml")
        Dim net As NetworkGraph = g.CreateGraph


        Dim stiffness = 81.76F
        Dim repulsion = 40000.0F
        Dim damping = 0.5F


        Dim m_fdgPhysics = New ForceDirected2D(net, '// instance of Graph
        stiffness, '// stiffness of the spring
                                                   repulsion, '// Node repulsion rate 
                                                   damping    '// damping rate  
                                                   )

        Call m_fdgPhysics.Updates(net)

        Call net.ApplyAnalysis

        Dim ress As Image = net.DrawImage(New Size(1024, 768), displayId:=False)

        Call ress.SaveAs("x:\gggg.bmp",)

        ' Dim m_fdgRenderer As New IRenderer(m_fdgPhysics)

        '  Dim timeStep = 0.05F
        '   m_fdgRenderer.Draw(timeStep)


        '   Call g.Save("x:\gggggg\ddddd.xml")

        Dim xml As Text.Xml.XmlDoc = Text.Xml.XmlDoc.FromXmlFile("F:\GCModeller\GCI Project\DataVisualization\Cytoscape\test.cytoscape.xgmml")
        xml.xmlns.xsd = "22333333"
        xml.xmlns.xsi = "@#"
        xml.xmlns.Set("ggy", "oK!")
        Call xml.Save("x:\dddd.xml", Encodings.UTF8)
    End Function


End Module
