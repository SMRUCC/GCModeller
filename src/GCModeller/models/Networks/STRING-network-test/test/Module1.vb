#Region "Microsoft.VisualBasic::fdc94e5bc0990e9d166e666372a84984, ..\GCModeller\models\Networks\STRING-network-test\test\Module1.vb"

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

Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.Model.Network.STRING
Imports Microsoft.VisualBasic.Data.visualize.Network

Module Module1

    Sub Main()
        Dim edges = InteractExports.ImportsTsv("D:\GCModeller\src\GCModeller\models\Networks\STRING-network-test\string_interactions.tsv")
        Dim nodes = "D:\GCModeller\src\GCModeller\models\Networks\STRING-network-test\string_network_coordinates.txt".LoadTsv(Of Coordinates)

        Call GraphModel _
            .CreateGraph(edges, nodes) _
            .DrawImage(canvasSize:="2000,2000", radiusScale:=6, hideDisconnectedNode:=True, fontSizeFactor:=3, minRadius:=10) _
            .Save("D:\GCModeller\src\GCModeller\models\Networks\STRING-network-test\string_interactions.png")

        Pause()
    End Sub
End Module

