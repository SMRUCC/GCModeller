Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace v2

    Public Class Transportation : Implements INamedValue

        ''' <summary>
        ''' the reaction model reference id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property guid As String Implements INamedValue.Key
        <XmlAttribute> Public Property membrane As String()

        <XmlElement> Public Property enzymes As String()

        ''' <summary>
        ''' 转运过程的名称
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String

        ''' <summary>
        ''' 被转运的底物列表
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("substrate")> Public Property substrates As CompoundFactor()

        ''' <summary>
        ''' 被转运的产物列表
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("product")> Public Property products As CompoundFactor()

        ''' <summary>
        ''' 转运过程所使用的能量耦合方式：例如 ``symport``、``antiport``、``ATP``
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property couplingType As String

        ''' <summary>
        ''' 该转运过程所对应的反应编号
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("reaction")> Public Property reaction As String()

        Public Overrides Function ToString() As String
            Return $"{enzymes.GetJson}@{membrane.JoinBy(",")}"
        End Function

    End Class
End Namespace