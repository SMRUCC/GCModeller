Imports System.Text
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME
Imports Microsoft.VisualBasic

Namespace GCML_Documents.XmlElements.Bacterial_GENOME

    Public Class MotifSite

        ''' <summary>
        ''' 请使用GUID进行赋值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property MotifName As String
        ''' <summary>
        ''' 以ATG为界限的位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute("ATG_Distance")> Public Property SitePosition As Integer

        <XmlElement("TF", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/transcript_factor")>
        Public Property Regulators As List(Of SignalTransductions.Regulator)

        Public Overrides Function ToString() As String
            Return MotifName
        End Function
    End Class
End Namespace


