#Region "Microsoft.VisualBasic::7a0fdc0bd89968e7321b64c2a6800725, ..\GCModeller\visualize\visualizeTools\ChromosomeMap\DrawingModels\RenderingColor.vb"

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
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.ComponentModel

Namespace DrawingModels

    Public Module ModelColorExtensions

        <Extension>
        Public Function ApplyingCOGCategoryColor(Of MyvaCOG As ICOGDigest)(genes As MyvaCOG(), chromesome As ChromesomeDrawingModel) As ChromesomeDrawingModel
            Dim colorProfiles = RenderingColor.InitCOGColors(Nothing).ToDictionary(Function(obj) obj.Key,
                                                                                   Function(obj) CType(New SolidBrush(obj.Value), Brush))
            Dim DefaultCogColor As New SolidBrush(chromesome.DrawingConfigurations.NoneCogColor)
            Dim geneTable As Dictionary(Of MyvaCOG) = genes.ToDictionary

            For Each gene As SegmentObject In chromesome.GeneObjects
                Dim Cog As MyvaCOG = geneTable.TryGetValue(gene.LocusTag)
                If Not Cog Is Nothing Then
                    If Not String.IsNullOrEmpty(Cog.COG) Then
                        If Cog.COG.Length > 1 Then
                            gene.Color = New SolidBrush(NeutralizeColor((From c As Char In Cog.COG Select DirectCast(colorProfiles(c.ToString), SolidBrush).Color).ToArray))
                        Else
                            gene.Color = colorProfiles(Cog.COG)
                        End If
                    Else
                        gene.Color = DefaultCogColor
                    End If
                Else
                    gene.Color = DefaultCogColor
                End If
            Next

            Call chromesome.MyvaCogColorProfile.InvokeSet(colorProfiles)

            Return chromesome
        End Function

        <Extension>
        Public Function ApplyingCOGNumberColors(Of MyvaCOG As ICOGDigest)(genes As MyvaCOG(), Chromesome As ChromesomeDrawingModel) As ChromesomeDrawingModel
            Dim ColorProfiles = RenderingColor.InitCOGColors((From cogAlign In genes
                                                              Select cogAlign.COG
                                                              Distinct).ToArray).ToDictionary(Function(obj) obj.Key,
                                                                                              Function(obj) DirectCast(New SolidBrush(obj.Value), Brush))
            Dim DefaultCogColor As New SolidBrush(Chromesome.DrawingConfigurations.NoneCogColor)
            Dim geneTable As Dictionary(Of MyvaCOG) = genes.ToDictionary

            For Each gene As SegmentObject In Chromesome.GeneObjects
                Dim Cog As MyvaCOG = geneTable.TryGetValue(gene.LocusTag)

                If Not Cog Is Nothing Then
                    If Not String.IsNullOrEmpty(Cog.COG) Then
                        gene.Color = ColorProfiles(Cog.COG)
                    Else
                        gene.Color = DefaultCogColor
                    End If
                Else
                    gene.Color = DefaultCogColor
                End If
            Next

            Call Chromesome.MyvaCogColorProfile.InvokeSet(ColorProfiles)

            Return Chromesome
        End Function
    End Module
End Namespace
