Imports System.Diagnostics
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra.LinearProgramming
Imports std = System.Math
Imports SMRUCC.genomics.Analysis.FBA
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

Module Program

    Const GEM_MODEL As String = "N:\NVP\models\GEMs\Streptomyces botrytidirepellens NEAU-LD23.xml"
    Const TOL As Double = 0.000001

    Dim passed As Integer = 0
    Dim failed As Integer = 0

    Sub Main(args As String())
        Dim mode As String = If(args.IsNullOrEmpty, "all", args(0).ToLower)

        If mode = "small" OrElse mode = "all" Then
            Call RunSmallScaleTests()
        End If
        If mode = "gem" OrElse mode = "all" Then
            Call RunGemTest(If(args.Length > 1, args(1), GEM_MODEL))
        End If

        Pause()
    End Sub

    Private Sub Check(name As String, expected As Double, actual As Double)
        Dim ok As Boolean = std.Abs(expected - actual) < TOL

        If ok Then
            passed += 1
            Console.WriteLine($"  [PASS] {name}: expected {expected}, actual {actual.ToString("G6")}")
        Else
            failed += 1
            Console.WriteLine($"  [FAIL] {name}: expected {expected}, actual {actual.ToString("G6")}, error = {(actual - expected).ToString("G4")}")
        End If
    End Sub

    ''' <summary>
    ''' 小规模问题：验证LPP求解器计算结果的正确性
    ''' </summary>
    Private Sub RunSmallScaleTests()
        Console.WriteLine("=========================================================")
        Console.WriteLine(" small scale LPP validation")
        Console.WriteLine("=========================================================")

        Call TestSparseTableauRow()
        Call TestClassicMaximize()
        Call TestMinimize()
        Call TestGreaterThanConstraint()
        Call TestEqualityWithBounds()
        Call TestNegativeLowerBound()
        Call TestSmallFbaNetwork()

        Console.WriteLine()
        Console.WriteLine($" small scale result: {passed} passed, {failed} failed")
        Console.WriteLine()
    End Sub

    ''' <summary>
    ''' the sparse tableau row is the core data structure of the simplex
    ''' method, checks the AXPY operation of this data structure
    ''' </summary>
    Private Sub TestSparseTableauRow()
        Dim a As New SparseTableauRow(4)
        Dim b As New SparseTableauRow(4)
        Dim c As New SparseTableauRow(4)

        a.Add(1, 2.0)
        a.Add(3, 4.0)
        a.Add(5, 6.0)

        b.Add(2, 1.0)
        b.Add(3, 3.0)
        b.Add(7, 2.0)

        ' a = a - 2b
        Call a.Axpy(b, 2.0, 0.0)

        Console.WriteLine("[0] sparse tableau row AXPY")

        Call Check("non-zeros", 5, a.Count)
        Call Check("a[1]", 2, a.Item(1))
        Call Check("a[2]", -2, a.Item(2))
        Call Check("a[3]", -2, a.Item(3))
        Call Check("a[5]", 6, a.Item(5))
        Call Check("a[7]", -4, a.Item(7))

        ' runs the AXPY again for checking the ping-pong buffer
        c.Add(1, 1.0)
        c.Add(6, 5.0)

        Call a.Axpy(c, 1.0, 0.0)

        Call Check("non-zeros(2)", 6, a.Count)
        Call Check("a[1](2)", 1, a.Item(1))
        Call Check("a[3](2)", -2, a.Item(3))
        Call Check("a[6](2)", -5, a.Item(6))

        ' the row should be kept in ascending order
        For i As Integer = 1 To a.Count - 1
            If a.Idx(i) <= a.Idx(i - 1) Then
                failed += 1
                Console.WriteLine("  [FAIL] the column index is not in ascending order")
                Exit For
            End If
        Next
    End Sub

    ''' <summary>
    ''' max 3x + 5y
    ''' s.t. x &lt;= 4, 2y &lt;= 12, 3x + 2y &lt;= 18
    ''' the optimal solution is x = 2, y = 6, objective = 36
    ''' </summary>
    Private Sub TestClassicMaximize()
        Dim A As Double()() = {
            New Double() {1.0, 0.0},
            New Double() {0.0, 2.0},
            New Double() {3.0, 2.0}
        }
        Dim lpp As New LPP(
            objectiveFunctionType:="Maximize",
            variableNames:={"x", "y"},
            objectiveFunctionCoefficients:={3.0, 5.0},
            constraintCoefficients:=A,
            constraintTypes:={"<=", "<=", "<="},
            constraintRightHandSides:={4.0, 12.0, 18.0},
            objectiveFunctionValue:=0
        )
        Dim result As LPPSolution = lpp.solve(showProgress:=False, strict:=True)

        Console.WriteLine("[1] classic maximize problem")

        If Not result.failureMessage.StringEmpty Then
            failed += 1
            Console.WriteLine($"  [FAIL] solver returns error: {result.failureMessage}")
            Return
        End If

        Call Check("objective", 36, result.ObjectiveFunctionValue)
        Call Check("x", 2, result.GetSolution("x"))
        Call Check("y", 6, result.GetSolution("y"))
    End Sub

    ''' <summary>
    ''' min 3x + 5y on the same constraint set, the optimal solution is zero
    ''' </summary>
    Private Sub TestMinimize()
        Dim A As Double()() = {
            New Double() {1.0, 0.0},
            New Double() {0.0, 2.0},
            New Double() {3.0, 2.0}
        }
        Dim lpp As New LPP(
            objectiveFunctionType:="Minimize",
            variableNames:={"x", "y"},
            objectiveFunctionCoefficients:={3.0, 5.0},
            constraintCoefficients:=A,
            constraintTypes:={"<=", "<=", "<="},
            constraintRightHandSides:={4.0, 12.0, 18.0},
            objectiveFunctionValue:=0
        )
        Dim result As LPPSolution = lpp.solve(showProgress:=False, strict:=True)

        Console.WriteLine("[2] classic minimize problem")

        If Not result.failureMessage.StringEmpty Then
            failed += 1
            Console.WriteLine($"  [FAIL] solver returns error: {result.failureMessage}")
            Return
        End If

        Call Check("objective", 0, result.ObjectiveFunctionValue)
        Call Check("x", 0, result.GetSolution("x"))
        Call Check("y", 0, result.GetSolution("y"))
    End Sub

    ''' <summary>
    ''' min 3x + 5y
    ''' s.t. x + y &gt;= 5, 2x + y &gt;= 8
    ''' the optimal solution is x = 5, y = 0, objective = 15
    ''' </summary>
    Private Sub TestGreaterThanConstraint()
        Dim A As Double()() = {
            New Double() {1.0, 1.0},
            New Double() {2.0, 1.0}
        }
        Dim lpp As New LPP(
            objectiveFunctionType:="Minimize",
            variableNames:={"x", "y"},
            objectiveFunctionCoefficients:={3.0, 5.0},
            constraintCoefficients:=A,
            constraintTypes:={">=", ">="},
            constraintRightHandSides:={5.0, 8.0},
            objectiveFunctionValue:=0
        )
        Dim result As LPPSolution = lpp.solve(showProgress:=False, strict:=True)

        Console.WriteLine("[3] minimize problem with >= constraints")

        If Not result.failureMessage.StringEmpty Then
            failed += 1
            Console.WriteLine($"  [FAIL] solver returns error: {result.failureMessage}")
            Return
        End If

        Call Check("objective", 15, result.ObjectiveFunctionValue)
        Call Check("x", 5, result.GetSolution("x"))
        Call Check("y", 0, result.GetSolution("y"))
    End Sub

    ''' <summary>
    ''' max x + y
    ''' s.t. x + y = 10, 0 &lt;= x &lt;= 3, 0 &lt;= y &lt;= 10
    ''' the optimal solution is x = 3, y = 7, objective = 10
    ''' 
    ''' this test case checks the upper bound handling of the bounded simplex
    ''' </summary>
    Private Sub TestEqualityWithBounds()
        Dim A As Double()() = {
            New Double() {1.0, 1.0}
        }
        Dim lpp As New LPP(
            objectiveFunctionType:="Maximize",
            variableNames:={"x", "y"},
            objectiveFunctionCoefficients:={1.0, 1.0},
            constraintCoefficients:=A,
            constraintTypes:={"="},
            constraintRightHandSides:={10.0},
            objectiveFunctionValue:=0,
            lowerBounds:={0.0, 0.0},
            upperBounds:={3.0, 10.0}
        )
        Dim result As LPPSolution = lpp.solve(showProgress:=False, strict:=True)

        Console.WriteLine("[4] maximize with variable upper bounds")

        If Not result.failureMessage.StringEmpty Then
            failed += 1
            Console.WriteLine($"  [FAIL] solver returns error: {result.failureMessage}")
            Return
        End If

        Call Check("objective", 10, result.ObjectiveFunctionValue)
        Call Check("x", 3, result.GetSolution("x"))
        Call Check("y", 7, result.GetSolution("y"))
    End Sub

    ''' <summary>
    ''' min x
    ''' s.t. x + y &gt;= 1, -5 &lt;= x &lt;= 5, 0 &lt;= y &lt;= 2
    ''' the optimal solution is x = -1, y = 2, objective = -1
    ''' 
    ''' this test case checks the negative lower bound handling: the variable
    ''' with a negative lower bound is splitted into two non-negative parts
    ''' </summary>
    Private Sub TestNegativeLowerBound()
        Dim A As Double()() = {
            New Double() {1.0, 1.0}
        }
        Dim lpp As New LPP(
            objectiveFunctionType:="Minimize",
            variableNames:={"x", "y"},
            objectiveFunctionCoefficients:={1.0, 0.0},
            constraintCoefficients:=A,
            constraintTypes:={">="},
            constraintRightHandSides:={1.0},
            objectiveFunctionValue:=0,
            lowerBounds:={-5.0, 0.0},
            upperBounds:={5.0, 2.0}
        )
        Dim result As LPPSolution = lpp.solve(showProgress:=False, strict:=True)

        Console.WriteLine("[5] minimize with a negative lower bound")

        If Not result.failureMessage.StringEmpty Then
            failed += 1
            Console.WriteLine($"  [FAIL] solver returns error: {result.failureMessage}")
            Return
        End If

        Call Check("objective", -1, result.ObjectiveFunctionValue)
        Call Check("x", -1, result.GetSolution("x"))
        Call Check("y", 2, result.GetSolution("y"))
    End Sub

    ''' <summary>
    ''' a small metabolic network:
    ''' 
    ''' R1: =&gt; A  (uptake)
    ''' R2: A =&gt; B
    ''' R3: B =&gt;  (the objective, biomass)
    ''' R4: A =&gt;  (a bypass reaction)
    ''' 
    ''' mass balance: A: v1 - v2 - v4 = 0, B: v2 - v3 = 0
    ''' all of the flux is limited in [0, 10], the maximum of the biomass 
    ''' flux v3 should be 10, with v1 = 10, v2 = 10, v4 = 0
    ''' </summary>
    Private Sub TestSmallFbaNetwork()
        Dim stoichiometry As LpSparseMatrix = LpSparseMatrix.FromJagged({
            New Double() {1.0, -1.0, 0.0, -1.0},
            New Double() {0.0, 1.0, -1.0, 0.0}
        })
        Dim matrix As New Matrix With {
            .Stoichiometry = stoichiometry,
            .Compounds = {"A", "B"},
            .Flux = New Dictionary(Of String, DoubleRange) From {
                {"R1", New DoubleRange(0, 10)},
                {"R2", New DoubleRange(0, 10)},
                {"R3", New DoubleRange(0, 10)},
                {"R4", New DoubleRange(0, 10)}
            },
            .Targets = {"R3"}
        }
        Dim result As LPPSolution = New LinearProgrammingEngine().Run(matrix, OptimizationType.MAX)
        Dim flux As Dictionary(Of String, Double) = result _
            .GetSolution() _
            .ToDictionary(Function(v) v.Name, Function(v) v.Value)

        Console.WriteLine("[6] small FBA network")

        If Not result.failureMessage.StringEmpty Then
            failed += 1
            Console.WriteLine($"  [FAIL] solver returns error: {result.failureMessage}")
            Return
        End If

        Call Check("objective(biomass)", 10, result.ObjectiveFunctionValue)
        Call Check("v1", 10, flux("R1"))
        Call Check("v2", 10, flux("R2"))
        Call Check("v3", 10, flux("R3"))
        Call Check("v4", 0, flux("R4"))
    End Sub

    ''' <summary>
    ''' 大规模问题：完整的GEM模型的FBA求解测试
    ''' </summary>
    Private Sub RunGemTest(modelFile As String)
        Console.WriteLine("=========================================================")
        Console.WriteLine(" genome scale GEM model FBA test")
        Console.WriteLine($" model: {modelFile}")
        Console.WriteLine("=========================================================")

        Dim watch As Stopwatch = Stopwatch.StartNew
        Dim gem As VirtualCell = modelFile.LoadXml(Of VirtualCell)
        Dim reactions = gem.metabolismStructure.reactions.AsEnumerable.ToArray
        Dim reversible As New Dictionary(Of String, Boolean)

        Console.WriteLine($"load GEM model in {watch.ElapsedMilliseconds} ms, {reactions.Length} reactions")

        ' 可逆性判定：方程式中出现 "<=>" 即为可逆反应
        For Each reaction In reactions
            Dim note As String = If(reaction.note, "")

            reversible(reaction.ID) = note.Contains("<=>")
        Next

        watch.Restart()

        Dim metabolic As Equation() = reactions _
            .Select(Function(r) r.BuildEquation) _
            .ToArray
        Dim matrix As Matrix = metabolic.BuildMatrix()
        Dim nReversible As Integer = 0

        For Each id As String In matrix.Flux.Keys.ToArray
            If reversible.ContainsKey(id) AndAlso reversible(id) Then
                matrix.Flux(id) = New DoubleRange(-1000, 1000)
                nReversible += 1
            Else
                matrix.Flux(id) = New DoubleRange(0, 1000)
            End If
        Next

        Console.WriteLine($"build stoichiometric matrix in {watch.ElapsedMilliseconds} ms: {matrix.Stoichiometry}")
        Console.WriteLine($"  {nReversible} reversible reactions, {matrix.Flux.Count - nReversible} irreversible reactions")

        watch.Restart()

        Dim result As LPPSolution = New LinearProgrammingEngine().Run(matrix)

        Console.WriteLine($"solve FBA problem in {watch.ElapsedMilliseconds} ms")

        If Not result.failureMessage.StringEmpty Then
            Console.WriteLine($" [FAILED] {result.failureMessage}")
            Return
        End If

        Dim flux = result.GetSolution.ToArray
        Dim nonZero As Integer = flux.Count(Function(v) std.Abs(v.Value) > TOL)
        Dim top As NamedValue(Of Double)() = flux _
            .Where(Function(v) std.Abs(v.Value) > TOL) _
            .OrderByDescending(Function(v) std.Abs(v.Value)) _
            .Take(10) _
            .ToArray

        Console.WriteLine()
        Console.WriteLine($" objective (sum of all flux) = {result.ObjectiveFunctionValue.ToString("G6")}")
        Console.WriteLine($" non-zero flux: {nonZero} / {flux.Length} ({100 * nonZero / flux.Length}%)")
        Console.WriteLine($" solve time = {result.SolveTime} ms, phase 1 time = {result.FeasibleSolutionTime} ms")
        Console.WriteLine()
        Console.WriteLine(" top 10 flux:")

        For Each v In top
            Console.WriteLine($"   {v.Name} = {v.Value.ToString("G5")}")
        Next

        Console.WriteLine()
    End Sub

End Module
