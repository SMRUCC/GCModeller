Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' Some gene sequence is not exists in the KEGG database that can be download from this database.
''' http://www.microbesonline.org/cgi-bin/fetchLocus.cgi?locus=5219138&amp;disp=4
''' </summary>
''' 
<PackageNamespace("MicrobesOnline.fetchLocus",
                  Category:=APICategories.SoftwareTools,
                  Url:="http://www.microbesonline.org/cgi-bin/fetchLocus.cgi?locus=<gene_locus>&disp=4")>
Public Module fetchLocus

    Const FetchLocus As String = "http://www.microbesonline.org/cgi-bin/fetchLocus.cgi?locus={0}&disp=4"

    ''' <summary>
    ''' {prot, nt}
    ''' </summary>
    ''' <param name="locusId"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Fetch.Seq", Info:="Downloads nt and prot sequence from microbesonline database")>
    Public Function Downloads(locusId As String) _
        As <FunctionReturns("The return array has two elements, first element is the prot sequnece and the second element is the nt sequence.")> FastaToken()
        Dim page As String = String.Format(FetchLocus, locusId).GET
        Dim ms = Regex.Matches(page, "<pre>.+?</pre>", RegexOptions.Singleline Or RegexOptions.IgnoreCase)
        Dim seqs As String() = ms.ToArray(Function(x) Mid(x, 6).Replace("</pre>", ""))
        Dim lstFa As FastaToken() = seqs.ToArray(Function(seq) FastaToken.TryParse(seq))
        Return lstFa.Take(2).ToArray
    End Function

    <ExportAPI("Locus.Parser")>
    Public Function locusId(url As String) As String
        Dim id As String = Regex.Match(url, "locus=.+?&", RegexOptions.IgnoreCase).Value
        id = id.Split("="c).Last
        id = Mid(id, 1, Len(id) - 1)
        Return id
    End Function
End Module
