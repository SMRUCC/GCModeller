
Imports Microsoft.VisualBasic.CommandLine
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.CompilerServices

Namespace MarkupCompiler

    Public Class v2MarkupCompiler : Inherits Compiler(Of VirtualCell)

        Protected Overrides Function CompileImpl(args As CommandLine) As Integer
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace