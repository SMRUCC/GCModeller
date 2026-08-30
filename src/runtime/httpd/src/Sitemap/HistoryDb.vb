Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Text.Encodings.Web
Imports System.Text.Json
Imports System.Text.Json.Serialization

''' <summary>
''' The page history record of a single url: it contains the md5 fingerprint
''' of the page content and the unix timestamp queue of the page content
''' update time.
'''
''' ```json
''' {
'''   "https://gcmodeller.org/index.html": {
'''     "md5": "d41d8cd98f00b204e9800998ecf8427e",
'''     "timestamp": [1756540800, 1756627200]
'''   }
''' }
''' ```
''' </summary>
Public Class PageRecord

    ''' <summary>
    ''' the md5 fingerprint of the page content
    ''' </summary>
    ''' <returns></returns>
    <JsonPropertyName("md5")>
    Public Property Md5 As String

    ''' <summary>
    ''' the unix timestamp queue of the page content update time, the
    ''' first element of this queue is the first time that this page have
    ''' been observed by the sitemap generator.
    ''' </summary>
    ''' <returns></returns>
    <JsonPropertyName("timestamp")>
    Public Property Timestamp As Long()

    ''' <summary>
    ''' get the last update time of this page in unix timestamp
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property LastUpdate As Long
        Get
            If Timestamp Is Nothing OrElse Timestamp.Length = 0 Then
                Return 0
            End If

            Return Timestamp.Max()
        End Get
    End Property

    ''' <summary>
    ''' how many times that the page content have been changed
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property UpdateCount As Integer
        Get
            Return If(Timestamp Is Nothing, 0, Timestamp.Length)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{Md5} x{UpdateCount}"
    End Function
End Class

''' <summary>
''' the summary report of a history database synchronization
''' </summary>
Public Class HistoryStats

    ''' <summary>
    ''' how many url that is the first time observed by the sitemap generator
    ''' </summary>
    ''' <returns></returns>
    Public Property Added As Integer
    ''' <summary>
    ''' how many page that its content have been changed since the last
    ''' build of the sitemap
    ''' </summary>
    ''' <returns></returns>
    Public Property Changed As Integer
    ''' <summary>
    ''' how many page that keeps the same content as the last build
    ''' </summary>
    ''' <returns></returns>
    Public Property Unchanged As Integer
    ''' <summary>
    ''' how many url record that is stored inside the history database,
    ''' this value includes the url that is not exists in the current
    ''' website anymore.
    ''' </summary>
    ''' <returns></returns>
    Public Property TotalPages As Integer

    Public Overrides Function ToString() As String
        Return $"{Added} added, {Changed} changed, {Unchanged} unchanged"
    End Function
End Class

''' <summary>
''' The json based history database of the sitemap generator.
'''
''' The database file is stored inside the output directory of the sitemap
''' generator, and the file name is started with a dot character, so that it
''' will be a hidden file in the linux filesystem.
''' </summary>
Public Class HistoryDb

    ''' <summary>
    ''' the url to the page history record map
    ''' </summary>
    ''' <returns></returns>
    Public Property Pages As Dictionary(Of String, PageRecord)

    ''' <summary>
    ''' the errors that is raised when loading or saving the database file
    ''' </summary>
    ''' <returns></returns>
    Public Property ErrorMessage As String

    Public Sub New()
        Pages = New Dictionary(Of String, PageRecord)
    End Sub

    Private Shared ReadOnly JsonOptions As JsonSerializerOptions = New JsonSerializerOptions With {
        .WriteIndented = True,
        .Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    }

    ''' <summary>
    ''' load the history database from a json file, an empty database will
    ''' be returns when the given file is not exists or the file content is
    ''' not a valid json document.
    ''' </summary>
    ''' <param name="saveTo"></param>
    ''' <returns></returns>
    Public Shared Function Load(saveTo As String) As HistoryDb
        If String.IsNullOrWhiteSpace(saveTo) OrElse Not System.IO.File.Exists(saveTo) Then
            Return New HistoryDb
        End If

        Try
            Dim json As String = System.IO.File.ReadAllText(saveTo)
            Dim pages As Dictionary(Of String, PageRecord) =
                JsonSerializer.Deserialize(Of Dictionary(Of String, PageRecord))(json, JsonOptions)

            If pages Is Nothing Then
                Return New HistoryDb
            End If

            Return New HistoryDb With {.Pages = pages}
        Catch ex As Exception
            Return New HistoryDb With {
                .ErrorMessage = $"the history database '{saveTo}' is corrupted: {ex.Message}"
            }
        End Try
    End Function

    ''' <summary>
    ''' compare the md5 fingerprint of each page with the history database,
    ''' and then push the current build time into the update timestamp queue
    ''' of the page that its content have been changed.
    ''' </summary>
    ''' <param name="entries">
    ''' the url entries that is produced by the crawler or the scanner
    ''' </param>
    ''' <param name="now">the current build time in unix timestamp</param>
    ''' <param name="maxTimestamps">
    ''' the max size limit of the update timestamp queue of a single url
    ''' </param>
    ''' <returns></returns>
    Public Function Sync(entries As IEnumerable(Of UrlEntry), now As Long, maxTimestamps As Integer) As HistoryStats
        Dim stats As New HistoryStats

        If entries Is Nothing Then
            Return stats
        End If

        For Each entry As UrlEntry In entries
            If entry Is Nothing OrElse String.IsNullOrWhiteSpace(entry.Loc) Then
                Continue For
            End If

            Dim loc As String = entry.Loc.Trim
            Dim fingerprint As String = entry.ContentMd5

            If String.IsNullOrEmpty(fingerprint) Then
                ' the page content is not available, skip the history record
                entry.UpdateTimes = If(entry.UpdateTimes, New Long(-1) {})
                Continue For
            End If

            If Pages.ContainsKey(loc) Then
                Dim record As PageRecord = Pages(loc)

                If record Is Nothing Then
                    record = New PageRecord
                    Pages(loc) = record
                End If

                If String.Equals(record.Md5, fingerprint, StringComparison.OrdinalIgnoreCase) Then
                    stats.Unchanged += 1
                Else
                    ' the page content have been changed since the last build
                    record.Md5 = fingerprint
                    record.Timestamp = appendTimestamp(record.Timestamp, now, maxTimestamps)

                    stats.Changed += 1
                End If

                entry.IsNewPage = False
            Else
                ' this is the first time that this url have been observed
                Call Pages.Add(loc, New PageRecord With {
                    .Md5 = fingerprint,
                    .Timestamp = {now}
                })

                entry.IsNewPage = True
                stats.Added += 1
            End If

            entry.UpdateTimes = If(Pages(loc).Timestamp, New Long(-1) {})
            entry.LastChanged = Pages(loc).LastUpdate
        Next

        stats.TotalPages = Pages.Count

        Return stats
    End Function

    ''' <summary>
    ''' push a new timestamp into the update timestamp queue, and then keeps
    ''' the most recent <paramref name="maxTimestamps"/> elements of the queue.
    ''' </summary>
    ''' <param name="queue"></param>
    ''' <param name="now"></param>
    ''' <param name="maxTimestamps"></param>
    ''' <returns></returns>
    Private Shared Function appendTimestamp(queue As Long(), now As Long, maxTimestamps As Integer) As Long()
        Dim list As New List(Of Long)

        If Not queue Is Nothing Then
            For Each time As Long In queue
                If time > 0 Then
                    Call list.Add(time)
                End If
            Next
        End If

        ' do not push a duplicated timestamp into the queue
        If list.Count = 0 OrElse list(list.Count - 1) <> now Then
            Call list.Add(now)
        End If

        Call list.Sort()

        If maxTimestamps > 0 AndAlso list.Count > maxTimestamps Then
            list = list.Skip(list.Count - maxTimestamps).ToList
        End If

        Return list.ToArray
    End Function

    ''' <summary>
    ''' save the history database as a json file
    ''' </summary>
    ''' <param name="saveTo"></param>
    ''' <returns></returns>
    Public Function Save(saveTo As String) As Boolean
        Try
            Dim dir As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(saveTo))

            If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
                Call Directory.CreateDirectory(dir)
            End If

            ' the url key is sorted in the ordinal order, so that the
            ' generated json document is determinate: the json file of the
            ' same data will always be the same bytes.
            Dim ordered As New SortedDictionary(Of String, PageRecord)(StringComparer.Ordinal)

            For Each page As KeyValuePair(Of String, PageRecord) In Pages
                If page.Key Is Nothing Then
                    Continue For
                End If

                ordered(page.Key) = page.Value
            Next

            Dim json As String = JsonSerializer.Serialize(ordered, JsonOptions)
            Dim tmp As String = saveTo & ".tmp"

            Call System.IO.File.WriteAllText(tmp, json, New UTF8Encoding(False))
            ' the atomic replace of the database file
            Call System.IO.File.Move(tmp, saveTo, True)

            Return True
        Catch ex As Exception
            ErrorMessage = ex.Message
            Return False
        End Try
    End Function

    Public Overrides Function ToString() As String
        Return $"{Pages.Count} url records"
    End Function
End Class
