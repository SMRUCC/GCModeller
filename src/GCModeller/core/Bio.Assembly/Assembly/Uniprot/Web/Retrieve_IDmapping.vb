Imports System.Collections.Specialized
Imports System.Threading
Imports Microsoft.VisualBasic.Text.HtmlParser

Namespace Assembly.Uniprot.Web

    Public Module Retrieve_IDmapping

        Public Function Mapping(uploadQuery As IEnumerable(Of String), from As IdTypes, [to] As IdTypes)
            Dim args As New NameValueCollection

            Call args.Add(NameOf(from), from.ToString)
            Call args.Add(NameOf([to]), [to].ToString)
            Call args.Add(NameOf(uploadQuery), uploadQuery.JoinBy(vbLf))

            Dim url$ = "http://www.uniprot.org/uploadlists/"
            Dim html As String = url.PostRequest(args, "http://www.uniprot.org/uploadlists/",)
            Dim query$ = html.HTMLTitle.Split.First

            Call Thread.Sleep(1000)

            ' http://www.uniprot.org/uniprot/
            ' query=yourlist:M20170110ACFE4208EAFA842A78A1B3BA7138A93D9543F8P
            ' sort=yourlist:M20170110ACFE4208EAFA842A78A1B3BA7138A93D9543F8P
            ' columns=yourlist(M20170110ACFE4208EAFA842A78A1B3BA7138A93D9543F8P),isomap(M20170110ACFE4208EAFA842A78A1B3BA7138A93D9543F8P),id,entry%20name,reviewed,protein%20names,genes,organism,length
            url = "http://www.uniprot.org/uniprot/?"
            url &= "query=" & query & "&"
            url &= "sort=" & query & "&"
            query = query.Split(":"c).Last
            url &= $"columns=yourlist({query}),isomap({query}),id,entry%20name,reviewed,protein%20names,genes,organism,length"
            html = url.GET()

            Throw New NotImplementedException
        End Function
    End Module

    Public Enum Formats
        ''' <summary>
        ''' FASTA (canonical)
        ''' </summary>
        canonical
        ''' <summary>
        ''' FASTA (canonical &amp; isoform)
        ''' </summary>
        isoform
        ''' <summary>
        ''' Tab-separated
        ''' </summary>
        tab
        ''' <summary>
        ''' Text
        ''' </summary>
        txt
        ''' <summary>
        ''' Excel
        ''' </summary>
        xlsx
        ''' <summary>
        ''' GFF
        ''' </summary>
        gff
        ''' <summary>
        ''' XML
        ''' </summary>
        xml
        ''' <summary>
        ''' Mapping Table
        ''' </summary>
        mappingTable
        ''' <summary>
        ''' RDF/XML
        ''' </summary>
        rdf
        ''' <summary>
        ''' Target List
        ''' </summary>
        list
    End Enum

    Public Enum IdTypes
        ''' <summary>
        ''' UniProtKB
        ''' </summary>
        ACC
        ''' <summary>
        ''' UniRef90
        ''' </summary>
        NF90
    End Enum
End Namespace