#Region "Microsoft.VisualBasic::7653db0d5d4a62fe4ec6381253546445, engine\vcell\Analysis.vb"

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

    ' Module CLI
    ' 
    '     Function: DiffExpression, Union
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Serialization.JSON

Partial Module CLI

    <ExportAPI("/union")>
    <Usage("/union /in <json.dataset_folder> [/out <result.folder>]")>
    Public Function Union(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{in$.TrimDIR}.union/"

        Return vcellkit.Analysis _
            .UnionSnapshot([in]) _
            .SaveTo(out & "/mass/metabolome.csv") _
            .CLICode
    End Function

    <ExportAPI("/diff")>
    <Usage("/diff /normal <result.json> /exp <experiment.json> [/result <output_folder>]")>
    <Description("Different expression of ``exp vs normal``.")>
    Public Function DiffExpression(args As CommandLine) As Integer
        Dim normal$ = args("/normal")
        Dim exp$ = args("/exp")
        Dim out$ = args("/result") Or $"{normal.TrimSuffix}.vs.{exp.BaseName}/"
        Dim normalData = normal.ReadAllText.LoadJSON(Of Dictionary(Of String, Double))
        Dim expData = exp.ReadAllText.LoadJSON(Of Dictionary(Of String, Double))
        Dim diff = normalData.Keys _
            .Select(Function(id)
                        Return New NamedValue(Of Double) With {
                            .Name = id,
                            .Value = Math.Log(expData(id) / normalData(id), 2)
                        }
                    End Function) _
            .OrderBy(Function(a) a.Value) _
            .ToDictionary() _
            .FlatTable
        Dim threshold = Math.Log(1.5, 2)

        Call diff.GetJson.SaveTo($"{out}/all.json")

        Dim up = diff.Where(Function(a) Math.Abs(a.Value) >= threshold).ToDictionary
        Dim down = diff.Where(Function(a) Math.Abs(a.Value) <= threshold).ToDictionary

        Call up.GetJson.SaveTo($"{out}/up.json")
        Call down.GetJson.SaveTo($"{out}/down.json")
        Call up.Keys.SaveTo($"{out}/up.txt")
        Call down.Keys.SaveTo($"{out}/down.txt")

        Return 0
    End Function
End Module
