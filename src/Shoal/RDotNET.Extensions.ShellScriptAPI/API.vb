Imports RDotNET.Internals
Imports System.Collections
Imports System.Collections.Generic
Imports System.Numerics
Imports System.Runtime.CompilerServices
Imports RDotNET.REngineExtension
Imports RDotNET.SymbolicExpressionExtension
Imports RDotNET.Extensions.VisualBasic.Serialization
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.RSystem
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.HybridsScripting
Imports Microsoft.VisualBasic.CommandLine.Reflection

<LanguageEntryPoint("R.NET", "R API interface to the shoal shell language.")>
<[PackageNamespace]("R.NET",
                    Description:="R API interface to the shoal shell language.",
                    Category:=APICategories.SoftwareTools,
                    Publisher:="xie.guigang@gcmodeller.org",
                    Url:="https://rdotnet.codeplex.com/")>
Public Module API

    Dim REngine As REngine

#Region "Hybrid Interfaces"

    <EntryInterface(InterfaceTypes.EntryPointInit)>
    <ExportAPI("_r_dotnet._init()", Info:="Automatically search for the path of the R system and then construct a R session for you.")>
    Public Function Init() As RDotNET.REngine
        REngine = RServer
        Return RSystem.RServer
    End Function

    <EntryInterface(InterfaceTypes.Evaluate)>
    Public Function Evaluate(scriptLine As String) As Object
        Dim value = RSystem.RServer.Evaluate(statement:=scriptLine)
        Return value
    End Function

    <EntryInterface(InterfaceTypes.SetValue)>
    Public Function SetSymbol(Variable As String, value As Object) As Boolean
        Try
            Call RServer.SetSymbol(Variable, value)
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    <ExportAPI("CType", Info:="Casting R type object into a .NET object.")>
    Public Function TypeCast(<Parameter("R.Data", "The source result which was from the R expression.")>
                             RData As RDotNET.SymbolicExpression,
                             <Parameter("Cast.Type", "The System.Type schema information that will casting the R object as the .NET object.")>
                             Type As System.Type) As Object
        Return LoadRStream(RData, Type)
    End Function

