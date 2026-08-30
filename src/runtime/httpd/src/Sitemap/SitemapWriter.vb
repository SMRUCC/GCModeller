Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Xml
Imports System.Xml.Linq

''' <summary>
''' The sitemap.xml document builder, the generated xml document is
''' following the sitemaps.org protocol version 0.9 and it owns a
''' ``&lt;?xml-stylesheet?>`` processing instruction that links the
''' generated sitemap.xsl file.
''' </summary>
Public Module SitemapWriter

    ''' <summary>
    ''' the sitemaps.org protocol namespace
    ''' </summary>
    Public Const SitemapNamespace As String = "http://www.sitemaps.org/schemas/sitemap/0.9"
    ''' <summary>
    ''' the default xsl stylesheet file name
    ''' </summary>
    Public Const DefaultStyleSheet As String = "sitemap.xsl"

    ReadOnly changeFrequencies As New HashSet(Of String) From {
        "always", "hourly", "daily", "weekly", "monthly", "yearly", "never"
    }

    ''' <summary>
    ''' build the sitemap xml document from a collection of url entries
    ''' </summary>
    ''' <param name="urls"></param>
    ''' <param name="xslHref">
    ''' the relative url of the sitemap.xsl file, a Nothing value means
    ''' do not link any xsl stylesheet file in the generated xml document.
    ''' </param>
    ''' <returns></returns>
    Public Function Build(urls As IEnumerable(Of UrlEntry), Optional xslHref As String = DefaultStyleSheet) As XDocument
        Dim ns As XNamespace = SitemapNamespace
        Dim urlset As New XElement(ns + "urlset")
        Dim document As New XDocument(New XDeclaration("1.0", "UTF-8", Nothing), urlset)

        If Not String.IsNullOrWhiteSpace(xslHref) Then
            Call document.AddFirst(New XProcessingInstruction("xml-stylesheet", $"type=""text/xsl"" href=""{xslHref}"""))
        End If

        For Each url As UrlEntry In If(urls, Enumerable.Empty(Of UrlEntry)())
            If url Is Nothing OrElse String.IsNullOrWhiteSpace(url.Loc) Then
                Continue For
            End If

            Dim node As New XElement(ns + "url")

            Call node.Add(New XElement(ns + "loc", url.Loc.Trim))

            If Not String.IsNullOrWhiteSpace(url.LastMod) Then
                Call node.Add(New XElement(ns + "lastmod", url.LastMod.Trim))
            End If

            If Not String.IsNullOrWhiteSpace(url.ChangeFreq) AndAlso changeFrequencies.Contains(url.ChangeFreq.Trim.ToLower) Then
                Call node.Add(New XElement(ns + "changefreq", url.ChangeFreq.Trim.ToLower))
            End If

            Dim priority As Double = url.Priority

            If priority <= 0 Then
                priority = UrlEntry.PriorityOf(url.Depth)
            ElseIf priority > 1 Then
                priority = 1
            End If

            Call node.Add(New XElement(ns + "priority", priority.ToString("F2", Globalization.CultureInfo.InvariantCulture)))

            Call urlset.Add(node)
        Next

        Return document
    End Function

    ''' <summary>
    ''' save the sitemap xml document as a utf-8 encoded text file
    ''' </summary>
    ''' <param name="document"></param>
    ''' <param name="saveTo"></param>
    ''' <returns></returns>
    Public Function Save(document As XDocument, saveTo As String) As Boolean
        Dim settings As New XmlWriterSettings With {
            .Encoding = New UTF8Encoding(False),
            .Indent = True,
            .IndentChars = "  ",
            .OmitXmlDeclaration = False
        }

        Dim dir As String = Path.GetDirectoryName(Path.GetFullPath(saveTo))

        If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
            Call Directory.CreateDirectory(dir)
        End If

        Using writer As XmlWriter = XmlWriter.Create(saveTo, settings)
            Call document.Save(writer)
        End Using

        Return True
    End Function

    ''' <summary>
    ''' count the url entry size inside a sitemap xml document
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    Public Function CountUrls(path As String) As Integer
        Try
            Dim document As XDocument = XDocument.Load(path)
            Dim ns As XNamespace = SitemapNamespace

            Return document.Root.Elements(ns + "url").Count
        Catch ex As Exception
            Return 0
        End Try
    End Function
End Module
