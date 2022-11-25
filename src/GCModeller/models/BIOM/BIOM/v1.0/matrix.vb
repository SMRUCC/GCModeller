#Region "Microsoft.VisualBasic::52732d205295db1a00fc9a1baf5f23db, GCModeller\models\BIOM\BIOM\v1.0\matrix.vb"

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


    ' Code Statistics:

    '   Total Lines: 127
    '    Code Lines: 71
    ' Comment Lines: 39
    '   Blank Lines: 17
    '     File Size: 4.88 KB


    '     Class IntegerMatrix
    ' 
    '         Function: LoadFile
    ' 
    '     Class FloatMatrix
    ' 
    '         Function: LoadFile
    ' 
    '     Class StringMatrix
    ' 
    '         Function: LoadFile
    ' 
    '     Module MatrixConversion
    ' 
    '         Function: RequiredConvertToDenseMatrix, ToDenseMatrix, ToSparseMatrix
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.MIME.application
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript

Namespace v10

    ' 20190528 The json deserializer of json contract module 
    ' have bugs On parse Date time string using new json 
    ' deserilizer for void such problem

    ''' <summary>
    ''' BIOM json with integer matrix data
    ''' </summary>
    Public Class IntegerMatrix : Inherits BIOMDataSet(Of Integer)

        Public Overloads Shared Function LoadFile(path$) As IntegerMatrix
            Dim jsonText$ = path.ReadAllText
            Dim jsonObj As JsonElement = json.ParseJson(jsonText)
            Dim biom As IntegerMatrix = jsonObj.CreateObject(GetType(IntegerMatrix), decodeMetachar:=True)
            Return biom
        End Function
    End Class

    ''' <summary>
    ''' BIOM json with double matrix data
    ''' </summary>
    Public Class FloatMatrix : Inherits BIOMDataSet(Of Double)

        Public Overloads Shared Function LoadFile(path$) As FloatMatrix
            Dim jsonText$ = path.ReadAllText
            Dim jsonObj As JsonElement = json.ParseJson(jsonText)
            Dim biom As FloatMatrix = jsonObj.CreateObject(GetType(FloatMatrix), decodeMetachar:=True)
            Return biom
        End Function
    End Class

    ''' <summary>
    ''' BIOM json with string matrix data
    ''' </summary>
    Public Class StringMatrix : Inherits BIOMDataSet(Of String)

        Public Overloads Shared Function LoadFile(path$) As StringMatrix
            Dim jsonText$ = path.ReadAllText
            Dim jsonObj As JsonElement = json.ParseJson(jsonText)
            Dim biom As StringMatrix = jsonObj.CreateObject(GetType(StringMatrix), decodeMetachar:=True)
            Return biom
        End Function
    End Class

    ''' <summary>
    ''' 在<see cref="matrix_type.sparse"/>和<see cref="matrix_type.dense"/>
    ''' 这两种布局的矩阵之间相互转换
    ''' </summary>
    Public Module MatrixConversion

        ''' <summary>
        ''' 目标是否是稀疏矩阵，需要转换为一个完整的矩阵对象
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="table"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function RequiredConvertToDenseMatrix(Of T As {IComparable(Of T), IEquatable(Of T), IComparable})(table As BIOMDataSet(Of T)) As Boolean
            Return Strings.LCase(table.matrix_type) = matrix_type.sparse
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="table"></param>
        ''' <param name="shape">``[rows, columns]``</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' if matrix_type is "sparse", 
        ''' 
        ''' ```
        ''' [[row, column, value],
        '''  [row, column, value],
        '''  ...
        ''' ]
        ''' ```
        ''' </remarks>
        <Extension>
        Public Function ToDenseMatrix(Of T)(table As T()(), shape As Integer()) As T()()
            Dim rows = shape(Scan0)
            Dim columns = shape(1)
            Dim matrix As T()() = MAT(Of T)(rows, columns)
            Dim i, j As Integer

            For Each row As T() In table
                i = CInt(CObj(row(Scan0)))
                j = CInt(CObj(row(1)))

                matrix(i)(j) = row(2)
            Next

            Return matrix
        End Function

        <Extension>
        Public Function ToSparseMatrix(Of T)(table As T()(), isEmpty As Func(Of T, Boolean)) As T()()
            Dim columns As Integer = table(Scan0).Length
            Dim obj As T
            Dim sparser = Iterator Function() As IEnumerable(Of T())
                              For i As Integer = 0 To table.Length - 1
                                  For j As Integer = 0 To columns - 1
                                      obj = table(i)(j)

                                      If Not isEmpty(obj) Then
                                          Yield {
                                              CType(CObj(i), T),  ' row
                                              CType(CObj(j), T),  ' column
                                              obj                 ' value
                                          }
                                      End If
                                  Next
                              Next
                          End Function

            Return sparser().ToArray
        End Function
    End Module
End Namespace
