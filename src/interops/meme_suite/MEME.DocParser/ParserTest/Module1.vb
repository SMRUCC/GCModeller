#Region "Microsoft.VisualBasic::fcc75a9e4cd93f629fa1bc437c11a5c6, meme_suite\MEME.DocParser\ParserTest\Module1.vb"

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

' Module Module1
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Interops.NBCR

Module Module1

    Sub Main()
        'Dim motifs = LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.MEME.Text.Load("F:\1.13.RegPrecise_network\MEME_OUT\DEGs.MEME\MMX-TO-NY.Down.fa.MEME_OUT\250bp.txt")
        'Call motifs.SaveAsXml("./test.xml")

        Dim motifs = MEME_Suite.ParsePWNFile("M:\project\20251010-wheat\20251105\LargePanicleDevelopment\blastp\TF\PlantTFDB_TF_binding_motifs_from_experiments").ToArray
        Dim list As New XmlList(Of MotifPWM)(motifs)

        Call list.GetXml.SaveTo("M:\project\20251010-wheat\20251105\LargePanicleDevelopment\blastp\TF\PlantTFDB_TF_binding_motifs_from_experiments.xml")
    End Sub
End Module
