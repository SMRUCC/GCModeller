Imports SMRUCC.genomics.Analysis.RetroPath.Chem

Namespace Search

    Public Class SearchState

        Public Pending As New List(Of (key As String, mol As Molecule))()
        Public Steps As New List(Of RetroStep)()
        Public Used As HashSet(Of String)

        Public Function StateKey() As String
            Return Pending.Select(Function(t) t.Item1).OrderBy(Function(x) x, StringComparer.Ordinal).JoinBy("|")
        End Function

        Public Function TotalAtoms() As Int32
            Return Aggregate tup In Pending Let mol = tup.mol Into Sum(mol.NumAtoms())
        End Function

    End Class

    Public Class SearchStats

        Public ApplicationsTried As Int64 = 0
        Public StatesGenerated As Int64 = 0
        Public RulesApplied As Int64 = 0
        Public MaxDepthReached As Int32 = 0
        Public ElapsedMs As Int64 = 0

    End Class
End Namespace