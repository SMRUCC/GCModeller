#Region "Microsoft.VisualBasic::760f45e1c7ab6fa54642b889d7c70ce5, GCModeller\data\GO_gene-ontology\obographs\obographs\EnrichmentVisualize.vb"

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

    '   Total Lines: 29
    '    Code Lines: 25
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 1.09 KB


    ' Module EnrichmentVisualize
    ' 
    '     Function: DrawGraph
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver

Public Module EnrichmentVisualize

    Public Function DrawGraph(dag As NetworkGraph,
                              Optional size$ = "8000,6000",
                              Optional bg$ = "white",
                              Optional margin$ = g.DefaultPadding,
                              Optional networkLayoutIteration% = 100) As GraphicsData

        If networkLayoutIteration > 0 Then
            Call VBDebugger.WaitOutput()
            Call Console.Clear()
            Call dag.doForceLayout(iterations:=networkLayoutIteration, showProgress:=True)
            Call Console.Clear()
        End If

        Return dag.DrawImage(
            canvasSize:=size,
            padding:=margin,
            background:=bg,
            labelerIterations:=-1
        )
    End Function
End Module
