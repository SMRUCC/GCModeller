Imports System.Reflection
Imports System.Reflection.Emit

Namespace IL
    Public Class ILInstruction

        ' Properties
        Public Property CodeField As OpCode
        Public Property Operand As Object
        Public Property OperandData As Byte()
        Public Property Offset As Integer?

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
        ''' <param name="offset">
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
