Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions

Public Module Installed

    Public Function Packages(packageName As String) As Boolean
        Return RServer.Library(packageName)
    End Function
End Module

Public Module Install

    ''' <summary>
    ''' 查看目标程序包是否已经安装在R系统里面
    ''' </summary>
    ''' <param name="packageName"></param>
    ''' <returns></returns>
    Public Function Packages(packageName As String) As Boolean
        Try
            Call RServer.Evaluate($"install.packages('{packageName}')")
        Catch ex As Exception
            Call App.LogException(ex)
            Return False
        End Try
        Return True
    End Function
End Module

''' <summary>
''' R Engine extensions.(似乎对于RDotNet而言，在一个应用程序的实例进程之中仅允许一个REngine的实例存在，所以在这里就统一的使用一个公共的REngine的实例对象)
''' </summary>
Public Module RSystem

    Private Const SPLIT_REGX_EXPRESSION As String = "[,] (?=(?:[^""]|""[^""]*"")*$)"

    ''' <summary>
    ''' The default R Engine server.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property RServer As RDotNET.REngine

    ''' <summary>
    ''' Initialize the default R Engine.
    ''' </summary>
    Sub New()
        Try
            RSystem.RServer = RInit.StartEngineServices
        Catch ex As Exception
            ' 无法自动初始化，需要手动启动R的计算引擎
            ex = New Exception("R server can not be initialized automatically, please manual set up init later.", ex)
            Call App.LogException(ex)
        End Try
    End Sub

    ''' <summary>
    ''' Manual set up R init environment.
    ''' </summary>
    ''' <param name="R_HOME"></param>
    Public Sub TryInit(R_HOME As String)
        If RServer Is Nothing OrElse Not RServer.IsRunning Then
            _RServer = RInit.StartEngineServices(R_HOME)
        End If
    End Sub

    Public Sub TryInit()
        If RServer Is Nothing OrElse Not RServer.IsRunning Then
            _RServer = RInit.StartEngineServices
        End If
    End Sub

    ''' <summary>
    ''' Parses and returns the ‘DESCRIPTION’ file of a package.
    ''' </summary>
    ''' <param name="pkg">a character string with the package name.</param>
    ''' <returns></returns>
    Public Function packageVersion(pkg As String) As String
        Dim R As String = $"packageVersion(""{pkg}"")"
        Dim result As String()
        Try
            result = RServer.WriteLine(R)
        Catch ex As Exception
            ex = New Exception(R, ex)
            Call App.LogException(ex)
            Return ""
        End Try
        Dim ver As String = result.Get(Scan0)
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
        Dim Result As String = RServer.WriteLine("library()").JoinBy(vbCrLf)
        Dim sBuilder As StringBuilder = New StringBuilder(Result, 5 * 1024)

        sBuilder.Remove(0, 2)
        sBuilder.Remove(sBuilder.Length - 1, 1)

        Dim Array = Regex.Split(sBuilder.ToString, SPLIT_REGX_EXPRESSION)
        Dim Width As Integer = Array.Length / 3

        sBuilder.Clear()
        For i As Integer = 0 To Width - 1
            Dim s = String.Format("{1}  {0}  {2}", Array(i), Array(i + Width), Array(i + Width * 2))
            sBuilder.AppendLine(s)
        Next
        sBuilder.Replace("""", "")
        Call Console.WriteLine(sBuilder.ToString)

        Return sBuilder.ToString
    End Function

    ''' <summary>
    ''' The R engine execute a R script. source causes R to accept its input from the named file or URL or connection.
    ''' Input is read and parsed from that file until the end of the file is reached, then the parsed expressions are
    ''' evaluated sequentially in the chosen environment.
    ''' (R引擎执行文件系统之中的一个R脚本)
    ''' </summary>
    ''' <param name="file">a connection Or a character String giving the pathname Of the file Or URL To read from. ""
    ''' indicates the connection stdin().
    ''' </param>
    ''' <param name="local">TRUE, FALSE or an environment, determining where the parsed expressions are evaluated.
    ''' FALSE (the default) corresponds to the user's workspace (the global environment) and TRUE to the environment
    ''' from which source is called.</param>
    ''' <param name="echo">logical; if TRUE, each expression is printed after parsing, before evaluation.</param>
    ''' <param name="printEval">logical; if TRUE, the result of eval(i) is printed for each expression i; defaults to the value of echo.</param>
    ''' <param name="verbose">if TRUE, more diagnostics (than just echo = TRUE) are printed during parsing and evaluation of input,
    ''' including extra info for each expression.</param>
    ''' <param name="promptEcho">character; gives the prompt to be used if echo = TRUE.</param>
    ''' <param name="maxDeparseLength">integer; is used only if echo is TRUE and gives the maximal number of characters output for
    ''' the deparse of a single expression.</param>
    ''' <param name="chdir">logical; if TRUE And file Is a pathname, the R working directory Is temporarily changed to the
    ''' directory containing file for evaluating.</param>
    ''' <param name="encoding">character vector. The encoding(s) to be assumed when file is a character string: see file.
    ''' A possible value is "unknown" when the encoding is guessed: see the ‘Encodings’ section.</param>
    ''' <param name="continueEcho">character; gives the prompt to use on continuation lines if echo = TRUE.</param>
    ''' <param name="skipEcho">integer; how many comment lines at the start of the file to skip if echo = TRUE.</param>
    ''' <param name="keepSource">logical: should the source formatting be retained When echoing expressions, If possible?</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Note that running code via source differs in a few respects from entering it at the R command line. Since expressions are not executed
    ''' at the top level, auto-printing is not done. So you will need to include explicit print calls for things you want to be printed
    ''' (and remember that this includes plotting by lattice, FAQ Q7.22). Since the complete file is parsed before any of it is run, syntax
    ''' errors result in none of the code being run. If an error occurs in running a syntactically correct script, anything assigned into the
    ''' workspace by code that has been run will be kept (just as from the command line), but diagnostic information such as traceback() will
    ''' contain additional calls to withVisible.
    '''
    ''' All versions Of R accept input from a connection With End Of line marked by LF (As used On Unix), CRLF (As used On DOS/Windows) Or CR
    ''' (As used On classic Mac OS) And map this To newline. The final line can be incomplete, that Is missing the final End-Of-line marker.
    '''
    ''' If keep.source Is True(the Default In interactive use), the source Of functions Is kept so they can be listed exactly As input.
    '''
    ''' Unlike input from a console, lines In the file Or On a connection can contain an unlimited number Of characters.
    '''
    ''' When skip.echo > 0, that many comment lines at the start of the file will Not be echoed. This does Not affect the execution of the code at all.
    ''' If there are executable lines within the first skip.echo lines, echoing will start with the first of them.
    '''
    ''' If echo Is True And a deparsed expression exceeds max.deparse.length, that many characters are output followed by .... [TRUNCATED] .
    '''
    ''' [Encodings]
    ''' By Default the input Is read And parsed In the current encoding Of the R session. This Is usually what it required, but occasionally re-encoding
    ''' Is needed, e.g. If a file from a UTF-8-Using system Is To be read On Windows (Or vice versa).
    '''
    ''' The rest Of this paragraph applies If file Is an actual filename Or URL (And Not "" nor a connection). If encoding = "unknown", an attempt Is
    ''' made To guess the encoding: the result Of localeToCharset() Is used As a guide. If encoding has two Or more elements, they are tried In turn
    ''' until the file/URL can be read without Error In the trial encoding. If an actual encoding Is specified (rather than the Default Or "unknown")
    ''' In a Latin-1 Or UTF-8 locale Then character strings In the result will be translated To the current encoding And marked As such (see Encoding).
    '''
    ''' If file Is a connection (including one specified by "", it Is Not possible To re-encode the input inside source, And so the encoding argument
    ''' Is just used To mark character strings In the parsed input In Latin-1 And UTF-8 locales: see parse.
    ''' </remarks>
    Public Function Source(file As String,
                           Optional local As Boolean = True,
                           Optional echo As Boolean = False,
                           Optional printEval As Boolean = False,
                           Optional verbose As Boolean = False,
                           Optional promptEcho As Boolean = False,
                           Optional maxDeparseLength As Integer = 150,
                           Optional chdir As Boolean = False,
                           Optional encoding As String = "unknown",
                           Optional continueEcho As Boolean = False,
                           Optional skipEcho As Integer = 0,
                           Optional keepSource As Boolean = False) As String()
        Dim cmdl As String = $"source(""{If(local, UnixPath(file), file)}"", local = {Rbool(local)}, echo = {Rbool(echo)}, print.eval = {Rbool(printEval)},
                               verbose = {Rbool(verbose)},
                               prompt.echo = {Rbool(promptEcho)},
                               max.deparse.length = {maxDeparseLength}, chdir = {Rbool(chdir)},
                               encoding = {Rstring(encoding)},
                               continue.echo = {Rbool(continueEcho)},
                               skip.echo = {skipEcho}, keep.source = {Rbool(keepSource)});"
        Return RServer.WriteLine(cmdl)
    End Function

    ''' <summary>
    ''' Load a available R package which was installed in the R system.(加载一个可用的R包)
    ''' </summary>
    ''' <param name="packageName"></param>
    ''' <remarks></remarks>
    Public Sub Library(packageName As String)
        Dim result As SymbolicExpression = RServer.Evaluate($"library({packageName})")
    End Sub

    ''' <summary>
    ''' Display the current working directory
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getwd() As String
        Return RServer.WriteLine("getwd()").JoinBy(vbCrLf)
    End Function

    Public Function setwd(workingDir As String) As String()
        Return RServer.WriteLine($"setwd(""{workingDir}"")")
    End Function

    ''' <summary>
    ''' [Sequence Generation] Generate regular sequences. seq is a standard generic with a default method.
    ''' </summary>
    ''' <param name="From">the starting and (maximal) end values of the sequence. Of length 1 unless just from is supplied as an unnamed argument.</param>
    ''' <param name="To">the starting and (maximal) end values of the sequence. Of length 1 unless just from is supplied as an unnamed argument.</param>
    ''' <param name="By">number: increment of the sequence</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Seq([From] As Integer, [To] As Integer, Optional By As Double = 1) As Integer()
        Dim Vector(([To] - From) / By) As Integer
        Vector(Scan0) = [From]
        For i As Integer = 1 To Vector.Count - 1
            Vector(i) = Vector(i - 1) + By
        Next

        Return Vector
    End Function

    ''' <summary>
    ''' 枚举R中所有的颜色代码
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RColors As String() = New String() {  ' 枚举所有的颜色
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

    Public Function ColorMaps(Of T)(source As Generic.IEnumerable(Of T)) As Dictionary(Of T, String)
        Dim uniques As T() = source.Distinct.ToArray
        Dim colors As String() = RColors.Randomize
        Dim dict As Dictionary(Of T, String) = (From idx As Integer
                                                In uniques.Sequence
                                                Select id = uniques(idx), cl = colors(idx)) _
                                                    .ToDictionary(Function(obj) obj.id, elementSelector:=Function(obj) obj.cl)
        Return dict
    End Function
End Module