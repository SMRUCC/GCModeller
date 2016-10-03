#Region "Microsoft.VisualBasic::85c29feff5322fd1a2eb6a9ca2b35bba, ..\workbench\GenomicsBrowser\GenomeBrowser\VectorScriptProject.vb"

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


''' <summary>
''' 根据gff文档的描述生成VectorScript，然后再根据VectorScript生成图形数据，保存文件的时候是保存VectorScript的
''' </summary>
Module VectorScriptProject

    Public Function Gff2VectorScript(Gff As LANS.SystemsBiology.Assembly.NCBI.GenBank.PttGenomeBrief.GenomeFeature.GFF) As Microsoft.VisualBasic.Drawing.Drawing2D.Vectogram

        Dim VectorDiagram As New Microsoft.VisualBasic.Drawing.Drawing2D.Vectogram(New Size(1024, 768))
        Dim PreRight As Long
        Dim Level As Integer = 0
        Dim Factor As Double = 0.025

        For Each Feature In Gff.GenomeFeatures
            Dim Diagram = Feature2VectorElements(Feature, VectorDiagram, Factor)
            If Diagram.Left < PreRight Then
                Level += 1
                Diagram.Location = New Point(Diagram.Location.X, Diagram.Location.Y + Diagram.BodySize.Height * Level)
            Else
                Level = 0
            End If

            PreRight = Diagram.Right

            Call VectorDiagram.AddDrawingElement(Diagram)
        Next

        Return VectorDiagram
    End Function

    Public Function Feature2VectorElements(Feature As LANS.SystemsBiology.Assembly.NCBI.GenBank.PttGenomeBrief.GenomeFeature.Feature,
                                           VectorDiagram As Microsoft.VisualBasic.Drawing.Drawing2D.Vectogram, Factor As Double) _
        As Microsoft.VisualBasic.Drawing.Drawing2D.VectorElements.Arrow

        Dim Arrow As New Microsoft.VisualBasic.Drawing.Drawing2D.VectorElements.Arrow(
            Location:=New Point(Feature.BaseLocation.Start * Factor, 100),
            Size:=New Size(Feature.SegmentLength * Factor, 45),
            GDI:=VectorDiagram.GDIDevice,
            Color:=Color.DarkCyan) With {
                .DirectionLeft = Feature.Strand = LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation.Strands.Reverse,
                .BodyHeightPercentage = 0.8,
                .TooltipTag = Feature.GenerateDocumentLine,
                .HeadLengthPercentage = 0.25
        }

        Return Arrow
    End Function

End Module

