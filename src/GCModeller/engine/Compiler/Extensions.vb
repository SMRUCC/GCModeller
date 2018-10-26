Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports Excel = Microsoft.VisualBasic.MIME.Office.Excel.File
Imports XmlReaction = SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2.Reaction

Public Module Extensions

    <Extension> Public Function ToMarkup(model As CellularModule) As VirtualCell
        Return New VirtualCell With {
            .Taxonomy = model.Taxonomy,
            .MetabolismStructure = New MetabolismStructure With {
                .Reactions = model _
                    .Phenotype _
                    .fluxes _
                    .Select(Function(r)
                                Return New XmlReaction With {
                                    .ID = r.ID,
                                    .name = r.name,
                                    .Equation = r.GetEquationString
                                }
                            End Function) _
                    .ToArray
            }
        }
    End Function

    <Extension> Public Function ToTabular(model As CellularModule) As Excel

    End Function
End Module
