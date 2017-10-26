Imports SMRUCC.genomics.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.XML

Module chebiTest

    Sub Main()

        Dim tables As New TSVTables("D:\smartnucl_integrative\DATA\ChEBI\tsv")

        Dim alltypes = tables.GetChemicalData.Select(Function(c) c.TYPE).Distinct.JoinBy(";  ")

        Dim prope = tables.GetChemicalData().CreateProperty


        Dim model = EntityList.LoadDirectory("D:\smartnucl_integrative\DATA\ChEBI\chebi_cache\")
        Dim list = model.AsList
        Dim table = model.ToSearchModel

        Call model.GetXml.SaveTo("./chebi-100.xml")
    End Sub
End Module
