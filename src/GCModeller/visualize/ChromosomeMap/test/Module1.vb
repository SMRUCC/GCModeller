#Region "Microsoft.VisualBasic::6807cf3061bca7d401ac8d26eabed665, GCModeller\visualize\ChromosomeMap\test\Module1.vb"

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

    '   Total Lines: 38
    '    Code Lines: 35
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 1.27 KB


    ' Module Module1
    ' 
    '     Function: TestDEBUG
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels
Imports SMRUCC.genomics.Visualize.ChromosomeMap.PlasmidMap

Module Module1

    Sub Main()
        Call TestDEBUG()
    End Sub

    <ExportAPI("test_debug()", Info:="Just for debugging...")>
    Public Function TestDEBUG() As Boolean
        Dim model As New PlasmidMapDrawingModel With {
            .GeneObjects = {
                New SegmentObject With {
                    .LocusTag = "TEST_1",
                    .Direction = 0,
                    .CommonName = "TEST_Annotations_TEXT",
                    .Left = 500,
                    .Right = 600,
                    .Color = Brushes.Black
                },
                New SegmentObject With {
                    .LocusTag = "TEST_1",
                    .Direction = 0,
                    .CommonName = "TEST_Annotations_TEXT",
                    .Left = 1000,
                    .Right = 1300,
                    .Color = Brushes.Black
                }
            },
            .genomeSize = 1600
        }
        Call DrawingDevice.DrawMap(model).Save("./Test.png")
        Return True
    End Function
End Module
