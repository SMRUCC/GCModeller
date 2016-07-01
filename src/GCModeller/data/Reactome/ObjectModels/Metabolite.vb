Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ObjectModels

    Public Class Metabolite : Implements IMetabolite, sIdEnumerable

        Public Property ChEBI As String() Implements IMetabolite.ChEBI
        Public Property KEGGCompound As String Implements IMetabolite.KEGGCompound

        Public Property Identifier As String Implements sIdEnumerable.Identifier
        Public Property CommonNames As String()
        Public Property MetaboliteType As MetaboliteTypes

        Public Enum MetaboliteTypes
            SmallMolecule
            Complex
        End Enum

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]  {1}", MetaboliteType.ToString, Identifier)
        End Function
    End Class
End Namespace