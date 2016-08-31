#Region "Microsoft.VisualBasic::b7e4cb9f2363651eb3fc332f9e26f9eb, ..\GCModeller\core\Bio.Assembly\Assembly\DOOR\DOOR_API.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.DOOR

    ''' <summary>
    ''' We present a database DOOR (Database for prOkaryotic OpeRons) containing computationally predicted operons of all the sequenced prokaryotic genomes. 
    ''' All the operons In DOOR are predicted Using our own prediction program, which was ranked To be the best among 14 operon prediction programs by a recent independent review. 
    ''' Currently, the DOOR database contains operons for 675 prokaryotic genomes, And supports a number of search capabilities to facilitate easy access And utilization of the information stored in it. 
    ''' 
    ''' + (1) Querying the database: the database provides a search capability For a user To find desired operons And associated information through multiple querying methods. 
    ''' + (2) Searching For similar operons: the database provides a search capability For a user To find operons that have similar composition And Structure To a query operon. 
    ''' + (3) Prediction Of cis-regulatory motifs: the database provides a capability For motif identification In the promoter regions Of a user-specified group Of possibly coregulated operons, Using motif-finding tools. 
    ''' + (4) Operons For RNA genes: the database includes operons For RNA genes. (5) OperonWiki: the database provides a wiki page (OperonWiki) To facilitate interactions between users And the developer Of the database. 
    ''' 
    ''' We believe that DOOR provides a useful resource To many biologists working On bacteria And archaea, which can be accessed at http://csbl1.bmb.uga.edu/OperonDB.
    ''' </summary>
    <Cite(Title:="DOOR: a database for prokaryotic operons",
          DOI:="10.1093/nar/gkn757",
          Authors:="Mao, F.
Dam, P.
Chou, J.
Olman, V.
Xu, Y.",
          Year:=2009, Issue:="Database issue", Journal:="Nucleic Acids Res",
          Abstract:="We present a database DOOR (Database for prOkaryotic OpeRons) containing computationally predicted operons of all the sequenced prokaryotic genomes. 
All the operons in DOOR are predicted using our own prediction program, which was ranked to be the best among 14 operon prediction programs by a recent independent review. 
Currently, the DOOR database contains operons for 675 prokaryotic genomes, and supports a number of search capabilities to facilitate easy access and utilization of the information stored in it. 
<p>(1) Querying the database: the database provides a search capability for a user to find desired operons and associated information through multiple querying methods. 
<p>(2) Searching for similar operons: the database provides a search capability for a user to find operons that have similar composition and structure to a query operon. 
<p>(3) Prediction of cis-regulatory motifs: the database provides a capability for motif identification in the promoter regions of a user-specified group of possibly coregulated operons, using motif-finding tools. 
<p>(4) Operons for RNA genes: the database includes operons for RNA genes. (5) OperonWiki: the database provides a wiki page (OperonWiki) to facilitate interactions between users and the developer of the database. 
<p>We believe that DOOR provides a useful resource to many biologists working on bacteria and archaea, which can be accessed at http://csbl1.bmb.uga.edu/OperonDB.",
          AuthorAddress:="Computational Systems Biology Laboratory, Department of Biochemistry and Molecular Biology, University of Georgia, Athens, GA 30602, USA.",
          Keywords:="*Databases, Genetic
*Genome, Archaeal
*Genome, Bacterial
Genomics
*Operon
Software",
          Volume:=37,
          Pages:="D459-63",
          ISSN:="1362-4962 (Electronic)
0305-1048 (Linking)",
          PubMed:=18988623,
          URL:="http://csbl1.bmb.uga.edu/OperonDB")>
    <PackageNamespace("Door.API", Category:=APICategories.UtilityTools, Description:="Door operon prediction data.")>
    Public Module DOOR_API

        Public Function PTT2DOOR(PTT As NCBI.GenBank.TabularFormat.PTT) As DOOR
            Dim array = PTT.GeneObjects.ToArray
            Dim LQuery As GeneBrief() = LinqAPI.Exec(Of GeneBrief) <=
 _
                From o
                In array.SeqIterator
                Let idx As Integer = o.i
                Let x = o.obj
                Select New GeneBrief With {
                    .COG_number = x.COG,
                    .GI = x.PID,
                    .Length = x.Length,
                    .Location = x.Location,
                    .OperonID = idx,
                    .Product = x.Product,
                    .Synonym = x.Synonym
                }
            Dim xD As New DOOR With {
                .Genes = LQuery
            }
            xD.DOOROperonView = xD.CreateOperonView
            Return xD
        End Function

        ''' <summary>
        ''' Gets the first gene in the operon of the struct gene that inputs from the parameter.
        ''' </summary>
        ''' <param name="struct">操纵子里面的某一个结构基因成员的基因编号</param>
        ''' <returns></returns>
        <ExportAPI("Get.OprFirst", Info:="Gets the first gene in the operon of the struct gene that inputs from the parameter.")>
        Public Function GetOprFirst(struct As String, door As DOOR) As GeneBrief
            Dim gene = door.GetGene(struct)
            If gene Is Nothing Then
                Return Nothing
            End If

            Dim operon = door.DOOROperonView(gene.OperonID)
            Return operon.InitialX
        End Function

        ''' <summary>
        ''' {OperonID, GeneId()}()
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("OperonView.Create"), Extension>
        Public Function CreateOperonView(Door As DOOR) As OperonView
            Dim OperonIds As String() = LinqAPI.Exec(Of String) <=
 _
                From obj As GeneBrief
                In Door.Genes
                Select obj.OperonID
                Distinct
                Order By OperonID Ascending

            Dim LQuery = LinqAPI.Exec(Of Operon) <=
 _
                From OperonId As String
                In OperonIds.AsParallel
                Let genes = Door.[Select](OperonId)
                Select New Operon(OperonId, genes)

            Return New OperonView With {
                .Operons = LQuery,
                .__doorOperon = Door
            }
        End Function

        <ExportAPI("Doc.Load")>
        Public Function Load(FilePath As String) As DOOR
            Dim s_Data As String() = IO.File.ReadAllLines(FilePath)
            Dim DOOR As DOOR = LoadDocument(s_Data, FilePath)
            Return DOOR
        End Function

        ''' <summary>
        ''' 从文档之中的数据行之中加载数据
        ''' </summary>
        ''' <param name="s_Data"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Doc.Load")>
        Public Function LoadDocument(s_Data As String(), Optional path As String = Nothing) As DOOR
            Return DOOR.DocParser(s_Data, path)
        End Function

        <ExportAPI("Doc.Save")>
        Public Function SaveFile(data As Operon(), Path As String) As Boolean
            Dim sBuilder As String = GenerateDocument(data)
            Return sBuilder.SaveTo(Path, Encoding.ASCII)
        End Function

        Const docTitle As String = "OperonID	GI	Synonym	Start	End	Strand	Length	COG_number	Product"

        <ExportAPI("Doc.Create")>
        <Extension>
        Public Function GenerateDocument(data As IEnumerable(Of Operon)) As String
            Dim LQuery As String() = LinqAPI.Exec(Of String) <=
 _
                From Operon As Operon
                In data
                Select From gene As GeneBrief
                       In Operon.Value
                       Let strand = If(gene.Location.Strand = Strands.Forward, "+", "-")
                       Let rowData = {
                           Operon.Key,
                           gene.GI,
                           gene.Synonym,
                           CStr(gene.Location.Left),
                           CStr(gene.Location.Right),
                           strand,
                           CStr(gene.Location.FragmentSize),
                           gene.COG_number,
                           gene.Product
                       }
                       Select String.Join(vbTab, rowData)

            Dim sb As New StringBuilder(1024)

            Call sb.AppendLine(docTitle)

            For Each Line In LQuery
                Call sb.AppendLine(Line)
            Next

            Dim value As String = sb.ToString
            Return value
        End Function

        <ExportAPI("Doc.Reload")>
        Public Function Reload(data As Operon()) As DOOR
            Dim Doc As String = GenerateDocument(data).Replace(vbLf, "")
            Dim DOOR As DOOR = LoadDocument(Doc.Split(CChar(vbCr)))
            Return DOOR
        End Function
    End Module
End Namespace
