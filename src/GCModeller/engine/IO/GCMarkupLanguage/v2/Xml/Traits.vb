Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.Annotation

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
End Namespace