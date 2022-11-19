#Region "Microsoft.VisualBasic::e9d7aad4595b5ba18f2b6fe16ec331a4, GCModeller\visualize\DataVisualizationExtensions\CatalogProfiling\BubbleTerm.vb"

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

    '   Total Lines: 87
    '    Code Lines: 60
    ' Comment Lines: 12
    '   Blank Lines: 15
    '     File Size: 3.22 KB


    '     Class BubbleTerm
    ' 
    '         Properties: data, Factor, PValue, termId
    ' 
    '         Function: CreateBubbles, CreateEnrichColors, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports ColorPalette = Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Designer
Imports pathwayBrite = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Pathway
Imports stdVec = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

Namespace CatalogProfiling

    Public Class BubbleTerm

        ''' <summary>
        ''' [X]
        ''' </summary>
        ''' <returns></returns>
        Public Property Factor As Double
        ''' <summary>
        ''' [Y] -log10(p-value)
        ''' </summary>
        ''' <returns></returns>
        Public Property PValue As Double
        ''' <summary>
        ''' bubble radius
        ''' </summary>
        ''' <returns></returns>
        Public Property data As Double
        Public Property termId As String

        Public Overrides Function ToString() As String
            Return $"{termId} [{PValue}, {Factor}, {data}]"
        End Function

        Public Shared Function CreateBubbles(logP As stdVec,
                                             Impact As stdVec,
                                             values As stdVec,
                                             pathwayList As String()) As Dictionary(Of String, BubbleTerm())

            Dim bubbleData As New Dictionary(Of String, List(Of BubbleTerm))
            Dim KOmap As Dictionary(Of String, pathwayBrite) = pathwayBrite _
                .LoadFromResource _
                .ToDictionary(Function(map)
                                  Return map.EntryId
                              End Function)

            For i As Integer = 0 To pathwayList.Length - 1
                Dim map As pathwayBrite = KOmap.TryGetValue(pathwayList(i).Match("\d+"))

                If map Is Nothing Then
                    Continue For
                End If

                If Not bubbleData.ContainsKey(map.class) Then
                    bubbleData.Add(map.class, New List(Of BubbleTerm))
                End If

                bubbleData(map.class).Add(New BubbleTerm With {
                    .data = values(i),
                    .Factor = Impact(i),
                    .PValue = logP(i),
                    .termId = map.entry.text
                })
            Next

            Return bubbleData _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.Value.ToArray
                              End Function)
        End Function

        Public Shared Function CreateEnrichColors(bubbleData As Dictionary(Of String, BubbleTerm()), Optional theme As String = "Set1:c8") As Dictionary(Of String, Color)
            Dim enrichColors As New Dictionary(Of String, Color)
            Dim colorSet As Color() = ColorPalette.GetColors(theme)
            Dim keys As String() = bubbleData.Keys.ToArray

            For i As Integer = 0 To keys.Length - 1
                enrichColors(keys(i)) = colorSet(i)
            Next

            Return enrichColors
        End Function

    End Class
End Namespace