#End Region

    ''' <summary>
    ''' Creates a new CharacterVector with the specified values.
    ''' </summary>
    ''' <param name="vector">The values.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("charactervector")>
    Public Function CreateCharacterVector(vector As IEnumerable(Of Object)) As CharacterVector
        Return New CharacterVector(RServer, vector.CT(Of String))
    End Function

    <Extension> Private Function CT(Of T)(data As Generic.IEnumerable(Of Object)) As T()
        Return (From item In data Select CType(item, T)).ToArray
    End Function

    ''' <summary>
    ''' Creates a new ComplexVector with the specified values.
    ''' </summary>
    ''' <param name="vector">The values.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("complexvector")>
    Public Function CreateComplexVector(vector As IEnumerable(Of Object)) As ComplexVector
        Return New ComplexVector(RServer, vector.CT(Of Complex))
    End Function

    ''' <summary>
    ''' Creates a new IntegerVector with the specified values.
    ''' </summary>
    ''' <param name="vector">The values.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("integervector")>
    Public Function CreateIntegerVector(vector As IEnumerable(Of Object)) As IntegerVector
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New IntegerVector(REngine, vector.CT(Of Integer))
    End Function

    ''' <summary>
    ''' Creates a new LogicalVector with the specified values.
    ''' </summary>
    ''' <param name="vector">The values.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("logicalvector")>
    Public Function CreateLogicalVector(vector As IEnumerable(Of Object)) As LogicalVector
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New LogicalVector(REngine, vector.CT(Of Boolean))
    End Function

    ''' <summary>
    ''' Creates a new NumericVector with the specified values.
    ''' </summary>
    ''' <param name="vector">The values.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("numericvector")>
    Public Function CreateNumericVector(vector As IEnumerable(Of Object)) As NumericVector
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New NumericVector(REngine, vector.CT(Of Double))
    End Function

    ''' <summary>
    ''' Creates a new RawVector with the specified values.
    ''' </summary>
    ''' <param name="vector">The values.</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("rawvector")>
    Public Function CreateRawVector(vector As IEnumerable(Of Object)) As RawVector
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New RawVector(REngine, vector.CT(Of Byte))
    End Function

    ''' <summary>
    ''' Create a vector with a single value
    ''' </summary>
    ''' <param name="value">The value</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("2charactervector")>
    Public Function CreateCharacter(value As String) As CharacterVector
        Return CreateCharacterVector(New String() {value})
    End Function

    ''' <summary>
    ''' Create a vector with a single value
    ''' </summary>
    ''' <param name="value">The value</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("2complexvector")>
    Public Function CreateComplex(value As Complex) As ComplexVector
        Return CreateComplexVector(New System.Numerics.Complex() {value})
    End Function

    ''' <summary>
    ''' Create a vector with a single value
    ''' </summary>
    ''' <param name="value">The value</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("2logicalvector")>
    Public Function CreateLogical(value As Boolean) As LogicalVector
        Return CreateLogicalVector(New Boolean() {value})
    End Function

    ''' <summary>
    ''' Create a vector with a single value
    ''' </summary>
    ''' <param name="value">The value</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("2numericvector")>
    Public Function CreateNumeric(value As Double) As NumericVector
        Return CreateNumericVector(New Double() {value})
    End Function

    ''' <summary>
    ''' Create an integer vector with a single value
    ''' </summary>
    ''' <param name="value">The value</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("2integervector")>
    Public Function CreateInteger(value As Integer) As IntegerVector
        Return CreateIntegerVector(New Integer() {value})
    End Function

    ''' <summary>
    ''' Create a vector with a single value
    ''' </summary>
    ''' <param name="value">The value</param>
    ''' <returns>The new vector.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("2rawvector")>
    Public Function CreateRaw(value As Byte) As RawVector
        Return CreateRawVector(New Byte() {value})
    End Function

    ''' <summary>
    ''' Creates a new empty CharacterMatrix with the specified size.
    ''' </summary>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function CreateCharacterMatrix(rowCount As Integer, columnCount As Integer) As CharacterMatrix
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New CharacterMatrix(REngine, rowCount, columnCount)
    End Function

    ''' <summary>
    ''' Creates a new empty ComplexMatrix with the specified size.
    ''' </summary>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function CreateComplexMatrix(rowCount As Integer, columnCount As Integer) As ComplexMatrix
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New ComplexMatrix(REngine, rowCount, columnCount)
    End Function

    ''' <summary>
    ''' Creates a new empty IntegerMatrix with the specified size.
    ''' </summary>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function CreateIntegerMatrix(rowCount As Integer, columnCount As Integer) As IntegerMatrix
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New IntegerMatrix(REngine, rowCount, columnCount)
    End Function

    ''' <summary>
    ''' Creates a new empty LogicalMatrix with the specified size.
    ''' </summary>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function CreateLogicalMatrix(rowCount As Integer, columnCount As Integer) As LogicalMatrix
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New LogicalMatrix(REngine, rowCount, columnCount)
    End Function

    ''' <summary>
    ''' Creates a new empty NumericMatrix with the specified size.
    ''' </summary>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function CreateNumericMatrix(rowCount As Integer, columnCount As Integer) As NumericMatrix
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New NumericMatrix(REngine, rowCount, columnCount)
    End Function

    ''' <summary>
    ''' Creates a new empty RawMatrix with the specified size.
    ''' </summary>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension>
    Public Function CreateRawMatrix(rowCount As Integer, columnCount As Integer) As RawMatrix
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New RawMatrix(REngine, rowCount, columnCount)
    End Function

    ''' <summary>
    ''' Creates a new CharacterMatrix with the specified values.
    ''' </summary>
    ''' <param name="matrix">The values.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("charactermatrix")>
    Public Function CreateCharacterMatrix(matrix As Generic.IEnumerable(Of Generic.IEnumerable(Of Object))) As CharacterMatrix
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New CharacterMatrix(REngine, matrix.CT(Of String()).VectorCollectionToMatrix)
    End Function

    ''' <summary>
    ''' Creates a new ComplexMatrix with the specified values.
    ''' </summary>
    ''' <param name="matrix">The values.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("complexmatrix")>
    Public Function CreateComplexMatrix(matrix As Generic.IEnumerable(Of Generic.IEnumerable(Of Object))) As ComplexMatrix
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New ComplexMatrix(REngine, matrix.CT(Of Complex()).VectorCollectionToMatrix)
    End Function

    ''' <summary>
    ''' Creates a new IntegerMatrix with the specified values.
    ''' </summary>
    ''' <param name="matrix">The values.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("integermatrix")>
    Public Function CreateIntegerMatrix(matrix As Generic.IEnumerable(Of Generic.IEnumerable(Of Object))) As IntegerMatrix
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New IntegerMatrix(REngine, matrix.CT(Of Integer()).VectorCollectionToMatrix)
    End Function

    ''' <summary>
    ''' Creates a new LogicalMatrix with the specified values.
    ''' </summary>
    ''' <param name="matrix">The values.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("logicalmatrix")>
    Public Function CreateLogicalMatrix(matrix As Generic.IEnumerable(Of Generic.IEnumerable(Of Object))) As LogicalMatrix
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New LogicalMatrix(REngine, matrix.CT(Of Boolean()).VectorCollectionToMatrix)
    End Function

    ''' <summary>
    ''' Creates a new NumericMatrix with the specified values.
    ''' </summary>
    ''' <param name="matrix">The values.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("numericmatrix")>
    Public Function CreateNumericMatrix(matrix As Generic.IEnumerable(Of Generic.IEnumerable(Of Object))) As NumericMatrix
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New NumericMatrix(REngine, matrix.CT(Of Double()).VectorCollectionToMatrix)
    End Function

    ''' <summary>
    ''' Creates a new RawMatrix with the specified values.
    ''' </summary>
    ''' <param name="matrix">The values.</param>
    ''' <returns>The new matrix.</returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("rawmatrix")>
    Public Function CreateRawMatrix(matrix As Generic.IEnumerable(Of Generic.IEnumerable(Of Object))) As RawMatrix
        If REngine Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If Not REngine.IsRunning Then
            Throw New ArgumentException()
        End If
        Return New RawMatrix(REngine, matrix.CT(Of Byte()).VectorCollectionToMatrix)
    End Function

    ''' <summary>
    ''' Create an R data frame from managed arrays and objects.
    ''' </summary>
    ''' <param name="columns">The columns with the values for the data frame. These must be array of supported types (double, string, bool, integer, byte)</param>
    ''' <param name="columnNames">Column names. default: null.</param>
    ''' <param name="rowNames">Row names. Default null.</param>
    ''' <param name="checkRows">Check rows. See data.frame R documentation</param>
    ''' <param name="checkNames">See data.frame R documentation</param>
    ''' <param name="stringsAsFactors">Should columns of strings be considered as factors (categories). See data.frame R documentation</param>
    ''' <returns></returns>
    <System.Runtime.CompilerServices.Extension>
    <ExportAPI("data.frame")>
    Public Function CreateDataFrame(columns As Generic.IEnumerable(Of Object),
                                    Optional columnNames As String() = Nothing,
                                    Optional rowNames As String() = Nothing,
                                    Optional checkRows As Boolean = False,
                                    Optional checkNames As Boolean = True,
                                    Optional stringsAsFactors As Boolean = True) As DataFrame

        Dim df = REngine.GetSymbol("data.frame").AsFunction()
        Dim colVectors As SymbolicExpression() = ToVectors((From item In columns Select CType(item, IEnumerable)).ToArray)
        Dim namedColArgs As Tuple(Of String, SymbolicExpression)() = CreateNamedArgs(colVectors, columnNames)
        Dim args = New List(Of Tuple(Of String, SymbolicExpression))(namedColArgs)
        If rowNames IsNot Nothing Then
            args.Add(Tuple.Create("row.names", DirectCast(REngine.CreateCharacterVector(rowNames), SymbolicExpression)))
        End If
        args.Add(Tuple.Create("check.rows", DirectCast(REngine.CreateLogical(checkRows), SymbolicExpression)))
        args.Add(Tuple.Create("check.names", DirectCast(REngine.CreateLogical(checkNames), SymbolicExpression)))
        args.Add(Tuple.Create("stringsAsFactors", DirectCast(REngine.CreateLogical(stringsAsFactors), SymbolicExpression)))
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

    Friend Function ToVectors(columns As IEnumerable()) As SymbolicExpression()
        Return Array.ConvertAll(columns, Function(x) ToVector(x))
    End Function

    Friend Function ToVector(values As IEnumerable) As SymbolicExpression
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
            Return REngine.CreateIntegerVector(ints)
        End If
        If chars IsNot Nothing Then
            Return REngine.CreateCharacterVector(chars)
        End If
        If cplxs IsNot Nothing Then
            Return REngine.CreateComplexVector(cplxs)
        End If
        If logicals IsNot Nothing Then
            Return REngine.CreateLogicalVector(logicals)
        End If
        If nums IsNot Nothing Then
            Return REngine.CreateNumericVector(nums)
        End If
        If raws IsNot Nothing Then
            Return REngine.CreateRawVector(raws)
        End If
        Throw New NotSupportedException(String.Format("Cannot convert type {0} to an R vector", values.[GetType]()))

    End Function
End Module

