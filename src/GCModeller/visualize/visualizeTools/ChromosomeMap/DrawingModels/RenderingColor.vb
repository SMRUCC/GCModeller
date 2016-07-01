#Region "Microsoft.VisualBasic::96b24ac4e800e561fa99381bdaf83026, ..\GCModeller\visualize\visualizeTools\ChromosomeMap\DrawingModels\RenderingColor.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.GCModeller.DataVisualization
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Imaging

Namespace ChromosomeMap.DrawingModels

    Public Module ModelColorExtensions

        <Extension>
        Public Function ApplyingCOGCategoryColor(MyvaCog As MyvaCOG(), Chromesome As ChromesomeDrawingModel) As ChromesomeDrawingModel
            Dim ColorProfiles = RenderingColor.InitCOGColors(Nothing).ToDictionary(Function(obj) obj.Key,
                                                                                   Function(obj) CType(New SolidBrush(obj.Value), Brush))
            Dim DefaultCogColor As New SolidBrush(Chromesome.DrawingConfigurations.NoneCogColor)

            For Each gene As SegmentObject In Chromesome.GeneObjects
                Dim Cog = MyvaCog.GetItem(gene.LocusTag)
                If Not Cog Is Nothing Then
                    If Not String.IsNullOrEmpty(Cog.Category) Then
                        If Cog.Category.Count > 1 Then
                            gene.Color = New SolidBrush(NeutralizeColor((From c As Char In Cog.Category Select DirectCast(ColorProfiles(c.ToString), SolidBrush).Color).ToArray))
                        Else
                            gene.Color = ColorProfiles(Cog.Category)
                        End If
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

        <Extension>
        Public Function ApplyingCOGNumberColors(MyvaCog As MyvaCOG(), Chromesome As ChromesomeDrawingModel) As ChromesomeDrawingModel
            Dim ColorProfiles = RenderingColor.InitCOGColors((From cogAlign In MyvaCog
                                                              Select cogAlign.COG
                                                              Distinct).ToArray).ToDictionary(Function(obj) obj.Key,
                                                                                              Function(obj) DirectCast(New SolidBrush(obj.Value), Brush))
            Dim DefaultCogColor As New SolidBrush(Chromesome.DrawingConfigurations.NoneCogColor)

            For Each gene As SegmentObject In Chromesome.GeneObjects
                Dim Cog = MyvaCog.GetItem(gene.LocusTag)
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
