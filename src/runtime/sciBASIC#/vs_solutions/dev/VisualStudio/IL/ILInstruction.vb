Imports System
Imports System.Reflection
Imports System.Reflection.Emit

Namespace SDILReader
    Public Class ILInstruction
        ' Fields
        Private codeField As OpCode
        Private operandField As Object
        Private operandDataField As Byte()
        Private offsetField As Integer

        ' Properties
        Public Property Code As OpCode
            Get
                Return codeField
            End Get
            Set(ByVal value As OpCode)
                codeField = value
            End Set
        End Property

        Public Property Operand As Object
            Get
                Return operandField
            End Get
            Set(ByVal value As Object)
                operandField = value
            End Set
        End Property

        Public Property OperandData As Byte()
            Get
                Return operandDataField
            End Get
            Set(ByVal value As Byte())
                operandDataField = value
            End Set
        End Property

        Public Property Offset As Integer
            Get
                Return offsetField
            End Get
            Set(ByVal value As Integer)
                offsetField = value
            End Set
        End Property

        ''' <summary>
        ''' Returns a friendly strign representation of this instruction
        ''' </summary>
        ''' <returns></returns>
        Public Function GetCode() As String
            Dim result = ""
            result += GetExpandedOffset(offsetField) & " : " & codeField.ToString

            If operandField IsNot Nothing Then
                Select Case codeField.OperandType
                    Case OperandType.InlineField
                        Dim fOperand = CType(operandField, FieldInfo)
                        result += " " & ProcessSpecialTypes(fOperand.FieldType.ToString()) & " " & ProcessSpecialTypes(fOperand.ReflectedType.ToString()) & "::" & fOperand.Name & ""
                    Case OperandType.InlineMethod

                        Try
                            Dim mOperand = CType(operandField, MethodInfo)
                            result += " "
                            If Not mOperand.IsStatic Then result += "instance "
                            result += ProcessSpecialTypes(mOperand.ReturnType.ToString()) & " " & ProcessSpecialTypes(mOperand.ReflectedType.ToString()) & "::" & mOperand.Name & "()"
                        Catch

                            Try
                                Dim mOperand = CType(operandField, ConstructorInfo)
                                result += " "
                                If Not mOperand.IsStatic Then result += "instance "
                                result += "void " & ProcessSpecialTypes(mOperand.ReflectedType.ToString()) & "::" & mOperand.Name & "()"
                            Catch
                            End Try
                        End Try

                    Case OperandType.ShortInlineBrTarget, OperandType.InlineBrTarget
                        result += " " & GetExpandedOffset(CInt(operandField))
                    Case OperandType.InlineType
                        result += " " & ProcessSpecialTypes(operandField.ToString())
                    Case OperandType.InlineString

                        If Equals(operandField.ToString(), Microsoft.VisualBasic.Constants.vbCrLf) Then
                            result += " ""\r\n"""
                        Else
                            result += " """ & operandField.ToString() & """"
                        End If

                    Case OperandType.ShortInlineVar
                        result += operandField.ToString()
                    Case OperandType.InlineI, OperandType.InlineI8, OperandType.InlineR, OperandType.ShortInlineI, OperandType.ShortInlineR
                        result += operandField.ToString()
                    Case OperandType.InlineTok

                        If TypeOf operandField Is Type Then
                            result += CType(operandField, Type).FullName
                        Else
                            result += "not supported"
                        End If

                    Case Else
                        result += "not supported"
                End Select
            End If

            Return result
        End Function

        ''' <summary>
        ''' Add enough zeros to a number as to be represented on 4 characters
        ''' </summary>
        ''' <paramname="offset">
        ''' The number that must be represented on 4 characters
        ''' </param>
        ''' <returns>
        ''' </returns>
        Private Function GetExpandedOffset(ByVal offset As Long) As String
            Dim result As String = offset.ToString()
            Dim i = 0

            While result.Length < 4
                result = "0" & result
                i += 1
            End While

            Return result
        End Function

        Public Sub New()
        End Sub
    End Class
End Namespace
