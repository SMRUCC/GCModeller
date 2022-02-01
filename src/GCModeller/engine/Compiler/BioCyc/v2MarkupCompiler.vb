Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.CompilerServices

Namespace BioCyc

    Public Class v2MarkupCompiler : Inherits Compiler(Of VirtualCell)

        ReadOnly biocyc As Workspace
        ReadOnly genbank As GBFF.File

        Sub New(genbank As GBFF.File, biocyc As Workspace)
            Me.biocyc = biocyc
            Me.genbank = genbank
        End Sub

        Protected Overrides Function PreCompile(args As CommandLine) As Integer
            Dim info As New StringBuilder

            Using writer As New StringWriter(info)
                Call CLITools.AppSummary(GetType(v2MarkupCompiler).Assembly.FromAssembly, "", "", writer)
            End Using

            m_compiledModel = New VirtualCell With {
                .taxonomy = genbank.Source.GetTaxonomy
            }
            m_logging.WriteLine(info.ToString)

            Return 0
        End Function

        Protected Overrides Function CompileImpl(args As CommandLine) As Integer

        End Function
    End Class
End Namespace