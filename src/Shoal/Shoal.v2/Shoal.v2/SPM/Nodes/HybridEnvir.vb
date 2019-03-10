Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Scripting.ShoalShell.HTML

Namespace SPM.Nodes

    Public Class HybridEnvir : Inherits Assembly
        Implements HTML.IWikiHandle

        <XmlAttribute> Public Property Language As String
    End Class
End Namespace