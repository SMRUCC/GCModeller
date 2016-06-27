Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel

Namespace Assembly.DOMINE

    <XmlRoot("DOMINE-DATABASE")>
    Public Class Database : Inherits ITextFile

        Sub New()
        End Sub

        Sub New(file As String)
            FilePath = file
        End Sub

        Public Property Interaction As DOMINE.Tables.Interaction()
        Public Property Pfam As DOMINE.Tables.Pfam()
        Public Property Go As DOMINE.Tables.Go()
        Public Property PGMap As DOMINE.Tables.PGMap()

        Public Overrides Function Save(Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean
            If String.IsNullOrEmpty(Path) Then
                Path = Me.FilePath
            End If

            Return Me.GetXml.SaveTo(Path, Encoding.ASCII)
        End Function

        Public Overloads Shared Widening Operator CType(FilePath As String) As DOMINE.Database
            Dim File As Database = FilePath.LoadXml(Of DOMINE.Database)()
            File.FilePath = FilePath
            Return File
        End Operator

        Public Function GetInteractionDomains(DomainId As String) As String()
            Dim LQuery = (From itr In Interaction Where String.Equals(itr.Domain1, DomainId) Select itr.Domain2).ToList
            LQuery += From itr In Interaction Where String.Equals(itr.Domain2, DomainId) Select itr.Domain1
            Return LQuery.ToArray
        End Function
    End Class
End Namespace