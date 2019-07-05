Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Tasks.Models

    ''' <summary>
    ''' 和Query的一个比对结果
    ''' </summary>
    Public Class Hit : Implements INamedValue

        ''' <summary>
        ''' <see cref="HitName"></see>所在的物种
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property tag As String
        ''' <summary>
        ''' 和query蛋白质比对上的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property hitName As String Implements INamedValue.Key
        <XmlAttribute> Public Property identities As Double
        <XmlAttribute> Public Property positive As Double

        Public Overrides Function ToString() As String
            Return $"[{tag}] {hitName} {{identities:= {identities}, positive:= {positive}}}"
        End Function
    End Class
End Namespace