Imports System.IO

''' <summary>
''' A unified url entry model that is produced either by the http web crawler
''' or by the local static website directory scanner.
''' </summary>
Public Class UrlEntry

    ''' <summary>
    ''' The absolute url location of this page, this value should be an
    ''' absolute http url in the target website.
    ''' </summary>
    ''' <returns></returns>
    Public Property Loc As String
    ''' <summary>
    ''' Last modified time of the page in format ``yyyy-MM-dd``
    ''' </summary>
    ''' <returns></returns>
    Public Property LastMod As String
    ''' <summary>
    ''' The change frequency value: always, hourly, daily, weekly, monthly, yearly, never
    ''' </summary>
    ''' <returns></returns>
    Public Property ChangeFreq As String
    ''' <summary>
    ''' A value in range ``[0, 1]``, the bigger the value means the more
    ''' important of the page.
    ''' </summary>
    ''' <returns></returns>
    Public Property Priority As Double
    ''' <summary>
    ''' The crawl depth level of this page, the index page is level zero.
    ''' </summary>
    ''' <returns></returns>
    Public Property Depth As Integer
    ''' <summary>
    ''' the html ``&lt;title>`` text of the page, this value is optional
    ''' and is only used for the rendering of the sitemap.xsl page.
    ''' </summary>
    ''' <returns></returns>
    Public Property Title As String

    ''' <summary>
    ''' The local filesystem path of this page, this value is only available
    ''' in the local directory scan mode.
    ''' </summary>
    ''' <returns></returns>
    Public Property LocalFile As String

    ''' <summary>
    ''' The md5 fingerprint of the page content, this value is the input of
    ''' the history database: the page content is treated as changed when
    ''' this fingerprint value is not equals to the value that is stored
    ''' inside the history database.
    ''' </summary>
    ''' <returns></returns>
    Public Property ContentMd5 As String
    ''' <summary>
    ''' The html document size of this page in characters
    ''' </summary>
    ''' <returns></returns>
    Public Property ContentSize As Integer
    ''' <summary>
    ''' How many in-site pages that owns a hyper link point to this page
    ''' </summary>
    ''' <returns></returns>
    Public Property InLinks As Integer
    ''' <summary>
    ''' The role score of this page in range ``[0, 1]``: the index page gets
    ''' 1.0, the directory index page gets 0.6 and the root level page gets
    ''' 0.3.
    ''' </summary>
    ''' <returns></returns>
    Public Property Role As Double
    ''' <summary>
    ''' The unix timestamp queue of the page content update time, this value
    ''' is loaded from the history database.
    ''' </summary>
    ''' <returns></returns>
    Public Property UpdateTimes As Long()
    ''' <summary>
    ''' The unix timestamp of the last content update time of this page
    ''' </summary>
    ''' <returns></returns>
    Public Property LastChanged As Long
    ''' <summary>
    ''' Is this url the first time that observed by the sitemap generator?
    ''' </summary>
    ''' <returns></returns>
    Public Property IsNewPage As Boolean
    ''' <summary>
    ''' The average update interval of this page in time unit days, this
    ''' value is calculated from the <see cref="UpdateTimes"/> queue.
    ''' </summary>
    ''' <returns></returns>
    Public Property UpdateInterval As Double

    Public Overrides Function ToString() As String
        Return $"[{Priority.ToString("F2")}] {Loc}"
    End Function

    ''' <summary>
    ''' calculate the crawl depth level of a given url path based on its
    ''' relative path segments from the website root.
    ''' </summary>
    ''' <param name="relative">
    ''' A relative path or relative url, example as ``vignettes/GCModeller.html``
    ''' </param>
    ''' <returns></returns>
    Public Shared Function DepthOf(relative As String) As Integer
        If String.IsNullOrEmpty(relative) Then
            Return 0
        End If

        Dim path As String = relative.Replace("\"c, "/"c).Trim("/"c)

        ' strip the query and the anchor parts
        Dim cut As Integer = path.IndexOfAny({"?"c, "#"c})

        If cut > 0 Then
            path = path.Substring(0, cut)
        End If

        If path.Length = 0 Then
            Return 0
        End If

        Dim segments As String() = path.Split("/"c)
        Dim level As Integer = 0

        For Each segment As String In segments
            If segment.Length = 0 OrElse segment = "." OrElse segment = ".." Then
                Continue For
            End If

            level += 1
        Next

        ' the file name itself is not a directory level
        Return Math.Max(0, level - 1)
    End Function

    ''' <summary>
    ''' the priority value will be decreased by the crawl depth level:
    ''' the index page gets 1.0 and the deeper page gets the smaller value.
    ''' </summary>
    ''' <param name="depth"></param>
    ''' <returns></returns>
    Public Shared Function PriorityOf(depth As Integer) As Double
        If depth <= 0 Then
            Return 1.0
        End If

        Dim value As Double = 1.0 - depth * 0.15

        If value < 0.2 Then
            value = 0.2
        End If

        Return Math.Round(value, 2)
    End Function

    ''' <summary>
    ''' get the last modified time string in the sitemap protocol required
    ''' format: ``yyyy-MM-dd``
    ''' </summary>
    ''' <param name="time"></param>
    ''' <returns></returns>
    Public Shared Function LastModOf(time As DateTime) As String
        Return time.ToString("yyyy-MM-dd")
    End Function

    ''' <summary>
    ''' get the last modified time of a local html file, if the file is not
    ''' exists on the local filesystem then returns the current time.
    ''' </summary>
    ''' <param name="pageFile"></param>
    ''' <returns></returns>
    Public Shared Function LastModOf(pageFile As String) As String
        If pageFile Is Nothing OrElse Not System.IO.File.Exists(pageFile) Then
            Return LastModOf(DateTime.Now)
        End If

        Return LastModOf(New FileInfo(pageFile).LastWriteTime)
    End Function
End Class
