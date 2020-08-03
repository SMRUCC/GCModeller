#Region "Microsoft.VisualBasic::3aae7d26a2fbdbf19b3c71cc10f7aef0, RDotNET.Extensions.VisualBasic\RSystem.vb"

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

' Module RSystem
' 
'     Properties: R, RColors
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: ColorMaps, getwd, Library, packageVersion, params
'               Rvar, setwd
' 
'     Sub: (+2 Overloads) TryInit
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.My

''' <summary>
''' R Engine extensions.
''' (似乎对于RDotNet而言，在一个应用程序的实例进程之中仅允许一个REngine的实例存在，所以在这里就统一的使用一个公共的REngine的实例对象)
''' </summary>
Public Module RSystem

    Public Const NULL$ = "NULL"

    <Extension>
    Public Function params(additionals As String()) As String
        If additionals.IsNullOrEmpty Then
            Return ""
        Else
            Return ", " & additionals.JoinBy(", ")
        End If
    End Function

    Private Const SPLIT_REGX_EXPRESSION As String = "[,] (?=(?:[^""]|""[^""]*"")*$)"

    ''' <summary>
    ''' The default R Engine server.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property R As ExtendedEngine

    ''' <summary>
    ''' R server can not be initialized automatically, please manual set up init later.
    ''' </summary>
    Const UnableRunAutomatically As String = "R server can not be initialized automatically, please manual set up init later."
    Const rdotnet_engine As String = "rdotnet_engine"

    ''' <summary>
    ''' Initialize the default R Engine.(可以通过在命令行之中使用``/@set``开关设置``R_HOME``参数来手动设置R的文件夹位置)
    ''' </summary>
    Sub New()
        Dim R_HOME$ = App.GetVariable("R_HOME")

        'If Not SharedObject.GetObject(rdotnet_engine) Is Nothing Then
        '    R = SharedObject.GetObject(rdotnet_engine)
        '    Return
        'End If

        Try
            If R_HOME.StringEmpty Then
                Call TryInit()
            Else
                Call TryInit(R_HOME)
            End If
        Catch ex As Exception
            ' 无法自动初始化，需要手动启动R的计算引擎
            ex = New Exception(UnableRunAutomatically, ex)
            Call ex.PrintException
            Call App.LogException(ex)
            Call NativeLibrary.NativeUtility.SetEnvironmentVariablesLog.SaveTo("./R_inits.log")
        Finally
            If Not R Is Nothing Then
                Call SharedObject.SetObject(rdotnet_engine, R)
            End If
        End Try
    End Sub

    Public Function ref(name As String) As String
        Return "&" & Strings.Trim(name).Trim("&"c)
    End Function

    Public Function ref(var As var) As String
        Return "&" & var.name
    End Function

    ''' <summary>
    ''' Manual set up R init environment.
    ''' </summary>
    ''' <param name="R_HOME"></param>
    Public Sub TryInit(R_HOME As String)
        If R Is Nothing OrElse Not R.IsRunning Then
            _R = RInit.StartEngineServices(R_HOME)
        End If
    End Sub

    Public Sub TryInit()
        If R Is Nothing OrElse Not R.IsRunning Then
            _R = RInit.StartEngineServices
        End If
    End Sub

    ''' <summary>
    ''' 从一个变量名称创建一个R变量帮助对象
    ''' </summary>
    ''' <param name="name"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function Rvar(name As String) As var
        Return New var(name)
    End Function

    ''' <summary>
    ''' Parses and returns the ‘DESCRIPTION’ file of a package.
    ''' </summary>
    ''' <param name="pkg">a character string with the package name.</param>
    ''' <returns></returns>
    Public Function packageVersion(pkg As String) As String
        Dim R As String = $"packageVersion(""{pkg}"")"
        Dim result As String()
        Try
            result = RSystem.R.WriteLine(R)
        Catch ex As Exception
            ex = New Exception(R, ex)
            Call App.LogException(ex)
            Return ""
        End Try
        Dim ver As String = result.ElementAtOrDefault(Scan0)
        If Not String.IsNullOrEmpty(ver) Then
            ver = String.Join(".", Regex.Matches(ver, "\d+").ToArray)
        End If
        Return ver
    End Function

    ''' <summary>
    ''' 查看R引擎中已经安装的包
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Library() As String
        Dim result$ = R.WriteLine("library()").JoinBy(vbCrLf)
        Dim sBuilder As New StringBuilder(result, 5 * 1024)

        sBuilder.Remove(0, 2)
        sBuilder.Remove(sBuilder.Length - 1, 1)

        Dim array$() = Regex.Split(sBuilder.ToString, SPLIT_REGX_EXPRESSION)
        Dim width As Integer = array.Length / 3
        Dim s$

        sBuilder.Clear()

        For i As Integer = 0 To width - 1
            s = String.Format("{1}  {0}  {2}", array(i), array(i + width), array(i + width * 2))
            sBuilder.AppendLine(s)
        Next

        sBuilder.Replace("""", "")

        Call Console.WriteLine(sBuilder.ToString)

        Return sBuilder.ToString
    End Function

    ''' <summary>
    ''' Display the current working directory
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getwd() As String
        Return R.WriteLine("getwd()").JoinBy(vbCrLf)
    End Function

    Public Function setwd(workingDir As String) As String()
        Return R.WriteLine($"setwd(""{workingDir}"")")
    End Function

    ''' <summary>
    ''' 枚举R中所有的颜色代码
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RColors As String() = {
        "white", "aliceblue", "antiquewhite", "antiquewhite1", "antiquewhite2", "antiquewhite3", "antiquewhite4", "aquamarine", "aquamarine1", "aquamarine2", "aquamarine3", "aquamarine4", "azure", "azure1", "azure2", "azure3", "azure4",
        "beige", "bisque", "bisque1", "bisque2", "bisque3", "bisque4", "black", "blanchedalmond", "blue", "blue1", "blue2", "blue3", "blue4", "blueviolet", "brown", "brown1", "brown2", "brown3", "brown4", "burlywood", "burlywood1",
        "burlywood2", "burlywood3", "burlywood4",
        "cadetblue", "cadetblue1", "cadetblue2", "cadetblue3", "cadetblue4", "chartreuse", "chartreuse1", "chartreuse2", "chartreuse3", "chartreuse4", "chocolate", "chocolate1", "chocolate2", "chocolate3", "chocolate4", "coral", "coral1",
        "coral2", "coral3", "coral4", "cornflowerblue", "cornsilk", "cornsilk1", "cornsilk2", "cornsilk3", "cornsilk4", "cyan", "cyan1", "cyan2", "cyan3", "cyan4",
        "darkblue", "darkcyan", "darkgoldenrod", "darkgoldenrod1", "darkgoldenrod2", "darkgoldenrod3", "darkgoldenrod4", "darkgray", "darkgreen", "darkgrey", "darkkhaki", "darkmagenta", "darkolivegreen", "darkolivegreen1", "darkolivegreen2",
        "darkolivegreen3", "darkolivegreen4", "darkorange", "darkorange1", "darkorange2", "darkorange3", "darkorange4", "darkorchid", "darkorchid1", "darkorchid2", "darkorchid3", "darkorchid4", "darkred", "darksalmon", "darkseagreen",
        "darkseagreen1", "darkseagreen2", "darkseagreen3", "darkseagreen4", "darkslateblue", "darkslategray", "darkslategray1", "darkslategray2", "darkslategray3", "darkslategray4", "darkslategrey", "darkturquoise", "darkviolet", "deeppink",
        "deeppink1", "deeppink2", "deeppink3", "deeppink4", "deepskyblue", "deepskyblue1", "deepskyblue2", "deepskyblue3", "deepskyblue4", "dimgray", "dimgrey", "dodgerblue", "dodgerblue1", "dodgerblue2", "dodgerblue3", "dodgerblue4",
        "firebrick", "firebrick1", "firebrick2", "firebrick3", "firebrick4", "floralwhite", "forestgreen",
        "gainsboro", "ghostwhite", "gold", "gold1", "gold2", "gold3", "gold4", "goldenrod", "goldenrod1", "goldenrod2", "goldenrod3", "goldenrod4", "gray", "gray0", "gray1", "gray2", "gray3", "gray4", "gray5", "gray6", "gray7", "gray8",
        "gray9", "gray10", "gray11", "gray12", "gray13", "gray14", "gray15", "gray16", "gray17", "gray18", "gray19", "gray20", "gray21", "gray22", "gray23", "gray24", "gray25", "gray26", "gray27", "gray28", "gray29", "gray30", "gray31",
        "gray32", "gray33", "gray34", "gray35", "gray36", "gray37", "gray38", "gray39", "gray40", "gray41", "gray42", "gray43", "gray44", "gray45", "gray46", "gray47", "gray48", "gray49", "gray50", "gray51", "gray52", "gray53", "gray54",
        "gray55", "gray56", "gray57", "gray58", "gray59", "gray60", "gray61", "gray62", "gray63", "gray64", "gray65", "gray66", "gray67", "gray68", "gray69", "gray70", "gray71", "gray72", "gray73", "gray74", "gray75", "gray76", "gray77",
        "gray78", "gray79", "gray80", "gray81", "gray82", "gray83", "gray84", "gray85", "gray86", "gray87", "gray88", "gray89", "gray90", "gray91", "gray92", "gray93", "gray94", "gray95", "gray96", "gray97", "gray98", "gray99", "gray100",
        "green", "green1", "green2", "green3", "green4", "greenyellow", "grey", "grey0", "grey1", "grey2", "grey3", "grey4", "grey5", "grey6", "grey7", "grey8", "grey9", "grey10", "grey11", "grey12", "grey13", "grey14", "grey15", "grey16",
        "grey17", "grey18", "grey19", "grey20", "grey21", "grey22", "grey23", "grey24", "grey25", "grey26", "grey27", "grey28", "grey29", "grey30", "grey31", "grey32", "grey33", "grey34", "grey35", "grey36", "grey37", "grey38", "grey39",
        "grey40", "grey41", "grey42", "grey43", "grey44", "grey45", "grey46", "grey47", "grey48", "grey49", "grey50", "grey51", "grey52", "grey53", "grey54", "grey55", "grey56", "grey57", "grey58", "grey59", "grey60", "grey61", "grey62",
        "grey63", "grey64", "grey65", "grey66", "grey67", "grey68", "grey69", "grey70", "grey71", "grey72", "grey73", "grey74", "grey75", "grey76", "grey77", "grey78", "grey79", "grey80", "grey81", "grey82", "grey83", "grey84", "grey85",
        "grey86", "grey87", "grey88", "grey89", "grey90", "grey91", "grey92", "grey93", "grey94", "grey95", "grey96", "grey97", "grey98", "grey99", "grey100",
        "honeydew", "honeydew1", "honeydew2", "honeydew3", "honeydew4", "hotpink", "hotpink1", "hotpink2", "hotpink3", "hotpink4",
        "indianred", "indianred1", "indianred2", "indianred3", "indianred4", "ivory", "ivory1", "ivory2", "ivory3", "ivory4",
        "khaki", "khaki1", "khaki2", "khaki3", "khaki4",
        "lavender", "lavenderblush", "lavenderblush1", "lavenderblush2", "lavenderblush3", "lavenderblush4", "lawngreen", "lemonchiffon", "lemonchiffon1", "lemonchiffon2", "lemonchiffon3", "lemonchiffon4", "lightblue", "lightblue1",
        "lightblue2", "lightblue3", "lightblue4", "lightcoral", "lightcyan", "lightcyan1", "lightcyan2", "lightcyan3", "lightcyan4", "lightgoldenrod", "lightgoldenrod1", "lightgoldenrod2", "lightgoldenrod3", "lightgoldenrod4",
        "lightgoldenrodyellow", "lightgray", "lightgreen", "lightgrey", "lightpink", "lightpink1", "lightpink2", "lightpink3", "lightpink4", "lightsalmon", "lightsalmon1", "lightsalmon2", "lightsalmon3", "lightsalmon4", "lightseagreen",
        "lightskyblue", "lightskyblue1", "lightskyblue2", "lightskyblue3", "lightskyblue4", "lightslateblue", "lightslategray", "lightslategrey", "lightsteelblue", "lightsteelblue1", "lightsteelblue2", "lightsteelblue3", "lightsteelblue4",
        "lightyellow", "lightyellow1", "lightyellow2", "lightyellow3", "lightyellow4", "limegreen", "linen",
        "magenta", "magenta1", "magenta2", "magenta3", "magenta4", "maroon", "maroon1", "maroon2", "maroon3", "maroon4", "mediumaquamarine", "mediumblue", "mediumorchid", "mediumorchid1", "mediumorchid2", "mediumorchid3", "mediumorchid4",
        "mediumpurple", "mediumpurple1", "mediumpurple2", "mediumpurple3", "mediumpurple4", "mediumseagreen", "mediumslateblue", "mediumspringgreen", "mediumturquoise", "mediumvioletred", "midnightblue", "mintcream", "mistyrose", "mistyrose1",
        "mistyrose2", "mistyrose3", "mistyrose4", "moccasin",
        "navajowhite", "navajowhite1", "navajowhite2", "navajowhite3", "navajowhite4", "navy", "navyblue",
        "oldlace", "olivedrab", "olivedrab1", "olivedrab2", "olivedrab3", "olivedrab4", "orange", "orange1", "orange2", "orange3", "orange4", "orangered", "orangered1", "orangered2", "orangered3", "orangered4", "orchid", "orchid1", "orchid2",
        "orchid3", "orchid4",
        "palegoldenrod", "palegreen", "palegreen1", "palegreen2", "palegreen3", "palegreen4", "paleturquoise", "paleturquoise1", "paleturquoise2", "paleturquoise3", "paleturquoise4", "palevioletred", "palevioletred1", "palevioletred2",
        "palevioletred3", "palevioletred4", "papayawhip", "peachpuff", "peachpuff1", "peachpuff2", "peachpuff3", "peachpuff4", "peru", "pink", "pink1", "pink2", "pink3", "pink4", "plum", "plum1", "plum2", "plum3", "plum4", "powderblue", "purple",
        "purple1", "purple2", "purple3", "purple4",
        "red", "red1", "red2", "red3", "red4", "rosybrown", "rosybrown1", "rosybrown2", "rosybrown3", "rosybrown4", "royalblue", "royalblue1", "royalblue2", "royalblue3", "royalblue4",
        "saddlebrown", "salmon", "salmon1", "salmon2", "salmon3", "salmon4", "sandybrown", "seagreen", "seagreen1", "seagreen2", "seagreen3", "seagreen4", "seashell", "seashell1", "seashell2", "seashell3", "seashell4", "sienna", "sienna1",
        "sienna2", "sienna3", "sienna4", "skyblue", "skyblue1", "skyblue2", "skyblue3", "skyblue4", "slateblue", "slateblue1", "slateblue2", "slateblue3", "slateblue4", "slategray", "slategray1", "slategray2", "slategray3", "slategray4",
        "slategrey", "snow", "snow1", "snow2", "snow3", "snow4", "springgreen", "springgreen1", "springgreen2", "springgreen3", "springgreen4", "steelblue", "steelblue1", "steelblue2", "steelblue3", "steelblue4",
        "tan", "tan1", "tan2", "tan3", "tan4", "thistle", "thistle1", "thistle2", "thistle3", "thistle4", "tomato", "tomato1", "tomato2", "tomato3", "tomato4", "turquoise", "turquoise1", "turquoise2", "turquoise3", "turquoise4",
        "violet", "violetred", "violetred1", "violetred2", "violetred3", "violetred4",
        "wheat", "wheat1", "wheat2", "wheat3", "wheat4", "whitesmoke",
        "yellow", "yellow1", "yellow2", "yellow3", "yellow4", "yellowgreen"
    }

    ''' <summary>
    ''' Maps color to each object in the <paramref name="source"/> sequence.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <returns></returns>
    Public Function ColorMaps(Of T)(source As IEnumerable(Of T)) As Dictionary(Of T, String)
        With RColors.Shuffles.AsLoop
            Return source.ToDictionary(Function(x) x, Function() .Next)
        End With
    End Function

    ' <U+767D>

    Public Function ProcessingRUniCode(output As String) As String
        Dim unicodes As String() = output.Matches("[<]U[+][A-H0-9]+[>]").Distinct.ToArray
        Dim charCode As Integer
        Dim [char] As Char
        Dim str As New StringBuilder(output)

        For Each code As String In unicodes
            charCode = code.GetStackValue("<", ">").Split("+"c).Last.DoCall(AddressOf i32.GetHexInteger)
            [char] = Strings.ChrW(charCode)
            str.Replace(code, [char])
        Next

        Return str.ToString
    End Function
End Module
