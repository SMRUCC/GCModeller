Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

''' <summary>
''' Module loader
''' </summary>
Public Class Loader

    Dim define As Definition
    Dim massTable As New MassTable

    Sub New(define As Definition)
        Me.define = define
    End Sub

    Public Function CreateEnvironment(cell As CellularModule) As Vessel
        Dim channels As New List(Of Channel)
        Dim rnaMatrix = cell.Genotype.RNAMatrix.ToDictionary(Function(r) r.geneID)
        Dim proteinMatrix = cell.Genotype.ProteinMatrix.ToDictionary(Function(r) r.proteinID)
        Dim templateDNA As Variable()
        Dim productsRNA As Variable()
        Dim templateRNA As Variable()
        Dim productsPro As Variable()

        ' 先构建一般性的中心法则过程
        For Each cd As CentralDogma In cell.Genotype.centralDogmas
            Call massTable.AddNew(cd.geneID)
            Call massTable.AddNew(cd.RNA.Name)
            Call massTable.AddNew(cd.polypeptide)

            templateDNA = transcriptionTemplate(cd.geneID, rnaMatrix)
            productsRNA = {
                massTable.variable(cd.RNA.Name)
            }
            templateRNA = translationTemplate(cd.RNA.Name, proteinMatrix)
            productsPro = {
                massTable.variable(cd.polypeptide)
            }

            channels += New Channel(templateDNA, productsRNA)
            channels += New Channel(templateRNA, productsPro)
        Next

        ' 构建酶成熟的过程
        For Each complex As Protein In cell.Phenotype.proteins
            For Each compound In complex.compounds
                If Not massTable.Exists(compound) Then
                    Call massTable.AddNew(compound)
                End If
            Next
            For Each peptide In complex.polypeptides
                If Not massTable.Exists(peptide) Then
                    Throw New MissingMemberException(peptide)
                End If
            Next

            Dim unformed = massTable.variables(complex)
            Dim mature = {massTable.variable(complex.ProteinID)}

            channels += New Channel(unformed, mature)
        Next

        ' 构建代谢网络
        For Each reaction As Reaction In cell.Phenotype.fluxes
            For Each compound In reaction.AllCompounds
                If Not massTable.Exists(compound) Then
                    Call massTable.AddNew(compound)
                End If
            Next

            Dim left = massTable.variables(reaction.substrates)
            Dim right = massTable.variables(reaction.products)

            channels += New Channel(left, right) With {
                .bounds = reaction.bounds,
                .ID = reaction.ID,
                .Forward = New Controls With {
                    .Activation = massTable.variables(reaction.enzyme, 1)
                },
                .Reverse = New Controls With {.baseline = 10}
            }
        Next

        Return New Vessel With {
            .Channels = channels,
            .Mass = massTable.ToArray
        }
    End Function

    Private Function transcriptionTemplate(geneID$, matrix As Dictionary(Of String, RNAComposition)) As Variable()
        Return matrix(geneID) _
            .Where(Function(i) i.Value > 0) _
            .Select(Function(base)
                        Dim baseName = define.NucleicAcid(base.Name)
                        Return massTable.variable(baseName, base.Value)
                    End Function) _
            .AsList + massTable.template(geneID)
    End Function

    Private Function translationTemplate(mRNA$, matrix As Dictionary(Of String, ProteinComposition)) As Variable()
        Return matrix(mRNA) _
            .Where(Function(i) i.Value > 0) _
            .Select(Function(aa)
                        Dim aaName = define.AminoAcid(aa.Name)
                        Return massTable.variable(aaName, aa.Value)
                    End Function) _
            .AsList + massTable.template(mRNA)
    End Function
End Class