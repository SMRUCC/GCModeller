#Region "Microsoft.VisualBasic::4d44ca4afa012c3dcf16055e59fcb4d3, visualize\Cytoscape\Cytoscape\test\Module1.vb"

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


    ' Code Statistics:

    '   Total Lines: 95
    '    Code Lines: 51 (53.68%)
    ' Comment Lines: 8 (8.42%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 36 (37.89%)
    '     File Size: 2.91 KB


    ' Class vTable
    ' 
    '     Properties: AA
    ' 
    ' Module Module1
    ' 
    '     Function: Main
    ' 
    '     Sub: xgmmlLoaderTest
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml
Imports SMRUCC.genomics.Visualize.Cytoscape.API
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML

Public Class vTable : Inherits Dictionary(Of String, String)

    Public Property AA As Integer = 33333

End Class

Module Module1

    Private Sub xgmmlLoaderTest()
        Dim g = XGMML.RDFXml.Load("E:\GCModeller\src\interops\visualize\Cytoscape\data\demo.xgmml")
        Dim net As NetworkGraph = g.CreateGraph


        Dim stiffness = 81.76F
        Dim repulsion = 40000.0F
        Dim damping = 0.5F


        ' Dim m_fdgPhysics = New ForceDirected2D(net, '// instance of Graph
        '  stiffness, '// stiffness of the spring
        'repulsion, '/'/ Node repulsion rate 
        'damp'ing    '// damping rate  
        '                              )

        '  Call m_fdgPhysics.Updates(net)

        Call net.ApplyAnalysis

        '  Dim ress = net.DrawImage("1024,768", displayId:=False)

        '   Call ress.Save("x:\gggg.bmp")


        Pause()
    End Sub


    Function Main() As Integer

        Call xgmmlLoaderTest()

        Dim styles = Cyjs.style.JSON.Load("G:\GCModeller\src\interops\visualize\Cytoscape\data\metabolome-network.json")
        Dim asss = styles.Values.First

        Dim seleeee = asss.style.First.GetStyle
        Dim ssssss = asss.style(3).MySelector

        Pause()

        Dim ddd As New vTable With {.AA = 234234234}

        ddd.Add("123", "ffff")
        ddd.Add("6666", "fdasdasd")

        Call ddd.GetJson.debug
        Pause()
        '     Dim mm As New GraphAttribute With {.RDF = New InnerRDF With {.meta = New NetworkMetadata}, .Name = RandomDouble()}
        '  Dim gf As New Graph With {.Attributes = {mm, New GraphAttribute With {.Name = Now.ToString}}}
        '   Call gf.SaveAsXml("x:\11223.xml")

        '  Dim fdsfs = Graph.Load("x:\11223.xml")



        ' Dim m_fdgRenderer As New IRenderer(m_fdgPhysics)

        '  Dim timeStep = 0.05F
        '   m_fdgRenderer.Draw(timeStep)


        '   Call g.Save("x:\gggggg\ddddd.xml")

        Dim xml As XmlDoc = XmlDoc.FromXmlFile("F:\GCModeller\GCI Project\DataVisualization\Cytoscape\test.cytoscape.xgmml")
        xml.xmlns.xsd = "22333333"
        xml.xmlns.xsi = "@#"
        xml.xmlns.Set("ggy", "oK!")
        Call xml.Save("x:\dddd.xml", Encodings.UTF8)


        Return 0
    End Function


End Module
