﻿#Region "Microsoft.VisualBasic::68e9eeb92ee232a7f4870521b6ebae63, CLI_tools\eggHTS\CLI\CLI.vb"

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
'     Constructor: (+1 Overloads) Sub New
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Analysis.Microarray.DEGDesigner

<Package("eggHTS", Category:=APICategories.CLI_MAN, Publisher:="xie.guigang@gcmodeller.org",
         Revision:=25657,
         Description:="Custom KO classification set can be download from: http://www.kegg.jp/kegg-bin/get_htext?ko00001.keg. 
              You can replace the %s mark using kegg organism code in url example as: http://www.kegg.jp/kegg-bin/download_htext?htext=%s00001&format=htext&filedir= for download the custom KO classification set.")>
<CLI>
Module CLI

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
