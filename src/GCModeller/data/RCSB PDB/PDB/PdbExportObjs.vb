Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Public Class PdbItem
    Public Property PdbId As String
    Public Property ChainId As String

    Public Overrides Function ToString() As String
        Return String.Format("{0}-{1}", PdbId, ChainId)
    End Function
End Class

Public Class PdbComplexesAssembly
    <Column("PdbId")> Public Property PdbId As String
    Public Property ChainId As String
    <Collection("InteractionChainId", "; ")>
    Public Property InteractionChainId As String()

    Public Overrides Function ToString() As String
        Return String.Format("{0}-{1}", PdbId, ChainId)
    End Function

    Public Overloads Function Equals(Entry As PdbItem) As Boolean
        Return String.Equals(Entry.ChainId, ChainId, StringComparison.OrdinalIgnoreCase) AndAlso
            String.Equals(Entry.PdbId, PdbId, StringComparison.OrdinalIgnoreCase)
    End Function
End Class