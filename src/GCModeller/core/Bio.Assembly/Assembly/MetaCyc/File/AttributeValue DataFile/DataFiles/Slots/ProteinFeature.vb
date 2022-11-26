#Region "Microsoft.VisualBasic::ef97981992a36d887bc3a17d40902536, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\ProteinFeature.vb"

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

    '   Total Lines: 114
    '    Code Lines: 19
    ' Comment Lines: 82
    '   Blank Lines: 13
    '     File Size: 6.66 KB


    '     Class ProteinFeature
    ' 
    '         Properties: AttachedGroup, FeatureOf, LeftEndPosition, PossibleFeatureStates, ResidueNumber
    '                     RightEndPosition, Table
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    ''' <summary>
    ''' This class describes sites of interest (such as binding sites, modification sites, cleavage
    ''' sites) on a polypeptide. Instances of this class define a region of interest on a polypeptide,
    ''' plus, optionally, one or more states associated with the region. Different subclasses are used 
    ''' to specify single amino acid sites, linear regions, and regions involving noncontiguous 
    ''' segments of an amino-acid chain. For example, an instance F of this class could define an amino 
    ''' acid residue that can be phosphorylated, plus the fact that this residue can take on two 
    ''' possible states: PHOSPHORYLATED and UNPHOSPHORYLATED.
    ''' The feature instance itself does not describe the state of a particular protein. Instead, we 
    ''' would represent the phosphorylated and unphosphorylated forms of a protein by creating two 
    ''' instances of class Polypeptides. Both of those instances would link to the same feature F via 
    ''' the FEATURES slot. However, in the two proteins, F would be annotated differently to indicate 
    ''' the state of that feature. One protein would use an annotation label STATE with the value 
    ''' PHOSPHORYLATED to denote that the residue is phosphorylated, while the other would use the same 
    ''' annotation label STATE with the value UNPHOSPHORYLATED.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ProteinFeature : Inherits [Object]

        ''' <summary>
        ''' For a binding feature, this slot lists the entity that binds to the protein feature — it can be
        ''' either an instance of Chemicals or another Protein-Feature (e.g., in the case of crosslinks
        ''' forming between two sites on the same or different polypeptide).
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property AttachedGroup As String

        ''' <summary>
        ''' This slot points to the polypeptide frames with which this feature is associated (there
        ''' could be more than one such frame, if all are different forms of the same protein, e.g., a
        ''' modified and an unmodified form).
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>这个活性位点是属于哪一个蛋白质多肽链的</remarks>
        <MetaCycField()> Public Property FeatureOf As String

        ''' <summary>
        ''' For a feature that consists of a contiguous linear stretch of amino acids, this slot encodes
        ''' the residue number of the leftmost amino acid, with number 1 referring to the N-terminal
        ''' amino acid.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property LeftEndPosition As String

        ''' <summary>
        ''' For a given feature class, this slot describes the possible states available to instances of
        ''' the class. For example, a feature that represents a binding site can have either a bound or 
        ''' unbound state. The list of possible states is stored at the class level as values for this slot. 
        ''' 
        ''' A particular instance F of the class (a specific feature of a specific protein) can then be 
        ''' labeled with this state information using the STATE annotation when F appears in the FEATURES 
        ''' slot of the protein. For example, two forms of the same protein would link to the same feature 
        ''' F, but one form P1 would have the feature annotated label STATE and value BOUND, whereas the 
        ''' other form P2 would use the label STATE and value UNBOUND.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(type:=MetaCycField.Types.TStr)>
        Public Property PossibleFeatureStates As List(Of String)

        ''' <summary>
        ''' For a feature that consists of a single amino acid or some number of noncontiguous amino
        ''' acids, this slot contains the numeric index or indices of the amino acid residue or residues
        ''' that make up this site. Number 1 corresponds to the N-terminal amino acid.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(type:=MetaCycField.Types.TStr)>
        Public Property ResidueNumber As List(Of String)

        ''' <summary>
        ''' For a feature that consists of a contiguous linear stretch of amino acids, this slot encodes
        ''' the residue number of the rightmost amino acid, relative to the start of the protein.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property RightEndPosition As String

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.proteinfeatures
            End Get
        End Property

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As ProteinFeature
        '    Dim NewObj As ProteinFeature = New ProteinFeature

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of ProteinFeature) _
        '        (MetaCyc.File.AttributeValue.Object.Format(ProteinFeatures.AttributeList, e), NewObj)

        '    NewObj.ResidueNumber = StringQuery(NewObj.Object, "RESIDUE-NUMBER( \d+)?")
        '    If NewObj.Object.ContainsKey("FEATURE-OF") Then NewObj.FeatureOf = NewObj.Object("FEATURE-OF") Else NewObj.FeatureOf = String.Empty
        '    NewObj.PossibleFeatureStates = StringQuery(NewObj.Object, "POSSIBLE-FEATURE-STATES( \d+)?")
        '    If NewObj.Object.ContainsKey("LEFT-END-POSITION") Then NewObj.LeftEndPosition = NewObj.Object("LEFT-END-POSITION") Else NewObj.LeftEndPosition = String.Empty
        '    If NewObj.Object.ContainsKey("RIGHT-END-POSITION") Then NewObj.RightEndPosition = NewObj.Object("RIGHT-END-POSITION") Else NewObj.RightEndPosition = String.Empty
        '    If NewObj.Object.ContainsKey("ATTACHED-GROUP") Then NewObj.AttachedGroup = NewObj.Object("ATTACHED-GROUP") Else NewObj.AttachedGroup = String.Empty

        '    Return NewObj
        'End Operator
    End Class
End Namespace
