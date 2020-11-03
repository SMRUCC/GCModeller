#Region "Microsoft.VisualBasic::cb833a6a7ff1bde4f0cdc2030219f5ef, CLI_tools\metaProfiler\CLI\gast.vb"

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
    '     Function: ExportToMegan, StateNames
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Metagenomics

Partial Module CLI

    <ExportAPI("/gast.stat.names")>
    <Usage("/gast.stat.names /in <*.names> /gast <gast.out> [/out <out.Csv>]")>
    Public Function StateNames(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".stat.Csv")
        Dim gastOut As String = args("/gast")
        Dim result As Names() = ParseNames([in]).FillTaxonomy(gastOut).ToArray
        Call BIOM.Imports(result, 1000).ToJSON.SaveTo(out.TrimSuffix & ".Megan.biom")
        Call MeganImports.Out(result).Save(out.TrimSuffix & ".Megan.Csv", Encodings.ASCII)
        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Export.Megan.BIOM")>
    <Usage("/Export.Megan.BIOM /in <relative.table.csv> [/dense /out <out.biom.json>]")>
    <Description("Export v1.0 biom json file for data visualize in Megan program.")>
    <ArgumentAttribute("/in", False, AcceptTypes:={GetType(OTUData), GetType(DataSet)},
              Extensions:="*.csv",
              Description:="If the type of this input file is a dataset, then row ID should 
              be the taxonomy string, and all of the column should be the OTU abundance data.")>
    <ArgumentAttribute("/dense", True, CLITypes.Boolean,
              Description:="Dense matrxi type in biom json output file?")>
    Public Function ExportToMegan(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".biom.json")
        Dim data As OTUData()
        Dim type As Type = IO.TypeOf(File.ReadHeaderRow([in]), GetType(OTUData), GetType(DataSet))

        If type Is GetType(DataSet) Then
            data = DataSet.LoadDataSet([in]) _
                .Select(Function(d, i)
                            Return New OTUData With {
                                .OTU = $"OTU_{i.ToHexString}",
                                .Taxonomy = d.ID,
                                .Data = d.Properties.AsCharacter
                            }
                        End Function) _
                .ToArray
        Else
            data = [in].LoadCsv(Of OTUData)()
        End If

        Return data.EXPORT(denseMatrix:=args("/dense")) _
            .ToJSON _
            .SaveTo(out) _
            .CLICode
    End Function
End Module
