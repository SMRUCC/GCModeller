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
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel

Namespace DrawingModels

    Public Module ModelColorExtensions

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="MyvaCOG">需要实现有 <see cref="ICOGCatalog"/> 接口</typeparam>
        ''' <param name="genes"></param>
        ''' <param name="chromesome"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ApplyingCOGCategoryColor(Of MyvaCOG As ICOGCatalog)(genes As MyvaCOG(), chromesome As ChromesomeDrawingModel) As ChromesomeDrawingModel
            Dim colorProfiles As Dictionary(Of String, Brush) = RenderingColor _
                .InitCOGColors(Nothing) _
                .ToDictionary(Function(obj) obj.Key,
                              Function(obj) CType(New SolidBrush(obj.Value), Brush))
            With chromesome

                Dim defaultCOG_color As New SolidBrush(.DrawingConfigurations.NoneCogColor)
                Dim geneTable As Dictionary(Of MyvaCOG) = genes.ToDictionary

                For Each gene As SegmentObject In .GeneObjects
                    Dim COG_gene As MyvaCOG = geneTable.TryGetValue(gene.LocusTag)

                    If Not COG_gene Is Nothing AndAlso Not COG_gene.Catalog.IsBlank Then

                        If COG_gene.COG.Length > 1 Then
                            Dim neuColor As Color = COG_gene.Catalog _
                                .ToArray(Function(c) DirectCast(colorProfiles(CStr(c)), SolidBrush).Color) _
                                .NeutralizeColor
                            gene.Color = New SolidBrush(neuColor)
                        Else
                            gene.Color = colorProfiles.TryGetValue(COG_gene.Catalog, defaultCOG_color)
                        End If

                    Else
                        gene.Color = defaultCOG_color
                    End If
                Next

                .MyvaCogColorProfile = colorProfiles
            End With

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
