Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace MetabolicModel

    Public Class MetabolicEnzyme : Implements INamedValue

        Public Property ID As String Implements IKeyedEntity(Of String).Key

    End Class
End Namespace