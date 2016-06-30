Imports Microsoft.VisualBasic.Linq

Namespace Interpro.Xml

    Public Class Family
        Public Property Interpro As String
        Public Property Name As String
        Public Property Pfam As String()
        Public Property Includes As String()

        Public Overrides Function ToString() As String
            Return $"[{Interpro}] {Name}; //{Pfam.JoinBy(", ")}"
        End Function

        Public Shared Function CreateObject(interpro As Xml.Interpro, dict As Dictionary(Of String, Xml.Interpro)) As Family
            Dim includes = interpro.contains.ToArray(Function(x) dict(x.ipr_ref))
            Dim Pfam = (From x In interpro.member_list
                        Where String.Equals(x.db, "PFAM", StringComparison.OrdinalIgnoreCase)
                        Select x.dbkey).ToArray
            Dim LQuery = (From inter As Xml.Interpro
                          In includes
                          Select (From d As DbXref
                                  In inter.member_list
                                  Where String.Equals(d.db, "PFAM", StringComparison.OrdinalIgnoreCase)
                                  Select d.dbkey).ToArray).ToArray.MatrixToVector

            Return New Family With {
                .Interpro = interpro.id,
                .Name = interpro.short_name,
                .Pfam = Pfam,
                .Includes = LQuery
            }
        End Function
    End Class
End Namespace