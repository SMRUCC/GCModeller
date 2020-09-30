Imports Microsoft.VisualBasic.CommandLine
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.CompilerServices

Namespace AssemblyScript

    Public Class Compiler : Inherits Compiler(Of VirtualCell)

        Dim registry As Registry
        Dim session As Environment

        Public Shared Function Build(vhd As String, registry As Registry) As VirtualCell
            Dim assemblyScript As VHDFile = VHDFile.Parse(vhd)
        End Function

        Protected Overrides Function CompileImpl(args As CommandLine) As Integer
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace