#Region "Microsoft.VisualBasic::7f6c74aa76d15da3d3d3dec96025b855, ..\core\Bio.Assembly\Assembly\Expasy\API.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace Assembly.Expasy.AnnotationsTool

    Public Module API

        ''' <summary>
        ''' 从Expasy数据库之中创建基本的数据
        ''' </summary>
        ''' <param name="enzymes"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GenerateBasicDocument(enzymes As Expasy.Database.Enzyme()) As T_EnzymeClass_BLAST_OUT()
            Dim LQuery = LinqAPI.Exec(Of T_EnzymeClass_BLAST_OUT) <=
                From x As Expasy.Database.Enzyme
                In enzymes
                Select From id As String
                       In x.SwissProt
                       Select New T_EnzymeClass_BLAST_OUT With {
                           .uniprot = id,
                           .Class = x.Identification
                       }
            Return LQuery
        End Function

        Public Function EnzymeClassification(data As Generic.IEnumerable(Of T_EnzymeClass_BLAST_OUT)) As T_EnzymeClass_BLAST_OUT()
            Dim DefaultHandle As _____ENZYME_CLASS_HANDLER_ = AddressOf InternalDefaultHandler
            Dim ClassList = (From grouped_ClassesResult
                             In (From item In data Select item Group item By item.ProteinId Into Group).ToArray.AsParallel
                             Select __enzymeClassify(grouped_ClassesResult.Group.ToArray, DuplicatedHandler:=DefaultHandle)).ToArray
            Return ClassList
        End Function

        Public Function EnzymeClassification(data As Generic.IEnumerable(Of T_EnzymeClass_BLAST_OUT), Handle As _____ENZYME_CLASS_HANDLER_) As T_EnzymeClass_BLAST_OUT()
            Dim ClassList = (From grouped_ClassesResult
                             In (From item In data
                                 Select item
                                 Group item By item.ProteinId Into Group).ToArray.AsParallel
                             Select __enzymeClassify(grouped_ClassesResult.Group.ToArray, DuplicatedHandler:=Handle)).ToArray
            Return ClassList
        End Function

        ''' <summary>
        ''' 由于evalue已经是在做blast的时候已经通过evalue开关参数所限制了，都认为evalue符合要求，故而在这里以identities值为标准
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks>在上一层待用之中已经使用了查询并行化了，所以在本函数之中将不能够再使用并行化，以免影响程序效率</remarks>
        Private Function __enzymeClassify(data As T_EnzymeClass_BLAST_OUT(), DuplicatedHandler As _____ENZYME_CLASS_HANDLER_) As T_EnzymeClass_BLAST_OUT
            If data.Count = 1 Then
                Return data.First
            End If

            Dim ECList = (From item In data Select item.Class Distinct).ToArray
            If ECList.Count = 1 Then  '只有一种酶分类
                Dim MaxIdentitiesItem = (From item In data Select item Order By item.Identity Descending).First
                Return New T_EnzymeClass_BLAST_OUT With {
                    .ProteinId = data.First.ProteinId,
                    .Class = ECList.First,
                    .uniprot = MaxIdentitiesItem.uniprot,
                    .Identity = MaxIdentitiesItem.Identity,
                    .EValue = MaxIdentitiesItem.EValue
                }
            Else
                Return DuplicatedHandler(data)
            End If
        End Function

        Private Function InternalDefaultHandler(data As T_EnzymeClass_BLAST_OUT()) As T_EnzymeClass_BLAST_OUT
            Dim ECList = (From item In data Select item.Class Distinct).ToArray
            Dim Identities As Double() = New Double(ECList.Length - 1) {}
            Dim EValues As Double() = New Double(ECList.Length - 1) {}
            Dim n As Integer() = New Integer(ECList.Length - 1) {}

            For Each item In data
                Dim idx As Integer = Array.IndexOf(ECList, item.Class)

                Identities(idx) += item.Identity
                EValues(idx) += item.EValue
                n(idx) += 1
            Next

            For i As Integer = 0 To Identities.Length - 1
                EValues(i) /= n(i)
                Identities(i) /= n(i)
            Next

            Dim MaxIdentity As Integer = Array.IndexOf(Identities, Identities.Max)
            Dim DecidedEC = ECList(MaxIdentity)
            Dim DecidedClass As T_EnzymeClass_BLAST_OUT = (From item As T_EnzymeClass_BLAST_OUT
                                                           In data
                                                           Where String.Equals(item.Class, DecidedEC)
                                                           Select item
                                                           Order By item.Identity Descending).FirstOrDefault
            DecidedClass.Identity = Identities(MaxIdentity)
            DecidedClass.EValue = EValues(MaxIdentity)

            Return DecidedClass
        End Function

        ''' <summary>
        ''' Handler for process the duplicated enzyme classification data.
        ''' </summary>
        ''' <param name="DuplicatedData"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Delegate Function _____ENZYME_CLASS_HANDLER_(DuplicatedData As T_EnzymeClass_BLAST_OUT()) As T_EnzymeClass_BLAST_OUT

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Expasy"></param>
        ''' <param name="Aligned">经过筛选之后的之后比对结果</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InvokeAnnotations(Expasy As Expasy.Database.NomenclatureDB, Aligned As T_EnzymeClass_BLAST_OUT()) As EnzymeClass()
            Dim QueryProteins = (From hit As T_EnzymeClass_BLAST_OUT
                                 In Aligned
                                 Select hit
                                 Group hit By hit.ProteinId Into Group) _
                                      .ToDictionary(Function(item) item.ProteinId,
                                                    Function(item) item.Group.ToArray)  '先按照蛋白质的编号进行分组
            Dim LQuery As EnzymeClass() = (From item As KeyValuePair(Of String, T_EnzymeClass_BLAST_OUT())
                                           In QueryProteins
                                           Select New EnzymeClass With {
                                               .ProteinId = item.Key,
                                               .Hits = (From n In item.Value Select n.uniprot).ToArray,
                                               .EC_Class = (From n As T_EnzymeClass_BLAST_OUT
                                                            In item.Value
                                                            Select n.Class
                                                            Distinct).ToArray}).ToArray '得到初步的导出数据
            LQuery = (From item As EnzymeClass
                      In LQuery.AsParallel
                      Let annotations = (From ec As String In item.EC_Class
                                         Let ann As Expasy.Database.Enzyme = Expasy(ec)
                                         Select ann).ToArray
                      Select __generateAnnotations(item, annotations)).ToArray     '接着再匹配Expasy数据库之中的注释数据
            Return LQuery
        End Function

        Public Function InvokeKEGGAnnotations(dat As IEnumerable(Of EnzymeClass), KEGGReactions As IEnumerable(Of KEGG.DBGET.bGetObject.Reaction)) As EnzymeClass()
            Dim LQuery = (From x As EnzymeClass
                          In dat.AsParallel
                          Let KEGG_Annotationed As EnzymeClass = __getKEGGReaction(x, KEGGReactions)
                          Select KEGG_Annotationed).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 通过EC编号来查找KEGG Reaction数据库之中的相应的记录
        ''' </summary>
        ''' <param name="EC"></param>
        ''' <param name="KEGG"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __getKEGGReaction(EC As EnzymeClass, KEGG As Generic.IEnumerable(Of KEGG.DBGET.bGetObject.Reaction)) As EnzymeClass
            Dim LQuery = (From item In KEGG Where Array.IndexOf(EC.EC_Class, item.ECNum) > -1 Select item).ToArray
            EC.KEGG_ENTRIES = (From item In LQuery Select String.Format("[{0}] {1}", item.Entry, item.Equation)).ToArray
            Return EC
        End Function

        Private Function __generateAnnotations(raw As EnzymeClass, Annotations As Expasy.Database.Enzyme()) As EnzymeClass
            raw.ExpasyAnnotations = (From item In Annotations Select String.Format("[{0}] {1}", item.Identification, item.Description))
            raw.Catalysts = (From item As Database.Enzyme
                             In Annotations
                             Select (From nn As String
                                     In item.CatalyticActivity
                                     Select String.Format("[{0}] {1}", item.Identification, nn)).ToArray).ToVector
            Return raw
        End Function
    End Module
End Namespace
