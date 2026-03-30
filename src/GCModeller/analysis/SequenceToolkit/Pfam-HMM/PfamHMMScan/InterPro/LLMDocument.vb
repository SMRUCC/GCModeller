Imports System.Xml.Serialization

Namespace InterPro.Xml

    Public MustInherit Class LLMDocument

        <XmlAttribute("is-llm")> Public Property is_llm As Boolean
        <XmlAttribute("is-llm-reviewed")> Public Property is_llm_reviewed As Boolean
    End Class
End Namespace