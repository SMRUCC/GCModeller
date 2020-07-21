Imports System.Reflection
Imports System.Reflection.Emit

Namespace IL

    Public Class ILInstruction

        Public Property Code As OpCode
        Public Property Operand As Object
        Public Property OperandData As Byte()
        Public Property Offset As Integer

        ''' <summary>
        ''' Returns a friendly strign representation of this instruction
        ''' </summary>
        ''' <returns></returns>
        Public Function GetCode() As String
            Dim result = ""
            result += GetExpandedOffset(Offset) & " : " & Code.ToString

            If Operand IsNot Nothing Then
                Select Case Code.OperandType
                    Case OperandType.InlineField
                        Dim fOperand = CType(Operand, FieldInfo)
                        result += " " & ProcessSpecialTypes(fOperand.FieldType.ToString()) & " " & ProcessSpecialTypes(fOperand.ReflectedType.ToString()) & "::" & fOperand.Name & ""
                    Case OperandType.InlineMethod

                        Try
                            Dim mOperand = CType(Operand, MethodInfo)
                            result += " "
                            If Not mOperand.IsStatic Then result += "instance "
                            result += ProcessSpecialTypes(mOperand.ReturnType.ToString()) & " " & ProcessSpecialTypes(mOperand.ReflectedType.ToString()) & "::" & mOperand.Name & "()"
                        Catch

                            Try
                                Dim mOperand = CType(Operand, ConstructorInfo)
                                result += " "
                                If Not mOperand.IsStatic Then result += "instance "
                                result += "void " & ProcessSpecialTypes(mOperand.ReflectedType.ToString()) & "::" & mOperand.Name & "()"
                            Catch
                            End Try
                        End Try

                    Case OperandType.ShortInlineBrTarget, OperandType.InlineBrTarget
                        result += " " & GetExpandedOffset(CInt(Operand))
                    Case OperandType.InlineType
                        result += " " & ProcessSpecialTypes(Operand.ToString())
                    Case OperandType.InlineString

                        If Equals(Operand.ToString(), vbCrLf) Then
                            result += " ""\r\n"""
                        Else
                            result += " """ & Operand.ToString() & """"
                        End If

                    Case OperandType.ShortInlineVar
                        result += Operand.ToString()
                    Case OperandType.InlineI, OperandType.InlineI8, OperandType.InlineR, OperandType.ShortInlineI, OperandType.ShortInlineR
                        result += Operand.ToString()
                    Case OperandType.InlineTok

                        If TypeOf Operand Is Type Then
                            result += CType(Operand, Type).FullName
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
        Private Function GetExpandedOffset(offset As Long) As String
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
