Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Regprecise

    Public Class Effectors

        Public Property Effector As String
        Public Property TF As String
        Public Property Regulon As String
        Public Property Pathway As String
        Public Property BiologicalProcess As String
        Public Property KEGG As String
        Public Property MetaCyc As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function Fill(compound As ICompoundObject) As Effectors
            If String.IsNullOrEmpty(MetaCyc) Then
                MetaCyc = compound.Identifier
            End If
            Return Me
        End Function
    End Class
End Namespace