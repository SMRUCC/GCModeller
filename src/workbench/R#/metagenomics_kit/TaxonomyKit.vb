#Region "Microsoft.VisualBasic::ec855b7652d96cb353dce7c760aff34b, R#\metagenomics_kit\TaxonomyKit.vb"

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

    '   Total Lines: 339
    '    Code Lines: 248
    ' Comment Lines: 53
    '   Blank Lines: 38
    '     File Size: 14.48 KB


    ' Module TaxonomyKit
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: asOTUTable, buildTree, Consensus, filterLambda, Filters
    '               getOTUDataframe, InRange, Lineage, lineageTable, loadMothurTree
    '               LoadNcbiTaxonomyTree, ParseBIOMString, printTaxonomy, RangeFilter, readOTUTable
    '               TaxonomyBIOMString, TaxonomyRange, uniqueTaxonomy
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime
Imports Taxonomy = SMRUCC.genomics.Metagenomics.Taxonomy

''' <summary>
''' toolkit for process ncbi taxonomy tree data
''' </summary>
<Package("taxonomy_kit", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gcmodeller.org")>
Module TaxonomyKit

    Sub New()
        Internal.ConsolePrinter.AttachConsoleFormatter(Of Taxonomy)(AddressOf printTaxonomy)

        Internal.Object.Converts.addHandler(GetType(NcbiTaxonomyTree), AddressOf lineageTable)
        Internal.Object.Converts.addHandler(GetType(OTUTable()), AddressOf getOTUDataframe)
    End Sub

    Private Function getOTUDataframe(table As OTUTable(), args As list, env As Environment) As rdataframe
        Dim OTU_num As String() = table.Select(Function(r) r.ID).ToArray
        Dim taxonomy As String() = table.Select(Function(r) r.taxonomy.ToString).ToArray
        Dim sampleNames As String() = table.PropertyNames
        Dim matrix As New rdataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"OTU_num", OTU_num},
                {"taxonomy", taxonomy}
            }
        }

        For Each name As String In sampleNames
            matrix.columns(name) = table _
                .Select(Function(r) r(name)) _
                .ToArray
        Next

        Return matrix
    End Function

    Private Function lineageTable(x As Object, args As list, env As Environment) As rdataframe
        Dim tree As NcbiTaxonomyTree = DirectCast(x, NcbiTaxonomyTree)
        Dim taxonomy = tree.Taxonomy.Keys _
            .Select(Function(id)
                        Return tree _
                            .GetAscendantsWithRanksAndNames(id, True) _
                            .DoCall(Function(line)
                                        Return New Taxonomy(line) With {
                                            .ncbi_taxid = id
                                        }
                                    End Function)
                    End Function) _
            .ToArray
        Dim ncbi_taxid As Array = taxonomy.Select(Function(t) t.ncbi_taxid).ToArray
        Dim kingdom As Array = taxonomy.Select(Function(t) t.kingdom).ToArray
        Dim phylum As Array = taxonomy.Select(Function(t) t.phylum).ToArray
        Dim [class] As Array = taxonomy.Select(Function(t) t.class).ToArray
        Dim order As Array = taxonomy.Select(Function(t) t.order).ToArray
        Dim family As Array = taxonomy.Select(Function(t) t.family).ToArray
        Dim genus As Array = taxonomy.Select(Function(t) t.genus).ToArray
        Dim species As Array = taxonomy.Select(Function(t) t.species).ToArray

        Return New rdataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {NameOf(ncbi_taxid), ncbi_taxid},
                {NameOf(kingdom), kingdom},
                {NameOf(phylum), phylum},
                {NameOf([class]), [class]},
                {NameOf(order), order},
                {NameOf(family), family},
                {NameOf(genus), genus},
                {NameOf(species), species}
            }
        }
    End Function

    Private Function printTaxonomy(taxonomy As Taxonomy) As String
        Return $"<{taxonomy.lowestLevel}> {taxonomy.ToString(BIOMstyle:=True)}"
    End Function

    ''' <summary>
    ''' cast taxonomy object to biom style taxonomy string
    ''' </summary>
    ''' <param name="taxonomy"></param>
    ''' <param name="trim_genusName">
    ''' removes the genus name from the species name?
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("biom.string")>
    <RApiReturn(GetType(String))>
    Public Function TaxonomyBIOMString(<RRawVectorArgument>
                                       taxonomy As Object,
                                       Optional trim_genusName As Boolean = False,
                                       Optional env As Environment = Nothing) As Object

        Dim list = pipeline.TryCreatePipeline(Of Taxonomy)(taxonomy, env)

        If list.isError Then
            Return list.getError
        Else
            Return list.populates(Of Taxonomy)(env) _
                .Select(Function(tax)
                            Return tax.ToString(BIOMstyle:=True, trimGenus:=trim_genusName)
                        End Function) _
                .ToArray
        End If
    End Function

    <ExportAPI("biom_string.parse")>
    Public Function ParseBIOMString(<RRawVectorArgument> taxonomy As Object, Optional env As Environment = Nothing) As Object
        Dim strings As String() = REnv.asVector(Of String)(taxonomy)
        Dim taxlist As Taxonomy() = strings _
            .Select(AddressOf BIOMTaxonomyParser.Parse) _
            .ToArray

        Return taxlist
    End Function

    ''' <summary>
    ''' make taxonomy object unique
    ''' </summary>
    ''' <param name="taxonomy"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("unique_taxonomy")>
    Public Function uniqueTaxonomy(<RRawVectorArgument> taxonomy As Object, Optional env As Environment = Nothing) As Object
        Dim list = pipeline.TryCreatePipeline(Of Taxonomy)(taxonomy, env)

        If list.isError Then
            Return list.getError
        Else
            Return list.populates(Of Taxonomy)(env) _
                .GroupBy(Function(t) t.ToString()) _
                .Select(Function(sg) sg.First) _
                .ToArray
        End If
    End Function

    ''' <summary>
    ''' load ncbi taxonomy tree model from the given data files
    ''' </summary>
    ''' <param name="repo">a directory folder path which contains the NCBI taxonomy 
    ''' tree data files: ``nodes.dmp`` and ``names.dmp``.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Builds the following dictionnary from NCBI taxonomy ``nodes.dmp`` and ``names.dmp``
    ''' files 
    ''' 
    ''' ```json 
    ''' { Taxid namedtuple('Node', ['name', 'rank', 'parent', 'children']
    '''     } 
    ''' ``` 
    ''' + https://www.biostars.org/p/13452/ 
    ''' + https://pythonhosted.org/ete2/tutorial/tutorial_ncbitaxonomy.html
    ''' </remarks>
    <ExportAPI("Ncbi.taxonomy_tree")>
    Public Function LoadNcbiTaxonomyTree(repo As String) As NcbiTaxonomyTree
        Return New NcbiTaxonomyTree(repo)
    End Function

    <ExportAPI("taxonomy.filter")>
    <RApiReturn(GetType(Taxonomy), GetType(Predicate(Of Taxonomy)))>
    Public Function Filters(tree As NcbiTaxonomyTree, range As String(), Optional taxid As Integer() = Nothing) As Object
        Dim ranges As Taxonomy() = range _
            .Select(Function(id)
                        If id.IsPattern("\d+") Then
                            Return New Taxonomy(tree.GetAscendantsWithRanksAndNames(Integer.Parse(id), only_std_ranks:=True))
                        Else
                            Return New Taxonomy(BIOMTaxonomy.TaxonomyParser(id))
                        End If
                    End Function) _
            .Where(Function(t) t.lowestLevel <> TaxonomyRanks.NA) _
            .ToArray

        If taxid Is Nothing Then
            Return tree.filterLambda(ranges)
        Else
            Dim result As Taxonomy() = taxid _
                .Select(Function(id)
                            Return New Taxonomy(tree.GetAscendantsWithRanksAndNames(id, only_std_ranks:=True))
                        End Function) _
                .DoCall(AddressOf ranges.RangeFilter) _
                .ToArray

            Return result
        End If
    End Function

    <Extension>
    Friend Function filterLambda(tree As NcbiTaxonomyTree, ranges As Taxonomy()) As Predicate(Of Object)
        Return Function(target)
                   Dim relation As Relations

                   If target Is Nothing Then
                       Return False
                   ElseIf TypeOf target Is Taxonomy Then
                       ' do nothing 
                   ElseIf TypeOf target Is Long OrElse TypeOf target Is Integer Then
                       target = New Taxonomy(tree.GetAscendantsWithRanksAndNames(CInt(target), only_std_ranks:=True))
                   ElseIf TypeOf target Is String AndAlso DirectCast(target, String).IsPattern("\d+") Then
                       target = New Taxonomy(tree.GetAscendantsWithRanksAndNames(Integer.Parse(CStr(target)), only_std_ranks:=True))
                   Else
                       Return False
                   End If

                   For Each limits As Taxonomy In ranges
                       relation = limits.CompareWith(DirectCast(target, Taxonomy))

                       If relation = Relations.Equals OrElse relation = Relations.Include Then
                           Return True
                       End If
                   Next

                   Return False
               End Function
    End Function

    <Extension>
    Friend Iterator Function RangeFilter(Of T As Taxonomy)(ranges As Taxonomy(), targets As IEnumerable(Of T)) As IEnumerable(Of T)
        Dim filter = filterLambda(Nothing, ranges)

        For Each target As T In targets
            If filter(target) Then
                Yield target
            End If
        Next
    End Function

    ''' <summary>
    ''' get taxonomy lineage model from the ncbi taxonomy tree by given taxonomy id
    ''' </summary>
    ''' <param name="tree">the ncbi taxonomy tree model</param>
    ''' <param name="tax">the ncbi taxonomy id or taxonomy string in BIOM style.</param>
    ''' <param name="fullName"></param>
    ''' <returns></returns>
    <ExportAPI("lineage")>
    Public Function Lineage(tree As NcbiTaxonomyTree, <RRawVectorArgument> tax As String(), Optional fullName As Boolean = False) As Taxonomy()
        Return tax _
            .Select(Function(ncbi_taxid)
                        If ncbi_taxid.IsPattern("\d+") Then
                            Return New Taxonomy(tree.GetAscendantsWithRanksAndNames(Integer.Parse(ncbi_taxid), only_std_ranks:=Not fullName))
                        Else
                            Return New Taxonomy(BIOMTaxonomy.TaxonomyParser(ncbi_taxid))
                        End If
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' build taxonomy tree based on a given collection of taxonomy object.
    ''' </summary>
    ''' <param name="taxonomy"></param>
    ''' <returns></returns>
    <ExportAPI("as.taxonomy.tree")>
    Public Function buildTree(taxonomy As Taxonomy()) As TaxonomyTree
        Return TaxonomyTree.BuildTree(taxonomy.Select(Function(t) New gast.Taxonomy(t)), Nothing, Nothing)
    End Function

    <ExportAPI("consensus")>
    Public Function Consensus(tree As TaxonomyTree, rank As TaxonomyRanks) As Taxonomy()
        Return tree _
            .PopulateTaxonomy(rank) _
            .Select(Function(t) DirectCast(t, Taxonomy)) _
            .ToArray
    End Function

    ''' <summary>
    ''' Parse the result output from mothur command ``summary.tax``. 
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.mothurTree")>
    Public Function loadMothurTree(file As String) As MothurRankTree
        Return MothurRankTree.LoadTaxonomySummary(file)
    End Function

    <ExportAPI("as.OTU_table")>
    Public Function asOTUTable(tree As MothurRankTree) As OTUTable()
        Return tree.GetOTUTable
    End Function

    <ExportAPI("as.OTUtable")>
    Public Function asOTUTable(table As rdataframe,
                               Optional id As String = "OTU_num",
                               Optional taxonomy As String = "taxonomy",
                               Optional env As Environment = Nothing) As OTUTable()

        Dim unique_id As String() = REnv.asVector(Of String)(table.getColumnVector(id))
        Dim taxonomyStr As Taxonomy() = DirectCast(REnv.asVector(Of String)(table.getColumnVector(taxonomy)), String()) _
            .Select(Function(str) New Taxonomy(BIOMTaxonomy.TaxonomyParser(str))) _
            .ToArray

        Call table.columns.Remove(id)
        Call table.columns.Remove(taxonomy)

        Dim samples = table.forEachRow.ToArray
        Dim sample_ids As String() = table.colnames

        Return unique_id _
            .Select(Function(tax, i)
                        Return New OTUTable With {
                            .ID = tax,
                            .taxonomy = taxonomyStr(i),
                            .Properties = samples(i) _
                                .Zip(sample_ids) _
                                .ToDictionary(Function(a) a.Second,
                                              Function(a)
                                                  Return Val(a.First)
                                              End Function)
                        }
                    End Function) _
            .ToArray
    End Function

    <ExportAPI("read.OTUtable")>
    Public Function readOTUTable(file As String, Optional sumDuplicated As Boolean = True) As OTUTable()
        Dim otus As OTUTable() = file.LoadCsv(Of OTUTable)(mute:=True).ToArray

        If sumDuplicated Then
            Return otus _
                .GroupBy(Function(o) o.taxonomy.ToString) _
                .Select(Function(otu)
                            Dim allSampleName As String() = otu.PropertyNames
                            Dim v As Dictionary(Of String, Double) = allSampleName _
                                .ToDictionary(Function(name) name,
                                                Function(name)
                                                    Return Aggregate m As OTUTable
                                                            In otu
                                                            Into Sum(m(name))
                                                End Function)

                            Return New OTUTable With {
                                .ID = otu.Select(Function(m) m.ID).JoinBy("+"),
                                .taxonomy = otu.First.taxonomy,
                                .Properties = v
                            }
                        End Function) _
                .ToArray
        Else
            Return otus
        End If
    End Function

    <ExportAPI("taxonomy_range")>
    Public Function TaxonomyRange(tax As Taxonomy, rank As TaxonomyRanks) As Taxonomy
        Return tax.TaxonomyRankString(rank).DoCall(AddressOf BIOMTaxonomyParser.Parse)
    End Function

    <ROperator("in")>
    Public Function InRange(list As Taxonomy(), range As Taxonomy) As Boolean()
        Return list _
            .Select(Function(tax)
                        Dim compare As Relations = tax.CompareWith(another:=range)

                        Return compare = Relations.Equals OrElse
                               compare = Relations.IncludeBy
                    End Function) _
            .ToArray
    End Function
End Module
