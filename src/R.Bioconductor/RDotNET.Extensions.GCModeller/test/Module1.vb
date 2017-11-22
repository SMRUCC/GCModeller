Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports Microsoft.VisualBasic.Language.UnixBash
Imports RDotNET.Extensions.GCModeller

Module Module1

    Sub Main()

        Call MapDownloader.Downloads("./KEGGMaps/")


        Call (ls - l - r - "*.XML" <= "./KEGGMaps/") _
            .Select(AddressOf LoadXml(Of Map)) _
            .SaveRda("./KEGG.rda")
    End Sub
End Module
