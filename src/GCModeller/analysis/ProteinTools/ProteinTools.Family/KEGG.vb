Imports SMRUCC.genomics.AnalysisTools.ProteinTools.Family.FileSystem
Imports SMRUCC.genomics.Assembly.KEGG.Archives
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic

<PackageNamespace("KEGG.Prot.Family", Category:=APICategories.UtilityTools)>
Public Module KEGG

    ''' <summary>
    ''' 需要兼容KEGG和自己的Regprecise数据库_(:зゝ∠)_
    ''' </summary>
    ''' <param name="KEGG"></param>
    ''' <param name="Pfam"></param>
    ''' <returns></returns>
    <ExportAPI("FamilyDomain.Dumps", Info:="Dump the family database for the further analysis.")>
    Public Function FamilyDomains(KEGG As SequenceModel.FASTA.FastaFile,
                                  Pfam As IEnumerable(Of Sanger.Pfam.PfamString.PfamString)) As FamilyPfam
        Pfam = (From x As Sanger.Pfam.PfamString.PfamString
                In Pfam.AsParallel
                Where Not StringHelpers.IsNullOrEmpty(x.PfamString)
                Select x).ToList
        Dim dict As Dictionary(Of String, String) = KEGG.ToArray(Function(x) SequenceDump.TitleParser(x.Title)) _
            .ToDictionary(Function(x) x.Key,
                          Function(x) SequenceDump.KEGGFamily(x.Value))
        Dim seqDict As Dictionary(Of String, String) = KEGG.ToDictionary(Function(x) SequenceDump.TitleParser(x.Title).Key,
                                                                         Function(x) x.SequenceData)
        Dim LQuery = (From x In Pfam
                      Let family As String() = dict(x.ProteinId).Split("/"c)
                      Where Not StringHelpers.IsNullOrEmpty(family)
                      Select (From subX As String
                              In family
                              Select stringPfam = PfamString.CreateObject(x),'.InvokeSet(NameOf(PfamString.SequenceData), seqDict(x.ProteinId)),
                                  subX.ToLower,
                                  subX).ToArray).MatrixToVector _
                     .GroupBy(Function(x) x.ToLower) _
                     .ToArray(Function(x) Family.FileSystem.Family.CreateObject(x.First.subX, x.ToArray(Function(xx) xx.stringPfam))) _
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
