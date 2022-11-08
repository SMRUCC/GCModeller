Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Data

Public Class Enzyme : Implements INamedValue

    Public Property protein_id As String Implements INamedValue.Key
    Public Property reactions As Rhea.Reaction()

    Public Overrides Function ToString() As String
        Return protein_id
    End Function

End Class
