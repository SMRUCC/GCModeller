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
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace v2

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

            Call vcell.genome.regulations.Save(zip, $"/{NameOf(VirtualCell.genome)}/{NameOf(Genome.regulations)}.xml")
            Call vcell.genome.replicons.Save(zip, $"/{NameOf(VirtualCell.genome)}/{NameOf(Genome.replicons)}.xml")

            Call vcell.metabolismStructure.compounds.Save(zip, $"/{NameOf(VirtualCell.metabolismStructure)}/{NameOf(MetabolismStructure.compounds)}.xml")
            Call vcell.metabolismStructure.enzymes.Save(zip, $"/{NameOf(VirtualCell.metabolismStructure)}/{NameOf(MetabolismStructure.enzymes)}.xml")
            Call vcell.metabolismStructure.maps.Save(zip, $"/{NameOf(VirtualCell.metabolismStructure)}/{NameOf(MetabolismStructure.maps)}.xml")

            Call vcell.metabolismStructure.reactions.enzymatic.Save(zip, $"/{NameOf(VirtualCell.metabolismStructure)}/{NameOf(MetabolismStructure.reactions)}/{NameOf(ReactionGroup.enzymatic)}.xml")
            Call vcell.metabolismStructure.reactions.etc.Save(zip, $"/{NameOf(VirtualCell.metabolismStructure)}/{NameOf(MetabolismStructure.reactions)}/{NameOf(ReactionGroup.etc)}.xml")
        End Sub

        <Extension>
        Private Sub Save(Of T)(components As IEnumerable(Of T), zip As ZipStream, path As String)
            Dim str As String = New ZipComponent(Of T) With {
                 .components = components.ToArray
            }.GetXml

            Call zip.WriteText(str, path)
        End Sub
    End Module
End Namespace
