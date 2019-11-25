Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace Engine.ModelLoader

    ''' <summary>
    ''' 构建酶成熟的过程
    ''' </summary>
    Public Class ProteinMatureFluxLoader : Inherits FluxLoader

        Public ReadOnly Property polypeptides As String()
        Public ReadOnly Property proteinComplex As String()

        Public Sub New(loader As Loader)
            MyBase.New(loader)
        End Sub

        Public Overrides Iterator Function CreateFlux(cell As CellularModule) As IEnumerable(Of Channel)
            Dim polypeptides As New List(Of String)
            Dim proteinComplex As New List(Of String)

            For Each complex As Protein In cell.Phenotype.proteins
                For Each compound In complex.compounds
                    If Not MassTable.Exists(compound) Then
                        Call MassTable.AddNew(compound)
                    End If
                Next
                For Each peptide In complex.polypeptides
                    If Not MassTable.Exists(peptide) Then
                        Throw New MissingMemberException(peptide)
                    Else
                        polypeptides += peptide
                    End If
                Next

                Dim unformed = MassTable.variables(complex).ToArray
                Dim complexID As String = MassTable.AddNew(complex.ProteinID & ".complex")
                Dim mature As Variable = MassTable.variable(complexID)

                proteinComplex += complexID

                ' 酶的成熟过程也是一个不可逆的过程
                Yield New Channel(unformed, {mature}) With {
                    .ID = complex.DoCall(AddressOf Loader.GetProteinMatureId),
                    .bounds = New Boundary With {.forward = 1000, .reverse = 0},
                    .reverse = New Controls With {.baseline = 0},
                    .forward = New Controls With {.baseline = 10}
                }
            Next

            _polypeptides = polypeptides
            _proteinComplex = proteinComplex
        End Function
    End Class
End Namespace