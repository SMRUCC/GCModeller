Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace MetabolicModel

    Public Class MetabolicPathway : Implements INamedValue

        Public Property ID As String Implements IKeyedEntity(Of String).Key
        Public Property name As String
        Public Property metabolites As MetabolicCompound()
        Public Property metabolicNetwork As MetabolicReaction()

    End Class
End Namespace