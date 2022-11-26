#Region "Microsoft.VisualBasic::aa77369081913fd11e6dd28d155b56e8, GCModeller\core\Bio.Assembly\Assembly\DOOR\DOOR_API.vb"

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

    '   Total Lines: 168
    '    Code Lines: 110
    ' Comment Lines: 45
    '   Blank Lines: 13
    '     File Size: 8.40 KB


    '     Module DOOR_API
    ' 
    '         Function: CreateOperonView, GetOprFirst, Load, LoadDocument, PTT2DOOR
    '                   Reload, SaveFile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData

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
    <Package("Door.API", Category:=APICategories.UtilityTools, Description:="Door operon prediction data.")>
    Public Module DOOR_API

        ''' <summary>
        ''' 将NCBI之中的PTT蛋白表转换为DOOR文件
        ''' </summary>
        ''' <param name="PTT"></param>
        ''' <returns></returns>
        <Extension> Public Function PTT2DOOR(PTT As NCBI.GenBank.TabularFormat.PTT) As DOOR
            Dim array = PTT.GeneObjects.ToArray
            Dim LQuery As OperonGene() = LinqAPI.Exec(Of OperonGene) <=
 _
                From o
                In array.SeqIterator
                Let idx As Integer = o.i
                Let x = o.value
                Select New OperonGene With {
                    .COG_number = x.COG,
                    .GI = x.PID,
                    .Length = x.Length,
                    .Location = x.Location,
                    .OperonID = idx,
                    .Product = x.Product,
                    .Synonym = x.Synonym
                }

            Return New DOOR With {
                .Genes = LQuery
            }
        End Function

        ''' <summary>
        ''' Gets the first gene in the operon of the struct gene that inputs from the parameter.
        ''' </summary>
        ''' <param name="struct">操纵子里面的某一个结构基因成员的基因编号</param>
        ''' <returns></returns>
        Public Function GetOprFirst(struct As String, DOOR As DOOR) As OperonGene
            Dim gene As OperonGene = DOOR.GetGene(struct)
            If gene Is Nothing Then
                Return Nothing
            End If

            Dim operon As Operon = DOOR.DOOROperonView(gene.OperonID)
            Return operon.InitialX
        End Function

        ''' <summary>
        ''' 创建操纵子对象的集合视图: ``{OperonID, GeneId()}()``
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("OperonView.Create"), Extension>
        Public Function CreateOperonView(DOOR As DOOR) As OperonView
            Dim OperonIds As String() = LinqAPI.Exec(Of String) <=
 _
                From obj As OperonGene
                In DOOR.Genes
                Select obj.OperonID
                Distinct
                Order By OperonID Ascending

            Dim LQuery = LinqAPI.Exec(Of Operon) <=
 _
                From oprID As String
                In OperonIds.AsParallel
                Let genes = DOOR.[Select](oprID)
                Select New Operon(oprID, genes)

            Return New OperonView With {
                .Operons = LQuery,
                .DOOR = DOOR
            }
        End Function

        ''' <summary>
        ''' 读取DOOR数据库文件
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <ExportAPI("Doc.Load")> Public Function Load(path As String) As DOOR
            Dim lines As String() = File.ReadAllLines(path)
            Dim DOOR As DOOR = LoadDocument(lines, path)
            Return DOOR
        End Function

        ''' <summary>
        ''' 从文档之中的数据行之中加载数据
        ''' </summary>
        ''' <param name="lines"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Doc.Load")>
        Public Function LoadDocument(lines As String(), Optional path As String = Nothing) As DOOR
            Return DOOR_IO.Imports(lines, path)
        End Function

        ''' <summary>
        ''' 将操纵子模型数据保存为DOOR数据库格式
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        <ExportAPI("Doc.Save")>
        Public Function SaveFile(data As Operon(), Path As String) As Boolean
            Dim sBuilder As String = Text(data)
            Return sBuilder.SaveTo(Path, Encoding.ASCII)
        End Function

        <ExportAPI("Doc.Reload")>
        Public Function Reload(data As Operon()) As DOOR
            Dim Doc As String = Text(data).Replace(vbLf, "")
            Dim DOOR As DOOR = LoadDocument(Doc.Split(CChar(vbCr)))
            Return DOOR
        End Function
    End Module
End Namespace
