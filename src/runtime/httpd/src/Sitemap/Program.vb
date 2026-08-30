Imports System.ComponentModel
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection

''' <summary>
''' GCModeller Sitemap Generator
'''
''' A command line tool for making the ``sitemap.xml`` file of a website.
''' The tool walks through the static pages of the target website by the
''' http crawler or by the local directory scanner, extracts all of the
''' in-site url from these pages, and then generates a standard
''' ``sitemap.xml`` file.
'''
''' A ``sitemap.xsl`` stylesheet file is generated together with the
''' ``sitemap.xml`` file, the visual style of this stylesheet file is
''' extracted from the css theme of the target website, so that the
''' sitemap page that is rendered by the browser will looks like a part
''' of your own website.
''' </summary>
Module Program

    ''' <summary>
    ''' the default output file name of the sitemap xml document
    ''' </summary>
    Const SitemapXml As String = "sitemap.xml"
    ''' <summary>
    ''' the default output file name of the sitemap xsl stylesheet
    ''' </summary>
    Const SitemapXsl As String = "sitemap.xsl"

    Public Function Main(args As String()) As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    ''' <summary>
    ''' make the sitemap.xml and the sitemap.xsl file for a website
    ''' </summary>
    ''' <param name="site">
    ''' the target website: it can be an online website url, example as
    ''' ``https://gcmodeller.org/``, or a local directory path of a static
    ''' website, example as ``G:\gcmodeller.org-website``.
    ''' </param>
    ''' <param name="host">
    ''' the website base url of the local static website, this parameter
    ''' is required when the <paramref name="site"/> value is a local
    ''' directory path.
    ''' </param>
    ''' <param name="out">
    ''' the output directory of the generated sitemap.xml and sitemap.xsl
    ''' file.
    ''' </param>
    ''' <param name="sleep">
    ''' the http request interval in time unit seconds, this parameter is
    ''' only working in the online http crawl mode.
    ''' </param>
    ''' <param name="depth">
    ''' the max crawl depth level of the web crawler, the index page of
    ''' the website is the level zero.
    ''' </param>
    ''' <param name="max_urls">
    ''' the max url entry size of the generated sitemap.xml file, the
    ''' sitemap protocol limits the url entry size to 50,000.
    ''' </param>
    ''' <param name="changefreq">
    ''' the default page update frequency value.
    ''' </param>
    ''' <param name="exclude">
    ''' the url exclude patterns, multiple patterns should be separated by
    ''' the ``|`` character, the wildcard ``*`` and ``?`` is supported.
    ''' </param>
    ''' <param name="args">
    ''' the rest of the command line arguments, it contains the theme
    ''' override switches and the boolean flags.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("/make")>
    <Description("Make the sitemap.xml file and the theme based sitemap.xsl stylesheet file for a website. The target website can be an online http website or a local static website directory, the tool will crawl the in-site url from the static pages of the target website automatically.")>
    <Usage("/make --site <url_or_local_wwwroot_dir> [--host <site_base_url> --out <output_dir> --sleep <seconds> --depth <level> --max_urls <size> --changefreq <freq> --exclude <pattern|pattern> --no-xsl --no-orphans --quiet]")>
    <Argument("--site", False, CLITypes.String,
        AcceptTypes:={GetType(String)},
        Description:="The target website to make sitemap. This parameter value can be an online website url, example as https://gcmodeller.org/, and then the tool will crawl the static pages of this website through the http protocol. This parameter value can also be a local directory path of a static website, example as G:\gcmodeller.org-website, and then the tool will scan the html files inside this directory instead of the http request.")>
    <Argument("--host", True, CLITypes.String,
        AcceptTypes:={GetType(String)},
        Description:="The website base url of the local static website directory, example as https://gcmodeller.org/. This parameter is required when the --site parameter value is a local directory path, the local html file path will be mapped as the website url based on this base url value. If this parameter is not specified in the local directory scan mode, then http://localhost/ will be used as the default website base url.")>
    <Argument("--out", True, CLITypes.File,
        AcceptTypes:={GetType(String)},
        Description:="The output directory path of the generated result files. A sitemap.xml file and a sitemap.xsl file will be generated inside this directory, the default value of this parameter is the current working directory.")>
    <Argument("--sleep", True, CLITypes.Double,
        AcceptTypes:={GetType(Double)},
        Description:="The thread sleep interval in time unit 'seconds' before the crawler fetch the next url page. This parameter only works in the online http crawl mode, a bigger sleep value makes the crawler more friendly to the web server of the target website. The default value of this parameter is 0.5 seconds, and a zero value means no sleep between two http requests.")>
    <Argument("--depth", True, CLITypes.Integer,
        AcceptTypes:={GetType(Integer)},
        Description:="The max crawl depth level of the http web crawler, the index page of the target website is the level zero. The default value of this parameter is 5, a bigger value will crawl more pages of the target website but it also takes much more time to finish the job.")>
    <Argument("--max_urls", True, CLITypes.Integer,
        AcceptTypes:={GetType(Integer)},
        Description:="The max url entry size limit of the generated sitemap.xml file. The default value of this parameter is 5000, and the sitemaps.org protocol limits the max url entry size of a single sitemap file to 50,000.")>
    <Argument("--changefreq", True, CLITypes.String,
        AcceptTypes:={GetType(String)},
        Description:="The default page update frequency value of the url entry. The available value of this parameter is: always, hourly, daily, weekly, monthly, yearly and never. The default value of this parameter is weekly.")>
    <Argument("--exclude", True, CLITypes.String,
        AcceptTypes:={GetType(String)},
        Description:="The url exclude patterns, multiple patterns should be separated by the '|' character, example as */vignettes/*|*/test/*. The wildcard character '*' and '?' is supported inside the pattern text, and a pattern text without any wildcard character will be tested as the url substring match.")>
    <Argument("--no-xsl", True, CLITypes.Boolean,
        Description:="Do not generate the sitemap.xsl stylesheet file and do not link the stylesheet file inside the generated sitemap.xml file.")>
    <Argument("--no-orphans", True, CLITypes.Boolean,
        Description:="Do not include the html files that are not linked by any other page of the website. This parameter only works in the local directory scan mode, the orphan pages are included in the sitemap file by default.")>
    <Argument("--quiet", True, CLITypes.Boolean,
        Description:="Do not print the crawl or scan progress message to the console stdout.")>
    <Argument("--theme", True, CLITypes.String,
        AcceptTypes:={GetType(String)},
        Description:="The website theme override switches of the generated sitemap.xsl stylesheet file. The available switches are: --primary, --bg, --surface, --text, --link, --font, --radius, --dark and --light. The color switch value should be a css color expression, example as #ff3b2f. If these switches are not specified, then the whole visual style of the sitemap.xsl file will be extracted from the css style of the target website automatically.")>
    Public Function MakeSitemap(site As String,
                                Optional host As String = Nothing,
                                Optional out As String = "./",
                                Optional sleep As Double = 0.5,
                                Optional depth As Integer = 5,
                                Optional max_urls As Integer = 5000,
                                Optional changefreq As String = "weekly",
                                Optional exclude As String = Nothing,
                                Optional args As CommandLine = Nothing) As Integer

        Dim verbose As Boolean = Not flag(args, "--quiet")
        Dim noXsl As Boolean = flag(args, "--no-xsl")

        If String.IsNullOrWhiteSpace(site) Then
            Call [error]("the --site parameter value can not be empty!")
            Return -1
        End If

        Dim patterns As String() = splitPatterns(exclude)
        Dim data As SiteData = LoadSite(site,
                                        host:=host,
                                        sleep:=sleep,
                                        depth:=If(depth <= 0, 1, depth),
                                        maxUrls:=If(max_urls <= 0, 50000, max_urls),
                                        changefreq:=changefreq,
                                        patterns:=patterns,
                                        includeOrphans:=Not flag(args, "--no-orphans"),
                                        verbose:=verbose)

        If data Is Nothing Then
            Call [error]($"the --site parameter value '{site}' is neither a valid http url nor an exists local directory path!")
            Return -404
        End If

        If data.Entries.Count = 0 Then
            Call warn("there is no in-site url that is found from the target website, the generated sitemap file will be empty.")
        End If

        Dim theme As SiteTheme = AnalyzeTheme(data, args)

        If verbose Then
            Call Console.WriteLine()
            Call Console.WriteLine($"website theme: {theme}")
            Call Console.WriteLine($"css documents: {data.CssTexts.Count}, css rules: {theme.CssRules}")
        End If

        Dim outDir As String = Path.GetFullPath(If(String.IsNullOrWhiteSpace(out), "./", out))
        Dim xmlPath As String = Path.Combine(outDir, SitemapXml)
        Dim xslPath As String = Path.Combine(outDir, SitemapXsl)

        Try
            Call SitemapWriter.Save(SitemapWriter.Build(data.Entries, If(noXsl, Nothing, SitemapXsl)), xmlPath)

            If Not noXsl Then
                Call XslTemplate.Save(XslTemplate.Build(theme, data.BaseUrl), xslPath)
            End If
        Catch ex As Exception
            Call [error](ex.Message)
            Return -500
        End Try

        Call Console.WriteLine()
        Call Console.WriteLine($"sitemap.xml  -> {xmlPath}")

        If Not noXsl Then
            Call Console.WriteLine($"sitemap.xsl  -> {xslPath}")
        End If

        Call Console.WriteLine($"{data.Entries.Count} url entries, {data.VisitedPages} pages visited.")

        Return 0
    End Function

    ''' <summary>
    ''' regenerate the sitemap.xsl stylesheet file only, the sitemap.xml
    ''' file will not be touched by this command.
    ''' </summary>
    ''' <param name="site">
    ''' the target website: an online website url or a local directory path
    ''' of a static website.
    ''' </param>
    ''' <param name="host"></param>
    ''' <param name="out"></param>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/xsl")>
    <Description("Regenerate the theme based sitemap.xsl stylesheet file only, the sitemap.xml file will not be touched by this command.")>
    <Usage("/xsl --site <url_or_local_wwwroot_dir> [--host <site_base_url> --out <output_dir>]")>
    <Argument("--site", False, CLITypes.String,
        AcceptTypes:={GetType(String)},
        Description:="The target website url or the local static website directory path, the website theme will be extracted from the css style of this website.")>
    <Argument("--host", True, CLITypes.String,
        AcceptTypes:={GetType(String)},
        Description:="The website base url of the local static website directory, example as https://gcmodeller.org/. This parameter is only used in the local directory scan mode.")>
    <Argument("--out", True, CLITypes.File,
        AcceptTypes:={GetType(String)},
        Description:="The output directory path of the generated sitemap.xsl file, the default value of this parameter is the current working directory.")>
    Public Function MakeStylesheet(site As String,
                                   Optional host As String = Nothing,
                                   Optional out As String = "./",
                                   Optional args As CommandLine = Nothing) As Integer

        If String.IsNullOrWhiteSpace(site) Then
            Call [error]("the --site parameter value can not be empty!")
            Return -1
        End If

        ' only the index page and the css files are required for the
        ' website theme extraction
        Dim data As SiteData = LoadSite(site,
                                        host:=host,
                                        sleep:=0,
                                        depth:=1,
                                        maxUrls:=2,
                                        changefreq:="weekly",
                                        patterns:=Nothing,
                                        includeOrphans:=False,
                                        verbose:=False)

        If data Is Nothing Then
            Call [error]($"the --site parameter value '{site}' is neither a valid http url nor an exists local directory path!")
            Return -404
        End If

        Dim theme As SiteTheme = AnalyzeTheme(data, args)
        Dim outDir As String = Path.GetFullPath(If(String.IsNullOrWhiteSpace(out), "./", out))
        Dim xslPath As String = Path.Combine(outDir, SitemapXsl)

        Try
            Call XslTemplate.Save(XslTemplate.Build(theme, data.BaseUrl), xslPath)
        Catch ex As Exception
            Call [error](ex.Message)
            Return -500
        End Try

        Call Console.WriteLine($"sitemap.xsl  -> {xslPath}")
        Call Console.WriteLine($"website theme: {theme}")

        Return 0
    End Function

    ''' <summary>
    ''' print the website theme that is extracted from the css style of
    ''' the target website, this command is a helper command for testing
    ''' the theme extraction result.
    ''' </summary>
    ''' <param name="site"></param>
    ''' <param name="host"></param>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/theme")>
    <Description("Print the website theme that is extracted from the css style of the target website, this command is a helper for testing the css theme extraction result.")>
    <Usage("/theme --site <url_or_local_wwwroot_dir> [--host <site_base_url>]")>
    <Argument("--site", False, CLITypes.String,
        AcceptTypes:={GetType(String)},
        Description:="The target website url or the local static website directory path for the website theme extraction.")>
    <Argument("--host", True, CLITypes.String,
        AcceptTypes:={GetType(String)},
        Description:="The website base url of the local static website directory, this parameter is only used in the local directory scan mode.")>
    Public Function PrintTheme(site As String,
                               Optional host As String = Nothing,
                               Optional args As CommandLine = Nothing) As Integer

        If String.IsNullOrWhiteSpace(site) Then
            Call [error]("the --site parameter value can not be empty!")
            Return -1
        End If

        Dim data As SiteData = LoadSite(site,
                                        host:=host,
                                        sleep:=0,
                                        depth:=1,
                                        maxUrls:=2,
                                        changefreq:="weekly",
                                        patterns:=Nothing,
                                        includeOrphans:=False,
                                        verbose:=False)

        If data Is Nothing Then
            Call [error]($"the --site parameter value '{site}' is neither a valid http url nor an exists local directory path!")
            Return -404
        End If

        Dim theme As SiteTheme = AnalyzeTheme(data, args)

        Call Console.WriteLine($"website title : {theme.SiteTitle}")
        Call Console.WriteLine($"theme mode    : {If(theme.IsDark, "dark", "light")}")
        Call Console.WriteLine($"background    : {theme.Background}")
        Call Console.WriteLine($"surface       : {theme.Surface}")
        Call Console.WriteLine($"primary       : {theme.Primary}")
        Call Console.WriteLine($"text          : {theme.TextColor}")
        Call Console.WriteLine($"muted text    : {theme.MutedText}")
        Call Console.WriteLine($"link          : {theme.LinkColor}")
        Call Console.WriteLine($"border        : {theme.BorderColor}")
        Call Console.WriteLine($"row alt       : {theme.RowAlt}")
        Call Console.WriteLine($"font family   : {theme.FontFamily}")
        Call Console.WriteLine($"border radius : {theme.Radius}")
        Call Console.WriteLine($"css documents : {data.CssTexts.Count}")
        Call Console.WriteLine($"css rules     : {theme.CssRules}")

        Return 0
    End Function

    ''' <summary>
    ''' crawl or scan the target website as the <see cref="SiteData"/>
    ''' model based on the type of the given site parameter value.
    ''' </summary>
    ''' <param name="site"></param>
    ''' <param name="host"></param>
    ''' <param name="sleep"></param>
    ''' <param name="depth"></param>
    ''' <param name="maxUrls"></param>
    ''' <param name="changefreq"></param>
    ''' <param name="patterns"></param>
    ''' <param name="includeOrphans"></param>
    ''' <param name="verbose"></param>
    ''' <returns>
    ''' this function returns Nothing if the given site parameter value is
    ''' neither a valid http url nor an exists local directory path.
    ''' </returns>
    Private Function LoadSite(site As String,
                              host As String,
                              sleep As Double,
                              depth As Integer,
                              maxUrls As Integer,
                              changefreq As String,
                              patterns As String(),
                              includeOrphans As Boolean,
                              verbose As Boolean) As SiteData

        Try
            If UrlTool.IsHttpUrl(site) Then
                Return New WebCrawler With {
                    .SleepSeconds = sleep,
                    .MaxDepth = depth,
                    .MaxUrls = maxUrls,
                    .ChangeFreq = changefreq,
                    .ExcludePatterns = patterns,
                    .Verbose = verbose
                }.Crawl(site)
            ElseIf Directory.Exists(site) Then
                If String.IsNullOrWhiteSpace(host) Then
                    Call warn("the --host parameter is not specified, using 'http://localhost/' as the website base url.")
                End If

                Return New StaticScanner With {
                    .Host = host,
                    .MaxUrls = maxUrls,
                    .IncludeOrphans = includeOrphans,
                    .ExcludePatterns = patterns,
                    .Verbose = verbose
                }.Scan(site)
            End If
        Catch ex As Exception
            Call [error](ex.Message)
            Return Nothing
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' analyze the website theme from the crawled or scanned css documents
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="args">
    ''' the command line arguments that contains the theme override values
    ''' </param>
    ''' <returns></returns>
    Private Function AnalyzeTheme(data As SiteData, args As CommandLine) As SiteTheme
        Dim themeOverride As ThemeOverride = ThemeOverride.Parse(args)

        If themeOverride Is Nothing Then
            themeOverride = New ThemeOverride
        End If

        ' the <meta name="color-scheme"> value of the index page is a
        ' strong hint of the website theme mode
        If themeOverride.IsDark Is Nothing AndAlso Not String.IsNullOrEmpty(data.ColorScheme) Then
            Dim scheme As String = data.ColorScheme.ToLower

            If scheme.IndexOf("dark", StringComparison.Ordinal) > -1 AndAlso
                scheme.IndexOf("light", StringComparison.Ordinal) = -1 Then

                themeOverride.IsDark = True
            ElseIf scheme.IndexOf("light", StringComparison.Ordinal) > -1 Then
                themeOverride.IsDark = False
            End If
        End If

        Return SiteTheme.Extract(
            cssTexts:=data.CssTexts,
            themeOverride:=If(themeOverride.IsEmptyOverride, Nothing, themeOverride),
            siteTitle:=data.SiteTitle)
    End Function

    Private Function flag(args As CommandLine, name As String) As Boolean
        If args Is Nothing Then
            Return False
        End If

        Return CBool(args(name))
    End Function

    Private Function splitPatterns(exclude As String) As String()
        If String.IsNullOrWhiteSpace(exclude) Then
            Return Nothing
        End If

        Return exclude _
            .Split("|"c) _
            .Select(Function(s) s.Trim) _
            .Where(Function(s) s.Length > 0) _
            .ToArray
    End Function

    Private Sub warn(message As String)
        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine($"[warn] {message}")
        Console.ResetColor()
    End Sub

    Private Sub [error](message As String)
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine($"[error] {message}")
        Console.ResetColor()
    End Sub
End Module
