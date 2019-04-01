Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

''' <summary>
''' Module loader
''' </summary>
Public Module Loader

    <Extension>
    Public Function CreateEnvironment(cell As CellularModule) As Vessel
        Dim channels As New List(Of Channel)
        Dim massTable As New Dictionary(Of String, Factor)

        ' 先构建一般性的中心法则过程
        For Each cd As CentralDogma In cell.Genotype.centralDogmas
            Call massTable.Add(cd.geneID, cd.geneID)
            Call massTable.Add(cd.RNA.Name, cd.RNA.Name)
            Call massTable.Add(cd.polypeptide, cd.polypeptide)

            channels += New Channel({massTable.template(cd.geneID)}, {massTable.variable(cd.RNA.Name)})
            channels += New Channel({massTable.template(cd.RNA.Name)}, {massTable.variable(cd.polypeptide)})
        Next

        ' 构建酶成熟的过程
        For Each complex As Protein In cell.Phenotype.proteins
            For Each compound In complex.compounds
                If Not massTable.ContainsKey(compound) Then
                    massTable(compound) = compound
                End If
            Next
            For Each peptide In complex.polypeptides
                If Not massTable.ContainsKey(peptide) Then
                    Throw New MissingMemberException(peptide)
                End If
            Next

            channels += New Channel(massTable.variables(complex), {massTable.variable(complex.ProteinID)})
        Next

        ' 构建代谢网络
        For Each reaction As Reaction In cell.Phenotype.fluxes
            For Each compound In reaction.AllCompounds
                If Not massTable.ContainsKey(compound) Then
                    massTable(compound) = compound
                End If
            Next

            Dim left = reaction.substrates.variables(massTable)
            Dim right = reaction.products.variables(massTable)

            channels += New Channel(left, right) With {
                .bounds = reaction.bounds,
                .ID = reaction.ID,
                .Forward = New Regulation With {
                    .Activation = reaction.enzyme.variables(massTable, 1)
                },
                .Reverse = New Regulation With {.baseline = 10}
            }
        Next

        Return New Vessel With {
            .Channels = channels,
            .Mass = massTable.Values.ToArray
        }
    End Function

    <Extension>
    Private Function variables(compounds As IEnumerable(Of String), massTable As Dictionary(Of String, Factor), factor As Double) As IEnumerable(Of Variable)
        Return compounds.Select(Function(cpd) massTable.variable(cpd, factor))
    End Function

    <Extension>
    Private Function variables(compounds As IEnumerable(Of FactorString(Of Double)), massTable As Dictionary(Of String, Factor)) As IEnumerable(Of Variable)
        Return compounds.Select(Function(cpd) massTable.variable(cpd.text, cpd.factor))
    End Function

    <Extension>
    Private Iterator Function variables(massTable As Dictionary(Of String, Factor), complex As Protein) As IEnumerable(Of Variable)
        For Each compound In complex.compounds
            Yield massTable.variable(compound)
        Next
        For Each peptide In complex.polypeptides
            Yield massTable.variable(peptide)
        Next
    End Function

    <Extension>
    Private Function variable(massTable As Dictionary(Of String, Factor), mass As String, Optional coefficient As Double = 1) As Variable
        Return New Variable(massTable(mass), coefficient, False)
    End Function

    <Extension>
    Private Function template(massTable As Dictionary(Of String, Factor), mass As String) As Variable
        Return New Variable(massTable(mass), 1, True)
    End Function

    ''' <summary>
    ''' 重置反应环境模拟器之中的内容
    ''' </summary>
    ''' <param name="envir"></param>
    ''' <param name="massInit"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Reset(envir As Vessel, massInit As Dictionary(Of String, Double)) As Vessel
        For Each mass As Factor In envir.Mass
            mass.Value = massInit(mass.ID)
        Next

        Return envir
    End Function
End Module
