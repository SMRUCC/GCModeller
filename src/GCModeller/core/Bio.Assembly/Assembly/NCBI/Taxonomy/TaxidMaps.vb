Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Assembly.NCBI.Taxonomy

    Public Module TaxidMaps

        Public Delegate Function Mapping(id$) As Integer

        <Extension> Public Function MapByAcc(acc2taxid$) As Mapping
            Dim taxids As BucketDictionary(Of String, Integer) =
                ReadFile(acc2taxid) _
                .CreateBuckets(Function(x) x.Name,
                               Function(x) x.x)
            Return Function(acc$) If(taxids.ContainsKey(acc), taxids(acc), -1)
        End Function

        Public Function MapByGI(gi2taxid$) As Mapping
            Dim taxids As BucketDictionary(Of Integer, Integer) = Taxonomy.AcquireAuto(gi2taxid)

            Return Function(sgi$)
                       Dim gi% = CInt(Val(sgi$))
                       Return If(taxids.ContainsKey(gi), taxids(gi), -1)
                   End Function
        End Function

#Region "NT header parser"

        Public Function GetAccessionId(header$) As String
            Return header _
                .Split _
                .First _
                .Split("."c) _
                .First
        End Function
#End Region
    End Module
End Namespace