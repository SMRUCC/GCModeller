#Region "Microsoft.VisualBasic::342fc4e96b32048b3ea4ec766bdfc8e4, engine\vcellkit\Modeller\Modeller.vb"

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

' Module vcellModeller
' 
'     Function: applyKinetics, LoadVirtualCell, WriteZipAssembly
' 
'     Sub: createKineticsDbCache
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Data.SABIORK
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.Compiler
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RExpr = SMRUCC.Rsharp.Interpreter.ExecuteEngine.Expression

''' <summary>
''' virtual cell network kinetics modeller
''' </summary>
<Package("modeller", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gcmodeller.org")>
Module vcellModeller

    ' ((kcat * E) * S) / (Km + S)
    ' (Vmax * S) / (Km + S)

    ''' <summary>
    ''' apply the kinetics parameters from the sabio-rk database.
    ''' </summary>
    ''' <param name="vcell"></param>
    ''' <returns></returns>
    <ExportAPI("apply.kinetics")>
    Public Function applyKinetics(vcell As VirtualCell, Optional cache$ = "./.cache") As VirtualCell
        Return New Modeller(vcell, cache).Compile()
    End Function

    ''' <summary>
    ''' create data repository from the sabio-rk database
    ''' </summary>
    ''' <param name="export$"></param>
    <ExportAPI("cacheOf.enzyme_kinetics")>
    Public Sub createKineticsDbCache(Optional export$ = "./", Optional ko01000 As String = "ko01000")
        Call htext.GetInternalResource(ko01000) _
            .QueryByECNumbers(export) _
            .ToArray
    End Sub

    ''' <summary>
    ''' read the virtual cell model file
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    <ExportAPI("read.vcell")>
    Public Function LoadVirtualCell(path As String) As VirtualCell
        Return path.LoadXml(Of VirtualCell)
    End Function

    <ExportAPI("zip")>
    Public Function WriteZipAssembly(vcell As VirtualCell, file As String) As Boolean
        Return ZipAssembly.WriteZip(vcell, file)
    End Function

    ''' <summary>
    ''' create dynamics kinetics
    ''' </summary>
    ''' <param name="expr"></param>
    ''' <param name="parameters"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("kinetics")>
    Public Function Kinetics(expr As String,
                             <RListObjectArgument>
                             parameters As list,
                             Optional env As Environment = Nothing) As Kinetics

        Dim args = evalArgumentValues(parameters, env)

        Return New Kinetics With {
            .formula = ScriptEngine.ParseExpression(expr),
            .parameters = args.Keys.ToArray,
            .paramVals = .parameters _
                .Select(Function(r) args(r)) _
                .ToArray,
            .target = expr
        }
    End Function

    Private Function evalArgumentValues(parameters As list, env As Environment) As Dictionary(Of String, Object)
        Return parameters.slots _
            .ToDictionary(Function(d) d.Key,
                          Function(d)
                              Dim v As Object = d.Value

                              If TypeOf v Is InvokeParameter Then
                                  v = REnv.single(DirectCast(v, InvokeParameter).Evaluate(env))
                              ElseIf TypeOf v Is RExpr Then
                                  v = REnv.single(DirectCast(v, RExpr).Evaluate(env))
                              Else
                                  v = REnv.single(v)
                              End If

                              Return v
                          End Function)
    End Function

    <ExportAPI("kinetics_lambda")>
    Public Function CompileLambda(kinetics As Kinetics) As DynamicInvoke
        Return kinetics.CompileLambda
    End Function

    <ExportAPI("eval_lambda")>
    Public Function eval(kinetics As DynamicInvoke,
                         <RListObjectArgument>
                         Optional args As list = Nothing,
                         Optional env As Environment = Nothing) As Double

        Dim argv = evalArgumentValues(args, env)
        Dim fetch As Func(Of String, Double) =
            Function(ref)
                Return CDbl(argv(ref))
            End Function

        Return kinetics(fetch)
    End Function
End Module
