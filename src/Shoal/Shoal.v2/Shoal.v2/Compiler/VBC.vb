Imports Microsoft.VisualBasic.Emit.CodeDOM_VBC
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM

Namespace Compiler

    Public Class VBC

        Dim SPMgr As SPM.ShoalPackageMgr

        Public ReadOnly Property DefaultAppIcon As String = App.LocalDataTemp & "/Shoal.ico"

        Sub New(SPMgrDb As SPM.PackageModuleDb)
            SPMgr = New SPM.ShoalPackageMgr(SPMgrDb)
            Call My.Resources.Shoal.FlushStream(DefaultAppIcon)
        End Sub

        Public Function Compile(scriptPath As String, Output As String) As String
            Dim LDM As SyntaxModel = SyntaxModel.LoadFile(scriptPath)

            Dim assm As New CodeDOM.Program
            Call __compile(assm, LDM)

            Dim CodePreview As String = assm.Assembly.GenerateCode
            Call CodePreview.__DEBUG_ECHO
            Dim refList As String() = Parallel.GetReferences(GetType(VBC))

            Dim Exe = CodeDOMExtension.Compile(
                assm.Assembly,
                Options:=CodeDOMExtension.ExecutableProfile,
                Reference:=refList,
                DotNETReferenceAssembliesDir:=Parallel.ParallelLoading.RunTimeDirectory)
            Call FileIO.FileSystem.ReadAllBytes(Exe.Location).FlushStream(Output)
            Return CodePreview
        End Function

        Private Sub __compile(ByRef assm As CodeDOM.Program, Script As Interpreter.LDM.SyntaxModel)
            For Each Expr In Script.Expressions
                Select Case Expr.ExprTypeID
                    Case Interpreter.LDM.Expressions.ExpressionTypes.CollectionOpr
                    Case Interpreter.LDM.Expressions.ExpressionTypes.Comments
                    Case Interpreter.LDM.Expressions.ExpressionTypes.Die
                    Case Interpreter.LDM.Expressions.ExpressionTypes.GoTo
                    Case Interpreter.LDM.Expressions.ExpressionTypes.FunctionCalls
                    Case Interpreter.LDM.Expressions.ExpressionTypes.LineLable
                    Case Interpreter.LDM.Expressions.ExpressionTypes.Return
                        Call assm.__return(Expr.As(Of Interpreter.LDM.Expressions.Keywords.Return))
                    Case Interpreter.LDM.Expressions.ExpressionTypes.VariableDeclaration
                        Call assm.__localsInit(Expr.As(Of Interpreter.LDM.Expressions.VariableDeclaration))
                End Select
            Next
        End Sub
    End Class
End Namespace