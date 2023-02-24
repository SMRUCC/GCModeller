
Imports System.Reflection
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.xml

Namespace Level3

    Public Class FileReader

        ReadOnly xml As XmlElement

        Shared ReadOnly schemaGraph As New Dictionary(Of String, PropertyInfo)

        Shared Sub New()
            Dim type As Type = GetType(File)
            Dim properties As PropertyInfo() = type.GetProperties _
                .Where(Function(p) p.CanWrite) _
                .ToArray

            For Each prop As PropertyInfo In properties
                Dim tag As XmlElementAttribute = prop.GetCustomAttribute(Of XmlElementAttribute)

                If tag Is Nothing OrElse tag.ElementName.StringEmpty Then
                    Call schemaGraph.Add(prop.Name, prop)
                Else
                    Call schemaGraph.Add(tag.ElementName, prop)
                End If
            Next
        End Sub

        Sub New(file As String)
            xml = XmlElement.ParseXmlText(file.SolveStream)
        End Sub

        Public Function GetObject() As File
            Dim elements = xml.elements _
                .SafeQuery _
                .GroupBy(Function(tag) tag.name) _
                .ToDictionary(Function(k) k.Key,
                              Function(array)
                                  Return array.ToArray
                              End Function)
            Dim file As New File

            For Each field In schemaGraph
                Dim array As XmlElement() = elements.TryGetValue(field.Key)

                If array.IsNullOrEmpty Then
                    Continue For
                End If

                If Not field.Value.PropertyType.IsArray Then

                End If
            Next

            Return file
        End Function


    End Class
End Namespace