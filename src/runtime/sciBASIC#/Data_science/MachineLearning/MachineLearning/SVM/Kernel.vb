Imports System

Namespace SVM
    Friend Interface IQMatrix
        Function GetQ(ByVal column As Integer, ByVal len As Integer) As Single()
        Function GetQD() As Double()
        Sub SwapIndex(ByVal i As Integer, ByVal j As Integer)
    End Interface

    Friend MustInherit Class Kernel
        Implements IQMatrix

        Private _x As Node()()
        Private _xSquare As Double()
        Private _kernelType As KernelType
        Private _degree As Integer
        Private _gamma As Double
        Private _coef0 As Double
        Public MustOverride Function GetQ(ByVal column As Integer, ByVal len As Integer) As Single() Implements IQMatrix.GetQ
        Public MustOverride Function GetQD() As Double() Implements IQMatrix.GetQD

        Public Overridable Sub SwapIndex(ByVal i As Integer, ByVal j As Integer) Implements IQMatrix.SwapIndex
            _x.SwapIndex(i, j)

            If _xSquare IsNot Nothing Then
                _xSquare.SwapIndex(i, j)
            End If
        End Sub

        Private Shared Function powi(ByVal value As Double, ByVal times As Integer) As Double
            Dim tmp = value, ret = 1.0
            Dim t = times

            While t > 0
                If t Mod 2 = 1 Then ret *= tmp
                tmp = tmp * tmp
                t = CInt(t / 2)
            End While

            Return ret
        End Function

        Public Function KernelFunction(ByVal i As Integer, ByVal j As Integer) As Double
            Select Case _kernelType
                Case KernelType.LINEAR
                    Return dot(_x(i), _x(j))
                Case KernelType.POLY
                    Return powi(_gamma * dot(_x(i), _x(j)) + _coef0, _degree)
                Case KernelType.RBF
                    Return Math.Exp(-_gamma * (_xSquare(i) + _xSquare(j) - 2 * dot(_x(i), _x(j))))
                Case KernelType.SIGMOID
                    Return Math.Tanh(_gamma * dot(_x(i), _x(j)) + _coef0)
                Case KernelType.PRECOMPUTED
                    Return _x(i)(CInt(_x(j)(0).Value)).Value
                Case Else
                    Return 0
            End Select
        End Function

        Public Sub New(ByVal l As Integer, ByVal x_ As Node()(), ByVal param As Parameter)
            _kernelType = param.KernelType
            _degree = param.Degree
            _gamma = param.Gamma
            _coef0 = param.Coefficient0
            _x = CType(x_.Clone(), Node()())

            If _kernelType = KernelType.RBF Then
                _xSquare = New Double(l - 1) {}

                For i = 0 To l - 1
                    _xSquare(i) = dot(_x(i), _x(i))
                Next
            Else
                _xSquare = Nothing
            End If
        End Sub

        Private Shared Function dot(ByVal xNodes As Node(), ByVal yNodes As Node()) As Double
            Dim sum As Double = 0
            Dim xlen = xNodes.Length
            Dim ylen = yNodes.Length
            Dim i = 0
            Dim j = 0
            Dim x = xNodes(0)
            Dim y = yNodes(0)

            While True

                If x._index = y._index Then
                    sum += x._value * y._value
                    i += 1
                    j += 1

                    If i < xlen AndAlso j < ylen Then
                        x = xNodes(i)
                        y = yNodes(j)
                    ElseIf i < xlen Then
                        x = xNodes(i)
                        Exit While
                    ElseIf j < ylen Then
                        y = yNodes(j)
                        Exit While
                    Else
                        Exit While
                    End If
                Else

                    If x._index > y._index Then
                        Threading.Interlocked.Increment(j)

                        If j < ylen Then
                            y = yNodes(j)
                        Else
                            Exit While
                        End If
                    Else
                        Threading.Interlocked.Increment(i)

                        If i < xlen Then
                            x = xNodes(i)
                        Else
                            Exit While
                        End If
                    End If
                End If
            End While

            Return sum
        End Function

        Private Shared Function computeSquaredDistance(ByVal xNodes As Node(), ByVal yNodes As Node()) As Double
            Dim x = xNodes(0)
            Dim y = yNodes(0)
            Dim xLength = xNodes.Length
            Dim yLength = yNodes.Length
            Dim xIndex = 0
            Dim yIndex = 0
            Dim sum As Double = 0

            While True

                If x._index = y._index Then
                    Dim d = x._value - y._value
                    sum += d * d
                    xIndex += 1
                    yIndex += 1

                    If xIndex < xLength AndAlso yIndex < yLength Then
                        x = xNodes(xIndex)
                        y = yNodes(yIndex)
                    ElseIf xIndex < xLength Then
                        x = xNodes(xIndex)
                        Exit While
                    ElseIf yIndex < yLength Then
                        y = yNodes(yIndex)
                        Exit While
                    Else
                        Exit While
                    End If
                ElseIf x._index > y._index Then
                    sum += y._value * y._value

                    If Threading.Interlocked.Increment(yIndex) < yLength Then
                        y = yNodes(yIndex)
                    Else
                        Exit While
                    End If
                Else
                    sum += x._value * x._value

                    If Threading.Interlocked.Increment(xIndex) < xLength Then
                        x = xNodes(xIndex)
                    Else
                        Exit While
                    End If
                End If
            End While

            While xIndex < xLength
                Dim d = xNodes(xIndex)._value
                sum += d * d
                xIndex += 1
            End While

            While yIndex < yLength
                Dim d = yNodes(yIndex)._value
                sum += d * d
                yIndex += 1
            End While

            Return sum
        End Function

        Public Shared Function KernelFunction(ByVal x As Node(), ByVal y As Node(), ByVal param As Parameter) As Double
            Select Case param.KernelType
                Case KernelType.LINEAR
                    Return dot(x, y)
                Case KernelType.POLY
                    Return powi(param.Degree * dot(x, y) + param.Coefficient0, param.Degree)
                Case KernelType.RBF
                    Dim sum = computeSquaredDistance(x, y)
                    Return Math.Exp(-param.Gamma * sum)
                Case KernelType.SIGMOID
                    Return Math.Tanh(param.Gamma * dot(x, y) + param.Coefficient0)
                Case KernelType.PRECOMPUTED
                    Return x(CInt(y(0).Value)).Value
                Case Else
                    Return 0
            End Select
        End Function
    End Class
End Namespace
