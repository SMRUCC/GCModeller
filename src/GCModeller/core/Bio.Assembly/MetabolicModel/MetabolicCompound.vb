Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace MetabolicModel

    Public Class MetabolicCompound : Implements INamedValue, IMolecule

        Public Property id As String Implements IKeyedEntity(Of String).Key, IMolecule.EntryId
        Public Property name As String Implements IMolecule.Name
        Public Property formula As String Implements IMolecule.Formula
        Public Property moleculeWeight As Double Implements IMolecule.Mass
        Public Property xref As DBLink()

        Public Overrides Function ToString() As String
            Return name
        End Function

    End Class
End Namespace