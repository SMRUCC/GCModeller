Imports System.Text.RegularExpressions
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.Expasy.Database.CsvExport

    Public Class Enzyme
        Public Property Identification As String
        Public Property Description As String
        Public Property AlternateName As String()
        Public Property Cofactor As String()
        Public Property PROSITE As String()
        Public Property Comments As String

        Public Overrides Function ToString() As String
            Return Identification
        End Function

        Public Shared Function CreateObject(EnzymeData As Database.Enzyme) As Enzyme
            Dim EnzymeObject As Enzyme =
                New Enzyme With {
                    .AlternateName = EnzymeData.AlternateName,
                    .Cofactor = EnzymeData.Cofactor,
                    .Comments = EnzymeData.Comments,
                    .Description = EnzymeData.Description,
                    .Identification = EnzymeData.Identification,
                    .PROSITE = EnzymeData.PROSITE
            }
            Return EnzymeObject
        End Function
    End Class

    Public Class SwissProt
        Public Property [Class] As String
        Public Property Entry As String
        Public Property seq As String

        Public Overrides Function ToString() As String
            Return Entry
        End Function

        Public Shared Function CreateObjects(Enzyme As Database.Enzyme) As SwissProt()
            Dim LQuery = (From Id As String
                          In Enzyme.SwissProt
                          Select New SwissProt With {
                              .Class = Enzyme.Identification,
                              .Entry = Id}).ToArray
            Return LQuery
        End Function
    End Class
End Namespace