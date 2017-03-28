Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ProteinModel

Namespace Assembly.Uniprot.XML

    Public Module Extensions

        <Extension>
        Public Function ECNumberList(protein As entry) As String()
            Return protein?. _
                protein?. _
                recommendedName?. _
                ecNumber.
                ToArray(Function(ec) ec.value)
        End Function

        <Extension>
        Public Function OrganismScientificName(protein As entry) As String
            If protein.organism Is Nothing Then
                Return ""
            Else
                Return protein.organism.scientificName
            End If
        End Function

        <Extension> Public Function proteinFullName(protein As entry) As String
            If protein.protein Is Nothing Then
                Return ""
            Else
                Return protein.protein.FullName
            End If
        End Function

        <Extension> Public Function ORF(protein As entry) As String
            If protein.gene Is Nothing OrElse Not protein.gene.HaveKey("ORF") Then
                Return Nothing
            Else
                Return protein.gene.ORF.First
            End If
        End Function

        ''' <summary>
        ''' 获取蛋白质在细胞内的亚细胞定位结果
        ''' </summary>
        ''' <param name="protein"></param>
        ''' <returns></returns>
        <Extension> Public Function SubCellularLocations(protein As entry) As String()
            Dim cellularComments = protein _
                .CommentList _
                .TryGetValue("subcellular location", [default]:={})
            Return cellularComments _
                .Select(Function(c) c.subcellularLocations.ToArray(Function(x) x.locations)) _
                .IteratesALL _
                .IteratesALL _
                .Distinct _
                .ToArray
        End Function

        ''' <summary>
        ''' 获取蛋白质的功能结构信息
        ''' </summary>
        ''' <param name="prot"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetDomainData(prot As entry) As DomainModel()
            Dim features As feature() = prot.features.Takes("domain")
            Dim out As DomainModel() = features.ToArray(
                Function(f) New DomainModel With {
                    .DomainId = f.description,
                    .Start = f.location.begin.position,
                    .End = f.location.end.position
                })
            Return out
        End Function

        ''' <summary>
        ''' 生成KEGG或者GO注释分类的mapping表
        ''' </summary>
        ''' <param name="uniprotXML"></param>
        ''' <param name="type$"></param>
        ''' <param name="idType"></param>
        ''' <returns>``term --> geneID``</returns>
        <Extension>
        Public Function Term2Gene(uniprotXML As UniprotXML, Optional type$ = "GO", Optional idType As IDTypes = IDTypes.Accession) As IDMap()
            Dim out As New List(Of IDMap)
            Dim getID As Func(Of entry, String) = idType.GetID

            For Each prot As entry In uniprotXML.entries
                Dim ID As String = getID(prot)

                If prot.Xrefs.ContainsKey(type) Then
                    out += From term As dbReference
                           In prot.Xrefs(type)
                           Select New IDMap With {
                               .Key = term.id,
                               .Maps = ID
                           }
                End If
            Next

            Return out
        End Function
    End Module
End Namespace