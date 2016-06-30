Imports System.Xml.Serialization
Imports LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver.LDM

Namespace rFBA

    Public Class BaseEquation : Inherits LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver.Expression

        <XmlElement> Public Property Variables As Variable()

        Public Class Variable
            <XmlAttribute> Public Property pHandle As Integer
            <XmlAttribute> Public Property Pcc As Double
            <XmlAttribute> Public Property Weight As Double

            Protected Friend _Equation As BaseEquation
        End Class

        Public Overrides Function Evaluate() As Double
            Dim LQuery = (From item In Variables Select item.Pcc * (item._Equation._value) ^ item.Weight).Sum
            _value = LQuery
            Return LQuery
        End Function

        Public Overrides ReadOnly Property Value As Double
            Get
                Return _value
            End Get
        End Property

        Public Overrides Function get_ObjectHandle() As Framework.Kernel_Driver.DataStorage.FileModel.ObjectHandle
            Return New Framework.Kernel_Driver.DataStorage.FileModel.ObjectHandle With {.Handle = Handle, .Identifier = Identifier}
        End Function
    End Class

    Public Class NetworkModel : Inherits ModelBaseType

        <XmlElement> Public Property NetworkComponents As BaseEquation()
        <XmlAttribute> Public Property ObjectiveFunction As String()
        <XmlElement("include")> Public Property MetabolismHref As String
    End Class
End Namespace