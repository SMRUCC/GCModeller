Imports System.Xml.Serialization

Namespace ComponentModel.Annotation

    ''' <summary>
    ''' the metaTraits prediciton result
    ''' </summary>
    Public Class PhenotypeTrait

        ''' <summary>
        ''' phenotype group category
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property category As String
        <XmlAttribute> Public Property accession As String
        ''' <summary>
        ''' data type of the phenotype trait value
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property data_type As String
        ''' <summary>
        ''' the phenotype trait annotation result, value could be logical value, number or string factors, based on the <see cref="data_type"/>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property result As String

        ''' <summary>
        ''' data unit of the annotated phenotype trait <see cref="result"/> value
        ''' </summary>
        ''' <returns></returns>
        Public Property unit As String
        ''' <summary>
        ''' SVM prediction confidence score of the annotated phenotype trait <see cref="result"/> value
        ''' </summary>
        ''' <returns></returns>
        Public Property confidence As Double
        ''' <summary>
        ''' SVM model score of the annotated phenotype trait <see cref="result"/> value
        ''' </summary>
        ''' <returns></returns>
        Public Property score As Double
        ''' <summary>
        ''' SVM cv score of the annotated phenotype trait <see cref="result"/> value
        ''' </summary>
        ''' <returns></returns>
        Public Property cvScore As Double

    End Class
End Namespace