Imports System.Xml.Serialization

Partial Class Guid

    Public Class RegistryNumberF

        ''' <summary>
        ''' A none sense string, generate from the Uid object.
        ''' </summary>
        ''' <remarks></remarks>
        <XmlIgnore> Friend Uid2 As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public SerialNumber As Integer

        Public Overrides Function ToString() As String
            Return String.Format("{0}{1}", Uid2, Format(SerialNumber, "00000000"))
        End Function

        ''' <summary>
        ''' Do nothing in this object type constructor. 
        ''' </summary>
        ''' <remarks></remarks>
        Friend Sub New(Id As Integer)
            SerialNumber = Id
            Uid2 = ""
        End Sub

        Friend Sub New()
            Uid2 = ("")
        End Sub

        Public Shared Operator =(a As RegistryNumberF, b As RegistryNumberF) As Boolean
            Return a.SerialNumber = b.SerialNumber
        End Operator

        Public Shared Operator <>(a As RegistryNumberF, b As RegistryNumberF) As Boolean
            Return Not (a.SerialNumber = b.SerialNumber)
        End Operator

#Region "Type cast methods"
        ''' <summary>
        ''' Convert a serials string expression to a serials object instance.
        ''' (将一个序列号字符串转换为一个序列号对象实例)
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Widening Operator CType(s As String) As RegistryNumberF
            Return New RegistryNumberF With {.SerialNumber = Val(Mid(s, 5)), .Uid2 = Mid(s, 1, 4)}
        End Operator

        Public Shared Narrowing Operator CType(e As RegistryNumberF) As Integer
            Return e.SerialNumber
        End Operator

        Public Shared Narrowing Operator CType(e As RegistryNumberF) As String
            Return String.Format("{0}{1}", e.Uid2, Format(e.SerialNumber, "00000000"))
        End Operator
#End Region
    End Class
End Class