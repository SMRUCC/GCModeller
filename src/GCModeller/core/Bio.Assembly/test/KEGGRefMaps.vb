Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Module KEGGRefMaps

    Sub Main()

        Call ReactionXMLLayout()

        Dim repo = MapRepository.BuildRepository("D:\KEGGMaps")

        Call repo.GetXml.SaveTo("./maps.XML")
    End Sub

    Sub ReactionXMLLayout()

        ReactionWebAPI.Download("R00002").GetXml.SaveTo("./test___reaction.xml")


        Pause()
    End Sub
End Module
