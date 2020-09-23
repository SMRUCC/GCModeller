#Region "Microsoft.VisualBasic::58887c31097439678c94dbf30a070549, sub-system\FBA\FBA_DP\FBA\RSolver.vb"

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

    ' Class FBAlpRSolver
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: __initLpSolver, RSolving
    ' 
    '     Sub: (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
Imports RDotNet.Extensions.VisualBasic
Imports RDotNet.Extensions.VisualBasic.API.base
Imports RDotNet.Extensions.VisualBasic.API.utils

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
    Public Function RSolving(lpSolveRModel As lpSolveRModel, Optional ByRef script As String = "") As lpOUT
        printf("Start to compiling the input model.")
        printf("Please wait for a while...")
        script = lpSolveRModel.RScript
        printf("Compile job done! \nstart to solve this FBA model using R.")
        printf("Pushing the R script to the REngine...")

        SyncLock R
            With R
                .call = script

                printf("FBA model computation job done!")

                printf("Get the objective function value...")
                Dim Objective As String = .WriteLine("get.objective(lprec)")(Scan0)  'Get Objective Function value

                printf("Get the flux value for each reaction...")
                Dim FluxsDistribution As String() = .WriteLine("get.variables(lprec)")

                Return New lpOUT With {
                    .Objective = Objective,
                    .FluxsDistribution = FluxsDistribution
                }
            End With
        End SyncLock
    End Function

    Private Function __initLpSolver() As Boolean
        Printf("R << Installed.Packages(""lpSolveAPI"")")
        If installed.packages("lpSolveAPI") Is Nothing Then
            printf("Package 'lpSolveAP' was not installed, trying install this package...")
            printf("R << Install.Packages(""lpSolveAPI"")")
            If Not install.packages("lpSolveAPI") Then
                printf("Install library package ""lpsolveAPI"" failured!")
                Return False
            Else
                printf("Install library package ""lpsolveAPI"" successfully!")
            End If
        End If

        printf("R << Library(lpSolveAPI)")
        Library("lpSolveAPI")
        printf("Successfully load library 'lpSolveAPI'")
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
        System.GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
