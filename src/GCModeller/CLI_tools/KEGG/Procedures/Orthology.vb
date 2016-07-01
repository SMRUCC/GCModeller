#Region "Microsoft.VisualBasic::0db538b07cb0e7fa80a0d0c084c7352e, ..\GCModeller\CLI_tools\KEGG\Procedures\Orthology.vb"

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

Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic

Namespace Procedures

    Public Class Orthology : Inherits jp_kegg2

        ''' <summary>
        ''' Orhtology brief htext
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property BriefData As BriteHText = BriteHText.Load_ko00001
        Public ReadOnly Property PathwayHText As Pathway() = Pathway.LoadFromResource
        Public ReadOnly Property ModulesHText As [Module]() = [Module].LoadFromResource

        Public ReadOnly Property Entries As String()
            Get
                Return (From s As String
                        In BriefData.GetEntries
                        Where Not String.IsNullOrEmpty(s)
                        Select s
                        Distinct
                        Order By s Ascending).ToArray
            End Get
        End Property

        Sub New(uri As Oracle.LinuxCompatibility.MySQL.ConnectionUri)
            Call MyBase.New(uri)
        End Sub

        Public Sub Update(ort As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.Orthology)
            Dim transaction As New List(Of Oracle.LinuxCompatibility.MySQL.SQLTable)

            Call transaction.Add(__getDiseases(ort))
            Call transaction.Add(__getModules(ort))
            Call transaction.Add(__getOrthology(ort))
            Call transaction.Add(__getPathways(ort))
            Call transaction.Add(__getReferences(ort))
            Call transaction.Add(__getXRef(ort))
            Call transaction.Add(__getGenes(ort))

            Dim TransactSQL As String = String.Join(vbCrLf, transaction.ToArray(Function(t) t.GetReplaceSQL))
            Try
                Call KEGG.CommitTransaction(TransactSQL)
            Catch ex As Exception

            End Try

            If Not KEGG.GetErrMessage.IsNullOrEmpty Then
                Call TransactSQL.SaveTo(App.CurrentDirectory & "/" & ort.Entry & ".sql")
            End If
        End Sub

        ''' <summary>
        ''' 重新填上因为错误而丢失的数据
        ''' </summary>
        Public Sub FillMissing()
            Dim stores = KEGG.Query(Of LocalMySQL.orthology)("SELECT * FROM jp_kegg2.orthology;")
            Dim lstEntry As String() = stores.ToArray(Function(x) x.entry)

            For Each id As String In Entries
                If Array.IndexOf(lstEntry, id) > -1 Then Continue For

                Dim orthology = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.API.Query(id)
                Call Update(orthology)
            Next
        End Sub

        Private Function __getDiseases(ort As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.Orthology) As Oracle.LinuxCompatibility.MySQL.SQLTable()
            If ort.Disease.IsNullOrEmpty Then Return Nothing
            Dim data As New List(Of Oracle.LinuxCompatibility.MySQL.SQLTable)
            Dim diseases = (From entry In ort.Disease
                            Let disease = New LocalMySQL.disease With {
                                .entry_id = entry.Key,
                                .definition = entry.Value.Replace("'", "~")
                            }
                            Let od = New LocalMySQL.orthology_diseases With {
                                .entry_id = ort.Entry,
                                .disease = entry.Key
                            }
                            Select disease, od).ToArray
            For Each entry In diseases
                Call data.Add(entry.disease)
                Call data.Add(entry.od)
            Next

            Return data.ToArray
        End Function

        Private Function __getModules(ort As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.Orthology) As Oracle.LinuxCompatibility.MySQL.SQLTable()
            If ort.Module.IsNullOrEmpty Then Return Nothing
            Dim data As New List(Of Oracle.LinuxCompatibility.MySQL.SQLTable)
            Dim modules = (From entry In ort.Module
                           Let moduleData = New LocalMySQL.module With {
                               .entry = entry.Key,
                               .definition = entry.Value.Replace("'", "~")
                           }
                           Let om = New LocalMySQL.orthology_modules With {
                               .entry_id = ort.Entry,
                               .module = entry.Key
                           }
                           Select moduleData, om).ToArray
            For Each entry In modules
                Call data.Add(entry.moduleData)
                Call data.Add(entry.om)
            Next

            Return data.ToArray
        End Function

        Private Function __getPathways(ort As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.Orthology) As Oracle.LinuxCompatibility.MySQL.SQLTable()
            If ort.Pathway.IsNullOrEmpty Then Return Nothing
            Dim data As New List(Of Oracle.LinuxCompatibility.MySQL.SQLTable)
            Dim pathways = (From entry In ort.Pathway
                            Let pathwayData = New LocalMySQL.pathway With {
                                .entry_id = entry.Key,
                                .definition = entry.Value.Replace("'", "~")
                            }
                            Let op = New LocalMySQL.orthology_pathways With {
                                .entry_id = ort.Entry,
                                .pathway = entry.Key
                            }
                            Select pathwayData, op).ToArray
            For Each entry In pathways
                Call data.Add(entry.op)
                Call data.Add(entry.pathwayData)
            Next

            Return data.ToArray
        End Function

        Private Function __getGenes(ort As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.Orthology) As Oracle.LinuxCompatibility.MySQL.SQLTable()
            If ort.Genes.IsNullOrEmpty Then Return Nothing
            Dim genes = (From entry In ort.Genes
                         Let geneData = New LocalMySQL.gene With {
                             .locus_tag = entry.LocusId,
                             .definition = entry.Description.Replace("'", "~"),
                             .gene_name = entry.Description.Replace("'", "~"),
                             .kegg_sp = entry.SpeciesId,
                             .ec = ort.EC
                         }
                         Let og = New LocalMySQL.orthology_genes With {
                             .gene = entry.LocusId,
                             .ko = ort.Entry,
                             .name = entry.Description.Replace("'", "~"),
                             .sp_code = entry.SpeciesId,
                             .url = ""
                         }
                         Select geneData, og).ToArray
            Dim data As New List(Of Oracle.LinuxCompatibility.MySQL.SQLTable)

            For Each gene In genes
                Call data.Add(gene.geneData)
                Call data.Add(gene.og)
            Next

            Return data.ToArray
        End Function

        Private Function __getReferences(ort As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.Orthology) As Oracle.LinuxCompatibility.MySQL.SQLTable()
            If ort.References.IsNullOrEmpty Then Return Nothing

            Dim data As New List(Of Oracle.LinuxCompatibility.MySQL.SQLTable)
            Dim references = (From entry In ort.References
                              Let reference As LocalMySQL.reference =
                                  New LocalMySQL.reference With {
                                     .authors = String.Join(", ", entry.Authors).Replace("'", "~"),
                                     .journal = entry.Journal.Replace("'", "~"),
                                     .pmid = entry.PMID,
                                     .title = entry.Title.Replace("'", "~")
                                  }
                              Let [or] As LocalMySQL.orthology_references =
                                  New LocalMySQL.orthology_references With {
                                     .entry_id = ort.Entry,
                                     .pmid = entry.PMID}
                              Select reference, [or]).ToArray
            For Each entry In references
                Call data.Add(entry.or)
                Call data.Add(entry.reference)
            Next

            Return data.ToArray
        End Function

        Private Function __getOrthology(ort As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.Orthology) As Oracle.LinuxCompatibility.MySQL.SQLTable
            Dim Path = BriefData.GetHPath(ort.Entry)
            If Path Is Nothing Then
                Path = New SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.BriteHText() {}
            End If
            Path = Path.Skip(1).ToArray
            Dim orthology As New LocalMySQL.orthology With {
                .definition = ort.Definition.Replace("'", "~"),
                .entry = ort.Entry,
                .name = ort.Name,
                .disease = ort.Disease.GetLength,
                .genes = ort.Genes.GetLength,
                .modules = ort.Module.GetLength,
                .pathways = ort.Pathway.GetLength,
                .EC = ort.EC
            }

            If Path.Length > 0 Then
                orthology.brief_A = Path(0).ClassLabel.Replace("'", "~")
            End If
            If Path.Length > 1 Then
                orthology.brief_B = Path(1).ClassLabel.Replace("'", "~")
            End If
            If Path.Length > 2 Then
                orthology.brief_C = Path(2).ClassLabel.Replace("'", "~")
            End If
            If Path.Length > 3 Then
                orthology.brief_D = Path(3).ClassLabel.Replace("'", "~")
            End If
            If Path.Length > 4 Then
                orthology.brief_E = Path(4).ClassLabel.Replace("'", "~")
            End If

            Return orthology
        End Function

        ''' <summary>
        ''' 在这里更新Other DBs的数据
        ''' </summary>
        ''' <param name="ort"></param>
        Private Function __getXRef(ort As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.Orthology) As Oracle.LinuxCompatibility.MySQL.SQLTable()
            Dim xRefs = (From lnk In ort.xRefEntry
                         Select lnk
                         Group lnk By lnk.Key.ToUpper Into Group).ToDictionary(Function(obj) obj.ToUpper, elementSelector:=Function(obj) obj.Group.ToArray)
            Dim datas As New List(Of Oracle.LinuxCompatibility.MySQL.SQLTable)

            If xRefs.ContainsKey("COG") Then
                Dim source = xRefs("COG")
                Call datas.Add(source.ToArray(Function(cog) New LocalMySQL.xref_ko2cog With {.COG = cog.Value2, .ko = ort.Entry, .url = cog.Value1}))
            End If

            If xRefs.ContainsKey("GO") Then
                Dim source = xRefs("GO")
                Call datas.Add(source.ToArray(Function(go) New LocalMySQL.xref_ko2go With {.go = go.Value2, .ko = ort.Entry, .url = go.Value1}))
            End If

            If xRefs.ContainsKey("RN") Then
                Dim source = xRefs("RN")
                Call datas.Add(source.ToArray(Function(rn) New LocalMySQL.xref_ko2rn With {.rn = rn.Value2, .ko = ort.Entry, .url = rn.Value1}))
            End If

            Return datas.ToArray
        End Function

        Public Overrides Function ToString() As String
            Return KEGG.ToString
        End Function
    End Class
End Namespace
