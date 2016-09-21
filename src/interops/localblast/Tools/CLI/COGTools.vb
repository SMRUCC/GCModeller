#Region "Microsoft.VisualBasic::7ddc6a324ebaf043d9ebfbdad57dbc26, ..\interops\localblast\Tools\CLI\COGTools.vb"

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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.COG
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST

Partial Module CLI

    <ExportAPI("/COG.Statics",
               Usage:="/COG.Statics /in <myva_cogs.csv> [/locus <locus.txt/csv> /locuMap <Gene> /out <out.csv>]")>
    Public Function COGStatics(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim locus As String = args("/locus")
        Dim out As String
        If Not locus.FileExists Then
            out = args.GetValue("/out", inFile.TrimSuffix & ".COG.Stat.Csv")
        Else
            out = args.GetValue("/out", inFile.TrimSuffix & "." & IO.Path.GetFileNameWithoutExtension(locus) & ".COG.Stat.Csv")
        End If
        Dim myvaCogs = inFile.LoadCsv(Of MyvaCOG)

        If locus.FileExists Then
            Dim ext As String = IO.Path.GetExtension(locus)
            Dim locusTag As String()

            If String.Equals(ext, ".csv", StringComparison.OrdinalIgnoreCase) Then
                Dim temp = DocumentStream.EntityObject.LoadDataSet(locus, args.GetValue("/locusMap", "Gene"))
                locusTag = temp.ToArray(Function(x) x.Identifier)
            Else
                locusTag = locus.ReadAllLines
            End If

            myvaCogs = (From x As MyvaCOG In myvaCogs
                        Where Array.IndexOf(locusTag, x.QueryName) > -1
                        Select x).ToList
        End If

        Dim func As COG.Function = COG.Function.Default
        Dim stst = COGFunc.GetClass(myvaCogs, func)
        Return stst.SaveTo(out).CLICode
    End Function

    <ExportAPI("/EXPORT.COGs.from.DOOR",
               Usage:="/EXPORT.COGs.from.DOOR /in <DOOR.opr> [/out <out.csv>]")>
    Public Function ExportDOORCogs(args As CommandLine) As Integer
        Dim opr As String = args("/in")
        Dim out As String = args.GetValue("/out", opr.TrimSuffix & ".COGs.csv")
        Dim DOOR As DOOR = DOOR_API.Load(opr)

        Return (LinqAPI.MakeList(Of MyvaCOG) _
             <= From x As GeneBrief
                In DOOR.Genes
                Select New MyvaCOG With {
                    .COG = x.COG_number,
                    .MyvaCOG = x.COG_number,
                    .QueryName = x.Synonym,
                    .Description = x.Product,
                    .Category = x.COG_number
               }) >> OpenHandle(out)
    End Function

End Module
