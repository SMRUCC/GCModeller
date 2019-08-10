#Region "Microsoft.VisualBasic::7598801267b731d097125e47e5a9c86c, visualize\Circos\Circos\Colors\Maps\COG\COGColors.vb"

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

    '     Module COGColors
    ' 
    '         Function: GetCogColorProfile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace Colors

    Public Module COGColors

        <ExportAPI("COG.Colors")>
        Public Function GetCogColorProfile(MyvaCOG As ICOGCatalog(), defaultColor As String) As Func(Of String, String)
            Dim COGs As String() =
                LinqAPI.Exec(Of String) <= From gene As ICOGCatalog
                                           In MyvaCOG
                                           Let cId As String = gene.COG
                                           Where Not String.IsNullOrEmpty(cId)
                                           Select cId.ToUpper
                                           Distinct
            Dim ColorProfiles As New Dictionary(Of String, String)
            Dim Colors = PerlColor.Colors.Shuffles.AsList
            Dim i As Integer = 0

            Call Colors.Remove(defaultColor.ToLower)

            For i = Colors.Count To COGs.Length + 10
                Call Colors.Add("Color_" & i)
            Next

            i = 0

            For Each strId As String In COGs
                Dim ColorName As String = Colors(i)

                i += 1
                Call ColorProfiles.Add(strId, ColorName)
            Next

            Return AddressOf New MvyaColorProfile(MyvaCOG, ColorProfiles, defaultColor).GetColor
        End Function

    End Module
End Namespace
