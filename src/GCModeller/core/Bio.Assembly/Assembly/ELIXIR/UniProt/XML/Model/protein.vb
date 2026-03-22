Imports System.Xml.Serialization

Namespace Assembly.Uniprot.XML

    ''' <summary>
    ''' Describes the names for the protein and parts thereof.
    ''' Equivalent to the flat file DE-line.
    ''' </summary>
    Public Class protein

        Public Property recommendedName As recommendedName
        Public Property submittedName As recommendedName

        <XmlElement("alternativeName")>
        Public Property alternativeNames As recommendedName()

        ''' <summary>
        ''' <see cref="recommendedName"/> -> <see cref="submittedName"/> -> <see cref="alternativeNames"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property fullName As String
            Get
                If recommendedName Is Nothing OrElse recommendedName.fullName Is Nothing Then
                    If submittedName Is Nothing OrElse submittedName.fullName Is Nothing Then
                        Return alternativeNames.FirstOrDefault().fullName.value
                    Else
                        Return submittedName.fullName.value
                    End If
                Else
                    Return recommendedName.fullName.value
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return fullName
        End Function
    End Class
End Namespace