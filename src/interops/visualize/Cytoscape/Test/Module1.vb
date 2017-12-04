#Region "Microsoft.VisualBasic::f893792e8caa3fd5216fa55019350dd8, ..\interops\visualize\Cytoscape\Test\Module1.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.Interfaces
Imports Microsoft.VisualBasic.Data.visualize.Network.Visualize
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Visualize.Cytoscape.API
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML

Public Class vTable : Inherits Dictionary(Of String, String)

    Public Property AA As Integer = 33333

End Class

Module Module1



    Function Main() As Integer

        Dim styles = Cyjs.style.JSON.Load("G:\GCModeller\src\interops\visualize\Cytoscape\data\metabolome-network.json")
        Dim asss = styles.Values.First

        Dim seleeee = asss.style.First.GetStyle
        Dim ssssss = asss.style(3).MySelector

        Pause()

        Dim ddd As New vTable With {.AA = 234234234}

        ddd.Add("123", "ffff")
        ddd.Add("6666", "fdasdasd")

        Call ddd.GetJson.__DEBUG_ECHO
        Pause()
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

        Dim ress = net.DrawImage("1024,768", displayId:=False)

        Call ress.Save("x:\gggg.bmp")

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
