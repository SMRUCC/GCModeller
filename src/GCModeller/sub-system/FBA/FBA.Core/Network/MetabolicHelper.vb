Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra.LinearProgramming
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Public Module MetabolicHelper

    ''' <summary>
    ''' build FBA analysis matrix from a collection of the metabolic reaction
    ''' equation model.
    ''' </summary>
    ''' <param name="gem"></param>
    ''' <param name="bounds">
    ''' the flux bounds of each metabolic reaction, by default is (-10,10) for
    ''' all of the reactions. for a real GEM model, the irreversible reaction
    ''' should be [0, ub] and the reversible reaction should be [lb, ub] with
    ''' a negative lower bound.
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' the stoichiometric matrix of a metabolic network is a highly sparse
    ''' matrix, so that this function builds the matrix in the CSR sparse
    ''' matrix format in O(nnz) time, the dense jagged matrix is created in a
    ''' lazy manner to avoid the out of memory error on the genome scale model.
    ''' </remarks>
    <Extension>
    Public Function BuildMatrix(gem As IReadOnlyCollection(Of Equation), Optional bounds As DoubleRange = Nothing) As Matrix
        If bounds Is Nothing Then
            bounds = New DoubleRange(-10, 10)
        End If

        Dim reactions As Equation() = gem.ToArray
        Dim compounds As String() = reactions _
            .Select(Function(r) r.GetMetabolites) _
            .IteratesALL _
            .Select(Function(sp) sp.ID) _
            .Distinct _
            .OrderBy(Function(id) id) _
            .ToArray
        Dim compoundIndex As Dictionary(Of String, Integer) = compounds _
            .Select(Function(id, i) (id, i)) _
            .ToDictionary(Function(t) t.id, Function(t) t.i)
        Dim stoichiometry As LpSparseMatrix = BuildStoichiometry(reactions, compoundIndex)

        Return New Matrix With {
            .Stoichiometry = stoichiometry,
            .Compounds = compounds,
            .Flux = reactions _
                .ToDictionary(Function(flux) flux.Id,
                              Function(flux)
                                  Return New DoubleRange(bounds.Min, bounds.Max)
                              End Function),
            .Targets = reactions.Select(Function(r) r.Id).ToArray
        }
    End Function

    ''' <summary>
    ''' build the stoichiometric matrix in the CSR sparse format
    ''' </summary>
    Private Function BuildStoichiometry(reactions As Equation(),
                                        compoundIndex As Dictionary(Of String, Integer)) As LpSparseMatrix

        Dim rows As New List(Of Integer)(reactions.Length * 8)
        Dim cols As New List(Of Integer)(reactions.Length * 8)
        Dim vals As New List(Of Double)(reactions.Length * 8)
        Dim coefficients As New Dictionary(Of Integer, Double)()

        For j As Integer = 0 To reactions.Length - 1
            Dim reaction As Equation = reactions(j)

            coefficients.Clear()

            For Each metabolite In reaction.GetMetabolites
                Dim row As Integer = -1

                If Not compoundIndex.TryGetValue(metabolite.ID, row) Then
                    Continue For
                End If

                Dim coefficient As Double = reaction.GetCoEfficient(metabolite.ID, directional:=True)

                ' the same compound may be appeared in the both side of the
                ' reaction equation, so that the coefficient is overwritten
                ' here, which keeps the same behavior as the dense version.
                coefficients(row) = coefficient
            Next

            For Each coefficient As KeyValuePair(Of Integer, Double) In coefficients
                If coefficient.Value <> 0.0 Then
                    rows.Add(coefficient.Key)
                    cols.Add(j)
                    vals.Add(coefficient.Value)
                End If
            Next
        Next

        Return LpSparseMatrix.FromTriplets(
            rows:=compoundIndex.Count,
            columns:=reactions.Length,
            rowIdx:=rows.ToArray,
            colIdx:=cols.ToArray,
            vals:=vals.ToArray
        )
    End Function
End Module
