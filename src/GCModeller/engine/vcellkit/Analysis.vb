#Region "Microsoft.VisualBasic::8e5e6e837f1c2839b5acc495b6a40411, vcellkit\Analysis.vb"

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

' Module Analysis
' 
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON

<Package("vcellkit.analysis")>
Public Module Analysis

    <ExportAPI("union.matrix")>
    Public Function UnionSnapshot(result$, Optional setName$ = "mass\metabolome.json") As DataSet()
        Dim metabolites As New List(Of DataSet)

        For Each dir As String In ls - l - lsDIR <= result
            Dim name = dir.BaseName
            Dim data = $"{dir}/{setName}".LoadJsonFile(Of Dictionary(Of String, Double))

            metabolites += New DataSet With {
                .ID = name,
                .Properties = data
            }
        Next

        Return metabolites.Transpose
    End Function
End Module

