#Region "Microsoft.VisualBasic::81ae166706be7cde2bd2f8c8263d5baf, analysis\Metagenome\MetaFunction\test\metaTraitsTest.vb"

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

'   Total Lines: 14
'    Code Lines: 10 (71.43%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 4 (28.57%)
'     File Size: 531 B


' Module metaTraitsTest
' 
'     Sub: readFiles
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString

Module metaTraitsTest

    Sub Main()
        Call trainingTest()
        Call predicttest()
    End Sub

    Sub readFiles()
        Dim annos = TraitAnnotation.ParseTable("C:\Users\Administrator\Downloads\ncbi_species_summary_no_predictions.tsv").ToArray
        Dim microbials = TraitAnnotation.CreateProfiles(annos).OrderByDescending(Function(a) a.traits.Length).ToArray

        Call microbials.First.GetJson.SaveTo("Z:/metaTrait.json")

        Pause()
    End Sub

    Sub trainingTest()
        Dim trainingSet = TraitAnnotation.ParseTable("C:\Users\Administrator\Downloads\gtdb_species_summary_filtered.tsv").JoinIterates(TraitAnnotation.ParseTable("C:\Users\Administrator\Downloads\ncbi_species_summary_filtered.tsv")).ToArray
        Dim pfams As Dictionary(Of String, PfamString()) = "C:\Users\Administrator\Downloads\pfam".ListDirectory.ToDictionary(Function(d) d.BaseName, Function(d) $"{d}/Pfam.csv".LoadCsv(Of PfamString)(mute:=True).ToArray)


    End Sub

    Sub predicttest()

    End Sub
End Module

