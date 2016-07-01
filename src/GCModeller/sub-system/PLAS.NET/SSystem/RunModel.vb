Imports LANS.SystemsBiology.AnalysisTools.CellPhenotype.SSystem.Script
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream

Public Module RunModel

    Public Delegate Function IRunModel(args As CommandLine) As Integer

    Public ReadOnly Property RunMethods As IReadOnlyDictionary(Of String, Func(Of CommandLine, Integer)) =
        New HashDictionary(Of Func(Of CommandLine, Integer)) From {
 _
            {"script", AddressOf RunScript},
            {"model", AddressOf RunModel},
            {"sbml", AddressOf RunSBML}
    }

    Public Function RunScript(args As CommandLine) As Integer
        Return RunModel(Script.ScriptCompiler.Compile(Path:=args("-i")), args:=args)
    End Function

    Public Function RunModel(args As CommandLine) As Integer
        Return RunModel(Model.Load(args("-i")), args:=args)
    End Function

    Public Function RunSBML(args As CommandLine) As Integer
        Return RunModel(Model:=SBML.Compile(args("-i")), args:=args)
    End Function

    Public Function RunModel(Model As Script.Model, args As CommandLine) As Integer
        Dim CSV = Kernel.Kernel.Run(Model)
        Dim Out As String = args("-o")

        Call CSV.Save(Path:=Out)

        If String.Equals(args("-chart"), "T") Then
            Using Wrapper As DataFrame = DataFrame.CreateObject(CSV)
                ' Call Wrapper.ShowDialog()
            End Using
        End If
        Return 0
    End Function
End Module
