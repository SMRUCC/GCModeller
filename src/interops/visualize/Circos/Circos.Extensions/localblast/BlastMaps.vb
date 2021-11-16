#Region "Microsoft.VisualBasic::d7b9d6916bf1850f16a84bd860f48404, visualize\Circos\Circos.Extensions\localblast\BlastMaps.vb"

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

    '     Class BlastMaps
    ' 
    '         Properties: SpeciesColor, SubjectSpecies
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetEnumerator, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast

Namespace TrackDatas.Highlights

    ''' <summary>
    ''' 必须是同一个物种的
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BlastMaps : Inherits Highlights

        Dim hits As HitRecord()
        Dim Color As String
        Dim identityMode As IdentityColors
        Dim chr As String

        ''' <summary>
        ''' 使用直方图来显示比对成功的区域
        ''' </summary>
        ''' <param name="hits"></param>
        ''' <param name="Color"></param>
        ''' <param name="identityMode"></param>
        ''' <param name="chr"></param>
        Sub New(hits As HitRecord(), Color As String, Optional identityMode As IdentityColors = Nothing, Optional chr As String = "chr1")
            Me.hits = hits
            Me.Color = Color
            Me.identityMode = identityMode
            Me.chr = chr
        End Sub

        Public ReadOnly Property SpeciesColor As String
            Get
                Return Color
            End Get
        End Property

        Public ReadOnly Property SubjectSpecies As String
            Get
                If hits.IsNullOrEmpty Then
                    Return ""
                Else
                    Return hits.First.SubjectIDs
                End If
            End Get
        End Property

        Public Overrides Iterator Function GetEnumerator() As IEnumerator(Of ValueTrackData)
            Dim color As Func(Of Double, String)

            If Not identityMode Is Nothing Then
                color = AddressOf identityMode.GetColor
            Else
                color = Function(d) Me.Color
            End If

            For Each hit As HitRecord In hits
                Dim d As Double = hit.Identity
                Dim cl As String = color(d)

                Yield New ValueTrackData With {
                    .chr = chr,
                    .start = hit.QueryStart,
                    .end = hit.QueryEnd,
                    .formatting = New Formatting With {
                        .fill_color = cl
                    },
                    .value = 1
                }
            Next
        End Function

        Public Overrides Function ToString() As String
            Dim ssID As String = SubjectSpecies
            If String.IsNullOrEmpty(ssID) Then
                Return MyBase.ToString
            Else
                Return ssID
            End If
        End Function
    End Class
End Namespace
