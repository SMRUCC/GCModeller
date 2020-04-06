Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Zip
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace v2

    Module ZipWriter

        Public Function WriteZip(vcell As VirtualCell, zip As String) As Boolean
            Dim folderTmp As String = App.GetAppSysTempFile("", App.PID, RandomASCIIString(8, skipSymbols:=True))

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