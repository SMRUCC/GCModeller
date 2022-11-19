#Region "Microsoft.VisualBasic::64a82377a2f881f01e1a7a89ba5816e8, GCModeller\engine\IO\GCMarkupLanguage\v2\Assembly\ZipWriter.vb"

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
    '    Code Lines: 31
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 2.28 KB


    '     Module ZipWriter
    ' 
    '         Function: WriteZip
    ' 
    '         Sub: Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Zip
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace v2

    Module ZipWriter

        Public Function WriteZip(vcell As VirtualCell, zip As String) As Boolean
            Dim folderTmp As String = TempFileSystem.GetAppSysTempFile("", App.PID, RandomASCIIString(8, skipSymbols:=True))

            Call vcell.taxonomy.GetJson.SaveTo($"{folderTmp}/{NameOf(VirtualCell.taxonomy)}.json")
            Call vcell.properties.GetJson.SaveTo($"{folderTmp}/{NameOf(VirtualCell.properties)}.json")

            Call vcell.genome.regulations.Save($"{folderTmp}/{NameOf(VirtualCell.genome)}/{NameOf(Genome.regulations)}.Xml")
            Call vcell.genome.replicons.Save($"{folderTmp}/{NameOf(VirtualCell.genome)}/{NameOf(Genome.replicons)}.Xml")

            Call vcell.metabolismStructure.compounds.Save($"{folderTmp}/{NameOf(VirtualCell.metabolismStructure)}/{NameOf(MetabolismStructure.compounds)}.Xml")
            Call vcell.metabolismStructure.enzymes.Save($"{folderTmp}/{NameOf(VirtualCell.metabolismStructure)}/{NameOf(MetabolismStructure.enzymes)}.Xml")
            Call vcell.metabolismStructure.maps.Save($"{folderTmp}/{NameOf(VirtualCell.metabolismStructure)}/{NameOf(MetabolismStructure.maps)}.Xml")

            Call vcell.metabolismStructure.reactions.enzymatic.Save($"{folderTmp}/{NameOf(VirtualCell.metabolismStructure)}/{NameOf(MetabolismStructure.reactions)}/{NameOf(ReactionGroup.enzymatic)}.Xml")
            Call vcell.metabolismStructure.reactions.etc.Save($"{folderTmp}/{NameOf(VirtualCell.metabolismStructure)}/{NameOf(MetabolismStructure.reactions)}/{NameOf(ReactionGroup.etc)}.Xml")

            Call ZipLib.DirectoryArchive(folderTmp, zip, ArchiveAction.Replace, Overwrite.Always, CompressionLevel.Fastest)

            Return True
        End Function

        <Extension>
        Private Sub Save(Of T)(components As IEnumerable(Of T), path As String)
            Call New ZipComponent(Of T) With {
                 .components = components.ToArray
            }.GetXml _
             .SaveTo(path)
        End Sub
    End Module
End Namespace
