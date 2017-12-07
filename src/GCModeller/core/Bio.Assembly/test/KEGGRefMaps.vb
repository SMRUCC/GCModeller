Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Module KEGGRefMaps

    Sub Main()
        Dim repo = MapRepository.BuildRepository("D:\KEGGMaps")

        Call repo.GetXml.SaveTo("./maps.XML")
    End Sub
End Module
