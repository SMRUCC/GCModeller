#Region "Microsoft.VisualBasic::9fb79a89b872b346fa6d6d7fcd527232, analysis\SequenceToolkit\SequenceTools\CLI\SNP\calcFst.vb"

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
    '     Function: calcFst
    ' 
    ' /********************************************************************************/

#End Region

'Imports Microsoft.VisualBasic.CommandLine
'Imports Microsoft.VisualBasic.CommandLine.Reflection
'Imports Microsoft.VisualBasic.Data.csv
'Imports RDotNET.Extensions.Bioinformatics
'Imports RDotNET.Extensions.VisualBasic
'Imports RDotNET.Extensions.VisualBasic.API.base
'Imports SMRUCC.genomics.Analysis.SequenceTools.SNP

'Partial Module CLI

'    <ExportAPI("/calcFst", Usage:="/calcFst /in <in.csv> [/out <out.Csv>]")>
'    Public Function calcFst(args As CommandLine) As Integer
'        Dim [in] As String = args("/in")
'        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".calcFst.Csv")
'        Dim pop = [in].LoadCsv(Of GenotypeDetails)
'        Dim df As IO.File = pop.ExpandLocis
'        Dim name As String = "myFreq"
'        Dim types As New Dictionary(Of String, Type) From {
'            {polysat.Genomes, GetType(Integer)}
'        }

'        For Each ns As String In df.First.Skip(2)
'            Call types.Add(ns, GetType(Double))  ' data.frame里面的这一列是频数，Double
'        Next

'        Call df.Columns.Skip(1).JoinColumns _
'               .PushAsDataFrame(name,
'                                types:=types,
'                                rowNames:=df.Columns.First.Skip(1))   ' 向R之中推送数据

'        require("polysat")

'        Dim result = polysat.calcFst(name, null, null)

'        Throw New NotImplementedException
'    End Function
'End Module
