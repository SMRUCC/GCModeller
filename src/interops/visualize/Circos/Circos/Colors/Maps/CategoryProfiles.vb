#Region "Microsoft.VisualBasic::3bf50bfd451b8173d256fde134cae818, Circos\Colors\Maps\ColorAPI.vb"

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

    '     Module ColorAPI
    ' 
    '         Function: GenerateColors, GetCogColorProfile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Option Strict Off

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST

Namespace Colors

    <Package("Circos.COGs.ColorAPI")>
    Public Module CategoryProfiles

        <ExportAPI("COG.Colors")>
        Public Function GetCogColorProfile(MyvaCOG As MyvaCOG(), defaultColor As String) As Func(Of String, String)
            Dim COGs As String() =
                LinqAPI.Exec(Of String) <= From gene As MyvaCOG
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

        ''' <summary>
        ''' {Key, ColorString}
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Colors.Maps")>
        Public Function GenerateColors(categories$()) As Dictionary(Of String, String)
            Dim colors$() = PerlColor.Colors.Shuffles
            Dim maps As New Dictionary(Of String, String)

            For Each key As SeqValue(Of String) In categories.SeqIterator
                Call maps.Add(key.value$, colors(key))
            Next

            Return maps
        End Function
    End Module
End Namespace
