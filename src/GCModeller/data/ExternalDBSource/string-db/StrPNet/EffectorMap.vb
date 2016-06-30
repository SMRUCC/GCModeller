Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace StringDB.StrPNet

    ''' <summary>
    ''' Regprecise Effector与MetaCyc Compounds Mapping
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EffectorMap : Implements sIdEnumerable

        <Column("regprecise-effector")>
        Public Property Effector As String Implements sIdEnumerable.Identifier
        Public Property MetaCycId As String
        <Collection("Effector-Alias")>
        Public Property EffectorAlias As String()

        Public Overrides Function ToString() As String
            Return String.Format("{0} --> {1}", Effector, MetaCycId)
        End Function
    End Class
End Namespace