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