Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.ComponentModels

Namespace GCML_Documents.XmlElements.Metabolism

    Public Class Pathway : Inherits T_MetaCycEntity(Of Slots.Pathway)

        ''' <summary>
        ''' Unique-Id
        ''' </summary>
        ''' <remarks></remarks>
        <XmlElement> Public Name As String

        ''' <summary>
        ''' Reaction Handles.(指向代谢组网络中的反应对象的句柄)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute("Reaction-List")> Public Property MetabolismNetwork As String()
        Public Comment As String

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", Identifier, Name)
        End Function

        Public Shared Function CastTo(e As Slots.Pathway) As Pathway
            Dim Pathway As Pathway = New Pathway With {.BaseType = e}
            Pathway.Name = e.CommonName
            Pathway.Identifier = e.Identifier
            Return Pathway
        End Function
    End Class
End Namespace