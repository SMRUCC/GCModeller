#Region "Microsoft.VisualBasic::9670230c950adea74b4a3ae068acdf49, RDotNET\RDotNET\SymbolicExpressionExtension.vb"

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

    ' Module SymbolicExpressionExtension
    ' 
    '     Function: AsCharacter, AsCharacterMatrix, AsComplex, AsComplexMatrix, AsDataFrame
    '               AsEnvironment, AsExpression, AsFactor, AsFunction, AsInteger
    '               AsIntegerMatrix, AsLanguage, AsList, AsLogical, AsLogicalMatrix
    '               AsNumeric, AsNumericMatrix, AsRaw, AsRawMatrix, AsS4
    '               AsSymbol, AsVector, IsDataFrame, IsEnvironment, IsExpression
    '               IsFactor, IsFunction, IsLanguage, IsList, IsMatrix
    '               IsS4, IsSymbol, IsVector
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals
Imports System.Security.Permissions

''' <summary>
''' Provides extension methods for <see cref="SymbolicExpression"/>.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
Public Module SymbolicExpressionExtension

    ''' <summary>
    ''' Gets whether the specified expression is list.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns><c>True</c> if the specified expression is list.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function IsList(expression As SymbolicExpression) As Boolean
        If expression Is Nothing Then
            Throw New ArgumentNullException()
        End If
        ' See issue 81. Rf_isList in the R API is NOT the correct thing to use (yes, hard to be more conter-intuitive)
        ' ?is.list ==> "is.list returns TRUE if and only if its argument is a list or a pairlist of length > 0"
        Return (expression.Type = SymbolicExpressionType.List OrElse (expression.Type = SymbolicExpressionType.Pairlist AndAlso expression.GetFunction(Of Rf_length)()(expression.DangerousGetHandle()) > 0))
    End Function

    ''' <summary>
    ''' A cache of the REngine - WARNING this assumes there can be only one per process, initialized once only.
    ''' </summary>
    Private engine As REngine = Nothing

    Private asListFunction As [Function] = Nothing

    ''' <summary>
    ''' Converts the specified expression to a GenericVector.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The GenericVector. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsList(expression As SymbolicExpression) As GenericVector
        If Not Object.ReferenceEquals(engine, expression.Engine) OrElse engine Is Nothing Then
            engine = expression.Engine
            asListFunction = Nothing
        End If
        If asListFunction Is Nothing Then
            asListFunction = engine.Evaluate("invisible(as.list)").AsFunction()
        End If
        Dim newExpression = asListFunction.Invoke(expression)
        Return New GenericVector(newExpression.Engine, newExpression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Gets whether the specified expression is data frame.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns><c>True</c> if the specified expression is data frame.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function IsDataFrame(expression As SymbolicExpression) As Boolean
        If expression Is Nothing Then
            Throw New ArgumentNullException()
        End If
        Return expression.GetFunction(Of Rf_isFrame)()(expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Converts the specified expression to a DataFrame.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The DataFrame. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsDataFrame(expression As SymbolicExpression) As DataFrame
        If Not expression.IsDataFrame() Then
            Return Nothing
        End If
        Return New DataFrame(expression.Engine, expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Gets whether the specified expression is an S4 object.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns><c>True</c> if the specified expression is an S4 object.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function IsS4(expression As SymbolicExpression) As Boolean
        If expression Is Nothing Then
            Throw New ArgumentNullException()
        End If
        Return expression.GetFunction(Of Rf_isS4)()(expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Coerce the specified expression to an S4 object.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The DataFrame. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsS4(expression As SymbolicExpression) As S4Object
        If Not expression.IsS4() Then
            Return Nothing
        End If
        Return New S4Object(expression.Engine, expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Gets whether the specified expression is vector.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns><c>True</c> if the specified expression is vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function IsVector(expression As SymbolicExpression) As Boolean
        If expression Is Nothing Then
            Throw New ArgumentNullException()
        End If
        Return expression.GetFunction(Of Rf_isVector)()(expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Converts the specified expression to a DynamicVector.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The DynamicVector. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsVector(expression As SymbolicExpression) As DynamicVector
        If expression Is Nothing Then
            Throw New ArgumentNullException()
        End If
        Dim coerced As IntPtr = expression.GetFunction(Of Rf_coerceVector)()(expression.DangerousGetHandle(), expression.Type)
        Return New DynamicVector(expression.Engine, coerced)
    End Function

    ''' <summary>
    ''' Converts the specified expression to a LogicalVector.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The LogicalVector. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsLogical(expression As SymbolicExpression) As LogicalVector
        If Not expression.IsVector() Then
            Return Nothing
        End If
        Dim coerced As IntPtr = expression.GetFunction(Of Rf_coerceVector)()(expression.DangerousGetHandle(), SymbolicExpressionType.LogicalVector)
        Return New LogicalVector(expression.Engine, coerced)
    End Function

    ''' <summary>
    ''' Converts the specified expression to an IntegerVector.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The LogicalVector. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsInteger(expression As SymbolicExpression) As IntegerVector
        If Not expression.IsVector() Then
            Return Nothing
        End If
        Dim coerced As IntPtr = expression.GetFunction(Of Rf_coerceVector)()(expression.DangerousGetHandle(), SymbolicExpressionType.IntegerVector)
        Return New IntegerVector(expression.Engine, coerced)
    End Function

    ''' <summary>
    ''' Converts the specified expression to a NumericVector.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The LogicalVector. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsNumeric(expression As SymbolicExpression) As NumericVector
        If Not expression.IsVector() Then
            Return Nothing
        End If
        Dim coerced As IntPtr = expression.GetFunction(Of Rf_coerceVector)()(expression.DangerousGetHandle(), SymbolicExpressionType.NumericVector)
        Return New NumericVector(expression.Engine, coerced)
    End Function

    ''' <summary>
    ''' Converts the specified expression to a CharacterVector.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The LogicalVector. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsCharacter(expression As SymbolicExpression) As CharacterVector
        If Not expression.IsVector() Then
            Return Nothing
        End If
        Dim coerced As IntPtr = IntPtr.Zero
        If expression.IsFactor() Then
            coerced = expression.GetFunction(Of Rf_asCharacterFactor)()(expression.DangerousGetHandle())
        Else
            coerced = expression.GetFunction(Of Rf_coerceVector)()(expression.DangerousGetHandle(), SymbolicExpressionType.CharacterVector)
        End If
        Return New CharacterVector(expression.Engine, coerced)
    End Function

    ''' <summary>
    ''' Converts the specified expression to a ComplexVector.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The LogicalVector. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsComplex(expression As SymbolicExpression) As ComplexVector
        If Not expression.IsVector() Then
            Return Nothing
        End If
        Dim coerced As IntPtr = expression.GetFunction(Of Rf_coerceVector)()(expression.DangerousGetHandle(), SymbolicExpressionType.ComplexVector)
        Return New ComplexVector(expression.Engine, coerced)
    End Function

    ''' <summary>
    ''' Converts the specified expression to a RawVector.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The LogicalVector. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsRaw(expression As SymbolicExpression) As RawVector
        If Not expression.IsVector() Then
            Return Nothing
        End If
        Dim coerced As IntPtr = expression.GetFunction(Of Rf_coerceVector)()(expression.DangerousGetHandle(), SymbolicExpressionType.RawVector)
        Return New RawVector(expression.Engine, coerced)
    End Function

    ''' <summary>
    ''' Gets whether the specified expression is matrix.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns><c>True</c> if the specified expression is matrix.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function IsMatrix(expression As SymbolicExpression) As Boolean
        If expression Is Nothing Then
            Throw New ArgumentNullException()
        End If
        Return expression.GetFunction(Of Rf_isMatrix)()(expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Converts the specified expression to a LogicalMatrix.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The LogicalMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsLogicalMatrix(expression As SymbolicExpression) As LogicalMatrix
        If Not expression.IsVector() Then
            Return Nothing
        End If

        Dim rowCount As Integer = 0
        Dim columnCount As Integer = 0

        If expression.IsMatrix() Then
            If expression.Type = SymbolicExpressionType.LogicalVector Then
                Return New LogicalMatrix(expression.Engine, expression.DangerousGetHandle())
            Else
                rowCount = expression.GetFunction(Of Rf_nrows)()(expression.DangerousGetHandle())
                columnCount = expression.GetFunction(Of Rf_ncols)()(expression.DangerousGetHandle())
            End If
        End If

        If columnCount = 0 Then
            rowCount = expression.GetFunction(Of Rf_length)()(expression.DangerousGetHandle())
            columnCount = 1
        End If

        Dim coerced As IntPtr = expression.GetFunction(Of Rf_coerceVector)()(expression.DangerousGetHandle(), SymbolicExpressionType.LogicalVector)
        Dim [dim] = New IntegerVector(expression.Engine, {rowCount, columnCount})
        Dim dimSymbol As SymbolicExpression = expression.Engine.GetPredefinedSymbol("R_DimSymbol")
        Dim matrix = New LogicalMatrix(expression.Engine, coerced)
        matrix.SetAttribute(dimSymbol, [dim])
        Return matrix
    End Function

    ''' <summary>
    ''' Converts the specified expression to an IntegerMatrix.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The IntegerMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsIntegerMatrix(expression As SymbolicExpression) As IntegerMatrix
        If Not expression.IsVector() Then
            Return Nothing
        End If

        Dim rowCount As Integer = 0
        Dim columnCount As Integer = 0

        If expression.IsMatrix() Then
            If expression.Type = SymbolicExpressionType.IntegerVector Then
                Return New IntegerMatrix(expression.Engine, expression.DangerousGetHandle())
            Else
                rowCount = expression.GetFunction(Of Rf_nrows)()(expression.DangerousGetHandle())
                columnCount = expression.GetFunction(Of Rf_ncols)()(expression.DangerousGetHandle())
            End If
        End If

        If columnCount = 0 Then
            rowCount = expression.GetFunction(Of Rf_length)()(expression.DangerousGetHandle())
            columnCount = 1
        End If

        Dim coerced As IntPtr = expression.GetFunction(Of Rf_coerceVector)()(expression.DangerousGetHandle(), SymbolicExpressionType.IntegerVector)
        Dim [dim] = New IntegerVector(expression.Engine, {rowCount, columnCount})
        Dim dimSymbol As SymbolicExpression = expression.Engine.GetPredefinedSymbol("R_DimSymbol")
        Dim matrix = New IntegerMatrix(expression.Engine, coerced)
        matrix.SetAttribute(dimSymbol, [dim])
        Return matrix
    End Function

    ''' <summary>
    ''' Converts the specified expression to a NumericMatrix.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The NumericMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsNumericMatrix(expression As SymbolicExpression) As NumericMatrix
        If Not expression.IsVector() Then
            Return Nothing
        End If

        Dim rowCount As Integer = 0
        Dim columnCount As Integer = 0

        If expression.IsMatrix() Then
            If expression.Type = SymbolicExpressionType.NumericVector Then
                Return New NumericMatrix(expression.Engine, expression.DangerousGetHandle())
            Else
                rowCount = expression.GetFunction(Of Rf_nrows)()(expression.DangerousGetHandle())
                columnCount = expression.GetFunction(Of Rf_ncols)()(expression.DangerousGetHandle())
            End If
        End If

        If columnCount = 0 Then
            rowCount = expression.GetFunction(Of Rf_length)()(expression.DangerousGetHandle())
            columnCount = 1
        End If

        Dim coerced As IntPtr = expression.GetFunction(Of Rf_coerceVector)()(expression.DangerousGetHandle(), SymbolicExpressionType.NumericVector)
        Dim [dim] = New IntegerVector(expression.Engine, {rowCount, columnCount})
        Dim dimSymbol As SymbolicExpression = expression.Engine.GetPredefinedSymbol("R_DimSymbol")
        Dim matrix = New NumericMatrix(expression.Engine, coerced)
        matrix.SetAttribute(dimSymbol, [dim])
        Return matrix
    End Function

    ''' <summary>
    ''' Converts the specified expression to a CharacterMatrix.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The CharacterMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsCharacterMatrix(expression As SymbolicExpression) As CharacterMatrix
        If Not expression.IsVector() Then
            Return Nothing
        End If

        Dim rowCount As Integer = 0
        Dim columnCount As Integer = 0

        If expression.IsMatrix() Then
            If expression.Type = SymbolicExpressionType.CharacterVector Then
                Return New CharacterMatrix(expression.Engine, expression.DangerousGetHandle())
            Else
                rowCount = expression.GetFunction(Of Rf_nrows)()(expression.DangerousGetHandle())
                columnCount = expression.GetFunction(Of Rf_ncols)()(expression.DangerousGetHandle())
            End If
        End If

        If columnCount = 0 Then
            rowCount = expression.GetFunction(Of Rf_length)()(expression.DangerousGetHandle())
            columnCount = 1
        End If

        Dim coerced As IntPtr = expression.GetFunction(Of Rf_coerceVector)()(expression.DangerousGetHandle(), SymbolicExpressionType.CharacterVector)
        Dim [dim] = New IntegerVector(expression.Engine, {rowCount, columnCount})
        Dim dimSymbol As SymbolicExpression = expression.Engine.GetPredefinedSymbol("R_DimSymbol")
        Dim matrix = New CharacterMatrix(expression.Engine, coerced)
        matrix.SetAttribute(dimSymbol, [dim])
        Return matrix
    End Function

    ''' <summary>
    ''' Converts the specified expression to a ComplexMatrix.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The ComplexMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsComplexMatrix(expression As SymbolicExpression) As ComplexMatrix
        If Not expression.IsVector() Then
            Return Nothing
        End If

        Dim rowCount As Integer = 0
        Dim columnCount As Integer = 0

        If expression.IsMatrix() Then
            If expression.Type = SymbolicExpressionType.ComplexVector Then
                Return New ComplexMatrix(expression.Engine, expression.DangerousGetHandle())
            Else
                rowCount = expression.GetFunction(Of Rf_nrows)()(expression.DangerousGetHandle())
                columnCount = expression.GetFunction(Of Rf_ncols)()(expression.DangerousGetHandle())
            End If
        End If

        If columnCount = 0 Then
            rowCount = expression.GetFunction(Of Rf_length)()(expression.DangerousGetHandle())
            columnCount = 1
        End If

        Dim coerced As IntPtr = expression.GetFunction(Of Rf_coerceVector)()(expression.DangerousGetHandle(), SymbolicExpressionType.ComplexVector)
        Dim [dim] = New IntegerVector(expression.Engine, {rowCount, columnCount})
        Dim dimSymbol As SymbolicExpression = expression.Engine.GetPredefinedSymbol("R_DimSymbol")
        Dim matrix = New ComplexMatrix(expression.Engine, coerced)
        matrix.SetAttribute(dimSymbol, [dim])
        Return matrix
    End Function

    ''' <summary>
    ''' Converts the specified expression to a RawMatrix.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The RawMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsRawMatrix(expression As SymbolicExpression) As RawMatrix
        If Not expression.IsVector() Then
            Return Nothing
        End If

        Dim rowCount As Integer = 0
        Dim columnCount As Integer = 0

        If expression.IsMatrix() Then
            If expression.Type = SymbolicExpressionType.RawVector Then
                Return New RawMatrix(expression.Engine, expression.DangerousGetHandle())
            Else
                rowCount = expression.GetFunction(Of Rf_nrows)()(expression.DangerousGetHandle())
                columnCount = expression.GetFunction(Of Rf_ncols)()(expression.DangerousGetHandle())
            End If
        End If

        If columnCount = 0 Then
            rowCount = expression.GetFunction(Of Rf_length)()(expression.DangerousGetHandle())
            columnCount = 1
        End If

        Dim coerced As IntPtr = expression.GetFunction(Of Rf_coerceVector)()(expression.DangerousGetHandle(), SymbolicExpressionType.RawVector)
        Dim [dim] = New IntegerVector(expression.Engine, {rowCount, columnCount})
        Dim dimSymbol As SymbolicExpression = expression.Engine.GetPredefinedSymbol("R_DimSymbol")
        Dim matrix = New RawMatrix(expression.Engine, coerced)
        matrix.SetAttribute(dimSymbol, [dim])
        Return matrix
    End Function

    ''' <summary>
    ''' Specifies the expression is an <see cref="REnvironment"/> object or not.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns><c>True</c> if it is an environment.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function IsEnvironment(expression As SymbolicExpression) As Boolean
        If expression Is Nothing Then
            Throw New ArgumentNullException()
        End If
        Return expression.GetFunction(Of Rf_isEnvironment)()(expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Gets the expression as an <see cref="REnvironment"/>.
    ''' </summary>
    ''' <param name="expression">The environment.</param>
    ''' <returns>The environment.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsEnvironment(expression As SymbolicExpression) As REnvironment
        If Not expression.IsEnvironment() Then
            Return Nothing
        End If
        Return New REnvironment(expression.Engine, expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Specifies the expression is an <see cref="RDotNet.Expression"/> object or not.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns><c>True</c> if it is an expression.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function IsExpression(expression As SymbolicExpression) As Boolean
        If expression Is Nothing Then
            Throw New ArgumentNullException()
        End If
        Return expression.GetFunction(Of Rf_isExpression)()(expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Gets the expression as an <see cref="RDotNet.Expression"/>.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The expression.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsExpression(expression As SymbolicExpression) As Expression
        If Not expression.IsExpression() Then
            Return Nothing
        End If
        Return New Expression(expression.Engine, expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Specifies the expression is a symbol object or not.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns><c>True</c> if it is a symbol.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function IsSymbol(expression As SymbolicExpression) As Boolean
        If expression Is Nothing Then
            Throw New ArgumentNullException()
        End If
        Return expression.GetFunction(Of Rf_isSymbol)()(expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Gets the expression as a symbol.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The symbol.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsSymbol(expression As SymbolicExpression) As Symbol
        If Not expression.IsSymbol() Then
            Return Nothing
        End If
        Return New Symbol(expression.Engine, expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Specifies the expression is a language object or not.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns><c>True</c> if it is a language.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function IsLanguage(expression As SymbolicExpression) As Boolean
        If expression Is Nothing Then
            Throw New ArgumentNullException()
        End If
        Return expression.GetFunction(Of Rf_isLanguage)()(expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Gets the expression as a language.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The language.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsLanguage(expression As SymbolicExpression) As Language
        If Not expression.IsLanguage() Then
            Return Nothing
        End If
        Return New Language(expression.Engine, expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Specifies the expression is a function object or not.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns><c>True</c> if it is a function.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function IsFunction(expression As SymbolicExpression) As Boolean
        If expression Is Nothing Then
            Throw New ArgumentNullException()
        End If
        Return expression.GetFunction(Of Rf_isFunction)()(expression.DangerousGetHandle())
    End Function

    ''' <summary>
    ''' Gets the expression as a function.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The function.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsFunction(expression As SymbolicExpression) As [Function]
        Select Case expression.Type
            Case SymbolicExpressionType.Closure
                Return New Closure(expression.Engine, expression.DangerousGetHandle())

            Case SymbolicExpressionType.BuiltinFunction
                Return New BuiltinFunction(expression.Engine, expression.DangerousGetHandle())

            Case SymbolicExpressionType.SpecialFunction
                Return New SpecialFunction(expression.Engine, expression.DangerousGetHandle())
            Case Else

                Throw New ArgumentException()
        End Select
    End Function

    ''' <summary>
    ''' Gets whether the specified expression is factor.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns><c>True</c> if the specified expression is factor.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function IsFactor(expression As SymbolicExpression) As Boolean
        If expression Is Nothing Then
            Throw New ArgumentNullException()
        End If
        Dim handle = expression.DangerousGetHandle()
        Return expression.GetFunction(Of Rf_isFactor)()(handle)
    End Function

    ''' <summary>
    ''' Gets the expression as a factor.
    ''' </summary>
    ''' <param name="expression">The expression.</param>
    ''' <returns>The factor.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function AsFactor(expression As SymbolicExpression) As Factor
        If Not IsFactor(expression) Then
            Throw New ArgumentException("Not a factor.", "expression")
        End If
        Return New Factor(expression.Engine, expression.DangerousGetHandle())
    End Function
End Module

