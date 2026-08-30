Imports System
Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' The weight factors of the page priority calculation, the sum of all of
''' the weight value is 1.0
''' </summary>
Public Class ScoreWeights

    ''' <summary>
    ''' the crawl depth level of the page: the shallower page is more
    ''' important than the deeper page.
    ''' </summary>
    ''' <returns></returns>
    Public Property Depth As Double = 0.3
    ''' <summary>
    ''' how frequently that the page content have been changed, this value
    ''' is calculated from the update timestamp queue of the page.
    ''' </summary>
    ''' <returns></returns>
    Public Property Freq As Double = 0.22
    ''' <summary>
    ''' how many in-site pages that have a hyper link point to the page
    ''' </summary>
    ''' <returns></returns>
    Public Property InLinks As Double = 0.22
    ''' <summary>
    ''' the freshness of the page content: the page that have been updated
    ''' recently is more important than the page that have not been changed
    ''' for a long time.
    ''' </summary>
    ''' <returns></returns>
    Public Property Recency As Double = 0.12
    ''' <summary>
    ''' the role of the page: the index page and the directory index page
    ''' is the hub page of the website.
    ''' </summary>
    ''' <returns></returns>
    Public Property Role As Double = 0.09
    ''' <summary>
    ''' the html document size of the page in bytes
    ''' </summary>
    ''' <returns></returns>
    Public Property Size As Double = 0.05

    ''' <summary>
    ''' the sum of all of the weight factors
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Total As Double
        Get
            Return Depth + Freq + InLinks + Recency + Role + Size
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"depth={Depth}, freq={Freq}, links={InLinks}, recency={Recency}, role={Role}, size={Size}"
    End Function
End Class

''' <summary>
''' The page priority engine of the sitemap generator: it calculates the
''' changefreq and the priority value of each page based on the update
''' frequency, the crawl depth level, the in-site link reference count, the
''' content freshness, the page role and the content size.
''' </summary>
Public Module PageScoring

    ''' <summary>
    ''' the file name of the directory index page
    ''' </summary>
    ReadOnly indexNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        "index.html", "index.htm", "default.html", "default.htm", "home.html", "home.htm"
    }

    ''' <summary>
    ''' the neutral score of the update frequency, it is used by the page
    ''' that does not have enough history data.
    ''' </summary>
    Const NeutralFreqScore As Double = 0.6
    ''' <summary>
    ''' the half life in days of the page content freshness decay
    ''' </summary>
    Const FreshnessHalfLife As Double = 180
    ''' <summary>
    ''' the min priority value of the sitemap protocol
    ''' </summary>
    Const MinPriority As Double = 0.1
    ''' <summary>
    ''' the max priority value of the sitemap protocol
    ''' </summary>
    Const MaxPriority As Double = 1.0

    ''' <summary>
    ''' calculate the changefreq and the priority value of every page in the
    ''' given website data.
    ''' </summary>
    ''' <param name="data">
    ''' the website data that is produced by the crawler or the scanner, the
    ''' <see cref="UrlEntry.UpdateTimes"/> and the <see cref="UrlEntry.LastChanged"/>
    ''' value of each entry should be filled by the history database at first.
    ''' </param>
    ''' <param name="defaultChangeFreq">
    ''' the changefreq value that is used by the page that does not have
    ''' enough history data.
    ''' </param>
    ''' <param name="now">
    ''' the current build time in unix timestamp, this value is used by the
    ''' page content freshness calculation.
    ''' </param>
    ''' <param name="weights">
    ''' the weight factors of the priority calculation, the default weight
    ''' factors will be used when this parameter value is Nothing.
    ''' </param>
    ''' <returns>
    ''' the distribution of the changefreq value: the key is the changefreq
    ''' text and the value is how many pages that own such changefreq value.
    ''' </returns>
    Public Function Apply(data As SiteData,
                          Optional defaultChangeFreq As String = "weekly",
                          Optional now As Long = 0,
                          Optional weights As ScoreWeights = Nothing) As Dictionary(Of String, Integer)

        Dim distribution As New Dictionary(Of String, Integer)

        If data Is Nothing OrElse data.Entries Is Nothing OrElse data.Entries.Count = 0 Then
            Return distribution
        End If

        If weights Is Nothing Then
            weights = New ScoreWeights
        End If

        If now <= 0 Then
            now = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        End If

        Dim entries As List(Of UrlEntry) = data.Entries
        Dim size As Integer = entries.Count
        Dim raws As Double() = New Double(size - 1) {}
        Dim maxInLinks As Integer = 0
        Dim maxSize As Integer = 0

        ' pass 1: fill the in-site link reference count and the page role,
        ' and then measure the max value of the relative score factors.
        For i As Integer = 0 To size - 1
            Dim entry As UrlEntry = entries(i)

            entry.InLinks = Math.Max(entry.InLinks, data.InLinkCountOf(entry.Loc))
            entry.Role = RoleOf(entry.Loc, data.BaseUrl, entry.Depth)
            entry.UpdateInterval = AvgIntervalDays(entry.UpdateTimes)

            If entry.InLinks > maxInLinks Then
                maxInLinks = entry.InLinks
            End If

            If entry.ContentSize > maxSize Then
                maxSize = entry.ContentSize
            End If
        Next

        Dim linkScale As Double = Math.Log(1 + Math.Max(maxInLinks, 1))
        Dim sizeScale As Double = Math.Log(1 + Math.Max(maxSize, 1))
        Dim maxRaw As Double = 0

        ' pass 2: calculate the raw score of each page
        For i As Integer = 0 To size - 1
            Dim entry As UrlEntry = entries(i)
            Dim depthScore As Double = 1.0 / (1.0 + 0.6 * Math.Max(0, entry.Depth))
            Dim freqScore As Double = If(entry.UpdateInterval > 0,
                FreqScoreOf(entry.UpdateInterval),
                NeutralFreqScore)
            Dim linkScore As Double = Math.Log(1 + Math.Max(0, entry.InLinks)) / linkScale
            Dim recencyScore As Double = RecencyScoreOf(entry.LastChanged, now)
            Dim sizeScore As Double = Math.Log(1 + Math.Max(0, entry.ContentSize)) / sizeScale

            Dim raw As Double = weights.Depth * depthScore +
                                weights.Freq * freqScore +
                                weights.InLinks * linkScore +
                                weights.Recency * recencyScore +
                                weights.Role * entry.Role +
                                weights.Size * sizeScore

            raws(i) = raw

            If raw > maxRaw Then
                maxRaw = raw
            End If
        Next

        ' pass 3: the raw score is anchored by the best page of this website:
        ' the most important page of the website gets the priority 1.0 and
        ' the least important page gets the priority 0.1
        For i As Integer = 0 To size - 1
            Dim entry As UrlEntry = entries(i)
            Dim changefreq As String

            If entry.UpdateInterval > 0 Then
                changefreq = ChangeFreqOf(entry.UpdateInterval)
            Else
                changefreq = normalizeFreq(defaultChangeFreq)
            End If

            entry.ChangeFreq = changefreq

            If maxRaw > 0 Then
                entry.Priority = Math.Round(clamp(MinPriority, MaxPriority, MinPriority + 0.9 * raws(i) / maxRaw), 2)
            Else
                entry.Priority = MinPriority
            End If

            If distribution.ContainsKey(changefreq) Then
                distribution(changefreq) += 1
            Else
                distribution(changefreq) = 1
            End If
        Next

        Return distribution
    End Function

    ''' <summary>
    ''' the role score of a page: the index page of the website gets 1.0,
    ''' the directory index page gets 0.6, the root level page gets 0.3 and
    ''' the other page gets zero.
    ''' </summary>
    ''' <param name="loc"></param>
    ''' <param name="baseUrl">the website root url, example as ``https://gcmodeller.org/``</param>
    ''' <param name="depth"></param>
    ''' <returns></returns>
    Public Function RoleOf(loc As String, baseUrl As String, depth As Integer) As Double
        If String.IsNullOrWhiteSpace(loc) Then
            Return 0
        End If

        Dim relative As String = UrlTool.urlPath(loc)

        If Not String.IsNullOrEmpty(baseUrl) AndAlso
            relative.StartsWith(baseUrl, StringComparison.OrdinalIgnoreCase) Then

            relative = relative.Substring(baseUrl.Length)
        End If

        relative = relative.Trim("/"c)

        If relative.Length = 0 Then
            ' the website root url
            Return 1.0
        End If

        Dim segments As String() = relative.Split("/"c)
        Dim fileName As String = segments(segments.Length - 1)

        If indexNames.Contains(fileName) Then
            ' the /index.html is the index page of the website, and the
            ' /docs/index.html is the index page of a directory
            Return If(segments.Length = 1, 1.0, 0.6)
        End If

        If segments.Length = 1 Then
            Return 0.3
        End If

        Return 0
    End Function

    ''' <summary>
    ''' the average update interval in time unit days of a page, this value
    ''' is calculated from the update timestamp queue of the page.
    ''' </summary>
    ''' <param name="updateTimes">
    ''' the unix timestamp queue of the page content update time
    ''' </param>
    ''' <returns>
    ''' this function returns zero when the given timestamp queue contains
    ''' less than two elements: there is no way to measure the update
    ''' interval from a single timestamp.
    ''' </returns>
    Public Function AvgIntervalDays(updateTimes As Long()) As Double
        If updateTimes Is Nothing OrElse updateTimes.Length < 2 Then
            Return 0
        End If

        Dim times As List(Of Long) = updateTimes _
            .Where(Function(t) t > 0) _
            .OrderBy(Function(t) t) _
            .ToList

        If times.Count < 2 Then
            Return 0
        End If

        Dim total As Double = 0

        For i As Integer = 1 To times.Count - 1
            Dim delta As Double = times(i) - times(i - 1)

            If delta > 0 Then
                total += delta
            End If
        Next

        Dim avgSeconds As Double = total / (times.Count - 1)

        Return avgSeconds / 86400.0
    End Function

    ''' <summary>
    ''' the continuous score of the page update frequency: the page that is
    ''' updated more frequently gets the bigger score value.
    ''' </summary>
    ''' <param name="avgDays">the average update interval in time unit days</param>
    ''' <returns>a value in range ``[0, 1]``</returns>
    Public Function FreqScoreOf(avgDays As Double) As Double
        ' the log10 scale of the update interval in range [1 hour, 2 years]
        ' is mapped into the score range [1, 0]
        Const maxLog As Double = 2.863   ' log10(730 days)
        Const minLog As Double = -1.398  ' log10(1/24 days)

        If avgDays <= 0 Then
            Return NeutralFreqScore
        End If

        Dim score As Double = (maxLog - Math.Log10(avgDays)) / (maxLog - minLog)

        Return clamp(0, 1, score)
    End Function

    ''' <summary>
    ''' map the average update interval into the changefreq value of the
    ''' sitemap protocol
    ''' </summary>
    ''' <param name="avgDays">the average update interval in time unit days</param>
    ''' <returns></returns>
    Public Function ChangeFreqOf(avgDays As Double) As String
        If avgDays <= 0 Then
            Return "weekly"
        ElseIf avgDays <= 1 / 144.0 Then      ' 10 minutes
            Return "always"
        ElseIf avgDays <= 1 / 24.0 Then       ' 1 hour
            Return "hourly"
        ElseIf avgDays <= 1 Then              ' 1 day
            Return "daily"
        ElseIf avgDays <= 7 Then              ' 1 week
            Return "weekly"
        ElseIf avgDays <= 30 Then             ' 1 month
            Return "monthly"
        ElseIf avgDays <= 365 Then            ' 1 year
            Return "yearly"
        Else
            Return "never"
        End If
    End Function

    ''' <summary>
    ''' the freshness score of the page content: the page that have been
    ''' updated recently gets the bigger score value.
    ''' </summary>
    ''' <param name="lastChanged">
    ''' the unix timestamp of the last content update time of the page, a
    ''' zero value means the page does not have any history data.
    ''' </param>
    ''' <param name="now">the current build time in unix timestamp</param>
    ''' <returns>a value in range ``[0, 1]``</returns>
    Public Function RecencyScoreOf(lastChanged As Long, now As Long) As Double
        If lastChanged <= 0 Then
            ' the page does not have any history data, it have just been
            ' observed by the sitemap generator
            Return 1.0
        End If

        Dim daysSince As Double = (now - lastChanged) / 86400.0

        If daysSince <= 0 Then
            Return 1.0
        End If

        Return clamp(0, 1, Math.Exp(-daysSince / FreshnessHalfLife))
    End Function

    Private Function normalizeFreq(freq As String) As String
        If String.IsNullOrWhiteSpace(freq) Then
            Return "weekly"
        End If

        Dim value As String = freq.Trim.ToLower

        Select Case value
            Case "always", "hourly", "daily", "weekly", "monthly", "yearly", "never"
                Return value
            Case Else
                Return "weekly"
        End Select
    End Function

    Private Function clamp(min As Double, max As Double, value As Double) As Double
        If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
            Return min
        End If

        If value < min Then
            Return min
        ElseIf value > max Then
            Return max
        End If

        Return value
    End Function
End Module
