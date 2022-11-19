#Region "Microsoft.VisualBasic::1573bd7f812f66c2eda7b86e02740575, GCModeller\analysis\Metagenome\MetaFunction\CARD\CARDdata.vb"

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

    '   Total Lines: 136
    '    Code Lines: 80
    ' Comment Lines: 39
    '   Blank Lines: 17
    '     File Size: 4.98 KB


    ' Module CARDdata
    ' 
    '     Function: (+2 Overloads) AntibioticResistanceRelationship, FastaParser, TravelAntibioticResistance, TravelAntibioticTree
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models
Imports SMRUCC.genomics.foundation.OBO_Foundry.Tree
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' the Comprehensive Antibiotic Resistance Database
''' > https://card.mcmaster.ca/about
''' </summary>
Public Module CARDdata

    Public Iterator Function FastaParser(directory As String) As IEnumerable(Of SeqHeader)
        For Each fasta As FastaSeq In StreamIterator.SeqSource(handle:=directory, debug:=True)
            Dim headers$() = fasta.Headers

            If headers.Length = 4 Then
                ' protien
                Yield SeqHeader.ProteinHeader(headers)
            Else
                ' nucleotide
                Yield SeqHeader.NucleotideHeader(headers)
            End If
        Next
    End Function

    ''' <summary>
    ''' 函数返回``{aro_id, antibiotic_aro_id()}``
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function AntibioticResistanceRelationship(aro As IEnumerable(Of RawTerm)) As Dictionary(Of String, String())
        Return Builder.BuildTree(terms:=aro).AntibioticResistanceRelationship
    End Function

    ''' <summary>
    ''' 函数返回``{aro_id, antibiotic_aro_id()}``
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function AntibioticResistanceRelationship(tree As Dictionary(Of String, GenericTree)) As Dictionary(Of String, String())
        Dim relationships As New Dictionary(Of String, String())
        Dim antibiotic$()

        ' 自身具备有对抗生素的抗性或者他的parent具有对抗生素的抗性
        For Each term As GenericTree In tree.Values
            Dim resistance = term _
                .TravelAntibioticResistance _
                .ToArray

            If resistance.Length > 0 Then
                antibiotic = resistance _
                    .Select(Function(rel)
                                Return rel.drug
                            End Function) _
                    .Distinct _
                    .Where(Function(id) id.IsPattern("ARO[:]\d+", RegexICSng)) _
                    .ToArray

                If antibiotic.Length > 0 Then
                    relationships(term.ID) = antibiotic
                End If
            End If
        Next

        Return relationships
    End Function

    ''' <summary>
    ''' 获取当前的这个节点所有的对抗生素标记出抗性的父节点
    ''' </summary>
    ''' <param name="term"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function TravelAntibioticResistance(term As GenericTree) As IEnumerable(Of (drug$, term As GenericTree))
        Dim data As Dictionary(Of String, String()) = term.data

        ' 查看自身具备哪些抗性特征
        If data.ContainsKey(RawTerm.Key_relationship) Then
            Dim rel = data(RawTerm.Key_relationship) _
                .Where(Function(x) InStr(x, "confers_resistance_to") = 1) _
                .Select(Function(r)
                            Return r.Split("!"c) _
                                .First _
                                .GetTagValue(" ", trim:=True) _
                                .Value
                        End Function) _
                .ToArray

            If rel.Length > 0 Then
                ' 自身具备有抗性
                For Each drug In rel
                    Yield (drug, term)
                Next
            End If
        End If

        ' 这个递归不能够放在上面的If的Else分支之中
        ' 因为
        ' 除了自身具备有某一种抗性，可能其parent也会具备有其他的抗性
        For Each parent As GenericTree In term.is_a
            For Each rel In parent.TravelAntibioticResistance
                Yield rel
            Next
        Next
    End Function

    ''' <summary>
    ''' Antibiotics are commonly classified based on their mechanism of action, chemical structure or spectrum of activity.
    ''' 
    ''' ```
    ''' is_a: ARO:1000001 ! process or component of antibiotic biology or chemistry
    ''' ```
    ''' </summary>
    Const antibiotic_molecule$ = "ARO:1000003"

    ''' <summary>
    ''' ``ARO:1000003``为所有抗生素药物的root节点，所以在这函数之中查询出所有的归属于这个节点的
    ''' ARO term即可
    ''' </summary>
    ''' <param name="tree"></param>
    ''' <returns></returns>
    <Extension>
    Public Function TravelAntibioticTree(tree As Dictionary(Of String, GenericTree)) As Dictionary(Of String, GenericTree)
        Dim antibiotic_molecule As New Dictionary(Of String, GenericTree)

        For Each term In tree.Values
            If term.IsBaseType(CARDdata.antibiotic_molecule) Then
                antibiotic_molecule(term.ID) = term
            End If
        Next

        Return antibiotic_molecule
    End Function
End Module
