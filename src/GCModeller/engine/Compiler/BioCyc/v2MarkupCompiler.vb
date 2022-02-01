Imports Microsoft.VisualBasic.CommandLine
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.CompilerServices

Namespace BioCyc

    Public Class v2MarkupCompiler : Inherits Compiler(Of VirtualCell)

        ReadOnly biocyc As Workspace

        Sub New(biocyc As Workspace)
            Me.biocyc = biocyc
        End Sub

        Protected Overrides Function CompileImpl(args As CommandLine) As Integer

        End Function
    End Class
End Namespace