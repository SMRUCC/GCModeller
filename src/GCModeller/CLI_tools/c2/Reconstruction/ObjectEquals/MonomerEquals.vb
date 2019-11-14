#Region "Microsoft.VisualBasic::648eeb4e9d25c6120d9a15f04d2d7541, CLI_tools\c2\Reconstruction\ObjectEquals\MonomerEquals.vb"

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

    '     Class MonomerEquals
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetList, GetPfamIdList, Initialize
    '         Structure Protein
    ' 
    '             Properties: MolecularWeight, Pfam, UniqueId
    ' 
    '             Function: Cast, Equals, ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports c2.Reconstruction.Operation

Namespace Reconstruction.ObjectEquals

    ''' <summary>
    ''' 这个等价性判断对象是根据结构域来进行的，将模型中剩余的缺口进行补齐
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MonomerEquals : Inherits c2.Reconstruction.ObjectEquals.EqualsOperation

        Sub New(Session As OperationSession)
            Call MyBase.New(Session)
        End Sub

        ''' <summary>
        ''' 判断标准：DBLink属性中的Pfam结构域分布要一致，相对分子质量一致
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Initialize() As Integer
            Dim rctProteins = (From Protein As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein
                               In MyBase.Reconstructed.GetProteins.AsParallel
                               Where Protein.Types.IndexOf("Polypeptides") > -1
                               Select MonomerEquals.Protein.Cast(Protein)).ToArray   '类型转换
            Dim sbjProteins = (From Protein As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein
                               In MyBase.Subject.GetProteins.AsParallel
                               Where Protein.Types.IndexOf("Polypeptides") > -1
                               Select MonomerEquals.Protein.Cast(Protein)).ToArray
            '进行等价性匹配并生成EqualsList
            Dim LQuery = From Protein As MonomerEquals.Protein In sbjProteins
                         Let array = (From prot As MonomerEquals.Protein In rctProteins.AsParallel
                                      Where Protein.Equals(prot)
                                      Select prot.UniqueId).ToArray
                         Select New KeyValuePair(Of String, String())(Protein.UniqueId, value:=array) '
            MyBase.EqualsList = LQuery.ToArray

            Return EqualsList.Count
        End Function

        Public Function GetList() As OperationSession.HomologousProteinsF
            Dim LQuery = From item In MyBase.EqualsList Where Not item.Value.IsNullOrEmpty Select New KeyValuePair(Of String, String)(item.Key, item.Value.First) '
            Return New OperationSession.HomologousProteinsF With {.Proteins = LQuery.ToArray}
        End Function

        Private Structure Protein
            Public Property UniqueId As String
            Public Property Pfam As String()
            Public Property MolecularWeight As Double

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="obj"></param>
            ''' <returns></returns>
            ''' <remarks>在比较之前，Pfam列表已经进行了排序了</remarks>
            Public Overrides Function Equals(obj As Object) As Boolean
                Dim Protein As Protein = DirectCast(obj, Protein)
                If System.Math.Abs(Protein.MolecularWeight - Me.MolecularWeight) < 10 Then
                    If Not (Protein.Pfam.IsNullOrEmpty OrElse Me.Pfam.IsNullOrEmpty) AndAlso (Protein.Pfam.Count = Me.Pfam.Count) Then
                        For i As Integer = 0 To Pfam.Count - 1
                            If Not String.Equals(Pfam(i), Protein.Pfam(i)) Then
                                Return False
                            End If
                        Next
                        Return True
                    End If
                End If

                Return False
            End Function

            Public Shared Function Cast(Prot As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein) As Protein
                Dim Protein As Protein = New Protein With {
                    .UniqueId = Prot.Identifier,
                    .Pfam = GetPfamIdList(Prot),
                    .MolecularWeight = Val(Prot.MolecularWeightSeq)}
                Return Protein
            End Function

            Public Overrides Function ToString() As String
                Return UniqueId
            End Function
        End Structure

        ''' <summary>
        ''' Get the pfam domain id list in the DBLINKS property of a protein object.
        ''' </summary>
        ''' <param name="Protein"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetPfamIdList(Protein As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein) As String()
            Dim DBLinks = (From str As String In Protein.DBLinks Select LANS.SystemsBiology.Assembly.MetaCyc.Schema.DBLinkManager.DBLink.TryParse(str)).ToArray
            Dim LQuery = (From link In DBLinks Where String.Equals(link.DBName, "PFAM") Select link.AccessionId).ToList
            Call LQuery.Sort()
            Return LQuery.ToArray
        End Function
    End Class
End Namespace
