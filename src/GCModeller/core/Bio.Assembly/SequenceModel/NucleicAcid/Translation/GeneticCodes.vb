#Region "Microsoft.VisualBasic::95935a429a4cf5677a52816c9d0dc057, GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\Translation\GeneticCodes.vb"

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

    '   Total Lines: 83
    '    Code Lines: 23
    ' Comment Lines: 57
    '   Blank Lines: 3
    '     File Size: 2.90 KB


    '     Enum GeneticCodes
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace SequenceModel.NucleotideModels.Translation

    ''' <summary>
    ''' http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/index.cgi?chapter=tgencodes#SG25
    ''' </summary>
    Public Enum GeneticCodes As Integer

        Auto = 0

        ''' <summary>
        ''' 1. The Standard Code
        ''' </summary>
        StandardCode = 1
        ''' <summary>
        ''' 2. The Vertebrate Mitochondrial Code
        ''' </summary>
        VertebrateMitochondrialCode = 2
        ''' <summary>
        ''' 3. The Yeast Mitochondrial Code
        ''' </summary>
        YeastMitochondrialCode = 3
        ''' <summary>
        ''' 4. The Mold, Protozoan, and Coelenterate Mitochondrial Code and the Mycoplasma/Spiroplasma Code
        ''' </summary>
        MoldProtozoanAndCoelenterateMitochondrialCodeAndMycoplasmaSpiroplasmaCode = 4
        ''' <summary>
        ''' 5. The Invertebrate Mitochondrial Code
        ''' </summary>
        InvertebrateMitochondrialCode = 5
        ''' <summary>
        ''' 6. The Ciliate, Dasycladacean and Hexamita Nuclear Code
        ''' </summary>
        CiliateDasycladaceanAndHexamitaNuclearCode = 6
        ''' <summary>
        ''' 9. The Echinoderm and Flatworm Mitochondrial Code
        ''' </summary>
        EchinodermAndFlatwormMitochondrialCode = 9
        ''' <summary>
        ''' 10. The Euplotid Nuclear Code
        ''' </summary>
        EuplotidNuclearCode = 10
        ''' <summary>
        ''' 11. The Bacterial, Archaeal and Plant Plastid Code
        ''' </summary>
        BacterialArchaealAndPlantPlastidCode = 11
        ''' <summary>
        ''' 12. The Alternative Yeast Nuclear Code
        ''' </summary>
        AlternativeYeastNuclearCode = 12
        ''' <summary>
        ''' 13. The Ascidian Mitochondrial Code
        ''' </summary>
        AscidianMitochondrialCode = 13
        ''' <summary>
        ''' 14. The Alternative Flatworm Mitochondrial Code
        ''' </summary>
        AlternativeFlatwormMitochondrialCode = 14
        ''' <summary>
        ''' 16. Chlorophycean Mitochondrial Code
        ''' </summary>
        ChlorophyceanMitochondrialCode = 16
        ''' <summary>
        ''' 21. Trematode Mitochondrial Code
        ''' </summary>
        TrematodeMitochondrialCode = 21
        ''' <summary>
        ''' 22. Scenedesmus obliquus Mitochondrial Code
        ''' </summary>
        ScenedesmusObliquusMitochondrialCode = 22
        ''' <summary>
        ''' 23. Thraustochytrium Mitochondrial Code
        ''' </summary>
        ThraustochytriumMitochondrialCode = 23
        ''' <summary>
        ''' 24. Pterobranchia Mitochondrial Code
        ''' </summary>
        PterobranchiaMitochondrialCode = 24
        ''' <summary>
        ''' 25. Candidate Division SR1 and Gracilibacteria Code
        ''' </summary>
        CandidateDivisionSR1AndGracilibacteriaCode = 25
    End Enum
End Namespace
