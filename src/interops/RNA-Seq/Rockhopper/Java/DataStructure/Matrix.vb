#Region "Microsoft.VisualBasic::ab0743766ba16ab40635737bc47be155, RNA-Seq\Rockhopper\Java\DataStructure\Matrix.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class Matrix
    ' 
    '         Properties: array, arrayCopy, cols, rows
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: (+2 Overloads) getMatrix, hypot, solve
    ' 
    '     Class LUDecomposition
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: solve
    ' 
    '     Class QRDecomposition
    ' 
    '         Properties: fullRank
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: solve
    ' 
    ' 
    ' /********************************************************************************/

#End Region

'
' * Copyright 2013 Brian Tjaden
' *
' * This file is part of Rockhopper.
' *
' * Rockhopper is free software: you can redistribute it and/or modify
' * it under the terms of the GNU General Public License as published by
' * the Free Software Foundation, either version 3 of the License, or
' * any later version.
' *
' * Rockhopper is distributed in the hope that it will be useful,
' * but WITHOUT ANY WARRANTY; without even the implied warranty of
' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' * GNU General Public License for more details.
' *
' * You should have received a copy of the GNU General Public License
' * (in the file gpl.txt) along with Rockhopper.  
' * If not, see <http://www.gnu.org/licenses/>.
' 
Namespace Java

    Public Class Matrix

        ''' <summary>
        '''******************************************
        ''' **********   INSTANCE VARIABLES   **********
        ''' </summary>

        Private M As Double()()
        Private _rows As Integer, _cols As Integer
        ' Number or rows and columns


        ''' <summary>
        '''************************************
        ''' **********   CONSTRUCTORS   **********
        ''' </summary>

        Public Sub New(rows As Integer, cols As Integer)
            'ORIGINAL LINE: M = new double[rows][cols];
            'JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            M = ReturnRectangularDoubleArray(rows, cols)
            Me._rows = rows
            Me._cols = cols
        End Sub

        Public Sub New(M As Double()())
            Me.M = M
            _rows = M.Length
            _cols = M(0).Length
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   PUBLIC INSTANCE METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Returns the number of rows in this Matrix.
        ''' </summary>
        Public Overridable ReadOnly Property rows() As Integer
            Get
                Return _rows
            End Get
        End Property

        ''' <summary>
        ''' Returns the number of columns in this Matrix.
        ''' </summary>
        Public Overridable ReadOnly Property cols() As Integer
            Get
                Return _cols
            End Get
        End Property

        ''' <summary>
        ''' Returns the 2D array representing this Matrix.
        ''' </summary>
        Public Overridable ReadOnly Property array() As Double()()
            Get
                Return M
            End Get
        End Property

        ''' <summary>
        ''' Returns a copy of the 2D array representing this Matrix.
        ''' </summary>

        Public Overridable ReadOnly Property arrayCopy() As Double()()
            Get
                'ORIGINAL LINE: double[][] M2 = new double[rows_Renamed][cols_Renamed];
                'JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
                Dim M2 As Double()() = ReturnRectangularDoubleArray(_rows, _cols)
                For i As Integer = 0 To _rows - 1
                    For j As Integer = 0 To _cols - 1
                        M2(i)(j) = M(i)(j)
                    Next
                Next
                Return M2
            End Get
        End Property

        ''' <summary>
        ''' Return a submatrix of this Matrix.
        ''' </summary>
        Public Overridable Function getMatrix(r As Integer(), x As Integer, y As Integer) As Matrix
            Dim mtrx As New Matrix(r.Length, y - x + 1)
            Dim M2 As Double()() = mtrx.array
            For i As Integer = 0 To r.Length - 1
                For j As Integer = x To y
                    M2(i)(j - x) = M(r(i))(j)
                Next
            Next
            Return mtrx
        End Function

        ''' <summary>
        ''' Return a submatrix of this Matrix.
        ''' </summary>
        Public Overridable Function getMatrix(x1 As Integer, x2 As Integer, y1 As Integer, y2 As Integer) As Matrix
            Dim mtrx As New Matrix(x2 - x1 + 1, y2 - y1 + 1)
            Dim M2 As Double()() = mtrx.array
            For i As Integer = x1 To x2
                For j As Integer = y1 To y2
                    M2(i - x1)(j - y1) = M(i)(j)
                Next
            Next
            Return mtrx
        End Function

        Public Overridable Function solve(B As Matrix) As Matrix
            If _rows = _cols Then
                Return (New LUDecomposition(Me)).solve(B)
            Else
                Return (New QRDecomposition(Me)).solve(B)
            End If
        End Function



        ''' <summary>
        '''*************************************
        ''' **********   CLASS METHODS   **********
        ''' </summary>

        Public Shared Function hypot(x As Double, y As Double) As Double
            Dim r As Double
            If Math.Abs(x) > Math.Abs(y) Then
                r = y / x
                r = Math.Abs(x) * Math.Sqrt(1 + r * r)
            ElseIf y <> 0 Then
                r = x / y
                r = Math.Abs(y) * Math.Sqrt(1 + r * r)
            Else
                r = 0.0
            End If
            Return r
        End Function

    End Class



    ''' <summary>
    '''***************************************
    ''' **********   LU MATRIX CLASS   **********
    ''' </summary>

    Friend Class LUDecomposition

        ''' <summary>
        '''******************************************
        ''' **********   INSTANCE VARIABLES   **********
        ''' </summary>

        Private LU As Double()()
        Private rows As Integer
        Private cols As Integer
        Private signOfPivot As Integer
        Private pivot As Integer()



        ''' <summary>
        '''************************************
        ''' **********   CONSTRUCTORS   **********
        ''' </summary>

        Public Sub New(A As Matrix)

            LU = A.arrayCopy
            rows = A.rows
            cols = A.cols
            pivot = New Integer(rows - 1) {}
            For i As Integer = 0 To rows - 1
                pivot(i) = i
            Next
            signOfPivot = 1
            Dim LUrowi As Double()
            Dim LUcolj As Double() = New Double(rows - 1) {}

            For j As Integer = 0 To cols - 1
                For i As Integer = 0 To rows - 1
                    LUcolj(i) = LU(i)(j)
                Next
                For i As Integer = 0 To rows - 1
                    LUrowi = LU(i)
                    Dim kmax As Integer = Math.Min(i, j)
                    Dim s As Double = 0.0
                    For k As Integer = 0 To kmax - 1
                        s += LUrowi(k) * LUcolj(k)
                    Next

                    LUcolj(i) -= s
                    LUrowi(j) = LUcolj(i)
                Next
                Dim p As Integer = j
                For i As Integer = j + 1 To rows - 1
                    If Math.Abs(LUcolj(i)) > Math.Abs(LUcolj(p)) Then
                        p = i
                    End If
                Next
                If p <> j Then
                    Dim k As Integer = 0

                    For k = 0 To cols - 1
                        Dim t As Double = LU(p)(k)
                        LU(p)(k) = LU(j)(k)
                        LU(j)(k) = t
                    Next
                    k = pivot(p)
                    pivot(p) = pivot(j)
                    pivot(j) = k
                    signOfPivot = -signOfPivot
                End If
                If j < rows And LU(j)(j) <> 0.0 Then
                    For i As Integer = j + 1 To rows - 1
                        LU(i)(j) /= LU(j)(j)
                    Next
                End If
            Next
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   PUBLIC INSTANCE METHODS   **********
        ''' </summary>

        Public Overridable Function solve(B As Matrix) As Matrix

            Dim cols2 As Integer = B.cols
            Dim mtrx As Matrix = B.getMatrix(pivot, 0, cols2 - 1)
            Dim M2 As Double()() = mtrx.array

            For k As Integer = 0 To cols - 1
                For i As Integer = k + 1 To cols - 1
                    For j As Integer = 0 To cols2 - 1
                        M2(i)(j) -= M2(k)(j) * LU(i)(k)
                    Next
                Next
            Next

            For k As Integer = cols - 1 To 0 Step -1
                For j As Integer = 0 To cols2 - 1
                    M2(k)(j) /= LU(k)(k)
                Next
                For i As Integer = 0 To k - 1
                    For j As Integer = 0 To cols2 - 1
                        M2(i)(j) -= M2(k)(j) * LU(i)(k)
                    Next
                Next
            Next
            Return mtrx
        End Function

    End Class



    ''' <summary>
    '''***************************************
    ''' **********   QR MATRIX CLASS   **********
    ''' </summary>

    Friend Class QRDecomposition

        ''' <summary>
        '''******************************************
        ''' **********   INSTANCE VARIABLES   **********
        ''' </summary>

        Private QR As Double()()
        Private rows As Integer
        Private cols As Integer
        Private Rdiag As Double()



        ''' <summary>
        '''************************************
        ''' **********   CONSTRUCTORS   **********
        ''' </summary>

        Public Sub New(A As Matrix)
            QR = A.arrayCopy
            rows = A.rows
            cols = A.cols
            Rdiag = New Double(cols - 1) {}
            For k As Integer = 0 To cols - 1
                Dim nrm As Double = 0
                For i As Integer = k To rows - 1
                    nrm = Matrix.hypot(nrm, QR(i)(k))
                Next
                If nrm <> 0.0 Then
                    If QR(k)(k) < 0 Then
                        nrm = -nrm
                    End If
                    For i As Integer = k To rows - 1
                        QR(i)(k) /= nrm
                    Next
                    QR(k)(k) += 1.0
                    For j As Integer = k + 1 To cols - 1
                        Dim s As Double = 0.0
                        For i As Integer = k To rows - 1
                            s += QR(i)(k) * QR(i)(j)
                        Next
                        s = -s / QR(k)(k)
                        For i As Integer = k To rows - 1
                            QR(i)(j) += s * QR(i)(k)
                        Next
                    Next
                End If
                Rdiag(k) = -nrm
            Next
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   PUBLIC INSTANCE METHODS   **********
        ''' </summary>

        Public Overridable ReadOnly Property fullRank() As Boolean
            Get
                For j As Integer = 0 To cols - 1
                    If Rdiag(j) = 0 Then
                        Return False
                    End If
                Next
                Return True
            End Get
        End Property

        Public Overridable Function solve(B As Matrix) As Matrix
            Dim cols2 As Integer = B.cols
            Dim M2 As Double()() = B.arrayCopy
            For k As Integer = 0 To cols - 1
                For j As Integer = 0 To cols2 - 1
                    Dim s As Double = 0.0
                    For i As Integer = k To rows - 1
                        s += QR(i)(k) * M2(i)(j)
                    Next
                    s = -s / QR(k)(k)
                    For i As Integer = k To rows - 1
                        M2(i)(j) += s * QR(i)(k)
                    Next
                Next
            Next
            For k As Integer = cols - 1 To 0 Step -1
                For j As Integer = 0 To cols2 - 1
                    M2(k)(j) /= Rdiag(k)
                Next
                For i As Integer = 0 To k - 1
                    For j As Integer = 0 To cols2 - 1
                        M2(i)(j) -= M2(k)(j) * QR(i)(k)
                    Next
                Next
            Next
            Return ((New Matrix(M2)).getMatrix(0, cols - 1, 0, cols2 - 1))
        End Function
    End Class

End Namespace
