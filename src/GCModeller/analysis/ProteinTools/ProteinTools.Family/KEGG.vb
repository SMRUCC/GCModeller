#Region "Microsoft.VisualBasic::f110b58bc232bd3e23cf870ff1d2e557, analysis\ProteinTools\ProteinTools.Family\KEGG.vb"

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

    ' Module KEGG
    ' 
    '     Function: FamilyDomains, ParsingFamilyDef
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.ProteinTools.Family.FileSystem
Imports SMRUCC.genomics.Assembly.KEGG.Archives
Imports SMRUCC.genomics.Data

<Package("KEGG.Prot.Family", Category:=APICategories.UtilityTools)>
Public Module KEGG

    ''' <summary>
    ''' 需要兼容KEGG和自己的Regprecise数据库_(:зゝ∠)_
    ''' </summary>
    ''' <param name="KEGG"></param>
    ''' <param name="Pfam"></param>
    ''' <returns></returns>
    <ExportAPI("FamilyDomain.Dumps", Info:="Dump the family database for the further analysis.")>
    Public Function FamilyDomains(KEGG As SequenceModel.FASTA.FastaFile,
                                  Pfam As IEnumerable(Of Xfam.Pfam.PfamString.PfamString)) As FamilyPfam
        Pfam = (From x As Xfam.Pfam.PfamString.PfamString
                In Pfam.AsParallel
                Where Not x.PfamString.IsNullOrEmpty
                Select x).AsList
        Dim dict As Dictionary(Of String, String) = KEGG.Select(Function(x) SequenceDump.TitleParser(x.Title)) _
            .ToDictionary(Function(x) x.Key,
                          Function(x) SequenceDump.KEGGFamily(x.Value))
        Dim seqDict As Dictionary(Of String, String) = KEGG.ToDictionary(Function(x) SequenceDump.TitleParser(x.Title).Key,
                                                                         Function(x) x.SequenceData)
        Dim LQuery = (From x In Pfam
                      Let family As String() = dict(x.ProteinId).Split("/"c)
                      Where Not family.IsNullOrEmpty
                      Select (From subX As String
                              In family
                              Select stringPfam = PfamString.CreateObject(x),'.InvokeSet(NameOf(PfamString.SequenceData), seqDict(x.ProteinId)),
                                  subX.ToLower,
                                  subX).ToArray).ToVector _
                     .GroupBy(Function(x) x.ToLower) _
                     .Select(Function(x) Family.FileSystem.Family.CreateObject(x.First.subX, x.Select(Function(xx) xx.stringPfam).ToArray)) _
                     .OrderBy(Function(x) x.Family).ToArray
        Dim FamilyDb As New FamilyPfam With {
            .Build = Now.ToString,
            .Family = (From x As FileSystem.Family
                       In LQuery
                       Where Not (String.IsNullOrEmpty(x.Family) OrElse
                           String.Equals(x.Family, "*"))
                       Select x).ToArray
        }
        Return FamilyDb
    End Function

    ''' <summary>
    ''' 测试用的函数
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Title.Parser", Info:="Parsing the family information from the annotation in the KEGG database.")>
    Public Function ParsingFamilyDef(title As String) As String
        Dim lfamily As String = SequenceDump.KEGGFamily(title)
        Return lfamily
    End Function
End Module
