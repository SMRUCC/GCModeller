Imports System.IO
Imports System.Reflection
Imports System.Reflection.Emit
Imports System.Threading
Imports Microsoft.VisualBasic.Language
Imports stdNum = System.Math

Namespace IL

    ''' <summary>
    ''' Parsing the IL of a Method Body
    ''' 
    ''' > https://www.codeproject.com/articles/14058/parsing-the-il-of-a-method-body
    ''' </summary>
    Public Class MethodBodyReader
        Public instructions As List(Of ILInstruction) = Nothing
        Protected il As Stream
        Private mi As MethodInfo = Nothing

        '#Region "il read methods"
        '        Private Function ReadInt16(ByVal _il As Byte(), ByRef position As i32) As Integer
        '            Return ((il(++position) Or (il(++position) << 8)))
        '        End Function

        '        Private Function ReadUInt16(ByVal _il As Byte(), ByRef position As i32) As UShort
        '            Return ((il(++position) Or (il(++position) << 8)))
        '        End Function

        '        Private Function ReadInt32(ByVal _il As Byte(), ByRef position As i32) As Integer
        '            Return (((il(++position) Or (il(++position) << 8)) Or (il(++position) << &H10)) Or (il(++position) << &H18))
        '        End Function

        '        Private Function ReadInt64(ByVal _il As Byte(), ByRef position As i32) As ULong
        '            Return (((il(++position) Or (il(++position) << 8)) Or (il(++position) << &H10)) Or (il(++position) << &H18) Or (il(++position) << &H20) Or (il(++position) << &H28) Or (il(++position) << &H30) Or (il(++position) << &H38))
        '        End Function

        '        Private Function ReadDouble(ByVal _il As Byte(), ByRef position As i32) As Double
        '            Return (((il(++position) Or (il(++position) << 8)) Or (il(++position) << &H10)) Or (il(++position) << &H18) Or (il(++position) << &H20) Or (il(++position) << &H28) Or (il(++position) << &H30) Or (il(++position) << &H38))
        '        End Function

        '        Private Function ReadSByte(ByVal _il As Byte(), ByRef position As i32) As SByte
        '            Return il(++position)
        '        End Function

        '        Private Function ReadByte(ByVal _il As Byte(), ByRef position As i32) As Byte
        '            Return il(++position)
        '        End Function

        '        Private Function ReadSingle(ByVal _il As Byte(), ByRef position As i32) As Single
        '            Return (((il(++position) Or (il(++position) << 8)) Or (il(++position) << &H10)) Or (il(++position) << &H18))
        '        End Function
        '#End Region

        ''' <summary>
        ''' Constructs the array of ILInstructions according to the IL byte code.
        ''' </summary>
        ''' <param name="module"></param>
        Private Sub ConstructInstructions(ByVal [module] As [Module])
            ' Dim position As i32 = Scan0
            Dim il As New BinaryReader(Me.il)

            instructions = New List(Of ILInstruction)()

            While il.BaseStream.Position < il.BaseStream.Length   'position < il.Length
                Dim instruction As New ILInstruction()

                ' get the operation code of the current instruction
                Dim code = OpCodes.Nop
                Dim value As UShort = il.ReadByte '(++position)

                If value <> &HFE Then
                    code = singleByteOpCodes(value)
                Else
                    value = il.ReadByte  ' il(++position)
                    code = multiByteOpCodes(value)
                    value = CUShort(value Or &HFE00)
                End If

                instruction.Code = code
                instruction.Offset = il.BaseStream.Position - 1

                Dim metadataToken = 0

                ' get the operand of the current operation
                Select Case code.OperandType
                    Case OperandType.InlineBrTarget
                        metadataToken = il.ReadInt32 ' (il, position)
                        metadataToken += il.BaseStream.Position  ' position
                        instruction.Operand = metadataToken
                    Case OperandType.InlineField
                        metadataToken = il.ReadInt32 ' (il, position)
                        instruction.Operand = [module].ResolveField(metadataToken)
                    Case OperandType.InlineMethod
                        metadataToken = il.ReadInt32 ' (il, position)

                        Try
                            instruction.Operand = [module].ResolveMethod(metadataToken)
                        Catch
                            ' instruction.Operand = [module].ResolveMember(metadataToken)
                            Continue While
                        End Try

                    Case OperandType.InlineSig
                        metadataToken = il.ReadInt32 ' (il, position)
                        instruction.Operand = [module].ResolveSignature(metadataToken)
                    Case OperandType.InlineTok
                        metadataToken = il.ReadInt32 ' (il, position)

                        Try
                            instruction.Operand = [module].ResolveType(metadataToken)
                        Catch
                            ' SSS : see what to do here
                        End Try

                    Case OperandType.InlineType
                        metadataToken = il.ReadInt32 ' (il, position)
                        ' now we call the ResolveType always using the generic attributes type in order
                        ' to support decompilation of generic methods and classes

                        ' thanks to the guys from code project who commented on this missing feature

                        instruction.Operand = [module].ResolveType(metadataToken, mi.DeclaringType.GetGenericArguments(), mi.GetGenericArguments())
                    Case OperandType.InlineI
                        instruction.Operand = il.ReadInt32' (il, position)

                    Case OperandType.InlineI8
                        instruction.Operand = il.ReadInt64' (il, position)

                    Case OperandType.InlineNone
                        instruction.Operand = Nothing

                    Case OperandType.InlineR
                        instruction.Operand = il.ReadDouble' (il, position)

                    Case OperandType.InlineString
                        metadataToken = il.ReadInt32 '(il, position)
                        instruction.Operand = [module].ResolveString(metadataToken)

                    Case OperandType.InlineSwitch
                        Dim count = il.ReadInt32 ' (il, position)
                        Dim casesAddresses = New Integer(count - 1) {}

                        For i = 0 To count - 1
                            casesAddresses(i) = il.ReadInt32 ' (il, position)
                        Next

                        Dim cases = New Integer(count - 1) {}
                        Dim position_i As Integer = il.BaseStream.Position

                        For i = 0 To count - 1
                            cases(i) = position_i + casesAddresses(i)
                        Next


                    Case OperandType.InlineVar
                        instruction.Operand = il.ReadUInt16' (il, position)

                    Case OperandType.ShortInlineBrTarget
                        instruction.Operand = il.ReadSByte + il.BaseStream.Position  ' (il, position) + position

                    Case OperandType.ShortInlineI
                        instruction.Operand = il.ReadSByte'(il, position)

                    Case OperandType.ShortInlineR
                        instruction.Operand = il.ReadSingle' (il, position)

                    Case OperandType.ShortInlineVar
                        instruction.Operand = il.ReadByte '(il, position)

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
                il = New MemoryStream(mi.GetMethodBody().GetILAsByteArray())
                ConstructInstructions(mi.Module)
            End If
        End Sub
    End Class
End Namespace
