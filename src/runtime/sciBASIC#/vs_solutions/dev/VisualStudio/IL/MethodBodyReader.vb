Imports System.Reflection
Imports System.Reflection.Emit
Imports System.Threading
Imports stdNum = System.Math

Namespace IL

    ''' <summary>
    ''' Parsing the IL of a Method Body
    ''' 
    ''' > https://www.codeproject.com/articles/14058/parsing-the-il-of-a-method-body
    ''' </summary>
    Public Class MethodBodyReader
        Public instructions As List(Of ILInstruction) = Nothing
        Protected il As Byte() = Nothing
        Private mi As MethodInfo = Nothing

#Region "il read methods"
        Private Function ReadInt16(ByVal _il As Byte(), ByRef position As Integer) As Integer
            Return il(stdNum.Min(Interlocked.Increment(position), position - 1)) Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << 8
        End Function

        Private Function ReadUInt16(ByVal _il As Byte(), ByRef position As Integer) As UShort
            Return il(stdNum.Min(Interlocked.Increment(position), position - 1)) Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << 8
        End Function

        Private Function ReadInt32(ByVal _il As Byte(), ByRef position As Integer) As Integer
            Return il(stdNum.Min(Interlocked.Increment(position), position - 1)) Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << 8 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H10 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H18
        End Function

        Private Function ReadInt64(ByVal _il As Byte(), ByRef position As Integer) As ULong
            Return il(stdNum.Min(Interlocked.Increment(position), position - 1)) Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << 8 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H10 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H18 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H20 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H28 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H30 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H38
        End Function

        Private Function ReadDouble(ByVal _il As Byte(), ByRef position As Integer) As Double
            Return il(stdNum.Min(Interlocked.Increment(position), position - 1)) Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << 8 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H10 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H18 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H20 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H28 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H30 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H38
        End Function

        Private Function ReadSByte(ByVal _il As Byte(), ByRef position As Integer) As SByte
            Return il(stdNum.Min(Interlocked.Increment(position), position - 1))
        End Function

        Private Function ReadByte(ByVal _il As Byte(), ByRef position As Integer) As Byte
            Return il(stdNum.Min(Interlocked.Increment(position), position - 1))
        End Function

        Private Function ReadSingle(ByVal _il As Byte(), ByRef position As Integer) As Single
            Return il(stdNum.Min(Interlocked.Increment(position), position - 1)) Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << 8 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H10 Or il(stdNum.Min(Interlocked.Increment(position), position - 1)) << &H18
        End Function
#End Region

        ''' <summary>
        ''' Constructs the array of ILInstructions according to the IL byte code.
        ''' </summary>
        ''' <param name="module"></param>
        Private Sub ConstructInstructions(ByVal [module] As [Module])
            Dim il = Me.il
            Dim position = 0
            instructions = New List(Of ILInstruction)()

            While position < il.Length
                Dim instruction As ILInstruction = New ILInstruction()

                ' get the operation code of the current instruction
                Dim code = OpCodes.Nop
                Dim value As UShort = il(stdNum.Min(Interlocked.Increment(position), position - 1))

                If value <> &HFE Then
                    code = singleByteOpCodes(value)
                Else
                    value = il(stdNum.Min(Interlocked.Increment(position), position - 1))
                    code = multiByteOpCodes(value)
                    value = CUShort(value Or &HFE00)
                End If

                instruction.Code = code
                instruction.Offset = position - 1

                Dim metadataToken = 0

                ' get the operand of the current operation
                Select Case code.OperandType
                    Case OperandType.InlineBrTarget
                        metadataToken = ReadInt32(il, position)
                        metadataToken += position
                        instruction.Operand = metadataToken
                    Case OperandType.InlineField
                        metadataToken = ReadInt32(il, position)
                        instruction.Operand = [module].ResolveField(metadataToken)
                    Case OperandType.InlineMethod
                        metadataToken = ReadInt32(il, position)

                        Try
                            instruction.Operand = [module].ResolveMethod(metadataToken)
                        Catch
                            instruction.Operand = [module].ResolveMember(metadataToken)
                        End Try

                    Case OperandType.InlineSig
                        metadataToken = ReadInt32(il, position)
                        instruction.Operand = [module].ResolveSignature(metadataToken)
                    Case OperandType.InlineTok
                        metadataToken = ReadInt32(il, position)

                        Try
                            instruction.Operand = [module].ResolveType(metadataToken)
                        Catch
                            ' SSS : see what to do here
                        End Try

                    Case OperandType.InlineType
                        metadataToken = ReadInt32(il, position)
                        ' now we call the ResolveType always using the generic attributes type in order
                        ' to support decompilation of generic methods and classes

                        ' thanks to the guys from code project who commented on this missing feature

                        instruction.Operand = [module].ResolveType(metadataToken, mi.DeclaringType.GetGenericArguments(), mi.GetGenericArguments())
                    Case OperandType.InlineI
                        instruction.Operand = ReadInt32(il, position)
                        Exit Select
                    Case OperandType.InlineI8
                        instruction.Operand = ReadInt64(il, position)
                        Exit Select
                    Case OperandType.InlineNone
                        instruction.Operand = Nothing
                        Exit Select
                    Case OperandType.InlineR
                        instruction.Operand = ReadDouble(il, position)
                        Exit Select
                    Case OperandType.InlineString
                        metadataToken = ReadInt32(il, position)
                        instruction.Operand = [module].ResolveString(metadataToken)
                        Exit Select
                    Case OperandType.InlineSwitch
                        Dim count = ReadInt32(il, position)
                        Dim casesAddresses = New Integer(count - 1) {}

                        For i = 0 To count - 1
                            casesAddresses(i) = ReadInt32(il, position)
                        Next

                        Dim cases = New Integer(count - 1) {}

                        For i = 0 To count - 1
                            cases(i) = position + casesAddresses(i)
                        Next

                        Exit Select
                    Case OperandType.InlineVar
                        instruction.Operand = ReadUInt16(il, position)
                        Exit Select
                    Case OperandType.ShortInlineBrTarget
                        instruction.Operand = ReadSByte(il, position) + position
                        Exit Select
                    Case OperandType.ShortInlineI
                        instruction.Operand = ReadSByte(il, position)
                        Exit Select
                    Case OperandType.ShortInlineR
                        instruction.Operand = ReadSingle(il, position)
                        Exit Select
                    Case OperandType.ShortInlineVar
                        instruction.Operand = ReadByte(il, position)
                        Exit Select
                    Case Else
                        Throw New Exception("Unknown operand type.")
                End Select

                instructions.Add(instruction)
            End While
        End Sub

        Public Function GetRefferencedOperand(ByVal [module] As [Module], ByVal metadataToken As Integer) As Object
            Dim assemblyNames As AssemblyName() = [module].Assembly.GetReferencedAssemblies()

            For i = 0 To assemblyNames.Length - 1
                Dim modules As [Module]() = Assembly.Load(assemblyNames(i)).GetModules()

                For j = 0 To modules.Length - 1

                    Try
                        Dim t = modules(j).ResolveType(metadataToken)
                        Return t
                    Catch
                    End Try
                Next
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' Gets the IL code of the method
        ''' </summary>
        ''' <returns></returns>
        Public Function GetBodyCode() As String
            Dim result = ""

            If instructions IsNot Nothing Then
                For i = 0 To instructions.Count - 1
                    result += instructions(i).GetCode() & vbLf
                Next
            End If

            Return result
        End Function

        ''' <summary>
        ''' MethodBodyReader constructor
        ''' </summary>
        ''' <param name="mi">
        ''' The System.Reflection defined MethodInfo
        ''' </param>
        Public Sub New(ByVal mi As MethodInfo)
            Me.mi = mi

            If mi.GetMethodBody() IsNot Nothing Then
                il = mi.GetMethodBody().GetILAsByteArray()
                ConstructInstructions(mi.Module)
            End If
        End Sub
    End Class
End Namespace
