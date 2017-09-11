Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.Uniprot.Web

Public Module Mappings

    <Extension>
    Public Function UserCustomMaps(DEGgenes As IEnumerable(Of EntityObject), tsv$) As EntityObject()
        Dim DEPgenes = DEGgenes.ToArray

        With tsv.MappingReader Or New Dictionary(Of String, String())().AsDefault
            If .Count > 0 Then
                ' 将用户基因号转换为uniprot编号
                For Each gene In DEPgenes
                    If .ContainsKey(gene.ID) Then
                        gene.ID = .ref(gene.ID).First
                    End If
                Next
            End If
        End With

        Return DEPgenes
    End Function
End Module
