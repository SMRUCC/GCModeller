Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Assembly.NCBI.Taxonomy

    Public Module TaxidMaps

        Public Delegate Function Mapping(id$) As Integer

        <Extension> Public Function MapByAcc(acc2taxid$) As Mapping
            Dim taxids As BucketDictionary(Of String, Integer) =
                ReadFile(acc2taxid) _
                .CreateBuckets(Function(x) x.Name,
                               Function(x) x.Value)
            Return Function(acc$) If(taxids.ContainsKey(acc), taxids(acc), -1)
        End Function

        Public Function MapByGI(gi2taxid$) As Mapping
            Dim taxids As BucketDictionary(Of Integer, Integer) = Taxonomy.AcquireAuto(gi2taxid)

            Return Function(sgi$)
                       Dim gi% = CInt(Val(sgi$))
                       Return If(taxids.ContainsKey(gi), taxids(gi), -1)
                   End Function
        End Function

        ''' <summary>
        ''' 对默认的nt库的fasta标题进行解析操作
        ''' </summary>
        ''' <param name="mapping"></param>
        ''' <param name="is_gi2taxid"></param>
        ''' <returns></returns>
        Public Function Reference2Taxid(mapping As Mapping, is_gi2taxid As Boolean) As Mapping
            If is_gi2taxid Then
                Return Function(ref$)
                           Dim gis$ = Regex.Match(ref, "gi\|\d+").Value
                           Dim gi$ = gis.Split("|"c).LastOrDefault

                           If String.IsNullOrEmpty(gi) Then
                               Call ref.PrintException
                               Return -1
                           End If
                           Return mapping(gi)
                       End Function
            Else
                Return Function(ref$)
                           Dim acc$ = GetAccessionId(ref)

                           If String.IsNullOrEmpty(acc) Then
                               Call ref.PrintException
                               Return -1
                           End If
                           Return mapping(acc)
                       End Function
            End If
        End Function

#Region "NT header parser"

        ''' <summary>
        ''' 从标准的nt fasta标题之中解析出``accessionId``.
        ''' </summary>
        ''' <param name="header$"></param>
        ''' <returns></returns>
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