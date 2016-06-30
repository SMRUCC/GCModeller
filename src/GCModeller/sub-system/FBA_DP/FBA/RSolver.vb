Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Extensions
Imports System.Text
Imports RDotNET
Imports RDotNET.Extensions.VisualBasic.RSystem
Imports RDotNET.Extensions.VisualBasic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream

''' <summary>
''' 求解FBA线性规划问题的模块对象
''' </summary>
Public Class FBAlpRSolver : Implements IDisposable

    ''' <summary>
    ''' 初始化一个求解FBA线性规划问题的模块对象实例
    ''' </summary>
    ''' <param name="rBin">Parameter for manual initialize the REngine, Example likes: "C:\Program Files\R\R-3.1.0\bin".</param>
    ''' <remarks></remarks>
    Sub New(rBin As String)
        Printf("REngine was found at location: ""%s""", rBin)
        Printf("Initialize R...")
        Call RSystem.TryInit(rBin)

        If __initLpSolver() = False Then
            Printf("An unexpect error occur while the program trying to initialize the R program.")
            Printf("Operation aborted!")
            Throw New TaskSchedulerException(INIT_FAILURE)
        End If
    End Sub

    Const INIT_FAILURE As String =
        "[Operation aborted!] An unexpect error occur while the program trying to initialize the R program."

    ''' <summary>
    ''' using the R program to solve the linear programming problem that from the fba model.
    ''' (使用R模块来求解FBA模型中的线性规划问题, {ObjectiveFunction, FluxDistribution()})
    ''' </summary>
    ''' <param name="lpSolveRModel"></param>
    ''' <returns>{ObjectiveFunction, FluxDistribution()}</returns>
    ''' <param name="script">Generated script output</param>
    ''' <remarks></remarks>
    Public Function RSolving(lpSolveRModel As FBA.lpSolveRModel, Optional ByRef script As String = "") As lpOUT
        Printf("Start to compiling the input model.")
        Printf("Please wait for a while...")
        script = lpSolveRModel.RScript
        Printf("Compile job done! \nstart to solve this FBA model using R.")

        Printf("Pushing the R script to the REngine...")
        Call RServer.Evaluate(script)
        Printf("FBA model computation job done!")

        Printf("Get the objective function value...")
        Dim Objective As String = RServer.WriteLine("get.objective(lprec)")(Scan0)  'Get Objective Function value

        Printf("Get the flux value for each reaction...")
        Dim FluxsDistribution As String() = RServer.WriteLine("get.variables(lprec)")

        Return New lpOUT With {
            .Objective = Objective,
            .FluxsDistribution = FluxsDistribution
        }
    End Function

    Private Function __initLpSolver() As Boolean
        Printf("R << Installed.Packages(""lpSolveAPI"")")
        If Not Installed.Packages("lpSolveAPI") Then
            Printf("Package 'lpSolveAP' was not installed, trying install this package...")
            Printf("R << Install.Packages(""lpSolveAPI"")")
            If Not Install.Packages("lpSolveAPI") Then
                Printf("Install library package ""lpsolveAPI"" failured!")
                Return False
            Else
                Printf("Install library package ""lpsolveAPI"" successfully!")
            End If
        End If

        Printf("R << Library(lpSolveAPI)")
        Library("lpSolveAPI")
        Printf("Successfully load library 'lpSolveAPI'")
        Return True
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 检测冗余的调用

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO:  释放托管状态(托管对象)。
            End If

            ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
            ' TODO:  将大型字段设置为 null。
        End If
        Me.disposedValue = True
    End Sub

    ' TODO:  仅当上面的 Dispose( disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
    'Protected Overrides Sub Finalize()
    '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose( disposing As Boolean)中。
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Visual Basic 添加此代码是为了正确实现可处置模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class