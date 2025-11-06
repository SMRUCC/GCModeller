Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Namespace v2

    Public Class protein

        <XmlAttribute> Public Property protein_id As String

        <XmlAttribute> Public Property name As String

        <XmlElement> Public Property ligand As String()
        <XmlElement> Public Property peptide_chains As String()

        Public Property note As String

        Public Shared Iterator Function ProteinRoutine(list As protein(), protein_id As String, visited As Index(Of String)) As IEnumerable(Of protein)
            If Not protein_id Like visited Then
                Dim direct As protein() = list _
                    .SafeQuery _
                    .AsParallel _
                    .Where(Function(prot) prot.protein_id = protein_id) _
                    .ToArray
                Dim complex As protein() = list _
                    .SafeQuery _
                    .AsParallel _
                    .Where(Function(prot)
                               Return (Not prot.peptide_chains.IsNullOrEmpty) AndAlso
                                   prot.peptide_chains.Contains(protein_id)
                           End Function) _
                    .ToArray

                Call visited.Add(protein_id)

                For Each prot As protein In direct
                    Yield prot
                Next
                For Each prot As protein In complex
                    Yield prot

                    For Each find As protein In ProteinRoutine(list, prot.protein_id, visited)
                        Yield find
                    Next
                Next
            End If
        End Function

    End Class
End Namespace