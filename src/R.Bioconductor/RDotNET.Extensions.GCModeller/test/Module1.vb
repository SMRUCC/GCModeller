Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports Microsoft.VisualBasic.Language.UnixBash
Imports RDotNET.Extensions.GCModeller

Module Module1

    Sub Main()

        ' Call MapDownloader.Downloads("./KEGGMaps/")


        Call (ls - l - r - "*.XML" <= "D:\smartnucl_integrative\biodeepDB\protocols\biodeepMSMS1\biodeepMSMS\data\KEGGMaps") _
            .Select(AddressOf LoadXml(Of Map)) _
            .SaveRda("D:\smartnucl_integrative\DATA\KEGG\medicus\drug\drug", "./KEGG.rda")
    End Sub
End Module
