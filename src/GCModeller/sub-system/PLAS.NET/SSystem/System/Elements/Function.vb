Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Kernel.ObjectModels

    ''' <summary>
    ''' User define function
    ''' </summary>
    Public Structure [Function] : Implements sIdEnumerable

        ''' <summary>
        ''' The function name
        ''' </summary>
        <XmlAttribute> Public Property Name As String Implements sIdEnumerable.Identifier

        ''' <summary>
        ''' [function name](args) expression
        ''' </summary>
        <XmlAttribute> Public Property Declaration As String

        Public Overrides Function ToString() As String
            Return $"{Name} <- {Declaration}"
        End Function

        Public Shared Widening Operator CType(s As String) As [Function]
            Dim Tokens = s.Split
            Dim [Function] = New [Function]

            [Function].Name = Tokens(1)
            [Function].Declaration = Mid(s, 7 + Len([Function].Name))

            Return [Function]
        End Operator
    End Structure

    Public Structure Constant : Implements IKeyValuePairObject(Of String, String)

        <XmlAttribute> Public Property Name As String Implements IKeyValuePairObject(Of String, String).Identifier
        <XmlAttribute> Public Property Expression As String Implements IKeyValuePairObject(Of String, String).Value

        Public Overrides Function ToString() As String
            Return String.Format("CONST {0} {1}", Name, Expression)
        End Function
    End Structure
End Namespace