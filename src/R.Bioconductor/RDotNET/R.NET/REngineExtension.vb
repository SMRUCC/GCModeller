Imports RDotNet.Internals
Imports System.Collections
Imports System.Collections.Generic
Imports System.Numerics

Imports RDotNET.SymbolicExpressionExtension

''' <summary>
''' Provides extension methods for <see cref="REngine"/>.
''' </summary>
Public Module REngineExtension

    <System.Runtime.CompilerServices.Extension> _
    Public Sub Install(engine As REngine, device As Graphics.IGraphicsDevice)
        Dim adapter = New Graphics.GraphicsDeviceAdapter(device)
        AddHandler engine.Disposing, Sub(sender, e) Call adapter.Dispose()
        adapter.SetEngine(engine)
    End Sub

    ''' <summary>
    ''' Creates a new empty CharacterVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="length">The length.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateCharacterVector(engine As REngine, length As Integer) As CharacterVector
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New CharacterVector(engine, length)
    End Function

    ''' <summary>
    ''' Creates a new empty ComplexVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="length">The length.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateComplexVector(engine As REngine, length As Integer) As ComplexVector
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New ComplexVector(engine, length)
    End Function

    ''' <summary>
    ''' Creates a new empty IntegerVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="length">The length.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateIntegerVector(engine As REngine, length As Integer) As IntegerVector
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New IntegerVector(engine, length)
    End Function

    ''' <summary>
    ''' Creates a new empty LogicalVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="length">The length.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateLogicalVector(engine As REngine, length As Integer) As LogicalVector
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New LogicalVector(engine, length)
    End Function

    ''' <summary>
    ''' Creates a new empty NumericVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="length">The length.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateNumericVector(engine As REngine, length As Integer) As NumericVector
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New NumericVector(engine, length)
    End Function

    ''' <summary>
    ''' Creates a new empty RawVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="length">The length.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateRawVector(engine As REngine, length As Integer) As RawVector
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New RawVector(engine, length)
    End Function

    ''' <summary>
    ''' Creates a new CharacterVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="vector">The values.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateCharacterVector(engine As REngine, vector As IEnumerable(Of String)) As CharacterVector
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New CharacterVector(engine, vector)
    End Function

    ''' <summary>
    ''' Creates a new ComplexVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="vector">The values.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateComplexVector(engine As REngine, vector As IEnumerable(Of Complex)) As ComplexVector
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New ComplexVector(engine, vector)
    End Function

    ''' <summary>
    ''' Creates a new IntegerVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="vector">The values.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateIntegerVector(engine As REngine, vector As IEnumerable(Of Integer)) As IntegerVector
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New IntegerVector(engine, vector)
    End Function

    ''' <summary>
    ''' Creates a new LogicalVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="vector">The values.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateLogicalVector(engine As REngine, vector As IEnumerable(Of Boolean)) As LogicalVector
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New LogicalVector(engine, vector)
    End Function

    ''' <summary>
    ''' Creates a new NumericVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="vector">The values.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateNumericVector(engine As REngine, vector As IEnumerable(Of Double)) As NumericVector
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New NumericVector(engine, vector)
    End Function

    ''' <summary>
    ''' Creates a new RawVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="vector">The values.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateRawVector(engine As REngine, vector As IEnumerable(Of Byte)) As RawVector
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New RawVector(engine, vector)
    End Function

    ''' <summary>
    ''' Create a vector with a single value
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="value">The value</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateCharacter(engine As REngine, value As String) As CharacterVector
        Return CreateCharacterVector(engine, New String() {value})
    End Function

    ''' <summary>
    ''' Create a vector with a single value
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="value">The value</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateComplex(engine As REngine, value As Complex) As ComplexVector
        Return CreateComplexVector(engine, New System.Numerics.Complex() {value})
    End Function

    ''' <summary>
    ''' Create a vector with a single value
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="value">The value</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateLogical(engine As REngine, value As Boolean) As LogicalVector
        Return CreateLogicalVector(engine, New Boolean() {value})
    End Function

    ''' <summary>
    ''' Create a vector with a single value
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="value">The value</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateNumeric(engine As REngine, value As Double) As NumericVector
        Return CreateNumericVector(engine, New Double() {value})
    End Function

    ''' <summary>
    ''' Create an integer vector with a single value
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="value">The value</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateInteger(engine As REngine, value As Integer) As IntegerVector
        Return CreateIntegerVector(engine, New Integer() {value})
    End Function

    ''' <summary>
    ''' Create a vector with a single value
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="value">The value</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateRaw(engine As REngine, value As Byte) As RawVector
        Return CreateRawVector(engine, New Byte() {value})
    End Function

    ''' <summary>
    ''' Creates a new empty CharacterMatrix with the specified size.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateCharacterMatrix(engine As REngine, rowCount As Integer, columnCount As Integer) As CharacterMatrix
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New CharacterMatrix(engine, rowCount, columnCount)
    End Function

    ''' <summary>
    ''' Creates a new empty ComplexMatrix with the specified size.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateComplexMatrix(engine As REngine, rowCount As Integer, columnCount As Integer) As ComplexMatrix
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New ComplexMatrix(engine, rowCount, columnCount)
    End Function

    ''' <summary>
    ''' Creates a new empty IntegerMatrix with the specified size.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateIntegerMatrix(engine As REngine, rowCount As Integer, columnCount As Integer) As IntegerMatrix
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New IntegerMatrix(engine, rowCount, columnCount)
    End Function

    ''' <summary>
    ''' Creates a new empty LogicalMatrix with the specified size.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateLogicalMatrix(engine As REngine, rowCount As Integer, columnCount As Integer) As LogicalMatrix
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New LogicalMatrix(engine, rowCount, columnCount)
    End Function

    ''' <summary>
    ''' Creates a new empty NumericMatrix with the specified size.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateNumericMatrix(engine As REngine, rowCount As Integer, columnCount As Integer) As NumericMatrix
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New NumericMatrix(engine, rowCount, columnCount)
    End Function

    ''' <summary>
    ''' Creates a new empty RawMatrix with the specified size.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateRawMatrix(engine As REngine, rowCount As Integer, columnCount As Integer) As RawMatrix
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New RawMatrix(engine, rowCount, columnCount)
    End Function

    ''' <summary>
    ''' Creates a new CharacterMatrix with the specified values.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="matrix">The values.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateCharacterMatrix(engine As REngine, matrix As String(,)) As CharacterMatrix
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New CharacterMatrix(engine, matrix)
    End Function

    ''' <summary>
    ''' Creates a new ComplexMatrix with the specified values.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="matrix">The values.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateComplexMatrix(engine As REngine, matrix As Complex(,)) As ComplexMatrix
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New ComplexMatrix(engine, matrix)
    End Function

    ''' <summary>
    ''' Creates a new IntegerMatrix with the specified values.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="matrix">The values.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateIntegerMatrix(engine As REngine, matrix As Integer(,)) As IntegerMatrix
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New IntegerMatrix(engine, matrix)
    End Function

    ''' <summary>
    ''' Creates a new LogicalMatrix with the specified values.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="matrix">The values.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateLogicalMatrix(engine As REngine, matrix As Boolean(,)) As LogicalMatrix
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New LogicalMatrix(engine, matrix)
    End Function

    ''' <summary>
    ''' Creates a new NumericMatrix with the specified values.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="matrix">The values.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateNumericMatrix(engine As REngine, matrix As Double(,)) As NumericMatrix
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New NumericMatrix(engine, matrix)
    End Function

    ''' <summary>
    ''' Creates a new RawMatrix with the specified values.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="matrix">The values.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateRawMatrix(engine As REngine, matrix As Byte(,)) As RawMatrix
        If engine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New RawMatrix(engine, matrix)
    End Function

    ''' <summary>
    ''' Create an R data frame from managed arrays and objects.
    ''' </summary>
    ''' <param name="engine">R engine</param>
    ''' <param name="columns">The columns with the values for the data frame. These must be array of supported types (double, string, bool, integer, byte)</param>
    ''' <param name="columnNames">Column names. default: null.</param>
    ''' <param name="rowNames">Row names. Default null.</param>
    ''' <param name="checkRows">Check rows. See data.frame R documentation</param>
    ''' <param name="checkNames">See data.frame R documentation</param>
    ''' <param name="stringsAsFactors">Should columns of strings be considered as factors (categories). See data.frame R documentation</param>
    ''' <returns></returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateDataFrame(engine As REngine, columns As IEnumerable(), Optional columnNames As String() = Nothing, Optional rowNames As String() = Nothing, Optional checkRows As Boolean = False, Optional checkNames As Boolean = True, _
        Optional stringsAsFactors As Boolean = True) As DataFrame
        Dim df = engine.GetSymbol("data.frame").AsFunction()
        Dim colVectors As SymbolicExpression() = ToVectors(engine, columns)
        Dim namedColArgs As Tuple(Of String, SymbolicExpression)() = CreateNamedArgs(colVectors, columnNames)
        Dim args = New List(Of Tuple(Of String, SymbolicExpression))(namedColArgs)
        If rowNames IsNot Nothing Then
            args.Add(Tuple.Create("row.names", DirectCast(engine.CreateCharacterVector(rowNames), SymbolicExpression)))
        End If
        args.Add(Tuple.Create("check.rows", DirectCast(engine.CreateLogical(checkRows), SymbolicExpression)))
        args.Add(Tuple.Create("check.names", DirectCast(engine.CreateLogical(checkNames), SymbolicExpression)))
        args.Add(Tuple.Create("stringsAsFactors", DirectCast(engine.CreateLogical(stringsAsFactors), SymbolicExpression)))
        Dim result = df.InvokeNamed(args.ToArray()).AsDataFrame()
        Return result
    End Function

    Private Function CreateNamedArgs(colVectors As SymbolicExpression(), columnNames As String()) As Tuple(Of String, SymbolicExpression)()
        If columnNames IsNot Nothing AndAlso colVectors.Length <> columnNames.Length Then
            Throw New ArgumentException("columnNames", "when not null, the number of column names must match the number of SEXP")
        End If
        Dim args = New List(Of Tuple(Of String, SymbolicExpression))()
        For i As Integer = 0 To colVectors.Length - 1
            args.Add(Tuple.Create(If(columnNames IsNot Nothing, columnNames(i), ""), colVectors(i)))
        Next
        Return args.ToArray()
    End Function

    Friend Function ToVectors(engine As REngine, columns As IEnumerable()) As SymbolicExpression()
        Return Array.ConvertAll(columns, Function(x) ToVector(engine, x))
    End Function

    Friend Function ToVector(engine As REngine, values As IEnumerable) As SymbolicExpression
        If values Is Nothing Then
            Throw New ArgumentNullException("values", "values to transform to an R vector must not be null")
        End If
        Dim ints As IEnumerable(Of Integer) = TryCast(values, IEnumerable(Of Integer))
        Dim chars As IEnumerable(Of String) = TryCast(values, IEnumerable(Of String))
        Dim cplxs As IEnumerable(Of Complex) = TryCast(values, IEnumerable(Of Complex))
        Dim logicals As IEnumerable(Of Boolean) = TryCast(values, IEnumerable(Of Boolean))
        Dim nums As IEnumerable(Of Double) = TryCast(values, IEnumerable(Of Double))
        Dim raws As IEnumerable(Of Byte) = TryCast(values, IEnumerable(Of Byte))
        Dim sexpVec As SymbolicExpression = TryCast(values, SymbolicExpression)

        If sexpVec IsNot Nothing AndAlso sexpVec.IsVector() Then
            Return sexpVec
        End If
        If ints IsNot Nothing Then
            Return engine.CreateIntegerVector(ints)
        End If
        If chars IsNot Nothing Then
            Return engine.CreateCharacterVector(chars)
        End If
        If cplxs IsNot Nothing Then
            Return engine.CreateComplexVector(cplxs)
        End If
        If logicals IsNot Nothing Then
            Return engine.CreateLogicalVector(logicals)
        End If
        If nums IsNot Nothing Then
            Return engine.CreateNumericVector(nums)
        End If
        If raws IsNot Nothing Then
            Return engine.CreateRawVector(raws)
        End If
        Throw New NotSupportedException(String.Format("Cannot convert type {0} to an R vector", values.[GetType]()))

    End Function

    ''' <summary>
    ''' Creates a new environment.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="parent">The parent environment.</param>
    ''' <returns>The newly created environment.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateEnvironment(engine As REngine, parent As REnvironment) As REnvironment
        If engine Is Nothing Then
            Throw New ArgumentNullException("engine")
        End If
        If parent Is Nothing Then
            Throw New ArgumentNullException("parent")
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New REnvironment(engine, parent)
    End Function

    ''' <summary>
    ''' Creates a new isolated environment.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <returns>The newly created isolated environment.</returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function CreateIsolatedEnvironment(engine As REngine) As REnvironment
        If engine Is Nothing Then
            Throw New ArgumentNullException("engine")
        End If
        If Not engine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New REnvironment(engine, engine.GetFunction(Of Rf_allocSExp)()(SymbolicExpressionType.Environment))
    End Function
End Module
