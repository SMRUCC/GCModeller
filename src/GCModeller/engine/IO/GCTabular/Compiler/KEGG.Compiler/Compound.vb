#Region "Microsoft.VisualBasic::1813c683bfa838a768357cfa25464551, engine\IO\GCTabular\Compiler\KEGG.Compiler\Compound.vb"

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

    '     Module Compound
    ' 
    '         Function: Compile, GenerateObject, (+2 Overloads) NormalizeUniqueId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.GCModeller.Assembly

Namespace KEGG.Compiler

    Module Compound

        Public Function Compile(KEGGCompounds As Generic.IEnumerable(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound)) As List(Of FileStream.Metabolite)
            Dim Metabolites = (From Model In KEGGCompounds.AsParallel Select GenerateObject(Model)).AsList
            Dim Distinct As Dictionary(Of String, FileStream.Metabolite) = New Dictionary(Of String, FileStream.Metabolite)
            For Each item In Metabolites
                If Not Distinct.ContainsKey(item.Identifier) Then
                    Call Distinct.Add(item.Identifier, item)
                End If
            Next

            Return Distinct.Values.AsList
        End Function

        Public Function GenerateObject(KEGGCompound As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound) As FileStream.Metabolite
            'Dim Metabolite As FileStream.Metabolite = New FileStream.Metabolite With {
            '    .KEGGCompound = KEGGCompound.Entry,
            '    .Identifier = NormalizeUniqueId(KEGGCompound),
            '    .InitialAmount = 10,
            '    .CommonNames = KEGGCompound.CommonNames,
            '    .MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.Compound,
            '    .MolWeight = KEGGCompound.MolWeight,
            '    .Formula = KEGGCompound.Formula,
            '    .ChEBI = KEGGCompound.CHEBI,
            '    .PUBCHEM = KEGGCompound.PUBCHEM}

            'Return Metabolite
        End Function

        Public Function NormalizeUniqueId(Compound As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound) As String
            If Compound.CommonNames.IsNullOrEmpty Then
                If String.IsNullOrEmpty(Compound.Formula) Then
                    Return NormalizeUniqueId(Compound.Entry, Compound.Entry)
                Else
                    Return NormalizeUniqueId(Compound.Formula, Compound.Entry)
                End If
            Else
                Return NormalizeUniqueId((From strId As String In Compound.CommonNames Select strId Order By Len(strId) Ascending).First, Compound.Entry)
            End If
        End Function

        Public Function NormalizeUniqueId(preparedId As String, EntryId As String) As String
            If Regex.Match(preparedId, "[CG]\d{5}").Success Then
                Return "CPD-" & Mid(preparedId, 2)
            End If

            Dim IdBuilder As StringBuilder = New StringBuilder(preparedId.ToUpper)
            Call IdBuilder.Replace(" ", "-")
            Call IdBuilder.Replace(",", "-")
            Call IdBuilder.Replace("(", "[")
            Call IdBuilder.Replace(")", "]-")
            Call IdBuilder.Replace("_", "-")
            Call IdBuilder.Replace("'", "")
            Call IdBuilder.Append(String.Format("-{0}", Regex.Match(EntryId, "\d+")))
            Call IdBuilder.Replace("--", "")

            Return IdBuilder.ToString
        End Function
    End Module
End Namespace
