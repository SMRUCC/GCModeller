
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports Microsoft.VisualBasic.Linq

Namespace Ptf

    Public NotInheritable Class AnnotationReader

        Private Sub New()
        End Sub

        Public Shared Function KO(protein As ProteinAnnotation) As String
            If protein.attributes.ContainsKey("ko") Then
                Return protein("ko")
            Else
                Return ""
            End If
        End Function

        Public Shared Function Pfam(protein As entry) As String
            Return protein.features _
                .SafeQuery _
                .Where(Function(f) f.type = "domain") _
                .Select(Function(d)
                            Return $"{d.description}({d.location.begin.position}|{d.location.end.position})"
                        End Function) _
                .JoinBy("+")
        End Function
    End Class
End Namespace