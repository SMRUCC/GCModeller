Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    ''' <summary>
    ''' 为CDS字段记录所特化的对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CDS : Inherits Feature

        Public Property db_xref_GI As String
        Public Property db_xref_GO As String
        Public Property db_xref_InterPro As String()
        Public Property db_xref_UniprotKBSwissProt As String
        Public Property db_xref_UniprotKBTrEMBL As String

        Public Property DBLinks As MetaCyc.Schema.DBLinkManager

        Sub New(CDS_Feature As Feature)
            Me.Location = CDS_Feature.Location
            Me.KeyName = CDS_Feature.KeyName

            Call CDS_Feature.CopyTo(Me.__innerList)

            Dim db_xref = (From str As String In Me.QueryDuplicated("db_xref")
                           Let Tokens As String() = str.Split(":"c)
                           Select Key = Tokens.First, Value = Tokens.Last
                           Group By Key Into Group).ToArray.ToDictionary(keySelector:=Function(item) item.Key, elementSelector:=Function(item) (From obj In item.Group Select obj.Value).ToArray)

            Dim TempChunk As String() = Nothing

            Call db_xref.TryGetValue("GI", TempChunk) : If Not TempChunk.IsNullOrEmpty Then db_xref_GI = TempChunk.First
            Call db_xref.TryGetValue("GOA", TempChunk) : If Not TempChunk.IsNullOrEmpty Then db_xref_GO = TempChunk.First
            Call db_xref.TryGetValue("UniProtKB/Swiss-Prot", TempChunk) : If Not TempChunk.IsNullOrEmpty Then db_xref_UniprotKBSwissProt = TempChunk.First
            Call db_xref.TryGetValue("UniProtKB/TrEMBL", TempChunk) : If Not TempChunk.IsNullOrEmpty Then db_xref_UniprotKBTrEMBL = TempChunk.First
            Call db_xref.TryGetValue("InterPro", db_xref_InterPro)

            Me.gbRaw = CDS_Feature.gbRaw
        End Sub
    End Class
End Namespace