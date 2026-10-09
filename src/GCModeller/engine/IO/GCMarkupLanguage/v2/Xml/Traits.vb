Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace v2

    ''' <summary>
    ''' the cellular phenotype traits
    ''' </summary>
    Public Class Traits

        ''' <summary>
        ''' the phenoetype terms that annotated for this cell model
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("phenotype")> Public Property phenotype As PhenotypeTrait()

        Public Overrides Function ToString() As String
            Return phenotype.GetJson
        End Function

    End Class

    Public Class PhenotypeTrait

        Public Property category As String
        Public Property accession As String
        Public Property unit As String
        Public Property data_type As String
        Public Property result As String
        Public Property confidence As Double
        Public Property score As Double
        Public Property cvScore As Double

    End Class
End Namespace