Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Partial Module CLI

    <ExportAPI("/Reads.OTU.Taxonomy",
               Usage:="/Reads.OTU.Taxonomy /in <blastnMaps.csv> /OTU <OTU_data.csv> /tax <taxonomy:nodes/names> [/out <out.csv>]")>
    Public Function ReadsOTU_Taxonomy(args As CommandLine) As Integer

    End Function
End Module