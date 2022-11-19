#Region "Microsoft.VisualBasic::a08ce0b9af5e93a5ce164c79fee1bbc2, GCModeller\data\GO_gene-ontology\GeneOntology\DAG\Fields.vb"

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

    '   Total Lines: 47
    '    Code Lines: 36
    ' Comment Lines: 0
    '   Blank Lines: 11
    '     File Size: 1.41 KB


    '     Structure def
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Structure synonym
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language

Namespace DAG

    Public Structure def

        Public def$
        Public ref As NamedValue(Of String)()

        Sub New(value$)
            Dim refs$ = Regex.Match(value, "\s+\[.+?\]").Value

            def = Mid(value$, 1, value.Length - ref.Length)
            refs = refs.GetStackValue("[", "]")
            ref = LinqAPI.Exec(Of NamedValue(Of String)) <=
 _
                From t As String
                In refs.Split(","c)
                Select t.Trim.GetTagValue(":", trim:=True)
        End Sub

        Public Overrides Function ToString() As String
            Dim refs As String = ref.Select(Function(r) $"{r.Name}:{r.Value}").JoinBy(", ")
            Return $"def: ""{def}"" [{refs}]"
        End Function
    End Structure

    Public Structure synonym

        Dim name$, type$
        Dim synonym As NamedValue(Of String)

        Sub New(value$)
            Dim tokens$() = CommandLine.GetTokens(value$)

            name = tokens(Scan0)
            type = tokens(1)
            synonym = tokens(2).GetStackValue("[", "]").GetTagValue(":")
        End Sub

        Public Overrides Function ToString() As String
            Return $"synonym: ""{name}"" {type} [{synonym.Name}:{synonym.Value}]"
        End Function
    End Structure
End Namespace
