#Region "Microsoft.VisualBasic::5cfbc87efded66b6a6f8210c4b3c89f8, ..\GCModeller\sub-system\PLAS.NET\SSystem\RunModel.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports SMRUCC.genomics.Analysis.SSystem.Script

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
        Return RunModel(Script.ScriptCompiler.Compile(path:=args("-i")), args:=args)
    End Function

    Public Function RunModel(args As CommandLine) As Integer
        Return RunModel(Script.Model.Load(args("-i")), args:=args)
    End Function

    Public Function RunSBML(args As CommandLine) As Integer
        Return RunModel(Model:=SBML.Compile(args("-i")), args:=args)
    End Function

    Public Function RunModel(Model As Script.Model, args As CommandLine) As Integer
        Dim CSV = Kernel.Kernel.Run(Model)
        Dim Out As String = args("-o")

        Call CSV.SaveTo(path:=Out)

        If String.Equals(args("-chart"), "T") Then
            '     Using Wrapper As DataFrame = DataFrame.CreateObject(CSV)
            ' Call Wrapper.ShowDialog()
            '  End Using
        End If
        Return 0
    End Function
End Module

