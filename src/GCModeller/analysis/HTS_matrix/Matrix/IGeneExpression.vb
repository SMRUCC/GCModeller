Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Interface IGeneExpression : Inherits IReadOnlyId

    ''' <summary>
    ''' the gene expression values across all samples
    ''' </summary>
    ''' <returns></returns>
    Property Expression As Dictionary(Of String, Double)

End Interface
