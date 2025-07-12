#Region "Microsoft.VisualBasic::13b2e044f1db9b0357dc1caa4cd7d97f, engine\IO\GCMarkupLanguage\v2\Assembly\ZipWriter.vb"

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

'   Total Lines: 41
'    Code Lines: 31 (75.61%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 10 (24.39%)
'     File Size: 2.32 KB


'     Module ZipWriter
' 
'         Function: WriteZip
' 
'         Sub: Save
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Zip
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

''' <summary>
''' An internal module for save the virtual cell model in zip archive format
''' </summary>
Module ZipWriter

    Public Function WriteZip(vcell As VirtualCell, zip As String) As Boolean
        Using archive = New ZipStream(zip)
            Call vcell.writeZIP(zip:=archive)
        End Using

        Return True
    End Function

    <Extension>
    Private Sub writeZIP(vcell As VirtualCell, zip As ZipStream)
        Call zip.WriteText(vcell.taxonomy.GetJson, $"/{NameOf(VirtualCell.taxonomy)}.json")
        Call zip.WriteText(vcell.properties.GetJson, $"/{NameOf(VirtualCell.properties)}.json")

        Call vcell.genome.regulations.Save(zip, $"/{NameOf(VirtualCell.genome)}/{NameOf(Genome.regulations)}.jsonl")

        For Each replicon As replicon In vcell.genome.replicons.UniqueNames
            Dim dir As String = $"/{NameOf(VirtualCell.genome)}/{NameOf(Genome.replicons)}/{replicon.genomeName}/"
            Dim meta As New Dictionary(Of String, String) From {
                {NameOf(replicon.genomeName), replicon.genomeName},
                {NameOf(replicon.isPlasmid), replicon.isPlasmid}
            }

            Call zip.WriteText(meta.GetJson, $"{dir}/index.json")
            Call replicon.operons.Save(zip, $"{dir}/operons.jsonl")
            Call replicon.RNAs.Save(zip, $"{dir}/RNAs.jsonl")
        Next

        Dim root_dir As String = $"/{NameOf(VirtualCell.metabolismStructure)}"

        Call vcell.metabolismStructure.compounds.Save(zip, $"{root_dir}/{NameOf(MetabolismStructure.compounds)}.jsonl")
        Call vcell.metabolismStructure.enzymes.Save(zip, $"{root_dir}/{NameOf(MetabolismStructure.enzymes)}.jsonl")
        Call vcell.metabolismStructure.maps.Save(zip, $"{root_dir}/{NameOf(MetabolismStructure.maps)}.jsonl")

        Call vcell.metabolismStructure.reactions.enzymatic.Save(zip, $"{root_dir}/{NameOf(MetabolismStructure.reactions)}/{NameOf(ReactionGroup.enzymatic)}.jsonl")
        Call vcell.metabolismStructure.reactions.none_enzymatic.Save(zip, $"{root_dir}/{NameOf(MetabolismStructure.reactions)}/{NameOf(ReactionGroup.none_enzymatic)}.jsonl")
    End Sub

    Private Iterator Function jsonl(Of T)(list As IEnumerable(Of T)) As IEnumerable(Of String)
        For Each item As T In list.SafeQuery
            Yield item.GetJson
        Next
    End Function

    <Extension>
    Private Sub Save(Of T)(components As IEnumerable(Of T), zip As ZipStream, path As String)
        Call zip.WriteLines(jsonl(components), path)
    End Sub
End Module
