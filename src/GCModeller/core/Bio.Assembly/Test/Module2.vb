Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Module Module2

    Sub Main()
        Dim form As New WebForm("https://rest.kegg.jp/get/hsa00010")
        Dim pathway = PathwayTextParser.ParsePathway(form)

        Pause()
    End Sub
End Module
