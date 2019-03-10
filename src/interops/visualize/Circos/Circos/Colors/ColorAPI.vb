﻿#Region "Microsoft.VisualBasic::10258731df83f45a59780875686a197d, visualize\Circos\Circos\Colors\ColorAPI.vb"

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
    '         Properties: Colors
    ' 
    '         Function: GenerateColors, GetCogColorProfile
    '         Class __innerProfiles
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: GetColor
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST

Namespace Colors

    <Package("Circos.COGs.ColorAPI")>
    Public Module ColorAPI

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
            Dim Colors = ColorAPI.Colors.Shuffles.AsList
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

            Return AddressOf New __innerProfiles(MyvaCOG, ColorProfiles, defaultColor).GetColor
        End Function

        Private Class __innerProfiles

            ReadOnly __colors As Dictionary(Of String, String)
            ReadOnly __defaultColor As String

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="myvaCOGs"></param>
            ''' <param name="profiles">Color profiles</param>
            Sub New(myvaCOGs As MyvaCOG(), profiles As Dictionary(Of String, String), defaultColor As String)
                __colors = New Dictionary(Of String, String)
                __defaultColor = defaultColor

                For Each line As MyvaCOG In myvaCOGs
                    If String.IsNullOrEmpty(line.COG) Then
                        Call __colors.Add(line.QueryName, defaultColor)
                    Else
                        Call __colors.Add(line.QueryName, profiles(line.COG.ToUpper))
                    End If
                Next
            End Sub

            Public Function GetColor(geneId As String) As String
                Return If(Not String.IsNullOrEmpty(geneId) AndAlso
                    __colors.ContainsKey(geneId),
                    __colors(geneId), __defaultColor)
            End Function
        End Class

        ''' <summary>
        ''' {Key, ColorString}
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Colors.Maps")>
        Public Function GenerateColors(categories$()) As Dictionary(Of String, String)
            Dim colors$() = _Colors.Shuffles
            Dim hash As New Dictionary(Of String, String)

            For Each key As SeqValue(Of String) In categories.SeqIterator
                Call hash.Add(key.value$, colors(key.i%))
            Next

            Return hash
        End Function

        Public ReadOnly Property Colors As String() = {
 _
            "white", "aliceblue", "antiquewhite", "antiquewhite1", "antiquewhite2", "antiquewhite3", "antiquewhite4", "aquamarine", "aquamarine1", "aquamarine2",
            "aquamarine3", "aquamarine4", "azure", "azure1", "azure2", "azure3", "azure4",
 _
            "beige", "bisque", "bisque1", "bisque2", "bisque3", "bisque4", "black", "blanchedalmond", "blue", "blue1", "blue2", "blue3", "blue4", "blueviolet",
            "brown", "brown1", "brown2", "brown3", "brown4", "burlywood", "burlywood1", "burlywood2", "burlywood3", "burlywood4",
 _
            "cadetblue", "cadetblue1", "cadetblue2", "cadetblue3", "cadetblue4", "chartreuse", "chartreuse1", "chartreuse2", "chartreuse3", "chartreuse4", "chocolate",
            "chocolate1", "chocolate2", "chocolate3", "chocolate4", "coral", "coral1", "coral2", "coral3", "coral4", "cornflowerblue", "cornsilk", "cornsilk1",
            "cornsilk2", "cornsilk3", "cornsilk4", "cyan", "cyan1", "cyan2", "cyan3", "cyan4",
 _
            "darkblue", "darkcyan", "darkgoldenrod", "darkgoldenrod1", "darkgoldenrod2", "darkgoldenrod3", "darkgoldenrod4", "darkgray", "darkgreen", "darkgrey",
            "darkkhaki", "darkmagenta", "darkolivegreen", "darkolivegreen1", "darkolivegreen2", "darkolivegreen3", "darkolivegreen4", "darkorange", "darkorange1",
            "darkorange2", "darkorange3", "darkorange4", "darkorchid", "darkorchid1", "darkorchid2", "darkorchid3", "darkorchid4", "darkred", "darksalmon", "darkseagreen",
            "darkseagreen1", "darkseagreen2", "darkseagreen3", "darkseagreen4", "darkslateblue", "darkslategray", "darkslategray1", "darkslategray2", "darkslategray3",
            "darkslategray4", "darkslategrey", "darkturquoise", "darkviolet", "deeppink", "deeppink1", "deeppink2", "deeppink3", "deeppink4", "deepskyblue",
            "deepskyblue1", "deepskyblue2", "deepskyblue3", "deepskyblue4", "dimgray", "dimgrey", "dodgerblue", "dodgerblue1", "dodgerblue2", "dodgerblue3", "dodgerblue4",
 _
            "firebrick", "firebrick1", "firebrick2", "firebrick3", "firebrick4", "floralwhite", "forestgreen",
 _
            "gainsboro", "ghostwhite", "gold", "gold1", "gold2", "gold3", "gold4", "goldenrod", "goldenrod1", "goldenrod2", "goldenrod3", "goldenrod4", "gray", "gray0",
            "gray1", "gray2", "gray3", "gray4", "gray5", "gray6", "gray7", "gray8", "gray9", "gray10", "gray11", "gray12", "gray13", "gray14", "gray15", "gray16",
            "gray17", "gray18", "gray19", "gray20", "gray21", "gray22", "gray23", "gray24", "gray25", "gray26", "gray27", "gray28", "gray29", "gray30", "gray31",
            "gray32", "gray33", "gray34", "gray35", "gray36", "gray37", "gray38", "gray39", "gray40", "gray41", "gray42", "gray43", "gray44", "gray45", "gray46", "gray47",
            "gray48", "gray49", "gray50", "gray51", "gray52", "gray53", "gray54", "gray55", "gray56", "gray57", "gray58", "gray59", "gray60", "gray61", "gray62", "gray63",
            "gray64", "gray65", "gray66", "gray67", "gray68", "gray69", "gray70", "gray71", "gray72", "gray73", "gray74", "gray75", "gray76", "gray77", "gray78", "gray79",
            "gray80", "gray81", "gray82", "gray83", "gray84", "gray85", "gray86", "gray87", "gray88", "gray89", "gray90", "gray91", "gray92", "gray93", "gray94", "gray95",
            "gray96", "gray97", "gray98", "gray99", "gray100", "green", "green1", "green2", "green3", "green4", "greenyellow", "grey", "grey0", "grey1", "grey2", "grey3",
            "grey4", "grey5", "grey6", "grey7", "grey8", "grey9", "grey10", "grey11", "grey12", "grey13", "grey14", "grey15", "grey16", "grey17", "grey18", "grey19",
            "grey20", "grey21", "grey22", "grey23", "grey24", "grey25", "grey26", "grey27", "grey28", "grey29", "grey30", "grey31", "grey32", "grey33", "grey34", "grey35",
            "grey36", "grey37", "grey38", "grey39", "grey40", "grey41", "grey42", "grey43", "grey44", "grey45", "grey46", "grey47", "grey48", "grey49", "grey50", "grey51",
            "grey52", "grey53", "grey54", "grey55", "grey56", "grey57", "grey58", "grey59", "grey60", "grey61", "grey62", "grey63", "grey64", "grey65", "grey66", "grey67",
            "grey68", "grey69", "grey70", "grey71", "grey72", "grey73", "grey74", "grey75", "grey76", "grey77", "grey78", "grey79", "grey80", "grey81", "grey82", "grey83",
            "grey84", "grey85", "grey86", "grey87", "grey88", "grey89", "grey90", "grey91", "grey92", "grey93", "grey94", "grey95", "grey96", "grey97", "grey98", "grey99",
            "grey100",
 _
            "honeydew", "honeydew1", "honeydew2", "honeydew3", "honeydew4", "hotpink", "hotpink1", "hotpink2", "hotpink3", "hotpink4",
 _
            "indianred", "indianred1", "indianred2", "indianred3", "indianred4", "ivory", "ivory1", "ivory2", "ivory3", "ivory4",
 _
            "khaki", "khaki1", "khaki2", "khaki3", "khaki4",
            "lavender", "lavenderblush", "lavenderblush1", "lavenderblush2", "lavenderblush3", "lavenderblush4", "lawngreen", "lemonchiffon", "lemonchiffon1", "lemonchiffon2",
            "lemonchiffon3", "lemonchiffon4", "lightblue", "lightblue1", "lightblue2", "lightblue3", "lightblue4", "lightcoral", "lightcyan", "lightcyan1", "lightcyan2",
            "lightcyan3", "lightcyan4", "lightgoldenrod", "lightgoldenrod1", "lightgoldenrod2", "lightgoldenrod3", "lightgoldenrod4", "lightgoldenrodyellow", "lightgray",
            "lightgreen", "lightgrey", "lightpink", "lightpink1", "lightpink2", "lightpink3", "lightpink4", "lightsalmon", "lightsalmon1", "lightsalmon2", "lightsalmon3",
            "lightsalmon4", "lightseagreen", "lightskyblue", "lightskyblue1", "lightskyblue2", "lightskyblue3", "lightskyblue4", "lightslateblue", "lightslategray",
            "lightslategrey", "lightsteelblue", "lightsteelblue1", "lightsteelblue2", "lightsteelblue3", "lightsteelblue4", "lightyellow", "lightyellow1", "lightyellow2",
            "lightyellow3", "lightyellow4", "limegreen", "linen",
 _
            "magenta", "magenta1", "magenta2", "magenta3", "magenta4", "maroon", "maroon1", "maroon2", "maroon3", "maroon4", "mediumaquamarine", "mediumblue", "mediumorchid",
            "mediumorchid1", "mediumorchid2", "mediumorchid3", "mediumorchid4", "mediumpurple", "mediumpurple1", "mediumpurple2", "mediumpurple3", "mediumpurple4",
            "mediumseagreen", "mediumslateblue", "mediumspringgreen", "mediumturquoise", "mediumvioletred", "midnightblue", "mintcream", "mistyrose", "mistyrose1",
            "mistyrose2", "mistyrose3", "mistyrose4", "moccasin",
 _
            "navajowhite", "navajowhite1", "navajowhite2", "navajowhite3", "navajowhite4", "navy", "navyblue",
 _
            "oldlace", "olivedrab", "olivedrab1", "olivedrab2", "olivedrab3", "olivedrab4", "orange", "orange1", "orange2", "orange3", "orange4", "orangered", "orangered1",
            "orangered2", "orangered3", "orangered4", "orchid", "orchid1", "orchid2", "orchid3", "orchid4",
 _
            "palegoldenrod", "palegreen", "palegreen1", "palegreen2", "palegreen3", "palegreen4", "paleturquoise", "paleturquoise1", "paleturquoise2", "paleturquoise3",
            "paleturquoise4", "palevioletred", "palevioletred1", "palevioletred2", "palevioletred3", "palevioletred4", "papayawhip", "peachpuff", "peachpuff1", "peachpuff2",
            "peachpuff3", "peachpuff4", "peru", "pink", "pink1", "pink2", "pink3", "pink4", "plum", "plum1", "plum2", "plum3", "plum4", "powderblue", "purple", "purple1",
            "purple2", "purple3", "purple4",
 _
            "red", "red1", "red2", "red3", "red4", "rosybrown", "rosybrown1", "rosybrown2", "rosybrown3", "rosybrown4", "royalblue", "royalblue1", "royalblue2", "royalblue3",
            "royalblue4",
 _
            "saddlebrown", "salmon", "salmon1", "salmon2", "salmon3", "salmon4", "sandybrown", "seagreen", "seagreen1", "seagreen2", "seagreen3", "seagreen4", "seashell",
            "seashell1", "seashell2", "seashell3", "seashell4", "sienna", "sienna1", "sienna2", "sienna3", "sienna4", "skyblue", "skyblue1", "skyblue2", "skyblue3",
            "skyblue4", "slateblue", "slateblue1", "slateblue2", "slateblue3", "slateblue4", "slategray", "slategray1", "slategray2", "slategray3", "slategray4",
            "slategrey", "snow", "snow1", "snow2", "snow3", "snow4", "springgreen", "springgreen1", "springgreen2", "springgreen3", "springgreen4", "steelblue", "steelblue1",
            "steelblue2", "steelblue3", "steelblue4",
 _
            "tan", "tan1", "tan2", "tan3", "tan4", "thistle", "thistle1", "thistle2", "thistle3", "thistle4", "tomato", "tomato1", "tomato2", "tomato3", "tomato4", "turquoise",
            "turquoise1", "turquoise2", "turquoise3", "turquoise4",
 _
            "violet", "violetred", "violetred1", "violetred2", "violetred3", "violetred4",
 _
            "wheat", "wheat1", "wheat2", "wheat3", "wheat4", "whitesmoke",
 _
            "yellow", "yellow1", "yellow2", "yellow3", "yellow4", "yellowgreen"
        }
    End Module
End Namespace
