Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Utilities

Namespace OokLanguage

    <Export(GetType(ITaggerProvider)), ContentType("ook!"), TagType(GetType(OokTokenTag))>
    Friend NotInheritable Class OokTokenTagProvider
        Implements ITaggerProvider

        Public Function CreateTagger(Of T As ITag)(buffer As ITextBuffer) As ITagger(Of T) Implements ITaggerProvider.CreateTagger
#Disable Warning
            Return TryCast(New OokTokenTagger(buffer), ITagger(Of T))
#Enable Warning
        End Function
    End Class
End Namespace