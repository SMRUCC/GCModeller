#Region "Microsoft.VisualBasic::ecdcac5f18b033209f8464ccb4596c87, visualize\Circos\Circos\Colors\Maps\COG\MvyaColorProfile.vb"

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

    '     Class MvyaColorProfile
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetColor
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace Colors

    ''' <summary>
    ''' A helper for assign color of myva cog result
    ''' </summary>
    Public Class MvyaColorProfile

        ReadOnly colors As Dictionary(Of String, String)
        ReadOnly defaultColor As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myvaCOGs"></param>
        ''' <param name="profiles">Color profiles</param>
        Sub New(myvaCOGs As ICOGCatalog(), profiles As Dictionary(Of String, String), defaultColor As String)
            Me.colors = New Dictionary(Of String, String)
            Me.defaultColor = defaultColor

            For Each line As ICOGCatalog In myvaCOGs
                If String.IsNullOrEmpty(line.COG) Then
                    Call colors.Add(line.Key, defaultColor)
                Else
                    Call colors.Add(line.Key, profiles(line.COG.ToUpper))
                End If
            Next
        End Sub

        Public Function GetColor(geneId As String) As String
            If Not String.IsNullOrEmpty(geneId) AndAlso colors.ContainsKey(geneId) Then
                Return colors(geneId)
            Else
                Return defaultColor
            End If
        End Function
    End Class
End Namespace
