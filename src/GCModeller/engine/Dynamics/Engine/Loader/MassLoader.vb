Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace Engine.ModelLoader

    Public Class MassLoader

        Public ReadOnly Property massTable As MassTable

        Public ReadOnly proteinComplex As New Dictionary(Of String, String)

        Sub New(loader As Loader)
            massTable = loader.massTable
        End Sub

        Public Sub doMassLoadingOn(cell As CellularModule)
            ' 在这里需要首选构建物质列表
            ' 否则下面的转录和翻译过程的构建会出现找不到物质因子对象的问题
            For Each reaction As Reaction In cell.Phenotype.fluxes
                For Each compound In reaction.AllCompounds
                    If Not massTable.Exists(compound) Then
                        Call massTable.AddNew(compound)
                    End If
                Next
            Next

            Dim complexID As String

            For Each complex As Protein In cell.Phenotype.proteins
                complexID = massTable.AddNew(complex.ProteinID & ".complex")
                proteinComplex.Add(complex.ProteinID, complexID)
            Next
        End Sub

    End Class
End Namespace