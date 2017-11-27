Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports Microsoft.VisualBasic.Language.UnixBash
Imports RDotNET.Extensions.GCModeller
Imports SMRUCC.genomics.Assembly.KEGG.Medical

Module Module1

    Sub Main()

        ' Call MapDownloader.Downloads("./KEGGMaps/")
        Dim drugMaps = DrugParser _
            .ParseStream("D:\smartnucl_integrative\DATA\KEGG\medicus\drug\drug") _
            .BuildDrugCompoundMaps(DrugParser.LoadDrugGroup("D:\smartnucl_integrative\DATA\KEGG\medicus\dgroup\dgroup"))

        Call (ls - l - r - "*.XML" <= "D:\smartnucl_integrative\biodeepDB\protocols\biodeepMSMS1\biodeepMSMS\data\KEGGMaps") _
            .Select(AddressOf LoadXml(Of Map)) _
            .SaveRda(drugMaps, "./KEGG.rda")
    End Sub
End Module
