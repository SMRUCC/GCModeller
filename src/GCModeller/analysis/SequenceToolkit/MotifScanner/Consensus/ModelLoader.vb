Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Data
Imports Microsoft.VisualBasic.Language.UnixBash

Public Module ModelLoader

    ''' <summary>
    ''' Load kegg organism model
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function LoadKEGGModels(repo As String) As Dictionary(Of String, OrganismModel)
        Return (ls - l - r - "*.Xml" <= repo) _
            .Select(AddressOf LoadXml(Of OrganismModel)) _
            .ToDictionary(Function(org)

                              ' 2018-2-10 因为这两个物种都具有相同的Taxonomy编号，所以在这里就不适合使用taxon_id来作为唯一标识符了
                              '
                              ' TAX: 611301
                              ' Xanthomonas citri subsp. citri mf20
                              ' TAX: 611301
                              ' Xanthomonas citri subsp. citri MN10

                              Return org.GetGenbankSource
                          End Function)
    End Function
End Module
