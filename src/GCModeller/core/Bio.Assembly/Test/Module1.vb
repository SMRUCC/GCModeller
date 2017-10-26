Imports SMRUCC.genomics.Assembly.EBI.ChEBI.XML

Module Module1

    Sub Main()
        Dim model = EntityList.LoadDirectory("D:\smartnucl_integrative\DATA\ChEBI\chebi_cache\")
        Dim list = model.AsList
        Dim table = model.ToSearchModel

        Call model.GetXml.SaveTo("./chebi-100.xml")
    End Sub
End Module
