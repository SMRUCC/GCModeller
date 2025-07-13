﻿#Region "Microsoft.VisualBasic::ad25bcf3717390d3cced5dcdee8ad927, engine\vcellkit\Modeller\Modeller.vb"

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


' Code Statistics:

'   Total Lines: 159
'    Code Lines: 103 (64.78%)
' Comment Lines: 38 (23.90%)
'    - Xml Docs: 94.74%
' 
'   Blank Lines: 18 (11.32%)
'     File Size: 6.16 KB


' Module vcellModeller
' 
'     Function: applyKinetics, CompileLambda, eval, evalArgumentValues, Kinetics
'               LoadVirtualCell, readJSON, writeJSON, WriteZipAssembly
' 
'     Sub: createKineticsDbCache
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Data.SABIORK.docuRESTfulWeb
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
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
Public Module vcellModeller

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
    ''' <param name="path">
    ''' the model file extension could be:
    ''' 
    ''' xml - small virtual cell model in a xml file
    ''' zip - large virtual cell model file save as multiple components in a zip file
    ''' json - large virtual cell model file save as json stream file
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("read.vcell")>
    Public Function LoadVirtualCell(path As String) As VirtualCell
        Select Case path.ExtensionSuffix
            Case "zip" : Return ZipAssembly.CreateVirtualCellXml(path)
            Case "xml" : Return path.LoadXml(Of VirtualCell)
            Case "json" : Return vcellModeller.readJSON(path)

            Case Else
                Throw New InvalidProgramException($"Unknown file format with extension suffix name: {path.ExtensionSuffix}!")
        End Select
    End Function

    ''' <summary>
    ''' save the virtual cell model as a large json file
    ''' </summary>
    ''' <param name="vcell"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("write.json_model")>
    Public Function writeJSON(vcell As VirtualCell, file As String, Optional indent As Boolean = True) As Boolean
        Dim s As Stream = file.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
        Dim json As JsonElement = vcell.CreateJSONElement
        Call json.WriteJSON(s, New JSONSerializerOptions With {.indent = indent})
        Call s.Flush()
        Call s.Dispose()
        Return True
    End Function

    <ExportAPI("read.json_model")>
    Public Function readJSON(file As String) As VirtualCell
        Dim s As Stream = file.OpenReadonly
        Dim reader As New JsonParser(New StreamReader(s), tqdm:=True)
        Dim json As JsonObject = DirectCast(reader.OpenJSON, JsonObject)
        Dim model As VirtualCell = json.CreateObject(Of VirtualCell)
        Return model
    End Function

    ''' <summary>
    ''' save the virtual cell model as zip archive file
    ''' </summary>
    ''' <param name="vcell"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("write.zip")>
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
        Return kinetics.CompileLambda(New Dictionary(Of String, CentralDogma))
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
