#Region "Microsoft.VisualBasic::2bcec26f1f03e8923afd99469e13c1f9, ..\GCModeller\CLI_tools\MEME\Cli\Views + Stats\FamilyStatics.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
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

Imports MEME.Analysis
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting
Imports RDotNet.Extensions.Bioinformatics.VennDiagram.ModelAPI
Imports RDotNet.Extensions.VisualBasic
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("--family.statics",
               Usage:="--family.statics /sites <motifSites.csv> /mods <directory.kegg_modules>")>
    Public Function FamilyStatics(args As CommandLine) As Integer
        Dim input As String = args("/sites")
        Dim Sites = input.LoadCsv(Of MotifSite)
        Dim LQuery = (From site As MotifSite
                      In Sites.AsParallel
                      Let [mod] As String = site.Tag.Split("\"c)(2).Split("_"c).Last.ToUpper
                      Select Family =
                          If(String.IsNullOrEmpty(site.Family.Trim),
                          "Other",
                          site.Family.NormalizePathString(normAs:="")),
                          [mod]).ToArray

        Dim modDetails = FileIO.FileSystem.GetFiles(args("/mods"), FileIO.SearchOption.SearchAllSubDirectories, "*.xml") _
            .ToArray(Function(file) file.LoadXml(Of bGetObject.Module), Parallel:=True) _
            .ToDictionary(Function([mod]) [mod].EntryId.Split("_"c).Last.ToUpper)

        Dim modFamilies = (From site In LQuery
                           Select site
                           Group site By site.mod Into Group) _
                                .ToDictionary(Function([mod]) [mod].mod,
                                              Function(mm) mm.Group.ToArray(Function(dd) dd.Family).ToArray)
        Dim modBrites = BriteHEntry.Module.LoadFromResource.ToDictionary(Function([mod]) [mod].Entry.Key)

        Dim doc As New DocumentStream.File

        doc += {"Type", "Class", "Category", "Modules", "Families", "sites", "genes"}
        doc += modFamilies.ToArray(
            Function(mm) New String() {
                modBrites(mm.Key).Category,
                modBrites(mm.Key).Class,
                modBrites(mm.Key).SubCategory,
                mm.Key,
                (From name As String In mm.Value Select name Distinct Order By name Ascending).ToArray.JoinBy("; "),
                mm.Value.Length,
                modDetails(mm.Key).GetPathwayGenes.Length}.ToCsvRow)

        Call doc.Save(input.TrimSuffix & ".modFamilies.csv", LazySaved:=False)

        Dim AllFamilies As String() = (From site In LQuery Select site.Family Distinct).ToArray
        Dim AllTypes As String() = (From entry In modBrites Select entry.Value.Class Distinct).ToArray

        doc = New DocumentStream.File + "Type".Join(AllFamilies)

        Dim modTypeGroup = (From mm As KeyValuePair(Of String, String())
                            In modFamilies
                            Let entry = modBrites(mm.Key)
                            Select entry.Class, mm
                            Group By [Class] Into Group) _
                                .ToDictionary(Function(type) type.Class,
                                              Function(type) (From mmFam As String
                                                              In (From mmod
                                                                  In type.Group.ToArray
                                                                  Select mmod.mm.Value).MatrixAsIterator
                                                              Select mmFam
                                                              Group mmFam By mmFam Into Count) _
                                                                 .ToDictionary(Function(mmfam) mmfam.mmFam,
                                                                               Function(mmmdf) mmmdf.Count))
        For Each type As String In AllTypes
            Dim typeDescrib As Dictionary(Of String, Integer) = modTypeGroup.TryGetValue(type)
            If typeDescrib Is Nothing Then
                Call $"{type} is not exists in genome...".__DEBUG_ECHO
                Continue For
            End If

            ' type   fami1, fam2, fam3
            Dim row = type.Join(AllFamilies.ToArray(Function(fName) If(typeDescrib.ContainsKey(fName), CStr(typeDescrib(fName)), "0")))
            doc += row
        Next
        Call doc.Save(input.TrimSuffix & ".modFamilies.TypeStat.csv", LazySaved:=False)

        Dim FamilyMods = (From site In LQuery
                          Select site
                          Group site By site.Family Into Group) _
                               .ToDictionary(Function(ss) ss.Family,
                                             Function(ss) ss.Group.ToArray(Function(obj) obj.mod))
        doc = New DocumentStream.File + {"Family", "Modules"} +
            FamilyMods.ToArray(
                Function(fm) New String() {fm.Key, fm.Value.Distinct.JoinBy("; ")}.ToCsvRow)
        Call doc.Save(input.TrimSuffix & ".FamilyMods.csv", LazySaved:=False)

        Dim ffff = VectorMapper(FamilyMods.ToArray(Function(f) f.Value.Distinct.ToArray.As(Of IEnumerable(Of String))))
        Dim colors = RSystem.ColorMaps(ffff.Sequence)
        Dim serials = FamilyMods.ToArray(
            Function(fm, idx) New Partition With {
                .Name = fm.Key,
                .Vector = ffff(idx),
                .Color = colors(idx)})
        Dim venn As New VennDiagram With {
            .saveTiff = "./Families.venn.tiff",
            .partitions = serials,
            .Title = "FamilyStatics"
        }

        Call venn.SaveTo(input.TrimSuffix & ".Families.venn.R")

        ' 代谢途径 - 调控位点家族， 边属性为基因

        Return 0
    End Function
End Module
