
Imports System.Xml.Serialization

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' 反应左端代谢物在经过了代谢反应之后结果上的转换变化的结果（反应的右端）
    ''' </summary>
    Public Class ReactionCompoundTransform

        ''' <summary>
        ''' the kegg compound id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Overridable Property from As String

        ''' <summary>
        ''' the kegg compound id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Overridable Property [to] As String

        Public Overrides Function ToString() As String
            Return $"{from}->{[to]}"
        End Function
    End Class
End Namespace