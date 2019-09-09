#Region "Microsoft.VisualBasic::ff5c0fc0fdc4836becd39d41a03fc553, RDotNET\Graphics\Colors.vb"

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

    '     Class Colors
    ' 
    '         Properties: AliceBlue, AntiqueWhite, AntiqueWhite1, AntiqueWhite2, AntiqueWhite3
    '                     AntiqueWhite4, Aquamarine, Aquamarine1, Aquamarine2, Aquamarine3
    '                     Aquamarine4, Azure, Azure1, Azure2, Azure3
    '                     Azure4, Beige, Bisque, Bisque1, Bisque2
    '                     Bisque3, Bisque4, Black, BlanchedAlmond, Blue
    '                     Blue1, Blue2, Blue3, Blue4, BlueViolet
    '                     Brown, Brown1, Brown2, Brown3, Brown4
    '                     Burlywood, Burlywood1, Burlywood2, Burlywood3, Burlywood4
    '                     CadetBlue, CadetBlue1, CadetBlue2, CadetBlue3, CadetBlue4
    '                     Chartreuse, Chartreuse1, Chartreuse2, Chartreuse3, Chartreuse4
    '                     Chocolate, Chocolate1, Chocolate2, Chocolate3, Chocolate4
    '                     Coral, Coral1, Coral2, Coral3, Coral4
    '                     CornflowerBlue, Cornsilk, Cornsilk1, Cornsilk2, Cornsilk3
    '                     Cornsilk4, Cyan, Cyan1, Cyan2, Cyan3
    '                     Cyan4, DarkBlue, DarkCyan, DarkGoldenrod, DarkGoldenrod1
    '                     DarkGoldenrod2, DarkGoldenrod3, DarkGoldenrod4, DarkGray, DarkGreen
    '                     DarkGrey, DarkKhaki, DarkMagenta, DarkOliveGreen, DarkOliveGreen1
    '                     DarkOliveGreen2, DarkOliveGreen3, DarkOliveGreen4, DarkOrange, DarkOrange1
    '                     DarkOrange2, DarkOrange3, DarkOrange4, DarkOrchid, DarkOrchid1
    '                     DarkOrchid2, DarkOrchid3, DarkOrchid4, DarkRed, DarkSalmon
    '                     DarkSeaGreen, DarkSeaGreen1, DarkSeaGreen2, DarkSeaGreen3, DarkSeaGreen4
    '                     DarkSlateBlue, DarkSlateGray, DarkSlateGray1, DarkSlateGray2, DarkSlateGray3
    '                     DarkSlateGray4, DarkSlateGrey, DarkTurquoise, DarkViolet, DeepPink
    '                     DeepPink1, DeepPink2, DeepPink3, DeepPink4, DeepSkyBlue
    '                     DeepSkyBlue1, DeepSkyBlue2, DeepSkyBlue3, DeepSkyBlue4, DimGray
    '                     DimGrey, DodgerBlue, DodgerBlue1, DodgerBlue2, DodgerBlue3
    '                     DodgerBlue4, Firebrick, Firebrick1, Firebrick2, Firebrick3
    '                     Firebrick4, FloralWhite, ForestGreen, Gainsboro, GhostWhite
    '                     Gold, Gold1, Gold2, Gold3, Gold4
    '                     Goldenrod, Goldenrod1, Goldenrod2, Goldenrod3, Goldenrod4
    '                     Gray, Gray0, Gray1, Gray10, Gray100
    '                     Gray11, Gray12, Gray13, Gray14, Gray15
    '                     Gray16, Gray17, Gray18, Gray19, Gray2
    '                     Gray20, Gray21, Gray22, Gray23, Gray24
    '                     Gray25, Gray26, Gray27, Gray28, Gray29
    '                     Gray3, Gray30, Gray31, Gray32, Gray33
    '                     Gray34, Gray35, Gray36, Gray37, Gray38
    '                     Gray39, Gray4, Gray40, Gray41, Gray42
    '                     Gray43, Gray44, Gray45, Gray46, Gray47
    '                     Gray48, Gray49, Gray5, Gray50, Gray51
    '                     Gray52, Gray53, Gray54, Gray55, Gray56
    '                     Gray57, Gray58, Gray59, Gray6, Gray60
    '                     Gray61, Gray62, Gray63, Gray64, Gray65
    '                     Gray66, Gray67, Gray68, Gray69, Gray7
    '                     Gray70, Gray71, Gray72, Gray73, Gray74
    '                     Gray75, Gray76, Gray77, Gray78, Gray79
    '                     Gray8, Gray80, Gray81, Gray82, Gray83
    '                     Gray84, Gray85, Gray86, Gray87, Gray88
    '                     Gray89, Gray9, Gray90, Gray91, Gray92
    '                     Gray93, Gray94, Gray95, Gray96, Gray97
    '                     Gray98, Gray99, Green, Green1, Green2
    '                     Green3, Green4, GreenYellow, Grey, Grey0
    '                     Grey1, Grey10, Grey100, Grey11, Grey12
    '                     Grey13, Grey14, Grey15, Grey16, Grey17
    '                     Grey18, Grey19, Grey2, Grey20, Grey21
    '                     Grey22, Grey23, Grey24, Grey25, Grey26
    '                     Grey27, Grey28, Grey29, Grey3, Grey30
    '                     Grey31, Grey32, Grey33, Grey34, Grey35
    '                     Grey36, Grey37, Grey38, Grey39, Grey4
    '                     Grey40, Grey41, Grey42, Grey43, Grey44
    '                     Grey45, Grey46, Grey47, Grey48, Grey49
    '                     Grey5, Grey50, Grey51, Grey52, Grey53
    '                     Grey54, Grey55, Grey56, Grey57, Grey58
    '                     Grey59, Grey6, Grey60, Grey61, Grey62
    '                     Grey63, Grey64, Grey65, Grey66, Grey67
    '                     Grey68, Grey69, Grey7, Grey70, Grey71
    '                     Grey72, Grey73, Grey74, Grey75, Grey76
    '                     Grey77, Grey78, Grey79, Grey8, Grey80
    '                     Grey81, Grey82, Grey83, Grey84, Grey85
    '                     Grey86, Grey87, Grey88, Grey89, Grey9
    '                     Grey90, Grey91, Grey92, Grey93, Grey94
    '                     Grey95, Grey96, Grey97, Grey98, Grey99
    '                     Honeydew, Honeydew1, Honeydew2, Honeydew3, Honeydew4
    '                     HotPink, HotPink1, HotPink2, HotPink3, HotPink4
    '                     IndianRed, IndianRed1, IndianRed2, IndianRed3, IndianRed4
    '                     Ivory, Ivory1, Ivory2, Ivory3, Ivory4
    '                     Khaki, Khaki1, Khaki2, Khaki3, Khaki4
    '                     Lavender, LavenderBlush, LavenderBlush1, LavenderBlush2, LavenderBlush3
    '                     LavenderBlush4, LawnGreen, LemonChiffon, LemonChiffon1, LemonChiffon2
    '                     LemonChiffon3, LemonChiffon4, LightBlue, LightBlue1, LightBlue2
    '                     LightBlue3, LightBlue4, LightCoral, LightCyan, LightCyan1
    '                     LightCyan2, LightCyan3, LightCyan4, LightGoldenrod, LightGoldenrod1
    '                     LightGoldenrod2, LightGoldenrod3, LightGoldenrod4, LightGoldenrodYellow, LightGray
    '                     LightGreen, LightGrey, LightPink, LightPink1, LightPink2
    '                     LightPink3, LightPink4, LightSalmon, LightSalmon1, LightSalmon2
    '                     LightSalmon3, LightSalmon4, LightSeaGreen, LightSkyBlue, LightSkyBlue1
    '                     LightSkyBlue2, LightSkyBlue3, LightSkyBlue4, LightSlateBlue, LightSlateGray
    '                     LightSlateGrey, LightSteelBlue, LightSteelBlue1, LightSteelBlue2, LightSteelBlue3
    '                     LightSteelBlue4, LightYellow, LightYellow1, LightYellow2, LightYellow3
    '                     LightYellow4, LimeGreen, Linen, Magenta, Magenta1
    '                     Magenta2, Magenta3, Magenta4, Maroon, Maroon1
    '                     Maroon2, Maroon3, Maroon4, MediumAquamarine, MediumBlue
    '                     MediumOrchid, MediumOrchid1, MediumOrchid2, MediumOrchid3, MediumOrchid4
    '                     MediumPurple, MediumPurple1, MediumPurple2, MediumPurple3, MediumPurple4
    '                     MediumSeaGreen, MediumSlateBlue, MediumSpringGreen, MediumTurquoise, MediumVioletRed
    '                     MidnightBlue, MintCream, MistyRose, MistyRose1, MistyRose2
    '                     MistyRose3, MistyRose4, Moccasin, NavajoWhite, NavajoWhite1
    '                     NavajoWhite2, NavajoWhite3, NavajoWhite4, Navy, NavyBlue
    '                     OldLace, OliveDrab, OliveDrab1, OliveDrab2, OliveDrab3
    '                     OliveDrab4, Orange, Orange1, Orange2, Orange3
    '                     Orange4, OrangeRed, OrangeRed1, OrangeRed2, OrangeRed3
    '                     OrangeRed4, Orchid, Orchid1, Orchid2, Orchid3
    '                     Orchid4, PaleGoldenrod, PaleGreen, PaleGreen1, PaleGreen2
    '                     PaleGreen3, PaleGreen4, PaleTurquoise, PaleTurquoise1, PaleTurquoise2
    '                     PaleTurquoise3, PaleTurquoise4, PaleVioletRed, PaleVioletRed1, PaleVioletRed2
    '                     PaleVioletRed3, PaleVioletRed4, PapayaWhip, PeachPuff, PeachPuff1
    '                     PeachPuff2, PeachPuff3, PeachPuff4, Peru, Pink
    '                     Pink1, Pink2, Pink3, Pink4, Plum
    '                     Plum1, Plum2, Plum3, Plum4, PowderBlue
    '                     Purple, Purple1, Purple2, Purple3, Purple4
    '                     Red, Red1, Red2, Red3, Red4
    '                     RosyBrown, RosyBrown1, RosyBrown2, RosyBrown3, RosyBrown4
    '                     RoyalBlue, RoyalBlue1, RoyalBlue2, RoyalBlue3, RoyalBlue4
    '                     SaddleBrown, Salmon, Salmon1, Salmon2, Salmon3
    '                     Salmon4, SandyBrown, SeaGreen, SeaGreen1, SeaGreen2
    '                     SeaGreen3, SeaGreen4, Seashell, Seashell1, Seashell2
    '                     Seashell3, Seashell4, Sienna, Sienna1, Sienna2
    '                     Sienna3, Sienna4, SkyBlue, SkyBlue1, SkyBlue2
    '                     SkyBlue3, SkyBlue4, SlateBlue, SlateBlue1, SlateBlue2
    '                     SlateBlue3, SlateBlue4, SlateGray, SlateGray1, SlateGray2
    '                     SlateGray3, SlateGray4, SlateGrey, Snow, Snow1
    '                     Snow2, Snow3, Snow4, SpringGreen, SpringGreen1
    '                     SpringGreen2, SpringGreen3, SpringGreen4, SteelBlue, SteelBlue1
    '                     SteelBlue2, SteelBlue3, SteelBlue4, Tan, Tan1
    '                     Tan2, Tan3, Tan4, Thistle, Thistle1
    '                     Thistle2, Thistle3, Thistle4, Tomato, Tomato1
    '                     Tomato2, Tomato3, Tomato4, Transparent, Turquoise
    '                     Turquoise1, Turquoise2, Turquoise3, Turquoise4, Violet
    '                     VioletRed, VioletRed1, VioletRed2, VioletRed3, VioletRed4
    '                     Wheat, Wheat1, Wheat2, Wheat3, Wheat4
    '                     White, WhiteSmoke, Yellow, Yellow1, Yellow2
    '                     Yellow3, Yellow4, YellowGreen
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Graphics

    ''' <summary>
    ''' Implements a set of predefined colors.
    ''' </summary>
    Public NotInheritable Class Colors
        Private Sub New()
        End Sub
        ''' <summary>Gets Transparent.</summary>
        Public Shared ReadOnly Property Transparent() As Color
            Get
                Return Color.FromArgb(0, Byte.MaxValue, Byte.MaxValue, Byte.MaxValue)
            End Get
        End Property

        ''' <summary>Gets AliceBlue.</summary>
        Public Shared ReadOnly Property AliceBlue() As Color
            Get
                Return Color.FromRgb(240, 248, 255)
            End Get
        End Property

        ''' <summary>Gets AntiqueWhite.</summary>
        Public Shared ReadOnly Property AntiqueWhite() As Color
            Get
                Return Color.FromRgb(250, 235, 215)
            End Get
        End Property

        ''' <summary>Gets AntiqueWhite1.</summary>
        Public Shared ReadOnly Property AntiqueWhite1() As Color
            Get
                Return Color.FromRgb(255, 239, 219)
            End Get
        End Property

        ''' <summary>Gets AntiqueWhite2.</summary>
        Public Shared ReadOnly Property AntiqueWhite2() As Color
            Get
                Return Color.FromRgb(238, 223, 204)
            End Get
        End Property

        ''' <summary>Gets AntiqueWhite3.</summary>
        Public Shared ReadOnly Property AntiqueWhite3() As Color
            Get
                Return Color.FromRgb(205, 192, 176)
            End Get
        End Property

        ''' <summary>Gets AntiqueWhite4.</summary>
        Public Shared ReadOnly Property AntiqueWhite4() As Color
            Get
                Return Color.FromRgb(139, 131, 120)
            End Get
        End Property

        ''' <summary>Gets Aquamarine.</summary>
        Public Shared ReadOnly Property Aquamarine() As Color
            Get
                Return Color.FromRgb(127, 255, 212)
            End Get
        End Property

        ''' <summary>Gets Aquamarine1.</summary>
        Public Shared ReadOnly Property Aquamarine1() As Color
            Get
                Return Color.FromRgb(127, 255, 212)
            End Get
        End Property

        ''' <summary>Gets Aquamarine2.</summary>
        Public Shared ReadOnly Property Aquamarine2() As Color
            Get
                Return Color.FromRgb(118, 238, 198)
            End Get
        End Property

        ''' <summary>Gets Aquamarine3.</summary>
        Public Shared ReadOnly Property Aquamarine3() As Color
            Get
                Return Color.FromRgb(102, 205, 170)
            End Get
        End Property

        ''' <summary>Gets Aquamarine4.</summary>
        Public Shared ReadOnly Property Aquamarine4() As Color
            Get
                Return Color.FromRgb(69, 139, 116)
            End Get
        End Property

        ''' <summary>Gets Azure.</summary>
        Public Shared ReadOnly Property Azure() As Color
            Get
                Return Color.FromRgb(240, 255, 255)
            End Get
        End Property

        ''' <summary>Gets Azure1.</summary>
        Public Shared ReadOnly Property Azure1() As Color
            Get
                Return Color.FromRgb(240, 255, 255)
            End Get
        End Property

        ''' <summary>Gets Azure2.</summary>
        Public Shared ReadOnly Property Azure2() As Color
            Get
                Return Color.FromRgb(224, 238, 238)
            End Get
        End Property

        ''' <summary>Gets Azure3.</summary>
        Public Shared ReadOnly Property Azure3() As Color
            Get
                Return Color.FromRgb(193, 205, 205)
            End Get
        End Property

        ''' <summary>Gets Azure4.</summary>
        Public Shared ReadOnly Property Azure4() As Color
            Get
                Return Color.FromRgb(131, 139, 139)
            End Get
        End Property

        ''' <summary>Gets Beige.</summary>
        Public Shared ReadOnly Property Beige() As Color
            Get
                Return Color.FromRgb(245, 245, 220)
            End Get
        End Property

        ''' <summary>Gets Bisque.</summary>
        Public Shared ReadOnly Property Bisque() As Color
            Get
                Return Color.FromRgb(255, 228, 196)
            End Get
        End Property

        ''' <summary>Gets Bisque1.</summary>
        Public Shared ReadOnly Property Bisque1() As Color
            Get
                Return Color.FromRgb(255, 228, 196)
            End Get
        End Property

        ''' <summary>Gets Bisque2.</summary>
        Public Shared ReadOnly Property Bisque2() As Color
            Get
                Return Color.FromRgb(238, 213, 183)
            End Get
        End Property

        ''' <summary>Gets Bisque3.</summary>
        Public Shared ReadOnly Property Bisque3() As Color
            Get
                Return Color.FromRgb(205, 183, 158)
            End Get
        End Property

        ''' <summary>Gets Bisque4.</summary>
        Public Shared ReadOnly Property Bisque4() As Color
            Get
                Return Color.FromRgb(139, 125, 107)
            End Get
        End Property

        ''' <summary>Gets Black.</summary>
        Public Shared ReadOnly Property Black() As Color
            Get
                Return Color.FromRgb(0, 0, 0)
            End Get
        End Property

        ''' <summary>Gets BlanchedAlmond.</summary>
        Public Shared ReadOnly Property BlanchedAlmond() As Color
            Get
                Return Color.FromRgb(255, 235, 205)
            End Get
        End Property

        ''' <summary>Gets Blue.</summary>
        Public Shared ReadOnly Property Blue() As Color
            Get
                Return Color.FromRgb(0, 0, 255)
            End Get
        End Property

        ''' <summary>Gets Blue1.</summary>
        Public Shared ReadOnly Property Blue1() As Color
            Get
                Return Color.FromRgb(0, 0, 255)
            End Get
        End Property

        ''' <summary>Gets Blue2.</summary>
        Public Shared ReadOnly Property Blue2() As Color
            Get
                Return Color.FromRgb(0, 0, 238)
            End Get
        End Property

        ''' <summary>Gets Blue3.</summary>
        Public Shared ReadOnly Property Blue3() As Color
            Get
                Return Color.FromRgb(0, 0, 205)
            End Get
        End Property

        ''' <summary>Gets Blue4.</summary>
        Public Shared ReadOnly Property Blue4() As Color
            Get
                Return Color.FromRgb(0, 0, 139)
            End Get
        End Property

        ''' <summary>Gets BlueViolet.</summary>
        Public Shared ReadOnly Property BlueViolet() As Color
            Get
                Return Color.FromRgb(138, 43, 226)
            End Get
        End Property

        ''' <summary>Gets Brown.</summary>
        Public Shared ReadOnly Property Brown() As Color
            Get
                Return Color.FromRgb(165, 42, 42)
            End Get
        End Property

        ''' <summary>Gets Brown1.</summary>
        Public Shared ReadOnly Property Brown1() As Color
            Get
                Return Color.FromRgb(255, 64, 64)
            End Get
        End Property

        ''' <summary>Gets Brown2.</summary>
        Public Shared ReadOnly Property Brown2() As Color
            Get
                Return Color.FromRgb(238, 59, 59)
            End Get
        End Property

        ''' <summary>Gets Brown3.</summary>
        Public Shared ReadOnly Property Brown3() As Color
            Get
                Return Color.FromRgb(205, 51, 51)
            End Get
        End Property

        ''' <summary>Gets Brown4.</summary>
        Public Shared ReadOnly Property Brown4() As Color
            Get
                Return Color.FromRgb(139, 35, 35)
            End Get
        End Property

        ''' <summary>Gets Burlywood.</summary>
        Public Shared ReadOnly Property Burlywood() As Color
            Get
                Return Color.FromRgb(222, 184, 135)
            End Get
        End Property

        ''' <summary>Gets Burlywood1.</summary>
        Public Shared ReadOnly Property Burlywood1() As Color
            Get
                Return Color.FromRgb(255, 211, 155)
            End Get
        End Property

        ''' <summary>Gets Burlywood2.</summary>
        Public Shared ReadOnly Property Burlywood2() As Color
            Get
                Return Color.FromRgb(238, 197, 145)
            End Get
        End Property

        ''' <summary>Gets Burlywood3.</summary>
        Public Shared ReadOnly Property Burlywood3() As Color
            Get
                Return Color.FromRgb(205, 170, 125)
            End Get
        End Property

        ''' <summary>Gets Burlywood4.</summary>
        Public Shared ReadOnly Property Burlywood4() As Color
            Get
                Return Color.FromRgb(139, 115, 85)
            End Get
        End Property

        ''' <summary>Gets CadetBlue.</summary>
        Public Shared ReadOnly Property CadetBlue() As Color
            Get
                Return Color.FromRgb(95, 158, 160)
            End Get
        End Property

        ''' <summary>Gets CadetBlue1.</summary>
        Public Shared ReadOnly Property CadetBlue1() As Color
            Get
                Return Color.FromRgb(152, 245, 255)
            End Get
        End Property

        ''' <summary>Gets CadetBlue2.</summary>
        Public Shared ReadOnly Property CadetBlue2() As Color
            Get
                Return Color.FromRgb(142, 229, 238)
            End Get
        End Property

        ''' <summary>Gets CadetBlue3.</summary>
        Public Shared ReadOnly Property CadetBlue3() As Color
            Get
                Return Color.FromRgb(122, 197, 205)
            End Get
        End Property

        ''' <summary>Gets CadetBlue4.</summary>
        Public Shared ReadOnly Property CadetBlue4() As Color
            Get
                Return Color.FromRgb(83, 134, 139)
            End Get
        End Property

        ''' <summary>Gets Chartreuse.</summary>
        Public Shared ReadOnly Property Chartreuse() As Color
            Get
                Return Color.FromRgb(127, 255, 0)
            End Get
        End Property

        ''' <summary>Gets Chartreuse1.</summary>
        Public Shared ReadOnly Property Chartreuse1() As Color
            Get
                Return Color.FromRgb(127, 255, 0)
            End Get
        End Property

        ''' <summary>Gets Chartreuse2.</summary>
        Public Shared ReadOnly Property Chartreuse2() As Color
            Get
                Return Color.FromRgb(118, 238, 0)
            End Get
        End Property

        ''' <summary>Gets Chartreuse3.</summary>
        Public Shared ReadOnly Property Chartreuse3() As Color
            Get
                Return Color.FromRgb(102, 205, 0)
            End Get
        End Property

        ''' <summary>Gets Chartreuse4.</summary>
        Public Shared ReadOnly Property Chartreuse4() As Color
            Get
                Return Color.FromRgb(69, 139, 0)
            End Get
        End Property

        ''' <summary>Gets Chocolate.</summary>
        Public Shared ReadOnly Property Chocolate() As Color
            Get
                Return Color.FromRgb(210, 105, 30)
            End Get
        End Property

        ''' <summary>Gets Chocolate1.</summary>
        Public Shared ReadOnly Property Chocolate1() As Color
            Get
                Return Color.FromRgb(255, 127, 36)
            End Get
        End Property

        ''' <summary>Gets Chocolate2.</summary>
        Public Shared ReadOnly Property Chocolate2() As Color
            Get
                Return Color.FromRgb(238, 118, 33)
            End Get
        End Property

        ''' <summary>Gets Chocolate3.</summary>
        Public Shared ReadOnly Property Chocolate3() As Color
            Get
                Return Color.FromRgb(205, 102, 29)
            End Get
        End Property

        ''' <summary>Gets Chocolate4.</summary>
        Public Shared ReadOnly Property Chocolate4() As Color
            Get
                Return Color.FromRgb(139, 69, 19)
            End Get
        End Property

        ''' <summary>Gets Coral.</summary>
        Public Shared ReadOnly Property Coral() As Color
            Get
                Return Color.FromRgb(255, 127, 80)
            End Get
        End Property

        ''' <summary>Gets Coral1.</summary>
        Public Shared ReadOnly Property Coral1() As Color
            Get
                Return Color.FromRgb(255, 114, 86)
            End Get
        End Property

        ''' <summary>Gets Coral2.</summary>
        Public Shared ReadOnly Property Coral2() As Color
            Get
                Return Color.FromRgb(238, 106, 80)
            End Get
        End Property

        ''' <summary>Gets Coral3.</summary>
        Public Shared ReadOnly Property Coral3() As Color
            Get
                Return Color.FromRgb(205, 91, 69)
            End Get
        End Property

        ''' <summary>Gets Coral4.</summary>
        Public Shared ReadOnly Property Coral4() As Color
            Get
                Return Color.FromRgb(139, 62, 47)
            End Get
        End Property

        ''' <summary>Gets CornflowerBlue.</summary>
        Public Shared ReadOnly Property CornflowerBlue() As Color
            Get
                Return Color.FromRgb(100, 149, 237)
            End Get
        End Property

        ''' <summary>Gets Cornsilk.</summary>
        Public Shared ReadOnly Property Cornsilk() As Color
            Get
                Return Color.FromRgb(255, 248, 220)
            End Get
        End Property

        ''' <summary>Gets Cornsilk1.</summary>
        Public Shared ReadOnly Property Cornsilk1() As Color
            Get
                Return Color.FromRgb(255, 248, 220)
            End Get
        End Property

        ''' <summary>Gets Cornsilk2.</summary>
        Public Shared ReadOnly Property Cornsilk2() As Color
            Get
                Return Color.FromRgb(238, 232, 205)
            End Get
        End Property

        ''' <summary>Gets Cornsilk3.</summary>
        Public Shared ReadOnly Property Cornsilk3() As Color
            Get
                Return Color.FromRgb(205, 200, 177)
            End Get
        End Property

        ''' <summary>Gets Cornsilk4.</summary>
        Public Shared ReadOnly Property Cornsilk4() As Color
            Get
                Return Color.FromRgb(139, 136, 120)
            End Get
        End Property

        ''' <summary>Gets Cyan.</summary>
        Public Shared ReadOnly Property Cyan() As Color
            Get
                Return Color.FromRgb(0, 255, 255)
            End Get
        End Property

        ''' <summary>Gets Cyan1.</summary>
        Public Shared ReadOnly Property Cyan1() As Color
            Get
                Return Color.FromRgb(0, 255, 255)
            End Get
        End Property

        ''' <summary>Gets Cyan2.</summary>
        Public Shared ReadOnly Property Cyan2() As Color
            Get
                Return Color.FromRgb(0, 238, 238)
            End Get
        End Property

        ''' <summary>Gets Cyan3.</summary>
        Public Shared ReadOnly Property Cyan3() As Color
            Get
                Return Color.FromRgb(0, 205, 205)
            End Get
        End Property

        ''' <summary>Gets Cyan4.</summary>
        Public Shared ReadOnly Property Cyan4() As Color
            Get
                Return Color.FromRgb(0, 139, 139)
            End Get
        End Property

        ''' <summary>Gets DarkBlue.</summary>
        Public Shared ReadOnly Property DarkBlue() As Color
            Get
                Return Color.FromRgb(0, 0, 139)
            End Get
        End Property

        ''' <summary>Gets DarkCyan.</summary>
        Public Shared ReadOnly Property DarkCyan() As Color
            Get
                Return Color.FromRgb(0, 139, 139)
            End Get
        End Property

        ''' <summary>Gets DarkGoldenrod.</summary>
        Public Shared ReadOnly Property DarkGoldenrod() As Color
            Get
                Return Color.FromRgb(184, 134, 11)
            End Get
        End Property

        ''' <summary>Gets DarkGoldenrod1.</summary>
        Public Shared ReadOnly Property DarkGoldenrod1() As Color
            Get
                Return Color.FromRgb(255, 185, 15)
            End Get
        End Property

        ''' <summary>Gets DarkGoldenrod2.</summary>
        Public Shared ReadOnly Property DarkGoldenrod2() As Color
            Get
                Return Color.FromRgb(238, 173, 14)
            End Get
        End Property

        ''' <summary>Gets DarkGoldenrod3.</summary>
        Public Shared ReadOnly Property DarkGoldenrod3() As Color
            Get
                Return Color.FromRgb(205, 149, 12)
            End Get
        End Property

        ''' <summary>Gets DarkGoldenrod4.</summary>
        Public Shared ReadOnly Property DarkGoldenrod4() As Color
            Get
                Return Color.FromRgb(139, 101, 8)
            End Get
        End Property

        ''' <summary>Gets DarkGray.</summary>
        Public Shared ReadOnly Property DarkGray() As Color
            Get
                Return Color.FromRgb(169, 169, 169)
            End Get
        End Property

        ''' <summary>Gets DarkGreen.</summary>
        Public Shared ReadOnly Property DarkGreen() As Color
            Get
                Return Color.FromRgb(0, 100, 0)
            End Get
        End Property

        ''' <summary>Gets DarkGrey.</summary>
        Public Shared ReadOnly Property DarkGrey() As Color
            Get
                Return Color.FromRgb(169, 169, 169)
            End Get
        End Property

        ''' <summary>Gets DarkKhaki.</summary>
        Public Shared ReadOnly Property DarkKhaki() As Color
            Get
                Return Color.FromRgb(189, 183, 107)
            End Get
        End Property

        ''' <summary>Gets DarkMagenta.</summary>
        Public Shared ReadOnly Property DarkMagenta() As Color
            Get
                Return Color.FromRgb(139, 0, 139)
            End Get
        End Property

        ''' <summary>Gets DarkOliveGreen.</summary>
        Public Shared ReadOnly Property DarkOliveGreen() As Color
            Get
                Return Color.FromRgb(85, 107, 47)
            End Get
        End Property

        ''' <summary>Gets DarkOliveGreen1.</summary>
        Public Shared ReadOnly Property DarkOliveGreen1() As Color
            Get
                Return Color.FromRgb(202, 255, 112)
            End Get
        End Property

        ''' <summary>Gets DarkOliveGreen2.</summary>
        Public Shared ReadOnly Property DarkOliveGreen2() As Color
            Get
                Return Color.FromRgb(188, 238, 104)
            End Get
        End Property

        ''' <summary>Gets DarkOliveGreen3.</summary>
        Public Shared ReadOnly Property DarkOliveGreen3() As Color
            Get
                Return Color.FromRgb(162, 205, 90)
            End Get
        End Property

        ''' <summary>Gets DarkOliveGreen4.</summary>
        Public Shared ReadOnly Property DarkOliveGreen4() As Color
            Get
                Return Color.FromRgb(110, 139, 61)
            End Get
        End Property

        ''' <summary>Gets DarkOrange.</summary>
        Public Shared ReadOnly Property DarkOrange() As Color
            Get
                Return Color.FromRgb(255, 140, 0)
            End Get
        End Property

        ''' <summary>Gets DarkOrange1.</summary>
        Public Shared ReadOnly Property DarkOrange1() As Color
            Get
                Return Color.FromRgb(255, 127, 0)
            End Get
        End Property

        ''' <summary>Gets DarkOrange2.</summary>
        Public Shared ReadOnly Property DarkOrange2() As Color
            Get
                Return Color.FromRgb(238, 118, 0)
            End Get
        End Property

        ''' <summary>Gets DarkOrange3.</summary>
        Public Shared ReadOnly Property DarkOrange3() As Color
            Get
                Return Color.FromRgb(205, 102, 0)
            End Get
        End Property

        ''' <summary>Gets DarkOrange4.</summary>
        Public Shared ReadOnly Property DarkOrange4() As Color
            Get
                Return Color.FromRgb(139, 69, 0)
            End Get
        End Property

        ''' <summary>Gets DarkOrchid.</summary>
        Public Shared ReadOnly Property DarkOrchid() As Color
            Get
                Return Color.FromRgb(153, 50, 204)
            End Get
        End Property

        ''' <summary>Gets DarkOrchid1.</summary>
        Public Shared ReadOnly Property DarkOrchid1() As Color
            Get
                Return Color.FromRgb(191, 62, 255)
            End Get
        End Property

        ''' <summary>Gets DarkOrchid2.</summary>
        Public Shared ReadOnly Property DarkOrchid2() As Color
            Get
                Return Color.FromRgb(178, 58, 238)
            End Get
        End Property

        ''' <summary>Gets DarkOrchid3.</summary>
        Public Shared ReadOnly Property DarkOrchid3() As Color
            Get
                Return Color.FromRgb(154, 50, 205)
            End Get
        End Property

        ''' <summary>Gets DarkOrchid4.</summary>
        Public Shared ReadOnly Property DarkOrchid4() As Color
            Get
                Return Color.FromRgb(104, 34, 139)
            End Get
        End Property

        ''' <summary>Gets DarkRed.</summary>
        Public Shared ReadOnly Property DarkRed() As Color
            Get
                Return Color.FromRgb(139, 0, 0)
            End Get
        End Property

        ''' <summary>Gets DarkSalmon.</summary>
        Public Shared ReadOnly Property DarkSalmon() As Color
            Get
                Return Color.FromRgb(233, 150, 122)
            End Get
        End Property

        ''' <summary>Gets DarkSeaGreen.</summary>
        Public Shared ReadOnly Property DarkSeaGreen() As Color
            Get
                Return Color.FromRgb(143, 188, 143)
            End Get
        End Property

        ''' <summary>Gets DarkSeaGreen1.</summary>
        Public Shared ReadOnly Property DarkSeaGreen1() As Color
            Get
                Return Color.FromRgb(193, 255, 193)
            End Get
        End Property

        ''' <summary>Gets DarkSeaGreen2.</summary>
        Public Shared ReadOnly Property DarkSeaGreen2() As Color
            Get
                Return Color.FromRgb(180, 238, 180)
            End Get
        End Property

        ''' <summary>Gets DarkSeaGreen3.</summary>
        Public Shared ReadOnly Property DarkSeaGreen3() As Color
            Get
                Return Color.FromRgb(155, 205, 155)
            End Get
        End Property

        ''' <summary>Gets DarkSeaGreen4.</summary>
        Public Shared ReadOnly Property DarkSeaGreen4() As Color
            Get
                Return Color.FromRgb(105, 139, 105)
            End Get
        End Property

        ''' <summary>Gets DarkSlateBlue.</summary>
        Public Shared ReadOnly Property DarkSlateBlue() As Color
            Get
                Return Color.FromRgb(72, 61, 139)
            End Get
        End Property

        ''' <summary>Gets DarkSlateGray.</summary>
        Public Shared ReadOnly Property DarkSlateGray() As Color
            Get
                Return Color.FromRgb(47, 79, 79)
            End Get
        End Property

        ''' <summary>Gets DarkSlateGray1.</summary>
        Public Shared ReadOnly Property DarkSlateGray1() As Color
            Get
                Return Color.FromRgb(151, 255, 255)
            End Get
        End Property

        ''' <summary>Gets DarkSlateGray2.</summary>
        Public Shared ReadOnly Property DarkSlateGray2() As Color
            Get
                Return Color.FromRgb(141, 238, 238)
            End Get
        End Property

        ''' <summary>Gets DarkSlateGray3.</summary>
        Public Shared ReadOnly Property DarkSlateGray3() As Color
            Get
                Return Color.FromRgb(121, 205, 205)
            End Get
        End Property

        ''' <summary>Gets DarkSlateGray4.</summary>
        Public Shared ReadOnly Property DarkSlateGray4() As Color
            Get
                Return Color.FromRgb(82, 139, 139)
            End Get
        End Property

        ''' <summary>Gets DarkSlateGrey.</summary>
        Public Shared ReadOnly Property DarkSlateGrey() As Color
            Get
                Return Color.FromRgb(47, 79, 79)
            End Get
        End Property

        ''' <summary>Gets DarkTurquoise.</summary>
        Public Shared ReadOnly Property DarkTurquoise() As Color
            Get
                Return Color.FromRgb(0, 206, 209)
            End Get
        End Property

        ''' <summary>Gets DarkViolet.</summary>
        Public Shared ReadOnly Property DarkViolet() As Color
            Get
                Return Color.FromRgb(148, 0, 211)
            End Get
        End Property

        ''' <summary>Gets DeepPink.</summary>
        Public Shared ReadOnly Property DeepPink() As Color
            Get
                Return Color.FromRgb(255, 20, 147)
            End Get
        End Property

        ''' <summary>Gets DeepPink1.</summary>
        Public Shared ReadOnly Property DeepPink1() As Color
            Get
                Return Color.FromRgb(255, 20, 147)
            End Get
        End Property

        ''' <summary>Gets DeepPink2.</summary>
        Public Shared ReadOnly Property DeepPink2() As Color
            Get
                Return Color.FromRgb(238, 18, 137)
            End Get
        End Property

        ''' <summary>Gets DeepPink3.</summary>
        Public Shared ReadOnly Property DeepPink3() As Color
            Get
                Return Color.FromRgb(205, 16, 118)
            End Get
        End Property

        ''' <summary>Gets DeepPink4.</summary>
        Public Shared ReadOnly Property DeepPink4() As Color
            Get
                Return Color.FromRgb(139, 10, 80)
            End Get
        End Property

        ''' <summary>Gets DeepSkyBlue.</summary>
        Public Shared ReadOnly Property DeepSkyBlue() As Color
            Get
                Return Color.FromRgb(0, 191, 255)
            End Get
        End Property

        ''' <summary>Gets DeepSkyBlue1.</summary>
        Public Shared ReadOnly Property DeepSkyBlue1() As Color
            Get
                Return Color.FromRgb(0, 191, 255)
            End Get
        End Property

        ''' <summary>Gets DeepSkyBlue2.</summary>
        Public Shared ReadOnly Property DeepSkyBlue2() As Color
            Get
                Return Color.FromRgb(0, 178, 238)
            End Get
        End Property

        ''' <summary>Gets DeepSkyBlue3.</summary>
        Public Shared ReadOnly Property DeepSkyBlue3() As Color
            Get
                Return Color.FromRgb(0, 154, 205)
            End Get
        End Property

        ''' <summary>Gets DeepSkyBlue4.</summary>
        Public Shared ReadOnly Property DeepSkyBlue4() As Color
            Get
                Return Color.FromRgb(0, 104, 139)
            End Get
        End Property

        ''' <summary>Gets DimGray.</summary>
        Public Shared ReadOnly Property DimGray() As Color
            Get
                Return Color.FromRgb(105, 105, 105)
            End Get
        End Property

        ''' <summary>Gets DimGrey.</summary>
        Public Shared ReadOnly Property DimGrey() As Color
            Get
                Return Color.FromRgb(105, 105, 105)
            End Get
        End Property

        ''' <summary>Gets DodgerBlue.</summary>
        Public Shared ReadOnly Property DodgerBlue() As Color
            Get
                Return Color.FromRgb(30, 144, 255)
            End Get
        End Property

        ''' <summary>Gets DodgerBlue1.</summary>
        Public Shared ReadOnly Property DodgerBlue1() As Color
            Get
                Return Color.FromRgb(30, 144, 255)
            End Get
        End Property

        ''' <summary>Gets DodgerBlue2.</summary>
        Public Shared ReadOnly Property DodgerBlue2() As Color
            Get
                Return Color.FromRgb(28, 134, 238)
            End Get
        End Property

        ''' <summary>Gets DodgerBlue3.</summary>
        Public Shared ReadOnly Property DodgerBlue3() As Color
            Get
                Return Color.FromRgb(24, 116, 205)
            End Get
        End Property

        ''' <summary>Gets DodgerBlue4.</summary>
        Public Shared ReadOnly Property DodgerBlue4() As Color
            Get
                Return Color.FromRgb(16, 78, 139)
            End Get
        End Property

        ''' <summary>Gets Firebrick.</summary>
        Public Shared ReadOnly Property Firebrick() As Color
            Get
                Return Color.FromRgb(178, 34, 34)
            End Get
        End Property

        ''' <summary>Gets Firebrick1.</summary>
        Public Shared ReadOnly Property Firebrick1() As Color
            Get
                Return Color.FromRgb(255, 48, 48)
            End Get
        End Property

        ''' <summary>Gets Firebrick2.</summary>
        Public Shared ReadOnly Property Firebrick2() As Color
            Get
                Return Color.FromRgb(238, 44, 44)
            End Get
        End Property

        ''' <summary>Gets Firebrick3.</summary>
        Public Shared ReadOnly Property Firebrick3() As Color
            Get
                Return Color.FromRgb(205, 38, 38)
            End Get
        End Property

        ''' <summary>Gets Firebrick4.</summary>
        Public Shared ReadOnly Property Firebrick4() As Color
            Get
                Return Color.FromRgb(139, 26, 26)
            End Get
        End Property

        ''' <summary>Gets FloralWhite.</summary>
        Public Shared ReadOnly Property FloralWhite() As Color
            Get
                Return Color.FromRgb(255, 250, 240)
            End Get
        End Property

        ''' <summary>Gets ForestGreen.</summary>
        Public Shared ReadOnly Property ForestGreen() As Color
            Get
                Return Color.FromRgb(34, 139, 34)
            End Get
        End Property

        ''' <summary>Gets Gainsboro.</summary>
        Public Shared ReadOnly Property Gainsboro() As Color
            Get
                Return Color.FromRgb(220, 220, 220)
            End Get
        End Property

        ''' <summary>Gets GhostWhite.</summary>
        Public Shared ReadOnly Property GhostWhite() As Color
            Get
                Return Color.FromRgb(248, 248, 255)
            End Get
        End Property

        ''' <summary>Gets Gold.</summary>
        Public Shared ReadOnly Property Gold() As Color
            Get
                Return Color.FromRgb(255, 215, 0)
            End Get
        End Property

        ''' <summary>Gets Gold1.</summary>
        Public Shared ReadOnly Property Gold1() As Color
            Get
                Return Color.FromRgb(255, 215, 0)
            End Get
        End Property

        ''' <summary>Gets Gold2.</summary>
        Public Shared ReadOnly Property Gold2() As Color
            Get
                Return Color.FromRgb(238, 201, 0)
            End Get
        End Property

        ''' <summary>Gets Gold3.</summary>
        Public Shared ReadOnly Property Gold3() As Color
            Get
                Return Color.FromRgb(205, 173, 0)
            End Get
        End Property

        ''' <summary>Gets Gold4.</summary>
        Public Shared ReadOnly Property Gold4() As Color
            Get
                Return Color.FromRgb(139, 117, 0)
            End Get
        End Property

        ''' <summary>Gets Goldenrod.</summary>
        Public Shared ReadOnly Property Goldenrod() As Color
            Get
                Return Color.FromRgb(218, 165, 32)
            End Get
        End Property

        ''' <summary>Gets Goldenrod1.</summary>
        Public Shared ReadOnly Property Goldenrod1() As Color
            Get
                Return Color.FromRgb(255, 193, 37)
            End Get
        End Property

        ''' <summary>Gets Goldenrod2.</summary>
        Public Shared ReadOnly Property Goldenrod2() As Color
            Get
                Return Color.FromRgb(238, 180, 34)
            End Get
        End Property

        ''' <summary>Gets Goldenrod3.</summary>
        Public Shared ReadOnly Property Goldenrod3() As Color
            Get
                Return Color.FromRgb(205, 155, 29)
            End Get
        End Property

        ''' <summary>Gets Goldenrod4.</summary>
        Public Shared ReadOnly Property Goldenrod4() As Color
            Get
                Return Color.FromRgb(139, 105, 20)
            End Get
        End Property

        ''' <summary>Gets Gray.</summary>
        Public Shared ReadOnly Property Gray() As Color
            Get
                Return Color.FromRgb(190, 190, 190)
            End Get
        End Property

        ''' <summary>Gets Gray0.</summary>
        Public Shared ReadOnly Property Gray0() As Color
            Get
                Return Color.FromRgb(0, 0, 0)
            End Get
        End Property

        ''' <summary>Gets Gray1.</summary>
        Public Shared ReadOnly Property Gray1() As Color
            Get
                Return Color.FromRgb(3, 3, 3)
            End Get
        End Property

        ''' <summary>Gets Gray10.</summary>
        Public Shared ReadOnly Property Gray10() As Color
            Get
                Return Color.FromRgb(26, 26, 26)
            End Get
        End Property

        ''' <summary>Gets Gray100.</summary>
        Public Shared ReadOnly Property Gray100() As Color
            Get
                Return Color.FromRgb(255, 255, 255)
            End Get
        End Property

        ''' <summary>Gets Gray11.</summary>
        Public Shared ReadOnly Property Gray11() As Color
            Get
                Return Color.FromRgb(28, 28, 28)
            End Get
        End Property

        ''' <summary>Gets Gray12.</summary>
        Public Shared ReadOnly Property Gray12() As Color
            Get
                Return Color.FromRgb(31, 31, 31)
            End Get
        End Property

        ''' <summary>Gets Gray13.</summary>
        Public Shared ReadOnly Property Gray13() As Color
            Get
                Return Color.FromRgb(33, 33, 33)
            End Get
        End Property

        ''' <summary>Gets Gray14.</summary>
        Public Shared ReadOnly Property Gray14() As Color
            Get
                Return Color.FromRgb(36, 36, 36)
            End Get
        End Property

        ''' <summary>Gets Gray15.</summary>
        Public Shared ReadOnly Property Gray15() As Color
            Get
                Return Color.FromRgb(38, 38, 38)
            End Get
        End Property

        ''' <summary>Gets Gray16.</summary>
        Public Shared ReadOnly Property Gray16() As Color
            Get
                Return Color.FromRgb(41, 41, 41)
            End Get
        End Property

        ''' <summary>Gets Gray17.</summary>
        Public Shared ReadOnly Property Gray17() As Color
            Get
                Return Color.FromRgb(43, 43, 43)
            End Get
        End Property

        ''' <summary>Gets Gray18.</summary>
        Public Shared ReadOnly Property Gray18() As Color
            Get
                Return Color.FromRgb(46, 46, 46)
            End Get
        End Property

        ''' <summary>Gets Gray19.</summary>
        Public Shared ReadOnly Property Gray19() As Color
            Get
                Return Color.FromRgb(48, 48, 48)
            End Get
        End Property

        ''' <summary>Gets Gray2.</summary>
        Public Shared ReadOnly Property Gray2() As Color
            Get
                Return Color.FromRgb(5, 5, 5)
            End Get
        End Property

        ''' <summary>Gets Gray20.</summary>
        Public Shared ReadOnly Property Gray20() As Color
            Get
                Return Color.FromRgb(51, 51, 51)
            End Get
        End Property

        ''' <summary>Gets Gray21.</summary>
        Public Shared ReadOnly Property Gray21() As Color
            Get
                Return Color.FromRgb(54, 54, 54)
            End Get
        End Property

        ''' <summary>Gets Gray22.</summary>
        Public Shared ReadOnly Property Gray22() As Color
            Get
                Return Color.FromRgb(56, 56, 56)
            End Get
        End Property

        ''' <summary>Gets Gray23.</summary>
        Public Shared ReadOnly Property Gray23() As Color
            Get
                Return Color.FromRgb(59, 59, 59)
            End Get
        End Property

        ''' <summary>Gets Gray24.</summary>
        Public Shared ReadOnly Property Gray24() As Color
            Get
                Return Color.FromRgb(61, 61, 61)
            End Get
        End Property

        ''' <summary>Gets Gray25.</summary>
        Public Shared ReadOnly Property Gray25() As Color
            Get
                Return Color.FromRgb(64, 64, 64)
            End Get
        End Property

        ''' <summary>Gets Gray26.</summary>
        Public Shared ReadOnly Property Gray26() As Color
            Get
                Return Color.FromRgb(66, 66, 66)
            End Get
        End Property

        ''' <summary>Gets Gray27.</summary>
        Public Shared ReadOnly Property Gray27() As Color
            Get
                Return Color.FromRgb(69, 69, 69)
            End Get
        End Property

        ''' <summary>Gets Gray28.</summary>
        Public Shared ReadOnly Property Gray28() As Color
            Get
                Return Color.FromRgb(71, 71, 71)
            End Get
        End Property

        ''' <summary>Gets Gray29.</summary>
        Public Shared ReadOnly Property Gray29() As Color
            Get
                Return Color.FromRgb(74, 74, 74)
            End Get
        End Property

        ''' <summary>Gets Gray3.</summary>
        Public Shared ReadOnly Property Gray3() As Color
            Get
                Return Color.FromRgb(8, 8, 8)
            End Get
        End Property

        ''' <summary>Gets Gray30.</summary>
        Public Shared ReadOnly Property Gray30() As Color
            Get
                Return Color.FromRgb(77, 77, 77)
            End Get
        End Property

        ''' <summary>Gets Gray31.</summary>
        Public Shared ReadOnly Property Gray31() As Color
            Get
                Return Color.FromRgb(79, 79, 79)
            End Get
        End Property

        ''' <summary>Gets Gray32.</summary>
        Public Shared ReadOnly Property Gray32() As Color
            Get
                Return Color.FromRgb(82, 82, 82)
            End Get
        End Property

        ''' <summary>Gets Gray33.</summary>
        Public Shared ReadOnly Property Gray33() As Color
            Get
                Return Color.FromRgb(84, 84, 84)
            End Get
        End Property

        ''' <summary>Gets Gray34.</summary>
        Public Shared ReadOnly Property Gray34() As Color
            Get
                Return Color.FromRgb(87, 87, 87)
            End Get
        End Property

        ''' <summary>Gets Gray35.</summary>
        Public Shared ReadOnly Property Gray35() As Color
            Get
                Return Color.FromRgb(89, 89, 89)
            End Get
        End Property

        ''' <summary>Gets Gray36.</summary>
        Public Shared ReadOnly Property Gray36() As Color
            Get
                Return Color.FromRgb(92, 92, 92)
            End Get
        End Property

        ''' <summary>Gets Gray37.</summary>
        Public Shared ReadOnly Property Gray37() As Color
            Get
                Return Color.FromRgb(94, 94, 94)
            End Get
        End Property

        ''' <summary>Gets Gray38.</summary>
        Public Shared ReadOnly Property Gray38() As Color
            Get
                Return Color.FromRgb(97, 97, 97)
            End Get
        End Property

        ''' <summary>Gets Gray39.</summary>
        Public Shared ReadOnly Property Gray39() As Color
            Get
                Return Color.FromRgb(99, 99, 99)
            End Get
        End Property

        ''' <summary>Gets Gray4.</summary>
        Public Shared ReadOnly Property Gray4() As Color
            Get
                Return Color.FromRgb(10, 10, 10)
            End Get
        End Property

        ''' <summary>Gets Gray40.</summary>
        Public Shared ReadOnly Property Gray40() As Color
            Get
                Return Color.FromRgb(102, 102, 102)
            End Get
        End Property

        ''' <summary>Gets Gray41.</summary>
        Public Shared ReadOnly Property Gray41() As Color
            Get
                Return Color.FromRgb(105, 105, 105)
            End Get
        End Property

        ''' <summary>Gets Gray42.</summary>
        Public Shared ReadOnly Property Gray42() As Color
            Get
                Return Color.FromRgb(107, 107, 107)
            End Get
        End Property

        ''' <summary>Gets Gray43.</summary>
        Public Shared ReadOnly Property Gray43() As Color
            Get
                Return Color.FromRgb(110, 110, 110)
            End Get
        End Property

        ''' <summary>Gets Gray44.</summary>
        Public Shared ReadOnly Property Gray44() As Color
            Get
                Return Color.FromRgb(112, 112, 112)
            End Get
        End Property

        ''' <summary>Gets Gray45.</summary>
        Public Shared ReadOnly Property Gray45() As Color
            Get
                Return Color.FromRgb(115, 115, 115)
            End Get
        End Property

        ''' <summary>Gets Gray46.</summary>
        Public Shared ReadOnly Property Gray46() As Color
            Get
                Return Color.FromRgb(117, 117, 117)
            End Get
        End Property

        ''' <summary>Gets Gray47.</summary>
        Public Shared ReadOnly Property Gray47() As Color
            Get
                Return Color.FromRgb(120, 120, 120)
            End Get
        End Property

        ''' <summary>Gets Gray48.</summary>
        Public Shared ReadOnly Property Gray48() As Color
            Get
                Return Color.FromRgb(122, 122, 122)
            End Get
        End Property

        ''' <summary>Gets Gray49.</summary>
        Public Shared ReadOnly Property Gray49() As Color
            Get
                Return Color.FromRgb(125, 125, 125)
            End Get
        End Property

        ''' <summary>Gets Gray5.</summary>
        Public Shared ReadOnly Property Gray5() As Color
            Get
                Return Color.FromRgb(13, 13, 13)
            End Get
        End Property

        ''' <summary>Gets Gray50.</summary>
        Public Shared ReadOnly Property Gray50() As Color
            Get
                Return Color.FromRgb(127, 127, 127)
            End Get
        End Property

        ''' <summary>Gets Gray51.</summary>
        Public Shared ReadOnly Property Gray51() As Color
            Get
                Return Color.FromRgb(130, 130, 130)
            End Get
        End Property

        ''' <summary>Gets Gray52.</summary>
        Public Shared ReadOnly Property Gray52() As Color
            Get
                Return Color.FromRgb(133, 133, 133)
            End Get
        End Property

        ''' <summary>Gets Gray53.</summary>
        Public Shared ReadOnly Property Gray53() As Color
            Get
                Return Color.FromRgb(135, 135, 135)
            End Get
        End Property

        ''' <summary>Gets Gray54.</summary>
        Public Shared ReadOnly Property Gray54() As Color
            Get
                Return Color.FromRgb(138, 138, 138)
            End Get
        End Property

        ''' <summary>Gets Gray55.</summary>
        Public Shared ReadOnly Property Gray55() As Color
            Get
                Return Color.FromRgb(140, 140, 140)
            End Get
        End Property

        ''' <summary>Gets Gray56.</summary>
        Public Shared ReadOnly Property Gray56() As Color
            Get
                Return Color.FromRgb(143, 143, 143)
            End Get
        End Property

        ''' <summary>Gets Gray57.</summary>
        Public Shared ReadOnly Property Gray57() As Color
            Get
                Return Color.FromRgb(145, 145, 145)
            End Get
        End Property

        ''' <summary>Gets Gray58.</summary>
        Public Shared ReadOnly Property Gray58() As Color
            Get
                Return Color.FromRgb(148, 148, 148)
            End Get
        End Property

        ''' <summary>Gets Gray59.</summary>
        Public Shared ReadOnly Property Gray59() As Color
            Get
                Return Color.FromRgb(150, 150, 150)
            End Get
        End Property

        ''' <summary>Gets Gray6.</summary>
        Public Shared ReadOnly Property Gray6() As Color
            Get
                Return Color.FromRgb(15, 15, 15)
            End Get
        End Property

        ''' <summary>Gets Gray60.</summary>
        Public Shared ReadOnly Property Gray60() As Color
            Get
                Return Color.FromRgb(153, 153, 153)
            End Get
        End Property

        ''' <summary>Gets Gray61.</summary>
        Public Shared ReadOnly Property Gray61() As Color
            Get
                Return Color.FromRgb(156, 156, 156)
            End Get
        End Property

        ''' <summary>Gets Gray62.</summary>
        Public Shared ReadOnly Property Gray62() As Color
            Get
                Return Color.FromRgb(158, 158, 158)
            End Get
        End Property

        ''' <summary>Gets Gray63.</summary>
        Public Shared ReadOnly Property Gray63() As Color
            Get
                Return Color.FromRgb(161, 161, 161)
            End Get
        End Property

        ''' <summary>Gets Gray64.</summary>
        Public Shared ReadOnly Property Gray64() As Color
            Get
                Return Color.FromRgb(163, 163, 163)
            End Get
        End Property

        ''' <summary>Gets Gray65.</summary>
        Public Shared ReadOnly Property Gray65() As Color
            Get
                Return Color.FromRgb(166, 166, 166)
            End Get
        End Property

        ''' <summary>Gets Gray66.</summary>
        Public Shared ReadOnly Property Gray66() As Color
            Get
                Return Color.FromRgb(168, 168, 168)
            End Get
        End Property

        ''' <summary>Gets Gray67.</summary>
        Public Shared ReadOnly Property Gray67() As Color
            Get
                Return Color.FromRgb(171, 171, 171)
            End Get
        End Property

        ''' <summary>Gets Gray68.</summary>
        Public Shared ReadOnly Property Gray68() As Color
            Get
                Return Color.FromRgb(173, 173, 173)
            End Get
        End Property

        ''' <summary>Gets Gray69.</summary>
        Public Shared ReadOnly Property Gray69() As Color
            Get
                Return Color.FromRgb(176, 176, 176)
            End Get
        End Property

        ''' <summary>Gets Gray7.</summary>
        Public Shared ReadOnly Property Gray7() As Color
            Get
                Return Color.FromRgb(18, 18, 18)
            End Get
        End Property

        ''' <summary>Gets Gray70.</summary>
        Public Shared ReadOnly Property Gray70() As Color
            Get
                Return Color.FromRgb(179, 179, 179)
            End Get
        End Property

        ''' <summary>Gets Gray71.</summary>
        Public Shared ReadOnly Property Gray71() As Color
            Get
                Return Color.FromRgb(181, 181, 181)
            End Get
        End Property

        ''' <summary>Gets Gray72.</summary>
        Public Shared ReadOnly Property Gray72() As Color
            Get
                Return Color.FromRgb(184, 184, 184)
            End Get
        End Property

        ''' <summary>Gets Gray73.</summary>
        Public Shared ReadOnly Property Gray73() As Color
            Get
                Return Color.FromRgb(186, 186, 186)
            End Get
        End Property

        ''' <summary>Gets Gray74.</summary>
        Public Shared ReadOnly Property Gray74() As Color
            Get
                Return Color.FromRgb(189, 189, 189)
            End Get
        End Property

        ''' <summary>Gets Gray75.</summary>
        Public Shared ReadOnly Property Gray75() As Color
            Get
                Return Color.FromRgb(191, 191, 191)
            End Get
        End Property

        ''' <summary>Gets Gray76.</summary>
        Public Shared ReadOnly Property Gray76() As Color
            Get
                Return Color.FromRgb(194, 194, 194)
            End Get
        End Property

        ''' <summary>Gets Gray77.</summary>
        Public Shared ReadOnly Property Gray77() As Color
            Get
                Return Color.FromRgb(196, 196, 196)
            End Get
        End Property

        ''' <summary>Gets Gray78.</summary>
        Public Shared ReadOnly Property Gray78() As Color
            Get
                Return Color.FromRgb(199, 199, 199)
            End Get
        End Property

        ''' <summary>Gets Gray79.</summary>
        Public Shared ReadOnly Property Gray79() As Color
            Get
                Return Color.FromRgb(201, 201, 201)
            End Get
        End Property

        ''' <summary>Gets Gray8.</summary>
        Public Shared ReadOnly Property Gray8() As Color
            Get
                Return Color.FromRgb(20, 20, 20)
            End Get
        End Property

        ''' <summary>Gets Gray80.</summary>
        Public Shared ReadOnly Property Gray80() As Color
            Get
                Return Color.FromRgb(204, 204, 204)
            End Get
        End Property

        ''' <summary>Gets Gray81.</summary>
        Public Shared ReadOnly Property Gray81() As Color
            Get
                Return Color.FromRgb(207, 207, 207)
            End Get
        End Property

        ''' <summary>Gets Gray82.</summary>
        Public Shared ReadOnly Property Gray82() As Color
            Get
                Return Color.FromRgb(209, 209, 209)
            End Get
        End Property

        ''' <summary>Gets Gray83.</summary>
        Public Shared ReadOnly Property Gray83() As Color
            Get
                Return Color.FromRgb(212, 212, 212)
            End Get
        End Property

        ''' <summary>Gets Gray84.</summary>
        Public Shared ReadOnly Property Gray84() As Color
            Get
                Return Color.FromRgb(214, 214, 214)
            End Get
        End Property

        ''' <summary>Gets Gray85.</summary>
        Public Shared ReadOnly Property Gray85() As Color
            Get
                Return Color.FromRgb(217, 217, 217)
            End Get
        End Property

        ''' <summary>Gets Gray86.</summary>
        Public Shared ReadOnly Property Gray86() As Color
            Get
                Return Color.FromRgb(219, 219, 219)
            End Get
        End Property

        ''' <summary>Gets Gray87.</summary>
        Public Shared ReadOnly Property Gray87() As Color
            Get
                Return Color.FromRgb(222, 222, 222)
            End Get
        End Property

        ''' <summary>Gets Gray88.</summary>
        Public Shared ReadOnly Property Gray88() As Color
            Get
                Return Color.FromRgb(224, 224, 224)
            End Get
        End Property

        ''' <summary>Gets Gray89.</summary>
        Public Shared ReadOnly Property Gray89() As Color
            Get
                Return Color.FromRgb(227, 227, 227)
            End Get
        End Property

        ''' <summary>Gets Gray9.</summary>
        Public Shared ReadOnly Property Gray9() As Color
            Get
                Return Color.FromRgb(23, 23, 23)
            End Get
        End Property

        ''' <summary>Gets Gray90.</summary>
        Public Shared ReadOnly Property Gray90() As Color
            Get
                Return Color.FromRgb(229, 229, 229)
            End Get
        End Property

        ''' <summary>Gets Gray91.</summary>
        Public Shared ReadOnly Property Gray91() As Color
            Get
                Return Color.FromRgb(232, 232, 232)
            End Get
        End Property

        ''' <summary>Gets Gray92.</summary>
        Public Shared ReadOnly Property Gray92() As Color
            Get
                Return Color.FromRgb(235, 235, 235)
            End Get
        End Property

        ''' <summary>Gets Gray93.</summary>
        Public Shared ReadOnly Property Gray93() As Color
            Get
                Return Color.FromRgb(237, 237, 237)
            End Get
        End Property

        ''' <summary>Gets Gray94.</summary>
        Public Shared ReadOnly Property Gray94() As Color
            Get
                Return Color.FromRgb(240, 240, 240)
            End Get
        End Property

        ''' <summary>Gets Gray95.</summary>
        Public Shared ReadOnly Property Gray95() As Color
            Get
                Return Color.FromRgb(242, 242, 242)
            End Get
        End Property

        ''' <summary>Gets Gray96.</summary>
        Public Shared ReadOnly Property Gray96() As Color
            Get
                Return Color.FromRgb(245, 245, 245)
            End Get
        End Property

        ''' <summary>Gets Gray97.</summary>
        Public Shared ReadOnly Property Gray97() As Color
            Get
                Return Color.FromRgb(247, 247, 247)
            End Get
        End Property

        ''' <summary>Gets Gray98.</summary>
        Public Shared ReadOnly Property Gray98() As Color
            Get
                Return Color.FromRgb(250, 250, 250)
            End Get
        End Property

        ''' <summary>Gets Gray99.</summary>
        Public Shared ReadOnly Property Gray99() As Color
            Get
                Return Color.FromRgb(252, 252, 252)
            End Get
        End Property

        ''' <summary>Gets Green.</summary>
        Public Shared ReadOnly Property Green() As Color
            Get
                Return Color.FromRgb(0, 255, 0)
            End Get
        End Property

        ''' <summary>Gets Green1.</summary>
        Public Shared ReadOnly Property Green1() As Color
            Get
                Return Color.FromRgb(0, 255, 0)
            End Get
        End Property

        ''' <summary>Gets Green2.</summary>
        Public Shared ReadOnly Property Green2() As Color
            Get
                Return Color.FromRgb(0, 238, 0)
            End Get
        End Property

        ''' <summary>Gets Green3.</summary>
        Public Shared ReadOnly Property Green3() As Color
            Get
                Return Color.FromRgb(0, 205, 0)
            End Get
        End Property

        ''' <summary>Gets Green4.</summary>
        Public Shared ReadOnly Property Green4() As Color
            Get
                Return Color.FromRgb(0, 139, 0)
            End Get
        End Property

        ''' <summary>Gets GreenYellow.</summary>
        Public Shared ReadOnly Property GreenYellow() As Color
            Get
                Return Color.FromRgb(173, 255, 47)
            End Get
        End Property

        ''' <summary>Gets Grey.</summary>
        Public Shared ReadOnly Property Grey() As Color
            Get
                Return Color.FromRgb(190, 190, 190)
            End Get
        End Property

        ''' <summary>Gets Grey0.</summary>
        Public Shared ReadOnly Property Grey0() As Color
            Get
                Return Color.FromRgb(0, 0, 0)
            End Get
        End Property

        ''' <summary>Gets Grey1.</summary>
        Public Shared ReadOnly Property Grey1() As Color
            Get
                Return Color.FromRgb(3, 3, 3)
            End Get
        End Property

        ''' <summary>Gets Grey10.</summary>
        Public Shared ReadOnly Property Grey10() As Color
            Get
                Return Color.FromRgb(26, 26, 26)
            End Get
        End Property

        ''' <summary>Gets Grey100.</summary>
        Public Shared ReadOnly Property Grey100() As Color
            Get
                Return Color.FromRgb(255, 255, 255)
            End Get
        End Property

        ''' <summary>Gets Grey11.</summary>
        Public Shared ReadOnly Property Grey11() As Color
            Get
                Return Color.FromRgb(28, 28, 28)
            End Get
        End Property

        ''' <summary>Gets Grey12.</summary>
        Public Shared ReadOnly Property Grey12() As Color
            Get
                Return Color.FromRgb(31, 31, 31)
            End Get
        End Property

        ''' <summary>Gets Grey13.</summary>
        Public Shared ReadOnly Property Grey13() As Color
            Get
                Return Color.FromRgb(33, 33, 33)
            End Get
        End Property

        ''' <summary>Gets Grey14.</summary>
        Public Shared ReadOnly Property Grey14() As Color
            Get
                Return Color.FromRgb(36, 36, 36)
            End Get
        End Property

        ''' <summary>Gets Grey15.</summary>
        Public Shared ReadOnly Property Grey15() As Color
            Get
                Return Color.FromRgb(38, 38, 38)
            End Get
        End Property

        ''' <summary>Gets Grey16.</summary>
        Public Shared ReadOnly Property Grey16() As Color
            Get
                Return Color.FromRgb(41, 41, 41)
            End Get
        End Property

        ''' <summary>Gets Grey17.</summary>
        Public Shared ReadOnly Property Grey17() As Color
            Get
                Return Color.FromRgb(43, 43, 43)
            End Get
        End Property

        ''' <summary>Gets Grey18.</summary>
        Public Shared ReadOnly Property Grey18() As Color
            Get
                Return Color.FromRgb(46, 46, 46)
            End Get
        End Property

        ''' <summary>Gets Grey19.</summary>
        Public Shared ReadOnly Property Grey19() As Color
            Get
                Return Color.FromRgb(48, 48, 48)
            End Get
        End Property

        ''' <summary>Gets Grey2.</summary>
        Public Shared ReadOnly Property Grey2() As Color
            Get
                Return Color.FromRgb(5, 5, 5)
            End Get
        End Property

        ''' <summary>Gets Grey20.</summary>
        Public Shared ReadOnly Property Grey20() As Color
            Get
                Return Color.FromRgb(51, 51, 51)
            End Get
        End Property

        ''' <summary>Gets Grey21.</summary>
        Public Shared ReadOnly Property Grey21() As Color
            Get
                Return Color.FromRgb(54, 54, 54)
            End Get
        End Property

        ''' <summary>Gets Grey22.</summary>
        Public Shared ReadOnly Property Grey22() As Color
            Get
                Return Color.FromRgb(56, 56, 56)
            End Get
        End Property

        ''' <summary>Gets Grey23.</summary>
        Public Shared ReadOnly Property Grey23() As Color
            Get
                Return Color.FromRgb(59, 59, 59)
            End Get
        End Property

        ''' <summary>Gets Grey24.</summary>
        Public Shared ReadOnly Property Grey24() As Color
            Get
                Return Color.FromRgb(61, 61, 61)
            End Get
        End Property

        ''' <summary>Gets Grey25.</summary>
        Public Shared ReadOnly Property Grey25() As Color
            Get
                Return Color.FromRgb(64, 64, 64)
            End Get
        End Property

        ''' <summary>Gets Grey26.</summary>
        Public Shared ReadOnly Property Grey26() As Color
            Get
                Return Color.FromRgb(66, 66, 66)
            End Get
        End Property

        ''' <summary>Gets Grey27.</summary>
        Public Shared ReadOnly Property Grey27() As Color
            Get
                Return Color.FromRgb(69, 69, 69)
            End Get
        End Property

        ''' <summary>Gets Grey28.</summary>
        Public Shared ReadOnly Property Grey28() As Color
            Get
                Return Color.FromRgb(71, 71, 71)
            End Get
        End Property

        ''' <summary>Gets Grey29.</summary>
        Public Shared ReadOnly Property Grey29() As Color
            Get
                Return Color.FromRgb(74, 74, 74)
            End Get
        End Property

        ''' <summary>Gets Grey3.</summary>
        Public Shared ReadOnly Property Grey3() As Color
            Get
                Return Color.FromRgb(8, 8, 8)
            End Get
        End Property

        ''' <summary>Gets Grey30.</summary>
        Public Shared ReadOnly Property Grey30() As Color
            Get
                Return Color.FromRgb(77, 77, 77)
            End Get
        End Property

        ''' <summary>Gets Grey31.</summary>
        Public Shared ReadOnly Property Grey31() As Color
            Get
                Return Color.FromRgb(79, 79, 79)
            End Get
        End Property

        ''' <summary>Gets Grey32.</summary>
        Public Shared ReadOnly Property Grey32() As Color
            Get
                Return Color.FromRgb(82, 82, 82)
            End Get
        End Property

        ''' <summary>Gets Grey33.</summary>
        Public Shared ReadOnly Property Grey33() As Color
            Get
                Return Color.FromRgb(84, 84, 84)
            End Get
        End Property

        ''' <summary>Gets Grey34.</summary>
        Public Shared ReadOnly Property Grey34() As Color
            Get
                Return Color.FromRgb(87, 87, 87)
            End Get
        End Property

        ''' <summary>Gets Grey35.</summary>
        Public Shared ReadOnly Property Grey35() As Color
            Get
                Return Color.FromRgb(89, 89, 89)
            End Get
        End Property

        ''' <summary>Gets Grey36.</summary>
        Public Shared ReadOnly Property Grey36() As Color
            Get
                Return Color.FromRgb(92, 92, 92)
            End Get
        End Property

        ''' <summary>Gets Grey37.</summary>
        Public Shared ReadOnly Property Grey37() As Color
            Get
                Return Color.FromRgb(94, 94, 94)
            End Get
        End Property

        ''' <summary>Gets Grey38.</summary>
        Public Shared ReadOnly Property Grey38() As Color
            Get
                Return Color.FromRgb(97, 97, 97)
            End Get
        End Property

        ''' <summary>Gets Grey39.</summary>
        Public Shared ReadOnly Property Grey39() As Color
            Get
                Return Color.FromRgb(99, 99, 99)
            End Get
        End Property

        ''' <summary>Gets Grey4.</summary>
        Public Shared ReadOnly Property Grey4() As Color
            Get
                Return Color.FromRgb(10, 10, 10)
            End Get
        End Property

        ''' <summary>Gets Grey40.</summary>
        Public Shared ReadOnly Property Grey40() As Color
            Get
                Return Color.FromRgb(102, 102, 102)
            End Get
        End Property

        ''' <summary>Gets Grey41.</summary>
        Public Shared ReadOnly Property Grey41() As Color
            Get
                Return Color.FromRgb(105, 105, 105)
            End Get
        End Property

        ''' <summary>Gets Grey42.</summary>
        Public Shared ReadOnly Property Grey42() As Color
            Get
                Return Color.FromRgb(107, 107, 107)
            End Get
        End Property

        ''' <summary>Gets Grey43.</summary>
        Public Shared ReadOnly Property Grey43() As Color
            Get
                Return Color.FromRgb(110, 110, 110)
            End Get
        End Property

        ''' <summary>Gets Grey44.</summary>
        Public Shared ReadOnly Property Grey44() As Color
            Get
                Return Color.FromRgb(112, 112, 112)
            End Get
        End Property

        ''' <summary>Gets Grey45.</summary>
        Public Shared ReadOnly Property Grey45() As Color
            Get
                Return Color.FromRgb(115, 115, 115)
            End Get
        End Property

        ''' <summary>Gets Grey46.</summary>
        Public Shared ReadOnly Property Grey46() As Color
            Get
                Return Color.FromRgb(117, 117, 117)
            End Get
        End Property

        ''' <summary>Gets Grey47.</summary>
        Public Shared ReadOnly Property Grey47() As Color
            Get
                Return Color.FromRgb(120, 120, 120)
            End Get
        End Property

        ''' <summary>Gets Grey48.</summary>
        Public Shared ReadOnly Property Grey48() As Color
            Get
                Return Color.FromRgb(122, 122, 122)
            End Get
        End Property

        ''' <summary>Gets Grey49.</summary>
        Public Shared ReadOnly Property Grey49() As Color
            Get
                Return Color.FromRgb(125, 125, 125)
            End Get
        End Property

        ''' <summary>Gets Grey5.</summary>
        Public Shared ReadOnly Property Grey5() As Color
            Get
                Return Color.FromRgb(13, 13, 13)
            End Get
        End Property

        ''' <summary>Gets Grey50.</summary>
        Public Shared ReadOnly Property Grey50() As Color
            Get
                Return Color.FromRgb(127, 127, 127)
            End Get
        End Property

        ''' <summary>Gets Grey51.</summary>
        Public Shared ReadOnly Property Grey51() As Color
            Get
                Return Color.FromRgb(130, 130, 130)
            End Get
        End Property

        ''' <summary>Gets Grey52.</summary>
        Public Shared ReadOnly Property Grey52() As Color
            Get
                Return Color.FromRgb(133, 133, 133)
            End Get
        End Property

        ''' <summary>Gets Grey53.</summary>
        Public Shared ReadOnly Property Grey53() As Color
            Get
                Return Color.FromRgb(135, 135, 135)
            End Get
        End Property

        ''' <summary>Gets Grey54.</summary>
        Public Shared ReadOnly Property Grey54() As Color
            Get
                Return Color.FromRgb(138, 138, 138)
            End Get
        End Property

        ''' <summary>Gets Grey55.</summary>
        Public Shared ReadOnly Property Grey55() As Color
            Get
                Return Color.FromRgb(140, 140, 140)
            End Get
        End Property

        ''' <summary>Gets Grey56.</summary>
        Public Shared ReadOnly Property Grey56() As Color
            Get
                Return Color.FromRgb(143, 143, 143)
            End Get
        End Property

        ''' <summary>Gets Grey57.</summary>
        Public Shared ReadOnly Property Grey57() As Color
            Get
                Return Color.FromRgb(145, 145, 145)
            End Get
        End Property

        ''' <summary>Gets Grey58.</summary>
        Public Shared ReadOnly Property Grey58() As Color
            Get
                Return Color.FromRgb(148, 148, 148)
            End Get
        End Property

        ''' <summary>Gets Grey59.</summary>
        Public Shared ReadOnly Property Grey59() As Color
            Get
                Return Color.FromRgb(150, 150, 150)
            End Get
        End Property

        ''' <summary>Gets Grey6.</summary>
        Public Shared ReadOnly Property Grey6() As Color
            Get
                Return Color.FromRgb(15, 15, 15)
            End Get
        End Property

        ''' <summary>Gets Grey60.</summary>
        Public Shared ReadOnly Property Grey60() As Color
            Get
                Return Color.FromRgb(153, 153, 153)
            End Get
        End Property

        ''' <summary>Gets Grey61.</summary>
        Public Shared ReadOnly Property Grey61() As Color
            Get
                Return Color.FromRgb(156, 156, 156)
            End Get
        End Property

        ''' <summary>Gets Grey62.</summary>
        Public Shared ReadOnly Property Grey62() As Color
            Get
                Return Color.FromRgb(158, 158, 158)
            End Get
        End Property

        ''' <summary>Gets Grey63.</summary>
        Public Shared ReadOnly Property Grey63() As Color
            Get
                Return Color.FromRgb(161, 161, 161)
            End Get
        End Property

        ''' <summary>Gets Grey64.</summary>
        Public Shared ReadOnly Property Grey64() As Color
            Get
                Return Color.FromRgb(163, 163, 163)
            End Get
        End Property

        ''' <summary>Gets Grey65.</summary>
        Public Shared ReadOnly Property Grey65() As Color
            Get
                Return Color.FromRgb(166, 166, 166)
            End Get
        End Property

        ''' <summary>Gets Grey66.</summary>
        Public Shared ReadOnly Property Grey66() As Color
            Get
                Return Color.FromRgb(168, 168, 168)
            End Get
        End Property

        ''' <summary>Gets Grey67.</summary>
        Public Shared ReadOnly Property Grey67() As Color
            Get
                Return Color.FromRgb(171, 171, 171)
            End Get
        End Property

        ''' <summary>Gets Grey68.</summary>
        Public Shared ReadOnly Property Grey68() As Color
            Get
                Return Color.FromRgb(173, 173, 173)
            End Get
        End Property

        ''' <summary>Gets Grey69.</summary>
        Public Shared ReadOnly Property Grey69() As Color
            Get
                Return Color.FromRgb(176, 176, 176)
            End Get
        End Property

        ''' <summary>Gets Grey7.</summary>
        Public Shared ReadOnly Property Grey7() As Color
            Get
                Return Color.FromRgb(18, 18, 18)
            End Get
        End Property

        ''' <summary>Gets Grey70.</summary>
        Public Shared ReadOnly Property Grey70() As Color
            Get
                Return Color.FromRgb(179, 179, 179)
            End Get
        End Property

        ''' <summary>Gets Grey71.</summary>
        Public Shared ReadOnly Property Grey71() As Color
            Get
                Return Color.FromRgb(181, 181, 181)
            End Get
        End Property

        ''' <summary>Gets Grey72.</summary>
        Public Shared ReadOnly Property Grey72() As Color
            Get
                Return Color.FromRgb(184, 184, 184)
            End Get
        End Property

        ''' <summary>Gets Grey73.</summary>
        Public Shared ReadOnly Property Grey73() As Color
            Get
                Return Color.FromRgb(186, 186, 186)
            End Get
        End Property

        ''' <summary>Gets Grey74.</summary>
        Public Shared ReadOnly Property Grey74() As Color
            Get
                Return Color.FromRgb(189, 189, 189)
            End Get
        End Property

        ''' <summary>Gets Grey75.</summary>
        Public Shared ReadOnly Property Grey75() As Color
            Get
                Return Color.FromRgb(191, 191, 191)
            End Get
        End Property

        ''' <summary>Gets Grey76.</summary>
        Public Shared ReadOnly Property Grey76() As Color
            Get
                Return Color.FromRgb(194, 194, 194)
            End Get
        End Property

        ''' <summary>Gets Grey77.</summary>
        Public Shared ReadOnly Property Grey77() As Color
            Get
                Return Color.FromRgb(196, 196, 196)
            End Get
        End Property

        ''' <summary>Gets Grey78.</summary>
        Public Shared ReadOnly Property Grey78() As Color
            Get
                Return Color.FromRgb(199, 199, 199)
            End Get
        End Property

        ''' <summary>Gets Grey79.</summary>
        Public Shared ReadOnly Property Grey79() As Color
            Get
                Return Color.FromRgb(201, 201, 201)
            End Get
        End Property

        ''' <summary>Gets Grey8.</summary>
        Public Shared ReadOnly Property Grey8() As Color
            Get
                Return Color.FromRgb(20, 20, 20)
            End Get
        End Property

        ''' <summary>Gets Grey80.</summary>
        Public Shared ReadOnly Property Grey80() As Color
            Get
                Return Color.FromRgb(204, 204, 204)
            End Get
        End Property

        ''' <summary>Gets Grey81.</summary>
        Public Shared ReadOnly Property Grey81() As Color
            Get
                Return Color.FromRgb(207, 207, 207)
            End Get
        End Property

        ''' <summary>Gets Grey82.</summary>
        Public Shared ReadOnly Property Grey82() As Color
            Get
                Return Color.FromRgb(209, 209, 209)
            End Get
        End Property

        ''' <summary>Gets Grey83.</summary>
        Public Shared ReadOnly Property Grey83() As Color
            Get
                Return Color.FromRgb(212, 212, 212)
            End Get
        End Property

        ''' <summary>Gets Grey84.</summary>
        Public Shared ReadOnly Property Grey84() As Color
            Get
                Return Color.FromRgb(214, 214, 214)
            End Get
        End Property

        ''' <summary>Gets Grey85.</summary>
        Public Shared ReadOnly Property Grey85() As Color
            Get
                Return Color.FromRgb(217, 217, 217)
            End Get
        End Property

        ''' <summary>Gets Grey86.</summary>
        Public Shared ReadOnly Property Grey86() As Color
            Get
                Return Color.FromRgb(219, 219, 219)
            End Get
        End Property

        ''' <summary>Gets Grey87.</summary>
        Public Shared ReadOnly Property Grey87() As Color
            Get
                Return Color.FromRgb(222, 222, 222)
            End Get
        End Property

        ''' <summary>Gets Grey88.</summary>
        Public Shared ReadOnly Property Grey88() As Color
            Get
                Return Color.FromRgb(224, 224, 224)
            End Get
        End Property

        ''' <summary>Gets Grey89.</summary>
        Public Shared ReadOnly Property Grey89() As Color
            Get
                Return Color.FromRgb(227, 227, 227)
            End Get
        End Property

        ''' <summary>Gets Grey9.</summary>
        Public Shared ReadOnly Property Grey9() As Color
            Get
                Return Color.FromRgb(23, 23, 23)
            End Get
        End Property

        ''' <summary>Gets Grey90.</summary>
        Public Shared ReadOnly Property Grey90() As Color
            Get
                Return Color.FromRgb(229, 229, 229)
            End Get
        End Property

        ''' <summary>Gets Grey91.</summary>
        Public Shared ReadOnly Property Grey91() As Color
            Get
                Return Color.FromRgb(232, 232, 232)
            End Get
        End Property

        ''' <summary>Gets Grey92.</summary>
        Public Shared ReadOnly Property Grey92() As Color
            Get
                Return Color.FromRgb(235, 235, 235)
            End Get
        End Property

        ''' <summary>Gets Grey93.</summary>
        Public Shared ReadOnly Property Grey93() As Color
            Get
                Return Color.FromRgb(237, 237, 237)
            End Get
        End Property

        ''' <summary>Gets Grey94.</summary>
        Public Shared ReadOnly Property Grey94() As Color
            Get
                Return Color.FromRgb(240, 240, 240)
            End Get
        End Property

        ''' <summary>Gets Grey95.</summary>
        Public Shared ReadOnly Property Grey95() As Color
            Get
                Return Color.FromRgb(242, 242, 242)
            End Get
        End Property

        ''' <summary>Gets Grey96.</summary>
        Public Shared ReadOnly Property Grey96() As Color
            Get
                Return Color.FromRgb(245, 245, 245)
            End Get
        End Property

        ''' <summary>Gets Grey97.</summary>
        Public Shared ReadOnly Property Grey97() As Color
            Get
                Return Color.FromRgb(247, 247, 247)
            End Get
        End Property

        ''' <summary>Gets Grey98.</summary>
        Public Shared ReadOnly Property Grey98() As Color
            Get
                Return Color.FromRgb(250, 250, 250)
            End Get
        End Property

        ''' <summary>Gets Grey99.</summary>
        Public Shared ReadOnly Property Grey99() As Color
            Get
                Return Color.FromRgb(252, 252, 252)
            End Get
        End Property

        ''' <summary>Gets Honeydew.</summary>
        Public Shared ReadOnly Property Honeydew() As Color
            Get
                Return Color.FromRgb(240, 255, 240)
            End Get
        End Property

        ''' <summary>Gets Honeydew1.</summary>
        Public Shared ReadOnly Property Honeydew1() As Color
            Get
                Return Color.FromRgb(240, 255, 240)
            End Get
        End Property

        ''' <summary>Gets Honeydew2.</summary>
        Public Shared ReadOnly Property Honeydew2() As Color
            Get
                Return Color.FromRgb(224, 238, 224)
            End Get
        End Property

        ''' <summary>Gets Honeydew3.</summary>
        Public Shared ReadOnly Property Honeydew3() As Color
            Get
                Return Color.FromRgb(193, 205, 193)
            End Get
        End Property

        ''' <summary>Gets Honeydew4.</summary>
        Public Shared ReadOnly Property Honeydew4() As Color
            Get
                Return Color.FromRgb(131, 139, 131)
            End Get
        End Property

        ''' <summary>Gets HotPink.</summary>
        Public Shared ReadOnly Property HotPink() As Color
            Get
                Return Color.FromRgb(255, 105, 180)
            End Get
        End Property

        ''' <summary>Gets HotPink1.</summary>
        Public Shared ReadOnly Property HotPink1() As Color
            Get
                Return Color.FromRgb(255, 110, 180)
            End Get
        End Property

        ''' <summary>Gets HotPink2.</summary>
        Public Shared ReadOnly Property HotPink2() As Color
            Get
                Return Color.FromRgb(238, 106, 167)
            End Get
        End Property

        ''' <summary>Gets HotPink3.</summary>
        Public Shared ReadOnly Property HotPink3() As Color
            Get
                Return Color.FromRgb(205, 96, 144)
            End Get
        End Property

        ''' <summary>Gets HotPink4.</summary>
        Public Shared ReadOnly Property HotPink4() As Color
            Get
                Return Color.FromRgb(139, 58, 98)
            End Get
        End Property

        ''' <summary>Gets IndianRed.</summary>
        Public Shared ReadOnly Property IndianRed() As Color
            Get
                Return Color.FromRgb(205, 92, 92)
            End Get
        End Property

        ''' <summary>Gets IndianRed1.</summary>
        Public Shared ReadOnly Property IndianRed1() As Color
            Get
                Return Color.FromRgb(255, 106, 106)
            End Get
        End Property

        ''' <summary>Gets IndianRed2.</summary>
        Public Shared ReadOnly Property IndianRed2() As Color
            Get
                Return Color.FromRgb(238, 99, 99)
            End Get
        End Property

        ''' <summary>Gets IndianRed3.</summary>
        Public Shared ReadOnly Property IndianRed3() As Color
            Get
                Return Color.FromRgb(205, 85, 85)
            End Get
        End Property

        ''' <summary>Gets IndianRed4.</summary>
        Public Shared ReadOnly Property IndianRed4() As Color
            Get
                Return Color.FromRgb(139, 58, 58)
            End Get
        End Property

        ''' <summary>Gets Ivory.</summary>
        Public Shared ReadOnly Property Ivory() As Color
            Get
                Return Color.FromRgb(255, 255, 240)
            End Get
        End Property

        ''' <summary>Gets Ivory1.</summary>
        Public Shared ReadOnly Property Ivory1() As Color
            Get
                Return Color.FromRgb(255, 255, 240)
            End Get
        End Property

        ''' <summary>Gets Ivory2.</summary>
        Public Shared ReadOnly Property Ivory2() As Color
            Get
                Return Color.FromRgb(238, 238, 224)
            End Get
        End Property

        ''' <summary>Gets Ivory3.</summary>
        Public Shared ReadOnly Property Ivory3() As Color
            Get
                Return Color.FromRgb(205, 205, 193)
            End Get
        End Property

        ''' <summary>Gets Ivory4.</summary>
        Public Shared ReadOnly Property Ivory4() As Color
            Get
                Return Color.FromRgb(139, 139, 131)
            End Get
        End Property

        ''' <summary>Gets Khaki.</summary>
        Public Shared ReadOnly Property Khaki() As Color
            Get
                Return Color.FromRgb(240, 230, 140)
            End Get
        End Property

        ''' <summary>Gets Khaki1.</summary>
        Public Shared ReadOnly Property Khaki1() As Color
            Get
                Return Color.FromRgb(255, 246, 143)
            End Get
        End Property

        ''' <summary>Gets Khaki2.</summary>
        Public Shared ReadOnly Property Khaki2() As Color
            Get
                Return Color.FromRgb(238, 230, 133)
            End Get
        End Property

        ''' <summary>Gets Khaki3.</summary>
        Public Shared ReadOnly Property Khaki3() As Color
            Get
                Return Color.FromRgb(205, 198, 115)
            End Get
        End Property

        ''' <summary>Gets Khaki4.</summary>
        Public Shared ReadOnly Property Khaki4() As Color
            Get
                Return Color.FromRgb(139, 134, 78)
            End Get
        End Property

        ''' <summary>Gets Lavender.</summary>
        Public Shared ReadOnly Property Lavender() As Color
            Get
                Return Color.FromRgb(230, 230, 250)
            End Get
        End Property

        ''' <summary>Gets LavenderBlush.</summary>
        Public Shared ReadOnly Property LavenderBlush() As Color
            Get
                Return Color.FromRgb(255, 240, 245)
            End Get
        End Property

        ''' <summary>Gets LavenderBlush1.</summary>
        Public Shared ReadOnly Property LavenderBlush1() As Color
            Get
                Return Color.FromRgb(255, 240, 245)
            End Get
        End Property

        ''' <summary>Gets LavenderBlush2.</summary>
        Public Shared ReadOnly Property LavenderBlush2() As Color
            Get
                Return Color.FromRgb(238, 224, 229)
            End Get
        End Property

        ''' <summary>Gets LavenderBlush3.</summary>
        Public Shared ReadOnly Property LavenderBlush3() As Color
            Get
                Return Color.FromRgb(205, 193, 197)
            End Get
        End Property

        ''' <summary>Gets LavenderBlush4.</summary>
        Public Shared ReadOnly Property LavenderBlush4() As Color
            Get
                Return Color.FromRgb(139, 131, 134)
            End Get
        End Property

        ''' <summary>Gets LawnGreen.</summary>
        Public Shared ReadOnly Property LawnGreen() As Color
            Get
                Return Color.FromRgb(124, 252, 0)
            End Get
        End Property

        ''' <summary>Gets LemonChiffon.</summary>
        Public Shared ReadOnly Property LemonChiffon() As Color
            Get
                Return Color.FromRgb(255, 250, 205)
            End Get
        End Property

        ''' <summary>Gets LemonChiffon1.</summary>
        Public Shared ReadOnly Property LemonChiffon1() As Color
            Get
                Return Color.FromRgb(255, 250, 205)
            End Get
        End Property

        ''' <summary>Gets LemonChiffon2.</summary>
        Public Shared ReadOnly Property LemonChiffon2() As Color
            Get
                Return Color.FromRgb(238, 233, 191)
            End Get
        End Property

        ''' <summary>Gets LemonChiffon3.</summary>
        Public Shared ReadOnly Property LemonChiffon3() As Color
            Get
                Return Color.FromRgb(205, 201, 165)
            End Get
        End Property

        ''' <summary>Gets LemonChiffon4.</summary>
        Public Shared ReadOnly Property LemonChiffon4() As Color
            Get
                Return Color.FromRgb(139, 137, 112)
            End Get
        End Property

        ''' <summary>Gets LightBlue.</summary>
        Public Shared ReadOnly Property LightBlue() As Color
            Get
                Return Color.FromRgb(173, 216, 230)
            End Get
        End Property

        ''' <summary>Gets LightBlue1.</summary>
        Public Shared ReadOnly Property LightBlue1() As Color
            Get
                Return Color.FromRgb(191, 239, 255)
            End Get
        End Property

        ''' <summary>Gets LightBlue2.</summary>
        Public Shared ReadOnly Property LightBlue2() As Color
            Get
                Return Color.FromRgb(178, 223, 238)
            End Get
        End Property

        ''' <summary>Gets LightBlue3.</summary>
        Public Shared ReadOnly Property LightBlue3() As Color
            Get
                Return Color.FromRgb(154, 192, 205)
            End Get
        End Property

        ''' <summary>Gets LightBlue4.</summary>
        Public Shared ReadOnly Property LightBlue4() As Color
            Get
                Return Color.FromRgb(104, 131, 139)
            End Get
        End Property

        ''' <summary>Gets LightCoral.</summary>
        Public Shared ReadOnly Property LightCoral() As Color
            Get
                Return Color.FromRgb(240, 128, 128)
            End Get
        End Property

        ''' <summary>Gets LightCyan.</summary>
        Public Shared ReadOnly Property LightCyan() As Color
            Get
                Return Color.FromRgb(224, 255, 255)
            End Get
        End Property

        ''' <summary>Gets LightCyan1.</summary>
        Public Shared ReadOnly Property LightCyan1() As Color
            Get
                Return Color.FromRgb(224, 255, 255)
            End Get
        End Property

        ''' <summary>Gets LightCyan2.</summary>
        Public Shared ReadOnly Property LightCyan2() As Color
            Get
                Return Color.FromRgb(209, 238, 238)
            End Get
        End Property

        ''' <summary>Gets LightCyan3.</summary>
        Public Shared ReadOnly Property LightCyan3() As Color
            Get
                Return Color.FromRgb(180, 205, 205)
            End Get
        End Property

        ''' <summary>Gets LightCyan4.</summary>
        Public Shared ReadOnly Property LightCyan4() As Color
            Get
                Return Color.FromRgb(122, 139, 139)
            End Get
        End Property

        ''' <summary>Gets LightGoldenrod.</summary>
        Public Shared ReadOnly Property LightGoldenrod() As Color
            Get
                Return Color.FromRgb(238, 221, 130)
            End Get
        End Property

        ''' <summary>Gets LightGoldenrod1.</summary>
        Public Shared ReadOnly Property LightGoldenrod1() As Color
            Get
                Return Color.FromRgb(255, 236, 139)
            End Get
        End Property

        ''' <summary>Gets LightGoldenrod2.</summary>
        Public Shared ReadOnly Property LightGoldenrod2() As Color
            Get
                Return Color.FromRgb(238, 220, 130)
            End Get
        End Property

        ''' <summary>Gets LightGoldenrod3.</summary>
        Public Shared ReadOnly Property LightGoldenrod3() As Color
            Get
                Return Color.FromRgb(205, 190, 112)
            End Get
        End Property

        ''' <summary>Gets LightGoldenrod4.</summary>
        Public Shared ReadOnly Property LightGoldenrod4() As Color
            Get
                Return Color.FromRgb(139, 129, 76)
            End Get
        End Property

        ''' <summary>Gets LightGoldenrodYellow.</summary>
        Public Shared ReadOnly Property LightGoldenrodYellow() As Color
            Get
                Return Color.FromRgb(250, 250, 210)
            End Get
        End Property

        ''' <summary>Gets LightGray.</summary>
        Public Shared ReadOnly Property LightGray() As Color
            Get
                Return Color.FromRgb(211, 211, 211)
            End Get
        End Property

        ''' <summary>Gets LightGreen.</summary>
        Public Shared ReadOnly Property LightGreen() As Color
            Get
                Return Color.FromRgb(144, 238, 144)
            End Get
        End Property

        ''' <summary>Gets LightGrey.</summary>
        Public Shared ReadOnly Property LightGrey() As Color
            Get
                Return Color.FromRgb(211, 211, 211)
            End Get
        End Property

        ''' <summary>Gets LightPink.</summary>
        Public Shared ReadOnly Property LightPink() As Color
            Get
                Return Color.FromRgb(255, 182, 193)
            End Get
        End Property

        ''' <summary>Gets LightPink1.</summary>
        Public Shared ReadOnly Property LightPink1() As Color
            Get
                Return Color.FromRgb(255, 174, 185)
            End Get
        End Property

        ''' <summary>Gets LightPink2.</summary>
        Public Shared ReadOnly Property LightPink2() As Color
            Get
                Return Color.FromRgb(238, 162, 173)
            End Get
        End Property

        ''' <summary>Gets LightPink3.</summary>
        Public Shared ReadOnly Property LightPink3() As Color
            Get
                Return Color.FromRgb(205, 140, 149)
            End Get
        End Property

        ''' <summary>Gets LightPink4.</summary>
        Public Shared ReadOnly Property LightPink4() As Color
            Get
                Return Color.FromRgb(139, 95, 101)
            End Get
        End Property

        ''' <summary>Gets LightSalmon.</summary>
        Public Shared ReadOnly Property LightSalmon() As Color
            Get
                Return Color.FromRgb(255, 160, 122)
            End Get
        End Property

        ''' <summary>Gets LightSalmon1.</summary>
        Public Shared ReadOnly Property LightSalmon1() As Color
            Get
                Return Color.FromRgb(255, 160, 122)
            End Get
        End Property

        ''' <summary>Gets LightSalmon2.</summary>
        Public Shared ReadOnly Property LightSalmon2() As Color
            Get
                Return Color.FromRgb(238, 149, 114)
            End Get
        End Property

        ''' <summary>Gets LightSalmon3.</summary>
        Public Shared ReadOnly Property LightSalmon3() As Color
            Get
                Return Color.FromRgb(205, 129, 98)
            End Get
        End Property

        ''' <summary>Gets LightSalmon4.</summary>
        Public Shared ReadOnly Property LightSalmon4() As Color
            Get
                Return Color.FromRgb(139, 87, 66)
            End Get
        End Property

        ''' <summary>Gets LightSeaGreen.</summary>
        Public Shared ReadOnly Property LightSeaGreen() As Color
            Get
                Return Color.FromRgb(32, 178, 170)
            End Get
        End Property

        ''' <summary>Gets LightSkyBlue.</summary>
        Public Shared ReadOnly Property LightSkyBlue() As Color
            Get
                Return Color.FromRgb(135, 206, 250)
            End Get
        End Property

        ''' <summary>Gets LightSkyBlue1.</summary>
        Public Shared ReadOnly Property LightSkyBlue1() As Color
            Get
                Return Color.FromRgb(176, 226, 255)
            End Get
        End Property

        ''' <summary>Gets LightSkyBlue2.</summary>
        Public Shared ReadOnly Property LightSkyBlue2() As Color
            Get
                Return Color.FromRgb(164, 211, 238)
            End Get
        End Property

        ''' <summary>Gets LightSkyBlue3.</summary>
        Public Shared ReadOnly Property LightSkyBlue3() As Color
            Get
                Return Color.FromRgb(141, 182, 205)
            End Get
        End Property

        ''' <summary>Gets LightSkyBlue4.</summary>
        Public Shared ReadOnly Property LightSkyBlue4() As Color
            Get
                Return Color.FromRgb(96, 123, 139)
            End Get
        End Property

        ''' <summary>Gets LightSlateBlue.</summary>
        Public Shared ReadOnly Property LightSlateBlue() As Color
            Get
                Return Color.FromRgb(132, 112, 255)
            End Get
        End Property

        ''' <summary>Gets LightSlateGray.</summary>
        Public Shared ReadOnly Property LightSlateGray() As Color
            Get
                Return Color.FromRgb(119, 136, 153)
            End Get
        End Property

        ''' <summary>Gets LightSlateGrey.</summary>
        Public Shared ReadOnly Property LightSlateGrey() As Color
            Get
                Return Color.FromRgb(119, 136, 153)
            End Get
        End Property

        ''' <summary>Gets LightSteelBlue.</summary>
        Public Shared ReadOnly Property LightSteelBlue() As Color
            Get
                Return Color.FromRgb(176, 196, 222)
            End Get
        End Property

        ''' <summary>Gets LightSteelBlue1.</summary>
        Public Shared ReadOnly Property LightSteelBlue1() As Color
            Get
                Return Color.FromRgb(202, 225, 255)
            End Get
        End Property

        ''' <summary>Gets LightSteelBlue2.</summary>
        Public Shared ReadOnly Property LightSteelBlue2() As Color
            Get
                Return Color.FromRgb(188, 210, 238)
            End Get
        End Property

        ''' <summary>Gets LightSteelBlue3.</summary>
        Public Shared ReadOnly Property LightSteelBlue3() As Color
            Get
                Return Color.FromRgb(162, 181, 205)
            End Get
        End Property

        ''' <summary>Gets LightSteelBlue4.</summary>
        Public Shared ReadOnly Property LightSteelBlue4() As Color
            Get
                Return Color.FromRgb(110, 123, 139)
            End Get
        End Property

        ''' <summary>Gets LightYellow.</summary>
        Public Shared ReadOnly Property LightYellow() As Color
            Get
                Return Color.FromRgb(255, 255, 224)
            End Get
        End Property

        ''' <summary>Gets LightYellow1.</summary>
        Public Shared ReadOnly Property LightYellow1() As Color
            Get
                Return Color.FromRgb(255, 255, 224)
            End Get
        End Property

        ''' <summary>Gets LightYellow2.</summary>
        Public Shared ReadOnly Property LightYellow2() As Color
            Get
                Return Color.FromRgb(238, 238, 209)
            End Get
        End Property

        ''' <summary>Gets LightYellow3.</summary>
        Public Shared ReadOnly Property LightYellow3() As Color
            Get
                Return Color.FromRgb(205, 205, 180)
            End Get
        End Property

        ''' <summary>Gets LightYellow4.</summary>
        Public Shared ReadOnly Property LightYellow4() As Color
            Get
                Return Color.FromRgb(139, 139, 122)
            End Get
        End Property

        ''' <summary>Gets LimeGreen.</summary>
        Public Shared ReadOnly Property LimeGreen() As Color
            Get
                Return Color.FromRgb(50, 205, 50)
            End Get
        End Property

        ''' <summary>Gets Linen.</summary>
        Public Shared ReadOnly Property Linen() As Color
            Get
                Return Color.FromRgb(250, 240, 230)
            End Get
        End Property

        ''' <summary>Gets Magenta.</summary>
        Public Shared ReadOnly Property Magenta() As Color
            Get
                Return Color.FromRgb(255, 0, 255)
            End Get
        End Property

        ''' <summary>Gets Magenta1.</summary>
        Public Shared ReadOnly Property Magenta1() As Color
            Get
                Return Color.FromRgb(255, 0, 255)
            End Get
        End Property

        ''' <summary>Gets Magenta2.</summary>
        Public Shared ReadOnly Property Magenta2() As Color
            Get
                Return Color.FromRgb(238, 0, 238)
            End Get
        End Property

        ''' <summary>Gets Magenta3.</summary>
        Public Shared ReadOnly Property Magenta3() As Color
            Get
                Return Color.FromRgb(205, 0, 205)
            End Get
        End Property

        ''' <summary>Gets Magenta4.</summary>
        Public Shared ReadOnly Property Magenta4() As Color
            Get
                Return Color.FromRgb(139, 0, 139)
            End Get
        End Property

        ''' <summary>Gets Maroon.</summary>
        Public Shared ReadOnly Property Maroon() As Color
            Get
                Return Color.FromRgb(176, 48, 96)
            End Get
        End Property

        ''' <summary>Gets Maroon1.</summary>
        Public Shared ReadOnly Property Maroon1() As Color
            Get
                Return Color.FromRgb(255, 52, 179)
            End Get
        End Property

        ''' <summary>Gets Maroon2.</summary>
        Public Shared ReadOnly Property Maroon2() As Color
            Get
                Return Color.FromRgb(238, 48, 167)
            End Get
        End Property

        ''' <summary>Gets Maroon3.</summary>
        Public Shared ReadOnly Property Maroon3() As Color
            Get
                Return Color.FromRgb(205, 41, 144)
            End Get
        End Property

        ''' <summary>Gets Maroon4.</summary>
        Public Shared ReadOnly Property Maroon4() As Color
            Get
                Return Color.FromRgb(139, 28, 98)
            End Get
        End Property

        ''' <summary>Gets MediumAquamarine.</summary>
        Public Shared ReadOnly Property MediumAquamarine() As Color
            Get
                Return Color.FromRgb(102, 205, 170)
            End Get
        End Property

        ''' <summary>Gets MediumBlue.</summary>
        Public Shared ReadOnly Property MediumBlue() As Color
            Get
                Return Color.FromRgb(0, 0, 205)
            End Get
        End Property

        ''' <summary>Gets MediumOrchid.</summary>
        Public Shared ReadOnly Property MediumOrchid() As Color
            Get
                Return Color.FromRgb(186, 85, 211)
            End Get
        End Property

        ''' <summary>Gets MediumOrchid1.</summary>
        Public Shared ReadOnly Property MediumOrchid1() As Color
            Get
                Return Color.FromRgb(224, 102, 255)
            End Get
        End Property

        ''' <summary>Gets MediumOrchid2.</summary>
        Public Shared ReadOnly Property MediumOrchid2() As Color
            Get
                Return Color.FromRgb(209, 95, 238)
            End Get
        End Property

        ''' <summary>Gets MediumOrchid3.</summary>
        Public Shared ReadOnly Property MediumOrchid3() As Color
            Get
                Return Color.FromRgb(180, 82, 205)
            End Get
        End Property

        ''' <summary>Gets MediumOrchid4.</summary>
        Public Shared ReadOnly Property MediumOrchid4() As Color
            Get
                Return Color.FromRgb(122, 55, 139)
            End Get
        End Property

        ''' <summary>Gets MediumPurple.</summary>
        Public Shared ReadOnly Property MediumPurple() As Color
            Get
                Return Color.FromRgb(147, 112, 219)
            End Get
        End Property

        ''' <summary>Gets MediumPurple1.</summary>
        Public Shared ReadOnly Property MediumPurple1() As Color
            Get
                Return Color.FromRgb(171, 130, 255)
            End Get
        End Property

        ''' <summary>Gets MediumPurple2.</summary>
        Public Shared ReadOnly Property MediumPurple2() As Color
            Get
                Return Color.FromRgb(159, 121, 238)
            End Get
        End Property

        ''' <summary>Gets MediumPurple3.</summary>
        Public Shared ReadOnly Property MediumPurple3() As Color
            Get
                Return Color.FromRgb(137, 104, 205)
            End Get
        End Property

        ''' <summary>Gets MediumPurple4.</summary>
        Public Shared ReadOnly Property MediumPurple4() As Color
            Get
                Return Color.FromRgb(93, 71, 139)
            End Get
        End Property

        ''' <summary>Gets MediumSeaGreen.</summary>
        Public Shared ReadOnly Property MediumSeaGreen() As Color
            Get
                Return Color.FromRgb(60, 179, 113)
            End Get
        End Property

        ''' <summary>Gets MediumSlateBlue.</summary>
        Public Shared ReadOnly Property MediumSlateBlue() As Color
            Get
                Return Color.FromRgb(123, 104, 238)
            End Get
        End Property

        ''' <summary>Gets MediumSpringGreen.</summary>
        Public Shared ReadOnly Property MediumSpringGreen() As Color
            Get
                Return Color.FromRgb(0, 250, 154)
            End Get
        End Property

        ''' <summary>Gets MediumTurquoise.</summary>
        Public Shared ReadOnly Property MediumTurquoise() As Color
            Get
                Return Color.FromRgb(72, 209, 204)
            End Get
        End Property

        ''' <summary>Gets MediumVioletRed.</summary>
        Public Shared ReadOnly Property MediumVioletRed() As Color
            Get
                Return Color.FromRgb(199, 21, 133)
            End Get
        End Property

        ''' <summary>Gets MidnightBlue.</summary>
        Public Shared ReadOnly Property MidnightBlue() As Color
            Get
                Return Color.FromRgb(25, 25, 112)
            End Get
        End Property

        ''' <summary>Gets MintCream.</summary>
        Public Shared ReadOnly Property MintCream() As Color
            Get
                Return Color.FromRgb(245, 255, 250)
            End Get
        End Property

        ''' <summary>Gets MistyRose.</summary>
        Public Shared ReadOnly Property MistyRose() As Color
            Get
                Return Color.FromRgb(255, 228, 225)
            End Get
        End Property

        ''' <summary>Gets MistyRose1.</summary>
        Public Shared ReadOnly Property MistyRose1() As Color
            Get
                Return Color.FromRgb(255, 228, 225)
            End Get
        End Property

        ''' <summary>Gets MistyRose2.</summary>
        Public Shared ReadOnly Property MistyRose2() As Color
            Get
                Return Color.FromRgb(238, 213, 210)
            End Get
        End Property

        ''' <summary>Gets MistyRose3.</summary>
        Public Shared ReadOnly Property MistyRose3() As Color
            Get
                Return Color.FromRgb(205, 183, 181)
            End Get
        End Property

        ''' <summary>Gets MistyRose4.</summary>
        Public Shared ReadOnly Property MistyRose4() As Color
            Get
                Return Color.FromRgb(139, 125, 123)
            End Get
        End Property

        ''' <summary>Gets Moccasin.</summary>
        Public Shared ReadOnly Property Moccasin() As Color
            Get
                Return Color.FromRgb(255, 228, 181)
            End Get
        End Property

        ''' <summary>Gets NavajoWhite.</summary>
        Public Shared ReadOnly Property NavajoWhite() As Color
            Get
                Return Color.FromRgb(255, 222, 173)
            End Get
        End Property

        ''' <summary>Gets NavajoWhite1.</summary>
        Public Shared ReadOnly Property NavajoWhite1() As Color
            Get
                Return Color.FromRgb(255, 222, 173)
            End Get
        End Property

        ''' <summary>Gets NavajoWhite2.</summary>
        Public Shared ReadOnly Property NavajoWhite2() As Color
            Get
                Return Color.FromRgb(238, 207, 161)
            End Get
        End Property

        ''' <summary>Gets NavajoWhite3.</summary>
        Public Shared ReadOnly Property NavajoWhite3() As Color
            Get
                Return Color.FromRgb(205, 179, 139)
            End Get
        End Property

        ''' <summary>Gets NavajoWhite4.</summary>
        Public Shared ReadOnly Property NavajoWhite4() As Color
            Get
                Return Color.FromRgb(139, 121, 94)
            End Get
        End Property

        ''' <summary>Gets Navy.</summary>
        Public Shared ReadOnly Property Navy() As Color
            Get
                Return Color.FromRgb(0, 0, 128)
            End Get
        End Property

        ''' <summary>Gets NavyBlue.</summary>
        Public Shared ReadOnly Property NavyBlue() As Color
            Get
                Return Color.FromRgb(0, 0, 128)
            End Get
        End Property

        ''' <summary>Gets OldLace.</summary>
        Public Shared ReadOnly Property OldLace() As Color
            Get
                Return Color.FromRgb(253, 245, 230)
            End Get
        End Property

        ''' <summary>Gets OliveDrab.</summary>
        Public Shared ReadOnly Property OliveDrab() As Color
            Get
                Return Color.FromRgb(107, 142, 35)
            End Get
        End Property

        ''' <summary>Gets OliveDrab1.</summary>
        Public Shared ReadOnly Property OliveDrab1() As Color
            Get
                Return Color.FromRgb(192, 255, 62)
            End Get
        End Property

        ''' <summary>Gets OliveDrab2.</summary>
        Public Shared ReadOnly Property OliveDrab2() As Color
            Get
                Return Color.FromRgb(179, 238, 58)
            End Get
        End Property

        ''' <summary>Gets OliveDrab3.</summary>
        Public Shared ReadOnly Property OliveDrab3() As Color
            Get
                Return Color.FromRgb(154, 205, 50)
            End Get
        End Property

        ''' <summary>Gets OliveDrab4.</summary>
        Public Shared ReadOnly Property OliveDrab4() As Color
            Get
                Return Color.FromRgb(105, 139, 34)
            End Get
        End Property

        ''' <summary>Gets Orange.</summary>
        Public Shared ReadOnly Property Orange() As Color
            Get
                Return Color.FromRgb(255, 165, 0)
            End Get
        End Property

        ''' <summary>Gets Orange1.</summary>
        Public Shared ReadOnly Property Orange1() As Color
            Get
                Return Color.FromRgb(255, 165, 0)
            End Get
        End Property

        ''' <summary>Gets Orange2.</summary>
        Public Shared ReadOnly Property Orange2() As Color
            Get
                Return Color.FromRgb(238, 154, 0)
            End Get
        End Property

        ''' <summary>Gets Orange3.</summary>
        Public Shared ReadOnly Property Orange3() As Color
            Get
                Return Color.FromRgb(205, 133, 0)
            End Get
        End Property

        ''' <summary>Gets Orange4.</summary>
        Public Shared ReadOnly Property Orange4() As Color
            Get
                Return Color.FromRgb(139, 90, 0)
            End Get
        End Property

        ''' <summary>Gets OrangeRed.</summary>
        Public Shared ReadOnly Property OrangeRed() As Color
            Get
                Return Color.FromRgb(255, 69, 0)
            End Get
        End Property

        ''' <summary>Gets OrangeRed1.</summary>
        Public Shared ReadOnly Property OrangeRed1() As Color
            Get
                Return Color.FromRgb(255, 69, 0)
            End Get
        End Property

        ''' <summary>Gets OrangeRed2.</summary>
        Public Shared ReadOnly Property OrangeRed2() As Color
            Get
                Return Color.FromRgb(238, 64, 0)
            End Get
        End Property

        ''' <summary>Gets OrangeRed3.</summary>
        Public Shared ReadOnly Property OrangeRed3() As Color
            Get
                Return Color.FromRgb(205, 55, 0)
            End Get
        End Property

        ''' <summary>Gets OrangeRed4.</summary>
        Public Shared ReadOnly Property OrangeRed4() As Color
            Get
                Return Color.FromRgb(139, 37, 0)
            End Get
        End Property

        ''' <summary>Gets Orchid.</summary>
        Public Shared ReadOnly Property Orchid() As Color
            Get
                Return Color.FromRgb(218, 112, 214)
            End Get
        End Property

        ''' <summary>Gets Orchid1.</summary>
        Public Shared ReadOnly Property Orchid1() As Color
            Get
                Return Color.FromRgb(255, 131, 250)
            End Get
        End Property

        ''' <summary>Gets Orchid2.</summary>
        Public Shared ReadOnly Property Orchid2() As Color
            Get
                Return Color.FromRgb(238, 122, 233)
            End Get
        End Property

        ''' <summary>Gets Orchid3.</summary>
        Public Shared ReadOnly Property Orchid3() As Color
            Get
                Return Color.FromRgb(205, 105, 201)
            End Get
        End Property

        ''' <summary>Gets Orchid4.</summary>
        Public Shared ReadOnly Property Orchid4() As Color
            Get
                Return Color.FromRgb(139, 71, 137)
            End Get
        End Property

        ''' <summary>Gets PaleGoldenrod.</summary>
        Public Shared ReadOnly Property PaleGoldenrod() As Color
            Get
                Return Color.FromRgb(238, 232, 170)
            End Get
        End Property

        ''' <summary>Gets PaleGreen.</summary>
        Public Shared ReadOnly Property PaleGreen() As Color
            Get
                Return Color.FromRgb(152, 251, 152)
            End Get
        End Property

        ''' <summary>Gets PaleGreen1.</summary>
        Public Shared ReadOnly Property PaleGreen1() As Color
            Get
                Return Color.FromRgb(154, 255, 154)
            End Get
        End Property

        ''' <summary>Gets PaleGreen2.</summary>
        Public Shared ReadOnly Property PaleGreen2() As Color
            Get
                Return Color.FromRgb(144, 238, 144)
            End Get
        End Property

        ''' <summary>Gets PaleGreen3.</summary>
        Public Shared ReadOnly Property PaleGreen3() As Color
            Get
                Return Color.FromRgb(124, 205, 124)
            End Get
        End Property

        ''' <summary>Gets PaleGreen4.</summary>
        Public Shared ReadOnly Property PaleGreen4() As Color
            Get
                Return Color.FromRgb(84, 139, 84)
            End Get
        End Property

        ''' <summary>Gets PaleTurquoise.</summary>
        Public Shared ReadOnly Property PaleTurquoise() As Color
            Get
                Return Color.FromRgb(175, 238, 238)
            End Get
        End Property

        ''' <summary>Gets PaleTurquoise1.</summary>
        Public Shared ReadOnly Property PaleTurquoise1() As Color
            Get
                Return Color.FromRgb(187, 255, 255)
            End Get
        End Property

        ''' <summary>Gets PaleTurquoise2.</summary>
        Public Shared ReadOnly Property PaleTurquoise2() As Color
            Get
                Return Color.FromRgb(174, 238, 238)
            End Get
        End Property

        ''' <summary>Gets PaleTurquoise3.</summary>
        Public Shared ReadOnly Property PaleTurquoise3() As Color
            Get
                Return Color.FromRgb(150, 205, 205)
            End Get
        End Property

        ''' <summary>Gets PaleTurquoise4.</summary>
        Public Shared ReadOnly Property PaleTurquoise4() As Color
            Get
                Return Color.FromRgb(102, 139, 139)
            End Get
        End Property

        ''' <summary>Gets PaleVioletRed.</summary>
        Public Shared ReadOnly Property PaleVioletRed() As Color
            Get
                Return Color.FromRgb(219, 112, 147)
            End Get
        End Property

        ''' <summary>Gets PaleVioletRed1.</summary>
        Public Shared ReadOnly Property PaleVioletRed1() As Color
            Get
                Return Color.FromRgb(255, 130, 171)
            End Get
        End Property

        ''' <summary>Gets PaleVioletRed2.</summary>
        Public Shared ReadOnly Property PaleVioletRed2() As Color
            Get
                Return Color.FromRgb(238, 121, 159)
            End Get
        End Property

        ''' <summary>Gets PaleVioletRed3.</summary>
        Public Shared ReadOnly Property PaleVioletRed3() As Color
            Get
                Return Color.FromRgb(205, 104, 137)
            End Get
        End Property

        ''' <summary>Gets PaleVioletRed4.</summary>
        Public Shared ReadOnly Property PaleVioletRed4() As Color
            Get
                Return Color.FromRgb(139, 71, 93)
            End Get
        End Property

        ''' <summary>Gets PapayaWhip.</summary>
        Public Shared ReadOnly Property PapayaWhip() As Color
            Get
                Return Color.FromRgb(255, 239, 213)
            End Get
        End Property

        ''' <summary>Gets PeachPuff.</summary>
        Public Shared ReadOnly Property PeachPuff() As Color
            Get
                Return Color.FromRgb(255, 218, 185)
            End Get
        End Property

        ''' <summary>Gets PeachPuff1.</summary>
        Public Shared ReadOnly Property PeachPuff1() As Color
            Get
                Return Color.FromRgb(255, 218, 185)
            End Get
        End Property

        ''' <summary>Gets PeachPuff2.</summary>
        Public Shared ReadOnly Property PeachPuff2() As Color
            Get
                Return Color.FromRgb(238, 203, 173)
            End Get
        End Property

        ''' <summary>Gets PeachPuff3.</summary>
        Public Shared ReadOnly Property PeachPuff3() As Color
            Get
                Return Color.FromRgb(205, 175, 149)
            End Get
        End Property

        ''' <summary>Gets PeachPuff4.</summary>
        Public Shared ReadOnly Property PeachPuff4() As Color
            Get
                Return Color.FromRgb(139, 119, 101)
            End Get
        End Property

        ''' <summary>Gets Peru.</summary>
        Public Shared ReadOnly Property Peru() As Color
            Get
                Return Color.FromRgb(205, 133, 63)
            End Get
        End Property

        ''' <summary>Gets Pink.</summary>
        Public Shared ReadOnly Property Pink() As Color
            Get
                Return Color.FromRgb(255, 192, 203)
            End Get
        End Property

        ''' <summary>Gets Pink1.</summary>
        Public Shared ReadOnly Property Pink1() As Color
            Get
                Return Color.FromRgb(255, 181, 197)
            End Get
        End Property

        ''' <summary>Gets Pink2.</summary>
        Public Shared ReadOnly Property Pink2() As Color
            Get
                Return Color.FromRgb(238, 169, 184)
            End Get
        End Property

        ''' <summary>Gets Pink3.</summary>
        Public Shared ReadOnly Property Pink3() As Color
            Get
                Return Color.FromRgb(205, 145, 158)
            End Get
        End Property

        ''' <summary>Gets Pink4.</summary>
        Public Shared ReadOnly Property Pink4() As Color
            Get
                Return Color.FromRgb(139, 99, 108)
            End Get
        End Property

        ''' <summary>Gets Plum.</summary>
        Public Shared ReadOnly Property Plum() As Color
            Get
                Return Color.FromRgb(221, 160, 221)
            End Get
        End Property

        ''' <summary>Gets Plum1.</summary>
        Public Shared ReadOnly Property Plum1() As Color
            Get
                Return Color.FromRgb(255, 187, 255)
            End Get
        End Property

        ''' <summary>Gets Plum2.</summary>
        Public Shared ReadOnly Property Plum2() As Color
            Get
                Return Color.FromRgb(238, 174, 238)
            End Get
        End Property

        ''' <summary>Gets Plum3.</summary>
        Public Shared ReadOnly Property Plum3() As Color
            Get
                Return Color.FromRgb(205, 150, 205)
            End Get
        End Property

        ''' <summary>Gets Plum4.</summary>
        Public Shared ReadOnly Property Plum4() As Color
            Get
                Return Color.FromRgb(139, 102, 139)
            End Get
        End Property

        ''' <summary>Gets PowderBlue.</summary>
        Public Shared ReadOnly Property PowderBlue() As Color
            Get
                Return Color.FromRgb(176, 224, 230)
            End Get
        End Property

        ''' <summary>Gets Purple.</summary>
        Public Shared ReadOnly Property Purple() As Color
            Get
                Return Color.FromRgb(160, 32, 240)
            End Get
        End Property

        ''' <summary>Gets Purple1.</summary>
        Public Shared ReadOnly Property Purple1() As Color
            Get
                Return Color.FromRgb(155, 48, 255)
            End Get
        End Property

        ''' <summary>Gets Purple2.</summary>
        Public Shared ReadOnly Property Purple2() As Color
            Get
                Return Color.FromRgb(145, 44, 238)
            End Get
        End Property

        ''' <summary>Gets Purple3.</summary>
        Public Shared ReadOnly Property Purple3() As Color
            Get
                Return Color.FromRgb(125, 38, 205)
            End Get
        End Property

        ''' <summary>Gets Purple4.</summary>
        Public Shared ReadOnly Property Purple4() As Color
            Get
                Return Color.FromRgb(85, 26, 139)
            End Get
        End Property

        ''' <summary>Gets Red.</summary>
        Public Shared ReadOnly Property Red() As Color
            Get
                Return Color.FromRgb(255, 0, 0)
            End Get
        End Property

        ''' <summary>Gets Red1.</summary>
        Public Shared ReadOnly Property Red1() As Color
            Get
                Return Color.FromRgb(255, 0, 0)
            End Get
        End Property

        ''' <summary>Gets Red2.</summary>
        Public Shared ReadOnly Property Red2() As Color
            Get
                Return Color.FromRgb(238, 0, 0)
            End Get
        End Property

        ''' <summary>Gets Red3.</summary>
        Public Shared ReadOnly Property Red3() As Color
            Get
                Return Color.FromRgb(205, 0, 0)
            End Get
        End Property

        ''' <summary>Gets Red4.</summary>
        Public Shared ReadOnly Property Red4() As Color
            Get
                Return Color.FromRgb(139, 0, 0)
            End Get
        End Property

        ''' <summary>Gets RosyBrown.</summary>
        Public Shared ReadOnly Property RosyBrown() As Color
            Get
                Return Color.FromRgb(188, 143, 143)
            End Get
        End Property

        ''' <summary>Gets RosyBrown1.</summary>
        Public Shared ReadOnly Property RosyBrown1() As Color
            Get
                Return Color.FromRgb(255, 193, 193)
            End Get
        End Property

        ''' <summary>Gets RosyBrown2.</summary>
        Public Shared ReadOnly Property RosyBrown2() As Color
            Get
                Return Color.FromRgb(238, 180, 180)
            End Get
        End Property

        ''' <summary>Gets RosyBrown3.</summary>
        Public Shared ReadOnly Property RosyBrown3() As Color
            Get
                Return Color.FromRgb(205, 155, 155)
            End Get
        End Property

        ''' <summary>Gets RosyBrown4.</summary>
        Public Shared ReadOnly Property RosyBrown4() As Color
            Get
                Return Color.FromRgb(139, 105, 105)
            End Get
        End Property

        ''' <summary>Gets RoyalBlue.</summary>
        Public Shared ReadOnly Property RoyalBlue() As Color
            Get
                Return Color.FromRgb(65, 105, 225)
            End Get
        End Property

        ''' <summary>Gets RoyalBlue1.</summary>
        Public Shared ReadOnly Property RoyalBlue1() As Color
            Get
                Return Color.FromRgb(72, 118, 255)
            End Get
        End Property

        ''' <summary>Gets RoyalBlue2.</summary>
        Public Shared ReadOnly Property RoyalBlue2() As Color
            Get
                Return Color.FromRgb(67, 110, 238)
            End Get
        End Property

        ''' <summary>Gets RoyalBlue3.</summary>
        Public Shared ReadOnly Property RoyalBlue3() As Color
            Get
                Return Color.FromRgb(58, 95, 205)
            End Get
        End Property

        ''' <summary>Gets RoyalBlue4.</summary>
        Public Shared ReadOnly Property RoyalBlue4() As Color
            Get
                Return Color.FromRgb(39, 64, 139)
            End Get
        End Property

        ''' <summary>Gets SaddleBrown.</summary>
        Public Shared ReadOnly Property SaddleBrown() As Color
            Get
                Return Color.FromRgb(139, 69, 19)
            End Get
        End Property

        ''' <summary>Gets Salmon.</summary>
        Public Shared ReadOnly Property Salmon() As Color
            Get
                Return Color.FromRgb(250, 128, 114)
            End Get
        End Property

        ''' <summary>Gets Salmon1.</summary>
        Public Shared ReadOnly Property Salmon1() As Color
            Get
                Return Color.FromRgb(255, 140, 105)
            End Get
        End Property

        ''' <summary>Gets Salmon2.</summary>
        Public Shared ReadOnly Property Salmon2() As Color
            Get
                Return Color.FromRgb(238, 130, 98)
            End Get
        End Property

        ''' <summary>Gets Salmon3.</summary>
        Public Shared ReadOnly Property Salmon3() As Color
            Get
                Return Color.FromRgb(205, 112, 84)
            End Get
        End Property

        ''' <summary>Gets Salmon4.</summary>
        Public Shared ReadOnly Property Salmon4() As Color
            Get
                Return Color.FromRgb(139, 76, 57)
            End Get
        End Property

        ''' <summary>Gets SandyBrown.</summary>
        Public Shared ReadOnly Property SandyBrown() As Color
            Get
                Return Color.FromRgb(244, 164, 96)
            End Get
        End Property

        ''' <summary>Gets SeaGreen.</summary>
        Public Shared ReadOnly Property SeaGreen() As Color
            Get
                Return Color.FromRgb(46, 139, 87)
            End Get
        End Property

        ''' <summary>Gets SeaGreen1.</summary>
        Public Shared ReadOnly Property SeaGreen1() As Color
            Get
                Return Color.FromRgb(84, 255, 159)
            End Get
        End Property

        ''' <summary>Gets SeaGreen2.</summary>
        Public Shared ReadOnly Property SeaGreen2() As Color
            Get
                Return Color.FromRgb(78, 238, 148)
            End Get
        End Property

        ''' <summary>Gets SeaGreen3.</summary>
        Public Shared ReadOnly Property SeaGreen3() As Color
            Get
                Return Color.FromRgb(67, 205, 128)
            End Get
        End Property

        ''' <summary>Gets SeaGreen4.</summary>
        Public Shared ReadOnly Property SeaGreen4() As Color
            Get
                Return Color.FromRgb(46, 139, 87)
            End Get
        End Property

        ''' <summary>Gets Seashell.</summary>
        Public Shared ReadOnly Property Seashell() As Color
            Get
                Return Color.FromRgb(255, 245, 238)
            End Get
        End Property

        ''' <summary>Gets Seashell1.</summary>
        Public Shared ReadOnly Property Seashell1() As Color
            Get
                Return Color.FromRgb(255, 245, 238)
            End Get
        End Property

        ''' <summary>Gets Seashell2.</summary>
        Public Shared ReadOnly Property Seashell2() As Color
            Get
                Return Color.FromRgb(238, 229, 222)
            End Get
        End Property

        ''' <summary>Gets Seashell3.</summary>
        Public Shared ReadOnly Property Seashell3() As Color
            Get
                Return Color.FromRgb(205, 197, 191)
            End Get
        End Property

        ''' <summary>Gets Seashell4.</summary>
        Public Shared ReadOnly Property Seashell4() As Color
            Get
                Return Color.FromRgb(139, 134, 130)
            End Get
        End Property

        ''' <summary>Gets Sienna.</summary>
        Public Shared ReadOnly Property Sienna() As Color
            Get
                Return Color.FromRgb(160, 82, 45)
            End Get
        End Property

        ''' <summary>Gets Sienna1.</summary>
        Public Shared ReadOnly Property Sienna1() As Color
            Get
                Return Color.FromRgb(255, 130, 71)
            End Get
        End Property

        ''' <summary>Gets Sienna2.</summary>
        Public Shared ReadOnly Property Sienna2() As Color
            Get
                Return Color.FromRgb(238, 121, 66)
            End Get
        End Property

        ''' <summary>Gets Sienna3.</summary>
        Public Shared ReadOnly Property Sienna3() As Color
            Get
                Return Color.FromRgb(205, 104, 57)
            End Get
        End Property

        ''' <summary>Gets Sienna4.</summary>
        Public Shared ReadOnly Property Sienna4() As Color
            Get
                Return Color.FromRgb(139, 71, 38)
            End Get
        End Property

        ''' <summary>Gets SkyBlue.</summary>
        Public Shared ReadOnly Property SkyBlue() As Color
            Get
                Return Color.FromRgb(135, 206, 235)
            End Get
        End Property

        ''' <summary>Gets SkyBlue1.</summary>
        Public Shared ReadOnly Property SkyBlue1() As Color
            Get
                Return Color.FromRgb(135, 206, 255)
            End Get
        End Property

        ''' <summary>Gets SkyBlue2.</summary>
        Public Shared ReadOnly Property SkyBlue2() As Color
            Get
                Return Color.FromRgb(126, 192, 238)
            End Get
        End Property

        ''' <summary>Gets SkyBlue3.</summary>
        Public Shared ReadOnly Property SkyBlue3() As Color
            Get
                Return Color.FromRgb(108, 166, 205)
            End Get
        End Property

        ''' <summary>Gets SkyBlue4.</summary>
        Public Shared ReadOnly Property SkyBlue4() As Color
            Get
                Return Color.FromRgb(74, 112, 139)
            End Get
        End Property

        ''' <summary>Gets SlateBlue.</summary>
        Public Shared ReadOnly Property SlateBlue() As Color
            Get
                Return Color.FromRgb(106, 90, 205)
            End Get
        End Property

        ''' <summary>Gets SlateBlue1.</summary>
        Public Shared ReadOnly Property SlateBlue1() As Color
            Get
                Return Color.FromRgb(131, 111, 255)
            End Get
        End Property

        ''' <summary>Gets SlateBlue2.</summary>
        Public Shared ReadOnly Property SlateBlue2() As Color
            Get
                Return Color.FromRgb(122, 103, 238)
            End Get
        End Property

        ''' <summary>Gets SlateBlue3.</summary>
        Public Shared ReadOnly Property SlateBlue3() As Color
            Get
                Return Color.FromRgb(105, 89, 205)
            End Get
        End Property

        ''' <summary>Gets SlateBlue4.</summary>
        Public Shared ReadOnly Property SlateBlue4() As Color
            Get
                Return Color.FromRgb(71, 60, 139)
            End Get
        End Property

        ''' <summary>Gets SlateGray.</summary>
        Public Shared ReadOnly Property SlateGray() As Color
            Get
                Return Color.FromRgb(112, 128, 144)
            End Get
        End Property

        ''' <summary>Gets SlateGray1.</summary>
        Public Shared ReadOnly Property SlateGray1() As Color
            Get
                Return Color.FromRgb(198, 226, 255)
            End Get
        End Property

        ''' <summary>Gets SlateGray2.</summary>
        Public Shared ReadOnly Property SlateGray2() As Color
            Get
                Return Color.FromRgb(185, 211, 238)
            End Get
        End Property

        ''' <summary>Gets SlateGray3.</summary>
        Public Shared ReadOnly Property SlateGray3() As Color
            Get
                Return Color.FromRgb(159, 182, 205)
            End Get
        End Property

        ''' <summary>Gets SlateGray4.</summary>
        Public Shared ReadOnly Property SlateGray4() As Color
            Get
                Return Color.FromRgb(108, 123, 139)
            End Get
        End Property

        ''' <summary>Gets SlateGrey.</summary>
        Public Shared ReadOnly Property SlateGrey() As Color
            Get
                Return Color.FromRgb(112, 128, 144)
            End Get
        End Property

        ''' <summary>Gets Snow.</summary>
        Public Shared ReadOnly Property Snow() As Color
            Get
                Return Color.FromRgb(255, 250, 250)
            End Get
        End Property

        ''' <summary>Gets Snow1.</summary>
        Public Shared ReadOnly Property Snow1() As Color
            Get
                Return Color.FromRgb(255, 250, 250)
            End Get
        End Property

        ''' <summary>Gets Snow2.</summary>
        Public Shared ReadOnly Property Snow2() As Color
            Get
                Return Color.FromRgb(238, 233, 233)
            End Get
        End Property

        ''' <summary>Gets Snow3.</summary>
        Public Shared ReadOnly Property Snow3() As Color
            Get
                Return Color.FromRgb(205, 201, 201)
            End Get
        End Property

        ''' <summary>Gets Snow4.</summary>
        Public Shared ReadOnly Property Snow4() As Color
            Get
                Return Color.FromRgb(139, 137, 137)
            End Get
        End Property

        ''' <summary>Gets SpringGreen.</summary>
        Public Shared ReadOnly Property SpringGreen() As Color
            Get
                Return Color.FromRgb(0, 255, 127)
            End Get
        End Property

        ''' <summary>Gets SpringGreen1.</summary>
        Public Shared ReadOnly Property SpringGreen1() As Color
            Get
                Return Color.FromRgb(0, 255, 127)
            End Get
        End Property

        ''' <summary>Gets SpringGreen2.</summary>
        Public Shared ReadOnly Property SpringGreen2() As Color
            Get
                Return Color.FromRgb(0, 238, 118)
            End Get
        End Property

        ''' <summary>Gets SpringGreen3.</summary>
        Public Shared ReadOnly Property SpringGreen3() As Color
            Get
                Return Color.FromRgb(0, 205, 102)
            End Get
        End Property

        ''' <summary>Gets SpringGreen4.</summary>
        Public Shared ReadOnly Property SpringGreen4() As Color
            Get
                Return Color.FromRgb(0, 139, 69)
            End Get
        End Property

        ''' <summary>Gets SteelBlue.</summary>
        Public Shared ReadOnly Property SteelBlue() As Color
            Get
                Return Color.FromRgb(70, 130, 180)
            End Get
        End Property

        ''' <summary>Gets SteelBlue1.</summary>
        Public Shared ReadOnly Property SteelBlue1() As Color
            Get
                Return Color.FromRgb(99, 184, 255)
            End Get
        End Property

        ''' <summary>Gets SteelBlue2.</summary>
        Public Shared ReadOnly Property SteelBlue2() As Color
            Get
                Return Color.FromRgb(92, 172, 238)
            End Get
        End Property

        ''' <summary>Gets SteelBlue3.</summary>
        Public Shared ReadOnly Property SteelBlue3() As Color
            Get
                Return Color.FromRgb(79, 148, 205)
            End Get
        End Property

        ''' <summary>Gets SteelBlue4.</summary>
        Public Shared ReadOnly Property SteelBlue4() As Color
            Get
                Return Color.FromRgb(54, 100, 139)
            End Get
        End Property

        ''' <summary>Gets Tan.</summary>
        Public Shared ReadOnly Property Tan() As Color
            Get
                Return Color.FromRgb(210, 180, 140)
            End Get
        End Property

        ''' <summary>Gets Tan1.</summary>
        Public Shared ReadOnly Property Tan1() As Color
            Get
                Return Color.FromRgb(255, 165, 79)
            End Get
        End Property

        ''' <summary>Gets Tan2.</summary>
        Public Shared ReadOnly Property Tan2() As Color
            Get
                Return Color.FromRgb(238, 154, 73)
            End Get
        End Property

        ''' <summary>Gets Tan3.</summary>
        Public Shared ReadOnly Property Tan3() As Color
            Get
                Return Color.FromRgb(205, 133, 63)
            End Get
        End Property

        ''' <summary>Gets Tan4.</summary>
        Public Shared ReadOnly Property Tan4() As Color
            Get
                Return Color.FromRgb(139, 90, 43)
            End Get
        End Property

        ''' <summary>Gets Thistle.</summary>
        Public Shared ReadOnly Property Thistle() As Color
            Get
                Return Color.FromRgb(216, 191, 216)
            End Get
        End Property

        ''' <summary>Gets Thistle1.</summary>
        Public Shared ReadOnly Property Thistle1() As Color
            Get
                Return Color.FromRgb(255, 225, 255)
            End Get
        End Property

        ''' <summary>Gets Thistle2.</summary>
        Public Shared ReadOnly Property Thistle2() As Color
            Get
                Return Color.FromRgb(238, 210, 238)
            End Get
        End Property

        ''' <summary>Gets Thistle3.</summary>
        Public Shared ReadOnly Property Thistle3() As Color
            Get
                Return Color.FromRgb(205, 181, 205)
            End Get
        End Property

        ''' <summary>Gets Thistle4.</summary>
        Public Shared ReadOnly Property Thistle4() As Color
            Get
                Return Color.FromRgb(139, 123, 139)
            End Get
        End Property

        ''' <summary>Gets Tomato.</summary>
        Public Shared ReadOnly Property Tomato() As Color
            Get
                Return Color.FromRgb(255, 99, 71)
            End Get
        End Property

        ''' <summary>Gets Tomato1.</summary>
        Public Shared ReadOnly Property Tomato1() As Color
            Get
                Return Color.FromRgb(255, 99, 71)
            End Get
        End Property

        ''' <summary>Gets Tomato2.</summary>
        Public Shared ReadOnly Property Tomato2() As Color
            Get
                Return Color.FromRgb(238, 92, 66)
            End Get
        End Property

        ''' <summary>Gets Tomato3.</summary>
        Public Shared ReadOnly Property Tomato3() As Color
            Get
                Return Color.FromRgb(205, 79, 57)
            End Get
        End Property

        ''' <summary>Gets Tomato4.</summary>
        Public Shared ReadOnly Property Tomato4() As Color
            Get
                Return Color.FromRgb(139, 54, 38)
            End Get
        End Property

        ''' <summary>Gets Turquoise.</summary>
        Public Shared ReadOnly Property Turquoise() As Color
            Get
                Return Color.FromRgb(64, 224, 208)
            End Get
        End Property

        ''' <summary>Gets Turquoise1.</summary>
        Public Shared ReadOnly Property Turquoise1() As Color
            Get
                Return Color.FromRgb(0, 245, 255)
            End Get
        End Property

        ''' <summary>Gets Turquoise2.</summary>
        Public Shared ReadOnly Property Turquoise2() As Color
            Get
                Return Color.FromRgb(0, 229, 238)
            End Get
        End Property

        ''' <summary>Gets Turquoise3.</summary>
        Public Shared ReadOnly Property Turquoise3() As Color
            Get
                Return Color.FromRgb(0, 197, 205)
            End Get
        End Property

        ''' <summary>Gets Turquoise4.</summary>
        Public Shared ReadOnly Property Turquoise4() As Color
            Get
                Return Color.FromRgb(0, 134, 139)
            End Get
        End Property

        ''' <summary>Gets Violet.</summary>
        Public Shared ReadOnly Property Violet() As Color
            Get
                Return Color.FromRgb(238, 130, 238)
            End Get
        End Property

        ''' <summary>Gets VioletRed.</summary>
        Public Shared ReadOnly Property VioletRed() As Color
            Get
                Return Color.FromRgb(208, 32, 144)
            End Get
        End Property

        ''' <summary>Gets VioletRed1.</summary>
        Public Shared ReadOnly Property VioletRed1() As Color
            Get
                Return Color.FromRgb(255, 62, 150)
            End Get
        End Property

        ''' <summary>Gets VioletRed2.</summary>
        Public Shared ReadOnly Property VioletRed2() As Color
            Get
                Return Color.FromRgb(238, 58, 140)
            End Get
        End Property

        ''' <summary>Gets VioletRed3.</summary>
        Public Shared ReadOnly Property VioletRed3() As Color
            Get
                Return Color.FromRgb(205, 50, 120)
            End Get
        End Property

        ''' <summary>Gets VioletRed4.</summary>
        Public Shared ReadOnly Property VioletRed4() As Color
            Get
                Return Color.FromRgb(139, 34, 82)
            End Get
        End Property

        ''' <summary>Gets Wheat.</summary>
        Public Shared ReadOnly Property Wheat() As Color
            Get
                Return Color.FromRgb(245, 222, 179)
            End Get
        End Property

        ''' <summary>Gets Wheat1.</summary>
        Public Shared ReadOnly Property Wheat1() As Color
            Get
                Return Color.FromRgb(255, 231, 186)
            End Get
        End Property

        ''' <summary>Gets Wheat2.</summary>
        Public Shared ReadOnly Property Wheat2() As Color
            Get
                Return Color.FromRgb(238, 216, 174)
            End Get
        End Property

        ''' <summary>Gets Wheat3.</summary>
        Public Shared ReadOnly Property Wheat3() As Color
            Get
                Return Color.FromRgb(205, 186, 150)
            End Get
        End Property

        ''' <summary>Gets Wheat4.</summary>
        Public Shared ReadOnly Property Wheat4() As Color
            Get
                Return Color.FromRgb(139, 126, 102)
            End Get
        End Property

        ''' <summary>Gets White.</summary>
        Public Shared ReadOnly Property White() As Color
            Get
                Return Color.FromRgb(255, 255, 255)
            End Get
        End Property

        ''' <summary>Gets WhiteSmoke.</summary>
        Public Shared ReadOnly Property WhiteSmoke() As Color
            Get
                Return Color.FromRgb(245, 245, 245)
            End Get
        End Property

        ''' <summary>Gets Yellow.</summary>
        Public Shared ReadOnly Property Yellow() As Color
            Get
                Return Color.FromRgb(255, 255, 0)
            End Get
        End Property

        ''' <summary>Gets Yellow1.</summary>
        Public Shared ReadOnly Property Yellow1() As Color
            Get
                Return Color.FromRgb(255, 255, 0)
            End Get
        End Property

        ''' <summary>Gets Yellow2.</summary>
        Public Shared ReadOnly Property Yellow2() As Color
            Get
                Return Color.FromRgb(238, 238, 0)
            End Get
        End Property

        ''' <summary>Gets Yellow3.</summary>
        Public Shared ReadOnly Property Yellow3() As Color
            Get
                Return Color.FromRgb(205, 205, 0)
            End Get
        End Property

        ''' <summary>Gets Yellow4.</summary>
        Public Shared ReadOnly Property Yellow4() As Color
            Get
                Return Color.FromRgb(139, 139, 0)
            End Get
        End Property

        ''' <summary>Gets YellowGreen.</summary>
        Public Shared ReadOnly Property YellowGreen() As Color
            Get
                Return Color.FromRgb(154, 205, 50)
            End Get
        End Property
    End Class
End Namespace
