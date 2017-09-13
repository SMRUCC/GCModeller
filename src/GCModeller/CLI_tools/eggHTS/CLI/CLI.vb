#Region "Microsoft.VisualBasic::00bee44b0bd1829ef4b5c9c4a1768561, ..\CLI_tools\eggHTS\CLI\CLI.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
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

Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Analysis.Microarray.DEGDesigner

<CLI> Module CLI

    Sub New()
        Call Settings.Templates.WriteExcelTemplate(Of Designer)()

        Dim path As New Value(Of String)

        If Not (path = Settings.Session.Templates & "/" & Parameters.DefaultFileName).FileExists Then
            Call New Parameters With {
                .ForceDirectedArgs = ForceDirectedArgs.DefaultNew
            }.WriteProfile(path)
        End If

        If Not (path = Settings.Session.Templates & "/iTraq.sign.csv").FileExists Then
            Call {
                New iTraqSymbols
            }.SaveTo(path)
        End If
    End Sub
End Module
