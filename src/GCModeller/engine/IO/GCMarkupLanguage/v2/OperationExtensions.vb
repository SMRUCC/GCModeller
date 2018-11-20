Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace v2

    Public Module OperationExtensions

        ''' <summary>
        ''' 基因组之中的基因发生了缺失突变
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="geneList$"></param>
        ''' <returns></returns>
        <Extension> Public Function DeleteMutation(model As VirtualCell, geneList$()) As VirtualCell
            Dim deleted As Index(Of String) = geneList

            For Each replicon As replicon In model.genome.replicons
                replicon.genes = replicon.genes.Where(Function(g) Not g.locus_tag.IsOneOfA(deleted)).ToArray
            Next

            model.genome.regulations = model.genome _
                .regulations _
                .Where(Function(reg)
                           Return Not reg.regulator.IsOneOfA(deleted) AndAlso Not reg.target.IsOneOfA(deleted)
                       End Function) _
                .ToArray
            model.MetabolismStructure.Enzymes = model.MetabolismStructure _
                .Enzymes _
                .Where(Function(enz) Not enz.geneID.IsOneOfA(deleted)) _
                .ToArray

            For Each [module] As FunctionalCategory In model.MetabolismStructure.maps
                For Each pathway As Pathway In [module].pathways
                    pathway.enzymes = pathway _
                        .enzymes _
                        .Where(Function(enz) Not enz.Comment.IsOneOfA(deleted)) _
                        .ToArray
                Next
            Next

            Return model
        End Function
    End Module
End Namespace