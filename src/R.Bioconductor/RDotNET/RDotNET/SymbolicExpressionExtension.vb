Imports RDotNet.Internals
Imports System
Imports System.Security.Permissions
Imports System.Runtime.CompilerServices


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
        <Extension()>
        Public Function IsList(ByVal expression As SymbolicExpression) As Boolean
            If expression Is Nothing Then
                Throw New ArgumentNullException()
            End If
            ' See issue 81. Rf_isList in the R API is NOT the correct thing to use (yes, hard to be more conter-intuitive)
            ' ?is.list ==> "is.list returns TRUE if and only if its argument is a list or a pairlist of length > 0"
            Return (expression.Type = SymbolicExpressionType.List OrElse (expression.Type = SymbolicExpressionType.Pairlist AndAlso expression.GetFunction(Of Rf_length)()(expression.DangerousGetHandle()) > 0))
        End Function

        ''' <summary>
        ''' Converts the specified expression to a GenericVector.
        ''' </summary>
        ''' <param name="expression">The expression.</param>
        ''' <returns>The GenericVector. Returns <c>null</c> if the specified expression is not vector.</returns>
        <Extension()>
        Public Function AsList(ByVal expression As SymbolicExpression) As GenericVector
            Return asListMethod(expression)
        End Function

        ''' <summary>
        ''' A cache of the REngine - WARNING this assumes there can be only one per process, initialised once only.
        ''' </summary>
        Private engine As REngine = Nothing
        Private asListFunction As [Function] = Nothing

        Private Function asListMethod(ByVal expression As SymbolicExpression) As GenericVector
            If Not ReferenceEquals(engine, expression.Engine) OrElse engine Is Nothing Then
                engine = expression.Engine
                asListFunction = Nothing
            End If

            If asListFunction Is Nothing Then asListFunction = engine.Evaluate("invisible(as.list)").AsFunction()
            Dim newExpression = asListFunction.Invoke(expression)
            Return New GenericVector(newExpression.Engine, newExpression.DangerousGetHandle())
        End Function

        ''' <summary>
        ''' Gets whether the specified expression is data frame.
        ''' </summary>
        ''' <param name="expression">The expression.</param>
        ''' <returns><c>True</c> if the specified expression is data frame.</returns>
        <Extension()>
        Public Function IsDataFrame(ByVal expression As SymbolicExpression) As Boolean
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
        <Extension()>
        Public Function AsDataFrame(ByVal expression As SymbolicExpression) As DataFrame
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
        <Extension()>
        Public Function IsS4(ByVal expression As SymbolicExpression) As Boolean
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
        <Extension()>
        Public Function AsS4(ByVal expression As SymbolicExpression) As S4Object
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
        <Extension()>
        Public Function IsVector(ByVal expression As SymbolicExpression) As Boolean
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
        <Extension()>
        Public Function AsVector(ByVal expression As SymbolicExpression) As DynamicVector
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
        <Extension()>
        Public Function AsLogical(ByVal expression As SymbolicExpression) As LogicalVector
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
        <Extension()>
        Public Function AsInteger(ByVal expression As SymbolicExpression) As IntegerVector
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
        <Extension()>
        Public Function AsNumeric(ByVal expression As SymbolicExpression) As NumericVector
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
        <Extension()>
        Public Function AsCharacter(ByVal expression As SymbolicExpression) As CharacterVector
            If Not expression.IsVector() Then
                Return Nothing
            End If

            Dim coerced = IntPtr.Zero

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
        <Extension()>
        Public Function AsComplex(ByVal expression As SymbolicExpression) As ComplexVector
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
        <Extension()>
        Public Function AsRaw(ByVal expression As SymbolicExpression) As RawVector
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
        <Extension()>
        Public Function IsMatrix(ByVal expression As SymbolicExpression) As Boolean
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
        <Extension()>
        Public Function AsLogicalMatrix(ByVal expression As SymbolicExpression) As LogicalMatrix
            If Not expression.IsVector() Then
                Return Nothing
            End If

            Dim rowCount = 0
            Dim columnCount = 0

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
            Dim dimSymbol = expression.Engine.GetPredefinedSymbol("R_DimSymbol")
            Dim matrix = New LogicalMatrix(expression.Engine, coerced)
            matrix.SetAttribute(dimSymbol, [dim])
            Return matrix
        End Function

        ''' <summary>
        ''' Converts the specified expression to an IntegerMatrix.
        ''' </summary>
        ''' <param name="expression">The expression.</param>
        ''' <returns>The IntegerMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
        <Extension()>
        Public Function AsIntegerMatrix(ByVal expression As SymbolicExpression) As IntegerMatrix
            If Not expression.IsVector() Then
                Return Nothing
            End If

            Dim rowCount = 0
            Dim columnCount = 0

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
            Dim dimSymbol = expression.Engine.GetPredefinedSymbol("R_DimSymbol")
            Dim matrix = New IntegerMatrix(expression.Engine, coerced)
            matrix.SetAttribute(dimSymbol, [dim])
            Return matrix
        End Function

        ''' <summary>
        ''' Converts the specified expression to a NumericMatrix.
        ''' </summary>
        ''' <param name="expression">The expression.</param>
        ''' <returns>The NumericMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
        <Extension()>
        Public Function AsNumericMatrix(ByVal expression As SymbolicExpression) As NumericMatrix
            If Not expression.IsVector() Then
                Return Nothing
            End If

            Dim rowCount = 0
            Dim columnCount = 0

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
            Dim dimSymbol = expression.Engine.GetPredefinedSymbol("R_DimSymbol")
            Dim matrix = New NumericMatrix(expression.Engine, coerced)
            matrix.SetAttribute(dimSymbol, [dim])
            Return matrix
        End Function

        ''' <summary>
        ''' Converts the specified expression to a CharacterMatrix.
        ''' </summary>
        ''' <param name="expression">The expression.</param>
        ''' <returns>The CharacterMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
        <Extension()>
        Public Function AsCharacterMatrix(ByVal expression As SymbolicExpression) As CharacterMatrix
            If Not expression.IsVector() Then
                Return Nothing
            End If

            Dim rowCount = 0
            Dim columnCount = 0

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
            Dim dimSymbol = expression.Engine.GetPredefinedSymbol("R_DimSymbol")
            Dim matrix = New CharacterMatrix(expression.Engine, coerced)
            matrix.SetAttribute(dimSymbol, [dim])
            Return matrix
        End Function

        ''' <summary>
        ''' Converts the specified expression to a ComplexMatrix.
        ''' </summary>
        ''' <param name="expression">The expression.</param>
        ''' <returns>The ComplexMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
        <Extension()>
        Public Function AsComplexMatrix(ByVal expression As SymbolicExpression) As ComplexMatrix
            If Not expression.IsVector() Then
                Return Nothing
            End If

            Dim rowCount = 0
            Dim columnCount = 0

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
            Dim dimSymbol = expression.Engine.GetPredefinedSymbol("R_DimSymbol")
            Dim matrix = New ComplexMatrix(expression.Engine, coerced)
            matrix.SetAttribute(dimSymbol, [dim])
            Return matrix
        End Function

        ''' <summary>
        ''' Converts the specified expression to a RawMatrix.
        ''' </summary>
        ''' <param name="expression">The expression.</param>
        ''' <returns>The RawMatrix. Returns <c>null</c> if the specified expression is not vector.</returns>
        <Extension()>
        Public Function AsRawMatrix(ByVal expression As SymbolicExpression) As RawMatrix
            If Not expression.IsVector() Then
                Return Nothing
            End If

            Dim rowCount = 0
            Dim columnCount = 0

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
            Dim dimSymbol = expression.Engine.GetPredefinedSymbol("R_DimSymbol")
            Dim matrix = New RawMatrix(expression.Engine, coerced)
            matrix.SetAttribute(dimSymbol, [dim])
            Return matrix
        End Function

        ''' <summary>
        ''' Specifies the expression is an <see cref="REnvironment"/> object or not.
        ''' </summary>
        ''' <param name="expression">The expression.</param>
        ''' <returns><c>True</c> if it is an environment.</returns>
        <Extension()>
        Public Function IsEnvironment(ByVal expression As SymbolicExpression) As Boolean
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
        <Extension()>
        Public Function AsEnvironment(ByVal expression As SymbolicExpression) As REnvironment
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
        <Extension()>
        Public Function IsExpression(ByVal expression As SymbolicExpression) As Boolean
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
        <Extension()>
        Public Function AsExpression(ByVal expression As SymbolicExpression) As Expression
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
        <Extension()>
        Public Function IsSymbol(ByVal expression As SymbolicExpression) As Boolean
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
        <Extension()>
        Public Function AsSymbol(ByVal expression As SymbolicExpression) As Symbol
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
        <Extension()>
        Public Function IsLanguage(ByVal expression As SymbolicExpression) As Boolean
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
        <Extension()>
        Public Function AsLanguage(ByVal expression As SymbolicExpression) As Language
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
        <Extension()>
        Public Function IsFunction(ByVal expression As SymbolicExpression) As Boolean
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
        <Extension()>
        Public Function AsFunction(ByVal expression As SymbolicExpression) As [Function]
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
        <Extension()>
        Public Function IsFactor(ByVal expression As SymbolicExpression) As Boolean
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
        <Extension()>
        Public Function AsFactor(ByVal expression As SymbolicExpression) As Factor
            If Not expression.IsFactor() Then
                Throw New ArgumentException("Not a factor.", "expression")
            End If

            Return New Factor(expression.Engine, expression.DangerousGetHandle())
        End Function
    End Module

