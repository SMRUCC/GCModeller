Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.Assembly.NCBI.GenBank

Module gbTest
    Sub Main()
        Call dbXref2LocationGuid()
    End Sub

    Sub dbXref2LocationGuid()
        Dim gb = GBFF.File.Load("K:\20191112\wildtype\Yersinia_pseudotuberculosis_IP_32953..gbff")
        Dim xrefs As Index(Of String) = "K:\20191112\wildtype\EG\1025.txt".ReadAllLines
        Dim genes = gb.Features.Where(Function(f)
                                          Return f.Query("db_xref") Like xrefs
                                      End Function).Select(Function(g) g.Location.ToString).Distinct.ToArray

        Call genes.SaveTo("K:\20191112\wildtype\EG\1025_EG.txt")

        Pause()
    End Sub
End Module
