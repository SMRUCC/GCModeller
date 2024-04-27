#Region "Microsoft.VisualBasic::37e942fb68bbbf27ce88c91c255761eb, G:/GCModeller/src/workbench/modules/Knowledge_base/ncbi_kb//MeSH/MeshTree/MeshCategory.vb"

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

    '   Total Lines: 115
    '    Code Lines: 21
    ' Comment Lines: 92
    '   Blank Lines: 2
    '     File Size: 4.99 KB


    '     Enum MeshCategory
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Namespace MeSH.Tree

    ''' <summary>
    ''' # MeSH Tree Structures
    '''
    ''' MeSH descriptors are organized in 16 categories: category A for anatomic 
    ''' terms, category B for organisms, C for diseases, D for drugs and chemicals, 
    ''' etc. Each category is further divided into subcategories. Within each 
    ''' subcategory, descriptors are arrayed hierarchically from most general 
    ''' to most specific in up to thirteen hierarchical levels. Because of the
    ''' branching structure of the hierarchies, these lists are sometimes referred 
    ''' to as "trees". Each MeSH descriptor appears in at least one place in the 
    ''' trees, and may appear in as many additional places as may be appropriate.
    ''' Those who use MeSH should find the most specific MeSH descriptor that 
    ''' is available to represent each concept of interest.
    ''' 
    ''' For example, articles concerning Streptococcus pneumoniae will be found 
    ''' under the descriptor Streptococcus Pneumoniae rather than the broader 
    ''' term Streptococcus, while an article referring to a new streptococcal 
    ''' bacterium which is not yet in the vocabulary will be listed directly under 
    ''' Streptococcus. Accordingly, the user may consult the trees to find additional
    ''' subject headings which are more specific than a given heading, and broader 
    ''' headings as well. For example, under Abnormalities, there are specific 
    ''' abnormalities:
    ''' 
    ''' ```
    ''' Congenital Abnormalities C16.131
    '''     Abnormalities, Drug Induced C16.131.042
    '''     Abnormalities, Multiple C16.131.077
    '''         22q11 Deletion Syndrome C16.131.077.019
    '''             DiGeorge Syndrome C16.131.077.019.500
    ''' ```
    ''' 
    ''' In the MeSH Browser, each descriptor is followed by the number that indicates
    ''' its tree location. It may also be followed by one or more additional numbers,
    ''' in smaller type, and truncated at the third level, indicating other tree
    ''' locations of the same term. The numbers serve only to locate the descriptors 
    ''' in each tree and to alphabetize those at a given tree level. They have no 
    ''' intrinsic significance; e.g., the fact that D12.776.641 and D12.644.641 both 
    ''' have the three digit group 641 does not imply any common characteristic. The
    ''' numbers are subject to change when new descriptors are added or the hierarchical 
    ''' arrangement is revised to reflect vocabulary changes.
    ''' </summary>
    ''' <remarks>
    ''' ABCDEFGHIJKLMNVZ
    ''' </remarks>
    Public Enum MeshCategory
        ''' <summary>
        ''' Anatomy [A] 
        ''' </summary>
        <Description("Anatomy")> A
        ''' <summary>
        ''' Organisms [B] 
        ''' </summary>
        <Description("Organisms")> B
        ''' <summary>
        ''' Diseases [C] 
        ''' </summary>
        <Description("Diseases")> C
        ''' <summary>
        ''' Chemicals and Drugs [D] 
        ''' </summary>
        <Description("Chemicals and Drugs")> D
        ''' <summary>
        ''' Analytical, Diagnostic and Therapeutic Techniques, and Equipment [E] 
        ''' </summary>
        <Description("Analytical, Diagnostic and Therapeutic Techniques, and Equipment")> E
        ''' <summary>
        ''' Psychiatry and Psychology [F] 
        ''' </summary>
        <Description("Psychiatry and Psychology")> F
        ''' <summary>
        ''' Phenomena and Processes [G] 
        ''' </summary>
        <Description("Phenomena and Processes")> G
        ''' <summary>
        ''' Disciplines and Occupations [H] 
        ''' </summary>
        <Description("Disciplines and Occupations")> H
        ''' <summary>
        ''' Anthropology, Education, Sociology, and Social Phenomena [I] 
        ''' </summary>
        <Description("Anthropology, Education, Sociology, and Social Phenomena")> I
        ''' <summary>
        ''' Technology, Industry, and Agriculture [J] 
        ''' </summary>
        <Description("Technology, Industry, and Agriculture")> J
        ''' <summary>
        ''' Humanities [K] 
        ''' </summary>
        <Description("Humanities")> K
        ''' <summary>
        ''' Information Science [L] 
        ''' </summary>
        <Description("Information Science")> L
        ''' <summary>
        ''' Named Groups [M] 
        ''' </summary>
        <Description("Named Groups")> M
        ''' <summary>
        ''' Health Care [N] 
        ''' </summary>
        <Description("Health Care")> N
        ''' <summary>
        ''' Publication Characteristics [V] 
        ''' </summary>
        <Description("Publication Characteristics")> V
        ''' <summary>
        ''' Geographicals [Z] 
        ''' </summary>
        <Description("Geographicals")> Z
    End Enum
End Namespace
