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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Mathematical.diffEq
Imports SMRUCC.genomics.Analysis.SSystem.Script

Public Module RunModel

    ''' <summary>
    ''' Run model from commandline.
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    Public Delegate Function IRunModel(args As CommandLine) As Integer

    Public ReadOnly Property RunMethods As IReadOnlyDictionary(Of String, IRunModel) =
        New HashDictionary(Of IRunModel) From {
 _
            {"script", AddressOf RunScript},
            {"model", AddressOf RunModel},
            {"sbml", AddressOf RunSBML}
    }

    Public Function RunScript(args As CommandLine) As Integer
        Return Script.ScriptCompiler.Compile(path:=args("-i")).RunModel(args:=args)
    End Function

    Public Function RunModel(args As CommandLine) As Integer
        Return Script.Model.Load(args("-i")).RunModel(args:=args)
    End Function

    Public Function RunSBML(args As CommandLine) As Integer
        Return SBML.Compile(args("-i")).RunModel(args:=args)
    End Function

    ' /precise 0.1
    ' /time 10

    <Extension>
    Public Function RunModel(Model As Script.Model, args As CommandLine) As Integer
        Dim t As Double = args.GetDouble("/time")
        Dim out As String = args.GetValue("-o", args("-i").TrimSuffix & ".out.Csv")

        If t > 0 Then
            Model.FinalTime = t
        End If

        If args.GetBoolean("/ODEs") Then
            Call "PLAS using ODEs solver....".__DEBUG_ECHO

            Dim p As Double = args.GetValue("/precise", 10000)
            Dim output As out = Kernel.ODEs.RunSystem(Model)
            Dim df As File = output.DataFrame(xDisp:="#Time")

            Return df.Save(out, Encodings.ASCII)
        Else
            Dim p As Double = args.GetValue("/precise", 0.1)
            Dim ds As IEnumerable(Of DataSet) = Kernel.Kernel.Run(Model, p)
            Dim maps As New Dictionary(Of String, String) From {
                {NameOf(DataSet.Identifier), "#Time"}
            }
            Return ds.SaveTo(path:=out, nonParallel:=True, maps:=maps).CLICode
        End If
    End Function
End Module

