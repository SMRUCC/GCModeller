#Region "Microsoft.VisualBasic::2ea61b59dd2de1b73ca07fc0822e190a, CLI_tools\MEME\Cli\Views + Stats\FamilyStatics.vb"

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
    '     Function: FamilyStatics
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports RDotNet.Extensions.Bioinformatics.VennDiagram.ModelAPI
Imports RDotNet.Extensions.VisualBasic
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel

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
            .Select(Function(file) file.LoadXml(Of bGetObject.Module)) _
            .ToDictionary(Function([mod]) [mod].EntryId.Split("_"c).Last.ToUpper)

        Dim modFamilies = (From site In LQuery
                           Select site
                           Group site By site.mod Into Group) _
                                .ToDictionary(Function([mod]) [mod].mod,
                                              Function(mm) mm.Group.Select(Function(dd) dd.Family).ToArray)
        Dim modBrites = BriteHEntry.Module.LoadFromResource.ToDictionary(Function([mod]) [mod].Entry.Key)

        Dim doc As New IO.File

        doc += {"Type", "Class", "Category", "Modules", "Families", "sites", "genes"}
        doc += modFamilies.Select(
            Function(mm) New RowObject({
                modBrites(mm.Key).Category,
                modBrites(mm.Key).Class,
                modBrites(mm.Key).SubCategory,
                mm.Key,
                (From name As String In mm.Value Select name Distinct Order By name Ascending).ToArray.JoinBy("; "),
                mm.Value.Length,
                modDetails(mm.Key).GetPathwayGenes.Length}))

        Call doc.Save(input.TrimSuffix & ".modFamilies.csv")

        Dim AllFamilies As String() = (From site In LQuery Select site.Family Distinct).ToArray
        Dim AllTypes As String() = (From entry In modBrites Select entry.Value.Class Distinct).ToArray

        doc = New IO.File + "Type".Join(AllFamilies)

        Dim modTypeGroup = (From mm As KeyValuePair(Of String, String())
                            In modFamilies
                            Let entry = modBrites(mm.Key)
                            Select entry.Class, mm
                            Group By [Class] Into Group) _
                                .ToDictionary(Function(type) type.Class,
                                              Function(type) (From mmFam As String
                                                              In (From mmod
                                                                  In type.Group.ToArray
                                                                  Select mmod.mm.Value).IteratesALL
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
            Dim row = type.Join(AllFamilies.Select(Function(fName) If(typeDescrib.ContainsKey(fName), CStr(typeDescrib(fName)), "0")))
            doc += row
        Next
        Call doc.Save(input.TrimSuffix & ".modFamilies.TypeStat.csv")

        Dim FamilyMods = (From site In LQuery
                          Select site
                          Group site By site.Family Into Group) _
                               .ToDictionary(Function(ss) ss.Family,
                                             Function(ss) ss.Group.Select(Function(obj) obj.mod))
        doc = New IO.File + {"Family", "Modules"} +
            FamilyMods.Select(
                Function(fm) New RowObject({fm.Key, fm.Value.Distinct.JoinBy("; ")}))
        Call doc.Save(input.TrimSuffix & ".FamilyMods.csv")

        Dim ffff = VectorMapper(FamilyMods.Select(Function(f) TryCast(f.Value.Distinct.ToArray, IEnumerable(Of String))))
        Dim colors = RSystem.ColorMaps(ffff.Sequence)
        Dim serials = FamilyMods.Select(
            Function(fm, idx) New Partition With {
                .Name = fm.Key,
                .Vector = ffff(idx),
                .Color = colors(idx)}).ToArray
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
