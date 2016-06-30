Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNet.Extensions.VisualBasic
Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder.RTypes
Imports RDotNet.Extensions.VisualBasic.Services

Namespace VennDiagram

    ''' <summary>
    '''Creates a Venn diagram with four sets.
    ''' </summary>
    ''' <remarks>
    ''' The function defaults to placing the ellipses so that area1 corresponds to lower left,
    ''' area2 corresponds to lower right, area3 corresponds to middle left and area4 corresponds
    ''' to middle right. Refer to the example below to see how the 31 partial areas are ordered.
    ''' Arguments with length of 15 (label.col, cex, fontface, fontfamily) will follow the order
    ''' in the example.
    '''
    ''' Value
    '''
    ''' Returns an Object Of Class gList containing the grid objects that make up the diagram.
    ''' Also displays the diagram In a graphical device unless specified With ind = False.
    ''' Grid:grid.draw can be used to draw the gList object in a graphical device.
    ''' </remarks>
    <RFunc("draw.quad.venn")> Public Class drawQuadVenn : Inherits IRToken

        ''' <summary>
        ''' The size of the first set
        ''' </summary>
        ''' <returns></returns>
        Public Property area1 As RExpression
        ''' <summary>
        ''' The size of the second set
        ''' </summary>
        ''' <returns></returns>
        Public Property area2 As RExpression
        ''' <summary>
        ''' The size of the third set
        ''' </summary>
        ''' <returns></returns>
        Public Property area3 As RExpression
        ''' <summary>
        ''' The size of the fourth set
        ''' </summary>
        ''' <returns></returns>
        Public Property area4 As RExpression
        ''' <summary>
        ''' The size of the intersection between the first and the second set
        ''' </summary>
        ''' <returns></returns>
        Public Property n12 As RExpression
        ''' <summary>
        ''' The size of the intersection between the first and the third set
        ''' </summary>
        ''' <returns></returns>
        Public Property n13 As RExpression
        ''' <summary>
        ''' The size of the intersection between the first and the fourth set
        ''' </summary>
        ''' <returns></returns>
        Public Property n14 As RExpression
        ''' <summary>
        ''' The size of the intersection between the second and the third set
        ''' </summary>
        ''' <returns></returns>
        Public Property n23 As RExpression
        ''' <summary>
        ''' The size of the intersection between the second and the fourth set
        ''' </summary>
        ''' <returns></returns>
        Public Property n24 As RExpression
        ''' <summary>
        ''' The size of the intersection between the third and the fourth set
        ''' </summary>
        ''' <returns></returns>
        Public Property n34 As RExpression
        ''' <summary>
        ''' The size of the intersection between the first, second and third sets
        ''' </summary>
        ''' <returns></returns>
        Public Property n123 As RExpression
        ''' <summary>
        ''' The size of the intersection between the first, second and fourth sets
        ''' </summary>
        ''' <returns></returns>
        Public Property n124 As RExpression
        ''' <summary>
        ''' The size of the intersection between the first, third and fourth sets
        ''' </summary>
        ''' <returns></returns>
        Public Property n134 As RExpression
        ''' <summary>
        ''' The size of the intersection between the second, third and fourth sets
        ''' </summary>
        ''' <returns></returns>
        Public Property n234 As RExpression
        ''' <summary>
        ''' The size of the intersection between all four sets
        ''' </summary>
        ''' <returns></returns>
        Public Property n1234 As RExpression
        ''' <summary>
        ''' A vector (length 4) of strings giving the category names of the sets
        ''' </summary>
        ''' <returns></returns>
        Public Property category As RExpression = rep("", 4)
        ''' <summary>
        ''' A vector (length 4) of numbers giving the line width of the circles' circumferences
        ''' </summary>
        ''' <returns></returns>
        Public Property lwd As RExpression = rep(2, 4)
        ''' <summary>
        ''' A vector (length 4) giving the dash pattern of the circles' circumferences
        ''' </summary>
        ''' <returns></returns>
        Public Property lty As RExpression = rep("solid", 4)
        ''' <summary>
        ''' A vector (length 4) giving the colours of the circles' circumferences
        ''' </summary>
        ''' <returns></returns>
        Public Property col As RExpression = rep("black", 4)
        ''' <summary>
        ''' A vector (length 4) giving the colours of the circles' areas
        ''' </summary>
        ''' <returns></returns>
        Public Property fill As RExpression = NULL
        ''' <summary>
        ''' A vector (length 4) giving the alpha transparency of the circles' areas
        ''' </summary>
        ''' <returns></returns>
        Public Property alpha As RExpression = rep(0.5, 4)
        ''' <summary>
        ''' A vector (length 15) giving the colours of the areas' labels
        ''' </summary>
        ''' <returns></returns>
        <Parameter("label.col")> Public Property labelCol As RExpression = rep("black", 15)
        ''' <summary>
        ''' A vector (length 15) giving the size of the areas' labels
        ''' </summary>
        ''' <returns></returns>
        Public Property cex As RExpression = rep(1, 15)
        ''' <summary>
        ''' A vector (length 15) giving the fontface of the areas' labels
        ''' </summary>
        ''' <returns></returns>
        Public Property fontface As RExpression = rep("plain", 15)
        ''' <summary>
        ''' A vector (length 15) giving the fontfamily of the areas' labels
        ''' </summary>
        ''' <returns></returns>
        Public Property fontfamily As RExpression = rep("serif", 15)
        ''' <summary>
        ''' A vector (length 4) giving the positions (in degrees) of the category names along the circles,
        ''' with 0 (default) at 12 o'clock
        ''' </summary>
        ''' <returns></returns>
        <Parameter("cat.pos")> Public Property catPos = c(-15, 15, 0, 0)
        ''' <summary>
        ''' A vector (length 4) giving the distances (in npc units) of the category names from the edges
        ''' of the circles (can be negative)
        ''' </summary>
        ''' <returns></returns>
        <Parameter("cat.dist")> Public Property catDist As RExpression = c(0.22, 0.22, 0.11, 0.11)
        ''' <summary>
        ''' A vector (length 4) giving the size of the category names
        ''' </summary>
        ''' <returns></returns>
        <Parameter("cat.col")> Public Property catCol As RExpression = rep("black", 4)
        ''' <summary>
        ''' A vector (length 4) giving the colours of the category names
        ''' </summary>
        ''' <returns></returns>
        <Parameter("cat.cex")> Public Property catCex As RExpression = rep(1, 4)
        ''' <summary>
        ''' A vector (length 4) giving the fontface of the category names
        ''' </summary>
        ''' <returns></returns>
        <Parameter("cat.fontface")> Public Property catFontface As RExpression = rep("plain", 4)
        ''' <summary>
        ''' A vector (length 4) giving the fontfamily of the category names
        ''' </summary>
        ''' <returns></returns>
        <Parameter("cat.fontfamily")> Public Property catFontfamily = rep("serif", 4)
        ''' <summary>
        ''' List of 4 vectors of length 2 indicating horizontal and vertical justification of each category name
        ''' </summary>
        ''' <returns></returns>
        <Parameter("cat.just")> Public Property catJust As RExpression = rep(list(c(0.5, 0.5)), 4)
        ''' <summary>
        ''' Number of degrees to rotate the entire diagram
        ''' </summary>
        ''' <returns></returns>
        <Parameter("rotation.degree")> Public Property rotationDegree As RExpression = 0
        ''' <summary>
        ''' A vector (length 2) indicating (x,y) of the rotation centre
        ''' </summary>
        ''' <returns></returns>
        <Parameter("rotation.centre")> Public Property rotationCentre As RExpression = c(0.5, 0.5)
        ''' <summary>
        ''' Boolean indicating whether the function is to automatically draw the diagram before returning
        ''' the gList object or not
        ''' </summary>
        ''' <returns></returns>
        Public Property ind As Boolean = True
        ''' <summary>
        ''' A function or string used to rescale areas
        ''' </summary>
        ''' <returns></returns>
        <Parameter("cex.prop")> Public Property cexProp As RExpression = NULL
        ''' <summary>
        ''' Can be either 'raw' or 'percent'. This is the format that the numbers will be printed in.
        ''' Can pass in a vector with the second element being printed under the first
        ''' </summary>
        ''' <returns></returns>
        <Parameter("print.mode")> Public Property printMode As String = "raw"
        ''' <summary>
        ''' If one of the elements in print.mode is 'percent', then this is how many significant digits will be kept
        ''' </summary>
        ''' <returns></returns>
        Public Property sigdigs As Integer = 3
        ''' <summary>
        ''' If this is equal to true, then the vector passed into area.vector will be directly assigned
        ''' to the areas of the corresponding regions. Only use this if you know which positions in the
        ''' vector correspond to which regions in the diagram
        ''' </summary>
        ''' <returns></returns>
        <Parameter("direct.area")> Public Property directArea As Boolean = False
        ''' <summary>
        ''' An argument to be used when direct.area is true. These are the areas of the corresponding
        ''' regions in the Venn Diagram
        ''' </summary>
        ''' <returns></returns>
        <Parameter("area.vector")> Public Property areaVector As Integer = 0
    End Class
End Namespace